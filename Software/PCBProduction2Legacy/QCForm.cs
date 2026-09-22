using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using GerberVS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PCBProduction2
{
    public partial class QCForm : Form
    {
        // Gerber
        public LibGerberVS gerberVS = new LibGerberVS();
        public List<QCNet> allNets = new List<QCNet>();

        // Params
        public int renderDpi = 600;
        public float holeTentCoeff = 1.4f;
        public float minAnnularRingMm = 0.1f;

        public QCForm()
        {
            InitializeComponent();
        }

        private MyBitmapData DrawPanelFromGbrLayerFileBytes(String inputFile, String profileFile)
        {
            Bitmap im = DrawPanelFromGbrLayerFileImage(inputFile, profileFile, false);
            return new MyBitmapData(im);
        }

        private Bitmap DrawPanelFromGbrLayerFileImage(String inputFile, String profileFile, bool transparentBG)
        {
            // Render Gerber Layer
            GerberProject proj = gerberVS.CreateNewProject();
            gerberVS.OpenLayerFromFileName(proj, inputFile);
            gerberVS.OpenLayerFromFileName(proj, profileFile);
            proj.FileInfo[0].Color = Color.White;
            proj.FileInfo[1].Color = Color.Transparent;
            RectangleF bounds = Utils.FindBoundsOfProfileImage(proj.FileInfo[1].Image);
            GerberRenderInformation renderInformation = new GerberRenderInformation();
            renderInformation.RenderQuality = GerberRenderQuality.Default;
            renderInformation.ImageWidth = bounds.Width;
            renderInformation.ImageHeight = bounds.Height;
            renderInformation.LowerLeftX = -bounds.Left;
            renderInformation.LowerLeftY = -bounds.Bottom;
            float inkPrinterDpmm = renderDpi / 25.4f;
            float inkPrinterDpi = renderDpi;
            double imWidth = (proj.FileInfo.First().Image.Unit == GerberUnit.Inch) ? (inkPrinterDpi * renderInformation.ImageWidth) : (inkPrinterDpmm * renderInformation.ImageWidth);
            double imHeight = (proj.FileInfo.First().Image.Unit == GerberUnit.Inch) ? (inkPrinterDpi * renderInformation.ImageHeight) : (inkPrinterDpmm * renderInformation.ImageHeight);
            Bitmap layerImg = new Bitmap((int)Math.Floor(imWidth), (int)Math.Floor(imHeight), (transparentBG) ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
            if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
            {
                layerImg.SetResolution(inkPrinterDpi, inkPrinterDpi);
            }
            else
            {
                layerImg.SetResolution(inkPrinterDpmm, inkPrinterDpmm);
            }
            Graphics g = Graphics.FromImage(layerImg);
            if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
            {
                g.PageUnit = GraphicsUnit.Inch;
            }
            else
            {
                g.PageUnit = GraphicsUnit.Millimeter;
            }
            gerberVS.RenderAllLayers(g, proj, renderInformation);
            g.Dispose();

            // Free resources
            gerberVS.UnloadAllLayers(proj);

            // Return raw
            return layerImg;
        }

        private void QCForm_Shown(object sender, EventArgs e)
        {
            try
            {
                // Найти все файлы
                String gbrProfileFile = "";
                String gbrDrillFile = "";
                String gbrCopperTopFile = "";
                List<String> gbrCopperInnerFiles = new List<string>();
                String gbrCopperBottomFile = "";
                String gbrMaskTopFile = "";
                String gbrMaskBottomFile = "";
                List<String> allGerberFiles = Directory.GetFiles(MainForm.gerberFilePath).ToList();
                allGerberFiles.Sort((a, b) => File.ReadAllText(a).Length.CompareTo(File.ReadAllText(b).Length));
                foreach (String pFile in allGerberFiles)
                {
                    bool isBottom = (pFile.ToLower().Contains("bottom") || pFile.ToLower().EndsWith(".gbl") || pFile.ToLower().EndsWith(".gbo") || pFile.ToLower().EndsWith(".gbs") || pFile.ToLower().EndsWith(".gbp"));
                    bool isItCopper = (pFile.ToLower().Contains("copper") || pFile.ToLower().EndsWith(".gbl") || pFile.ToLower().EndsWith(".gtl"));
                    bool isItInnerCopper = (pFile.ToLower().Contains("copper_inner") || pFile.ToLower().EndsWith(".g1") || pFile.ToLower().EndsWith(".g2") || pFile.ToLower().EndsWith(".g3") || pFile.ToLower().EndsWith(".g4") || pFile.ToLower().EndsWith(".g5") || pFile.ToLower().EndsWith(".g6") || pFile.ToLower().EndsWith(".gp1") || pFile.ToLower().EndsWith(".gp2") || pFile.ToLower().EndsWith(".gp3") || pFile.ToLower().EndsWith(".gp4") || pFile.ToLower().EndsWith(".gp5") || pFile.ToLower().EndsWith(".gp6"));
                    bool isItSoldermask = (pFile.ToLower().Contains("soldermask") || pFile.ToLower().EndsWith(".gbs") || pFile.ToLower().EndsWith(".gts"));
                    if (isItInnerCopper)
                    {
                        gbrCopperInnerFiles.Add(pFile);
                    }
                    else if (isItCopper)
                    {
                        if (isBottom)
                        {
                            gbrCopperBottomFile = pFile;
                        }
                        else
                        {
                            gbrCopperTopFile = pFile;
                        }
                    }
                    else if (isItSoldermask)
                    {
                        if (isBottom)
                        {
                            gbrMaskBottomFile = pFile;
                        }
                        else
                        {
                            gbrMaskTopFile = pFile;
                        }
                    }
                    else if (pFile.ToLower().EndsWith(".gko") || pFile.ToLower().Contains("outline") || pFile.ToLower().Contains("profile"))
                    {
                        gbrProfileFile = pFile;
                    }
                    else if (pFile.ToLower().EndsWith(".xln") || pFile.ToLower().EndsWith(".drl"))
                    {
                        gbrDrillFile = pFile;
                    }
                }

                // Рендеринг картинок
                MyBitmapData bmpDrills = DrawPanelFromGbrLayerFileBytes(gbrDrillFile, gbrProfileFile);
                MyBitmapData bmpCopperTop = DrawPanelFromGbrLayerFileBytes(gbrCopperTopFile, gbrProfileFile);
                List<MyBitmapData> bmpCopperInner = new List<MyBitmapData>();
                for (int i = 0; i < gbrCopperInnerFiles.Count; i++)
                {
                    bmpCopperInner.Add(DrawPanelFromGbrLayerFileBytes(gbrCopperInnerFiles[i], gbrProfileFile));
                }
                MyBitmapData bmpCopperBottom = DrawPanelFromGbrLayerFileBytes(gbrCopperBottomFile, gbrProfileFile);
                MyBitmapData bmpMaskTop = DrawPanelFromGbrLayerFileBytes(gbrMaskTopFile, gbrProfileFile);
                MyBitmapData bmpMaskBottom = DrawPanelFromGbrLayerFileBytes(gbrMaskBottomFile, gbrProfileFile);

                // Получение картинки открытых пятаков
                MyBitmapData bmpPadsTop = new MyBitmapData(new Bitmap(bmpCopperTop.width, bmpCopperTop.height, PixelFormat.Format24bppRgb));
                MyBitmapData bmpPadsBottom = new MyBitmapData(new Bitmap(bmpCopperBottom.width, bmpCopperBottom.height, PixelFormat.Format24bppRgb));
                for (int i = 0; i < bmpCopperTop.width; i++)
                {
                    for (int j = 0; j < bmpCopperTop.height; j++)
                    {
                        if (bmpCopperTop.data[bmpCopperTop.GetIndexByCoord(i,j)] && bmpMaskTop.data[bmpMaskTop.GetIndexByCoord(i, j)])
                        {
                            bmpPadsTop.data[bmpPadsTop.GetIndexByCoord(i, j)] = true;
                        }
                        if (bmpCopperBottom.data[bmpCopperBottom.GetIndexByCoord(i, j)] && bmpMaskBottom.data[bmpMaskBottom.GetIndexByCoord(i, j)])
                        {
                            bmpPadsBottom.data[bmpPadsBottom.GetIndexByCoord(i, j)] = true;
                        }
                    }
                }

                // Распознавание фигур
                MyBitmapData bmpDrillsBak = bmpDrills.Copy();
                List<Point> topPadCenters = bmpPadsTop.GetAllShapes().ConvertAll((e) => e.centerPoint);
                List<Point> bottomPadCenters = bmpPadsBottom.GetAllShapes().ConvertAll((e) => e.centerPoint);
                List<Point> holeCenters = bmpDrills.GetAllShapes().ConvertAll((e) => e.centerPoint);

                // Отверстие/поясок - этап 1
                float imageDpmm = renderDpi / 25.4f;
                Bitmap bmpDrillsDilatedB = new Bitmap(bmpDrillsBak.width, bmpDrillsBak.height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bmpDrillsDilatedB))
                {
                    for (int i = 0; i < holeCenters.Count; i++)
                    {
                        // Это отверстие металлизировано?
                        bool metal = false;
                        int pxindex = bmpCopperTop.GetIndexByCoord(holeCenters[i].X, holeCenters[i].Y);
                        if (bmpCopperTop.data[pxindex] || bmpCopperBottom.data[pxindex])
                        {
                            metal = true;
                        }
                        foreach (MyBitmapData lbm in bmpCopperInner)
                        {
                            if (lbm.data[pxindex])
                            {
                                metal = true;
                            }
                        }

                        // Если да - рисуем увеличенную версию
                        if (metal)
                        {
                            List<Point> shape = bmpDrillsBak.GetShapeAndTurnBlackFlood(new Point(holeCenters[i].X, holeCenters[i].Y));
                            if (shape.Count > 0)
                            {
                                int minX = (shape.Min((e) => e.X));
                                int minY = (shape.Min((e) => e.Y));
                                int maxX = (shape.Max((e) => e.X));
                                int maxY = (shape.Max((e) => e.Y));
                                Rectangle hr = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                                int inflateByPixels = (int)((float)hr.Width * (holeTentCoeff - 1.0f) * 0.5f);
                                if (inflateByPixels < Math.Floor(minAnnularRingMm * imageDpmm))
                                {
                                    inflateByPixels = (int)Math.Floor(minAnnularRingMm * imageDpmm);
                                }
                                hr.Inflate(inflateByPixels, inflateByPixels);
                                g.FillEllipse(Brushes.White, hr);
                            }
                        }
                    }
                }

                // Сравниванием то что должно быть с медью - этап 2
                MyBitmapData bmpDrillsDilated = new MyBitmapData(bmpDrillsDilatedB);
                Bitmap holeFailureMap = new Bitmap(bmpDrillsDilated.width, bmpDrillsDilated.height);
                bool holeFailuresDetected = false;
                for (int i = 0; i < bmpDrillsDilated.width; i++)
                {
                    for (int j = 0; j < bmpDrillsDilated.height; j++)
                    {
                        int pindex = bmpDrillsDilated.GetIndexByCoord(i, j);
                        if (bmpDrillsDilated.data[pindex])
                        {
                            bool mismatchDetected = false;
                            if (!bmpCopperTop.data[pindex])
                            {
                                mismatchDetected = true;
                            }
                            else if (!bmpCopperBottom.data[pindex])
                            {
                                mismatchDetected = true;
                            }
                            else
                            {
                                foreach (MyBitmapData lbm in bmpCopperInner)
                                {
                                    if (!lbm.data[pindex])
                                    {
                                        mismatchDetected = true;
                                        break;
                                    }
                                }
                            }
                            if (mismatchDetected)
                            {
                                holeFailureMap.SetPixel(i, j, Color.White);
                                holeFailuresDetected = true;
                            }
                        }
                    }
                }

                // Формирование списка цепей
                allNets.Clear();
                for (int i = 0; i < holeCenters.Count; i++)
                {
                    // Есть ли цепь которая уже содержит эту via? Пропускаем.
                    if (allNets.Any((e) => e.viaCenters.Contains(holeCenters[i])))
                    {
                        continue;
                    }

                    // Цепь
                    QCNet curNet = new QCNet();

                    // Все via этой цепи и сама форма на каждом слою
                    List<Point> vias = new List<Point>();
                    List<Point> totalShapeTop = new List<Point>();
                    List<List<Point>> totalShapeInner = new List<List<Point>>();
                    List<Point> totalShapeBottom = new List<Point>();
                    vias.Add(holeCenters[i]);
                    totalShapeTop.Clear();
                    totalShapeInner.Clear();
                    totalShapeBottom.Clear();
                    while (true)
                    {
                        // Найти дорожки которые касаются отверстий в списке vias
                        for (int j = 0; j < vias.Count; j++)
                        {
                            // Найти
                            List<Point> shapeTop = bmpCopperTop.GetShapeAndTurnBlackFlood(vias[j]);
                            List<List<Point>> shapeInner = new List<List<Point>>();
                            for (int k = 0; k < bmpCopperInner.Count; k++)
                            {
                                shapeInner.Add(bmpCopperInner[k].GetShapeAndTurnBlackFlood(vias[j]));
                            }
                            List<Point> shapeBottom = bmpCopperBottom.GetShapeAndTurnBlackFlood(vias[j]);

                            // Добавить в общую форму всех слоев
                            totalShapeTop.AddRange(shapeTop);
                            for (int k = 0; k < shapeInner.Count; k++)
                            {
                                if (totalShapeInner.Count <= k)
                                {
                                    totalShapeInner.Add(new List<Point>());
                                }
                                totalShapeInner[k].AddRange(shapeInner[k]);
                            }
                            totalShapeBottom.AddRange(shapeBottom);
                        }
                        List<Point> allLayerShape = new List<Point>();
                        allLayerShape.AddRange(totalShapeTop);
                        for (int k = 0; k < totalShapeInner.Count; k++)
                        {
                            allLayerShape.AddRange(totalShapeInner[k]);
                        }
                        allLayerShape.AddRange(totalShapeBottom);

                        // Каких еще точек она касается (via)
                        bool hasNewVias = false;
                        for (int k = 0; k < holeCenters.Count; k++)
                        {
                            if (allLayerShape.Contains(holeCenters[k]) && !vias.Contains(holeCenters[k]))
                            {
                                vias.Add(holeCenters[k]);
                                hasNewVias = true;
                            }
                        }
                        if (!hasNewVias)
                        {
                            break;
                        }
                    }

                    // Проверка пустоты (в случае если отверстие не металлизировано)
                    if (totalShapeTop.Count < 1 && totalShapeBottom.Count < 1 && totalShapeInner.All((e) => e.Count < 1))
                    {
                        continue;
                    }

                    // Записать
                    curNet.trackByLayer.Add(totalShapeTop);
                    for (int k = 0; k < totalShapeInner.Count; k++)
                    {
                        curNet.trackByLayer.Add(totalShapeInner[k]);
                    }
                    curNet.trackByLayer.Add(totalShapeBottom);
                    curNet.viaCenters.AddRange(vias);

                    // Каких еще точек она касается (top)
                    for (int k = 0; k < topPadCenters.Count; k++)
                    {
                        if (totalShapeTop.Contains(topPadCenters[k]))
                        {
                            curNet.topPadCenters.Add(topPadCenters[k]);
                        }
                    }

                    // Каких еще точек она касается (bottom)
                    for (int k = 0; k < bottomPadCenters.Count; k++)
                    {
                        if (totalShapeBottom.Contains(bottomPadCenters[k]))
                        {
                            curNet.bottomPadCenters.Add(bottomPadCenters[k]);
                        }
                    }

                    // Добавить
                    allNets.Add(curNet);
                }

                // Без via (prepare)
                List<Point> emptyPoints = new List<Point>();

                // Без via (top)
                for (int i = 0; i < topPadCenters.Count; i++)
                {
                    // Есть ли цепь которая уже содержит эту TopPad? Пропускаем.
                    if (allNets.Any((e) => e.topPadCenters.Contains(topPadCenters[i])))
                    {
                        continue;
                    }

                    // Shape
                    QCNet curNet = new QCNet();
                    List<Point> topTrackShape = bmpCopperTop.GetShapeAndTurnBlackFlood(topPadCenters[i]);
                    if (topTrackShape.Count < 1)
                    {
                        continue;
                    }
                    for (int k = 0; k < topPadCenters.Count; k++)
                    {
                        if (topTrackShape.Contains(topPadCenters[k]))
                        {
                            curNet.topPadCenters.Add(topPadCenters[k]);
                        }
                    }
                    curNet.trackByLayer.Add(topTrackShape);
                    for (int j = 0; j < bmpCopperInner.Count; j++)
                    {
                        curNet.trackByLayer.Add(emptyPoints);
                    }
                    curNet.trackByLayer.Add(emptyPoints);

                    // Добавить
                    allNets.Add(curNet);
                }

                // Без via (bottom)
                for (int i = 0; i < bottomPadCenters.Count; i++)
                {
                    // Есть ли цепь которая уже содержит эту BottomPad? Пропускаем.
                    if (allNets.Any((e) => e.bottomPadCenters.Contains(bottomPadCenters[i])))
                    {
                        continue;
                    }

                    // Shape
                    QCNet curNet = new QCNet();
                    List<Point> bottomTrackShape = bmpCopperBottom.GetShapeAndTurnBlackFlood(bottomPadCenters[i]);
                    if (bottomTrackShape.Count < 1)
                    {
                        continue;
                    }
                    for (int k = 0; k < bottomPadCenters.Count; k++)
                    {
                        if (bottomTrackShape.Contains(bottomPadCenters[k]))
                        {
                            curNet.bottomPadCenters.Add(bottomPadCenters[k]);
                        }
                    }
                    for (int j = 0; j < bmpCopperInner.Count; j++)
                    {
                        curNet.trackByLayer.Add(emptyPoints);
                    }
                    curNet.trackByLayer.Add(emptyPoints);
                    curNet.trackByLayer.Add(bottomTrackShape);

                    // Добавить
                    allNets.Add(curNet);
                }

                // UI картинка
                pbPCB.HoleFailureMap = holeFailureMap;
                pbPCB.ImagesForEachLayer.Clear();
                pbPCB.ImagesForEachLayer.Add(DrawPanelFromGbrLayerFileImage(gbrCopperTopFile, gbrProfileFile, true));
                for (int i = 0; i < gbrCopperInnerFiles.Count; i++)
                {
                    pbPCB.ImagesForEachLayer.Add(DrawPanelFromGbrLayerFileImage(gbrCopperInnerFiles[i], gbrProfileFile, true));
                }
                pbPCB.ImagesForEachLayer.Add(DrawPanelFromGbrLayerFileImage(gbrCopperBottomFile, gbrProfileFile, true));
                pbPCB.Invalidate();

                // UI список
                lbNets.Items.Clear();
                for (int i = 0; i < allNets.Count; i++)
                {
                    lbNets.Items.Add("Цепь #" + i.ToString() + " (" + allNets[i].viaCenters.Count.ToString() + " via)");
                }

                // UI ошибки
                if (holeFailuresDetected)
                {
                    lblHoleTentStatus.Text = "Обнаружен сбой тентирование некоторых из отверстий, недостаточный поясок (annular ring).";
                    lblHoleTentStatus.ForeColor = Color.Red;
                }
                else
                {
                    lblHoleTentStatus.Text = "Тентирование отверстий в норме.";
                    lblHoleTentStatus.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "У вас ошибка в Gerber-файлах: " + ex.Message);
                Close();
                return;
            }
        }

        private void lbNets_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selIdx = lbNets.SelectedIndex;
                if (selIdx < 0) selIdx = 0;
                if (selIdx >= (allNets.Count - 1)) selIdx = allNets.Count - 1;
                pbPCB.SelectedNet = allNets[selIdx];
            }
            catch { }
            pbPCB.Invalidate();
        }

        private void cbShowErrors_CheckedChanged(object sender, EventArgs e)
        {
            pbPCB.ShowErrors = cbShowErrors.Checked;
            pbPCB.Invalidate();
        }
    }
}