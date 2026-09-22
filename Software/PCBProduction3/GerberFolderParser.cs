using Emgu.CV.Dnn;
using GerberVS;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    public class ELayer
    {
        public String fileName = "";
        public bool isCopperOuter = false;
        public bool isCopperInner = false;
        public int copperIdx = 0;
        public int totalIdx = 0;
        public bool shouldMirror = false;
        public Bitmap? bitmap;

        public ELayer(String fileName, bool isCopperOuter, bool isCopperInner, int copperIdx, int totalIdx, bool shouldMirror)
        {
            this.fileName = fileName;
            this.isCopperOuter = isCopperOuter;
            this.isCopperInner = isCopperInner;
            this.copperIdx = copperIdx;
            this.totalIdx = totalIdx;
            this.shouldMirror = shouldMirror;
            this.bitmap = null;
        }
    }

    public class GerberFolderParser
    {
        public static LibGerberVS gerberVS = new LibGerberVS();
        public static String gbrProfileFile = "";
        public static String gbrDrillFile = "";
        public static String mntFile = "";
        public static String mnbFile = "";
        public static String gbrMaskBottomFile = "";
        public static String gbrMaskTopFile = "";
        public static List<String> copperOuterLayers = new List<String>();
        public static List<String> nonCopperLayers = new List<String>();
        public static List<String> copperInnerLayers = new List<String>();
        public static List<ELayer> loadedLayers = new List<ELayer>();
        public const String STOUFFER_FILE = "stouffer.dat";
        public const String WHITE_FILE = "white.dat";
        public const String DRILLS_TOP_FILE = "drills_top.dat";
        public const String DRILLS_BOTTOM_FILE = "drills_bottom.dat";
        public const float allowSoldermaskTentingHoleDiameterLessThanMm = 3f;

        public static void LoadLayers(String gerberFilePath)
        {
            // Gerber Files
            List<String> sortedList = Directory.GetFiles(gerberFilePath).ToList();
            sortedList.Sort();
            gbrProfileFile = "";
            gbrDrillFile = "";
            mntFile = "";
            mnbFile = "";
            gbrMaskBottomFile = "";
            gbrMaskTopFile = "";
            copperOuterLayers.Clear();
            copperInnerLayers.Clear();
            nonCopperLayers.Clear();
            loadedLayers.Clear();
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
                        bool isBottom = (pFile.ToLower().Contains("bottom") || pFile.ToLower().EndsWith(".gbl") || pFile.ToLower().EndsWith(".gbo") || pFile.ToLower().EndsWith(".gbs") || pFile.ToLower().EndsWith(".gbp"));
                        bool isItSoldermask = (pFile.ToLower().Contains("soldermask") || pFile.ToLower().EndsWith(".gbs") || pFile.ToLower().EndsWith(".gts"));
                        bool isItOuterCopper = (pFile.ToLower().Contains("copper_top") || pFile.ToLower().Contains("copper_bottom") || pFile.ToLower().EndsWith(".gbl") || pFile.ToLower().EndsWith(".gtl"));
                        bool isItInnerCopper = (pFile.ToLower().Contains("copper_inner") || pFile.ToLower().EndsWith(".g1") || pFile.ToLower().EndsWith(".g2") || pFile.ToLower().EndsWith(".g3") || pFile.ToLower().EndsWith(".g4") || pFile.ToLower().EndsWith(".g5") || pFile.ToLower().EndsWith(".g6") || pFile.ToLower().EndsWith(".gp1") || pFile.ToLower().EndsWith(".gp2") || pFile.ToLower().EndsWith(".gp3") || pFile.ToLower().EndsWith(".gp4") || pFile.ToLower().EndsWith(".gp5") || pFile.ToLower().EndsWith(".gp6"));
                        if (isItOuterCopper)
                        {
                            copperOuterLayers.Add(pFile);
                        }
                        else if (isItInnerCopper)
                        {
                            copperInnerLayers.Add(pFile);
                        }
                        else
                        {
                            nonCopperLayers.Add(pFile);
                            if (isItSoldermask)
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
                        }
                    }
                }
                else if (pFile.ToLower().EndsWith(".xln") || pFile.ToLower().EndsWith(".drl"))
                {
                    gbrDrillFile = pFile;
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
                bool shouldMirror = (fn.ToLower().Contains("bottom") || fn.ToLower().EndsWith(".gbo") || fn.ToLower().EndsWith(".gbs") || fn.ToLower().EndsWith(".gbp"));
                if (fn.ToLower().EndsWith(".gbp") || fn.ToLower().EndsWith(".gtp") || fn.ToLower().Contains("solderpaste"))
                {
                    loadedLayers.Add(new ELayer(fn, false, false, 0, totalIdxCounter, shouldMirror));
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

            // Белый
            loadedLayers.Add(new ELayer(WHITE_FILE, false, false, 0, totalIdxCounter, false));
            totalIdxCounter++;

            // Отверстия
            loadedLayers.Add(new ELayer(DRILLS_TOP_FILE, false, false, 0, totalIdxCounter, false));
            totalIdxCounter++;
            loadedLayers.Add(new ELayer(DRILLS_BOTTOM_FILE, false, false, 0, totalIdxCounter, true));
            totalIdxCounter++;
        }

        public static Bitmap DrawPanelFromGbrLayerFile(String inputFile, String profileFile, bool shouldInvert, bool shouldMirror, String text, int rDPI)
        {
            Bitmap? layerImg = null;
            try
            {
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
                    }
                    else
                    {
                        maskCopperFile = outerLoadedLayers[1].fileName;
                    }
                }

                // Stouffer
                if (inputFile == WHITE_FILE)
                {
                    float myWorkImagePhysWidthMmX = MainForm.workImagePhysWidthMm + 20.0f;
                    int workImageWidthX = (int)(myWorkImagePhysWidthMmX * (rDPI / 25.4f));
                    int workImageHeightX = (int)(MainForm.workImagePhysHeightMm * (rDPI / 25.4f));
                    int zagWidthX = (int)(workImageWidthX + ((MainForm.holderHolesIndentMm + MainForm.spaceOuterMm) * inkPrinterDpmm * 2f));
                    int zagHeightX = (int)(workImageHeightX + ((MainForm.holderHolesIndentMm + MainForm.spaceOuterMm) * inkPrinterDpmm * 2f));
                    Bitmap xTotalBmpX = new Bitmap(zagWidthX, zagHeightX, PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(xTotalBmpX))
                    {
                        g.Clear(Color.White);
                    }
                    return xTotalBmpX;
                } else if (inputFile == STOUFFER_FILE)
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
                        sg.Clear((shouldInvert) ? Color.White : Color.Black);
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
                                if (shouldInvert)
                                {
                                    W = 255 - W;
                                }
                                RectangleF lorect = new Rectangle((int)(imWidth * sx / 5f), (int)(imHeight * sy / 5f), (int)(imWidth / 5f), (int)(imHeight / 5f));
                                sg.FillRectangle(new SolidBrush(Color.FromArgb(W, W, W)), lorect);
                                sg.DrawString((sqIdx + 1).ToString(), fnt, (shouldInvert) ? Brushes.White : Brushes.Black, lorect);
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
                    layerImg = new Bitmap(Utils.Dilation(Utils.And(rawLayerImg1, rawLayerImg2), 0, shouldInvert), new Size((int)Math.Floor(imWidth), (int)Math.Floor(imHeight)));
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
                    using (Bitmap rawLayerImg1 = new Bitmap((int)Math.Floor(2f * imWidth), (int)Math.Floor(2f * imHeight), System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        using (Bitmap rawLayerImg2 = new Bitmap((maskMode) ? (int)Math.Floor(2f * imWidth) : 1, (maskMode) ? (int)Math.Floor(2f * imHeight) : 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                        {
                            using (Bitmap rawLayerImg3 = new Bitmap((maskMode) ? (int)Math.Floor(2f * imWidth) : 1, (maskMode) ? (int)Math.Floor(2f * imHeight) : 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                            {
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
                                layerImg = new Bitmap(Utils.Dilation((maskMode) ? Utils.AndOr(rawLayerImg1, rawLayerImg2, rawLayerImg3) : rawLayerImg1, dilationVal, shouldInvert), new Size((int)Math.Floor(imWidth), (int)Math.Floor(imHeight)));
                                if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                                {
                                    layerImg.SetResolution(inkPrinterDpi, inkPrinterDpi);
                                }
                                else
                                {
                                    layerImg.SetResolution(inkPrinterDpmm, inkPrinterDpmm);
                                }
                            }
                        }
                    }
                }

                // Pre-process layer image
                float myWorkImagePhysWidthMm = MainForm.workImagePhysWidthMm + ((stencilMode) ? 20.0f : 0.0f);
                int workImageWidth = (int)(myWorkImagePhysWidthMm * (rDPI / 25.4f));
                int workImageHeight = (int)(MainForm.workImagePhysHeightMm * (rDPI / 25.4f));
                int zagWidth = (int)(workImageWidth + ((MainForm.holderHolesIndentMm + MainForm.spaceOuterMm) * inkPrinterDpmm * 2f));
                int zagHeight = (int)(workImageHeight + ((MainForm.holderHolesIndentMm + MainForm.spaceOuterMm) * inkPrinterDpmm * 2f));
                Bitmap xTotalBmp = new Bitmap(zagWidth, zagHeight, PixelFormat.Format24bppRgb);
                int gapBetweenPCBsPx = (int)Math.Ceiling(MainForm.gapBetweenPCBsMm * inkPrinterDpmm);
                using (Bitmap paddedBmp = new Bitmap(layerImg.Width + (gapBetweenPCBsPx * 2), layerImg.Height + (gapBetweenPCBsPx * 2)))
                {
                    paddedBmp.SetResolution(layerImg.HorizontalResolution, layerImg.VerticalResolution);
                    using (Graphics gp = Graphics.FromImage(paddedBmp))
                    {
                        gp.Clear(Color.Black);
                        gp.DrawImage(layerImg, new Point(gapBetweenPCBsPx, gapBetweenPCBsPx));
                    }
                    using (Bitmap tfBmp = (shouldInvert) ? Utils.TransformInvert(paddedBmp) : paddedBmp)
                    {
                        // Calculate pixel width and height
                        int oriWorkImageWidth = (int)(MainForm.workImagePhysWidthMm * (rDPI / 25.4f));
                        int oriWorkImageHeight = (int)(MainForm.workImagePhysHeightMm * (rDPI / 25.4f));

                        // Stack
                        using (Bitmap totalBmp = new Bitmap(oriWorkImageWidth, oriWorkImageHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                        {
                            if (inputFile == STOUFFER_FILE)
                            {
                                totalBmp.SetResolution(inkPrinterDpmm, inkPrinterDpmm);
                            }
                            else if (proj.FileInfo.First().Image.Unit == GerberUnit.Inch)
                            {
                                totalBmp.SetResolution(inkPrinterDpi, inkPrinterDpi);
                            }
                            else
                            {
                                totalBmp.SetResolution(inkPrinterDpmm, inkPrinterDpmm);
                            }
                            using (Graphics gt = Graphics.FromImage(totalBmp))
                            {
                                gt.Clear((shouldInvert) ? Color.White : Color.Black);
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
                                xg.Clear((shouldInvert) ? Color.White : Color.Black);
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
                                if (shouldInvert && !stencilMode)
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
                                if (text.Length > 0) xg.DrawString(text, new Font(FontFamily.GenericSansSerif, emSize, FontStyle.Regular), (shouldInvert) ? Brushes.Black : Brushes.White, textBounds, sf);

                                xg.DrawImage(totalBmp, new Point((int)((MainForm.holderHolesIndentMm + MainForm.spaceOuterMm + stencilDeltaMm) * inkPrinterDpmm), (int)((MainForm.holderHolesIndentMm + MainForm.spaceOuterMm) * inkPrinterDpmm)));
                            }
                        }
                    }
                }

                // Free resources
                gerberVS.UnloadAllLayers(proj);

                // Return raw
                if (shouldMirror)
                {
                    xTotalBmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                layerImg?.Dispose();
                return xTotalBmp;
            } catch (Exception exc)
            {
                layerImg?.Dispose();
                throw;
            }
        }
    }
}
