using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using GerberVS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace PCBProduction2
{
    public partial class LCDForm : Form
    {
        // Gerber
        public LibGerberVS gerberVS = new LibGerberVS();

        // Params
        public Bitmap? panelBitmapAtHighDPI;
        public Bitmap? panelBitmapWarped;
        public Bitmap? panelBitmapScaledRectPx;
        public Bitmap? panelChannelConverted;
        public int pxWidthUm = 19;
        public int pxHeightUm = 24;
        public bool stencilMode = false;

        // UI Controls
        public int rTicks = 0;
        public int maxTicks = 2;
        public int selAnchor = 1;
        public bool hasPendingRerender = false;
        public PointF anchorLeftTopLocationMm = PointF.Empty;
        public PointF anchorRightTopLocationMm = PointF.Empty;
        public PointF anchorLeftBottomLocationMm = PointF.Empty;
        public PointF anchorRightBottomLocationMm = PointF.Empty;

        // Timer
        public int exposureTimeSec = 0;

        // Camera Capture
        public object lockObject = new object();
        public VideoCapture? cncCamCapture1;
        public VideoCapture? cncCamCapture2;
        public VideoCapture? cncCamCapture3;
        public VideoCapture? cncCamCapture4;
        public Mat curRawFrame1 = new Mat();
        public Mat curRawFrame2 = new Mat();
        public Mat curRawFrame3 = new Mat();
        public Mat curRawFrame4 = new Mat();
        public Mat curGrayFrame1 = new Mat();
        public Mat curGrayFrame2 = new Mat();
        public Mat curGrayFrame3 = new Mat();
        public Mat curGrayFrame4 = new Mat();
        public bool cncConnected = false;

        // Индекс камеры каждого из реперных отверстий
        public int tlCamIdx = -1;
        public int trCamIdx = -1;
        public int blCamIdx = -1;
        public int brCamIdx = -1;

        // Сколько пикселей камеры в миллиметре видимой ей LCD-матрицы
        public float tlCamPxPerMmX = 0f;
        public float tlCamPxPerMmY = 0f;
        public float trCamPxPerMmX = 0f;
        public float trCamPxPerMmY = 0f;
        public float blCamPxPerMmX = 0f;
        public float blCamPxPerMmY = 0f;
        public float brCamPxPerMmX = 0f;
        public float brCamPxPerMmY = 0f;

        // Где сейчас по координатной системе камеры находится каждый из реперных знаков
        public PointF curAnchorPosTLCam = new PointF(0f, 0f);
        public PointF curAnchorPosTRCam = new PointF(0f, 0f);
        public PointF curAnchorPosBLCam = new PointF(0f, 0f);
        public PointF curAnchorPosBRCam = new PointF(0f, 0f);

        // Auto Detect
        public Thread? autoDetectThread;
        public bool shouldAutoDetectStop = false;
        public bool autoDetectPending = false;

        // Рисовать шахматкой
        public bool drawTLAsCB = false;
        public bool drawTRAsCB = false;
        public bool drawBLAsCB = false;
        public bool drawBRAsCB = false;

        // Определение шахматки по камерам
        public bool cbDetectionOn = false;
        public PointF cb1 = new PointF(0f, 0f);
        public PointF cb2 = new PointF(0f, 0f);
        public PointF cb3 = new PointF(0f, 0f);
        public PointF cb4 = new PointF(0f, 0f);

        // Pattern
        public Size patternSizeToDetect = new Size(5, 3);
        public int patternWidth = 6;
        public int patternHeight = 4;

        // Date
        private static DateTime JanFirst1970 = new DateTime(1970, 1, 1);
        public static long GetEpochTime()
        {
            return (long)((DateTime.Now.ToUniversalTime() - JanFirst1970).TotalSeconds + 0.5);
        }

        public LCDForm()
        {
            InitializeComponent();
        }

        public void AutoDetectProcess()
        {
            try
            {
                // Начало
                autoDetectPending = true;
                UpdateUI();
                
                // Цикл
                for (int i = 0; i < 4; i++)
                {
                    // Начальное положение
                    SetInitialAnchorValues();

                    // Какой реперный знак рисуем шахматкой
                    drawTLAsCB = (i <= 0);
                    drawTRAsCB = (i == 1);
                    drawBLAsCB = (i == 2);
                    drawBRAsCB = (i >= 3);

                    // Сбрасываем состояние детектора в камерах
                    cncCamCapture1?.Pause();
                    cncCamCapture2?.Pause();
                    cncCamCapture3?.Pause();
                    cncCamCapture4?.Pause();
                    lock (lockObject) {
                        cbDetectionOn = false;
                        cb1 = new Point(0, 0);
                        cb2 = new Point(0, 0);
                        cb3 = new Point(0, 0);
                        cb4 = new Point(0, 0);
                    }
                    cncCamCapture1?.Start();
                    cncCamCapture2?.Start();
                    cncCamCapture3?.Start();
                    cncCamCapture4?.Start();
                    if (shouldAutoDetectStop) break;

                    // Рисуем и отображаем
                    RenderImg();
                    for (int yy = 0; yy < 10; yy++)
                    {
                        Thread.Sleep(350);
                    }
                    if (shouldAutoDetectStop) break;

                    // Ждем когда камеры увидят
                    cbDetectionOn = true;
                    int timeout_counter = 0;
                    while (cb1.X <= 0 && cb1.Y <= 0 && cb2.X <= 0 && cb2.Y <= 0 && cb3.X <= 0 && cb3.Y <= 0 && cb4.X <= 0 && cb4.Y <= 0)
                    {
                        Thread.Sleep(100);
                        timeout_counter++;
                        if (timeout_counter >= 100 || shouldAutoDetectStop)
                        {
                            break;
                        }
                    }
                    cbDetectionOn = false;
                    if (shouldAutoDetectStop) break;

                    // Что увидели? Индекс камеры.
                    int curAnchorCamIdx = -1;
                    PointF curAnchorPositionInCamView = new PointF(0f, 0f);
                    if (cb1.X > 0 && cb1.Y > 0)
                    {
                        curAnchorCamIdx = 1;
                        curAnchorPositionInCamView = cb1;
                    }
                    else if (cb2.X > 0 && cb2.Y > 0)
                    {
                        curAnchorCamIdx = 2;
                        curAnchorPositionInCamView = cb2;
                    }
                    else if (cb3.X > 0 && cb3.Y > 0)
                    {
                        curAnchorCamIdx = 3;
                        curAnchorPositionInCamView = cb3;
                    }
                    else if (cb4.X > 0 && cb4.Y > 0)
                    {
                        curAnchorCamIdx = 4;
                        curAnchorPositionInCamView = cb4;
                    }
                    else
                    {
                        curAnchorCamIdx = -1;
                        throw new Exception("Не обнаружен шахматный рисунок ни по одной из камер (этап 1) - калибровка не удалась.");
                    }

                    // Записываем индекс и местоположение
                    if (i <= 0)
                    {
                        tlCamIdx = curAnchorCamIdx;
                        curAnchorPosTLCam = curAnchorPositionInCamView;
                    }
                    else if (i == 1)
                    {
                        trCamIdx = curAnchorCamIdx;
                        curAnchorPosTRCam = curAnchorPositionInCamView;
                    }
                    else if (i == 2)
                    {
                        blCamIdx = curAnchorCamIdx;
                        curAnchorPosBLCam = curAnchorPositionInCamView;
                    }
                    else
                    {
                        brCamIdx = curAnchorCamIdx;
                        curAnchorPosBRCam = curAnchorPositionInCamView;
                    }

                    // Сдвигаем этот реперный знак
                    if (i <= 0)
                    {
                        anchorLeftTopLocationMm = new PointF(anchorLeftTopLocationMm.X - 1.0f, anchorLeftTopLocationMm.Y - 1.0f);
                    }
                    else if (i == 1)
                    {
                        anchorRightTopLocationMm = new PointF(anchorRightTopLocationMm.X + 1.0f, anchorRightTopLocationMm.Y - 1.0f);
                    }
                    else if (i == 2)
                    {
                        anchorLeftBottomLocationMm = new PointF(anchorLeftBottomLocationMm.X - 1.0f, anchorLeftBottomLocationMm.Y + 1.0f);
                    }
                    else
                    {
                        anchorRightBottomLocationMm = new PointF(anchorRightBottomLocationMm.X + 1.0f, anchorRightBottomLocationMm.Y + 1.0f);
                    }

                    // Сбрасываем состояние детектора в камерах
                    cncCamCapture1?.Pause();
                    cncCamCapture2?.Pause();
                    cncCamCapture3?.Pause();
                    cncCamCapture4?.Pause();
                    lock (lockObject) {
                        cbDetectionOn = false;
                        cb1 = new Point(0, 0);
                        cb2 = new Point(0, 0);
                        cb3 = new Point(0, 0);
                        cb4 = new Point(0, 0);
                    }
                    cncCamCapture1?.Start();
                    cncCamCapture2?.Start();
                    cncCamCapture3?.Start();
                    cncCamCapture4?.Start();
                    if (shouldAutoDetectStop) break;

                    // Рисуем и отображаем заново
                    RenderImg();
                    for (int yy = 0; yy < 10; yy++)
                    {
                        Thread.Sleep(450);
                    }
                    if (shouldAutoDetectStop) break;

                    // Ждем когда камеры увидят
                    cbDetectionOn = true;
                    timeout_counter = 0;
                    while (cb1.X <= 0 && cb1.Y <= 0 && cb2.X <= 0 && cb2.Y <= 0 && cb3.X <= 0 && cb3.Y <= 0 && cb4.X <= 0 && cb4.Y <= 0)
                    {
                        Thread.Sleep(100);
                        timeout_counter++;
                        if (timeout_counter >= 100 || shouldAutoDetectStop)
                        {
                            break;
                        }
                    }
                    cbDetectionOn = false;
                    if (shouldAutoDetectStop) break;

                    // Новое местоположение
                    int newAnchorCamIdx = -1;
                    PointF newAnchorPositionInCamView = new PointF(0f, 0f);
                    if (cb1.X > 0 && cb1.Y > 0)
                    {
                        newAnchorCamIdx = 1;
                        newAnchorPositionInCamView = cb1;
                    }
                    else if (cb2.X > 0 && cb2.Y > 0)
                    {
                        newAnchorCamIdx = 2;
                        newAnchorPositionInCamView = cb2;
                    }
                    else if (cb3.X > 0 && cb3.Y > 0)
                    {
                        newAnchorCamIdx = 3;
                        newAnchorPositionInCamView = cb3;
                    }
                    else if (cb4.X > 0 && cb4.Y > 0)
                    {
                        newAnchorCamIdx = 4;
                        newAnchorPositionInCamView = cb4;
                    }
                    else
                    {
                        newAnchorCamIdx = -1;
                        newAnchorPositionInCamView = new Point(0, 0);
                        throw new Exception("Не обнаружен шахматный рисунок ни по одной из камер (этап 2) - калибровка не удалась.");
                    }

                    // Проверка
                    if (curAnchorCamIdx != newAnchorCamIdx)
                    {
                        throw new Exception("Сдвинутый реперный знак найден на другой камере - калибровка не удалась.");
                    }

                    // Сравниваем и вычисляем пиксели на мм
                    if (i <= 0)
                    {
                        tlCamPxPerMmX = -(newAnchorPositionInCamView.X - curAnchorPositionInCamView.X);
                        tlCamPxPerMmY = -(newAnchorPositionInCamView.Y - curAnchorPositionInCamView.Y);
                    }
                    else if (i == 1)
                    {
                        trCamPxPerMmX = newAnchorPositionInCamView.X - curAnchorPositionInCamView.X;
                        trCamPxPerMmY = -(newAnchorPositionInCamView.Y - curAnchorPositionInCamView.Y);
                    }
                    else if (i == 2)
                    {
                        blCamPxPerMmX = -(newAnchorPositionInCamView.X - curAnchorPositionInCamView.X);
                        blCamPxPerMmY = newAnchorPositionInCamView.Y - curAnchorPositionInCamView.Y;
                    }
                    else
                    {
                        brCamPxPerMmX = newAnchorPositionInCamView.X - curAnchorPositionInCamView.X;
                        brCamPxPerMmY = newAnchorPositionInCamView.Y - curAnchorPositionInCamView.Y;
                    }
                }
                if (shouldAutoDetectStop)
                {
                    throw new Exception("Операция отменена.");
                }
                ShowInitialWorkPosInPictureBox();
                SaveCalibrationData();
                ShowMessageBox("Автокалибровка успешна.");
            }
            catch (Exception ex) {
                if (!shouldAutoDetectStop)
                {
                    ShowMessageBox(ex.Message);
                }
            }
            finally
            {
                try
                {
                    cncCamCapture1?.Set(CapProp.Exposure, 0.0f);
                    cncCamCapture2?.Set(CapProp.Exposure, 0.0f);
                    cncCamCapture3?.Set(CapProp.Exposure, 0.0f);
                    cncCamCapture4?.Set(CapProp.Exposure, 0.0f);
                }
                catch { }
                autoDetectPending = false;
                drawTLAsCB = false;
                drawTRAsCB = false;
                drawBLAsCB = false;
                drawBRAsCB = false;
                if (!shouldAutoDetectStop)
                {
                    SetInitialAnchorValues();
                    RenderImg();
                    UpdateUI();
                }
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

        public bool IsAutoDetectPending()
        {
            return autoDetectPending;
        }

        public bool HasAutoDetectSucceeded()
        {
            bool f0 = tlCamIdx > 0 && trCamIdx > 0 && blCamIdx > 0 && brCamIdx > 0;
            bool f1 = tlCamPxPerMmX != 0f && tlCamPxPerMmY != 0f && trCamPxPerMmX != 0f && trCamPxPerMmY != 0f && blCamPxPerMmX != 0f && blCamPxPerMmY != 0f && brCamPxPerMmX != 0f && brCamPxPerMmY != 0f;
            bool f2 = curAnchorPosTLCam.X > 0 && curAnchorPosTLCam.Y > 0 && curAnchorPosTRCam.X > 0 && curAnchorPosTRCam.Y > 0 && curAnchorPosBLCam.X > 0 && curAnchorPosBLCam.Y > 0 && curAnchorPosBRCam.X > 0 && curAnchorPosBRCam.Y > 0;
            return f0 && f1 && f2;
        }

        public void UpdateUI()
        {
            if (InvokeRequired)
            {
                Invoke(UpdateUI);
            }
            else
            {
                String lblText = "";
                lblText += "Левый верхний X: " + anchorLeftTopLocationMm.X.ToString("0.00") + ", Y: " + anchorLeftTopLocationMm.Y.ToString("0.00") + "\n";
                lblText += "Правый верхний X: " + anchorRightTopLocationMm.X.ToString("0.00") + ", Y: " + anchorRightTopLocationMm.Y.ToString("0.00") + "\n";
                lblText += "Левый нижний X: " + anchorLeftBottomLocationMm.X.ToString("0.00") + ", Y: " + anchorLeftBottomLocationMm.Y.ToString("0.00") + "\n";
                lblText += "Правый нижний X: " + anchorRightBottomLocationMm.X.ToString("0.00") + ", Y: " + anchorRightBottomLocationMm.Y.ToString("0.00") + "\n";
                lblText += "Выбранное отверстие: " + selAnchor.ToString() + "\n";
                lblText += "Идет рендеринг: " + ((hasPendingRerender) ? ((rTicks >= maxTicks) ? "да" : "в очереди") : "нет") + "\n";
                lblAnchors.Text = lblText;
                String calText = "";
                if (IsAutoDetectPending())
                {
                    calText = "Выполняется автокалибровка...";
                }
                else if (HasAutoDetectSucceeded())
                {
                    calText += "Наведите крест точно на реперный знак и нажмите V, чтобы скорректировать небольшую погрешность автокалибровки.\nДалее наведите крест на отверстия в заготовке и нажмите N чтобы выровнять рисунок." + "\n";
                    calText += "Начальная рабочая позиция TopLeft: " + curAnchorPosTLCam.X.ToString() + ", " + curAnchorPosTLCam.Y.ToString() + "\n";
                    calText += "Начальная рабочая позиция TopRight: " + curAnchorPosTRCam.X.ToString() + ", " + curAnchorPosTRCam.Y.ToString() + "\n";
                    calText += "Начальная рабочая позиция BottomLeft: " + curAnchorPosBLCam.X.ToString() + ", " + curAnchorPosBLCam.Y.ToString() + "\n";
                    calText += "Начальная рабочая позиция BottomRight: " + curAnchorPosBRCam.X.ToString() + ", " + curAnchorPosBRCam.Y.ToString() + "\n";
                }
                else
                {
                    calText = "Автокалибровка не удалась, доступно только ручное перемещение реперных знаков.";
                }
                lblAutoAnchors.Text = calText;
            }
        }

        public void SetInitialAnchorValues()
        {
            float myWorkImagePhysWidthMm = MainForm.workImagePhysWidthMm + ((stencilMode) ? 20.0f : 0.0f);
            float stencilDeltaMm = (myWorkImagePhysWidthMm - MainForm.workImagePhysWidthMm) / 2.0f;
            float holderHoleRectTopMm = MainForm.spaceOuterMm;
            float holderHoleRectLeftMm = MainForm.spaceOuterMm + stencilDeltaMm;
            float holderHoleRectRightMm = MainForm.workImagePhysWidthMm + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f) + stencilDeltaMm;
            float holderHoleRectBottomMm = MainForm.workImagePhysHeightMm + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f);
            anchorLeftTopLocationMm = new PointF(holderHoleRectLeftMm, holderHoleRectTopMm);
            anchorRightTopLocationMm = new PointF(holderHoleRectRightMm, holderHoleRectTopMm);
            anchorLeftBottomLocationMm = new PointF(holderHoleRectLeftMm, holderHoleRectBottomMm);
            anchorRightBottomLocationMm = new PointF(holderHoleRectRightMm, holderHoleRectBottomMm);
            UpdateUI();
        }

        public bool AreAnchorsAtInitialPosition()
        {
            float myWorkImagePhysWidthMm = MainForm.workImagePhysWidthMm + ((stencilMode) ? 20.0f : 0.0f);
            float stencilDeltaMm = (myWorkImagePhysWidthMm - MainForm.workImagePhysWidthMm) / 2.0f;
            float holderHoleRectTopMm = MainForm.spaceOuterMm;
            float holderHoleRectLeftMm = MainForm.spaceOuterMm + stencilDeltaMm;
            float holderHoleRectRightMm = MainForm.workImagePhysWidthMm + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f) + stencilDeltaMm;
            float holderHoleRectBottomMm = MainForm.workImagePhysHeightMm + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f);
            if (Math.Abs(anchorLeftTopLocationMm.X - holderHoleRectLeftMm) < 0.01f && Math.Abs(anchorLeftTopLocationMm.Y - holderHoleRectTopMm) < 0.01f)
            {
                if (Math.Abs(anchorRightTopLocationMm.X - holderHoleRectRightMm) < 0.01f && Math.Abs(anchorRightTopLocationMm.Y - holderHoleRectTopMm) < 0.01f)
                {
                    if (Math.Abs(anchorLeftBottomLocationMm.X - holderHoleRectLeftMm) < 0.01f && Math.Abs(anchorLeftBottomLocationMm.Y - holderHoleRectBottomMm) < 0.01f)
                    {
                        if (Math.Abs(anchorRightBottomLocationMm.X - holderHoleRectRightMm) < 0.01f && Math.Abs(anchorRightBottomLocationMm.Y - holderHoleRectBottomMm) < 0.01f)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void DrawCheckerboard(Graphics g, PointF ptCenter, float cellSizePx)
        {
            float aOriginXPx = (ptCenter.X - (((float)patternWidth / 2.0f) * cellSizePx));
            float aOriginYPx = (ptCenter.Y - (((float)patternHeight / 2.0f) * cellSizePx));
            for (int ax = 0; ax < patternWidth; ax++)
            {
                for (int ay = 0; ay < patternHeight; ay++)
                {
                    bool isWhite = false;
                    if (ax % 2 == 0)
                    {
                        isWhite = ay % 2 == 0;
                    } else
                    {
                        isWhite = ay % 2 != 0;
                    }
                    g.FillRectangle((isWhite) ? Brushes.White : Brushes.Black, new RectangleF((aOriginXPx + (ax * cellSizePx)), (aOriginYPx + (ay * cellSizePx)), cellSizePx, cellSizePx));
                }
            }
        }

        public unsafe void RenderImg()
        {
            // Update UI
            UpdateUI();

            // Check
            if (panelBitmapAtHighDPI == null)
            {
                return;
            }
            if (panelBitmapWarped != null)
            {
                panelBitmapWarped.Dispose();
                panelBitmapWarped = null;
            }
            if (panelBitmapScaledRectPx != null)
            {
                panelBitmapScaledRectPx.Dispose();
                panelBitmapScaledRectPx = null;
            }
            if (panelChannelConverted != null)
            {
                panelChannelConverted.Dispose();
                panelChannelConverted = null;
            }

            // Warpage Calculate
            float dispDotsPerMm = 1000f / (float)Math.Min(pxWidthUm, pxHeightUm);
            float myWorkImagePhysWidthMm = MainForm.workImagePhysWidthMm + ((stencilMode) ? 20.0f : 0.0f);
            float stencilDeltaMm = (myWorkImagePhysWidthMm - MainForm.workImagePhysWidthMm) / 2.0f;
            float holderHoleRectTopMm = MainForm.spaceOuterMm;
            float holderHoleRectLeftMm = MainForm.spaceOuterMm + stencilDeltaMm;
            float holderHoleRectRightMm = MainForm.workImagePhysWidthMm + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f) + stencilDeltaMm;
            float holderHoleRectBottomMm = MainForm.workImagePhysHeightMm + MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f);
            PointF[] src = new PointF[4];
            src[0] = new PointF(holderHoleRectLeftMm * dispDotsPerMm, holderHoleRectTopMm * dispDotsPerMm);
            src[1] = new PointF(holderHoleRectRightMm * dispDotsPerMm, holderHoleRectTopMm * dispDotsPerMm);
            src[2] = new PointF(holderHoleRectLeftMm * dispDotsPerMm, holderHoleRectBottomMm * dispDotsPerMm);
            src[3] = new PointF(holderHoleRectRightMm * dispDotsPerMm, holderHoleRectBottomMm * dispDotsPerMm);
            PointF[] dst = new PointF[4];
            dst[0] = new PointF(anchorLeftTopLocationMm.X * dispDotsPerMm, anchorLeftTopLocationMm.Y * dispDotsPerMm);
            dst[1] = new PointF(anchorRightTopLocationMm.X * dispDotsPerMm, anchorRightTopLocationMm.Y * dispDotsPerMm);
            dst[2] = new PointF(anchorLeftBottomLocationMm.X * dispDotsPerMm, anchorLeftBottomLocationMm.Y * dispDotsPerMm);
            dst[3] = new PointF(anchorRightBottomLocationMm.X * dispDotsPerMm, anchorRightBottomLocationMm.Y * dispDotsPerMm);

            // Warpage Do
            if (drawTLAsCB || drawTRAsCB || drawBLAsCB || drawBRAsCB)
            {
                // Replace Anchors With Checkerboard (If Needed)
                panelBitmapWarped = new Bitmap(panelBitmapAtHighDPI.Width, panelBitmapAtHighDPI.Height, PixelFormat.Format24bppRgb);
                using (Graphics pim = Graphics.FromImage(panelBitmapWarped))
                {
                    int aidx = 0;
                    foreach (PointF anchor in dst)
                    {
                        bool shouldDrawThisAnchor = (drawTLAsCB && aidx == 0) || (drawTRAsCB && aidx == 1) || (drawBLAsCB && aidx == 2) || (drawBRAsCB && aidx == 3);
                        if (shouldDrawThisAnchor)
                        {
                            DrawCheckerboard(pim, anchor, dispDotsPerMm);
                        }
                        aidx++;
                    }
                }
            }
            else
            {
                // Warp (Move Anchors)
                if (src == null || dst == null) return;
                Image<Bgr, Byte> myImage = panelBitmapAtHighDPI.ToImage<Bgr, Byte>();
                using (var matrix = CvInvoke.GetPerspectiveTransform(src, dst))
                {
                    if (matrix == null) return;
                    using (var warpedImage = new Mat())
                    {
                        CvInvoke.WarpPerspective(myImage, warpedImage, matrix, new Size(panelBitmapAtHighDPI.Width, panelBitmapAtHighDPI.Height), Inter.Cubic);
                        panelBitmapWarped = warpedImage.ToBitmap();
                    }
                }
            }

            // Scale To Compensate For Rectangular Pixels
            if (pxWidthUm != pxHeightUm)
            {
                float scaleFactor = (float)Math.Min(pxWidthUm, pxHeightUm) / (float)Math.Max(pxWidthUm, pxHeightUm);
                if (pxWidthUm > pxHeightUm)
                {
                    // Scale Down X
                    panelBitmapScaledRectPx = new Bitmap(panelBitmapWarped, new Size((int)(panelBitmapWarped.Width * scaleFactor), panelBitmapWarped.Height));
                }
                else
                {
                    // Scale Down Y
                    panelBitmapScaledRectPx = new Bitmap(panelBitmapWarped, new Size(panelBitmapWarped.Width, (int)(panelBitmapWarped.Height * scaleFactor)));
                }
            }
            else
            {
                panelBitmapScaledRectPx = new Bitmap(panelBitmapWarped);
            }

            // Compress To RGB
            panelChannelConverted = new Bitmap((int)Math.Ceiling(panelBitmapScaledRectPx.Width / 3f), panelBitmapScaledRectPx.Height, PixelFormat.Format24bppRgb);
            BitmapData bitmapDataDst = panelChannelConverted.LockBits(new Rectangle(0, 0, panelChannelConverted.Width, panelChannelConverted.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bitmapDataSrc = panelBitmapScaledRectPx.LockBits(new Rectangle(0, 0, panelBitmapScaledRectPx.Width, panelBitmapScaledRectPx.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte* imgStartPtrSrc = (byte*)bitmapDataSrc.Scan0;
            byte* imgStartPtrDst = (byte*)bitmapDataDst.Scan0;
            for (int x = 0; x < bitmapDataSrc.Width; x += 3)
            {
                for (int y = 0; y < bitmapDataSrc.Height; y++)
                {
                    int xst = (x / 3);
                    long pxindexsrc1 = ((y * bitmapDataSrc.Stride) + (x * 3)) + 2;
                    long pxindexsrc2 = ((y * bitmapDataSrc.Stride) + ((x + 1) * 3)) + 2;
                    long pxindexsrc3 = ((y * bitmapDataSrc.Stride) + ((x + 2) * 3)) + 2;
                    long pxindexdst1 = ((y * bitmapDataDst.Stride) + (xst * 3)) + 2;
                    long pxindexdst2 = ((y * bitmapDataDst.Stride) + (xst * 3)) + 1;
                    long pxindexdst3 = ((y * bitmapDataDst.Stride) + (xst * 3)) + 0;
                    imgStartPtrDst[pxindexdst1] = imgStartPtrSrc[pxindexsrc1];
                    imgStartPtrDst[pxindexdst2] = imgStartPtrSrc[pxindexsrc2];
                    imgStartPtrDst[pxindexdst3] = imgStartPtrSrc[pxindexsrc3];
                }
            }
            panelChannelConverted.UnlockBits(bitmapDataDst);
            panelBitmapScaledRectPx.UnlockBits(bitmapDataSrc);

            // Display
            pbExpImg.Image = panelChannelConverted;

            // Finalize
            hasPendingRerender = false;
            rTicks = 0;
            UpdateUI();
        }

        private void LCDForm_Shown(object sender, EventArgs e)
        {
            SetInitialAnchorValues();
            RenderImg();
            bool res = AdjustDisplayAndFormSize();
            if (res)
            {
                try
                {
                    ConnectCam();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                    Close();
                    return;
                }
            }
        }

        public bool AdjustDisplayAndFormSize()
        {
            if (panelChannelConverted == null)
            {
                return false;
            }

            // Поиск мониторов и задание размера формы
            if (Screen.AllScreens.Length != 2)
            {
                MessageBox.Show(this, "К вашей системе подключено неверное количество мониторов, их должно быть два: для управления и засветки.");
                Close();
                return false;
            }
            Screen? controlScreen = Screen.AllScreens.MinBy((e) => e.Bounds.Width * e.Bounds.Height);
            Screen? exposureScreen = Screen.AllScreens.MaxBy((e) => e.Bounds.Width * e.Bounds.Height);
            if (controlScreen?.DeviceName == exposureScreen?.DeviceName)
            {
                MessageBox.Show(this, "Мониторы для управления и засветки должны иметь разное разрешение.");
                Close();
                return false;
            }
            if (exposureScreen!.Bounds.Left != controlScreen!.Bounds.Right && controlScreen.Bounds.Left != exposureScreen.Bounds.Right)
            {
                MessageBox.Show(this, "Несоответствие bounds мониторов по X.");
                Close();
                return false;
            }
            if (exposureScreen.Bounds.Top != controlScreen.Bounds.Top)
            {
                MessageBox.Show(this, "Несоответствие bounds мониторов по Y.");
                Close();
                return false;
            }
            try
            {
                this.Location = new Point(Math.Min(exposureScreen.Bounds.Left, controlScreen.Bounds.Left), Math.Min(exposureScreen.Bounds.Top, controlScreen.Bounds.Top));
                this.Width = controlScreen.Bounds.Width + exposureScreen.Bounds.Width;
                this.Height = Math.Max(controlScreen.Bounds.Height, exposureScreen.Bounds.Height);
            }
            catch { }

            // Ставим PictureBox посередине Exposure Screen
            if (panelChannelConverted.Width > exposureScreen.Bounds.Width || panelChannelConverted.Height > exposureScreen.Bounds.Height)
            {
                float scaleFactor = (float)exposureScreen.Bounds.Height / (float)panelChannelConverted.Height;
                pbExpImg.Width = (int)(panelChannelConverted.Width * scaleFactor);
                pbExpImg.Height = (int)(panelChannelConverted.Height * scaleFactor);
            }
            else
            {
                pbExpImg.Width = panelChannelConverted.Width;
                pbExpImg.Height = panelChannelConverted.Height;
            }
            pbExpImg.Left = (int)(exposureScreen.Bounds.Left + ((exposureScreen.Bounds.Width - pbExpImg.Width) / 2f));
            pbExpImg.Top = (int)(exposureScreen.Bounds.Top + ((exposureScreen.Bounds.Height - pbExpImg.Height) / 2f));

            // Ставим GroupBox посередине Control Screen
            gbControls.Left = (int)(controlScreen.Bounds.Left + ((controlScreen.Bounds.Width - gbControls.Width) / 2f));
            gbControls.Top = (int)(controlScreen.Bounds.Top + ((controlScreen.Bounds.Height - gbControls.Height) / 2f));

            return true;
        }

        private void timerDisplayResolution_Tick(object sender, EventArgs e)
        {
            if (hasPendingRerender && !IsAutoDetectPending())
            {
                if (rTicks < maxTicks)
                {
                    rTicks++;
                }
                else
                {
                    RenderImg();
                }
            }
        }

        public void CncCamCapture_ImageGrabbed1(object? sender, EventArgs e)
        {
            lock (lockObject)
            {
                bool frameRetrFlag = false;
                try
                {
                    frameRetrFlag = cncCamCapture1?.Retrieve(curRawFrame1) ?? false;
                    if (frameRetrFlag)
                    {
                        // Detect
                        if (cbDetectionOn && cb1.X <= 0 && cb1.Y <= 0)
                        {
                            var cornerPoints = new VectorOfPoint();
                            curGrayFrame1 = curRawFrame1.Split()[2];
                            bool result = CvInvoke.FindChessboardCornersSB(curGrayFrame1, patternSizeToDetect, cornerPoints);
                            if (result && cornerPoints.Size > 0)
                            {
                                cb1 = AveragePoint(cornerPoints);
                            }
                        }

                        // Отобразить
                        if (pbPreview1.Image != null)
                        {
                            pbPreview1.Image.Dispose();
                        }
                        try
                        {
                            if (curRawFrame1 != null)
                            {
                                pbPreview1.Image = curRawFrame1.ToBitmap();
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
                catch (Exception ex) { }
            }
        }

        public void CncCamCapture_ImageGrabbed2(object? sender, EventArgs e)
        {
            lock (lockObject)
            {
                bool frameRetrFlag = false;
                try
                {
                    frameRetrFlag = cncCamCapture2?.Retrieve(curRawFrame2) ?? false;
                    if (frameRetrFlag)
                    {
                        // Detect
                        if (cbDetectionOn && cb2.X <= 0 && cb2.Y <= 0)
                        {
                            var cornerPoints = new VectorOfPoint();
                            curGrayFrame2 = curRawFrame2.Split()[2];
                            bool result = CvInvoke.FindChessboardCornersSB(curGrayFrame2, patternSizeToDetect, cornerPoints);
                            if (result && cornerPoints.Size > 0)
                            {
                                cb2 = AveragePoint(cornerPoints);
                            }
                        }

                        // Отобразить
                        if (pbPreview2.Image != null)
                        {
                            pbPreview2.Image.Dispose();
                        }
                        try
                        {
                            if (curRawFrame2 != null)
                            {
                                pbPreview2.Image = curRawFrame2.ToBitmap();
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
                catch (Exception ex) { }
            }
        }

        public void CncCamCapture_ImageGrabbed3(object? sender, EventArgs e)
        {
            lock (lockObject)
            {
                bool frameRetrFlag = false;
                try
                {
                    frameRetrFlag = cncCamCapture3?.Retrieve(curRawFrame3) ?? false;
                    if (frameRetrFlag)
                    {
                        // Detect
                        if (cbDetectionOn && cb3.X <= 0 && cb3.Y <= 0)
                        {
                            var cornerPoints = new VectorOfPoint();
                            curGrayFrame3 = curRawFrame3.Split()[2];
                            bool result = CvInvoke.FindChessboardCornersSB(curGrayFrame3, patternSizeToDetect, cornerPoints);
                            if (result && cornerPoints.Size > 0)
                            {
                                cb3 = AveragePoint(cornerPoints);
                            }
                        }

                        // Отобразить
                        if (pbPreview3.Image != null)
                        {
                            pbPreview3.Image.Dispose();
                        }
                        try
                        {
                            if (curRawFrame3 != null)
                            {
                                pbPreview3.Image = curRawFrame3.ToBitmap();
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
                catch (Exception ex) { }
            }
        }

        public void CncCamCapture_ImageGrabbed4(object? sender, EventArgs e)
        {
            lock (lockObject)
            {
                bool frameRetrFlag = false;
                try
                {
                    frameRetrFlag = cncCamCapture4?.Retrieve(curRawFrame4) ?? false;
                    if (frameRetrFlag)
                    {
                        // Detect
                        if (cbDetectionOn && cb4.X <= 0 && cb4.Y <= 0)
                        {
                            var cornerPoints = new VectorOfPoint();
                            curGrayFrame4 = curRawFrame4.Split()[2];
                            bool result = CvInvoke.FindChessboardCornersSB(curGrayFrame4, patternSizeToDetect, cornerPoints);
                            if (result && cornerPoints.Size > 0)
                            {
                                cb4 = AveragePoint(cornerPoints);
                            }
                        }

                        // Отобразить
                        if (pbPreview4.Image != null)
                        {
                            pbPreview4.Image.Dispose();
                        }
                        try
                        {
                            if (curRawFrame4 != null)
                            {
                                pbPreview4.Image = curRawFrame4.ToBitmap();
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
                catch (Exception ex) { }
            }
        }

        public PointF AveragePoint(VectorOfPoint vop)
        {
            float sX = 0.0f;
            float sY = 0.0f;
            for (int i = 0; i < vop.Size; i++)
            {
                sX += vop[i].X / (float)vop.Size;
                sY += vop[i].Y / (float)vop.Size;
            }
            return new Point((int)sX, (int)sY);
        }

        private void ConnectCam()
        {
            // Матрицы
            curRawFrame1 = new Mat();
            curRawFrame2 = new Mat();
            curRawFrame3 = new Mat();
            curRawFrame4 = new Mat();

            // PictureBox
            pbPreview1.Image = new Bitmap(1920, 1080);
            pbPreview2.Image = new Bitmap(1920, 1080);
            pbPreview3.Image = new Bitmap(1920, 1080);
            pbPreview4.Image = new Bitmap(1920, 1080);

            // Камера 1
            cncCamCapture1 = new VideoCapture(0, VideoCapture.API.DShow);
            cncCamCapture1.Set(CapProp.FrameWidth, 1920);
            cncCamCapture1.Set(CapProp.FrameHeight, 1080);
            cncCamCapture1.Set(CapProp.Exposure, -0.7f);
            cncCamCapture1.ExceptionMode = true;
            int attempts = 0;
            while (!cncCamCapture1.IsOpened && attempts < 3)
            {
                Thread.Sleep(300);
                attempts++;
            }
            if (!cncCamCapture1.IsOpened)
            {
                throw new Exception("Не удалось обнаружить камеру 1.");
            }
            cncCamCapture1.ImageGrabbed += CncCamCapture_ImageGrabbed1;
            cncCamCapture1.Start();

            // Камера 2
            cncCamCapture2 = new VideoCapture(1, VideoCapture.API.DShow);
            cncCamCapture2.Set(CapProp.FrameWidth, 1920);
            cncCamCapture2.Set(CapProp.FrameHeight, 1080);
            cncCamCapture2.Set(CapProp.Exposure, -0.7f);
            cncCamCapture2.ExceptionMode = true;
            attempts = 0;
            while (!cncCamCapture2.IsOpened && attempts < 3)
            {
                Thread.Sleep(300);
                attempts++;
            }
            if (!cncCamCapture2.IsOpened)
            {
                try
                {
                    if (cncCamCapture1 != null) cncCamCapture1.Stop();
                }
                catch (Exception excpt) { }
                throw new Exception("Не удалось обнаружить камеру 2.");
            }
            cncCamCapture2.ImageGrabbed += CncCamCapture_ImageGrabbed2;
            cncCamCapture2.Start();

            // Камера 3
            cncCamCapture3 = new VideoCapture(2, VideoCapture.API.DShow);
            cncCamCapture3.Set(CapProp.FrameWidth, 1920);
            cncCamCapture3.Set(CapProp.FrameHeight, 1080);
            cncCamCapture3.Set(CapProp.Exposure, -0.7f);
            cncCamCapture3.ExceptionMode = true;
            attempts = 0;
            while (!cncCamCapture3.IsOpened && attempts < 3)
            {
                Thread.Sleep(300);
                attempts++;
            }
            if (!cncCamCapture3.IsOpened)
            {
                try
                {
                    if (cncCamCapture1 != null) cncCamCapture1.Stop();
                }
                catch (Exception excpt) { }
                try
                {
                    if (cncCamCapture2 != null) cncCamCapture2.Stop();
                }
                catch (Exception excpt) { }
                throw new Exception("Не удалось обнаружить камеру 3.");
            }
            cncCamCapture3.ImageGrabbed += CncCamCapture_ImageGrabbed3;
            cncCamCapture3.Start();

            // Камера 4
            cncCamCapture4 = new VideoCapture(3, VideoCapture.API.DShow);
            cncCamCapture4.Set(CapProp.FrameWidth, 1920);
            cncCamCapture4.Set(CapProp.FrameHeight, 1080);
            cncCamCapture4.Set(CapProp.Exposure, -0.7f);
            cncCamCapture4.ExceptionMode = true;
            attempts = 0;
            while (!cncCamCapture4.IsOpened && attempts < 3)
            {
                Thread.Sleep(300);
                attempts++;
            }
            if (!cncCamCapture4.IsOpened)
            {
                try
                {
                    if (cncCamCapture1 != null) cncCamCapture1.Stop();
                }
                catch (Exception excpt) { }
                try
                {
                    if (cncCamCapture2 != null) cncCamCapture2.Stop();
                }
                catch (Exception excpt) { }
                try
                {
                    if (cncCamCapture3 != null) cncCamCapture3.Stop();
                }
                catch (Exception excpt) { }
                throw new Exception("Не удалось обнаружить камеру 4.");
            }
            cncCamCapture4.ImageGrabbed += CncCamCapture_ImageGrabbed4;
            cncCamCapture4.Start();

            // Подключено
            cncConnected = true;

            // Auto Detect
            if (!LoadCalibrationData())
            {
                shouldAutoDetectStop = false;
                autoDetectThread = new Thread(AutoDetectProcess);
                autoDetectThread.Start();
            } else
            {
                ShowInitialWorkPosInPictureBox();
                UpdateUI();
            }
        }

        private void DisconnectCam()
        {
            shouldAutoDetectStop = true;
            if (IsAutoDetectPending())
            {
                try
                {
                    autoDetectThread?.Join();
                } catch (Exception excpt) { }
            }
            try
            {
                if (cncCamCapture1 != null) cncCamCapture1.Stop();
            }
            catch (Exception excpt) { }
            try
            {
                if (cncCamCapture2 != null) cncCamCapture2.Stop();
            }
            catch (Exception excpt) { }
            try
            {
                if (cncCamCapture3 != null) cncCamCapture3.Stop();
            }
            catch (Exception excpt) { }
            try
            {
                if (cncCamCapture4 != null) cncCamCapture4.Stop();
            }
            catch (Exception excpt) { }
            Thread.Sleep(1500);
            try
            {
                curRawFrame1.Dispose();
                curRawFrame2.Dispose();
                curRawFrame3.Dispose();
                curRawFrame4.Dispose();
            } catch {}
            cncConnected = false;
        }

        private void LCDForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DisconnectCam();
            }
            catch (Exception excpt) { }
            expTimer.Enabled = false;
            Cursor.Show();
        }

        private void expTimer_Tick(object sender, EventArgs e)
        {
            exposureTimeSec++;
            lblExpTime.Text = TimeSpan.FromSeconds(exposureTimeSec).ToString(@"mm\:ss");
        }

        private void LCDForm_KeyDown(object sender, KeyEventArgs e)
        {
            bool shouldUpdateUI = false;
            if (hasPendingRerender && (rTicks >= maxTicks))
            {
                return;
            }
            if (e.KeyCode == Keys.D1)
            {
                selAnchor = 1;
                shouldUpdateUI = true;
            }
            else if (e.KeyCode == Keys.D2)
            {
                selAnchor = 2;
                shouldUpdateUI = true;
            }
            else if (e.KeyCode == Keys.D3)
            {
                selAnchor = 3;
                shouldUpdateUI = true;
            }
            else if (e.KeyCode == Keys.D4)
            {
                selAnchor = 4;
                shouldUpdateUI = true;
            }
            else if (e.KeyCode == Keys.D5)
            {
                if (expTimer.Enabled)
                {
                    expTimer.Enabled = false;
                    Cursor.Show();
                }
                else
                {
                    exposureTimeSec = 0;
                    lblExpTime.Text = "00:00";
                    expTimer.Enabled = true;
                    Cursor.Hide();
                }
                shouldUpdateUI = true;
            }
            else if (e.KeyCode == Keys.D6)
            {
                Close();
            }
            else if (e.KeyCode == Keys.W || e.KeyCode == Keys.S || e.KeyCode == Keys.A || e.KeyCode == Keys.D)
            {
                if (IsAutoDetectPending())
                {
                    return;
                }
                float dX = ((e.KeyCode == Keys.A) ? -pxWidthUm : pxWidthUm) / 1000.0f;
                float dY = ((e.KeyCode == Keys.W) ? -pxHeightUm : pxHeightUm) / 1000.0f;
                if (e.KeyCode == Keys.W || e.KeyCode == Keys.S)
                {
                    dX = 0f;
                }
                else
                {
                    dY = 0f;
                }
                if (selAnchor == 1)
                {
                    anchorLeftTopLocationMm = new PointF(anchorLeftTopLocationMm.X + dX, anchorLeftTopLocationMm.Y + dY);
                }
                else if (selAnchor == 2)
                {
                    anchorRightTopLocationMm = new PointF(anchorRightTopLocationMm.X + dX, anchorRightTopLocationMm.Y + dY);
                }
                else if (selAnchor == 3)
                {
                    anchorLeftBottomLocationMm = new PointF(anchorLeftBottomLocationMm.X + dX, anchorLeftBottomLocationMm.Y + dY);
                }
                else if (selAnchor == 4)
                {
                    anchorRightBottomLocationMm = new PointF(anchorRightBottomLocationMm.X + dX, anchorRightBottomLocationMm.Y + dY);
                }
                hasPendingRerender = true;
                rTicks = 0;
                shouldUpdateUI = true;
            }
            else if (e.KeyCode == Keys.V) {
                if (IsAutoDetectPending())
                {
                    return;
                }
                if (!HasAutoDetectSucceeded())
                {
                    MessageBox.Show(this, "Автокалибровка не пройдена, доступно только ручное управление.");
                    return;
                }
                if (tlCamIdx == 1)
                {
                    curAnchorPosTLCam = pbPreview1.VisibleCenterPoint;
                } else if (tlCamIdx == 2)
                {
                    curAnchorPosTLCam = pbPreview2.VisibleCenterPoint;
                } else if (tlCamIdx == 3)
                {
                    curAnchorPosTLCam = pbPreview3.VisibleCenterPoint;
                } else
                {
                    curAnchorPosTLCam = pbPreview4.VisibleCenterPoint;
                }
                if (trCamIdx == 1)
                {
                    curAnchorPosTRCam = pbPreview1.VisibleCenterPoint;
                }
                else if (trCamIdx == 2)
                {
                    curAnchorPosTRCam = pbPreview2.VisibleCenterPoint;
                }
                else if (trCamIdx == 3)
                {
                    curAnchorPosTRCam = pbPreview3.VisibleCenterPoint;
                }
                else
                {
                    curAnchorPosTRCam = pbPreview4.VisibleCenterPoint;
                }
                if (blCamIdx == 1)
                {
                    curAnchorPosBLCam = pbPreview1.VisibleCenterPoint;
                }
                else if (blCamIdx == 2)
                {
                    curAnchorPosBLCam = pbPreview2.VisibleCenterPoint;
                }
                else if (blCamIdx == 3)
                {
                    curAnchorPosBLCam = pbPreview3.VisibleCenterPoint;
                }
                else
                {
                    curAnchorPosBLCam = pbPreview4.VisibleCenterPoint;
                }
                if (brCamIdx == 1)
                {
                    curAnchorPosBRCam = pbPreview1.VisibleCenterPoint;
                }
                else if (brCamIdx == 2)
                {
                    curAnchorPosBRCam = pbPreview2.VisibleCenterPoint;
                }
                else if (brCamIdx == 3)
                {
                    curAnchorPosBRCam = pbPreview3.VisibleCenterPoint;
                }
                else
                {
                    curAnchorPosBRCam = pbPreview4.VisibleCenterPoint;
                }
                SaveCalibrationData();
                shouldUpdateUI = true;
            }
            else if (e.KeyCode == Keys.N)
            {
                if (IsAutoDetectPending())
                {
                    return;
                }
                if (!HasAutoDetectSucceeded())
                {
                    MessageBox.Show(this, "Автокалибровка не пройдена, доступно только ручное управление.");
                    return;
                }
                Point workFinalPosTLCam = new Point();
                Point workFinalPosTRCam = new Point();
                Point workFinalPosBLCam = new Point();
                Point workFinalPosBRCam = new Point();
                if (tlCamIdx == 1)
                {
                    workFinalPosTLCam = pbPreview1.VisibleCenterPoint;
                }
                else if (tlCamIdx == 2)
                {
                    workFinalPosTLCam = pbPreview2.VisibleCenterPoint;
                }
                else if (tlCamIdx == 3)
                {
                    workFinalPosTLCam = pbPreview3.VisibleCenterPoint;
                }
                else if (tlCamIdx == 4)
                {
                    workFinalPosTLCam = pbPreview4.VisibleCenterPoint;
                }
                if (trCamIdx == 1)
                {
                    workFinalPosTRCam = pbPreview1.VisibleCenterPoint;
                }
                else if (trCamIdx == 2)
                {
                    workFinalPosTRCam = pbPreview2.VisibleCenterPoint;
                }
                else if (trCamIdx == 3)
                {
                    workFinalPosTRCam = pbPreview3.VisibleCenterPoint;
                }
                else if (trCamIdx == 4)
                {
                    workFinalPosTRCam = pbPreview4.VisibleCenterPoint;
                }
                if (blCamIdx == 1)
                {
                    workFinalPosBLCam = pbPreview1.VisibleCenterPoint;
                }
                else if (blCamIdx == 2)
                {
                    workFinalPosBLCam = pbPreview2.VisibleCenterPoint;
                }
                else if (blCamIdx == 3)
                {
                    workFinalPosBLCam = pbPreview3.VisibleCenterPoint;
                }
                else if (blCamIdx == 4)
                {
                    workFinalPosBLCam = pbPreview4.VisibleCenterPoint;
                }
                if (brCamIdx == 1)
                {
                    workFinalPosBRCam = pbPreview1.VisibleCenterPoint;
                }
                else if (brCamIdx == 2)
                {
                    workFinalPosBRCam = pbPreview2.VisibleCenterPoint;
                }
                else if (brCamIdx == 3)
                {
                    workFinalPosBRCam = pbPreview3.VisibleCenterPoint;
                }
                else if (brCamIdx == 4)
                {
                    workFinalPosBRCam = pbPreview4.VisibleCenterPoint;
                }
                PointF tlDiffMm = new PointF((((float)(workFinalPosTLCam.X - curAnchorPosTLCam.X)) / tlCamPxPerMmX), (((float)(workFinalPosTLCam.Y - curAnchorPosTLCam.Y)) / tlCamPxPerMmY));
                PointF trDiffMm = new PointF((((float)(workFinalPosTRCam.X - curAnchorPosTRCam.X)) / trCamPxPerMmX), (((float)(workFinalPosTRCam.Y - curAnchorPosTRCam.Y)) / trCamPxPerMmY));
                PointF blDiffMm = new PointF((((float)(workFinalPosBLCam.X - curAnchorPosBLCam.X)) / blCamPxPerMmX), (((float)(workFinalPosBLCam.Y - curAnchorPosBLCam.Y)) / blCamPxPerMmY));
                PointF brDiffMm = new PointF((((float)(workFinalPosBRCam.X - curAnchorPosBRCam.X)) / brCamPxPerMmX), (((float)(workFinalPosBRCam.Y - curAnchorPosBRCam.Y)) / brCamPxPerMmY));
                SetInitialAnchorValues();
                anchorLeftTopLocationMm = new PointF(anchorLeftTopLocationMm.X + tlDiffMm.X, anchorLeftTopLocationMm.Y + tlDiffMm.Y);
                anchorRightTopLocationMm = new PointF(anchorRightTopLocationMm.X + trDiffMm.X, anchorRightTopLocationMm.Y + trDiffMm.Y);
                anchorLeftBottomLocationMm = new PointF(anchorLeftBottomLocationMm.X + blDiffMm.X, anchorLeftBottomLocationMm.Y + blDiffMm.Y);
                anchorRightBottomLocationMm = new PointF(anchorRightBottomLocationMm.X + brDiffMm.X, anchorRightBottomLocationMm.Y + brDiffMm.Y);
                hasPendingRerender = true;
                rTicks = 0;
                shouldUpdateUI = true;
            }
            if (shouldUpdateUI)
            {
                UpdateUI();
            }
        }

        private void ShowInitialWorkPosInPictureBox()
        {
            if (InvokeRequired)
            {
                this.Invoke(ShowInitialWorkPosInPictureBox);
            }
            else
            {
                if (curAnchorPosTLCam.X > 0 && curAnchorPosTLCam.Y > 0)
                {
                    if (tlCamIdx == 1)
                    {
                        pbPreview1.SetTransformationSoThatCenterIs(curAnchorPosTLCam);
                    }
                    else if (tlCamIdx == 2)
                    {
                        pbPreview2.SetTransformationSoThatCenterIs(curAnchorPosTLCam);
                    }
                    else if (tlCamIdx == 3)
                    {
                        pbPreview3.SetTransformationSoThatCenterIs(curAnchorPosTLCam);
                    }
                    else if (tlCamIdx == 4)
                    {
                        pbPreview4.SetTransformationSoThatCenterIs(curAnchorPosTLCam);
                    }
                }

                if (curAnchorPosTRCam.X > 0 && curAnchorPosTRCam.Y > 0)
                {
                    if (trCamIdx == 1)
                    {
                        pbPreview1.SetTransformationSoThatCenterIs(curAnchorPosTRCam);
                    }
                    else if (trCamIdx == 2)
                    {
                        pbPreview2.SetTransformationSoThatCenterIs(curAnchorPosTRCam);
                    }
                    else if (trCamIdx == 3)
                    {
                        pbPreview3.SetTransformationSoThatCenterIs(curAnchorPosTRCam);
                    }
                    else if (trCamIdx == 4)
                    {
                        pbPreview4.SetTransformationSoThatCenterIs(curAnchorPosTRCam);
                    }
                }

                if (curAnchorPosBLCam.X > 0 && curAnchorPosBLCam.Y > 0)
                {
                    if (blCamIdx == 1)
                    {
                        pbPreview1.SetTransformationSoThatCenterIs(curAnchorPosBLCam);
                    }
                    else if (blCamIdx == 2)
                    {
                        pbPreview2.SetTransformationSoThatCenterIs(curAnchorPosBLCam);
                    }
                    else if (blCamIdx == 3)
                    {
                        pbPreview3.SetTransformationSoThatCenterIs(curAnchorPosBLCam);
                    }
                    else if (blCamIdx == 4)
                    {
                        pbPreview4.SetTransformationSoThatCenterIs(curAnchorPosBLCam);
                    }
                }

                if (curAnchorPosBRCam.X > 0 && curAnchorPosBRCam.Y > 0)
                {
                    if (brCamIdx == 1)
                    {
                        pbPreview1.SetTransformationSoThatCenterIs(curAnchorPosBRCam);
                    }
                    else if (brCamIdx == 2)
                    {
                        pbPreview2.SetTransformationSoThatCenterIs(curAnchorPosBRCam);
                    }
                    else if (brCamIdx == 3)
                    {
                        pbPreview3.SetTransformationSoThatCenterIs(curAnchorPosBRCam);
                    }
                    else if (brCamIdx == 4)
                    {
                        pbPreview4.SetTransformationSoThatCenterIs(curAnchorPosBRCam);
                    }
                }
            }
        }

        public bool LoadCalibrationData()
        {
            if (File.Exists("lcd.xml"))
            {
                FileStorage storage = new FileStorage("lcd.xml", FileStorage.Mode.Read);
                long epochSecs = storage.GetNode("ticks").ReadInt();
                if ((GetEpochTime() - 300) > epochSecs)
                {
                    storage.ReleaseAndGetString();
                    return false;
                }
                tlCamIdx = storage.GetNode("tl_cam_idx").ReadInt();
                tlCamPxPerMmX = storage.GetNode("tl_cam_px_per_mm_x").ReadFloat(0f);
                tlCamPxPerMmY = storage.GetNode("tl_cam_px_per_mm_y").ReadFloat(0f);
                trCamIdx = storage.GetNode("tr_cam_idx").ReadInt();
                trCamPxPerMmX = storage.GetNode("tr_cam_px_per_mm_x").ReadFloat(0f);
                trCamPxPerMmY = storage.GetNode("tr_cam_px_per_mm_y").ReadFloat(0f);
                blCamIdx = storage.GetNode("bl_cam_idx").ReadInt();
                blCamPxPerMmX = storage.GetNode("bl_cam_px_per_mm_x").ReadFloat(0f);
                blCamPxPerMmY = storage.GetNode("bl_cam_px_per_mm_y").ReadFloat(0f);
                brCamIdx = storage.GetNode("br_cam_idx").ReadInt();
                brCamPxPerMmX = storage.GetNode("br_cam_px_per_mm_x").ReadFloat(0f);
                brCamPxPerMmY = storage.GetNode("br_cam_px_per_mm_y").ReadFloat(0f);
                curAnchorPosTLCam = new PointF(storage.GetNode("cur_anchor_pos_tl_cam_x").ReadFloat(0f), storage.GetNode("cur_anchor_pos_tl_cam_y").ReadFloat(0f));
                curAnchorPosTRCam = new PointF(storage.GetNode("cur_anchor_pos_tr_cam_x").ReadFloat(0f), storage.GetNode("cur_anchor_pos_tr_cam_y").ReadFloat(0f));
                curAnchorPosBLCam = new PointF(storage.GetNode("cur_anchor_pos_bl_cam_x").ReadFloat(0f), storage.GetNode("cur_anchor_pos_bl_cam_y").ReadFloat(0f));
                curAnchorPosBRCam = new PointF(storage.GetNode("cur_anchor_pos_br_cam_x").ReadFloat(0f), storage.GetNode("cur_anchor_pos_br_cam_y").ReadFloat(0f));
                storage.ReleaseAndGetString();
                return true;
            } else
            {
                return false;
            }
        }

        public void SaveCalibrationData()
        {
            FileStorage storage = new FileStorage("lcd.xml", FileStorage.Mode.Write);
            storage.Write(tlCamIdx, "tl_cam_idx");
            storage.Write(tlCamPxPerMmX, "tl_cam_px_per_mm_x");
            storage.Write(tlCamPxPerMmY, "tl_cam_px_per_mm_y");
            storage.Write(trCamIdx, "tr_cam_idx");
            storage.Write(trCamPxPerMmX, "tr_cam_px_per_mm_x");
            storage.Write(trCamPxPerMmY, "tr_cam_px_per_mm_y");
            storage.Write(blCamIdx, "bl_cam_idx");
            storage.Write(blCamPxPerMmX, "bl_cam_px_per_mm_x");
            storage.Write(blCamPxPerMmY, "bl_cam_px_per_mm_y");
            storage.Write(brCamIdx, "br_cam_idx");
            storage.Write(brCamPxPerMmX, "br_cam_px_per_mm_x");
            storage.Write(brCamPxPerMmY, "br_cam_px_per_mm_y");
            storage.Write(curAnchorPosTLCam.X, "cur_anchor_pos_tl_cam_x");
            storage.Write(curAnchorPosTLCam.Y, "cur_anchor_pos_tl_cam_y");
            storage.Write(curAnchorPosTRCam.X, "cur_anchor_pos_tr_cam_x");
            storage.Write(curAnchorPosTRCam.Y, "cur_anchor_pos_tr_cam_y");
            storage.Write(curAnchorPosBLCam.X, "cur_anchor_pos_bl_cam_x");
            storage.Write(curAnchorPosBLCam.Y, "cur_anchor_pos_bl_cam_y");
            storage.Write(curAnchorPosBRCam.X, "cur_anchor_pos_br_cam_x");
            storage.Write(curAnchorPosBRCam.Y, "cur_anchor_pos_br_cam_y");
            storage.Write(GetEpochTime(), "ticks");
            storage.ReleaseAndGetString();
        }
    }
}
// TODO: Add AI hole match

// Try half-additive differential etch.
// ETCH/MICRO-ETCH BY IMMERSION!!!
// Final tech instruction