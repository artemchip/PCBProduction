using DirectShowLib;
using DirectShowLib.BDA;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using GerberVS;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBProduction2
{
    public partial class PNPForm : Form
    {
        // Gerber
        public LibGerberVS gerberVS = new LibGerberVS();

        // Захват видео с камеры
        public object lockObject = new object();
        public int displayCamIdx = 0;
        public Mat curRawFrame = new Mat();
        public VideoCapture? pnpCamCapture1;
        public VideoCapture? pnpCamCapture2;
        public VideoCapture? pnpCamCapture3;
        public int framesCaptured1 = 0;
        public int framesCaptured2 = 0;
        public int framesCaptured3 = 0;
        public bool hFlip = false;
        public bool vFlip = false;
        public bool rotateCamImage = false;
        public bool showCV = false;
        public float imageAngleDeg = 0f;
        public float rectWidthMm = 0.0f;
        public float rectHeightMm = 0.0f;
        public float imagePxPerMm = 10f;
        public bool showRect = false;
        public bool secondCamConnected = false;
        public bool thirdCamConnected = false;
        public int fovXFromPercent = 0;
        public int fovXToPercent = 100;
        public int fovYFromPercent = 0;
        public int fovYToPercent = 100;

        // Обработка изображений
        public Mat transformMatrix = new Mat();
        public Mat curRotFrame = new Mat();
        public Mat curRszFrame = new Mat();
        public Mat curDGrayFrame = new Mat();
        public Mat curGrayBlurredFrame = new Mat();
        public Mat curThreshFrame = new Mat();

        // Отображение изображений
        public Mat curDispFrame = new Mat();
        public float zoomFactor = 1.0f;

        // Калибровка движения
        public float motionStepsPerMmX = 800f;
        public float motionStepsPerMmY = 800f;
        public float motionSkewAngleRad = 0.0f;
        public Vector2 calibTopLeft = new Vector2(0f, 0f);
        public Vector2 calibTopRight = new Vector2(0f, 0f);
        public Vector2 calibBottomLeft = new Vector2(0f, 0f);
        public Vector2 calibBottomRight = new Vector2(0f, 0f);

        // Калибровка Backlash
        public float backlashMmX = 0.0f;
        public float backlashMmY = 0.0f;

        // Калибровка PNP
        public float nozzleRelMmX = 0.0f;
        public float nozzleRelMmY = 0.0f;
        public float placeHeightMm = 0.0f;
        public float safeHeightMm = 0.0f;
        public int cstepsPerSeq = 100;
        public int stepsPerCTurn = 100;
        public bool cPositiveClockwise = false;
        public float addAngleDeg = 0.0f;

        // Обработка PNP
        public bool activeSideTop = true;
        public List<PEComponent> topComponentList = new List<PEComponent>();
        public List<PEComponent> bottomComponentList = new List<PEComponent>();
        public List<PointF[]> boardContoursTop = new List<PointF[]>();
        public List<PointF[]> boardContoursBottom = new List<PointF[]>();

        // PNP текущий компонент
        public int selectedComponentId = -1;
        public int pnpAction = 0;
        public float pnpActionAngleDeg = 90.0f;
        public Vector2 posBeforePick = new Vector2(0f, 0f);

        // Обработка (общее)
        public Vector2 dCorner1Mm = new Vector2(0f, 0f);
        public Vector2 dCorner2Mm = new Vector2(0f, 0f);
        public Vector2 dCorner3Mm = new Vector2(0f, 0f);
        public Vector2 dCorner4Mm = new Vector2(0f, 0f);
        public Thread? machiningThread;
        public bool shouldMachiningStop = false;

        // Любимые места
        public PointF favLoc1 = new PointF(0f, 0f);
        public PointF favLoc2 = new PointF(0f, 0f);
        public PointF favLoc3 = new PointF(0f, 0f);
        public PointF favLoc4 = new PointF(0f, 0f);
        public PointF favLoc5 = new PointF(0f, 0f);
        public PointF favLoc6 = new PointF(0f, 0f);
        public PointF favLoc7 = new PointF(0f, 0f);
        public PointF favLoc8 = new PointF(0f, 0f);
        public PointF favLoc9 = new PointF(0f, 0f);
        public PointF favLoc10 = new PointF(0f, 0f);

        // TCP-соединение с PNP контроллером
        public TcpClient? tcpClient;
        public NetworkStream? tcpStream;
        public String machineIP = "";
        public bool lightOn = false;

        // Grbl подключение и исходящие от него статусы
        public SerialPort? grblPort;
        public float accXMmPerSecSq = 0.0f;
        public float accYMmPerSecSq = 0.0f;
        public float accZMmPerSecSq = 0.0f;
        public float maxTravelMmX = 0.0f;
        public float maxTravelMmY = 0.0f;
        public float maxTravelMmZ = 0.0f;
        public float maxFeedRateMmPerMinX = 0.0f;
        public float maxFeedRateMmPerMinY = 0.0f;
        public float maxFeedRateMmPerMinZ = 0.0f;
        public float absMmX = 0.0f;
        public float absMmY = 0.0f;
        public float absMmZ = 0.0f;
        public bool? dirX;
        public bool? dirY;
        public bool cncConnected = false;
        public bool pnpFault = false;
        public int grblCommandCmpl = 0;
        public bool areWeMoving = false;
        public Thread? motionFinishWaitThread;
        public Stopwatch msw = new Stopwatch();

        // Grbl настройки которые задаем мы
        public Vector3 homingCorner = new Vector3(0f, 1f, 1f); // X0 - слева, X1 - справа, Y0 - сверху, Y1 - снизу, Z0 - снизу, Z1 - сверху
        public bool xPlusIsRight = true; // X+ направлен вправо
        public bool yPlusIsDown = true; // Y+ направлен вних
        public bool zPlusIsWithdraw = true; // Z+ направлен вверх (выход из заготовки)

        // Обработчики событий
        public bool updatingSettingsUI = false;

        public PreviewForm previewForm = new PreviewForm();

        public PNPForm()
        {
            InitializeComponent();

            Vector2 topLeft = new Vector2(275.61f, 130.85f);
            Vector2 topRight = new Vector2(284.61f, 131.05f);
            Vector2 bottomLeft = new Vector2(275.46f, 139.73f);
            Vector2 bottomRight = new Vector2(284.31f, 139.93f);

            Vector2 trX1 = Vector2.Subtract(topRight, topLeft);
            Vector2 trX2 = Vector2.Subtract(bottomRight, bottomLeft);
            float angleRad = ((float)Math.Atan(trX1.Y / trX1.X) + (float)Math.Atan(trX2.Y / trX2.X)) / 2f;
            float angDegrees = (180f / 3.14159265f) * angleRad;
            Console.WriteLine(angDegrees);
        }

        private void btnPnpConnect_Click(object sender, EventArgs e)
        {
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Станок в работе над вашим Gerber-файлом или компонентом, функция недоступна.");
                return;
            }
            try
            {
                if ((int)btnPnpConnect.Tag == 1)
                {
                    if (grblCommandCmpl == 0)
                    {
                        ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                        return;
                    }
                    if (areWeMoving)
                    {
                        ShowMessageBox("Идет движение, дождитесь завершения.");
                        return;
                    }
                    DeinitCNCMachineConnection();
                }
                else
                {
                    InitCNCMachineConnection();
                }
                UpdateConnectionUIState();
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void PNPForm_Load(object sender, EventArgs e)
        {
            LoadSavedCalibrationDataAndUI();
            LoadInfoFromGerberAndInitUI();
            InitComponentListViewUI();
            RefreshComponentListViewUI();
            UpdateConnectionUIState();
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

        public void LoadInfoFromGerberAndInitUI()
        {
            try
            {
                // Очистка
                topComponentList.Clear();
                bottomComponentList.Clear();

                // Файлы Gerber
                String mntFile = "";
                String mnbFile = "";
                String gbrProfileFile = "";
                List<String> allGerberFiles = Directory.GetFiles(MainForm.gerberFilePath).ToList();
                allGerberFiles.Sort((a, b) => File.ReadAllText(a).Length.CompareTo(File.ReadAllText(b).Length));
                foreach (String pFile in allGerberFiles)
                {
                    if (pFile.ToLower().EndsWith(".gko") || pFile.ToLower().Contains("outline") || pFile.ToLower().Contains("profile"))
                    {
                        gbrProfileFile = pFile;
                    }
                    else if (pFile.ToLower().EndsWith(".mnt"))
                    {
                        mntFile = pFile;
                    }
                    else if (pFile.ToLower().EndsWith(".mnb"))
                    {
                        mnbFile = pFile;
                    }
                }
                if (gbrProfileFile.Length <= 0)
                {
                    throw new Exception("Не найдены файлы board outline.");
                }
                if (mntFile.Length <= 0 && mnbFile.Length <= 0)
                {
                    throw new Exception("Не найдены файлы .mnt и .mnb (centroid).");
                }

                // Размер и сколько плат в панели
                GerberProject proj = gerberVS.CreateNewProject();
                gerberVS.OpenLayerFromFileName(proj, gbrProfileFile);
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
                if (mntFile.Length > 0)
                {
                    String[] topStrings = File.ReadAllLines(mntFile);
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
                if (mnbFile.Length > 0)
                {
                    String[] bottomStrings = File.ReadAllLines(mnbFile);
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
                ShowMessageBox("У вас ошибка в Gerber-файлах: " + ex.Message);
                Close();
                return;
            }
        }

        public bool IsAngleString(String inStr)
        {
            String cleanStr = inStr.Replace(".", "").Replace(",", "").Replace(";", "").Replace(":", "").Replace(" ", "");
            return (((int)Utils.StrToDouble(cleanStr)) % 15) == 0;
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

        public void InitComponentListViewUI()
        {
            lvComponents.Enabled = true;
            lvComponents.GridLines = true;
            lvComponents.AllowColumnReorder = false;
            lvComponents.FullRowSelect = true;
            lvComponents.View = View.Details;
            lvComponents.Columns.Clear();
            lvComponents.Columns.Add("#");
            lvComponents.Columns.Add("Обозначение");
            lvComponents.Columns.Add("Величина");
            lvComponents.Columns.Add("X");
            lvComponents.Columns.Add("Y");
            lvComponents.Columns.Add("Угол");
            lvComponents.Columns.Add("Статус");
            foreach (ColumnHeader currColumn in lvComponents.Columns)
            {
                currColumn.Width = (int)((float)(lvComponents.Width - 16) / (float)lvComponents.Columns.Count);
            }
        }

        public void RefreshComponentListViewUI()
        {
            if (InvokeRequired)
            {
                Invoke(RefreshComponentListViewUI);
            }
            else
            {
                int topItemIndex = 0;
                try
                {
                    if (lvComponents.Items.Count > 0)
                    {
                        topItemIndex = lvComponents.TopItem.Index;
                    }
                }
                catch (Exception ex) { }
                lvComponents.BeginUpdate();
                lvComponents.Items.Clear();
                List<PEComponent> cmpList = new List<PEComponent>();
                if (activeSideTop)
                {
                    cmpList.AddRange(topComponentList);
                }
                else
                {
                    cmpList.AddRange(bottomComponentList);
                }
                for (int i = 0; i < cmpList.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem(i.ToString());
                    String statusStr = "?";
                    if (cmpList[i].placeStatus == 0)
                    {
                        if (selectedComponentId == cmpList[i].id)
                        {
                            statusStr = "Выбран для установки";
                            lvi.ForeColor = Color.Yellow;
                        }
                        else
                        {
                            statusStr = "Не установлен";
                        }
                    }
                    else if (cmpList[i].placeStatus == 1)
                    {
                        statusStr = "В процессе";
                        lvi.ForeColor = Color.Orange;
                    }
                    else if (cmpList[i].placeStatus == 2)
                    {
                        statusStr = "Установлен";
                        lvi.ForeColor = Color.Green;
                    }
                    lvi.SubItems.Add(cmpList[i].designator);
                    lvi.SubItems.Add(cmpList[i].value);
                    lvi.SubItems.Add(cmpList[i].pXMm.ToString("0.00"));
                    lvi.SubItems.Add(cmpList[i].pYMm.ToString("0.00"));
                    lvi.SubItems.Add(cmpList[i].angle.ToString("0"));
                    lvi.SubItems.Add(statusStr);
                    lvi.Tag = i;
                    lvComponents.Items.Add(lvi);
                }
                lvComponents.EndUpdate();
                try
                {
                    lvComponents.TopItem = lvComponents.Items[topItemIndex];
                }
                catch (Exception ex) { }
                if (selectedComponentId < 0)
                {
                    lblSelComp.Text = "Компонент не выбран";
                }
                else
                {
                    PEComponent? pecTop = null;
                    PEComponent? pecBottom = null;
                    try
                    {
                        pecTop = topComponentList.Where((e) => e.id == selectedComponentId).First();
                    }
                    catch { }
                    try
                    {
                        pecBottom = bottomComponentList.Where((e) => e.id == selectedComponentId).First();
                    }
                    catch { }
                    if (pecTop != null)
                    {
                        lblSelComp.Text = "Выбран " + pecTop.designator + " на стороне Top";
                    }
                    else if (pecBottom != null)
                    {
                        lblSelComp.Text = "Выбран " + pecBottom.designator + " на стороне Bottom";
                    }
                    else
                    {
                        lblSelComp.Text = "Выбран неизвестный компонент";
                    }
                }
            }
        }

        public bool IsMachining()
        {
            return machiningThread?.IsAlive == true;
        }

        private void LoadSavedCalibrationDataAndUI()
        {
            if (File.Exists("pnp_cal.xml"))
            {
                FileStorage storage = new FileStorage("pnp_cal.xml", FileStorage.Mode.Read);
                nozzleRelMmX = storage.GetNode("nozzle_rel_mm_x").ReadFloat();
                nozzleRelMmY = storage.GetNode("nozzle_rel_mm_y").ReadFloat();
                placeHeightMm = storage.GetNode("place_height_mm").ReadFloat();
                safeHeightMm = storage.GetNode("safe_height_mm").ReadFloat();
                cstepsPerSeq = storage.GetNode("c_steps_per_seq").ReadInt();
                stepsPerCTurn = storage.GetNode("steps_per_c_turn").ReadInt();
                cPositiveClockwise = storage.GetNode("c_positive_clockwise").ReadInt() > 0;
                addAngleDeg = storage.GetNode("add_angle_deg").ReadFloat();
                storage.ReleaseAndGetString();
            }
            if (File.Exists("pnp_net.xml"))
            {
                FileStorage storage = new FileStorage("pnp_net.xml", FileStorage.Mode.Read);
                machineIP = storage.GetNode("ip").ReadString();
                storage.ReleaseAndGetString();
            }
            if (File.Exists("pnp_motion.xml"))
            {
                FileStorage storage = new FileStorage("pnp_motion.xml", FileStorage.Mode.Read);
                motionStepsPerMmX = storage.GetNode("x_steps_per_mm").ReadFloat(-1f);
                motionStepsPerMmY = storage.GetNode("y_steps_per_mm").ReadFloat(-1f);
                motionSkewAngleRad = storage.GetNode("skew_angle_rad").ReadFloat();
                storage.ReleaseAndGetString();
            }
            if (File.Exists("pnp_backlash.xml"))
            {
                FileStorage storage = new FileStorage("pnp_backlash.xml", FileStorage.Mode.Read);
                backlashMmX = storage.GetNode("backlash_mm_x").ReadFloat(0f);
                backlashMmY = storage.GetNode("backlash_mm_y").ReadFloat(0f);
                storage.ReleaseAndGetString();
            }
            if (File.Exists("pnp_axes.xml"))
            {
                FileStorage storage = new FileStorage("pnp_axes.xml", FileStorage.Mode.Read);
                xPlusIsRight = storage.GetNode("x_plus_is_right").ReadInt() > 0;
                yPlusIsDown = storage.GetNode("y_plus_is_down").ReadInt() > 0;
                zPlusIsWithdraw = storage.GetNode("z_plus_is_withdraw").ReadInt() > 0;
                storage.ReleaseAndGetString();
            }
            if (File.Exists("pnp_image.xml"))
            {
                FileStorage storage = new FileStorage("pnp_image.xml", FileStorage.Mode.Read);
                hFlip = storage.GetNode("h_flip").ReadInt() > 0;
                vFlip = storage.GetNode("v_flip").ReadInt() > 0;
                rotateCamImage = storage.GetNode("rotate_cam_image").ReadInt() > 0;
                showCV = storage.GetNode("show_cv").ReadInt() > 0;
                imageAngleDeg = storage.GetNode("image_angle_deg").ReadFloat();
                rectWidthMm = storage.GetNode("rect_width_mm").ReadFloat();
                rectHeightMm = storage.GetNode("rect_height_mm").ReadFloat();
                imagePxPerMm = storage.GetNode("image_px_per_mm").ReadFloat();
                showRect = storage.GetNode("show_rect").ReadInt() > 0;
                fovXFromPercent = storage.GetNode("fov_x_from").ReadInt();
                fovXToPercent = storage.GetNode("fov_x_to").ReadInt();
                fovYFromPercent = storage.GetNode("fov_y_from").ReadInt();
                fovYToPercent = storage.GetNode("fov_y_to").ReadInt();
                storage.ReleaseAndGetString();
            }
            if (File.Exists("pnp_fav.xml"))
            {
                FileStorage storage = new FileStorage("pnp_fav.xml", FileStorage.Mode.Read);
                favLoc1 = new PointF(storage.GetNode("fav_loc_1_x_mm").ReadFloat(), storage.GetNode("fav_loc_1_y_mm").ReadFloat());
                favLoc2 = new PointF(storage.GetNode("fav_loc_2_x_mm").ReadFloat(), storage.GetNode("fav_loc_2_y_mm").ReadFloat());
                favLoc3 = new PointF(storage.GetNode("fav_loc_3_x_mm").ReadFloat(), storage.GetNode("fav_loc_3_y_mm").ReadFloat());
                favLoc4 = new PointF(storage.GetNode("fav_loc_4_x_mm").ReadFloat(), storage.GetNode("fav_loc_4_y_mm").ReadFloat());
                favLoc5 = new PointF(storage.GetNode("fav_loc_5_x_mm").ReadFloat(), storage.GetNode("fav_loc_5_y_mm").ReadFloat());
                favLoc6 = new PointF(storage.GetNode("fav_loc_6_x_mm").ReadFloat(), storage.GetNode("fav_loc_6_y_mm").ReadFloat());
                favLoc7 = new PointF(storage.GetNode("fav_loc_7_x_mm").ReadFloat(), storage.GetNode("fav_loc_7_y_mm").ReadFloat());
                favLoc8 = new PointF(storage.GetNode("fav_loc_8_x_mm").ReadFloat(), storage.GetNode("fav_loc_8_y_mm").ReadFloat());
                favLoc9 = new PointF(storage.GetNode("fav_loc_9_x_mm").ReadFloat(), storage.GetNode("fav_loc_9_y_mm").ReadFloat());
                favLoc10 = new PointF(storage.GetNode("fav_loc_10_x_mm").ReadFloat(), storage.GetNode("fav_loc_10_y_mm").ReadFloat());
                storage.ReleaseAndGetString();
            }
            updatingSettingsUI = true;
            SetMotionCompensateLabel();
            SetMotionCompensateNUD();
            SetBacklashUI();
            SetMachineAxesUI();
            SetNetworkUI();
            SetPNPCalUI();
            SetFavLocUI();
            SetImageSettingsUI();
            SetAbsPositionLabel();
            updatingSettingsUI = false;
        }

        public void SetCalibrationUIEnabled(bool flag)
        {
            if (InvokeRequired)
            {
                Invoke(SetCalibrationUIEnabled, flag);
            }
            else
            {
                nudBacklashMmX.Enabled = flag;
                nudBacklashMmY.Enabled = flag;
                nudSkewAngleRad.Enabled = flag;
                nudStepsPerMmX.Enabled = flag;
                nudStepsPerMmY.Enabled = flag;
                btnApplyMotion.Enabled = flag;
                btnCalibPanelTopLeft.Enabled = flag;
                btnCalibPanelTopRight.Enabled = flag;
                btnCalibPanelBottomLeft.Enabled = flag;
                btnCalibPanelBottomRight.Enabled = flag;
                btnCalibPanelCalculate.Enabled = flag;
            }
        }

        public void ShowMessageBox(String text)
        {
            if (InvokeRequired)
            {
                Invoke(ShowMessageBox, text);
            }
            else
            {
                MessageBox.Show(this, text);
            }
        }

        public void SetBacklashUI()
        {
            if (InvokeRequired)
            {
                Invoke(SetBacklashUI);
            }
            else
            {
                nudBacklashMmX.Value = (decimal)backlashMmX;
                nudBacklashMmY.Value = (decimal)backlashMmY;
            }
        }

        public void SetMotionCompensateLabel()
        {
            if (InvokeRequired)
            {
                Invoke(SetMotionCompensateLabel);
            }
            else
            {
                lblMotionCompens.Text = "Шагов на мм X: " + motionStepsPerMmX.ToString("0.000") + "\nШагов на мм Y: " + motionStepsPerMmY.ToString("0.000") + "\nКомпенсация угла кривизны XY, рад: " + motionSkewAngleRad.ToString("0.00000");
            }
        }

        public void SetMotionCompensateNUD()
        {
            if (InvokeRequired)
            {
                Invoke(SetMotionCompensateNUD);
            }
            else
            {
                nudStepsPerMmX.Value = (decimal)motionStepsPerMmX;
                nudStepsPerMmY.Value = (decimal)motionStepsPerMmY;
                nudSkewAngleRad.Value = (decimal)motionSkewAngleRad;
            }
        }

        public void SetPNPCalUI()
        {
            if (InvokeRequired)
            {
                Invoke(SetPNPCalUI);
            }
            else
            {
                nudPlaceHeightMm.Value = (decimal)placeHeightMm;
                nudSafeHeightMm.Value = (decimal)safeHeightMm;
                nudNozzleRelMmX.Value = (decimal)nozzleRelMmX;
                nudNozzleRelMmY.Value = (decimal)nozzleRelMmY;
                nudCStepsPerSeq.Value = (decimal)cstepsPerSeq;
                nudStepsPerCTurn.Value = (decimal)stepsPerCTurn;
                nudAddAngleDeg.Value = (decimal)addAngleDeg;
                cbCPositiveClockwise.Checked = cPositiveClockwise;
            }
        }

        public void SetFavLocUI()
        {
            if (InvokeRequired)
            {
                Invoke(SetFavLocUI);
            }
            else
            {
                nudLoc1XMm.Value = (decimal)favLoc1.X;
                nudLoc1YMm.Value = (decimal)favLoc1.Y;
                nudLoc2XMm.Value = (decimal)favLoc2.X;
                nudLoc2YMm.Value = (decimal)favLoc2.Y;
                nudLoc3XMm.Value = (decimal)favLoc3.X;
                nudLoc3YMm.Value = (decimal)favLoc3.Y;
                nudLoc4XMm.Value = (decimal)favLoc4.X;
                nudLoc4YMm.Value = (decimal)favLoc4.Y;
                nudLoc5XMm.Value = (decimal)favLoc5.X;
                nudLoc5YMm.Value = (decimal)favLoc5.Y;
                nudLoc6XMm.Value = (decimal)favLoc6.X;
                nudLoc6YMm.Value = (decimal)favLoc6.Y;
                nudLoc7XMm.Value = (decimal)favLoc7.X;
                nudLoc7YMm.Value = (decimal)favLoc7.Y;
                nudLoc8XMm.Value = (decimal)favLoc8.X;
                nudLoc8YMm.Value = (decimal)favLoc8.Y;
                nudLoc9XMm.Value = (decimal)favLoc9.X;
                nudLoc9YMm.Value = (decimal)favLoc9.Y;
                nudLoc10XMm.Value = (decimal)favLoc10.X;
                nudLoc10YMm.Value = (decimal)favLoc10.Y;
            }
        }

        public void SetImageSettingsUI()
        {
            if (InvokeRequired)
            {
                Invoke(SetImageSettingsUI);
            }
            else
            {
                cbImageFlipV.Checked = vFlip;
                cbImageFlipH.Checked = hFlip;
                cbImageShowAngleGrid.Checked = rotateCamImage;
                cbImageShowShapes.Checked = showCV;
                nudImageAngleDeg.Value = (decimal)imageAngleDeg;
                nudRectWidthMm.Value = (decimal)rectWidthMm;
                nudRectHeightMm.Value = (decimal)rectHeightMm;
                nudImagePxPerMm.Value = (decimal)imagePxPerMm;
                nudFOVXFrom.Value = (decimal)fovXFromPercent;
                nudFOVXTo.Value = (decimal)fovXToPercent;
                nudFOVYFrom.Value = (decimal)fovYFromPercent;
                nudFOVYTo.Value = (decimal)fovYToPercent;
                cbShowRect.Checked = showRect;
            }
        }

        public void SetMoving()
        {
            if (InvokeRequired)
            {
                Invoke(SetMoving);
            }
            else
            {
                btnXPos.Enabled = !areWeMoving;
                btnYPos.Enabled = !areWeMoving;
                btnXNeg.Enabled = !areWeMoving;
                btnYNeg.Enabled = !areWeMoving;
                btnZPos.Enabled = !areWeMoving;
                btnZNeg.Enabled = !areWeMoving;
                btnMoveToLoc1.Enabled = !areWeMoving;
                btnMoveToLoc2.Enabled = !areWeMoving;
                btnMoveToLoc3.Enabled = !areWeMoving;
                btnMoveToLoc4.Enabled = !areWeMoving;
                btnMoveToLoc5.Enabled = !areWeMoving;
                btnMoveToLoc6.Enabled = !areWeMoving;
                btnMoveToLoc7.Enabled = !areWeMoving;
                btnMoveToLoc8.Enabled = !areWeMoving;
                btnMoveToLoc9.Enabled = !areWeMoving;
                btnMoveToLoc10.Enabled = !areWeMoving;
                pnMotionIndicator.BackColor = (areWeMoving) ? Color.Red : Color.Green;
            }
        }

        public void SetAbsPositionLabel()
        {
            if (lblPosition.InvokeRequired)
            {
                lblPosition.Invoke(SetAbsPositionLabel);
            }
            else
            {
                if (!cncConnected)
                {
                    lblPosition.Text = "";
                }
                else if (pnpFault)
                {
                    lblPosition.Text = "Сбой соединения GRBL.";
                }
                else
                {
                    String totText = "Камера - X: " + absMmX.ToString("0.00") + ", Y: " + absMmY.ToString("0.00") + ", Z: " + absMmZ.ToString("0.00");
                    if (nozzleRelMmX != 0.0f || nozzleRelMmY != 0.0f)
                    {
                        totText += "\nСопло PNP - X: " + (absMmX + nozzleRelMmX).ToString("0.00") + ", Y: " + (absMmY + nozzleRelMmY).ToString("0.00");
                    }
                    lblPosition.Text = totText;
                }
            }
        }

        public void SetNetworkUI()
        {
            if (InvokeRequired)
            {
                Invoke(SetNetworkUI);
            }
            else
            {
                tbMachineIP.Text = machineIP;
            }
        }

        public void SetMachineAxesUI()
        {
            if (InvokeRequired)
            {
                Invoke(SetMachineAxesUI);
            }
            else
            {
                if (homingCorner.X < 0.5f && homingCorner.Y < 0.5f)
                {
                    rbXYTopLeft.Checked = true;
                    rbXYBottomLeft.Checked = false;
                    rbXYBottomRight.Checked = false;
                    rbXYTopRight.Checked = false;
                }
                else if (homingCorner.X > 0.5f && homingCorner.Y < 0.5f)
                {
                    rbXYTopRight.Checked = true;
                    rbXYBottomLeft.Checked = false;
                    rbXYBottomRight.Checked = false;
                    rbXYTopLeft.Checked = false;
                }
                else if (homingCorner.X < 0.5f && homingCorner.Y > 0.5f)
                {
                    rbXYBottomLeft.Checked = true;
                    rbXYBottomRight.Checked = false;
                    rbXYTopLeft.Checked = false;
                    rbXYTopRight.Checked = false;
                }
                else
                {
                    rbXYBottomRight.Checked = true;
                    rbXYBottomLeft.Checked = false;
                    rbXYTopLeft.Checked = false;
                    rbXYTopRight.Checked = false;
                }
                if (xPlusIsRight)
                {
                    rbXRight.Checked = true;
                    rbXLeft.Checked = false;
                }
                else
                {
                    rbXRight.Checked = false;
                    rbXLeft.Checked = true;
                }
                if (yPlusIsDown)
                {
                    rbYDown.Checked = true;
                    rbYUp.Checked = false;
                }
                else
                {
                    rbYDown.Checked = false;
                    rbYUp.Checked = true;
                }
                if (zPlusIsWithdraw)
                {
                    rbZAwayFromWorkpiece.Checked = true;
                    rbZTowardsWorkpiece.Checked = false;
                }
                else
                {
                    rbZAwayFromWorkpiece.Checked = false;
                    rbZTowardsWorkpiece.Checked = true;
                }
            }
        }

        public void UpdateConnectionUIState()
        {
            if (InvokeRequired)
            {
                Invoke(UpdateConnectionUIState);
            }
            else
            {
                if (cncConnected)
                {
                    btnPnpConnect.Text = "Отключиться от станка";
                    btnPnpConnect.Tag = 1;
                }
                else
                {
                    btnPnpConnect.Text = "Подключиться к станку (не используйте USB Hub!)";
                    btnPnpConnect.Tag = 0;
                }
                rbXLeft.Enabled = !cncConnected;
                rbXRight.Enabled = !cncConnected;
                rbYUp.Enabled = !cncConnected;
                rbYDown.Enabled = !cncConnected;
                rbZAwayFromWorkpiece.Enabled = !cncConnected;
                rbZTowardsWorkpiece.Enabled = !cncConnected;
                rbXYBottomLeft.Enabled = !cncConnected;
                rbXYBottomRight.Enabled = !cncConnected;
                rbXYTopLeft.Enabled = !cncConnected;
                rbXYTopRight.Enabled = !cncConnected;
                tbMachineIP.Enabled = !cncConnected;
            }
        }

        private void SetMaxResolution(int devIdx)
        {
            // Переменные
            int hr, bitCount = 0;
            IntPtr fetched = IntPtr.Zero;
            IBaseFilter sourceFilter = null;
            VideoInfoHeader v = new VideoInfoHeader();
            IEnumMediaTypes mediaTypeEnum;
            DsDevice[] capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            DsDevice vidDev = capDevices[devIdx];

            // Filtergraph
            var m_FilterGraph2 = new FilterGraph() as IFilterGraph2;
            if (m_FilterGraph2 == null) throw new Exception("Null filtergraph");
            hr = m_FilterGraph2.AddSourceFilterForMoniker(vidDev.Mon, null, vidDev.Name, out sourceFilter);
            var pRaw2 = DsFindPin.ByCategory(sourceFilter, PinCategory.Capture, 0);
            hr = pRaw2.EnumMediaTypes(out mediaTypeEnum);
            AMMediaType[] mediaTypes = new AMMediaType[1];
            hr = mediaTypeEnum.Next(1, mediaTypes, fetched);

            // Все разрешения
            int maxWidth = 640;
            int maxHeight = 480;
            while (mediaTypes[0] != null)
            {
                Marshal.PtrToStructure(mediaTypes[0].formatPtr, v);
                if (v.BmiHeader.Size != 0 && v.BmiHeader.BitCount != 0)
                {
                    if (v.BmiHeader.BitCount > bitCount)
                    {
                        bitCount = v.BmiHeader.BitCount;
                    }
                    if (v.BmiHeader.Width * v.BmiHeader.Height > maxWidth * maxHeight)
                    {
                        maxWidth = v.BmiHeader.Width;
                        maxHeight = v.BmiHeader.Height;
                    }
                }
                hr = mediaTypeEnum.Next(1, mediaTypes, fetched);
            }
            if (devIdx == 0)
            {
                pnpCamCapture1?.Set(CapProp.FrameWidth, maxWidth);
                pnpCamCapture1?.Set(CapProp.FrameHeight, maxHeight);
            }
            else if (devIdx == 1)
            {
                pnpCamCapture2?.Set(CapProp.FrameWidth, maxWidth);
                pnpCamCapture2?.Set(CapProp.FrameHeight, maxHeight);
            }
            else
            {
                pnpCamCapture3?.Set(CapProp.FrameWidth, maxWidth);
                pnpCamCapture3?.Set(CapProp.FrameHeight, maxHeight);
            }
        }

        public void InitCNCMachineConnection()
        {
            // TCP
            pnpFault = false;
            try
            {
                tcpClient?.Close();
            }
            catch { }
            tcpClient = new TcpClient();
            tcpClient.SendTimeout = 2000;
            tcpClient.ReceiveTimeout = 2000;
            if (!tcpClient!.ConnectAsync(machineIP, 7008).Wait(2000))
            {
                throw new Exception("Тайм-аут TCP-соединения.");
            }
            tcpStream = tcpClient?.GetStream();
            if (tcpStream == null)
            {
                throw new Exception("Ошибка при открытии потока TCP-соединения.");
            }
            lightOn = false;

            // GRBL
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length < 1)
            {
                throw new Exception("Grbl не найден.");
            }
            grblPort = null;
            int pIdx = 0;
            while (true)
            {
                grblPort = new SerialPort(ports[pIdx], 115200);
                grblPort.Open();
                grblPort.WriteTimeout = 3000;
                grblPort.ReadTimeout = 3000;
                grblPort.Write("?");
                String fLine = grblPort.ReadLine();
                if (fLine.ToLower().StartsWith("<"))
                {
                    break;
                }
                try
                {
                    grblPort?.Close();
                }
                catch (Exception e) { }
                pIdx++;
                if (pIdx >= ports.Length)
                {
                    try
                    {
                        tcpClient?.Close();
                    }
                    catch { }
                    throw new Exception("Grbl не найден.");
                }
            }
            grblPort!.DataReceived += GrblPort_DataReceived;
            grblPort!.ErrorReceived += GrblPort_ErrorReceived;

            // Сброс
            grblCommandCmpl = 0;
            grblPort.WriteLine("$X");
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                try
                {
                    tcpClient?.Close();
                }
                catch { }
                throw new Exception("Ошибка станка при запуске.");
            }

            // Включить оси
            grblCommandCmpl = 0;
            grblPort.WriteLine("$1=255");
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                try
                {
                    tcpClient?.Close();
                }
                catch { }
                throw new Exception("Ошибка станка при запуске.");
            }

            // Считать шаги на мм и другие параметры
            grblCommandCmpl = 0;
            grblPort.WriteLine("$$");
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                try
                {
                    tcpClient?.Close();
                }
                catch { }
                throw new Exception("Ошибка станка при чтении памяти.");
            }
            if (motionStepsPerMmX < 0f || motionStepsPerMmY < 0f)
            {
                try
                {
                    tcpClient?.Close();
                }
                catch { }
                throw new Exception("Ошибка чтения шагов на мм из памяти станка.");
            }
            SetMotionCompensateLabel();
            SetMotionCompensateNUD();

            // Home
            grblCommandCmpl = 0;
            grblPort.WriteLine("$H");
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                try
                {
                    tcpClient?.Close();
                }
                catch { }
                throw new Exception("Ошибка станка при перемещении в ноль.");
            }
            absMmX = (homingCorner.X > 0.5f) ? maxTravelMmX : 0f;
            absMmY = (homingCorner.Y > 0.5f) ? maxTravelMmY : 0f;
            absMmZ = (homingCorner.Z > 0.5f) ? maxTravelMmZ : 0f;

            // Относительное позиционирование
            grblCommandCmpl = 0;
            grblPort.WriteLine("G91");
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                try
                {
                    tcpClient?.Close();
                }
                catch { }
                throw new Exception("Ошибка станка при запуске.");
            }

            // Z
            if (safeHeightMm > 1.0f)
            {
                MoveToSafeZ();
            }

            // Камера 1
            framesCaptured1 = 100;
            pnpCamCapture1 = new VideoCapture(0, VideoCapture.API.DShow);
            pnpCamCapture1.Set(CapProp.Autofocus, 0.0f);
            pnpCamCapture1.Set(CapProp.Focus, tbCamFocus.Value);
            SetMaxResolution(0);
            pnpCamCapture1.ExceptionMode = false;
            int attempts = 0;
            while (!pnpCamCapture1.IsOpened && attempts < 3)
            {
                Thread.Sleep(1000);
                attempts++;
            }
            if (!pnpCamCapture1.IsOpened)
            {
                try
                {
                    tcpClient?.Close();
                }
                catch { }
                try
                {
                    grblPort?.Close();
                }
                catch { }
                throw new Exception("Не удалось обнаружить камеру.");
            }
            pnpCamCapture1.ImageGrabbed += pnpCamCapture1_ImageGrabbed;
            pnpCamCapture1.Start();
            framesCaptured1 = 100;

            // Камера 2
            try
            {
                framesCaptured2 = 100;
                pnpCamCapture2 = new VideoCapture(1, VideoCapture.API.DShow);
                pnpCamCapture2.Set(CapProp.Autofocus, 0.0f);
                pnpCamCapture2.Set(CapProp.Focus, tbCamFocus.Value);
                SetMaxResolution(1);
                pnpCamCapture2.ExceptionMode = false;
                attempts = 0;
                while (!pnpCamCapture2.IsOpened && attempts < 3)
                {
                    Thread.Sleep(1000);
                    attempts++;
                }
                if (!pnpCamCapture2.IsOpened)
                {
                    throw new Exception("Не удалось обнаружить камеру.");
                }
                pnpCamCapture2.ImageGrabbed += pnpCamCapture2_ImageGrabbed;
                pnpCamCapture2.Start();
                secondCamConnected = true;
                framesCaptured2 = 100;
            }
            catch
            {
                secondCamConnected = false;
            }

            // Камера 2
            try
            {
                framesCaptured3 = 100;
                pnpCamCapture3 = new VideoCapture(2, VideoCapture.API.DShow);
                pnpCamCapture3.Set(CapProp.Autofocus, 0.0f);
                pnpCamCapture3.Set(CapProp.Focus, tbCamFocus.Value);
                SetMaxResolution(2);
                pnpCamCapture3.ExceptionMode = false;
                attempts = 0;
                while (!pnpCamCapture3.IsOpened && attempts < 3)
                {
                    Thread.Sleep(1000);
                    attempts++;
                }
                if (!pnpCamCapture3.IsOpened)
                {
                    throw new Exception("Не удалось обнаружить камеру.");
                }
                pnpCamCapture3.ImageGrabbed += pnpCamCapture3_ImageGrabbed;
                pnpCamCapture3.Start();
                thirdCamConnected = true;
                framesCaptured3 = 100;
            }
            catch
            {
                thirdCamConnected = false;
            }

            // Остальное
            areWeMoving = false;
            dirX = null;
            dirY = null;
            calibBottomLeft = new Vector2(0f, 0f);
            calibTopLeft = new Vector2(0f, 0f);
            calibBottomRight = new Vector2(0f, 0f);
            calibTopRight = new Vector2(0f, 0f);
            pnpAction = 0;
            posBeforePick = new Vector2(0f, 0f);
            ResetComponentSelectionAndProcess();
            cncConnected = true;
            SetAbsPositionLabel();
            SetPanelAnglesLabel();
        }

        public void DeinitCNCMachineConnection()
        {
            try
            {
                grblPort?.Close();
            }
            catch { }
            try
            {
                tcpStream?.Close();
            }
            catch { }
            try
            {
                tcpClient?.Close();
            }
            catch { }
            try
            {
                secondCamConnected = false;
                thirdCamConnected = false;
                cncConnected = false;
            }
            catch { }
            try
            {
                if (pnpCamCapture1 != null) pnpCamCapture1.Stop();
            }
            catch { }
            try
            {
                if (pnpCamCapture2 != null) pnpCamCapture2.Stop();
            }
            catch { }
            try
            {
                if (pnpCamCapture3 != null) pnpCamCapture3.Stop();
            }
            catch { }
            ResetComponentSelectionAndProcess();
            RefreshComponentListViewUI();
        }

        public void ResetComponentSelectionAndProcess()
        {
            selectedComponentId = -1;
            for (int i = 0; i < topComponentList.Count; i++)
            {
                if (topComponentList[i].placeStatus == 1)
                {
                    topComponentList[i].placeStatus = 0;
                }
            }
            for (int i = 0; i < bottomComponentList.Count; i++)
            {
                if (bottomComponentList[i].placeStatus == 1)
                {
                    bottomComponentList[i].placeStatus = 0;
                }
            }
        }

        private void GrblPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String cmdRes = grblPort?.ReadExisting() ?? "";
            if (cmdRes.Length <= 0)
            {
                return;
            }
            if (cmdRes.Contains("$") && cmdRes.Contains("="))
            {
                if (cmdRes.Contains("$100") || cmdRes.Contains("$101") || cmdRes.Contains("$130") || cmdRes.Contains("$131") || cmdRes.Contains("$132") || cmdRes.Contains("$110") || cmdRes.Contains("$111") || cmdRes.Contains("$112") || cmdRes.Contains("$120") || cmdRes.Contains("$121") || cmdRes.Contains("$122"))
                {
                    Thread.Sleep(1000);
                    cmdRes += grblPort?.ReadExisting() ?? "";
                    String[] respLines = cmdRes.ReplaceLineEndings().Split(Environment.NewLine);
                    foreach (String rLine in respLines)
                    {
                        try
                        {
                            if (rLine.StartsWith("$100="))
                            {
                                motionStepsPerMmX = (float)Utils.StrToDouble(rLine.Replace("$100=", ""));
                            }
                            else if (rLine.StartsWith("$101="))
                            {
                                motionStepsPerMmY = (float)Utils.StrToDouble(rLine.Replace("$101=", ""));
                            }
                            else if (rLine.StartsWith("$130="))
                            {
                                maxTravelMmX = (float)Utils.StrToDouble(rLine.Replace("$130=", ""));
                            }
                            else if (rLine.StartsWith("$131="))
                            {
                                maxTravelMmY = (float)Utils.StrToDouble(rLine.Replace("$131=", ""));
                            }
                            else if (rLine.StartsWith("$132="))
                            {
                                maxTravelMmZ = (float)Utils.StrToDouble(rLine.Replace("$132=", ""));
                            }
                            else if (rLine.StartsWith("$110="))
                            {
                                maxFeedRateMmPerMinX = (float)Utils.StrToDouble(rLine.Replace("$110=", ""));
                            }
                            else if (rLine.StartsWith("$111="))
                            {
                                maxFeedRateMmPerMinY = (float)Utils.StrToDouble(rLine.Replace("$111=", ""));
                            }
                            else if (rLine.StartsWith("$112="))
                            {
                                maxFeedRateMmPerMinZ = (float)Utils.StrToDouble(rLine.Replace("$112=", ""));
                            }
                            else if (rLine.StartsWith("$120="))
                            {
                                accXMmPerSecSq = (float)Utils.StrToDouble(rLine.Replace("$120=", ""));
                            }
                            else if (rLine.StartsWith("$121="))
                            {
                                accYMmPerSecSq = (float)Utils.StrToDouble(rLine.Replace("$121=", ""));
                            }
                            else if (rLine.StartsWith("$122="))
                            {
                                accZMmPerSecSq = (float)Utils.StrToDouble(rLine.Replace("$122=", ""));
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowMessageBox("Станок вернул неверный формат настроек $$.");
                            grblCommandCmpl = 2;
                            pnpFault = true;
                            SetAbsPositionLabel();
                            return;
                        }
                    }
                    if (motionStepsPerMmX >= 0f && motionStepsPerMmY >= 0f && maxTravelMmX >= 0f && maxTravelMmY >= 0f && maxTravelMmZ >= 0f && maxFeedRateMmPerMinX >= 0f && maxFeedRateMmPerMinY >= 0f && maxFeedRateMmPerMinZ >= 0f && accXMmPerSecSq >= 0f && accYMmPerSecSq >= 0f && accZMmPerSecSq >= 0f)
                    {
                        grblCommandCmpl = 1;
                    }
                }
            }
            else if (cmdRes.ToLower().Contains("error:"))
            {
                grblCommandCmpl = 2;
                pnpFault = true;
                SetAbsPositionLabel();
                ShowMessageBox("Ошибка станка: " + cmdRes);
            }
            else if (cmdRes.ToLower().Contains("alarm") && !cmdRes.Contains("<"))
            {
                grblCommandCmpl = 2;
                pnpFault = true;
                SetAbsPositionLabel();
                ShowMessageBox("Ошибка станка: " + cmdRes);
            }
            else
            {
                if (grblCommandCmpl != 2)
                {
                    grblCommandCmpl = 1;
                }
            }
        }

        public void GrblSendLine(String cmd)
        {
            try
            {
                grblPort?.WriteLine(cmd);
            }
            catch
            {
                grblCommandCmpl = 2;
                pnpFault = true;
                SetAbsPositionLabel();
            }
        }

        public void MoveToZ(float tgtZMm)
        {
            // Проверить границы
            if (tgtZMm > maxTravelMmZ || tgtZMm < 0f)
            {
                throw new Exception("Движение превышает лимит оси Z.");
            }
            msw.Reset();
            msw.Start();
            grblCommandCmpl = 0;
            GrblSendLine(GetGCodeCommand("G0", 0f, 0f, tgtZMm - absMmZ, 0f));
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                ShowMessageBox("Ошибка станка при перемещении.");
                return;
            }
            msw.Stop();
            int leftToWaitMillis = MillisecToExecuteMove(0f, 0f, tgtZMm - absMmZ, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, maxFeedRateMmPerMinZ) - (int)msw.ElapsedMilliseconds;
            absMmZ = tgtZMm;
            SetAbsPositionLabel();
            if (leftToWaitMillis > 0)
            {
                Thread.Sleep(leftToWaitMillis);
            }
        }

        public void MoveToPlaceZ()
        {
            MoveToZ(placeHeightMm);
        }

        public void MoveToSafeZ()
        {
            if (safeHeightMm < placeHeightMm || safeHeightMm < 1.0f)
            {
                throw new Exception("Неверно указана безопасная высота и/или высота Place.");
            }
            MoveToZ(safeHeightMm);
        }

        public String TcpSendPlace()
        {
            MoveToPlaceZ();
            String res = TcpSendAndPerformJson("vacoff", 0);
            Thread.Sleep(300);
            MoveToSafeZ();
            return res;
        }

        public String TcpSendPick()
        {
            MoveToPlaceZ();
            String res = TcpSendAndPerformJson("vacon", 0);
            Thread.Sleep(300);
            Thread.Sleep(300);
            MoveToSafeZ();
            return res;
        }

        public String TcpSendRotate(int angleSteps)
        {
            return TcpSendAndPerformJson("crotate", (cPositiveClockwise) ? angleSteps : -angleSteps);
        }

        public String TcpSendAndPerformJson(String command, int arg)
        {
            JObject reqObj = new JObject();
            reqObj.Add("command", command);
            reqObj.Add("argument", arg);
            return TcpSendAndPerformJsonRaw(reqObj.ToString());
        }

        public String TcpSendAndPerformJsonRaw(String json)
        {
            bool connFail = false;
            int attempts = 0;
            while (attempts < 3)
            {
                // Реконнект если нужно
                if (connFail || tcpClient?.Connected != true)
                {
                    try
                    {
                        tcpClient?.Close();
                    }
                    catch { }
                    try
                    {
                        tcpClient = new TcpClient();
                        tcpClient.SendTimeout = 2000;
                        tcpClient.ReceiveTimeout = 2000;
                        if (!tcpClient!.ConnectAsync(machineIP, 7008).Wait(2000))
                        {
                            throw new Exception("Тайм-аут TCP-соединения.");
                        }
                        tcpStream = tcpClient?.GetStream();
                        if (tcpStream == null)
                        {
                            throw new Exception("Ошибка при открытии потока TCP-соединения.");
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }

                // Отправить команду
                try
                {
                    tcpStream?.Write(Encoding.UTF8.GetBytes(json));
                }
                catch
                {
                    connFail = true;
                    attempts++;
                    continue;
                }

                // Ждем ответа
                int waitTime = 0;
                while (tcpStream?.DataAvailable != true && waitTime < 1000)
                {
                    Thread.Sleep(10);
                    waitTime++;
                }
                if (waitTime >= 1000)
                {
                    connFail = true;
                    attempts++;
                    continue;
                }

                // Ответ получен, читаем
                byte[] responseData = new byte[512];
                StringBuilder response = new StringBuilder();
                try
                {
                    while (tcpStream?.DataAvailable == true)
                    {
                        int bytes = tcpStream.Read(responseData);
                        response.Append(Encoding.UTF8.GetString(responseData, 0, bytes));
                    }
                }
                catch
                {
                    connFail = true;
                    attempts++;
                    continue;
                }

                // Прочитали?
                return response.ToString();
            }
            pnpFault = true;
            SetAbsPositionLabel();
            throw new Exception("Превышено количество попыток TCP-связи со станком.");
        }

        private void GrblPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            pnpFault = true;
            SetAbsPositionLabel();
        }

        public void pnpCamCapture1_ImageGrabbed(object? sender, EventArgs e)
        {
            pnpCamCapture_ImageGrabbed(sender, e, 0);
        }

        public void pnpCamCapture2_ImageGrabbed(object? sender, EventArgs e)
        {
            pnpCamCapture_ImageGrabbed(sender, e, 1);
        }

        public void pnpCamCapture3_ImageGrabbed(object? sender, EventArgs e)
        {
            pnpCamCapture_ImageGrabbed(sender, e, 2);
        }

        public void pnpCamCapture_ImageGrabbed(object? sender, EventArgs e, int cIdx)
        {
            lock (lockObject)
            {
                bool frameRetrFlag = false;
                if (cIdx == 0)
                {
                    try
                    {
                        frameRetrFlag = pnpCamCapture1?.Retrieve(curRawFrame) ?? false;
                    }
                    catch (Exception ex) { }
                    framesCaptured1 += 1;
                }
                else if (cIdx == 1)
                {
                    try
                    {
                        frameRetrFlag = pnpCamCapture2?.Retrieve(curRawFrame) ?? false;
                    }
                    catch (Exception ex) { }
                    framesCaptured2 += 1;
                }
                else
                {
                    try
                    {
                        frameRetrFlag = pnpCamCapture3?.Retrieve(curRawFrame) ?? false;
                    }
                    catch (Exception ex) { }
                    framesCaptured3 += 1;
                }
                if (frameRetrFlag && cIdx == displayCamIdx)
                {
                    // Угловые линии
                    if (rotateCamImage)
                    {
                        CvInvoke.GetRotationMatrix2D(new PointF((curRawFrame.Cols - 1) / 2.0f, (curRawFrame.Rows - 1) / 2.0f), -imageAngleDeg, 1.0f, transformMatrix);
                        CvInvoke.WarpAffine(curRawFrame, curRotFrame, transformMatrix, new Size(curRawFrame.Cols, curRotFrame.Rows));
                    }

                    // Посчитать зум
                    float nFromX = fovXFromPercent * 0.01f;
                    float nToX = fovXToPercent * 0.01f;
                    float nFromY = fovYFromPercent * 0.01f;
                    float nToY = fovYToPercent * 0.01f;
                    if (nFromX < 0.0f)
                    {
                        nFromX = 0f;
                    }
                    if (nFromX > 1.0f)
                    {
                        nFromX = 1.0f;
                    }
                    if (nToX < 0.0f)
                    {
                        nToX = 0f;
                    }
                    if (nToX > 1.0f)
                    {
                        nToX = 1.0f;
                    }
                    if (nFromY < 0.0f)
                    {
                        nFromY = 0f;
                    }
                    if (nFromY > 1.0f)
                    {
                        nFromY = 1.0f;
                    }
                    if (nToY < 0.0f)
                    {
                        nToY = 0f;
                    }
                    if (nToY > 1.0f)
                    {
                        nToY = 1.0f;
                    }
                    if (nFromX >= nToX)
                    {
                        nFromX = 0f;
                        nToX = 1f;
                    }
                    if (nFromY >= nToY)
                    {
                        nFromY = 0f;
                        nToY = 1f;
                    }
                    int nW = (rotateCamImage) ? curRotFrame.Width : curRawFrame.Width;
                    int nH = (rotateCamImage) ? curRotFrame.Height : curRawFrame.Height;
                    Rectangle roi = new Rectangle((int)(nW * nFromX), (int)(nH * nFromY), (int)(nW * (nToX - nFromX)), (int)(nH * (nToY - nFromY)));
                    roi.Inflate(-(int)((roi.Width / 2.0f) - (roi.Width / (2.0f * zoomFactor))), -(int)((roi.Height / 2.0f) - (roi.Height / (2.0f * zoomFactor))));
                    CvInvoke.Resize(new Mat((rotateCamImage) ? curRotFrame : curRawFrame, roi), (!vFlip && !hFlip) ? curDispFrame : curRszFrame, new Size(pbPnpCamPreview.Width, (int)(((rotateCamImage) ? curRotFrame.Height : curRawFrame.Height) * ((float)pbPnpCamPreview.Width / ((rotateCamImage) ? (float)curRotFrame.Width : (float)curRawFrame.Width)))));

                    // Отразить
                    if (vFlip || hFlip)
                    {
                        if (vFlip && hFlip)
                        {
                            CvInvoke.Flip(curRszFrame, curDispFrame, FlipType.Both);
                        }
                        else
                        {
                            CvInvoke.Flip(curRszFrame, curDispFrame, (vFlip) ? FlipType.Vertical : FlipType.Horizontal);
                        }
                    }

                    // Контуры
                    if (showCV)
                    {
                        CvInvoke.CvtColor(curDispFrame, curDGrayFrame, ColorConversion.Bgr2Gray);
                        CvInvoke.GaussianBlur(curDGrayFrame, curGrayBlurredFrame, new Size(3, 3), 0);
                        CvInvoke.Threshold(curGrayBlurredFrame, curThreshFrame, 85, 255, ThresholdType.Binary);
                        Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                        Mat hier = new Mat();
                        CvInvoke.FindContours(curThreshFrame, contours, hier, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
                        if (contours.Size > 1)
                        {
                            for (int i = 1; i < Math.Min(10, contours.Size); i++)
                            {
                                // Цвет
                                MCvScalar clr = new MCvScalar(0, 0, 0);
                                if (i == 1)
                                {
                                    clr = new MCvScalar(0, 0, 255);
                                }
                                else if (i == 2)
                                {
                                    clr = new MCvScalar(0, 255, 255);
                                }
                                else if (i == 3)
                                {
                                    clr = new MCvScalar(0, 255, 0);
                                }
                                else if (i == 4)
                                {
                                    clr = new MCvScalar(255, 255, 0);
                                }
                                else
                                {
                                    clr = new MCvScalar(255, 0, 0);
                                }

                                // Размер
                                double perimeter = CvInvoke.ArcLength(contours[i], true);
                                if (perimeter < 100f)
                                {
                                    continue;
                                }

                                // Центр
                                var moments = CvInvoke.Moments(contours[i]);
                                int x = (int)(moments.M10 / moments.M00);
                                int y = (int)(moments.M01 / moments.M00);
                                CvInvoke.Circle(curDispFrame, new Point(x, y), 3, clr, 10);
                                CvInvoke.DrawContours(curDispFrame, contours, i, clr, 4);

                                // Это прямоугольник? Считаем угол.
                                VectorOfPoint approx = new VectorOfPoint();
                                CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);
                                if (approx.Size == 4)
                                {
                                    Vector2 gen1 = Vector2.Subtract(new Vector2(approx[0].X, approx[0].Y), new Vector2(approx[2].X, approx[2].Y));
                                    Vector2 gen2 = Vector2.Subtract(new Vector2(approx[1].X, approx[1].Y), new Vector2(approx[3].X, approx[3].Y));
                                    float angleRad = (float)Math.Atan2(gen2.Y - gen1.Y, gen2.X - gen1.X);
                                    float angDegrees = (180f / 3.14159265f) * angleRad;
                                    CvInvoke.PutText(curDispFrame, angDegrees.ToString("#.##") + " deg", approx[0], FontFace.HersheyPlain, 3f, clr, 4);
                                }
                            }
                        }
                    }

                    // Отобразить линии осей согласно посчитанным углам поверх цветного без искажения
                    Point xLineStart = new Point(0, (int)(curDispFrame.Height / 2f));
                    Point xLineEnd = new Point(curDispFrame.Width, (int)(curDispFrame.Height / 2f));
                    Point yLineStart = new Point((int)(curDispFrame.Width / 2f), 0);
                    Point yLineEnd = new Point((int)(curDispFrame.Width / 2f), curDispFrame.Height);
                    CvInvoke.Line(curDispFrame, xLineStart, xLineEnd, new MCvScalar(0, 0.0f, 255.0f));
                    CvInvoke.Line(curDispFrame, yLineStart, yLineEnd, new MCvScalar(255.0f, 0.0f, 0.0f));

                    // Отобразить штрихи расстояния
                    int xHalfPoint = (int)(curDispFrame.Width / 2f);
                    int yHalfPoint = (int)(curDispFrame.Height / 2f);
                    float effPxPerMm = imagePxPerMm * zoomFactor;
                    for (float x = xHalfPoint; x > 0f; x -= effPxPerMm)
                    {
                        CvInvoke.Line(curDispFrame, new Point((int)x, (int)(yHalfPoint - 10f)), new Point((int)x, (int)(yHalfPoint + 10f)), new MCvScalar(0, 0.0f, 255.0f));
                    }
                    for (float x = xHalfPoint; x < curDispFrame.Width; x += effPxPerMm)
                    {
                        CvInvoke.Line(curDispFrame, new Point((int)x, (int)(yHalfPoint - 10f)), new Point((int)x, (int)(yHalfPoint + 10f)), new MCvScalar(0, 0.0f, 255.0f));
                    }
                    for (float y = yHalfPoint; y < curDispFrame.Height; y += effPxPerMm)
                    {
                        CvInvoke.Line(curDispFrame, new Point((int)(xHalfPoint - 10f), (int)y), new Point((int)(xHalfPoint + 10f), (int)y), new MCvScalar(255.0f, 0.0f, 0.0f));
                    }
                    for (float y = yHalfPoint; y > 0f; y -= effPxPerMm)
                    {
                        CvInvoke.Line(curDispFrame, new Point((int)(xHalfPoint - 10f), (int)y), new Point((int)(xHalfPoint + 10f), (int)y), new MCvScalar(255.0f, 0.0f, 0.0f));
                    }

                    // Отобразить прямоугольник
                    if (rectWidthMm > 0f && rectHeightMm > 0f && showRect)
                    {
                        Rectangle rect = new Rectangle((int)(xHalfPoint - ((rectWidthMm * effPxPerMm) / 2f)), (int)(yHalfPoint - ((rectHeightMm * effPxPerMm) / 2f)), (int)(rectWidthMm * effPxPerMm), (int)(rectHeightMm * effPxPerMm));
                        CvInvoke.Rectangle(curDispFrame, rect, new MCvScalar(0, 255, 255));
                    }

                    // Отобразить
                    Image? oldIm = null;
                    if (pbPnpCamPreview.Image != null)
                    {
                        oldIm = pbPnpCamPreview.Image;
                    }
                    try
                    {
                        if (curDispFrame != null)
                        {
                            pbPnpCamPreview.Image = curDispFrame.ToBitmap();
                            oldIm?.Dispose();
                        }
                    }
                    catch (Exception ex) { }
                }
            }
        }

        public String GetGCodeCommand(String cmd, float mX, float mY, float mZ, float feedRate)
        {
            // Компенсация кривизны станка
            Vector2 compensatedX = Vector2.Transform(new Vector2(mX, 0f), Matrix3x2.CreateRotation(motionSkewAngleRad * 0.5f));
            Vector2 compensatedY = Vector2.Transform(new Vector2(0f, mY), Matrix3x2.CreateRotation(-motionSkewAngleRad * 0.5f));
            Vector2 vsum = compensatedX + compensatedY;
            float qX = (xPlusIsRight) ? vsum.X : -vsum.X;
            float qY = (yPlusIsDown) ? vsum.Y : -vsum.Y;
            float qZ = (zPlusIsWithdraw) ? mZ : -mZ;

            // Компенсация backlash X
            if (backlashMmX > 0.0f)
            {
                if (dirX == null)
                {
                    dirX = qX > 0f;
                }
                else
                {
                    if (qX > 0f)
                    {
                        // Напавление положительное
                        if (dirX == false)
                        {
                            // С отрицательного на положительное
                            qX += backlashMmX;
                            dirX = true;
                        }
                    }
                    else if (qX < 0f)
                    {
                        // Направление отрицательное
                        if (dirX == true)
                        {
                            // С положительного на отрицательное
                            qX -= backlashMmX;
                            dirX = false;
                        }
                    }
                }
            }

            // Компенсация backlash Y
            if (backlashMmY > 0.0f)
            {
                if (dirY == null)
                {
                    dirY = qY > 0f;
                }
                else
                {
                    if (qY > 0f)
                    {
                        // Напавление положительное
                        if (dirY == false)
                        {
                            // С отрицательного на положительное
                            qY += backlashMmY;
                            dirY = true;
                        }
                    }
                    else if (qY < 0f)
                    {
                        // Направление отрицательное
                        if (dirY == true)
                        {
                            // С положительного на отрицательное
                            qY -= backlashMmY;
                            dirY = false;
                        }
                    }
                }
            }

            // Формулировка команды
            String xPart = "";
            String yPart = "";
            String zPart = "";
            if (qX != 0.0f)
            {
                xPart = "X" + qX.ToString("0.###");
            }
            if (qY != 0.0f)
            {
                yPart = "Y" + qY.ToString("0.###");
            }
            if (qZ != 0.0f)
            {
                zPart = "Z" + qZ.ToString("0.###");
            }
            String coordPart = xPart + " " + yPart + " " + zPart;
            String fPart = "";
            if (feedRate > 0.0f)
            {
                fPart = "F" + feedRate.ToString("0");
            }
            if (coordPart.Length > 0)
            {
                if (fPart.Length > 0)
                {
                    return cmd + " " + coordPart + " " + fPart;
                }
                else
                {
                    return cmd + " " + coordPart;
                }
            }
            else
            {
                return "G91";
            }
        }

        private void SaveBacklash()
        {
            if (updatingSettingsUI) return;
            backlashMmX = (float)nudBacklashMmX.Value;
            backlashMmY = (float)nudBacklashMmY.Value;
            FileStorage storage = new FileStorage("pnp_backlash.xml", FileStorage.Mode.Write);
            storage.Write(backlashMmX, "backlash_mm_x");
            storage.Write(backlashMmY, "backlash_mm_y");
            storage.ReleaseAndGetString();
        }

        public void SaveMachineIP()
        {
            if (updatingSettingsUI) return;
            machineIP = tbMachineIP.Text;
            FileStorage storage = new FileStorage("pnp_net.xml", FileStorage.Mode.Write);
            storage.Write(machineIP, "ip");
            storage.ReleaseAndGetString();
        }

        public void SavePNPCal()
        {
            if (updatingSettingsUI) return;
            nozzleRelMmX = (float)nudNozzleRelMmX.Value;
            nozzleRelMmY = (float)nudNozzleRelMmY.Value;
            placeHeightMm = (float)nudPlaceHeightMm.Value;
            safeHeightMm = (float)nudSafeHeightMm.Value;
            cstepsPerSeq = (int)nudCStepsPerSeq.Value;
            stepsPerCTurn = (int)nudStepsPerCTurn.Value;
            cPositiveClockwise = cbCPositiveClockwise.Checked;
            addAngleDeg = (float)nudAddAngleDeg.Value;
            FileStorage storage = new FileStorage("pnp_cal.xml", FileStorage.Mode.Write);
            storage.Write(nozzleRelMmX, "nozzle_rel_mm_x");
            storage.Write(nozzleRelMmY, "nozzle_rel_mm_y");
            storage.Write(placeHeightMm, "place_height_mm");
            storage.Write(safeHeightMm, "safe_height_mm");
            storage.Write(cstepsPerSeq, "c_steps_per_seq");
            storage.Write(stepsPerCTurn, "steps_per_c_turn");
            storage.Write(cPositiveClockwise ? 1 : 0, "c_positive_clockwise");
            storage.Write(addAngleDeg, "add_angle_deg");
            storage.ReleaseAndGetString();
        }

        public void SaveFavLoc()
        {
            if (updatingSettingsUI) return;
            favLoc1 = new PointF((float)nudLoc1XMm.Value, (float)nudLoc1YMm.Value);
            favLoc2 = new PointF((float)nudLoc2XMm.Value, (float)nudLoc2YMm.Value);
            favLoc3 = new PointF((float)nudLoc3XMm.Value, (float)nudLoc3YMm.Value);
            favLoc4 = new PointF((float)nudLoc4XMm.Value, (float)nudLoc4YMm.Value);
            favLoc5 = new PointF((float)nudLoc5XMm.Value, (float)nudLoc5YMm.Value);
            favLoc6 = new PointF((float)nudLoc6XMm.Value, (float)nudLoc6YMm.Value);
            favLoc7 = new PointF((float)nudLoc7XMm.Value, (float)nudLoc7YMm.Value);
            favLoc8 = new PointF((float)nudLoc8XMm.Value, (float)nudLoc8YMm.Value);
            favLoc9 = new PointF((float)nudLoc9XMm.Value, (float)nudLoc9YMm.Value);
            favLoc10 = new PointF((float)nudLoc10XMm.Value, (float)nudLoc10YMm.Value);
            FileStorage storage = new FileStorage("pnp_fav.xml", FileStorage.Mode.Write);
            storage.Write(favLoc1.X, "fav_loc_1_x_mm");
            storage.Write(favLoc1.Y, "fav_loc_1_y_mm");
            storage.Write(favLoc2.X, "fav_loc_2_x_mm");
            storage.Write(favLoc2.Y, "fav_loc_2_y_mm");
            storage.Write(favLoc3.X, "fav_loc_3_x_mm");
            storage.Write(favLoc3.Y, "fav_loc_3_y_mm");
            storage.Write(favLoc4.X, "fav_loc_4_x_mm");
            storage.Write(favLoc4.Y, "fav_loc_4_y_mm");
            storage.Write(favLoc5.X, "fav_loc_5_x_mm");
            storage.Write(favLoc5.Y, "fav_loc_5_y_mm");
            storage.Write(favLoc6.X, "fav_loc_6_x_mm");
            storage.Write(favLoc6.Y, "fav_loc_6_y_mm");
            storage.Write(favLoc7.X, "fav_loc_7_x_mm");
            storage.Write(favLoc7.Y, "fav_loc_7_y_mm");
            storage.Write(favLoc8.X, "fav_loc_8_x_mm");
            storage.Write(favLoc8.Y, "fav_loc_8_y_mm");
            storage.Write(favLoc9.X, "fav_loc_9_x_mm");
            storage.Write(favLoc9.Y, "fav_loc_9_y_mm");
            storage.Write(favLoc10.X, "fav_loc_10_x_mm");
            storage.Write(favLoc10.Y, "fav_loc_10_y_mm");
            storage.ReleaseAndGetString();
        }

        public void SaveImageSettings()
        {
            if (updatingSettingsUI) return;
            imageAngleDeg = (float)nudImageAngleDeg.Value;
            hFlip = cbImageFlipH.Checked;
            vFlip = cbImageFlipV.Checked;
            rotateCamImage = cbImageShowAngleGrid.Checked;
            showCV = cbImageShowShapes.Checked;
            rectWidthMm = (float)nudRectWidthMm.Value;
            rectHeightMm = (float)nudRectHeightMm.Value;
            imagePxPerMm = (float)nudImagePxPerMm.Value;
            showRect = cbShowRect.Checked;
            fovXFromPercent = (int)nudFOVXFrom.Value;
            fovXToPercent = (int)nudFOVXTo.Value;
            fovYFromPercent = (int)nudFOVYFrom.Value;
            fovYToPercent = (int)nudFOVYTo.Value;
            FileStorage storage = new FileStorage("pnp_image.xml", FileStorage.Mode.Write);
            storage.Write(hFlip ? 1 : 0, "h_flip");
            storage.Write(vFlip ? 1 : 0, "v_flip");
            storage.Write(rotateCamImage ? 1 : 0, "rotate_cam_image");
            storage.Write(showCV ? 1 : 0, "show_cv");
            storage.Write(imageAngleDeg, "image_angle_deg");
            storage.Write(rectWidthMm, "rect_width_mm");
            storage.Write(rectHeightMm, "rect_height_mm");
            storage.Write(imagePxPerMm, "image_px_per_mm");
            storage.Write(showRect ? 1 : 0, "show_rect");
            storage.Write(fovXFromPercent, "fov_x_from");
            storage.Write(fovXToPercent, "fov_x_to");
            storage.Write(fovYFromPercent, "fov_y_from");
            storage.Write(fovYToPercent, "fov_y_to");
            storage.ReleaseAndGetString();
        }

        public void SaveMachineAxes()
        {
            if (updatingSettingsUI) return;
            if (rbXYTopLeft.Checked)
            {
                homingCorner = new Vector3(0f, 0f, 1f);
            }
            else if (rbXYTopRight.Checked)
            {
                homingCorner = new Vector3(1f, 0f, 1f);
            }
            else if (rbXYBottomLeft.Checked)
            {
                homingCorner = new Vector3(0f, 1f, 1f);
            }
            else if (rbXYBottomRight.Checked)
            {
                homingCorner = new Vector3(1f, 1f, 1f);
            }
            xPlusIsRight = rbXRight.Checked;
            yPlusIsDown = rbYDown.Checked;
            zPlusIsWithdraw = rbZAwayFromWorkpiece.Checked;
            FileStorage storage = new FileStorage("pnp_axes.xml", FileStorage.Mode.Write);
            storage.Write(xPlusIsRight ? 1 : 0, "x_plus_is_right");
            storage.Write(yPlusIsDown ? 1 : 0, "y_plus_is_down");
            storage.Write(zPlusIsWithdraw ? 1 : 0, "z_plus_is_withdraw");
            storage.ReleaseAndGetString();
        }

        private void PNPForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsMachining() || IsComponentPNPing())
            {
                e.Cancel = true;
            }
            else
            {
                DeinitCNCMachineConnection();
                UpdateConnectionUIState();
            }
        }

        private void tbZoom_ValueChanged(object sender, EventArgs e)
        {
            zoomFactor = tbZoom.Value;
        }

        private void move_Click(object sender, EventArgs e)
        {
            // Проверка
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Станок в работе над вашим Gerber-файлом, функция недоступна.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Движение уже идет.");
                return;
            }

            // Подключено, можем выполнять движение согласно параметрам
            float mmToMove = (float)Math.Pow(10.0f, tbMoveAmount.Value);

            // Отправить команду на станок
            float qX = 0.0f;
            float qY = 0.0f;
            float qZ = 0.0f;
            if (((Button)sender).Tag == btnXPos.Tag)
            {
                qX = mmToMove;
            }
            else if (((Button)sender).Tag == btnXNeg.Tag)
            {
                qX = -mmToMove;
            }
            else if (((Button)sender).Tag == btnYPos.Tag)
            {
                qY = -mmToMove;
            }
            else if (((Button)sender).Tag == btnYNeg.Tag)
            {
                qY = mmToMove;
            }
            else if (((Button)sender).Tag == btnZPos.Tag)
            {
                qZ = mmToMove;
            }
            else if (((Button)sender).Tag == btnZNeg.Tag)
            {
                qZ = -mmToMove;
            }
            else
            {
                ShowMessageBox("Непонятное движение.");
                return;
            }

            // Проверить границы
            if ((absMmX + qX) > maxTravelMmX || (absMmY + qY) > maxTravelMmY || (absMmZ + qZ) > maxTravelMmZ || (absMmX + qX) < 0f || (absMmY + qY) < 0f || (absMmZ + qZ) < 0f)
            {
                ShowMessageBox("Движение превышает лимит оси.");
                return;
            }
            msw.Reset();
            msw.Start();
            grblCommandCmpl = 0;
            GrblSendLine(GetGCodeCommand("G0", qX, qY, qZ, 0f));
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                ShowMessageBox("Ошибка станка при перемещении.");
                return;
            }
            absMmX += qX;
            absMmY += qY;
            absMmZ += qZ;
            SetAbsPositionLabel();
            msw.Stop();
            int leftToWaitMillis = MillisecToExecuteMove(qX, qY, qZ, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, maxFeedRateMmPerMinZ) - (int)msw.ElapsedMilliseconds;
            if (leftToWaitMillis > 0)
            {
                areWeMoving = true;
                SetMoving();
                motionFinishWaitThread = new Thread(new ThreadStart(delegate
                {
                    Thread.Sleep(leftToWaitMillis);
                    areWeMoving = false;
                    SetMoving();
                }));
                motionFinishWaitThread?.Start();
            }
        }

        private void btnApplyMotion_Click(object sender, EventArgs e)
        {
            // Проверка
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Идет обработка заготовки, функция недоступна.");
                return;
            }

            // Установить
            motionStepsPerMmX = (float)nudStepsPerMmX.Value;
            motionStepsPerMmY = (float)nudStepsPerMmY.Value;
            motionSkewAngleRad = (float)nudSkewAngleRad.Value;

            // Сохранить изменения в файл
            FileStorage storage = new FileStorage("pnp_motion.xml", FileStorage.Mode.Write);
            storage.Write(motionStepsPerMmX, "x_steps_per_mm");
            storage.Write(motionStepsPerMmY, "y_steps_per_mm");
            storage.Write(motionSkewAngleRad, "skew_angle_rad");
            storage.ReleaseAndGetString();

            // Записать X в станок
            grblCommandCmpl = 0;
            GrblSendLine("$100=" + motionStepsPerMmX.ToString("0.000"));
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                ShowMessageBox("Ошибка станка при записи параметров, начните калибровку заново.");
                return;
            }

            // Записать Y в станок
            grblCommandCmpl = 0;
            GrblSendLine("$101=" + motionStepsPerMmY.ToString("0.000"));
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                ShowMessageBox("Ошибка станка при записи параметров, начните калибровку заново.");
                return;
            }

            // Готово
            SetMotionCompensateLabel();
            ShowMessageBox("Новые параметры записаны в станок и сохранены.");
        }

        private void nudBacklashX_ValueChanged(object sender, EventArgs e)
        {
            SaveBacklash();
        }

        private void nudBacklashY_ValueChanged(object sender, EventArgs e)
        {
            SaveBacklash();
        }

        private void rbXYTopLeft_CheckedChanged(object sender, EventArgs e)
        {
            SaveMachineAxes();
        }

        private void rbXYTopRight_CheckedChanged(object sender, EventArgs e)
        {
            SaveMachineAxes();
        }

        private void rbXYBottomLeft_CheckedChanged(object sender, EventArgs e)
        {
            SaveMachineAxes();
        }

        private void rbXYBottomRight_CheckedChanged(object sender, EventArgs e)
        {
            SaveMachineAxes();
        }

        private void rbXRight_CheckedChanged(object sender, EventArgs e)
        {
            SaveMachineAxes();
        }

        private void rbXLeft_CheckedChanged(object sender, EventArgs e)
        {
            SaveMachineAxes();
        }

        private void rbYDown_CheckedChanged(object sender, EventArgs e)
        {
            SaveMachineAxes();
        }

        private void rbYUp_CheckedChanged(object sender, EventArgs e)
        {
            SaveMachineAxes();
        }

        public int MillisecToExecuteMove(float deltaX, float deltaY, float deltaZ, float fX, float fY, float fZ)
        {
            return Utils.MillisecToExecuteAxisMove(deltaX, fX, accXMmPerSecSq) + Utils.MillisecToExecuteAxisMove(deltaY, fY, accYMmPerSecSq) + Utils.MillisecToExecuteAxisMove(deltaZ, fZ, accZMmPerSecSq);
        }

        private void tbMachineIP_TextChanged(object sender, EventArgs e)
        {
            SaveMachineIP();
        }

        private void btnLight_Click(object sender, EventArgs e)
        {
            // Проверка
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Станок в работе над вашим Gerber-файлом, функция недоступна.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Движение уже идет.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, функция недоступна.");
                return;
            }

            // Выполнить
            try
            {
                if (lightOn)
                {
                    TcpSendAndPerformJson("lightoff", 0);
                    lightOn = false;
                }
                else
                {
                    TcpSendAndPerformJson("lighton", 0);
                    lightOn = true;
                }
            }
            catch (Exception exc)
            {
                ShowMessageBox(exc.Message);
            }
        }

        private void nudNozzleX_ValueChanged(object sender, EventArgs e)
        {
            SavePNPCal();
            SetAbsPositionLabel();
        }

        private void nudNozzleY_ValueChanged(object sender, EventArgs e)
        {
            SavePNPCal();
            SetAbsPositionLabel();
        }

        private void tbCamFocus_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cncConnected)
                {
                    pnpCamCapture1?.Set(CapProp.Focus, tbCamFocus.Value);
                }
            }
            catch { }
            try
            {
                if (cncConnected && secondCamConnected)
                {
                    pnpCamCapture2?.Set(CapProp.Focus, tbCamFocus.Value);
                }
            }
            catch { }
            try
            {
                if (cncConnected && thirdCamConnected)
                {
                    pnpCamCapture3?.Set(CapProp.Focus, tbCamFocus.Value);
                }
            }
            catch { }
        }

        private void btnPrintCalDot_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintCalDot;
            PrintDialog pdi = new PrintDialog();
            pdi.Document = pd;
            if (pdi.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void Pd_PrintCalDot(object sender, PrintPageEventArgs e)
        {
            e.HasMorePages = false;
            float oneMmInPrintUnits = 100.0f / 25.4f;
            float width = 0.7f * oneMmInPrintUnits;
            float height = 0.7f * oneMmInPrintUnits;
            float left = e.PageBounds.Left + (e.PageBounds.Width / 2.0f) - (width / 2.0f);
            float top = e.PageBounds.Top + (e.PageBounds.Height / 2.0f) - (height / 2.0f);
            e.Graphics?.FillEllipse(new SolidBrush(Color.Red), new RectangleF(left, top, width, height));
        }

        public void SetPNPCalSettingsEnabled(bool flag)
        {
            if (InvokeRequired)
            {
                Invoke(SetPNPCalSettingsEnabled, flag);
            }
            else
            {
                nudStepsPerMmX.Enabled = flag;
                nudStepsPerMmY.Enabled = flag;
                nudSkewAngleRad.Enabled = flag;
                nudBacklashMmX.Enabled = flag;
                nudBacklashMmY.Enabled = flag;
                nudPlaceHeightMm.Enabled = flag;
                nudSafeHeightMm.Enabled = flag;
                nudNozzleRelMmX.Enabled = flag;
                nudNozzleRelMmY.Enabled = flag;
                nudCStepsPerSeq.Enabled = flag;
                nudStepsPerCTurn.Enabled = flag;
                nudAddAngleDeg.Enabled = flag;
                cbCPositiveClockwise.Enabled = flag;
                btnCalibPanelBottomLeft.Enabled = flag;
                btnCalibPanelBottomRight.Enabled = flag;
                btnCalibPanelTopLeft.Enabled = flag;
                btnCalibPanelTopRight.Enabled = flag;
                btnCalibPanelCalculate.Enabled = flag;
            }
        }

        public void SetPNPParamsEnabled(bool flag)
        {
            if (InvokeRequired)
            {
                Invoke(SetPNPParamsEnabled, flag);
            }
            else
            {
                btnCTopLeft.Enabled = flag;
                btnCTopRight.Enabled = flag;
                btnCBottomLeft.Enabled = flag;
                btnCBottomRight.Enabled = flag;
                btnPick.Enabled = flag;
                btnRotateLeft.Enabled = flag;
                btnRotateRight.Enabled = flag;
                btnPlace.Enabled = flag;
                btnRevert.Enabled = flag;
                nudCmpRotAngle.Enabled = flag;
                lvComponents.Enabled = flag;
                btnPnpPreview.Enabled = flag;
                rbSideTop.Enabled = flag;
                rbSideBottom.Enabled = flag;
            }
        }

        public void SetPanelAnglesLabel()
        {
            if (InvokeRequired)
            {
                Invoke(SetPanelAnglesLabel);
            }
            else
            {
                String anglesStr = "";
                if (dCorner1Mm.Length() > 0f && dCorner2Mm.Length() > 0f)
                {
                    anglesStr += "Левый верхний угол X: " + dCorner1Mm.X.ToString("0.00") + ", Y: " + dCorner1Mm.Y.ToString("0.00") + ", правый верхний угол X: " + dCorner2Mm.X.ToString("0.00") + ", Y: " + dCorner2Mm.Y.ToString("0.00") + "\n";
                }
                if (dCorner3Mm.Length() > 0f && dCorner4Mm.Length() > 0f)
                {
                    anglesStr += "Левый нижний угол X: " + dCorner3Mm.X.ToString("0.00") + ", Y: " + dCorner3Mm.Y.ToString("0.00") + ", правый нижний угол X: " + dCorner4Mm.X.ToString("0.00") + ", Y: " + dCorner4Mm.Y.ToString("0.00") + "\n";
                }
                if (dCorner1Mm.Length() > 0f && dCorner2Mm.Length() > 0f)
                {
                    float angDegrees = (180f / 3.14159265f) * GetPanelRotAngleRad();
                    anglesStr += "Панель повернута на " + angDegrees.ToString("0.000") + "°";
                }
                lblPanelPosition.Text = anglesStr;
                btnCalibPanelTopLeft.ForeColor = (calibTopLeft.Length() >= 1f) ? Color.Green : Color.Black;
                btnCalibPanelTopRight.ForeColor = (calibTopRight.Length() >= 1f) ? Color.Green : Color.Black;
                btnCalibPanelBottomLeft.ForeColor = (calibBottomLeft.Length() >= 1f) ? Color.Green : Color.Black;
                btnCalibPanelBottomRight.ForeColor = (calibBottomRight.Length() >= 1f) ? Color.Green : Color.Black;
            }
        }

        private void btnCTopLeft_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Станок в работе над вашим Gerber-файлом, функция недоступна.");
                return;
            }
            dCorner1Mm = new Vector2(absMmX, absMmY);
            SetPanelAnglesLabel();
        }

        private void btnCTopRight_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Станок в работе над вашим Gerber-файлом, функция недоступна.");
                return;
            }
            dCorner2Mm = new Vector2(absMmX, absMmY);
            SetPanelAnglesLabel();
        }

        private void camWatchdogTimer_Tick(object sender, EventArgs e)
        {
            if (cncConnected)
            {
                if (framesCaptured1 > 3)
                {
                    framesCaptured1 = 0;
                }
                else
                {
                    // Reconnect
                    try
                    {
                        pnpCamCapture1?.Stop();
                    }
                    catch { }
                    try
                    {
                        pnpCamCapture1 = new VideoCapture(0, VideoCapture.API.DShow);
                        pnpCamCapture1.Set(CapProp.Autofocus, 0.0f);
                        pnpCamCapture1.Set(CapProp.Focus, tbCamFocus.Value);
                        SetMaxResolution(0);
                        pnpCamCapture1.ExceptionMode = false;
                        int attempts = 0;
                        while (!pnpCamCapture1.IsOpened && attempts < 3)
                        {
                            Thread.Sleep(500);
                            attempts++;
                        }
                        if (!pnpCamCapture1.IsOpened)
                        {
                            throw new Exception("Не удалось обнаружить камеру.");
                        }
                        pnpCamCapture1.ImageGrabbed += pnpCamCapture1_ImageGrabbed;
                        pnpCamCapture1.Start();
                    }
                    catch
                    {
                        pnpFault = true;
                        SetAbsPositionLabel();
                        return;
                    }
                }
                if (secondCamConnected)
                {
                    if (framesCaptured2 > 3)
                    {
                        framesCaptured2 = 0;
                    }
                    else
                    {
                        // Reconnect
                        try
                        {
                            pnpCamCapture2?.Stop();
                        }
                        catch { }
                        try
                        {
                            pnpCamCapture2 = new VideoCapture(1, VideoCapture.API.DShow);
                            pnpCamCapture2.Set(CapProp.Autofocus, 0.0f);
                            pnpCamCapture2.Set(CapProp.Focus, tbCamFocus.Value);
                            SetMaxResolution(1);
                            pnpCamCapture2.ExceptionMode = false;
                            int attempts = 0;
                            while (!pnpCamCapture2.IsOpened && attempts < 3)
                            {
                                Thread.Sleep(500);
                                attempts++;
                            }
                            if (!pnpCamCapture2.IsOpened)
                            {
                                throw new Exception("Не удалось обнаружить камеру 2.");
                            }
                            pnpCamCapture2.ImageGrabbed += pnpCamCapture2_ImageGrabbed;
                            pnpCamCapture2.Start();
                        }
                        catch
                        {
                            pnpFault = true;
                            SetAbsPositionLabel();
                            return;
                        }
                    }
                }
                if (thirdCamConnected)
                {
                    if (framesCaptured3 > 3)
                    {
                        framesCaptured3 = 0;
                    }
                    else
                    {
                        // Reconnect
                        try
                        {
                            pnpCamCapture3?.Stop();
                        }
                        catch { }
                        try
                        {
                            pnpCamCapture3 = new VideoCapture(2, VideoCapture.API.DShow);
                            pnpCamCapture3.Set(CapProp.Autofocus, 0.0f);
                            pnpCamCapture3.Set(CapProp.Focus, tbCamFocus.Value);
                            SetMaxResolution(2);
                            pnpCamCapture3.ExceptionMode = false;
                            int attempts = 0;
                            while (!pnpCamCapture3.IsOpened && attempts < 3)
                            {
                                Thread.Sleep(500);
                                attempts++;
                            }
                            if (!pnpCamCapture3.IsOpened)
                            {
                                throw new Exception("Не удалось обнаружить камеру 3.");
                            }
                            pnpCamCapture3.ImageGrabbed += pnpCamCapture3_ImageGrabbed;
                            pnpCamCapture3.Start();
                        }
                        catch
                        {
                            pnpFault = true;
                            SetAbsPositionLabel();
                            return;
                        }
                    }
                }
            }
        }

        private void nudPlaceHeightMm_ValueChanged(object sender, EventArgs e)
        {
            SavePNPCal();
            SetAbsPositionLabel();
        }

        private void nudCalibGridAngleDeg_ValueChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void nudCStepsPerSeq_ValueChanged(object sender, EventArgs e)
        {
            SavePNPCal();
        }

        private void nudStepsPerCTurn_ValueChanged(object sender, EventArgs e)
        {
            SavePNPCal();
        }

        public void CAxisCalibProcess()
        {
            try
            {
                // Идет обработка
                SetPNPCalSettingsEnabled(false);
                SetPNPParamsEnabled(false);
                SetCalibrationUIEnabled(false);
                shouldMachiningStop = false;

                // Инициализация движения
                Stopwatch sw = new Stopwatch();
                float feedRateMmPerMin = Math.Min(maxFeedRateMmPerMinX, maxFeedRateMmPerMinY);

                // Перемещаем Z в median
                MoveToSafeZ();

                // Переместить каретку так чтобы сопло PNP оказалось там где камера
                float tgtPosX = absMmX - nozzleRelMmX;
                float tgtPosY = absMmY - nozzleRelMmY;
                sw.Reset();
                sw.Start();
                grblCommandCmpl = 0;
                GrblSendLine(GetGCodeCommand("G0", tgtPosX - absMmX, tgtPosY - absMmY, 0f, feedRateMmPerMin));
                while (grblCommandCmpl < 1)
                {
                    Thread.Sleep(10);
                }
                if (grblCommandCmpl >= 2)
                {
                    throw new Exception("Ошибка станка при перемещении в начало панели.");
                }
                sw.Stop();
                int leftToWaitMillis = MillisecToExecuteMove(tgtPosX - absMmX, tgtPosY - absMmY, 0f, feedRateMmPerMin, feedRateMmPerMin, feedRateMmPerMin) - (int)sw.ElapsedMilliseconds;
                absMmX = tgtPosX;
                absMmY = tgtPosY;
                SetAbsPositionLabel();
                if (leftToWaitMillis > 0)
                {
                    Thread.Sleep(leftToWaitMillis);
                }

                // Dwell
                Thread.Sleep(300);

                // Pick
                TcpSendPick();

                // Dwell
                Thread.Sleep(300);

                // Rotate
                TcpSendRotate(cstepsPerSeq);

                // Dwell
                Thread.Sleep(300);

                // Place
                TcpSendPlace();

                // Dwell
                Thread.Sleep(300);

                // Переместить камеру обратно
                float bkPosX = absMmX + nozzleRelMmX;
                float bkPosY = absMmY + nozzleRelMmY;
                sw.Reset();
                sw.Start();
                grblCommandCmpl = 0;
                GrblSendLine(GetGCodeCommand("G0", bkPosX - absMmX, bkPosY - absMmY, 0f, feedRateMmPerMin));
                while (grblCommandCmpl < 1)
                {
                    Thread.Sleep(10);
                }
                if (grblCommandCmpl >= 2)
                {
                    throw new Exception("Ошибка станка при перемещении в начало панели.");
                }
                sw.Stop();
                leftToWaitMillis = MillisecToExecuteMove(bkPosX - absMmX, bkPosY - absMmY, 0f, feedRateMmPerMin, feedRateMmPerMin, feedRateMmPerMin) - (int)sw.ElapsedMilliseconds;
                absMmX = bkPosX;
                absMmY = bkPosY;
                SetAbsPositionLabel();
                if (leftToWaitMillis > 0)
                {
                    Thread.Sleep(leftToWaitMillis);
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
            finally
            {
                // Обработка завершена
                SetPNPCalSettingsEnabled(true);
                SetPNPParamsEnabled(true);
                SetCalibrationUIEnabled(true);
                SetAfterCAxisCalibButtonState();
            }
        }

        private void btnCCalib_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (btnCCalib.Tag == null)
            {
                if (IsMachining() || IsComponentPNPing())
                {
                    ShowMessageBox("Идет другой процесс обработки либо компонент в работе.");
                    return;
                }
                machiningThread = new Thread(CAxisCalibProcess);
                machiningThread.Start();
                btnCCalib.Tag = 1;
                btnCCalib.Text = "...";
                btnCCalib.Enabled = false;
                return;
            }
            else
            {
                ShowMessageBox("C-калибровка уже идет.");
                return;
            }
        }

        public void SetAfterCAxisCalibButtonState()
        {
            if (btnCCalib.InvokeRequired)
            {
                btnCCalib.Invoke(SetAfterCAxisCalibButtonState);
            }
            else
            {
                btnCCalib.Enabled = true;
                btnCCalib.Tag = null;
                btnCCalib.Text = "Выполнить";
            }
        }

        private void cbImageShowAngleGrid_CheckedChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void cbImageShowShapes_CheckedChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void cbImageFlipH_CheckedChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void cbImageFlipV_CheckedChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void btnPnpPreview_Click(object sender, EventArgs e)
        {
            // Контуры плат
            previewForm.stencilMode = false;
            previewForm.contourWidthMm = 0.4f;
            previewForm.contoursToPreview.Clear();
            if (activeSideTop)
            {
                previewForm.contoursToPreview.AddRange(boardContoursTop);
            }
            else
            {
                previewForm.contoursToPreview.AddRange(boardContoursBottom);
            }

            // Места компонентов
            previewForm.holesToPreview.Clear();
            List<PEComponent> cmpList = new List<PEComponent>();
            if (activeSideTop)
            {
                cmpList.AddRange(topComponentList);
            }
            else
            {
                cmpList.AddRange(bottomComponentList);
            }
            foreach (PEComponent pec in cmpList)
            {
                previewForm.holesToPreview.Add(new CHole(false, pec.pXMm, pec.pYMm, 1.0f));
            }

            // Реперные отверстия
            SizeF workImagePhysicalSizeInMm = new SizeF(MainForm.workImagePhysWidthMm, MainForm.workImagePhysHeightMm);
            float holderHoleRectTop = MainForm.spaceOuterMm;
            float holderHoleRectLeft = MainForm.spaceOuterMm;
            float holderHoleRectRight = MainForm.spaceOuterMm + workImagePhysicalSizeInMm.Width + (MainForm.holderHolesIndentMm * 2f);
            float holderHoleRectBottom = MainForm.spaceOuterMm + workImagePhysicalSizeInMm.Height + (MainForm.holderHolesIndentMm * 2f);
            previewForm.holesToPreview.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), MainForm.anchorHoleDiameterMm));
            previewForm.holesToPreview.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.5f), MainForm.anchorHoleDiameterMm));
            previewForm.holesToPreview.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 1.0f), MainForm.anchorHoleDiameterMm));
            previewForm.holesToPreview.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.5f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 1.0f), MainForm.anchorHoleDiameterMm));
            previewForm.holesToPreview.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 1.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 1.0f), MainForm.anchorHoleDiameterMm));
            previewForm.holesToPreview.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 1.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.5f), MainForm.anchorHoleDiameterMm));
            previewForm.holesToPreview.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 1.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), MainForm.anchorHoleDiameterMm));
            previewForm.holesToPreview.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.5f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), MainForm.anchorHoleDiameterMm));

            // Показать
            previewForm.ShowDialog();
        }

        private void rbSideTop_CheckedChanged(object sender, EventArgs e)
        {
            if (IsComponentPNPing())
            {
                // rbSideTop в такой ситуации disabled, но на всякий случай
                ShowMessageBox("Один из компонентов в работе, действие недопустимо.");
                return;
            }
            else
            {
                activeSideTop = rbSideTop.Checked;
                ResetComponentSelectionAndProcess();
                RefreshComponentListViewUI();
            }
        }

        private void rbSideBottom_CheckedChanged(object sender, EventArgs e)
        {
            if (IsComponentPNPing())
            {
                // rbSideBottom в такой ситуации disabled, но на всякий случай
                ShowMessageBox("Один из компонентов в работе, действие недопустимо.");
                return;
            }
            else
            {
                activeSideTop = !rbSideBottom.Checked;
                ResetComponentSelectionAndProcess();
                RefreshComponentListViewUI();
            }
        }

        private bool IsComponentPNPing()
        {
            return topComponentList.Where((e) => e.placeStatus == 1).Count() > 0 || bottomComponentList.Where((e) => e.placeStatus == 1).Count() > 0;
        }

        private void lvComponents_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = lvComponents.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    if (!IsMachining() && !IsComponentPNPing() && cncConnected && !pnpFault && !areWeMoving)
                    {
                        cmpPopupMenu.Show(Cursor.Position);
                    }
                }
            }
        }

        private void cmpSelect_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Выполняется другой компонент обработки или вы уже выбрали компонент и выполняете его монтаж.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            int selCmpIdx = -1;
            try
            {
                selCmpIdx = (int)lvComponents.FocusedItem.Tag;
            }
            catch (Exception eee)
            {
                ShowMessageBox("Компонент не выбран в списке.");
                return;
            }
            try
            {
                if (activeSideTop)
                {
                    if (topComponentList[selCmpIdx].placeStatus == 2)
                    {
                        ShowMessageBox("Компонент уже установлен.");
                        return;
                    }
                    selectedComponentId = topComponentList[selCmpIdx].id;
                }
                else
                {
                    if (bottomComponentList[selCmpIdx].placeStatus == 2)
                    {
                        ShowMessageBox("Компонент уже установлен.");
                        return;
                    }
                    selectedComponentId = bottomComponentList[selCmpIdx].id;
                }
            }
            catch
            {
                ShowMessageBox("Компонент не найден.");
                return;
            }
            RefreshComponentListViewUI();
        }

        private void cmpShow_Click(object sender, EventArgs e)
        {
            if (!PanelCheck())
            {
                ShowMessageBox("Сперва выберите начало панели, убедитесь что она не выходит за рабочее поле.");
                return;
            }
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Выполняется другой компонент обработки или вы уже выбрали компонент и выполняете его монтаж.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }

            // Координаты компонента
            int selCmpIdx = -1;
            try
            {
                selCmpIdx = (int)lvComponents.FocusedItem.Tag;
            }
            catch (Exception eee)
            {
                ShowMessageBox("Компонент не выбран в списке.");
                return;
            }
            Vector2? cmpCoord = null;
            try
            {
                if (activeSideTop)
                {
                    cmpCoord = new Vector2(topComponentList[selCmpIdx].pXMm, topComponentList[selCmpIdx].pYMm);
                }
                else
                {
                    cmpCoord = new Vector2(bottomComponentList[selCmpIdx].pXMm, bottomComponentList[selCmpIdx].pYMm);
                }
            }
            catch { }
            if (cmpCoord == null)
            {
                ShowMessageBox("Координаты компонента не найдены.");
                return;
            }

            // Safe Z
            MoveToSafeZ();

            // Перемещение камеры к посадочному месту компонента
            float feedRateMmPerMin = Math.Min(maxFeedRateMmPerMinX, maxFeedRateMmPerMinY);
            Vector2 tgtP = PanelCoordToWorkAreaCoord(new Vector2(cmpCoord?.X ?? 0f, cmpCoord?.Y ?? 0f));
            float tgtX = tgtP.X;
            float tgtY = tgtP.Y;
            if (tgtX > maxTravelMmX || tgtY > maxTravelMmY || tgtX < 0f || tgtY < 0f)
            {
                ShowMessageBox("Движение превышает лимит оси.");
                return;
            }
            msw.Reset();
            msw.Start();
            grblCommandCmpl = 0;
            GrblSendLine(GetGCodeCommand("G0", tgtX - absMmX, tgtY - absMmY, 0f, feedRateMmPerMin));
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                ShowMessageBox("Ошибка ЧПУ станка при перемещении.");
                return;
            }
            msw.Stop();
            int leftToWaitMillis = MillisecToExecuteMove(tgtX - absMmX, tgtY - absMmY, 0f, feedRateMmPerMin, feedRateMmPerMin, feedRateMmPerMin) - (int)msw.ElapsedMilliseconds;
            absMmX = tgtX;
            absMmY = tgtY;
            SetAbsPositionLabel();
            if (leftToWaitMillis > 0)
            {
                areWeMoving = true;
                SetMoving();
                motionFinishWaitThread = new Thread(new ThreadStart(delegate
                {
                    Thread.Sleep(leftToWaitMillis);
                    areWeMoving = false;
                    SetMoving();
                }));
                motionFinishWaitThread?.Start();
            }
        }

        private void btnPick_Click(object sender, EventArgs e)
        {
            if (!PanelCheck())
            {
                ShowMessageBox("Сперва выберите начало панели, убедитесь что она не выходит за рабочее поле.");
                return;
            }
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Выполняется движение согласно вашим указанием.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (selectedComponentId < 0)
            {
                ShowMessageBox("Компонент для монтажа не выбран из списка.");
                return;
            }

            // Запустить процесс взятия компонента, после отметить его как в процессе placestatus = 1
            if (btnPick.Tag == null)
            {
                pnpAction = 0;
                machiningThread = new Thread(PNPProcess);
                machiningThread.Start();
                btnPick.Tag = 1;
                btnPick.Text = "...";
                btnPick.Enabled = false;
                btnRevert.Enabled = false;
                btnPlace.Enabled = false;
                btnRotateLeft.Enabled = false;
                btnRotateRight.Enabled = false;
                btnRotateLeft1.Enabled = false;
                btnRotateRight1.Enabled = false;
                nudCmpRotAngle.Enabled = false;
                rbSideTop.Enabled = false;
                rbSideBottom.Enabled = false;
                return;
            }
            else
            {
                ShowMessageBox("PickProcess уже выполняется.");
                return;
            }
        }


        private void btnRevert_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Выполняется движение согласно вашим указанием.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (!IsComponentPNPing())
            {
                ShowMessageBox("Нет компонента в работе.");
                return;
            }

            // Запустить процесс перемещения обратно и поставить placeStatus = 0
            if (btnRevert.Tag == null)
            {
                pnpAction = 4;
                machiningThread = new Thread(PNPProcess);
                machiningThread.Start();
                btnRevert.Tag = 1;
                btnRevert.Text = "...";
                btnRevert.Enabled = false;
                btnPick.Enabled = false;
                btnPlace.Enabled = false;
                btnRotateLeft1.Enabled = false;
                btnRotateRight1.Enabled = false;
                btnRotateLeft.Enabled = false;
                btnRotateRight.Enabled = false;
                nudCmpRotAngle.Enabled = false;
                rbSideTop.Enabled = false;
                rbSideBottom.Enabled = false;
                return;
            }
            else
            {
                ShowMessageBox("Revert уже выполняется.");
                return;
            }
        }

        private void btnPlace_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Выполняется движение согласно вашим указанием.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (!IsComponentPNPing())
            {
                ShowMessageBox("Компонент не взят.");
                return;
            }

            // Запустить процесс place (опустить сопло, поднять и переместиться обратно) и поставить placeStatus = 2
            if (btnPlace.Tag == null)
            {
                pnpAction = 3;
                machiningThread = new Thread(PNPProcess);
                machiningThread.Start();
                btnPlace.Tag = 1;
                btnPlace.Text = "...";
                btnPlace.Enabled = false;
                btnPick.Enabled = false;
                btnRevert.Enabled = false;
                btnRotateLeft.Enabled = false;
                btnRotateRight.Enabled = false;
                btnRotateLeft1.Enabled = false;
                btnRotateRight1.Enabled = false;
                nudCmpRotAngle.Enabled = false;
                rbSideTop.Enabled = false;
                rbSideBottom.Enabled = false;
                return;
            }
            else
            {
                ShowMessageBox("Place уже выполняется.");
                return;
            }
        }

        private void btnRotateLeft_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Выполняется движение согласно вашим указанием.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (!IsComponentPNPing())
            {
                ShowMessageBox("Компонент не взят.");
                return;
            }

            // Запустить процесс rotate left
            if (btnRotateLeft.Tag == null)
            {
                pnpAction = 1;
                pnpActionAngleDeg = 90f;
                machiningThread = new Thread(PNPProcess);
                machiningThread.Start();
                btnRotateLeft.Tag = 1;
                btnRotateLeft.Text = "...";
                btnRotateLeft.Enabled = false;
                btnPick.Enabled = false;
                btnRevert.Enabled = false;
                btnPlace.Enabled = false;
                btnRotateLeft1.Enabled = false;
                btnRotateRight1.Enabled = false;
                btnRotateRight.Enabled = false;
                nudCmpRotAngle.Enabled = false;
                rbSideTop.Enabled = false;
                rbSideBottom.Enabled = false;
                return;
            }
            else
            {
                ShowMessageBox("RotateLeft уже выполняется.");
                return;
            }
        }

        private void btnRotateRight_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Выполняется движение согласно вашим указанием.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (!IsComponentPNPing())
            {
                ShowMessageBox("Компонент не взят.");
                return;
            }

            // Запустить процесс rotate right
            if (btnRotateRight.Tag == null)
            {
                pnpAction = 2;
                pnpActionAngleDeg = 90f;
                machiningThread = new Thread(PNPProcess);
                machiningThread.Start();
                btnRotateRight.Tag = 1;
                btnRotateRight.Text = "...";
                btnRotateRight.Enabled = false;
                btnPick.Enabled = false;
                btnRevert.Enabled = false;
                btnPlace.Enabled = false;
                btnRotateLeft1.Enabled = false;
                btnRotateRight1.Enabled = false;
                btnRotateLeft.Enabled = false;
                nudCmpRotAngle.Enabled = false;
                rbSideTop.Enabled = false;
                rbSideBottom.Enabled = false;
                return;
            }
            else
            {
                ShowMessageBox("RotateRight уже выполняется.");
                return;
            }
        }

        private float GetLyingComponentAngle()
        {
            if (InvokeRequired)
            {
                return Invoke(GetLyingComponentAngle);
            }
            else
            {
                return (float)nudCmpRotAngle.Value;
            }
        }

        private void PNPProcess()
        {
            try
            {
                // Идет обработка
                SetPNPCalSettingsEnabled(false);
                SetPNPParamsEnabled(false);
                SetCalibrationUIEnabled(false);
                shouldMachiningStop = false;

                // Угол (-угол под которым компонент лежит + угол под которым лежит панель)
                float lyingDeg = GetLyingComponentAngle();
                float lyingRad = ((float)Math.PI / 180f) * lyingDeg;
                float addAngleRad = ((float)Math.PI / 180f) * addAngleDeg;
                float xmyAngleRad = (-lyingRad) + GetPanelRotAngleRad() - addAngleRad;

                // Компонент
                PEComponent? pec = null;
                int pecIdx = -1;
                bool topSide = false;
                try
                {
                    pec = topComponentList.Where((e) => e.id == selectedComponentId).First();
                    pecIdx = topComponentList.IndexOf(pec);
                    topSide = true;
                }
                catch { }
                if (pec == null)
                {
                    try
                    {
                        pec = bottomComponentList.Where((e) => e.id == selectedComponentId).First();
                        pecIdx = bottomComponentList.IndexOf(pec);
                        topSide = false;
                    }
                    catch { }
                }
                if (pec == null || pecIdx < 0)
                {
                    throw new Exception("Компонент не найден.");
                }

                // Действие
                int leftToWaitMillis = 0;
                if (pnpAction == 0)
                {
                    // Pick And Move
                    Stopwatch sw = new Stopwatch();
                    float feedRateMmPerMin = Math.Min(maxFeedRateMmPerMinX, maxFeedRateMmPerMinY);

                    // Перемещаем Z в median
                    MoveToSafeZ();

                    // Сохраняем исходную позицию
                    posBeforePick = new Vector2(absMmX + 0f, absMmY + 0f);

                    // Переместить каретку так чтобы сопло PNP оказалось там где камера
                    float tgtPosX = absMmX - nozzleRelMmX;
                    float tgtPosY = absMmY - nozzleRelMmY;
                    sw.Reset();
                    sw.Start();
                    grblCommandCmpl = 0;
                    GrblSendLine(GetGCodeCommand("G0", tgtPosX - absMmX, tgtPosY - absMmY, 0f, feedRateMmPerMin));
                    while (grblCommandCmpl < 1)
                    {
                        Thread.Sleep(10);
                    }
                    if (grblCommandCmpl >= 2)
                    {
                        throw new Exception("Ошибка станка при перемещении в начало панели.");
                    }
                    sw.Stop();
                    leftToWaitMillis = MillisecToExecuteMove(tgtPosX - absMmX, tgtPosY - absMmY, 0f, feedRateMmPerMin, feedRateMmPerMin, feedRateMmPerMin) - (int)sw.ElapsedMilliseconds;
                    absMmX = tgtPosX;
                    absMmY = tgtPosY;
                    SetAbsPositionLabel();
                    if (leftToWaitMillis > 0)
                    {
                        Thread.Sleep(leftToWaitMillis);
                    }

                    // Dwell
                    Thread.Sleep(300);

                    // Pick
                    TcpSendPick();

                    // Dwell
                    Thread.Sleep(300);

                    // Переместить сопло к месту установки компонента
                    Vector2 tgtP = PanelCoordToWorkAreaCoord(new Vector2(pec?.pXMm ?? 0f, pec?.pYMm ?? 0f));
                    tgtPosX = tgtP.X - nozzleRelMmX;
                    tgtPosY = tgtP.Y - nozzleRelMmY;
                    sw.Reset();
                    sw.Start();
                    grblCommandCmpl = 0;
                    GrblSendLine(GetGCodeCommand("G0", tgtPosX - absMmX, tgtPosY - absMmY, 0f, feedRateMmPerMin));
                    while (grblCommandCmpl < 1)
                    {
                        Thread.Sleep(10);
                    }
                    if (grblCommandCmpl >= 2)
                    {
                        throw new Exception("Ошибка станка при перемещении в начало панели.");
                    }
                    sw.Stop();
                    leftToWaitMillis = MillisecToExecuteMove(tgtPosX - absMmX, tgtPosY - absMmY, 0f, feedRateMmPerMin, feedRateMmPerMin, feedRateMmPerMin) - (int)sw.ElapsedMilliseconds;
                    absMmX = tgtPosX;
                    absMmY = tgtPosY;
                    SetAbsPositionLabel();
                    if (leftToWaitMillis > 0)
                    {
                        Thread.Sleep(leftToWaitMillis);
                    }

                    // Rotate
                    TcpSendRotate((int)((xmyAngleRad * (float)stepsPerCTurn) / (2f * Math.PI)));

                    // Статус компонента
                    if (topSide)
                    {
                        topComponentList[pecIdx].placeStatus = 1;
                    }
                    else
                    {
                        bottomComponentList[pecIdx].placeStatus = 1;
                    }
                }
                else if (pnpAction == 1)
                {
                    // Rotate Left
                    TcpSendRotate((int)(stepsPerCTurn * -(pnpActionAngleDeg / 360.0f)));
                }
                else if (pnpAction == 2)
                {
                    // Rotate Right
                    TcpSendRotate((int)(stepsPerCTurn * (pnpActionAngleDeg / 360.0f)));
                }
                else if (pnpAction == 3 || pnpAction == 4)
                {
                    // Place
                    Stopwatch sw = new Stopwatch();
                    float feedRateMmPerMin = Math.Min(maxFeedRateMmPerMinX, maxFeedRateMmPerMinY);

                    // Действие
                    if (pnpAction == 3)
                    {
                        // Place здесь
                        TcpSendPlace();
                    }
                    else
                    {
                        // Перемещаем сопло к исходной позиции и делаем Place
                        float tgtPosX = posBeforePick.X - nozzleRelMmX;
                        float tgtPosY = posBeforePick.Y - nozzleRelMmY;
                        sw.Reset();
                        sw.Start();
                        grblCommandCmpl = 0;
                        GrblSendLine(GetGCodeCommand("G0", tgtPosX - absMmX, tgtPosY - absMmY, 0f, feedRateMmPerMin));
                        while (grblCommandCmpl < 1)
                        {
                            Thread.Sleep(10);
                        }
                        if (grblCommandCmpl >= 2)
                        {
                            throw new Exception("Ошибка станка при перемещении в начало панели.");
                        }
                        sw.Stop();
                        leftToWaitMillis = MillisecToExecuteMove(tgtPosX - absMmX, tgtPosY - absMmY, 0f, feedRateMmPerMin, feedRateMmPerMin, feedRateMmPerMin) - (int)sw.ElapsedMilliseconds;
                        absMmX = tgtPosX;
                        absMmY = tgtPosY;
                        SetAbsPositionLabel();
                        if (leftToWaitMillis > 0)
                        {
                            Thread.Sleep(leftToWaitMillis);
                        }

                        // De-rotate
                        TcpSendRotate((int)((-xmyAngleRad * (float)stepsPerCTurn) / (2f * Math.PI)));

                        // Place
                        TcpSendPlace();
                    }

                    // Переместить камеру к исходной позиции
                    sw.Reset();
                    sw.Start();
                    grblCommandCmpl = 0;
                    GrblSendLine(GetGCodeCommand("G0", posBeforePick.X - absMmX, posBeforePick.Y - absMmY, 0f, feedRateMmPerMin));
                    while (grblCommandCmpl < 1)
                    {
                        Thread.Sleep(10);
                    }
                    if (grblCommandCmpl >= 2)
                    {
                        throw new Exception("Ошибка станка при перемещении в начало панели.");
                    }
                    sw.Stop();
                    leftToWaitMillis = MillisecToExecuteMove(posBeforePick.X - absMmX, posBeforePick.Y - absMmY, 0f, feedRateMmPerMin, feedRateMmPerMin, feedRateMmPerMin) - (int)sw.ElapsedMilliseconds;
                    absMmX = posBeforePick.X;
                    absMmY = posBeforePick.Y;
                    SetAbsPositionLabel();
                    if (leftToWaitMillis > 0)
                    {
                        Thread.Sleep(leftToWaitMillis);
                    }

                    // Возврат
                    if (pnpAction == 4)
                    {
                        // Статус компонента, он не установлен
                        if (topSide)
                        {
                            topComponentList[pecIdx].placeStatus = 0;
                        }
                        else
                        {
                            bottomComponentList[pecIdx].placeStatus = 0;
                        }
                    }
                    else
                    {
                        // Статус компонента, он установлен
                        if (topSide)
                        {
                            topComponentList[pecIdx].placeStatus = 2;
                        }
                        else
                        {
                            bottomComponentList[pecIdx].placeStatus = 2;
                        }
                        selectedComponentId = -1;
                    }
                }
                else
                {
                    throw new Exception("Неизвестное PNP-действие");
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
            finally
            {
                // Обработка завершена
                SetPNPCalSettingsEnabled(true);
                SetPNPParamsEnabled(true);
                SetCalibrationUIEnabled(true);
                SetAfterPNPActionButtonState();
                RefreshComponentListViewUI();
            }
        }

        public void SetAfterPNPActionButtonState()
        {
            if (InvokeRequired)
            {
                Invoke(SetAfterPNPActionButtonState);
            }
            else
            {
                btnPick.Enabled = true;
                btnPick.Tag = null;
                btnPick.Text = "Взять компонент и переместить";
                btnRotateLeft.Enabled = true;
                btnRotateLeft.Tag = null;
                btnRotateLeft.Text = "⤹";
                btnRotateRight.Enabled = true;
                btnRotateRight.Tag = null;
                btnRotateRight.Text = "⤸";
                btnRotateLeft1.Enabled = true;
                btnRotateLeft1.Tag = null;
                btnRotateLeft1.Text = "⤹1";
                btnRotateRight1.Enabled = true;
                btnRotateRight1.Tag = null;
                btnRotateRight1.Text = "⤸1";
                btnPlace.Enabled = true;
                btnPlace.Tag = null;
                btnPlace.Text = "Поставить";
                btnRevert.Enabled = true;
                btnRevert.Tag = null;
                btnRevert.Text = "Вернуть (отмена)";
                nudCmpRotAngle.Enabled = !IsComponentPNPing();
                rbSideTop.Enabled = !IsComponentPNPing();
                rbSideBottom.Enabled = !IsComponentPNPing();
            }
        }

        private void cmpReset_Click(object sender, EventArgs e)
        {
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Выполняется другой компонент обработки или вы уже выбрали компонент и выполняете его монтаж.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }

            // Координаты компонента
            int selCmpIdx = -1;
            try
            {
                selCmpIdx = (int)lvComponents.FocusedItem.Tag;
            }
            catch (Exception eee)
            {
                ShowMessageBox("Компонент не выбран в списке.");
                return;
            }
            if (activeSideTop)
            {
                topComponentList[selCmpIdx].placeStatus = 0;
            }
            else
            {
                bottomComponentList[selCmpIdx].placeStatus = 0;
            }
            RefreshComponentListViewUI();
        }

        private void nudImagePxPerMm_ValueChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void nudRectHeightMm_ValueChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void nudRectWidthMm_ValueChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void cbShowRect_CheckedChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void cbCPositiveClockwise_CheckedChanged(object sender, EventArgs e)
        {
            SavePNPCal();
        }

        private void btnMoveToFavLoc_Click(object sender, EventArgs e)
        {
            // Проверка
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Станок в работе над вашим Gerber-файлом или компонентом, функция недоступна.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Движение уже идет.");
                return;
            }

            // Куда перемещаемся
            PointF qp = new PointF(0f, 0f);
            if (((Button)sender).Tag == btnMoveToLoc1.Tag)
            {
                qp = favLoc1;
            }
            else if (((Button)sender).Tag == btnMoveToLoc2.Tag)
            {
                qp = favLoc2;
            }
            else if (((Button)sender).Tag == btnMoveToLoc3.Tag)
            {
                qp = favLoc3;
            }
            else if (((Button)sender).Tag == btnMoveToLoc4.Tag)
            {
                qp = favLoc4;
            }
            else if (((Button)sender).Tag == btnMoveToLoc5.Tag)
            {
                qp = favLoc5;
            }
            else if (((Button)sender).Tag == btnMoveToLoc6.Tag)
            {
                qp = favLoc6;
            }
            else if (((Button)sender).Tag == btnMoveToLoc7.Tag)
            {
                qp = favLoc7;
            }
            else if (((Button)sender).Tag == btnMoveToLoc8.Tag)
            {
                qp = favLoc8;
            }
            else if (((Button)sender).Tag == btnMoveToLoc9.Tag)
            {
                qp = favLoc9;
            }
            else if (((Button)sender).Tag == btnMoveToLoc10.Tag)
            {
                qp = favLoc10;
            }
            else
            {
                ShowMessageBox("Неизвестно куда переместиться.");
                return;
            }

            // Проверить границы
            if (qp.X > maxTravelMmX || qp.Y > maxTravelMmY || qp.X < 0f || qp.Y < 0f)
            {
                ShowMessageBox("Движение превышает лимит оси.");
                return;
            }

            // Safe Z
            MoveToSafeZ();

            // Перемещение
            msw.Reset();
            msw.Start();
            grblCommandCmpl = 0;
            GrblSendLine(GetGCodeCommand("G0", qp.X - absMmX, qp.Y - absMmY, 0f, 0f));
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                ShowMessageBox("Ошибка станка при перемещении.");
                return;
            }
            absMmX = qp.X;
            absMmY = qp.Y;
            SetAbsPositionLabel();
            msw.Stop();
            int leftToWaitMillis = MillisecToExecuteMove(qp.X - absMmX, qp.Y - absMmY, 0f, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, maxFeedRateMmPerMinZ) - (int)msw.ElapsedMilliseconds;
            if (leftToWaitMillis > 0)
            {
                areWeMoving = true;
                SetMoving();
                motionFinishWaitThread = new Thread(new ThreadStart(delegate
                {
                    Thread.Sleep(leftToWaitMillis);
                    areWeMoving = false;
                    SetMoving();
                }));
                motionFinishWaitThread?.Start();
            }
        }

        private void nudFavLock_ValueChanged(object sender, EventArgs e)
        {
            SaveFavLoc();
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
            /*if (panelA1.X > maxTravelMmX || panelA1.Y > maxTravelMmY || panelA2.X > maxTravelMmX || panelA2.Y > maxTravelMmY || panelA3.X > maxTravelMmX || panelA3.Y > maxTravelMmY || panelA4.X > maxTravelMmX || panelA4.Y > maxTravelMmY)
            {
                return false;
            }*/
            return true;
        }

        public static float ConvertRange(float originalStart, float originalEnd, float newStart, float newEnd, float value)
        {
            float scale = (float)(newEnd - newStart) / (originalEnd - originalStart);
            return (newStart + ((value - originalStart) * scale));
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
                float amountX = ConvertRange(MainForm.spaceOuterMm, MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f) + MainForm.workImagePhysWidthMm, 0f, 1f, panelCoord.X);
                float amountY = ConvertRange(MainForm.spaceOuterMm, MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f) + MainForm.workImagePhysHeightMm, 0f, 1f, panelCoord.Y);
                Vector2 l1p = Vector2.Lerp(dCorner1Mm, dCorner2Mm, amountX);
                Vector2 l2p = Vector2.Lerp(dCorner3Mm, dCorner4Mm, amountX);
                return Vector2.Lerp(l1p, l2p, amountY);
            }
        }

        private void btnCBottomLeft_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Станок в работе над вашим Gerber-файлом, функция недоступна.");
                return;
            }
            dCorner3Mm = new Vector2(absMmX, absMmY);
            SetPanelAnglesLabel();
        }

        private void btnCBottomRight_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (IsMachining() || IsComponentPNPing())
            {
                ShowMessageBox("Станок в работе над вашим Gerber-файлом, функция недоступна.");
                return;
            }
            dCorner4Mm = new Vector2(absMmX, absMmY);
            SetPanelAnglesLabel();
        }

        private void nudAddAngleDeg_ValueChanged(object sender, EventArgs e)
        {
            SavePNPCal();
        }

        private void rbZAwayFromWorkpiece_CheckedChanged(object sender, EventArgs e)
        {
            SaveMachineAxes();
        }

        private void rbZTowardsWorkpiece_CheckedChanged(object sender, EventArgs e)
        {
            SaveMachineAxes();
        }

        private void nudSafeHeightMm_ValueChanged(object sender, EventArgs e)
        {
            SavePNPCal();
            SetAbsPositionLabel();
        }

        public void SetAfterTestCalibButtonState()
        {
            if (btnTestCalib.InvokeRequired)
            {
                btnTestCalib.Invoke(SetAfterTestCalibButtonState);
            }
            else
            {
                btnTestCalib.Enabled = true;
                btnTestCalib.Tag = null;
                btnTestCalib.Text = "Тест";
            }
        }

        private void TestCalibProcess()
        {
            try
            {
                // Засекаем время
                Stopwatch sw = new Stopwatch();
                float feedRateMmPerMin = Math.Min(maxFeedRateMmPerMinX, maxFeedRateMmPerMinY);
                int leftToWaitMillis = 0;

                // Идет обработка
                SetPNPCalSettingsEnabled(false);
                SetPNPParamsEnabled(false);
                SetCalibrationUIEnabled(false);
                shouldMachiningStop = false;

                // Панель общая
                dCorner1Mm = calibTopLeft;
                dCorner2Mm = calibTopRight;
                dCorner3Mm = new Vector2(0f, 0f);
                dCorner4Mm = new Vector2(0f, 0f);
                SetPanelAnglesLabel();

                // Отверстия которые нам нужно пройти
                float holderHoleRectTopMm = MainForm.spaceOuterMm;
                float holderHoleRectLeftMm = MainForm.spaceOuterMm;
                float holderHoleRectRightMm = MainForm.workImagePhysWidthMm + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f);
                float holderHoleRectBottomMm = MainForm.workImagePhysHeightMm + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f);
                List<PointF> anchorHolesToPassMm = new List<PointF>();
                anchorHolesToPassMm.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.0f)));
                anchorHolesToPassMm.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.5f)));
                anchorHolesToPassMm.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 1.0f)));
                anchorHolesToPassMm.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.5f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 1.0f)));
                anchorHolesToPassMm.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 1.0f)));
                anchorHolesToPassMm.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.5f)));
                anchorHolesToPassMm.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.0f)));
                anchorHolesToPassMm.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.5f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.0f)));

                // Проходимся по каждому
                foreach (PointF anchorHoleMm in anchorHolesToPassMm)
                {
                    // Компенсировать криво лежащую на рабочем поле панель
                    Vector2 tgtP = PanelCoordToWorkAreaCoord(new Vector2(anchorHoleMm.X, anchorHoleMm.Y));
                    float tgtPointX = tgtP.X;
                    float tgtPointY = tgtP.Y;

                    // Переместиться в эту точку
                    sw.Reset();
                    sw.Start();
                    grblCommandCmpl = 0;
                    GrblSendLine(GetGCodeCommand("G0", tgtPointX - absMmX, tgtPointY - absMmY, 0f, feedRateMmPerMin));
                    while (grblCommandCmpl < 1)
                    {
                        Thread.Sleep(10);
                    }
                    if (grblCommandCmpl >= 2)
                    {
                        throw new Exception("Ошибка ЧПУ станка при перемещении к реперному отверстию.");
                    }
                    sw.Stop();
                    leftToWaitMillis = MillisecToExecuteMove(tgtPointX - absMmX, tgtPointY - absMmY, 0f, feedRateMmPerMin, feedRateMmPerMin, maxFeedRateMmPerMinZ) - (int)sw.ElapsedMilliseconds;
                    absMmX = tgtPointX;
                    absMmY = tgtPointY;
                    SetAbsPositionLabel();
                    if (leftToWaitMillis > 0)
                    {
                        Thread.Sleep(leftToWaitMillis);
                    }

                    // Подождать
                    Thread.Sleep(4000);

                    // Отмена
                    if (shouldMachiningStop) break;
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
            finally
            {
                // Обработка завершена
                SetPNPCalSettingsEnabled(true);
                SetPNPParamsEnabled(true);
                SetCalibrationUIEnabled(true);
                SetAfterTestCalibButtonState();
            }
        }

        private void btnTestCalib_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsComponentPNPing())
            {
                ShowMessageBox("Один из компонентов в работе, действие недопустимо.");
                return;
            }
            if (calibTopLeft.Length() < 1f || calibTopRight.Length() < 1f)
            {
                ShowMessageBox("Сперва выберите начало панели, проверьте что она в пределах рабочего поля.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            Vector2 diff = new Vector2(calibTopRight.X - calibTopLeft.X, calibTopRight.Y - calibTopLeft.Y);
            float panelSizeWidthMm = MainForm.workImagePhysWidthMm + (MainForm.holderHolesIndentMm) * 2f;
            if (Math.Abs(diff.Length() - panelSizeWidthMm) > 0.5f)
            {
                ShowMessageBox("Неправильно выставлены точки, ширина панели не совпадает.");
                return;
            }
            if (btnTestCalib.Tag == null)
            {
                if (IsMachining())
                {
                    ShowMessageBox("Идет другой процесс обработки.");
                    return;
                }
                if (MessageBox.Show(this, "Показать результат калибровки?", "PCBProduction2", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    machiningThread = new Thread(TestCalibProcess);
                    machiningThread.Start();
                    btnTestCalib.Tag = 1;
                    btnTestCalib.Text = "Стоп";
                    return;
                }
            }
            else
            {
                btnTestCalib.Enabled = false;
                shouldMachiningStop = true;
                return;
            }
        }

        private void btnCalibPanelTopLeft_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsComponentPNPing())
            {
                ShowMessageBox("Один из компонентов в работе, действие недопустимо.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Идет обработка заготовки, функция недоступна.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            calibTopLeft = new Vector2(absMmX, absMmY);
            SetPanelAnglesLabel();
        }

        private void btnCalibPanelTopRight_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsComponentPNPing())
            {
                ShowMessageBox("Один из компонентов в работе, действие недопустимо.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Идет обработка заготовки, функция недоступна.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            calibTopRight = new Vector2(absMmX, absMmY);
            SetPanelAnglesLabel();
        }

        private void btnCalibPanelBottomLeft_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsComponentPNPing())
            {
                ShowMessageBox("Один из компонентов в работе, действие недопустимо.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Идет обработка заготовки, функция недоступна.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            calibBottomLeft = new Vector2(absMmX, absMmY);
            SetPanelAnglesLabel();
        }

        private void btnCalibPanelBottomRight_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsComponentPNPing())
            {
                ShowMessageBox("Один из компонентов в работе, действие недопустимо.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Идет обработка заготовки, функция недоступна.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            calibBottomRight = new Vector2(absMmX, absMmY);
            SetPanelAnglesLabel();
        }

        private void btnCalibPanelCalculate_Click(object sender, EventArgs e)
        {
            if (calibTopLeft.Length() < 1f || calibTopRight.Length() < 1f || calibBottomLeft.Length() < 1f || calibBottomRight.Length() < 1f)
            {
                ShowMessageBox("Сперва выберите начало калибровочной панели, проверьте что она в пределах рабочего поля.");
                return;
            }

            // Правильный размер панели
            float correctWidthMm = MainForm.workImagePhysWidthMm + (MainForm.holderHolesIndentMm * 2f);
            float correctHeightMm = MainForm.workImagePhysHeightMm + (MainForm.holderHolesIndentMm * 2f);

            // Под каким углом на рабочем поле лежит калибровочная панель
            Vector2 diff1 = Vector2.Subtract(calibTopRight, calibTopLeft);
            Vector2 diff2 = Vector2.Subtract(calibBottomRight, calibBottomLeft);
            float cpRotRad = (float)((Math.Atan(diff1.Y / diff1.X) + Math.Atan(diff2.Y / diff2.X)) / 2.0f);

            // Вычитаем начало
            Vector2 trCornerT = Vector2.Subtract(calibTopRight, calibTopLeft);
            Vector2 blCornerT = Vector2.Subtract(calibBottomLeft, calibTopLeft);
            Vector2 brCornerT = Vector2.Subtract(calibBottomRight, calibTopLeft);

            // Компенсируем криво лежащую панель
            Vector2 trCornerRot = Vector2.Transform(trCornerT, Matrix3x2.CreateRotation(-cpRotRad));
            Vector2 blCornerRot = Vector2.Transform(blCornerT, Matrix3x2.CreateRotation(-cpRotRad));
            Vector2 brCornerRot = Vector2.Transform(brCornerT, Matrix3x2.CreateRotation(-cpRotRad));

            // Считаем угол между осями
            Vector2 nv = brCornerRot - trCornerRot;
            float newSkewRad = (float)((Math.Atan(blCornerRot.X / blCornerRot.Y) + Math.Atan(nv.X / nv.Y)) / 2.0f);

            // Считаем шаги на мм (сколько нужно поставить
            float newMotionStepsX = (correctWidthMm / trCornerRot.Length()) * motionStepsPerMmX;
            float newMotionStepsY = (correctHeightMm / blCornerRot.Length()) * motionStepsPerMmY;

            // Результат
            ShowMessageBox("Кривизна осей, рад: " + newSkewRad.ToString("#.######") + " (прибавить к текущему значению), шагов на мм по X нужно: " + newMotionStepsX.ToString("#.###") + ", шагов на мм по Y нужно: " + newMotionStepsY.ToString("#.###"));
        }

        private void btnRotateLeft1_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Выполняется движение согласно вашим указанием.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (!IsComponentPNPing())
            {
                ShowMessageBox("Компонент не взят.");
                return;
            }

            // Запустить процесс rotate left
            if (btnRotateLeft.Tag == null)
            {
                pnpAction = 1;
                pnpActionAngleDeg = 1f;
                machiningThread = new Thread(PNPProcess);
                machiningThread.Start();
                btnRotateLeft1.Tag = 1;
                btnRotateLeft1.Text = "...";
                btnRotateLeft1.Enabled = false;
                btnPick.Enabled = false;
                btnRevert.Enabled = false;
                btnPlace.Enabled = false;
                btnRotateLeft.Enabled = false;
                btnRotateRight.Enabled = false;
                btnRotateRight1.Enabled = false;
                nudCmpRotAngle.Enabled = false;
                rbSideTop.Enabled = false;
                rbSideBottom.Enabled = false;
                return;
            }
            else
            {
                ShowMessageBox("RotateLeft уже выполняется.");
                return;
            }
        }

        private void btnRotateRight1_Click(object sender, EventArgs e)
        {
            if (!cncConnected)
            {
                ShowMessageBox("Сперва подключитесь к станку.");
                return;
            }
            if (pnpFault)
            {
                ShowMessageBox("PNP станок в состоянии сбоя, переподключитесь.");
                return;
            }
            if (IsMachining())
            {
                ShowMessageBox("Выполняется движение согласно вашим указанием.");
                return;
            }
            if (grblCommandCmpl == 0)
            {
                ShowMessageBox("Выполняется команда GRBL, отсоединиться без потери состояния невозможно.");
                return;
            }
            if (areWeMoving)
            {
                ShowMessageBox("Идет движение, дождитесь завершения.");
                return;
            }
            if (!IsComponentPNPing())
            {
                ShowMessageBox("Компонент не взят.");
                return;
            }

            // Запустить процесс rotate right
            if (btnRotateRight.Tag == null)
            {
                pnpAction = 2;
                pnpActionAngleDeg = 1f;
                machiningThread = new Thread(PNPProcess);
                machiningThread.Start();
                btnRotateRight1.Tag = 1;
                btnRotateRight1.Text = "...";
                btnRotateRight1.Enabled = false;
                btnPick.Enabled = false;
                btnRevert.Enabled = false;
                btnPlace.Enabled = false;
                btnRotateLeft.Enabled = false;
                btnRotateRight.Enabled = false;
                btnRotateLeft1.Enabled = false;
                nudCmpRotAngle.Enabled = false;
                rbSideTop.Enabled = false;
                rbSideBottom.Enabled = false;
                return;
            }
            else
            {
                ShowMessageBox("RotateRight уже выполняется.");
                return;
            }
        }

        private void btnCamNext_Click(object sender, EventArgs e)
        {
            int newCamIdx = displayCamIdx + 1;
            if (newCamIdx > 2)
            {
                newCamIdx = 2;
            }
            displayCamIdx = newCamIdx + 0;
        }

        private void btnCamPrev_Click(object sender, EventArgs e)
        {
            int newCamIdx = displayCamIdx - 1;
            if (newCamIdx < 0)
            {
                newCamIdx = 0;
            }
            displayCamIdx = newCamIdx + 0;
        }

        private void nudFOVXFrom_ValueChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void nudFOVXTo_ValueChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void nudFOVYFrom_ValueChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }

        private void nudFOVYTo_ValueChanged(object sender, EventArgs e)
        {
            SaveImageSettings();
        }
    }
}