    using DirectShowLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GerberVS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBProduction2
{
    public partial class ExposureForm : Form
    {
        // Gerber
        public LibGerberVS gerberVS = new LibGerberVS();

        // Params
        public bool isPhotoresistPositive = false;
        public int renderDpi = 300;
        public String topText = "Top";
        public String bottomText = "Bottom";
        public String gbrProfileFile = "";
        public String compensateByMatrix = "";
        public List<float> compensationMatrix = new List<float>();

        // Stouffer
        public const String STOUFFER_FILE = "stouffer.dat";
        public const String DRILLS_TOP_FILE = "drills_top.dat";
        public const String DRILLS_BOTTOM_FILE = "drills_bottom.dat";

        // UI
        public List<ELayer> loadedLayers = new List<ELayer>();
        public String gbrDrillFile = "";
        public float allowSoldermaskTentingHoleDiameterLessThanMm = 0.0f;

        public ushort[,] oldGamma;
        public bool gbError = false;

        // Прямая засветка
        public LCDParamsForm? lcdParamsForm;
        public LCDForm? lcdForm;

        public ExposureForm()
        {
            InitializeComponent();
        }

        private void ExposureForm_Shown(object sender, EventArgs e)
        {
            gbError = false;
            LoadGerberFileList();
            if (gbError)
            {
                return;
            }
            this.FormBorderStyle = FormBorderStyle.None;
            this.Location = new Point(0, 0);
            this.Size = Screen.FromControl(this).Bounds.Size;
            this.mainGroupBox.Location = new Point((int)(((float)this.Size.Width / 2f) - ((float)this.mainGroupBox.Size.Width / 2f)), (int)(((float)this.Size.Height / 2f) - ((float)this.mainGroupBox.Size.Height / 2f)));
            this.Cursor = new Cursor(Properties.Resources.cursor.Handle);
            tbBottomText.Cursor = this.Cursor;
            tbMatrixPath.Cursor = this.Cursor;
            tbTopText.Cursor = this.Cursor;
            nudRenderDpi.Cursor = this.Cursor;
            if (!CheckParams())
            {
                Close();
            }
            else
            {
                RefreshUIRender();
                RefreshUIDisplay();
            }
        }

        public void LoadGerberFileList()
        {
            try
            {
                // Файлы
                gbrProfileFile = "";
                gbrDrillFile = "";
                loadedLayers.Clear();
                List<String> sortedList = Directory.GetFiles(MainForm.gerberFilePath).ToList();
                sortedList.Sort();
                List<String> copperOuterLayers = new List<String>();
                List<String> copperInnerLayers = new List<String>();
                List<String> nonCopperLayers = new List<String>();
                foreach (String pFile in sortedList)
                {
                    if (pFile.ToLower().EndsWith(".gbr") || pFile.ToLower().EndsWith(".gko") || pFile.ToLower().EndsWith(".gtl") || pFile.ToLower().EndsWith(".gbl") || pFile.ToLower().EndsWith(".gto") || pFile.ToLower().EndsWith(".gbo") || pFile.ToLower().EndsWith(".gts") || pFile.ToLower().EndsWith(".gbs") || pFile.ToLower().EndsWith(".gtp") || pFile.ToLower().EndsWith(".gbp") || pFile.ToLower().EndsWith(".g1") || pFile.ToLower().EndsWith(".g2") || pFile.ToLower().EndsWith(".g3") || pFile.ToLower().EndsWith(".g4") || pFile.ToLower().EndsWith(".g5") || pFile.ToLower().EndsWith(".g6") || pFile.ToLower().EndsWith(".gp1") || pFile.ToLower().EndsWith(".gp2") || pFile.ToLower().EndsWith(".gp3") || pFile.ToLower().EndsWith(".gp4") || pFile.ToLower().EndsWith(".gp5") || pFile.ToLower().EndsWith(".gp6"))
                    {
                        if (pFile.ToLower().EndsWith(".gko") || pFile.ToLower().Contains("outline") || pFile.ToLower().Contains("profile"))
                        {
                            gbrProfileFile = pFile;
                        }
                        else
                        {
                            bool isItOuterCopper = (pFile.ToLower().Contains("copper_top") || pFile.ToLower().Contains("copper_bottom") || pFile.ToLower().EndsWith(".gbl") || pFile.ToLower().EndsWith(".gtl"));
                            bool isItInnerCopper = (pFile.ToLower().Contains("copper_inner") || pFile.ToLower().EndsWith(".g1") || pFile.ToLower().EndsWith(".g2") || pFile.ToLower().EndsWith(".g3") || pFile.ToLower().EndsWith(".g4") || pFile.ToLower().EndsWith(".g5") || pFile.ToLower().EndsWith(".g6") || pFile.ToLower().EndsWith(".gp1") || pFile.ToLower().EndsWith(".gp2") || pFile.ToLower().EndsWith(".gp3") || pFile.ToLower().EndsWith(".gp4") || pFile.ToLower().EndsWith(".gp5") || pFile.ToLower().EndsWith(".gp6"));
                            if (isItOuterCopper)
                            {
                                copperOuterLayers.Add(pFile);
                            }
                            else if (isItInnerCopper)
                            {
                                copperInnerLayers.Add(pFile);
                            } else
                            {
                                nonCopperLayers.Add(pFile);
                            }
                        }
                    }
                    else if (pFile.ToLower().EndsWith(".xln") || pFile.ToLower().EndsWith(".drl"))
                    {
                        gbrDrillFile = pFile;
                    }
                }

                // Упорядочить внешние слои
                if (copperOuterLayers.Count > 1)
                {
                    String fn = copperOuterLayers[0];
                    bool isBottom = (fn.ToLower().Contains("bottom") || fn.ToLower().EndsWith(".gbl"));
                    if (isBottom)
                    {
                        copperOuterLayers.Reverse();
                    }
                }

                // Загруженные слои по порядку (медь)
                int totalIdxCounter = 0;
                int copperIdxCounter = 0;
                if (copperOuterLayers.Count > 0)
                {
                    loadedLayers.Add(new ELayer(copperOuterLayers[0], true, false, 0, 0, false));
                    copperIdxCounter++;
                    totalIdxCounter++;
                }
                foreach (String fn in copperInnerLayers)
                {
                    bool shouldMirror = (copperIdxCounter >= (((float)(copperOuterLayers.Count + copperInnerLayers.Count)) / 2f));
                    loadedLayers.Add(new ELayer(fn, false, true, copperIdxCounter, totalIdxCounter, shouldMirror));
                    copperIdxCounter++;
                    totalIdxCounter++;
                }
                if (copperOuterLayers.Count > 1)
                {
                    loadedLayers.Add(new ELayer(copperOuterLayers[1], true, false, 0, 0, true));
                    copperIdxCounter++;
                    totalIdxCounter++;
                }

                // Не медь
                foreach (String fn in nonCopperLayers)
                {
                    bool shouldMirror = (fn.ToLower().Contains("bottom") || fn.ToLower().EndsWith(".gbo") || fn.ToLower().EndsWith(".gbs"));
                    if (fn.ToLower().EndsWith(".gbp") || fn.ToLower().EndsWith(".gtp") || fn.ToLower().Contains("solderpaste"))
                    {
                        loadedLayers.Add(new ELayer(fn, false, false, 0, totalIdxCounter, true));
                        loadedLayers.Add(new ELayer(fn, false, false, 0, totalIdxCounter, false));
                    }
                    else
                    {
                        loadedLayers.Add(new ELayer(fn, false, false, 0, totalIdxCounter, shouldMirror));
                    }
                    totalIdxCounter++;
                }

                // Штоуффер
                loadedLayers.Add(new ELayer(STOUFFER_FILE, false, false, 0, totalIdxCounter, false));
                totalIdxCounter++;

                // Отверстия
                loadedLayers.Add(new ELayer(DRILLS_TOP_FILE, false, false, 0, totalIdxCounter, false));
                totalIdxCounter++;
                loadedLayers.Add(new ELayer(DRILLS_BOTTOM_FILE, false, false, 0, totalIdxCounter, true));
                totalIdxCounter++;

                // UI
                clbLayers.Items.Clear();
                foreach (ELayer el in loadedLayers)
                {
                    clbLayers.Items.Add(Path.GetFileName(el.fileName));
                }
                clbLayers.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                gbError = true;
                MessageBox.Show(this, "У вас ошибка в Gerber-файлах: " + ex.Message);
                Close();
                return;
            }
        }

        public bool CheckParams()
        {
            isPhotoresistPositive = cbPositiveExposure.Checked;
            renderDpi = (int)nudRenderDpi.Value;
            topText = tbTopText.Text;
            bottomText = tbBottomText.Text;
            compensateByMatrix = (cbCompensateCNCLoss.Checked) ? tbMatrixPath.Text : "";
            if (compensateByMatrix.Length > 0)
            {
                try
                {
                    String textContent = File.ReadAllText(compensateByMatrix);
                    compensationMatrix = JsonConvert.DeserializeObject<List<float>>(textContent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                    return false;
                }
            }
            else
            {
                compensationMatrix.Clear();
            }
            if (renderDpi < 300 || renderDpi > 6000)
            {
                MessageBox.Show(this, "Слишком маленький или большой DPI рендеринга фотошаблонов.");
                return false;
            }
            if (gbrProfileFile.Length < 1)
            {
                MessageBox.Show(this, "Не найден файл Gerber Board Outline.");
                return false;
            }
            if (clbLayers.Items.Count < 1)
            {
                MessageBox.Show(this, "Не найдены файлы слоев Gerber.");
                return false;
            }
            return true;
        }

        private void btnPrintFilm_Click(object sender, EventArgs e)
        {
            if (loadedLayers.Count < 1)
            {
                MessageBox.Show(this, "Слои не загружены.");
                return;
            }
            if (clbLayers.CheckedIndices.Count < 1)
            {
                MessageBox.Show(this, "Ничего не выбрано для печати.");
                return;
            }
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintFilmPage;
            PrintDialog pdi = new PrintDialog();
            pdi.Document = pd;
            if (pdi.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void Pd_PrintFilmPage(object sender, PrintPageEventArgs e)
        {
            e.HasMorePages = false;

            // Предобработка Bitmapов и поворот так чтобы ширина была больше высоты
            List<Bitmap> bitmapsToRender = new List<Bitmap>();
            int idx = 0;
            foreach (ELayer el in loadedLayers)
            {
                if (el.bitmap != null && clbLayers.CheckedIndices.Contains(idx))
                {
                    Bitmap bmpCopy = new Bitmap(el.bitmap);
                    bmpCopy.SetResolution(1f, 1f);
                    if (el.bitmap.Width < el.bitmap.Height)
                    {
                        bmpCopy.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    bitmapsToRender.Add(bmpCopy);
                }
                idx++;
            }

            // Stack
            int widthLimitPx = (int)Math.Floor((((float)Math.Max(e.PageBounds.Width, e.PageBounds.Height)) / 100.0f) * renderDpi);
            int firstBmpWidthPx = 1;
            int firstBmpHeightPx = 1;
            try
            {
                firstBmpWidthPx = bitmapsToRender.First().Width;
                firstBmpHeightPx = bitmapsToRender.First().Height;
            }
            catch { }
            int colCount = (int)Math.Floor((float)widthLimitPx / (float)firstBmpWidthPx);
            int rowCount = (int)Math.Ceiling((float)bitmapsToRender.Count / (float)colCount);
            if (colCount > bitmapsToRender.Count)
            {
                colCount = bitmapsToRender.Count;
            }
            int gIdx = 0;
            Bitmap totalGridImage = new Bitmap(colCount * firstBmpWidthPx, rowCount * firstBmpHeightPx);
            totalGridImage.SetResolution(1f, 1f);
            using (Graphics g = Graphics.FromImage(totalGridImage))
            {
                for (int x = 0; x < colCount; x++)
                {
                    for (int y = 0; y < rowCount; y++)
                    {
                        g.DrawImage(bitmapsToRender[gIdx], new Point(x * firstBmpWidthPx, y * firstBmpHeightPx));
                        gIdx++;
                    }
                }
            }

            // Рендер общей картинки на страницу
            bool isMaskWide = totalGridImage.Width > totalGridImage.Height;
            bool isPaperWide = e.PageBounds.Width > e.PageBounds.Height;
            if (isMaskWide != isPaperWide)
            {
                totalGridImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            float widthU = totalGridImage.Width * 100.0f / renderDpi;
            float heightU = totalGridImage.Height * 100.0f / renderDpi;
            float left = e.PageBounds.Left + (e.PageBounds.Width / 2.0f) - (widthU / 2.0f);
            float top = e.PageBounds.Top + (e.PageBounds.Height / 2.0f) - (heightU / 2.0f);
            if (left < e.PageBounds.Left) left = e.PageBounds.Left;
            if (top < e.PageBounds.Top) top = e.PageBounds.Top;
            e.Graphics?.DrawImage(totalGridImage, left, top, widthU, heightU);
        }

        private void cbPositiveExposure_CheckedChanged(object sender, EventArgs e)
        {
            RefreshUIRender();
            RefreshUIDisplay();
        }

        private void nudEtchUndercutCompensateMm_ValueChanged(object sender, EventArgs e)
        {
            RefreshUIRender();
            RefreshUIDisplay();
        }

        private void nudRenderDpi_ValueChanged(object sender, EventArgs e)
        {
            RefreshUIRender();
            RefreshUIDisplay();
        }

        public void RefreshUIRender()
        {
            if (CheckParams())
            {
                for (int i = 0; i < loadedLayers.Count; i++)
                {
                    try
                    {
                        loadedLayers[i].bitmap = DrawPanelFromGbrLayerFile(loadedLayers[i].fileName, gbrProfileFile, isPhotoresistPositive, !loadedLayers[i].shouldMirror, (loadedLayers[i].shouldMirror) ? bottomText : topText, renderDpi);
                    } catch
                    {
                        loadedLayers[i].bitmap = null;
                    }
                }
            }
        }

        public Bitmap? RedOnly(Bitmap? srcBmp)
        {
            if (srcBmp == null) return null;
            Bitmap resBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            var mxRed = new float[][]
            {
                new float[] { 1,  0,  0,  0,  0},
                new float[] { 1,  0,  0,  0,  0},
                new float[] { 0,  0,  0,  0,  0},
                new float[] { 0,  0,  0,  1,  0},
                new float[] { 0,  0,  0,  0,  0}
            };
            using (Graphics g = Graphics.FromImage(resBmp))
            {
                using (var attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(new ColorMatrix(mxRed));
                    var rect = new Rectangle(Point.Empty, srcBmp.Size);
                    g.DrawImage(srcBmp, rect, 0, 0, srcBmp.Width, srcBmp.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return resBmp;
        }

        public void RefreshUIDisplay()
        {
            int selLayerIdx = clbLayers.SelectedIndex;
            if (selLayerIdx > (loadedLayers.Count - 1)) selLayerIdx = loadedLayers.Count - 1;
            if (selLayerIdx < 0) selLayerIdx = 0;
            if (loadedLayers.Count > 0)
            {
                Bitmap? bmpAtIdx = RedOnly(loadedLayers[selLayerIdx].bitmap);
                if (bmpAtIdx == null)
                {
                    pbPreview.Image = null;
                    lblWhiteCount.Text = "?";
                }
                else
                {
                    pbPreview.Height = (int)((float)pbPreview.Width * ((float)bmpAtIdx.Height / (float)bmpAtIdx.Width));
                    pbPreview.Image = bmpAtIdx;
                    int whitePixels = 0;
                    for (int x = 0; x < bmpAtIdx.Width; x++)
                    {
                        for (int y = 0; y < bmpAtIdx.Height; y++)
                        {
                            Color clr = bmpAtIdx.GetPixel(x, y);
                            if (clr.R > 200)
                            {
                                whitePixels++;
                            }
                        }
                    }
                    float whPercent = ((float)whitePixels * 100.0f) / (float)(bmpAtIdx.Width * bmpAtIdx.Height);
                    lblWhiteCount.Text = whPercent.ToString("0.##") + "% белых пикселей";
                }
            }
            else
            {
                pbPreview.Image = null;
            }
        }

        private Bitmap DrawPanelFromGbrLayerFile(String inputFile, String profileFile, bool shouldInvert, bool shouldMirror, String text, int rDPI)
        {
            Bitmap? layerImg;
            float inkPrinterDpmm = rDPI / 25.4f;
            float inkPrinterDpi = rDPI;
            GerberProject proj = gerberVS.CreateNewProject();

            // Stencil?
            String clFName = Path.GetFileName(inputFile);
            bool stencilMode = clFName.ToLower().Contains("solderpaste") || clFName.ToLower().EndsWith(".gtp") || clFName.ToLower().EndsWith(".gbp");

            // Mask?
            bool maskMode = clFName.ToLower().Contains("soldermask") || clFName.ToLower().EndsWith(".gts") || clFName.ToLower().EndsWith(".gbs");
            String maskCopperFile = "";
            if (maskMode)
            {
                List<ELayer> outerLoadedLayers = loadedLayers.Where((e) => e.isCopperOuter).ToList();
                if (clFName.ToLower().EndsWith(".gts") || clFName.ToLower().Contains("top"))
                {
                    maskCopperFile = outerLoadedLayers[0].fileName;
                } else
                {
                    maskCopperFile = outerLoadedLayers[1].fileName;
                }
            }

            // Stouffer
            if (inputFile == STOUFFER_FILE)
            {
                float stWidthMm = MainForm.workImagePhysWidthMm - (MainForm.gapBetweenPCBsMm * 2f);
                float stHeightMm = MainForm.workImagePhysHeightMm - (MainForm.gapBetweenPCBsMm * 2f);
                float[] transmittancePercent = { 100f, 71f, 50f, 36f, 25f, 18f, 12.5f, 9f, 6.25f, 4.5f, 3.13f, 2.25f, 1.57f, 1.12f, 0.79f, 0.56f, 0.4f, 0.28f, 0.2f, 0.14f, 0.1f };
                double imWidth = (inkPrinterDpmm * stWidthMm);
                double imHeight = (inkPrinterDpmm * stHeightMm);
                layerImg = new Bitmap((int)Math.Floor(imWidth), (int)Math.Floor(imHeight), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                layerImg.SetResolution(inkPrinterDpmm, inkPrinterDpmm);
                using (Graphics sg = Graphics.FromImage(layerImg))
                {
                    sg.Clear((isPhotoresistPositive) ? Color.White : Color.Black);
                    float emSize = (float)imWidth * 0.2f;
                    Font fnt = new Font(FontFamily.GenericSansSerif, emSize, FontStyle.Regular);
                    for (int sx = 0; sx < 5; sx++)
                    {
                        for (int sy = 0; sy < 5; sy++)
                        {
                            int sqIdx = (sx * 5) + sy;
                            if (sqIdx > (transmittancePercent.Length - 1))
                            {
                                sqIdx = transmittancePercent.Length - 1;
                            }
                            int W = (int)(255f * transmittancePercent[sqIdx] * 0.01f);
                            if (isPhotoresistPositive)
                            {
                                W = 255 - W;
                            }
                            RectangleF lorect = new Rectangle((int)(imWidth * sx / 5f), (int)(imHeight * sy / 5f), (int)(imWidth / 5f), (int)(imHeight / 5f));
                            sg.FillRectangle(new SolidBrush(Color.FromArgb(W, W, W)), lorect);
                            sg.DrawString((sqIdx + 1).ToString(), fnt, (isPhotoresistPositive) ? Brushes.White : Brushes.Black, lorect);
                        }
                    }
                }
            }
            else if (inputFile == DRILLS_TOP_FILE || inputFile == DRILLS_BOTTOM_FILE)
            {
                // Open Gerbers
                String rawCopperFile = "";
                List<ELayer> outerLoadedLayers = loadedLayers.Where((e) => e.isCopperOuter).ToList();
                if (inputFile == DRILLS_TOP_FILE)
                {
                    rawCopperFile = outerLoadedLayers[0].fileName;
                }
                else
                {
                    rawCopperFile = outerLoadedLayers[1].fileName;
                }
                gerberVS.OpenLayerFromFileName(proj, rawCopperFile);
                gerberVS.OpenLayerFromFileName(proj, gbrDrillFile);
                gerberVS.OpenLayerFromFileName(proj, profileFile);
                RectangleF bounds = Utils.FindBoundsOfProfileImage(proj.FileInfo[proj.FileInfo.Count - 1].Image);
                GerberRenderInformation renderInformation = new GerberRenderInformation();
                renderInformation.RenderQuality = GerberRenderQuality.Default;
                renderInformation.ImageWidth = bounds.Width;
                renderInformation.ImageHeight = bounds.Height;
                renderInformation.LowerLeftX = -bounds.Left;
                renderInformation.LowerLeftY = -bounds.Bottom;
                double imWidth = (proj.FileInfo.First().Image.Unit == GerberUnit.Inch) ? (inkPrinterDpi * renderInformation.ImageWidth) : (inkPrinterDpmm * renderInformation.ImageWidth);
                double imHeight = (proj.FileInfo.First().Image.Unit == GerberUnit.Inch) ? (inkPrinterDpi * renderInformation.ImageHeight) : (inkPrinterDpmm * renderInformation.ImageHeight);

                // Render 1
                Bitmap rawLayerImg1 = new Bitmap((int)Math.Floor(2f * imWidth), (int)Math.Floor(2f * imHeight), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                proj.FileInfo[0].Color = Color.White;
                proj.FileInfo[0].IsVisible = true;
                if (maskMode)
                {
                    proj.FileInfo[1].Color = Color.Transparent;
                    proj.FileInfo[1].IsVisible = false;
                    proj.FileInfo[2].Color = Color.Transparent;
                    proj.FileInfo[2].IsVisible = false;
                }
                proj.FileInfo[proj.FileInfo.Count - 1].Color = Color.Transparent;
                if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                {
                    rawLayerImg1.SetResolution(inkPrinterDpi * 2f, inkPrinterDpi * 2f);
                }
                else
                {
                    rawLayerImg1.SetResolution(inkPrinterDpmm * 2f, inkPrinterDpmm * 2f);
                }
                using (Graphics g = Graphics.FromImage(rawLayerImg1))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                    {
                        g.PageUnit = GraphicsUnit.Inch;
                    }
                    else
                    {
                        g.PageUnit = GraphicsUnit.Millimeter;
                    }
                    gerberVS.RenderAllLayersForVectorOutput(g, proj, renderInformation);
                }

                // Render 2
                Bitmap rawLayerImg2 = new Bitmap((int)Math.Floor(2f * imWidth), (int)Math.Floor(2f * imHeight), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                {
                    rawLayerImg2.SetResolution(inkPrinterDpi * 2f, inkPrinterDpi * 2f);
                }
                else
                {
                    rawLayerImg2.SetResolution(inkPrinterDpmm * 2f, inkPrinterDpmm * 2f);
                }
                using (Graphics gd = Graphics.FromImage(rawLayerImg2))
                {
                    Aperture[] apertures = proj.FileInfo[1].Image.ApertureArray();
                    foreach (GerberNet net in proj.FileInfo[1].Image.GerberNetList)
                    {
                        if (net.ApertureState == GerberApertureState.Flash)
                        {
                            double cxInFileUnits = (net.StartX - bounds.Left);
                            double cyInFileUnits = (bounds.Bottom - net.StartY);
                            double diameterInFileUnits = apertures[net.Aperture].Parameters()[0];
                            CHole curHole = new CHole(
                                false,
                                (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cxInFileUnits * 25.4f) : cxInFileUnits),
                                (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cyInFileUnits * 25.4f) : cyInFileUnits),
                                (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (diameterInFileUnits * 25.4f) : diameterInFileUnits)
                            );
                            float untentDiameterMm = Math.Max(curHole.diameterMm * 1.1f, curHole.diameterMm + 0.3f);
                            gd.FillEllipse(Brushes.White, new RectangleF((curHole.holeCenterMmX - (untentDiameterMm * 0.5f)) * inkPrinterDpmm * 2f, (curHole.holeCenterMmY - (untentDiameterMm * 0.5f)) * inkPrinterDpmm * 2f, untentDiameterMm * inkPrinterDpmm * 2f, untentDiameterMm * inkPrinterDpmm * 2f));
                        }
                    }
                }

                // Combine
                layerImg = new Bitmap(Utils.Dilation(Utils.And(rawLayerImg1, rawLayerImg2), 0, isPhotoresistPositive), new Size((int)Math.Floor(imWidth), (int)Math.Floor(imHeight)));
                if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                {
                    layerImg.SetResolution(inkPrinterDpi, inkPrinterDpi);
                }
                else
                {
                    layerImg.SetResolution(inkPrinterDpmm, inkPrinterDpmm);
                }
            }
            else
            {
                // Open Gerbers
                gerberVS.OpenLayerFromFileName(proj, inputFile);
                if (maskMode)
                {
                    gerberVS.OpenLayerFromFileName(proj, maskCopperFile);
                    gerberVS.OpenLayerFromFileName(proj, gbrDrillFile);
                }
                gerberVS.OpenLayerFromFileName(proj, profileFile);
                RectangleF bounds = Utils.FindBoundsOfProfileImage(proj.FileInfo[proj.FileInfo.Count - 1].Image);
                GerberRenderInformation renderInformation = new GerberRenderInformation();
                renderInformation.RenderQuality = GerberRenderQuality.Default;
                renderInformation.ImageWidth = bounds.Width;
                renderInformation.ImageHeight = bounds.Height;
                renderInformation.LowerLeftX = -bounds.Left;
                renderInformation.LowerLeftY = -bounds.Bottom;
                double imWidth = (proj.FileInfo.First().Image.Unit == GerberUnit.Inch) ? (inkPrinterDpi * renderInformation.ImageWidth) : (inkPrinterDpmm * renderInformation.ImageWidth);
                double imHeight = (proj.FileInfo.First().Image.Unit == GerberUnit.Inch) ? (inkPrinterDpi * renderInformation.ImageHeight) : (inkPrinterDpmm * renderInformation.ImageHeight);

                // Render 1
                Bitmap rawLayerImg1 = new Bitmap((int)Math.Floor(2f * imWidth), (int)Math.Floor(2f * imHeight), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                proj.FileInfo[0].Color = Color.White;
                proj.FileInfo[0].IsVisible = true;
                if (maskMode)
                {
                    proj.FileInfo[1].Color = Color.Transparent;
                    proj.FileInfo[1].IsVisible = false;
                    proj.FileInfo[2].Color = Color.Transparent;
                    proj.FileInfo[2].IsVisible = false;
                }
                proj.FileInfo[proj.FileInfo.Count - 1].Color = Color.Transparent;
                if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                {
                    rawLayerImg1.SetResolution(inkPrinterDpi * 2f, inkPrinterDpi * 2f);
                }
                else
                {
                    rawLayerImg1.SetResolution(inkPrinterDpmm * 2f, inkPrinterDpmm * 2f);
                }
                using (Graphics g = Graphics.FromImage(rawLayerImg1))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                    {
                        g.PageUnit = GraphicsUnit.Inch;
                    }
                    else
                    {
                        g.PageUnit = GraphicsUnit.Millimeter;
                    }
                    gerberVS.RenderAllLayersForVectorOutput(g, proj, renderInformation);
                }

                // Render 2
                Bitmap rawLayerImg2 = new Bitmap((int)Math.Floor(2f * imWidth), (int)Math.Floor(2f * imHeight), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                if (maskMode)
                {
                    proj.FileInfo[0].Color = Color.Transparent;
                    proj.FileInfo[0].IsVisible = false;
                    proj.FileInfo[1].Color = Color.White;
                    proj.FileInfo[1].IsVisible = true;
                    proj.FileInfo[2].Color = Color.Transparent;
                    proj.FileInfo[2].IsVisible = false;
                    proj.FileInfo[proj.FileInfo.Count - 1].Color = Color.Transparent;
                    if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                    {
                        rawLayerImg2.SetResolution(inkPrinterDpi * 2f, inkPrinterDpi * 2f);
                    }
                    else
                    {
                        rawLayerImg2.SetResolution(inkPrinterDpmm * 2f, inkPrinterDpmm * 2f);
                    }
                    using (Graphics g = Graphics.FromImage(rawLayerImg2))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                        {
                            g.PageUnit = GraphicsUnit.Inch;
                        }
                        else
                        {
                            g.PageUnit = GraphicsUnit.Millimeter;
                        }
                        gerberVS.RenderAllLayersForVectorOutput(g, proj, renderInformation);
                    }
                }

                // Render 3
                Bitmap rawLayerImg3 = new Bitmap((int)Math.Floor(2f * imWidth), (int)Math.Floor(2f * imHeight), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                if (maskMode)
                {
                    if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                    {
                        rawLayerImg3.SetResolution(inkPrinterDpi * 2f, inkPrinterDpi * 2f);
                    }
                    else
                    {
                        rawLayerImg3.SetResolution(inkPrinterDpmm * 2f, inkPrinterDpmm * 2f);
                    }
                    using (Graphics gd = Graphics.FromImage(rawLayerImg3))
                    {
                        Aperture[] apertures = proj.FileInfo[2].Image.ApertureArray();
                        foreach (GerberNet net in proj.FileInfo[2].Image.GerberNetList)
                        {
                            if (net.ApertureState == GerberApertureState.Flash)
                            {
                                double cxInFileUnits = (net.StartX - bounds.Left);
                                double cyInFileUnits = (bounds.Bottom - net.StartY);
                                double diameterInFileUnits = apertures[net.Aperture].Parameters()[0];
                                CHole curHole = new CHole(
                                    false,
                                    (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cxInFileUnits * 25.4f) : cxInFileUnits),
                                    (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cyInFileUnits * 25.4f) : cyInFileUnits),
                                    (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (diameterInFileUnits * 25.4f) : diameterInFileUnits)
                                );
                                if (curHole.diameterMm > allowSoldermaskTentingHoleDiameterLessThanMm)
                                {
                                    float untentDiameterMm = Math.Max(curHole.diameterMm * 1.1f, curHole.diameterMm + 0.3f);
                                    gd.FillEllipse(Brushes.White, new RectangleF((curHole.holeCenterMmX - (untentDiameterMm * 0.5f)) * inkPrinterDpmm * 2f, (curHole.holeCenterMmY - (untentDiameterMm * 0.5f)) * inkPrinterDpmm * 2f, untentDiameterMm * inkPrinterDpmm * 2f, untentDiameterMm * inkPrinterDpmm * 2f));
                                }
                            }
                        }
                    }
                }

                // Combine
                int dilationVal = 0;
                if (stencilMode)
                {
                    dilationVal = 4;
                }
                else if (maskMode)
                {
                    dilationVal = -4;
                }
                layerImg = new Bitmap(Utils.Dilation((maskMode) ? Utils.AndOr(rawLayerImg1, rawLayerImg2, rawLayerImg3) : rawLayerImg1, dilationVal, isPhotoresistPositive), new Size((int)Math.Floor(imWidth), (int)Math.Floor(imHeight)));
                if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                {
                    layerImg.SetResolution(inkPrinterDpi, inkPrinterDpi);
                }
                else
                {
                    layerImg.SetResolution(inkPrinterDpmm, inkPrinterDpmm);
                }
            }

            // Pre-process layer image
            int gapBetweenPCBsPx = (int)Math.Ceiling(MainForm.gapBetweenPCBsMm * inkPrinterDpmm);
            Bitmap paddedBmp = new Bitmap(layerImg.Width + (gapBetweenPCBsPx * 2), layerImg.Height + (gapBetweenPCBsPx * 2));
            paddedBmp.SetResolution(layerImg.HorizontalResolution, layerImg.VerticalResolution);
            using (Graphics gp = Graphics.FromImage(paddedBmp))
            {
                gp.Clear(Color.Black);
                gp.DrawImage(layerImg, new Point(gapBetweenPCBsPx, gapBetweenPCBsPx));
            }
            Bitmap tfBmp = (shouldInvert) ? Utils.TransformInvert(paddedBmp) : paddedBmp;

            // Calculate pixel width and height
            int oriWorkImageWidth = (int)(MainForm.workImagePhysWidthMm * (rDPI / 25.4f));
            int oriWorkImageHeight = (int)(MainForm.workImagePhysHeightMm * (rDPI / 25.4f));

            // Stack
            Bitmap totalBmp = new Bitmap(oriWorkImageWidth, oriWorkImageHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            if (inputFile == STOUFFER_FILE)
            {
                totalBmp.SetResolution(inkPrinterDpmm, inkPrinterDpmm);
            } else if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
            {
                totalBmp.SetResolution(inkPrinterDpi, inkPrinterDpi);
            }
            else
            {
                totalBmp.SetResolution(inkPrinterDpmm, inkPrinterDpmm);
            }
            using (Graphics gt = Graphics.FromImage(totalBmp))
            {
                gt.Clear((isPhotoresistPositive) ? Color.White : Color.Black);
                int possibleCols = (int)Math.Ceiling((double)oriWorkImageWidth / (double)tfBmp.Width);
                int possibleRows = (int)Math.Ceiling((double)oriWorkImageHeight / (double)tfBmp.Height);
                if ((possibleCols < 2 || possibleRows < 2) && (inputFile != STOUFFER_FILE))
                {
                    //throw new Exception("PCB too large.");
                }
                for (int ix = 0; ix < possibleCols; ix++)
                {
                    for (int iy = 0; iy < possibleRows; iy++)
                    {
                        gt.DrawImage(tfBmp, new Point(ix * (tfBmp.Width + 1), iy * (tfBmp.Height + 1)));
                    }
                }
            }

            // Make an image with anchor holes
            float myWorkImagePhysWidthMm = MainForm.workImagePhysWidthMm + ((stencilMode) ? 20.0f : 0.0f);
            int workImageWidth = (int)(myWorkImagePhysWidthMm * (rDPI / 25.4f));
            int workImageHeight = (int)(MainForm.workImagePhysHeightMm * (rDPI / 25.4f));
            int zagWidth = (int)(workImageWidth + ((MainForm.holderHolesIndentMm + MainForm.spaceOuterMm) * inkPrinterDpmm * 2f));
            int zagHeight = (int)(workImageHeight + ((MainForm.holderHolesIndentMm + MainForm.spaceOuterMm) * inkPrinterDpmm * 2f));
            Bitmap xTotalBmp = new Bitmap(zagWidth, zagHeight, PixelFormat.Format24bppRgb);
            xTotalBmp.SetResolution(totalBmp.HorizontalResolution, totalBmp.VerticalResolution);
            SizeF workImagePhysicalSizeInMm = new SizeF(MainForm.workImagePhysWidthMm, MainForm.workImagePhysHeightMm);
            float stencilDeltaMm = (myWorkImagePhysWidthMm - MainForm.workImagePhysWidthMm) / 2.0f;
            float holderHoleRectTopMm = MainForm.spaceOuterMm;
            float holderHoleRectLeftMm = MainForm.spaceOuterMm + stencilDeltaMm;
            float holderHoleRectLeftNoDeltaMm = MainForm.spaceOuterMm;
            float holderHoleRectRightMm = workImagePhysicalSizeInMm.Width + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f) + stencilDeltaMm;
            float holderHoleRectRightDoubleDeltaMm = myWorkImagePhysWidthMm + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f);
            float holderHoleRectBottomMm = workImagePhysicalSizeInMm.Height + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f);
            using (Graphics xg = Graphics.FromImage(xTotalBmp))
            {
                xg.Clear((isPhotoresistPositive) ? Color.White : Color.Black);
                List<PointF> anchorHolesToDraw = new List<PointF>();
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.0f)));
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.25f)));
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.5f)));
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 1.0f)));
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.5f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 1.0f)));
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 1.0f)));
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.5f)));
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.0f)));
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.5f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.0f)));
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.35f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.0f)));
                anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftMm, holderHoleRectRightMm, 0.2f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.0f)));
                if (stencilMode)
                {
                    anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftNoDeltaMm, holderHoleRectRightDoubleDeltaMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.0f)));
                    anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftNoDeltaMm, holderHoleRectRightDoubleDeltaMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.0f)));
                    anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftNoDeltaMm, holderHoleRectRightDoubleDeltaMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.25f)));
                    anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftNoDeltaMm, holderHoleRectRightDoubleDeltaMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.25f)));
                    anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftNoDeltaMm, holderHoleRectRightDoubleDeltaMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.5f)));
                    anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftNoDeltaMm, holderHoleRectRightDoubleDeltaMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.5f)));
                    anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftNoDeltaMm, holderHoleRectRightDoubleDeltaMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.75f)));
                    anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftNoDeltaMm, holderHoleRectRightDoubleDeltaMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 0.75f)));
                    anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftNoDeltaMm, holderHoleRectRightDoubleDeltaMm, 0.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 1.0f)));
                    anchorHolesToDraw.Add(new PointF(Utils.Lerp(holderHoleRectLeftNoDeltaMm, holderHoleRectRightDoubleDeltaMm, 1.0f), Utils.Lerp(holderHoleRectTopMm, holderHoleRectBottomMm, 1.0f)));
                }
                if (isPhotoresistPositive && !stencilMode)
                {
                    float electrodeWidthMm = MainForm.spaceOuterMm + MainForm.holderHolesIndentMm;
                    float electrodeHeightMm = MainForm.spaceOuterMm + MainForm.holderHolesIndentMm;
                    xg.FillRectangle(Brushes.Black, new Rectangle(0, 0, (int)(electrodeWidthMm * inkPrinterDpmm), (int)(electrodeHeightMm * inkPrinterDpmm)));
                    xg.FillRectangle(Brushes.Black, new Rectangle((int)((holderHoleRectRightMm + MainForm.spaceOuterMm - electrodeWidthMm) * inkPrinterDpmm), 0, (int)(electrodeWidthMm * inkPrinterDpmm), (int)(electrodeHeightMm * inkPrinterDpmm)));
                }
                Pen aPen = new Pen(new SolidBrush(Color.White));
                aPen.Width = inkPrinterDpmm * 0.1f;
                Pen bPen = new Pen(new SolidBrush(Color.White));
                bPen.Width = inkPrinterDpmm * 0.05f;
                foreach (PointF aholeCenter in anchorHolesToDraw)
                {
                    float anchorDiameterMm = MainForm.anchorHoleDiameterMm * 1.5f;
                    RectangleF holeRectPhysicalMm = new RectangleF(new PointF(aholeCenter.X - (anchorDiameterMm / 2.0f), aholeCenter.Y - (anchorDiameterMm / 2.0f)), new SizeF(anchorDiameterMm, anchorDiameterMm));
                    Rectangle holeRectPixel = new Rectangle((int)(holeRectPhysicalMm.X * inkPrinterDpmm), (int)(holeRectPhysicalMm.Y * inkPrinterDpmm), (int)(holeRectPhysicalMm.Width * inkPrinterDpmm), (int)(holeRectPhysicalMm.Height * inkPrinterDpmm));
                    xg.DrawEllipse(aPen, holeRectPixel);
                    xg.FillEllipse(Brushes.Black, holeRectPixel);
                    xg.DrawLine(bPen, new Point(holeRectPixel.Left, (int)((float)holeRectPixel.Top + (holeRectPixel.Height / 2f))), new Point(holeRectPixel.Right, (int)((float)holeRectPixel.Top + (holeRectPixel.Height / 2f))));
                    xg.DrawLine(bPen, new Point((int)((float)holeRectPixel.Left + (holeRectPixel.Width / 2f)), holeRectPixel.Top), new Point((int)((float)holeRectPixel.Left + (holeRectPixel.Width / 2f)), holeRectPixel.Bottom));
                }
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                float emSize = (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 1.2f;
                Rectangle textBounds = new Rectangle((int)(holderHoleRectLeftMm * inkPrinterDpmm) + (int)(((holderHoleRectRightMm - holderHoleRectLeftMm) * inkPrinterDpmm) / 2f), 0, (int)(((holderHoleRectRightMm - holderHoleRectLeftMm) * inkPrinterDpmm) / 2f), (int)((MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * inkPrinterDpmm));
                if (text.Length > 0) xg.DrawString(text, new Font(FontFamily.GenericSansSerif, emSize, FontStyle.Regular), (isPhotoresistPositive) ? Brushes.Black : Brushes.White, textBounds, sf);
                
                xg.DrawImage(totalBmp, new Point((int)((MainForm.holderHolesIndentMm + MainForm.spaceOuterMm + stencilDeltaMm) * inkPrinterDpmm), (int)((MainForm.holderHolesIndentMm + MainForm.spaceOuterMm) * inkPrinterDpmm)));
            }

            // Free resources
            gerberVS.UnloadAllLayers(proj);

            // Warp
            if (compensationMatrix.Count >= 8)
            {
                PointF[] src = new PointF[4];
                src[0] = new PointF(holderHoleRectLeftMm * inkPrinterDpmm, holderHoleRectTopMm * inkPrinterDpmm);
                src[1] = new PointF(holderHoleRectRightMm * inkPrinterDpmm, holderHoleRectTopMm * inkPrinterDpmm);
                src[2] = new PointF(holderHoleRectLeftMm * inkPrinterDpmm, holderHoleRectBottomMm * inkPrinterDpmm);
                src[3] = new PointF(holderHoleRectRightMm * inkPrinterDpmm, holderHoleRectBottomMm * inkPrinterDpmm);
                PointF[] dst = new PointF[4];
                dst[0] = new PointF(compensationMatrix[0] * inkPrinterDpmm, compensationMatrix[1] * inkPrinterDpmm);
                dst[1] = new PointF(compensationMatrix[2] * inkPrinterDpmm, compensationMatrix[3] * inkPrinterDpmm);
                dst[2] = new PointF(compensationMatrix[4] * inkPrinterDpmm, compensationMatrix[5] * inkPrinterDpmm);
                dst[3] = new PointF(compensationMatrix[6] * inkPrinterDpmm, compensationMatrix[7] * inkPrinterDpmm);
                Image<Bgr, Byte> myImage = xTotalBmp.ToImage<Bgr, Byte>();
                using (var matrix = CvInvoke.GetPerspectiveTransform(src, dst))
                {
                    using (var warpedImage = new Mat())
                    {
                        CvInvoke.WarpPerspective(myImage, warpedImage, matrix, new Size(xTotalBmp.Width, xTotalBmp.Height), Inter.Cubic);
                        Bitmap wBmp = warpedImage.ToBitmap();
                        if (shouldMirror)
                        {
                            wBmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        return wBmp;
                    }
                }
            }

            // Return raw
            if (shouldMirror)
            {
                xTotalBmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            return xTotalBmp;
        }

        private void clbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUIDisplay();
        }

        private void tbBottomText1_Leave(object sender, EventArgs e)
        {
            RefreshUIRender();
            RefreshUIDisplay();
        }

        private void tbBottomText2_Leave(object sender, EventArgs e)
        {
            RefreshUIRender();
            RefreshUIDisplay();
        }

        private void tbTopText1_Leave(object sender, EventArgs e)
        {
            RefreshUIRender();
            RefreshUIDisplay();
        }

        private void tbTopText2_Leave(object sender, EventArgs e)
        {
            RefreshUIRender();
            RefreshUIDisplay();
        }

        private void cbCompensateCNCLoss_CheckedChanged(object sender, EventArgs e)
        {
            RefreshUIRender();
            RefreshUIDisplay();
        }

        private void btnBrowseMatrix_Click(object sender, EventArgs e)
        {
            ofDialog.Reset();
            ofDialog.RestoreDirectory = true;
            ofDialog.ValidateNames = true;
            ofDialog.CheckFileExists = true;
            ofDialog.Title = "Открыть матрицу";
            ofDialog.DefaultExt = "*.json";
            ofDialog.AddExtension = true;
            ofDialog.SupportMultiDottedExtensions = false;
            ofDialog.Filter = "JSON файлы|*.json";
            ofDialog.FilterIndex = 0;
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                tbMatrixPath.Text = ofDialog.FileName;
            }
        }

        private void btnCloseWin_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSaveImg_Click(object sender, EventArgs e)
        {
            if (loadedLayers.Count < 1)
            {
                MessageBox.Show(this, "Слои не загружены.");
                return;
            }
            if (clbLayers.SelectedIndices.Count < 1)
            {
                MessageBox.Show(this, "Ничего не выбрано для сохранения.");
                return;
            }
            int selLayerIdx = clbLayers.SelectedIndex;
            if (selLayerIdx > (loadedLayers.Count - 1)) selLayerIdx = loadedLayers.Count - 1;
            if (selLayerIdx < 0) selLayerIdx = 0;
            Bitmap? bmpAtIdx = loadedLayers[selLayerIdx].bitmap;
            if (bmpAtIdx != null)
            {
                if (imgSaveDialog.ShowDialog() == DialogResult.OK)
                {
                    bmpAtIdx.Save(imgSaveDialog.FileName);
                }
            } else
            {
                MessageBox.Show(this, "Bitmap = null.");
                return;
            }
        }

        private void redButton1_Click(object sender, EventArgs e)
        {
            // Выбранный слой
            if (loadedLayers.Count < 1)
            {
                MessageBox.Show(this, "Слои не загружены.");
                return;
            }
            if (clbLayers.SelectedIndices.Count < 1)
            {
                MessageBox.Show(this, "Ничего не выбрано для сохранения.");
                return;
            }
            if ((compensateByMatrix.Length > 0) && (compensationMatrix.Count > 0))
            {
                MessageBox.Show(this, "Отключите компенсацию, сделаете на следующем экране.");
                return;
            }
            int selLayerIdx = clbLayers.SelectedIndex;
            if (selLayerIdx > (loadedLayers.Count - 1)) selLayerIdx = loadedLayers.Count - 1;
            if (selLayerIdx < 0) selLayerIdx = 0;

            // Размеры пикселя указать
            lcdParamsForm = new LCDParamsForm();
            if (lcdParamsForm?.ShowDialog() == DialogResult.OK)
            {
                // Render And Open
                int minUm = Math.Min(lcdParamsForm.pxWidthUm, lcdParamsForm.pxHeightUm);
                int rndDPI = (int)(25400f / minUm);
                lcdForm = new LCDForm();
                lcdForm.panelBitmapAtHighDPI = DrawPanelFromGbrLayerFile(loadedLayers[selLayerIdx].fileName, gbrProfileFile, isPhotoresistPositive, !loadedLayers[selLayerIdx].shouldMirror, (loadedLayers[selLayerIdx].shouldMirror) ? bottomText : topText, rndDPI);
                lcdForm.pxWidthUm = lcdParamsForm.pxWidthUm;
                lcdForm.pxHeightUm = lcdParamsForm.pxHeightUm;
                String clFName = Path.GetFileName(loadedLayers[selLayerIdx].fileName);
                lcdForm.stencilMode = clFName.ToLower().Contains("solderpaste") || clFName.ToLower().EndsWith(".gtp") || clFName.ToLower().EndsWith(".gbp");
                lcdForm?.ShowDialog();
            }
        }
    }
}