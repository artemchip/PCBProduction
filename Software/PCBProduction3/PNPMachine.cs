using GerberVS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    public class PNPMachine : CNCGeneralMachine
    {
        public class BEComponent
        {
            public String designator = "-";
            public int angle = -1000;
            public String value = "";
            public float bXMm = 0.0f;
            public float bYMm = 0.0f;
        }
        public class PEComponent
        {
            public int id = 0;
            public String designator = "-";
            public int angle = -1000;
            public String value = "";
            public float pXMm = 0.0f;
            public float pYMm = 0.0f;
            public int placeStatus = 0;
        }

        // Gerber
        public LibGerberVS gerberVS = new LibGerberVS();

        // Component List
        public List<PEComponent> topComponentList = new List<PEComponent>();
        public List<PEComponent> bottomComponentList = new List<PEComponent>();
        public List<PointF[]> boardContoursTop = new List<PointF[]>();
        public List<PointF[]> boardContoursBottom = new List<PointF[]>();
        private bool pnpPending = false;
        private Vector2 dCorner1Mm = new Vector2(0.0f, 0.0f);
        private Vector2 dCorner2Mm = new Vector2(0.0f, 0.0f);
        private Vector2 dCorner3Mm = new Vector2(0.0f, 0.0f);
        private Vector2 dCorner4Mm = new Vector2(0.0f, 0.0f);

        public PNPMachine(String gerberFilesDir, String configFilePath, int xCamerasNeeded, int xPreviewBoxWidth, int xPreviewBoxHeight, EventHandler xStateChanged, EventHandler<ErrorEventArgs> xErrorOccurred, EventHandler<NewCameraPreviewImageArgs> xNewCameraPreviewImage, EventHandler<UserInputRequiredArgs> xUserInputRequired) : base(configFilePath, xCamerasNeeded, xPreviewBoxWidth, xPreviewBoxHeight, xStateChanged, xErrorOccurred, xNewCameraPreviewImage, xUserInputRequired)
        {
            // Константа
            calibZMm = safeHeightMm;
            GerberFolderParser.LoadLayers(gerberFilesDir);
            LoadComponentInfoFromGerber();
        }

        public void LoadComponentInfoFromGerber()
        {
            try
            {
                // Очистка
                topComponentList.Clear();
                bottomComponentList.Clear();

                // Файлы Gerber
                if (GerberFolderParser.gbrProfileFile.Length <= 0)
                {
                    throw new Exception("Не найдены файлы board outline.");
                }
                if (GerberFolderParser.mntFile.Length <= 0 && GerberFolderParser.mnbFile.Length <= 0)
                {
                    throw new Exception("Не найдены файлы .mnt и .mnb (centroid).");
                }

                // Размер и сколько плат в панели
                GerberProject proj = gerberVS.CreateNewProject();
                gerberVS.OpenLayerFromFileName(proj, GerberFolderParser.gbrProfileFile);
                RectangleF bounds = Utils.FindBoundsOfProfileImage(proj.FileInfo[0].Image);
                float xOffsetMm = (proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (bounds.Left * 25.4f) : bounds.Left;
                float yOffsetMm = (proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (bounds.Top * 25.4f) : bounds.Top;
                boardContoursTop.Clear();
                boardContoursBottom.Clear();

                // Stack images
                SizeF workImagePhysicalSizeInMm = new SizeF(MainForm.workImagePhysWidthMm, MainForm.workImagePhysHeightMm);
                SizeF simagePhysicalSizeInMm = new SizeF((float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (bounds.Width * 25.4f) : bounds.Width), (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (bounds.Height * 25.4f) : bounds.Height));
                int possibleCols = (int)Math.Floor((double)workImagePhysicalSizeInMm.Width / (double)simagePhysicalSizeInMm.Width);
                int possibleRows = (int)Math.Floor((double)workImagePhysicalSizeInMm.Height / (double)simagePhysicalSizeInMm.Height);
                if (possibleCols < 1 || possibleRows < 1)
                {
                    throw new Exception("PCB too large.");
                }

                // Файлы поверхностного монтажа
                List<BEComponent> topBECompRaw = new List<BEComponent>();
                List<BEComponent> botBECompRaw = new List<BEComponent>();
                if (GerberFolderParser.mntFile.Length > 0)
                {
                    String[] topStrings = File.ReadAllLines(GerberFolderParser.mntFile);
                    foreach (String ts in topStrings)
                    {
                        topBECompRaw.Add(ParseCentroidFileLine(ts));
                    }
                }
                topBECompRaw.Sort(delegate (BEComponent b, BEComponent a) { return GetSortOrderForComponentType(a).CompareTo(GetSortOrderForComponentType(b)); });
                List<IGrouping<String, BEComponent>> groupingTop = topBECompRaw.GroupBy((e) => GetGroupFactorFor(e)).ToList();
                List<BEComponent> topBEComp = new List<BEComponent>();
                foreach (IGrouping<String, BEComponent> gt in groupingTop)
                {
                    foreach (BEComponent cmpGroupedTop in gt)
                    {
                        topBEComp.Add(cmpGroupedTop);
                    }
                }
                if (GerberFolderParser.mnbFile.Length > 0)
                {
                    String[] bottomStrings = File.ReadAllLines(GerberFolderParser.mnbFile);
                    foreach (String bs in bottomStrings)
                    {
                        botBECompRaw.Add(ParseCentroidFileLine(bs));
                    }
                }
                botBECompRaw.Sort(delegate (BEComponent b, BEComponent a) { return GetSortOrderForComponentType(a).CompareTo(GetSortOrderForComponentType(b)); });
                List<IGrouping<String, BEComponent>> groupingBot = botBECompRaw.GroupBy((e) => GetGroupFactorFor(e)).ToList();
                List<BEComponent> botBEComp = new List<BEComponent>();
                foreach (IGrouping<String, BEComponent> gb in groupingBot)
                {
                    foreach (BEComponent cmpGroupedBot in gb)
                    {
                        botBEComp.Add(cmpGroupedBot);
                    }
                }
                float panelWidthMm = (((MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f) + MainForm.workImagePhysWidthMm);
                int cidx = 0;
                for (int ix = 0; ix < possibleCols; ix++)
                {
                    for (int iy = 0; iy < possibleRows; iy++)
                    {
                        foreach (BEComponent bec in topBEComp)
                        {
                            PEComponent pec = new PEComponent();
                            pec.id = cidx;
                            pec.designator = bec.designator + "(" + ix.ToString() + "," + iy.ToString() + ")";
                            pec.value = bec.value;
                            float bXMmProc = bec.bXMm - xOffsetMm;
                            float bYMmProc = simagePhysicalSizeInMm.Height - (bec.bYMm - yOffsetMm);
                            pec.pXMm = MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + MainForm.gapBetweenPCBsMm + bXMmProc + ((simagePhysicalSizeInMm.Width + (MainForm.gapBetweenPCBsMm * 2.0f)) * ix);
                            pec.pYMm = MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + MainForm.gapBetweenPCBsMm + bYMmProc + ((simagePhysicalSizeInMm.Height + (MainForm.gapBetweenPCBsMm * 2.0f)) * iy);
                            pec.angle = bec.angle;
                            pec.placeStatus = 0;
                            if (pec.pXMm > (workImagePhysicalSizeInMm.Width + MainForm.spaceOuterMm + MainForm.holderHolesIndentMm))
                            {
                                continue;
                            }
                            if (pec.pYMm > (workImagePhysicalSizeInMm.Height + MainForm.spaceOuterMm + MainForm.holderHolesIndentMm))
                            {
                                continue;
                            }
                            topComponentList.Add(pec);
                            cidx++;
                        }
                        foreach (BEComponent bec in botBEComp)
                        {
                            PEComponent pec = new PEComponent();
                            pec.id = cidx;
                            pec.designator = bec.designator + "(" + ix.ToString() + "," + iy.ToString() + ")";
                            pec.value = bec.value;
                            float bXMmProc = bec.bXMm - xOffsetMm;
                            float bYMmProc = simagePhysicalSizeInMm.Height - (bec.bYMm - yOffsetMm);
                            float pecPXMm = MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + MainForm.gapBetweenPCBsMm + bXMmProc + ((simagePhysicalSizeInMm.Width + (MainForm.gapBetweenPCBsMm * 2.0f)) * ix);
                            pec.pXMm = panelWidthMm - pecPXMm;
                            pec.pYMm = MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + MainForm.gapBetweenPCBsMm + bYMmProc + ((simagePhysicalSizeInMm.Height + (MainForm.gapBetweenPCBsMm * 2.0f)) * iy);
                            pec.angle = bec.angle;
                            pec.placeStatus = 0;
                            if (pec.pXMm > (workImagePhysicalSizeInMm.Width + MainForm.spaceOuterMm + MainForm.holderHolesIndentMm))
                            {
                                continue;
                            }
                            if (pec.pYMm > (workImagePhysicalSizeInMm.Height + MainForm.spaceOuterMm + MainForm.holderHolesIndentMm))
                            {
                                continue;
                            }
                            bottomComponentList.Add(pec);
                            cidx++;
                        }
                        RectangleF bRect = new RectangleF(
                            MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + MainForm.gapBetweenPCBsMm + ((simagePhysicalSizeInMm.Width + (MainForm.gapBetweenPCBsMm * 2.0f)) * ix),
                            MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + MainForm.gapBetweenPCBsMm + ((simagePhysicalSizeInMm.Height + (MainForm.gapBetweenPCBsMm * 2.0f)) * iy),
                            simagePhysicalSizeInMm.Width,
                            simagePhysicalSizeInMm.Height
                        );
                        boardContoursTop.Add(new PointF[5] { new PointF(bRect.Left, bRect.Top), new PointF(bRect.Right, bRect.Top), new PointF(bRect.Right, bRect.Bottom), new PointF(bRect.Left, bRect.Bottom), new PointF(bRect.Left, bRect.Top) });
                        boardContoursBottom.Add(new PointF[5] { new PointF(panelWidthMm - bRect.Left, bRect.Top), new PointF(panelWidthMm - bRect.Right, bRect.Top), new PointF(panelWidthMm - bRect.Right, bRect.Bottom), new PointF(panelWidthMm - bRect.Left, bRect.Bottom), new PointF(panelWidthMm - bRect.Left, bRect.Top) });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("У вас ошибка в Gerber-файлах: " + ex.Message);
            }
        }

        public BEComponent ParseCentroidFileLine(String str)
        {
            String newStr = "";
            bool prevIsSpace = false;
            foreach (char c in str)
            {
                if (c == '"')
                {
                    prevIsSpace = false;
                }
                else if (c == ' ')
                {
                    if (!prevIsSpace)
                    {
                        newStr += ' ';
                        prevIsSpace = true;
                    }
                }
                else if (c == ';')
                {
                    if (!prevIsSpace)
                    {
                        newStr += ' ';
                        prevIsSpace = true;
                    }
                }
                else
                {
                    newStr += c;
                    prevIsSpace = false;
                }
            }
            String[] parts = newStr.Split(' ');
            BEComponent ec = new BEComponent();
            ec.designator = "";
            int cnt = 0;
            int fieldsFilled = 0;
            foreach (String p in parts)
            {
                String cn = p.Replace("mm", "");
                if (cn.Length >= 2 && cn.Length <= 10 && ec.designator.Length <= 0)
                {
                    if (char.IsLetter(cn.First()) && char.IsDigit(cn.Last()))
                    {
                        ec.designator = cn;
                        fieldsFilled++;
                    }
                }
                if (fieldsFilled < 3 || cnt < 2)
                {
                    try
                    {
                        float cnum = (float)Utils.StrToDouble(cn);
                        if (cnt == 0)
                        {
                            ec.bXMm = cnum;
                            fieldsFilled++;
                        }
                        else if (cnt == 1)
                        {
                            ec.bYMm = cnum;
                            fieldsFilled++;
                        }
                        cnt++;
                    }
                    catch
                    {
                        continue;
                    }
                }
                else if (ec.angle == -1000 && IsAngleString(cn))
                {
                    ec.angle = (int)Utils.StrToDouble(cn);
                    fieldsFilled++;
                }
                else if (ec.value.Length <= 0)
                {
                    ec.value = cn;
                    fieldsFilled++;
                }
                else if (fieldsFilled >= 5)
                {
                    break;
                }
            }
            if (fieldsFilled >= 3)
            {
                return ec;
            }
            else
            {
                throw new Exception("Не удалось распарсить строку: " + str);
            }
        }

        public bool IsAngleString(String inStr)
        {
            String cleanStr = inStr.Replace(".", "").Replace(",", "").Replace(";", "").Replace(":", "").Replace(" ", "");
            return (((int)Utils.StrToDouble(cleanStr)) % 15) == 0;
        }

        public int GetSortOrderForComponentType(BEComponent cmp)
        {
            int f1 = 0;
            if (cmp.designator.StartsWith("R"))
            {
                f1 = 1;
            }
            else if (cmp.designator.StartsWith("C"))
            {
                f1 = 2;
            }
            else if (cmp.designator.StartsWith("D"))
            {
                f1 = 3;
            }
            else if (cmp.designator.StartsWith("L"))
            {
                f1 = 4;
            }
            else if (cmp.designator.StartsWith("Q"))
            {
                f1 = 5;
            }
            else
            {
                f1 = 6;
            }
            return f1;
        }

        public String GetGroupFactorFor(BEComponent cmp)
        {
            String dsg = "!";
            if (cmp.designator.Length > 0)
            {
                dsg = cmp.designator.Substring(0, 1);
            }
            String val = cmp.value.ToUpper().Replace("?", "").Replace("?", "").Replace("?", "").Replace("?", "");
            return dsg + "-" + val;
        }

        public void StartPNPProcess(int componentId, float lyingDeg, bool demoPlaced)
        {
            if (!IsConnected() || IsFault())
            {
                throw new Exception("Нет связи со станком или он в состоянии Alarm.");
            }
            if (IsBusyWithLongTask())
            {
                throw new Exception("Станок занят другим процессом или активен шпиндель, действие недоступно.");
            }
            shouldCancelOp = false;
            workThread = new Thread(new ThreadStart(delegate
            {
                DoPNPProcess(componentId, lyingDeg, demoPlaced);
            }));
            workThread?.Start();
        }

        private void DoPNPProcess(int componentId, float lyingDeg, bool demoPlaced)
        {
            PEComponent? pec = null;
            int pecIdx = -1;
            bool topSide = false;
            try
            {
                // Reset
                shouldCancelOp = false;
                pnpPending = true;
                SetBusyWithLongTask(true);
                StateChanged.Invoke(this, new EventArgs());

                // Компонент
                try
                {
                    pec = topComponentList.Where((e) => e.id == componentId).First();
                    pecIdx = topComponentList.IndexOf(pec);
                    topSide = true;
                }
                catch { }
                if (pec == null)
                {
                    try
                    {
                        pec = bottomComponentList.Where((e) => e.id == componentId).First();
                        pecIdx = bottomComponentList.IndexOf(pec);
                        topSide = false;
                    }
                    catch { }
                }
                

                // Process
                if (componentId < 0)
                {
                    // Проверка калибровки
                    if (GetCamToToolDistX() == 0.0f || GetCamToToolDistY() == 0.0f)
                    {
                        throw new Exception("Смещение камера-инструмент не откалибровано.");
                    }

                    // Получить TL угол
                    int res = 0;
                    float panelAnchorsWMm = MainForm.workImagePhysWidthMm + (MainForm.holderHolesIndentMm * 2f);
                    float panelAnchorsHMm = MainForm.workImagePhysHeightMm + (MainForm.holderHolesIndentMm * 2f);
                    Vector2 tlDefaultCamLoc = new Vector2(((maxTravelMmX - panelAnchorsWMm) / 2.0f) + GetCamToToolDistX(), ((maxTravelMmY - panelAnchorsHMm) / 2.0f) + GetCamToToolDistY());
                    res = AskUserForInput("Убедитесь что панель лежит стороной TOP или BOTTOM кверху и её левый верхний угол соответствует таковому углу рабочего поля станка, проект " + MainForm.gerberFilesDir + " и лежит правильным углом (TopLeft->TopLeft станка).", new List<string> { "Да", "Нет" }, 1, false, false, true);
                    if (res > 0)
                    {
                        throw new Exception("Операция отменена.");
                    }
                    MoveToAbsolutePositionAndWaitForCompletion(false, tlDefaultCamLoc.X, tlDefaultCamLoc.Y, safeHeightMm, -1.0f);
                    res = AskUserForInput("Переместитесь так, чтобы реперное отверстие левого-верхнего угла панели оказалось в центре изображения камеры.", new List<string> { "Готово", "Отмена" }, 1, true, false, true);
                    if (res > 0)
                    {
                        throw new Exception("Операция отменена.");
                    }
                    dCorner1Mm = new Vector2(GetAbsXMm() - GetCamToToolDistX(), GetAbsYMm() - GetCamToToolDistY());

                    // Получить TR угол
                    Vector2 trDefaultCamLoc = new Vector2(GetAbsXMm() + MainForm.workImagePhysWidthMm + MainForm.holderHolesIndentMm + MainForm.spaceOuterMm, GetAbsYMm());
                    MoveToAbsolutePositionAndWaitForCompletion(false, trDefaultCamLoc.X, trDefaultCamLoc.Y, safeHeightMm, -1.0f);
                    res = AskUserForInput("Переместитесь так, чтобы реперное отверстие правого-верхнего угла панели оказалось в центре изображения камеры.", new List<string> { "Готово", "Отмена" }, 1, true, false, true);
                    if (res > 0)
                    {
                        throw new Exception("Операция отменена.");
                    }
                    dCorner2Mm = new Vector2(GetAbsXMm() - GetCamToToolDistX(), GetAbsYMm() - GetCamToToolDistY());

                    // Check
                    if (!PanelCheck())
                    {
                        throw new Exception("Неверная панель, либо она выходит за рабочее поле.");
                    }

                    // Перемещаемся в начало панели
                    MoveToAbsolutePositionAndWaitForCompletion(false, dCorner1Mm.X, dCorner2Mm.Y, safeHeightMm, -1.0f);
                } else
                {
                    // Задана ли панель?
                    if (!PanelCheck())
                    {
                        throw new Exception("Панель не задана или задана неверно.");
                    }

                    // Компонент
                    if (pec == null || pecIdx < 0)
                    {
                        throw new Exception("Компонент не найден.");
                    }

                    // Угол (-угол под которым компонент лежит + угол под которым лежит панель)
                    float lyingRad = ((float)Math.PI / 180f) * lyingDeg;
                    float bomAngleRad = (pec!.angle < 360 && pec!.angle > -360) ? (((float)Math.PI / 180f) * pec!.angle) : 0.0f;
                    float addAngleRad = ((float)Math.PI / 180f) * cAddAngle;
                    float xmyAngleRad = (-lyingRad) + GetPanelRotAngleRad() - addAngleRad + bomAngleRad;

                    // Перемещаем Z в median
                    MoveToSafeZ();

                    // Сохраняем исходную позицию
                    Vector2 posBeforePick = new Vector2(GetAbsXMm() + 0f, GetAbsYMm() + 0f);

                    // Переместить каретку так чтобы сопло PNP оказалось там где камера
                    float tgtPosX = GetAbsXMm() - GetCamToToolDistX();
                    float tgtPosY = GetAbsYMm() - GetCamToToolDistY();
                    MoveToAbsolutePositionAndWaitForCompletion(false, tgtPosX, tgtPosY, null, -1.0f);

                    // Dwell
                    Thread.Sleep(300);

                    // Pick
                    TcpSendPick();

                    // Dwell
                    Thread.Sleep(300);

                    // Переместить сопло к месту установки компонента
                    Vector2 tgtP = PanelCoordToWorkAreaCoord(new Vector2(pec?.pXMm ?? 0f, pec?.pYMm ?? 0f));
                    tgtPosX = tgtP.X;
                    tgtPosY = tgtP.Y;
                    MoveToAbsolutePositionAndWaitForCompletion(false, tgtPosX, tgtPosY, null, -1.0f);

                    // Rotate
                    TcpSendRotate((int)((xmyAngleRad * (float)stepsPerFullCTurn) / (2f * Math.PI)));

                    // Статус компонента
                    if (topSide)
                    {
                        topComponentList[pecIdx].placeStatus = 1;
                    }
                    else
                    {
                        bottomComponentList[pecIdx].placeStatus = 1;
                    }

                    // Если сложный компонент - спрашиваем пользователя как повернуть и двинуть
                    if (!(pec!.designator.StartsWith("R")) && !(pec!.designator.StartsWith("C")))
                    {
                        while (true)
                        {
                            int res = AskUserForInput("Выровняйте и поверните как нужно компонент", new List<string> { "Готово, поставить", "⤹", "⤸", "⤹1", "⤸1", "Отмена" }, 1, true, true, true);
                            if (res == 0)
                            {
                                // Ставим
                                TcpSendPlace();

                                // Статус компонента, он установлен
                                if (topSide)
                                {
                                    topComponentList[pecIdx].placeStatus = 2;
                                }
                                else
                                {
                                    bottomComponentList[pecIdx].placeStatus = 2;
                                }

                                // Демо
                                if (demoPlaced)
                                {
                                    tgtP = PanelCoordToWorkAreaCoord(new Vector2(pec?.pXMm ?? 0f, pec?.pYMm ?? 0f));
                                    tgtPosX = tgtP.X + GetCamToToolDistX();
                                    tgtPosY = tgtP.Y + GetCamToToolDistY();
                                    MoveToAbsolutePositionAndWaitForCompletion(false, tgtPosX, tgtPosY, null, -1.0f);
                                    AskUserForInput("Ознакомьтесь с установленным компонентом.", new List<string> { "ОК" }, 1, true, true, true);
                                }

                                // Исходная позиция
                                MoveToAbsolutePositionAndWaitForCompletion(false, posBeforePick.X, posBeforePick.Y, null, -1.0f);
                                break;
                            }
                            else if (res == 1)
                            {
                                // Rotate Left
                                TcpSendRotate((int)(stepsPerFullCTurn * -(90.0f / 360.0f)));
                            }
                            else if (res == 2)
                            {
                                // Rotate Right
                                TcpSendRotate((int)(stepsPerFullCTurn * (90.0f / 360.0f)));
                            }
                            else if (res == 3)
                            {
                                // Rotate Left
                                TcpSendRotate((int)(stepsPerFullCTurn * -(1.0f / 360.0f)));
                            }
                            else if (res == 4)
                            {
                                // Rotate Right
                                TcpSendRotate((int)(stepsPerFullCTurn * (1.0f / 360.0f)));
                            }
                            else
                            {
                                // Перемещаем сопло к исходной позиции и делаем Place
                                tgtPosX = posBeforePick.X - GetCamToToolDistX();
                                tgtPosY = posBeforePick.Y - GetCamToToolDistY();
                                MoveToAbsolutePositionAndWaitForCompletion(false, tgtPosX, tgtPosY, null, -1.0f);

                                // De-rotate
                                TcpSendRotate((int)((-xmyAngleRad * (float)stepsPerFullCTurn) / (2f * Math.PI)));

                                // Place
                                TcpSendPlace();

                                // Cancel
                                MoveToAbsolutePositionAndWaitForCompletion(false, posBeforePick.X, posBeforePick.Y, null, -1.0f);

                                // Статус компонента, он не установлен
                                if (topSide)
                                {
                                    topComponentList[pecIdx].placeStatus = 0;
                                }
                                else
                                {
                                    bottomComponentList[pecIdx].placeStatus = 0;
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Ставим
                        TcpSendPlace();

                        // Статус компонента, он установлен
                        if (topSide)
                        {
                            topComponentList[pecIdx].placeStatus = 2;
                        }
                        else
                        {
                            bottomComponentList[pecIdx].placeStatus = 2;
                        }

                        // Демо
                        if (demoPlaced)
                        {
                            tgtP = PanelCoordToWorkAreaCoord(new Vector2(pec?.pXMm ?? 0f, pec?.pYMm ?? 0f));
                            tgtPosX = tgtP.X + GetCamToToolDistX();
                            tgtPosY = tgtP.Y + GetCamToToolDistY();
                            MoveToAbsolutePositionAndWaitForCompletion(false, tgtPosX, tgtPosY, null, -1.0f);
                            AskUserForInput("Ознакомьтесь с установленным компонентом.", new List<string> { "ОК" }, 1, true, true, true);
                        }

                        // Исходная позиция
                        MoveToAbsolutePositionAndWaitForCompletion(false, posBeforePick.X, posBeforePick.Y, null, -1.0f);
                    }
                }

                // Finale
                pnpPending = false;
                SetBusyWithLongTask(false);
                StateChanged.Invoke(this, new EventArgs());
            }
            catch (Exception exc)
            {
                if (pecIdx >= 0)
                {
                    if (topSide)
                    {
                        topComponentList[pecIdx].placeStatus = 0;
                    }
                    else
                    {
                        bottomComponentList[pecIdx].placeStatus = 0;
                    }
                }
                pnpPending = false;
                SetBusyWithLongTask(false);
                ErrorOccurred.Invoke(this, new ErrorEventArgs(exc.Message));
            }
        }

        private float GetPanelRotAngleRad()
        {
            Vector2 diff1 = Vector2.Subtract(dCorner2Mm, dCorner1Mm);
            Vector2 diff2 = Vector2.Subtract(dCorner4Mm, dCorner3Mm);
            if (dCorner1Mm.Length() > 0f && dCorner2Mm.Length() > 0f)
            {
                if (dCorner3Mm.Length() > 0f && dCorner4Mm.Length() > 0f)
                {
                    return ((float)Math.Atan(diff1.Y / diff1.X) + (float)Math.Atan(diff2.Y / diff2.X)) / 2f;
                }
                else
                {
                    return (float)Math.Atan(diff1.Y / diff1.X);
                }
            }
            else
            {
                return 0.0f;
            }
        }

        private Vector2 PanelCoordToWorkAreaCoord(Vector2 panelCoord)
        {
            if (dCorner3Mm.Length() < 1f && dCorner4Mm.Length() < 1f)
            {
                Vector2 tgtRotated = Vector2.Transform(new Vector2(panelCoord.X - MainForm.spaceOuterMm, panelCoord.Y - MainForm.spaceOuterMm), Matrix3x2.CreateRotation(GetPanelRotAngleRad()));
                float tgtXMm = dCorner1Mm.X + tgtRotated.X;
                float tgtYMm = dCorner1Mm.Y + tgtRotated.Y;
                return new Vector2(tgtXMm, tgtYMm);
            }
            else
            {
                float amountX = Utils.ConvertRange(MainForm.spaceOuterMm, MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f) + MainForm.workImagePhysWidthMm, 0f, 1f, panelCoord.X);
                float amountY = Utils.ConvertRange(MainForm.spaceOuterMm, MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f) + MainForm.workImagePhysHeightMm, 0f, 1f, panelCoord.Y);
                Vector2 l1p = Vector2.Lerp(dCorner1Mm, dCorner2Mm, amountX);
                Vector2 l2p = Vector2.Lerp(dCorner3Mm, dCorner4Mm, amountX);
                return Vector2.Lerp(l1p, l2p, amountY);
            }
        }

        private bool PanelCheck()
        {
            if (dCorner1Mm.Length() == 0f || dCorner2Mm.Length() == 0f)
            {
                return false;
            }
            float panelSizeWidthMm = MainForm.workImagePhysWidthMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
            float panelSizeHeightMm = MainForm.workImagePhysHeightMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
            Vector2 panelA1 = PanelCoordToWorkAreaCoord(new Vector2(0f, 0f));
            Vector2 panelA2 = PanelCoordToWorkAreaCoord(new Vector2(0f, panelSizeHeightMm));
            Vector2 panelA3 = PanelCoordToWorkAreaCoord(new Vector2(panelSizeWidthMm, panelSizeHeightMm));
            Vector2 panelA4 = PanelCoordToWorkAreaCoord(new Vector2(panelSizeWidthMm, 0f));
            if (panelA1.X < 0 || panelA1.Y < 0 || panelA2.X < 0 || panelA2.Y < 0 || panelA3.X < 0 || panelA3.Y < 0 || panelA4.X < 0 || panelA4.Y < 0)
            {
                return false;
            }
            if (panelA1.X > maxTravelMmX || panelA1.Y > maxTravelMmY || panelA2.X > maxTravelMmX || panelA2.Y > maxTravelMmY || panelA3.X > maxTravelMmX || panelA3.Y > maxTravelMmY || panelA4.X > maxTravelMmX || panelA4.Y > maxTravelMmY)
            {
                return false;
            }
            return true;
        }

        public bool IsPanelSelected()
        {
            return PanelCheck();
        }
    }
}