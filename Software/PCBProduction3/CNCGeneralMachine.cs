using DirectShowLib;
using DirectShowLib.BDA;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    public class ALVertice
    {
        public float locXMm = 0f;
        public float locYMm = 0f;
        public float zTouchLevelMm = 0f;

        public ALVertice(float locXMm, float locYMm, float zTouchLevelMm)
        {
            this.locXMm = locXMm;
            this.locYMm = locYMm;
            this.zTouchLevelMm = zTouchLevelMm;
        }
    }

    public class CNCCameraInfo
    {
        public int framesCaptured = 0;
        public VideoCapture? cncCamCapture;
        public object lockObject = new object();
        public Mat curRawFrame = new Mat();
        public Mat transformMatrix = new Mat();
        public Mat curRotFrame = new Mat();
        public Mat curRszFrame = new Mat();
        public Mat curDGrayFrame = new Mat();
        public Mat curGrayBlurredFrame = new Mat();
        public Mat curThreshFrame = new Mat();
        public Mat curDispFrame = new Mat();
    }

    public class ErrorEventArgs : EventArgs
    {
        public String Msg { get; set; }
        public ErrorEventArgs(String msg)
        {
            Msg = msg;
        }
    }

    public class NewCameraPreviewImageArgs : EventArgs
    {
        public Mat Img { get; set; }
        public NewCameraPreviewImageArgs(Mat img)
        {
            Img = img;
        }
    }

    public class UserInputRequiredArgs : EventArgs
    {
        public int Type { get; set; }
        public String Text { get; set; }
        public List<String> Buttons { get; set; }

        public UserInputRequiredArgs(String text, List<String> buttons, int type)
        {
            Text = text;
            Buttons = buttons;
            Type = type;
        }
    }

    public class CNCGeneralMachine
    {
        // Видео с камеры
        private List<CNCCameraInfo> cameras = new List<CNCCameraInfo>();
        private int displayCamIdx = 0;
        private bool hFlip = false;
        private bool vFlip = false;
        private bool rotateCamImage = false;
        private bool showCV = false;
        private float imageAngleDeg = 0f;
        private float rectWidthMm = 0.0f;
        private float rectHeightMm = 0.0f;
        private float imagePxPerMm = 10f;
        private bool showRect = false;
        private int fovXFromPercent = 0;
        private int fovXToPercent = 100;
        private int fovYFromPercent = 0;
        private int fovYToPercent = 100;
        private System.Threading.Timer? camWatchdogTimer;

        // Отображение изображений
        private float zoomFactor = 1.0f;

        // Калибровка движения
        private float motionStepsPerMmX = 800f;
        private float motionStepsPerMmY = 800f;
        private float motionSkewAngleRad = 0.0f;

        // Grbl подключение и исходящие от него статусы
        private bool cncConnected = false;
        protected bool spindleActive = false;
        private bool cncFault = false;
        private SerialPort? grblPort;
        private int grblCommandCmpl = 0;
        private float accXMmPerSecSq = 0.0f;
        private float accYMmPerSecSq = 0.0f;
        private float accZMmPerSecSq = 0.0f;
        protected float maxTravelMmX = 0.0f;
        protected float maxTravelMmY = 0.0f;
        private float maxTravelMmZ = 0.0f;
        protected float maxFeedRateMmPerMinX = 0.0f;
        protected float maxFeedRateMmPerMinY = 0.0f;
        protected float maxFeedRateMmPerMinZ = 0.0f;
        private float absMmX = 0.0f;
        private float absMmY = 0.0f;
        private float absMmZ = 0.0f;

        // Константы для CNC3018
        private Vector3 homingCorner = new Vector3(0f, 1f, 1f); // X0 - слева, X1 - справа, Y0 - сверху, Y1 - снизу, Z0 - снизу, Z1 - сверху
        private bool xPlusIsRight = true; // X+ направлен вправо
        private bool yPlusIsDown = true; // Y+ направлен вних
        private bool zPlusIsWithdraw = true; // Z+ направлен вверх (выход из заготовки)

        // Константы размера калибровочного рисунка
        private int calibSubpatternCols = 6;
        private int calibSubpatternRows = 4;
        private float calibDistanceBetweenCentersMmW = 130.0f;
        private float calibDistanceBetweenCentersMmH = 230.0f;
        private float calibCellSizeMm = 1.0f;

        // Константы конкретно моего станка
        protected float calibZMm = 18.0f;

        // Инициализация, специфика
        private int camerasNeeded;
        private int previewBoxWidth;
        private int previewBoxHeight;
        private String configFilePath;

        // Обработчики
        protected EventHandler StateChanged;
        protected EventHandler<ErrorEventArgs> ErrorOccurred;
        private EventHandler<NewCameraPreviewImageArgs> NewCameraPreviewImage;
        private EventHandler<UserInputRequiredArgs> UserInputRequired;

        // Статусы
        private Thread? connectThread;
        private Thread? moveThread;
        protected Thread? workThread;
        private bool connectPending = false;
        private bool movePending = false;
        private bool busyWithLongTask = false;
        private int curUserEntry = 0;
        private bool curUserEntryAllowXYMove = false;
        private bool curUserEntryAllowZMove = false;
        private bool curUserEntryAllowSpindle = false;
        protected bool shouldCancelOp = false;

        // Grbl Probe
        protected float cncMachineXMm = 0.0f;
        protected float cncMachineYMm = 0.0f;
        protected float cncMachineZMm = 0.0f;
        protected float probeResultZMm = 0.0f;

        // Расстояние между инструментом и камерой
        private float camToToolDistX = 0.0f;
        private float camToToolDistY = 0.0f;

        // Калибровка PNP
        protected float cAddAngle = 0.0f;
        private bool cPositiveClockwise = false;
        protected int stepsPerFullCTurn = 6400;

        // PNP Network Interface
        private TcpClient? tcpClient;
        private NetworkStream? tcpStream;
        protected String machineIP = "";
        private bool lightOn = false;

        // Константы для моего PNP станка
        protected float placeHeightMm = 9.5f;
        protected float safeHeightMm = 20.0f;

        public static int RESERVED_TYPE = 10;

        public CNCGeneralMachine(String xConfigFilePath, int xCamerasNeeded, int xPreviewBoxWidth, int xPreviewBoxHeight, EventHandler xStateChanged, EventHandler<ErrorEventArgs> xErrorOccurred, EventHandler<NewCameraPreviewImageArgs> xNewCameraPreviewImage, EventHandler<UserInputRequiredArgs> xUserInputRequired)
        {
            camerasNeeded = xCamerasNeeded;
            previewBoxWidth = xPreviewBoxWidth;
            previewBoxHeight = xPreviewBoxHeight;
            camWatchdogTimer = new System.Threading.Timer(new TimerCallback(delegate
            {
                camWatchdogTick();
            }), null, 3000, 3000);
            StateChanged = xStateChanged;
            ErrorOccurred = xErrorOccurred;
            NewCameraPreviewImage = xNewCameraPreviewImage;
            UserInputRequired = xUserInputRequired;
            configFilePath = xConfigFilePath;
            shouldCancelOp = false;
            LoadSettingsFromFile();
        }

        private void LoadSettingsFromFile()
        {
            if (File.Exists(configFilePath))
            {
                FileStorage storage = new FileStorage(configFilePath, FileStorage.Mode.Read);
                motionStepsPerMmX = storage.GetNode("x_steps_per_mm").ReadFloat(-1f);
                motionStepsPerMmY = storage.GetNode("y_steps_per_mm").ReadFloat(-1f);
                motionSkewAngleRad = storage.GetNode("skew_angle_rad").ReadFloat();
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
                camToToolDistX = storage.GetNode("cam_to_tool_dist_x").ReadFloat();
                camToToolDistY = storage.GetNode("cam_to_tool_dist_y").ReadFloat();
                cPositiveClockwise = storage.GetNode("c_positive_clockwise").ReadInt() > 0;
                stepsPerFullCTurn = storage.GetNode("steps_per_full_c_turn").ReadInt();
                cAddAngle = storage.GetNode("c_add_angle").ReadFloat();
                machineIP = storage.GetNode("machine_ip").ReadString();
                storage.ReleaseAndGetString();
            }
        }

        public void SaveSettingsToFile()
        {
            FileStorage storage = new FileStorage(configFilePath, FileStorage.Mode.Write);
            storage.Write(motionStepsPerMmX, "x_steps_per_mm");
            storage.Write(motionStepsPerMmY, "y_steps_per_mm");
            storage.Write(motionSkewAngleRad, "skew_angle_rad");
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
            storage.Write(camToToolDistX, "cam_to_tool_dist_x");
            storage.Write(camToToolDistY, "cam_to_tool_dist_y");
            storage.Write(cPositiveClockwise ? 1 : 0, "c_positive_clockwise");
            storage.Write(stepsPerFullCTurn, "steps_per_full_c_turn");
            storage.Write(cAddAngle, "c_add_angle");
            storage.Write(machineIP, "machine_ip");
            storage.ReleaseAndGetString();
        }

        public void SubmitUserEntry(int ue)
        {
            if (ue < 0)
            {
                throw new Exception("Неверный ввод.");
            }
            if (curUserEntry >= 0)
            {
                throw new Exception("Ввод не требуется.");
            }
            curUserEntry = ue + 0;
            StateChanged.Invoke(this, new EventArgs());
        }

        public void SetImageFlipH(bool xChecked)
        {
            hFlip = xChecked;
            SaveSettingsToFile();
        }

        public bool GetImageFlipH()
        {
            return hFlip;
        }

        public void SetImageFlipV(bool xChecked)
        {
            vFlip = xChecked;
            SaveSettingsToFile();
        }

        public bool GetImageFlipV()
        {
            return vFlip;
        }

        public void SetShowCV(bool xChecked)
        {
            showCV = xChecked;
            SaveSettingsToFile();
        }

        public bool GetShowCV()
        {
            return showCV;
        }

        public void SetRotateImage(bool xChecked)
        {
            rotateCamImage = xChecked;
            SaveSettingsToFile();
        }

        public bool GetRotateImage()
        {
            return rotateCamImage;
        }

        public void SetShowRect(bool xChecked)
        {
            showRect = xChecked;
            SaveSettingsToFile();
        }

        public bool GetShowRect()
        {
            return showRect;
        }

        public void SetImageAngleDegrees(float xVal)
        {
            imageAngleDeg = xVal;
            SaveSettingsToFile();
        }

        public float GetImageAngleDegrees()
        {
            return imageAngleDeg;
        }

        public void SetRectWidthMm(float xVal)
        {
            rectWidthMm = xVal;
            SaveSettingsToFile();
        }

        public float GetRectWidthMm()
        {
            return rectWidthMm;
        }

        public void SetRectHeightMm(float xVal)
        {
            rectHeightMm = xVal;
            SaveSettingsToFile();
        }

        public float GetRectHeightMm()
        {
            return rectHeightMm;
        }

        public void SetImagePxPerMm(float xVal)
        {
            imagePxPerMm = xVal;
            SaveSettingsToFile();
        }

        public float GetImagePxPerMm()
        {
            return imagePxPerMm;
        }

        public void SetFOVXFromPercent(int xVal)
        {
            fovXFromPercent = xVal;
            SaveSettingsToFile();
        }

        public int GetFOVXFromPercent()
        {
            return fovXFromPercent;
        }

        public void SetFOVXToPercent(int xVal)
        {
            fovXToPercent = xVal;
            SaveSettingsToFile();
        }

        public int GetFOVXToPercent()
        {
            return fovXToPercent;
        }

        public void SetFOVYFromPercent(int xVal)
        {
            fovYFromPercent = xVal;
            SaveSettingsToFile();
        }

        public int GetFOVYFromPercent()
        {
            return fovYFromPercent;
        }

        public void SetFOVYToPercent(int xVal)
        {
            fovYToPercent = xVal;
            SaveSettingsToFile();
        }

        public int GetFOVYToPercent()
        {
            return fovYToPercent;
        }

        public void SetZoom(float xZoom)
        {
            zoomFactor = xZoom;
        }

        public float GetAbsXMm()
        {
            return absMmX;
        }

        public float GetAbsYMm()
        {
            return absMmY;
        }

        public float GetAbsZMm()
        {
            return absMmZ;
        }
        public float GetMotionStepsPerMmX()
        {
            return motionStepsPerMmX;
        }

        public float GetMotionStepsPerMmY()
        {
            return motionStepsPerMmY;
        }
        public float GetMotionSkewAngleRad()
        {
            return motionSkewAngleRad;
        }

        public void NextCameraPreview()
        {
            if (displayCamIdx < (cameras.Count - 1))
            {
                displayCamIdx++;
            }
        }

        public void PrevCameraPreview()
        {
            if (displayCamIdx > 0)
            {
                displayCamIdx--;
            }
        }

        public bool IsConnected()
        {
            return cncConnected;
        }

        public bool IsFault()
        {
            return cncFault;
        }

        public bool IsConnectPending()
        {
            return connectThread?.IsAlive == true && connectPending;
        }

        public void StartConnect(String pnpIP)
        {
            if (cncConnected)
            {
                throw new Exception("Подключение уже установлено.");
            }
            machineIP = pnpIP;
            SaveSettingsToFile();
            shouldCancelOp = false;
            connectThread = new Thread(DoConnect);
            connectThread?.Start();
        }

        public void SetBusyWithLongTask(bool flag)
        {
            busyWithLongTask = flag;
        }

        public bool IsBusyWithLongTask()
        {
            return (busyWithLongTask && workThread?.IsAlive == true) || IsConnectPending() || IsUIInitiatedMovementPending();
        }

        public bool IsUIInitiatedMovementPending()
        {
            return moveThread?.IsAlive == true && movePending;
        }

        public void StartMoveManual(float dX, float dY, float dZ)
        {
            bool hasUserInputWithMoveAllowed = curUserEntry < 0 && (curUserEntryAllowXYMove || curUserEntryAllowZMove);
            if (IsBusyWithLongTask() && !hasUserInputWithMoveAllowed)
            {
                throw new Exception("Станок занят работой над вашими файлами, движение вручную недоступно.");
            }
            shouldCancelOp = false;
            moveThread = new Thread(new ThreadStart(delegate
            {
                DoMoveUIInduced(dX, dY, dZ);
            }));
            moveThread?.Start();
        }

        private void DoMoveUIInduced(float qX, float qY, float qZ)
        {
            try
            {
                movePending = true;
                StateChanged.Invoke(this, new EventArgs());
                MoveRelativeAndWaitForCompletionInternal(false, qX, qY, qZ, -1.0f, true);
                movePending = false;
                StateChanged.Invoke(this, new EventArgs());
            }
            catch (Exception e)
            {
                movePending = false;
                ErrorOccurred.Invoke(this, new ErrorEventArgs(e.Message));
            }
        }

        ~CNCGeneralMachine()
        {
            camWatchdogTimer?.Dispose();
        }

        private Size GetMaxResolution(int devIdx)
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
            return new Size(maxWidth, maxHeight);
        }

        private void DoConnect()
        {
            try
            {
                connectPending = true;
                StateChanged.Invoke(this, new EventArgs());

                // TCP
                if (machineIP.Length > 0)
                {
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
                }
                lightOn = false;

                // GRBL
                cncFault = false;
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
                    throw new Exception("Ошибка ЧПУ станка при запуске.");
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
                    throw new Exception("Ошибка ЧПУ станка при запуске.");
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
                    throw new Exception("Ошибка ЧПУ станка при чтении памяти.");
                }
                if (motionStepsPerMmX < 0f || motionStepsPerMmY < 0f)
                {
                    throw new Exception("Ошибка чтения шагов на мм из памяти станка.");
                }

                // Home
                HomeAndStopSpindle();

                // Камера
                cameras.Clear();
                for (int i = 0; i < camerasNeeded; i++)
                {
                    CNCCameraInfo thisCamera = new CNCCameraInfo();
                    thisCamera.framesCaptured = 100;
                    thisCamera.cncCamCapture = new VideoCapture(i, VideoCapture.API.DShow);
                    thisCamera.cncCamCapture!.Set(CapProp.Autofocus, 0.0f);
                    Size maxRes = GetMaxResolution(i);
                    thisCamera.cncCamCapture!.Set(CapProp.FrameWidth, maxRes.Width);
                    thisCamera.cncCamCapture!.Set(CapProp.FrameHeight, maxRes.Height);
                    thisCamera.cncCamCapture!.ExceptionMode = false;
                    int attempts = 0;
                    while (!thisCamera.cncCamCapture.IsOpened && attempts < 3)
                    {
                        Thread.Sleep(500);
                        attempts++;
                    }
                    if (!thisCamera.cncCamCapture.IsOpened)
                    {
                        throw new Exception("Не удалось обнаружить камеру.");
                    }
                    int xi = i + 0;
                    thisCamera.cncCamCapture.ImageGrabbed += delegate (object? sender, EventArgs e)
                    {
                        ImageGrabbed(sender, e, xi);
                    };
                    thisCamera.cncCamCapture.Start();
                    thisCamera.framesCaptured = 100;
                    cameras.Add(thisCamera);
                }
                spindleActive = false;
                cncConnected = true;
                connectPending = false;
                StateChanged.Invoke(this, new EventArgs());
            }
            catch (Exception e)
            {
                ForceDisconnect();
                connectPending = false;
                ErrorOccurred.Invoke(this, new ErrorEventArgs(e.Message));
            }
        }

        private bool ShouldEnableUIMovementButtonsInternal()
        {
            bool flag = !IsUIInitiatedMovementPending() && IsConnected() && !IsFault();
            if (IsBusyWithLongTask() && curUserEntry >= 0)
            {
                flag = false;
            }
            return flag;
        }

        public bool ShouldEnableUIMoveXY()
        {
            if (!ShouldEnableUIMovementButtonsInternal())
            {
                return false;
            }
            if (curUserEntry < 0)
            {
                return curUserEntryAllowXYMove;
            } else
            {
                return true;
            }
        }

        public bool ShouldEnableUIMoveZ()
        {
            if (!ShouldEnableUIMovementButtonsInternal())
            {
                return false;
            }
            if (curUserEntry < 0)
            {
                return curUserEntryAllowZMove;
            }
            else
            {
                return true;
            }
        }

        public bool ShouldEnableSpindleToggleBtn()
        {
            if (!ShouldEnableUIMovementButtonsInternal())
            {
                return false;
            }
            if (curUserEntry < 0)
            {
                return curUserEntryAllowSpindle;
            }
            else
            {
                return true;
            }
        }

        private void HomeAndStopSpindle()
        {
            // Home
            grblCommandCmpl = 0;
            grblPort?.WriteLine("$H");
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                throw new Exception("Ошибка ЧПУ станка при перемещении в ноль.");
            }
            absMmX = (homingCorner.X > 0.5f) ? maxTravelMmX : 0f;
            absMmY = (homingCorner.Y > 0.5f) ? maxTravelMmY : 0f;
            absMmZ = (homingCorner.Z > 0.5f) ? maxTravelMmZ : 0f;

            // Относительное позиционирование
            grblCommandCmpl = 0;
            grblPort?.WriteLine("G91");
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                throw new Exception("Ошибка ЧПУ станка при запуске.");
            }

            // Остановить шпиндель
            grblCommandCmpl = 0;
            grblPort?.WriteLine("M5");
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                throw new Exception("Ошибка ЧПУ станка при остановке шпинделя на запуске.");
            }
        }

        private void ImageGrabbed(object? sender, EventArgs e, int cIdx)
        {
            if (cameras.Count < cIdx)
            {
                return;
            }
            lock (cameras[cIdx].lockObject)
            {
                bool frameRetrFlag = false;
                try
                {
                    frameRetrFlag = cameras[cIdx].cncCamCapture?.Retrieve(cameras[cIdx].curRawFrame) ?? false;
                }
                catch (Exception ex) { }
                cameras[cIdx].framesCaptured += 1;
                if (frameRetrFlag)
                {
                    // Угловые линии
                    if (rotateCamImage)
                    {
                        CvInvoke.GetRotationMatrix2D(new PointF((cameras[cIdx].curRawFrame.Cols - 1) / 2.0f, (cameras[cIdx].curRawFrame.Rows - 1) / 2.0f), -imageAngleDeg, 1.0f, cameras[cIdx].transformMatrix);
                        CvInvoke.WarpAffine(cameras[cIdx].curRawFrame, cameras[cIdx].curRotFrame, cameras[cIdx].transformMatrix, new Size(cameras[cIdx].curRawFrame.Cols, cameras[cIdx].curRotFrame.Rows));
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
                    int nW = (rotateCamImage) ? cameras[cIdx].curRotFrame.Width : cameras[cIdx].curRawFrame.Width;
                    int nH = (rotateCamImage) ? cameras[cIdx].curRotFrame.Height : cameras[cIdx].curRawFrame.Height;
                    Rectangle roi = new Rectangle((int)(nW * nFromX), (int)(nH * nFromY), (int)(nW * (nToX - nFromX)), (int)(nH * (nToY - nFromY)));
                    roi.Inflate(-(int)((roi.Width / 2.0f) - (roi.Width / (2.0f * zoomFactor))), -(int)((roi.Height / 2.0f) - (roi.Height / (2.0f * zoomFactor))));
                    CvInvoke.Resize(new Mat((rotateCamImage) ? cameras[cIdx].curRotFrame : cameras[cIdx].curRawFrame, roi), (!vFlip && !hFlip) ? cameras[cIdx].curDispFrame : cameras[cIdx].curRszFrame, new Size(previewBoxWidth, (int)(((rotateCamImage) ? cameras[cIdx].curRotFrame.Height : cameras[cIdx].curRawFrame.Height) * ((float)previewBoxWidth / ((rotateCamImage) ? (float)cameras[cIdx].curRotFrame.Width : (float)cameras[cIdx].curRawFrame.Width)))));

                    // Отразить
                    if (vFlip || hFlip)
                    {
                        if (vFlip && hFlip)
                        {
                            CvInvoke.Flip(cameras[cIdx].curRszFrame, cameras[cIdx].curDispFrame, FlipType.Both);
                        }
                        else
                        {
                            CvInvoke.Flip(cameras[cIdx].curRszFrame, cameras[cIdx].curDispFrame, (vFlip) ? FlipType.Vertical : FlipType.Horizontal);
                        }
                    }

                    // Контуры
                    if (showCV)
                    {
                        CvInvoke.CvtColor(cameras[cIdx].curDispFrame, cameras[cIdx].curDGrayFrame, ColorConversion.Bgr2Gray);
                        CvInvoke.GaussianBlur(cameras[cIdx].curDGrayFrame, cameras[cIdx].curGrayBlurredFrame, new Size(3, 3), 0);
                        CvInvoke.Threshold(cameras[cIdx].curGrayBlurredFrame, cameras[cIdx].curThreshFrame, 85, 255, ThresholdType.Binary);
                        Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
                        Mat hier = new Mat();
                        CvInvoke.FindContours(cameras[cIdx].curThreshFrame, contours, hier, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
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
                                CvInvoke.Circle(cameras[cIdx].curDispFrame, new Point(x, y), 3, clr, 10);
                                CvInvoke.DrawContours(cameras[cIdx].curDispFrame, contours, i, clr, 4);

                                // Это прямоугольник? Считаем угол.
                                VectorOfPoint approx = new VectorOfPoint();
                                CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);
                                if (approx.Size == 4)
                                {
                                    Vector2 gen1 = Vector2.Subtract(new Vector2(approx[0].X, approx[0].Y), new Vector2(approx[2].X, approx[2].Y));
                                    Vector2 gen2 = Vector2.Subtract(new Vector2(approx[1].X, approx[1].Y), new Vector2(approx[3].X, approx[3].Y));
                                    float angleRad = (float)Math.Atan2(gen2.Y - gen1.Y, gen2.X - gen1.X);
                                    float angDegrees = (180f / 3.14159265f) * angleRad;
                                    CvInvoke.PutText(cameras[cIdx].curDispFrame, angDegrees.ToString("#.##") + " deg", approx[0], FontFace.HersheyPlain, 3f, clr, 4);
                                }
                            }
                        }
                    }

                    // Отобразить линии осей согласно посчитанным углам поверх цветного без искажения
                    Point xLineStart = new Point(0, (int)(cameras[cIdx].curDispFrame.Height / 2f));
                    Point xLineEnd = new Point(cameras[cIdx].curDispFrame.Width, (int)(cameras[cIdx].curDispFrame.Height / 2f));
                    Point yLineStart = new Point((int)(cameras[cIdx].curDispFrame.Width / 2f), 0);
                    Point yLineEnd = new Point((int)(cameras[cIdx].curDispFrame.Width / 2f), cameras[cIdx].curDispFrame.Height);
                    CvInvoke.Line(cameras[cIdx].curDispFrame, xLineStart, xLineEnd, new MCvScalar(0, 0.0f, 255.0f));
                    CvInvoke.Line(cameras[cIdx].curDispFrame, yLineStart, yLineEnd, new MCvScalar(255.0f, 0.0f, 0.0f));

                    // Отобразить штрихи расстояния
                    int xHalfPoint = (int)(cameras[cIdx].curDispFrame.Width / 2f);
                    int yHalfPoint = (int)(cameras[cIdx].curDispFrame.Height / 2f);
                    float effPxPerMm = imagePxPerMm * zoomFactor;
                    for (float x = xHalfPoint; x > 0f; x -= effPxPerMm)
                    {
                        CvInvoke.Line(cameras[cIdx].curDispFrame, new Point((int)x, (int)(yHalfPoint - 10f)), new Point((int)x, (int)(yHalfPoint + 10f)), new MCvScalar(0, 0.0f, 255.0f));
                    }
                    for (float x = xHalfPoint; x < cameras[cIdx].curDispFrame.Width; x += effPxPerMm)
                    {
                        CvInvoke.Line(cameras[cIdx].curDispFrame, new Point((int)x, (int)(yHalfPoint - 10f)), new Point((int)x, (int)(yHalfPoint + 10f)), new MCvScalar(0, 0.0f, 255.0f));
                    }
                    for (float y = yHalfPoint; y < cameras[cIdx].curDispFrame.Height; y += effPxPerMm)
                    {
                        CvInvoke.Line(cameras[cIdx].curDispFrame, new Point((int)(xHalfPoint - 10f), (int)y), new Point((int)(xHalfPoint + 10f), (int)y), new MCvScalar(255.0f, 0.0f, 0.0f));
                    }
                    for (float y = yHalfPoint; y > 0f; y -= effPxPerMm)
                    {
                        CvInvoke.Line(cameras[cIdx].curDispFrame, new Point((int)(xHalfPoint - 10f), (int)y), new Point((int)(xHalfPoint + 10f), (int)y), new MCvScalar(255.0f, 0.0f, 0.0f));
                    }

                    // Отобразить прямоугольник
                    if (rectWidthMm > 0f && rectHeightMm > 0f && showRect)
                    {
                        Rectangle rect = new Rectangle((int)(xHalfPoint - ((rectWidthMm * effPxPerMm) / 2f)), (int)(yHalfPoint - ((rectHeightMm * effPxPerMm) / 2f)), (int)(rectWidthMm * effPxPerMm), (int)(rectHeightMm * effPxPerMm));
                        CvInvoke.Rectangle(cameras[cIdx].curDispFrame, rect, new MCvScalar(0, 255, 255));
                    }

                    // Отобразить
                    if (cIdx == displayCamIdx)
                    {
                        NewCameraPreviewImage.Invoke(this, new NewCameraPreviewImageArgs(cameras[cIdx].curDispFrame));
                    }
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
                                float machineMotionStepsPerMmX = (float)Utils.StrToDouble(rLine.Replace("$100=", ""));
                                if (Math.Abs(machineMotionStepsPerMmX - motionStepsPerMmX) > 0.001f)
                                {
                                    motionSkewAngleRad = 0.0f;
                                }
                                motionStepsPerMmX = machineMotionStepsPerMmX;
                            }
                            else if (rLine.StartsWith("$101="))
                            {
                                float machineMotionStepsPerMmY = (float)Utils.StrToDouble(rLine.Replace("$101=", ""));
                                if (Math.Abs(machineMotionStepsPerMmY - motionStepsPerMmY) > 0.001f)
                                {
                                    motionSkewAngleRad = 0.0f;
                                }
                                motionStepsPerMmY = machineMotionStepsPerMmY;
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
                            grblCommandCmpl = 2;
                            cncFault = true;
                            ErrorOccurred.Invoke(this, new ErrorEventArgs("Станок вернул неверный формат настроек $$."));
                            return;
                        }
                    }
                    if (motionStepsPerMmX >= 0f && motionStepsPerMmY >= 0f && maxTravelMmX >= 0f && maxTravelMmY >= 0f && maxTravelMmZ >= 0f && maxFeedRateMmPerMinX >= 0f && maxFeedRateMmPerMinY >= 0f && maxFeedRateMmPerMinZ >= 0f && accXMmPerSecSq >= 0f && accYMmPerSecSq >= 0f && accZMmPerSecSq >= 0f)
                    {
                        grblCommandCmpl = 1;
                    }
                }
            }
            else if (cmdRes.ToLower().Contains("prb:"))
            {
                try
                {
                    String rawVal1 = cmdRes.ToLower().Replace("[prb:", "").Replace("]", "");
                    String rawVal2 = rawVal1.Split(':')[0].Split(',')[2];
                    probeResultZMm = (float)Convert.ToDouble(rawVal2);
                }
                catch (Exception ex)
                {
                    grblCommandCmpl = 2;
                    cncFault = true;
                    ErrorOccurred.Invoke(this, new ErrorEventArgs("Ошибка ЧПУ станка при парсинге его ответа PRB: " + ex.Message));
                }
            }
            else if (cmdRes.StartsWith("<") || cmdRes.ToLower().Contains("mpos:"))
            {
                try
                {
                    String rawVal1 = cmdRes.ToLower().Replace("<", "").Replace(">", "").Replace("mpos:", "").Replace("|", ",");
                    String rawVal2X = rawVal1.Split(',')[1];
                    String rawVal2Y = rawVal1.Split(',')[2];
                    String rawVal2Z = rawVal1.Split(',')[3];
                    cncMachineXMm = (float)Convert.ToDouble(rawVal2X);
                    cncMachineYMm = (float)Convert.ToDouble(rawVal2Y);
                    cncMachineZMm = (float)Convert.ToDouble(rawVal2Z);
                }
                catch (Exception ex)
                {
                    grblCommandCmpl = 2;
                    cncFault = true;
                    ErrorOccurred.Invoke(this, new ErrorEventArgs("Ошибка ЧПУ станка при парсинге его ответа MPOS: " + ex.Message));
                }
            }
            else if (cmdRes.ToLower().Contains("error:"))
            {
                grblCommandCmpl = 2;
                cncFault = true;
                ErrorOccurred.Invoke(this, new ErrorEventArgs("Ошибка станка: " + cmdRes));
            }
            else if (cmdRes.ToLower().Contains("alarm") && !cmdRes.Contains("<"))
            {
                grblCommandCmpl = 2;
                cncFault = true;
                ErrorOccurred.Invoke(this, new ErrorEventArgs("Ошибка станка: " + cmdRes));
            }
            else
            {
                if (grblCommandCmpl != 2)
                {
                    grblCommandCmpl = 1;
                }
            }
        }

        private void GrblPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            cncFault = true;
            StateChanged.Invoke(this, new EventArgs());
        }

        protected int MillisecToExecuteMove(float deltaX, float deltaY, float deltaZ, float fX, float fY, float fZ)
        {
            int xTime = Utils.MillisecToExecuteAxisMove(deltaX, (fX > 0.0f) ? fX : maxFeedRateMmPerMinX, accXMmPerSecSq);
            int yTime = Utils.MillisecToExecuteAxisMove(deltaY, (fY > 0.0f) ? fY : maxFeedRateMmPerMinY, accYMmPerSecSq);
            int zTime = Utils.MillisecToExecuteAxisMove(deltaZ, (fZ > 0.0f) ? fZ : maxFeedRateMmPerMinZ, accZMmPerSecSq);
            int maxTime = 0;
            if (xTime > maxTime) maxTime = xTime;
            if (yTime > maxTime) maxTime = yTime;
            if (zTime > maxTime) maxTime = zTime;
            return maxTime;
        }
        private void GrblSendLine(String cmd)
        {
            try
            {
                grblPort?.WriteLine(cmd);
            }
            catch
            {
                grblCommandCmpl = 2;
                cncFault = true;
                StateChanged.Invoke(this, new EventArgs());
            }
        }

        protected void MoveToAbsolutePositionAndWaitForCompletion(bool useg1, float? newMmX, float? newMmY, float? newMmZ, float feedRateMmPerMin)
        {
            float dX = 0.0f;
            float dY = 0.0f;
            float dZ = 0.0f;
            if (newMmX != null)
            {
                dX = (float)(newMmX - absMmX);
            }
            if (newMmY != null)
            {
                dY = (float)(newMmY - absMmY);
            }
            if (newMmZ != null)
            {
                dZ = (float)(newMmZ - absMmZ);
            }
            MoveRelativeAndWaitForCompletion(useg1, dX, dY, dZ, feedRateMmPerMin);
        }

        private String GetGCodeCommand(String cmd, float mX, float mY, float mZ, float feedRate)
        {
            // Компенсация кривизны станка
            Vector2 compensatedX = Vector2.Transform(new Vector2(mX, 0f), Matrix3x2.CreateRotation(motionSkewAngleRad * 0.5f));
            Vector2 compensatedY = Vector2.Transform(new Vector2(0f, mY), Matrix3x2.CreateRotation(-motionSkewAngleRad * 0.5f));
            Vector2 vsum = compensatedX + compensatedY;
            float qX = (xPlusIsRight) ? vsum.X : -vsum.X;
            float qY = (yPlusIsDown) ? vsum.Y : -vsum.Y;
            float qZ = (zPlusIsWithdraw) ? mZ : -mZ;

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

        protected void MoveRelativeAndWaitForCompletion(bool useg1, float relMmX, float relMmY, float relMmZ, float feedRateMmPerMin) {
            MoveRelativeAndWaitForCompletionInternal(useg1, relMmX, relMmY, relMmZ, feedRateMmPerMin, false);
        }

        protected void MoveRelativeAndWaitForCompletionInternal(bool useg1, float relMmX, float relMmY, float relMmZ, float feedRateMmPerMin, bool fromUI)
        {
            bool hasAllowingMoveUserInput = curUserEntry < 0 && (curUserEntryAllowXYMove || curUserEntryAllowZMove);
            if (!fromUI && movePending)
            {
                throw new Exception("Уже идет перемещение из UI.");
            } else if (fromUI && ((busyWithLongTask && workThread?.IsAlive == true) || IsConnectPending()) && !hasAllowingMoveUserInput)
            {
                throw new Exception("Уже идет перемещение из задачи.");
            }
            if ((absMmX + relMmX) > maxTravelMmX || (absMmY + relMmY) > maxTravelMmY || (absMmZ + relMmZ) > maxTravelMmZ || (absMmX + relMmX) < 0f || (absMmY + relMmY) < 0f || (absMmZ + relMmZ) < 0f)
            {
                throw new Exception("Движение превышает лимит оси.");
            }
            start:
            Stopwatch sw = new Stopwatch();
            sw.Start();
            grblCommandCmpl = 0;
            GrblSendLine(GetGCodeCommand((useg1) ? "G1" : "G0", relMmX, relMmY, relMmZ, feedRateMmPerMin));
            int plannedMillis = MillisecToExecuteMove(relMmX, relMmY, relMmZ, feedRateMmPerMin, feedRateMmPerMin, feedRateMmPerMin);
            int factualMillis = 0;
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
                factualMillis += 10;
                if ((factualMillis > (plannedMillis * 5)) && (factualMillis > 1000))
                {
                    if (AskUserForInput("Соединение GRBL со станком оборвалось, переподключите и нажмите [да]. Если хотите отменить процесс, нажмите [нет].", new List<string> { "Да", "Нет" }, RESERVED_TYPE, false, false, false) <= 0)
                    {
                        String prevPortName = "COM1";
                        try
                        {
                            prevPortName = grblPort?.PortName ?? "COM1";
                            grblPort?.Close();
                        }
                        catch (Exception excpt) { }
                        grblPort = new SerialPort(prevPortName, 115200);
                        grblPort.Open();
                        grblPort.WriteTimeout = 3000;
                        grblPort.ReadTimeout = 3000;
                        goto start;
                    } else
                    {
                        cncFault = true;
                        throw new Exception("Процесс отменен из-за обрыва соединения со станком.");
                    }
                }
            }
            if (grblCommandCmpl >= 2)
            {
                throw new Exception("Ошибка станка при перемещении.");
            }
            sw.Stop();
            int leftToWaitMillis = plannedMillis - (int)sw.ElapsedMilliseconds;
            absMmX += relMmX;
            absMmY += relMmY;
            absMmZ += relMmZ;
            if (leftToWaitMillis > 0)
            {
                Thread.Sleep(leftToWaitMillis);
            }
            StateChanged.Invoke(this, new EventArgs());
        }

        private void camWatchdogTick()
        {
            if (cncConnected)
            {
                for (int i = 0; i < cameras.Count; i++)
                {
                    if (cameras[i].framesCaptured > 3)
                    {
                        cameras[i].framesCaptured = 0;
                    } else
                    {
                        // Reconnect
                        try
                        {
                            cameras[i].cncCamCapture?.Stop();
                        }
                        catch { }
                        try
                        {
                            cameras[i].cncCamCapture = new VideoCapture(i, VideoCapture.API.DShow);
                            cameras[i].cncCamCapture?.Set(CapProp.Autofocus, 0.0f);
                            Size maxRes = GetMaxResolution(i);
                            cameras[i].cncCamCapture!.Set(CapProp.FrameWidth, maxRes.Width);
                            cameras[i].cncCamCapture!.Set(CapProp.FrameHeight, maxRes.Height);
                            cameras[i].cncCamCapture!.ExceptionMode = false;
                            int attempts = 0;
                            while (!cameras[i].cncCamCapture!.IsOpened && attempts < 3)
                            {
                                Thread.Sleep(500);
                                attempts++;
                            }
                            if (!cameras[i].cncCamCapture!.IsOpened)
                            {
                                throw new Exception("Не удалось обнаружить камеру.");
                            }
                            int xi = i + 0;
                            cameras[i].cncCamCapture!.ImageGrabbed += delegate (object? sender, EventArgs e) {
                                ImageGrabbed(sender, e, xi);
                            };
                            cameras[i].cncCamCapture?.Start();
                        }
                        catch
                        {
                            cncFault = true;
                            StateChanged.Invoke(this, new EventArgs());
                            return;
                        }
                    }
                }
                
            }
        }

        protected void SetSpindleRpm(int rpm)
        {
            grblCommandCmpl = 0;
            if (rpm > 0)
            {
                GrblSendLine("M3 S" + rpm.ToString("0"));
            } else
            {
                GrblSendLine("M5");
            }
            int timeout = 0;
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
                timeout++;
                if (timeout > 200)
                {
                    break;
                }
            }
            if (grblCommandCmpl >= 2)
            {
                throw new Exception("Ошибка ЧПУ станка при запуске шпинделя.");
            }
            if (rpm > 0)
            {
                spindleActive = true;
                Thread.Sleep(4000);
            } else
            {
                spindleActive = false;
            }
        }

        public bool IsMotionStepCalibrated()
        {
            return motionStepsPerMmX != 800.0f && motionStepsPerMmY != 800.0f && motionSkewAngleRad != 0.0f;
        }

        public bool IsCamOffsetCalibrated()
        {
            return camToToolDistX != 0.0f && camToToolDistY != 0.0f;
        }

        public bool IsPNPCalibrated()
        {
            return stepsPerFullCTurn >= 16;
        }

        public bool IsCalibrated()
        {
            if (machineIP.Length > 0)
            {
                return IsCamOffsetCalibrated() && IsMotionStepCalibrated() && IsPNPCalibrated();
            }
            else
            {
                return IsCamOffsetCalibrated() && IsMotionStepCalibrated();
            }
        }

        protected int AskUserForInput(String text, List<String> buttons, int type, bool allowMoveXY, bool allowMoveZ, bool allowSpindle)
        {
            if (curUserEntry < 0)
            {
                throw new Exception("AskUserForInput: неверное состояние.");
            }
            curUserEntryAllowXYMove = allowMoveXY;
            curUserEntryAllowZMove = allowMoveZ;
            curUserEntryAllowSpindle = allowSpindle;
            curUserEntry = -1;
            UserInputRequired.Invoke(this, new UserInputRequiredArgs(text, buttons, type));
            while (curUserEntry < 0)
            {
                Thread.Sleep(100);
                if (shouldCancelOp)
                {
                    curUserEntry = 0;
                    StateChanged.Invoke(this, new EventArgs());
                    throw new Exception("Операция отменена.");
                } else if (cncFault)
                {
                    curUserEntry = 0;
                    StateChanged.Invoke(this, new EventArgs());
                    throw new Exception("Потеряна связь со станком.");
                }
            }
            int retVal = curUserEntry + 0;
            curUserEntry = 0;
            return retVal;
        }

        public bool IsCancellationRequested()
        {
            return shouldCancelOp;
        }

        public void StartCalibrationProcess(int which, int steps)
        {
            if (!IsConnected() || IsFault())
            {
                throw new Exception("Нет связи со станком или он в состоянии Alarm.");
            }
            if (IsBusyWithLongTask() || spindleActive)
            {
                throw new Exception("Станок занят другим процессом или активен шпиндель, действие недоступно.");
            }
            shouldCancelOp = false;
            workThread = new Thread(new ThreadStart(delegate
            {
                DoCalibrationProcess(which, steps);
            }));
            workThread?.Start();
        }

        private void DoCalibrationProcess(int which, int steps)
        {
            try
            {
                // State
                SetBusyWithLongTask(true);
                StateChanged.Invoke(this, new EventArgs());

                // Process
                if (which == 0)
                {
                    // Reset
                    motionStepsPerMmX = 800.0f;
                    motionStepsPerMmY = 800.0f;
                    motionSkewAngleRad = 0.0f;
                    ApplyCalibrationToMachineAndHomeIt();

                    // First Point
                    PointF firstPoint;
                    float largeSideMm = Math.Max(calibDistanceBetweenCentersMmW, calibDistanceBetweenCentersMmH);
                    float smallSideMm = Math.Min(calibDistanceBetweenCentersMmW, calibDistanceBetweenCentersMmH);
                    if (maxTravelMmX >= maxTravelMmY)
                    {
                        firstPoint = new PointF((maxTravelMmX - largeSideMm) / 2.0f, (maxTravelMmY - smallSideMm) / 2.0f);
                    }
                    else
                    {
                        firstPoint = new PointF((maxTravelMmX - smallSideMm) / 2.0f, (maxTravelMmY - largeSideMm) / 2.0f);
                    }
                    MoveToAbsolutePositionAndWaitForCompletion(false, firstPoint.X, firstPoint.Y, null, -1.0f);
                    MoveToAbsolutePositionAndWaitForCompletion(false, null, null, calibZMm, -1.0f);
                    int res = AskUserForInput("Переместите каретку так, чтобы центр изображения камеры был в центре шахматки в левом-верхнем углу рисунка.", new List<string> { "Сделано", "Отмена" }, 0, true, false, false);
                    if (res > 0)
                    {
                        throw new Exception("Операция отменена.");
                    }
                    Vector2 calibTopLeft = new Vector2(absMmX, absMmY);

                    // 2nd point
                    if (maxTravelMmX >= maxTravelMmY)
                    {
                        MoveRelativeAndWaitForCompletion(false, largeSideMm, 0.0f, 0.0f, -1.0f);
                    }
                    else
                    {
                        MoveRelativeAndWaitForCompletion(false, smallSideMm, 0.0f, 0.0f, -1.0f);
                    }
                    res = AskUserForInput("Переместите каретку так, чтобы центр изображения камеры был в центре шахматки в правом-верхнем углу рисунка.", new List<string> { "Сделано", "Отмена" }, 0, true, false, false);
                    if (res > 0)
                    {
                        throw new Exception("Операция отменена.");
                    }
                    Vector2 calibTopRight = new Vector2(absMmX, absMmY);

                    // 3rd point
                    if (maxTravelMmX >= maxTravelMmY)
                    {
                        MoveRelativeAndWaitForCompletion(false, 0.0f, smallSideMm, 0.0f, -1.0f);
                    }
                    else
                    {
                        MoveRelativeAndWaitForCompletion(false, 0.0f, largeSideMm, 0.0f, -1.0f);
                    }
                    res = AskUserForInput("Переместите каретку так, чтобы центр изображения камеры был в центре шахматки в правом-нижнем углу рисунка.", new List<string> { "Сделано", "Отмена" }, 0, true, false, false);
                    if (res > 0)
                    {
                        throw new Exception("Операция отменена.");
                    }
                    Vector2 calibBottomRight = new Vector2(absMmX, absMmY);

                    // 4th point
                    if (maxTravelMmX >= maxTravelMmY)
                    {
                        MoveRelativeAndWaitForCompletion(false, -largeSideMm, 0.0f, 0.0f, -1.0f);
                    }
                    else
                    {
                        MoveRelativeAndWaitForCompletion(false, -smallSideMm, 0.0f, 0.0f, -1.0f);
                    }
                    res = AskUserForInput("Переместите каретку так, чтобы центр изображения камеры был в центре шахматки в левом-нижнем углу рисунка.", new List<string> { "Сделано", "Отмена" }, 0, true, false, false);
                    if (res > 0)
                    {
                        throw new Exception("Операция отменена.");
                    }
                    Vector2 calibBottomLeft = new Vector2(absMmX, absMmY);

                    // Правильный размер панели
                    float correctWidthMm = 0.0f;
                    float correctHeightMm = 0.0f;
                    if (maxTravelMmX >= maxTravelMmY)
                    {
                        correctWidthMm = largeSideMm;
                        correctHeightMm = smallSideMm;
                    }
                    else
                    {
                        correctWidthMm = smallSideMm;
                        correctHeightMm = largeSideMm;
                    }

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
                    float newSkewRad = (float)(Math.Atan(blCornerRot.X / blCornerRot.Y));

                    // Считаем шаги на мм (сколько нужно поставить
                    float newMotionStepsX = (trCornerRot.Length() / correctWidthMm) * motionStepsPerMmX;
                    float newMotionStepsY = (blCornerRot.Length() / correctHeightMm) * motionStepsPerMmY;

                    // Apply
                    motionStepsPerMmX = newMotionStepsX;
                    motionStepsPerMmY = newMotionStepsY;
                    motionSkewAngleRad = newSkewRad;
                    ApplyCalibrationToMachineAndHomeIt();

                    // Save
                    SaveSettingsToFile();
                }
                else if (which == 1)
                {
                    // Z-Level
                    MoveToAbsolutePositionAndWaitForCompletion(false, null, null, calibZMm, -1.0f);

                    // Ask User
                    int res = AskUserForInput("Используя стрелки и выключатель шпинделя в панели слева, просверлите отверстие любого диаметра (0.1-1.0мм), чем меньше - тем лучше, затем не сдвигаясь по XY, нажмите кнопку [готово]. В случае с PNP - оставьте где-нибудь метку соплом.", new List<string> { "Готово", "Отмена" }, 0, true, true, true);
                    if (res > 0)
                    {
                        throw new Exception("Операция отменена.");
                    }

                    // Z-Level
                    MoveToAbsolutePositionAndWaitForCompletion(false, null, null, calibZMm, -1.0f);

                    // Зафиксировать позицию
                    float initialPosMmX = absMmX + 0.0f;
                    float initialPosMmY = absMmY + 0.0f;

                    // Ask Again
                    res = AskUserForInput("Переместитесь по XY так чтобы отверстие или пометка оказались в центре изображения камеры.", new List<string> { "Готово", "Отмена" }, 0, true, false, false);

                    // Reset
                    camToToolDistX = absMmX - initialPosMmX;
                    camToToolDistY = absMmY - initialPosMmY;

                    // Save
                    SaveSettingsToFile();
                } else if (which == 2)
                {
                    // Перемещаем Z в median
                    MoveToSafeZ();

                    // Переместить каретку так чтобы сопло PNP оказалось там где камера
                    float tgtPosX = absMmX - camToToolDistX;
                    float tgtPosY = absMmY - camToToolDistY;
                    MoveToAbsolutePositionAndWaitForCompletion(false, tgtPosX, tgtPosY, null, -1.0f);

                    // Dwell
                    Thread.Sleep(300);

                    // Pick
                    TcpSendPick();

                    // Dwell
                    Thread.Sleep(300);

                    // Rotate
                    TcpSendRotate(steps);

                    // Dwell
                    Thread.Sleep(300);

                    // Place
                    TcpSendPlace();

                    // Dwell
                    Thread.Sleep(300);

                    // Переместить камеру обратно
                    float bkPosX = absMmX + camToToolDistX;
                    float bkPosY = absMmY + camToToolDistY;
                    MoveToAbsolutePositionAndWaitForCompletion(false, bkPosX, bkPosY, null, -1.0f);
                } else
                {
                    throw new Exception("Неверный параметр.");
                }

                // State
                SetBusyWithLongTask(false);
                StateChanged.Invoke(this, new EventArgs());
            }
            catch (Exception exc)
            {
                SetBusyWithLongTask(false);
                ErrorOccurred.Invoke(this, new ErrorEventArgs(exc.Message));
            }
        }

        private void ApplyCalibrationToMachineAndHomeIt()
        {
            // Записать X в станок
            grblCommandCmpl = 0;
            GrblSendLine("$100=" + motionStepsPerMmX.ToString("0.000"));
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            if (grblCommandCmpl >= 2)
            {
                throw new Exception("Ошибка станка при записи параметров, начните калибровку заново.");
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
                throw new Exception("Ошибка станка при записи параметров, начните калибровку заново.");
            }

            // Home
            HomeAndStopSpindle();

            // Обновить UI
            StateChanged.Invoke(this, new EventArgs());
        }

        public void ToggleLightFromUI()
        {
            bool hasUserInputWithSpindleAllowed = curUserEntry < 0 && curUserEntryAllowSpindle;
            if (IsBusyWithLongTask() && !hasUserInputWithSpindleAllowed)
            {
                throw new Exception("Нельзя менять состояние шпинделя во время этой задачи.");
            }
            if (IsFault() || !IsConnected())
            {
                throw new Exception("Неверное состояние станка.");
            }
            if (machineIP.Length <= 0)
            {
                throw new Exception("У этого станка нет фонарика.");
            }
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

        public void ToggleSpindleFromUI()
        {
            bool hasUserInputWithSpindleAllowed = curUserEntry < 0 && curUserEntryAllowSpindle;
            if (IsBusyWithLongTask() && !hasUserInputWithSpindleAllowed)
            {
                throw new Exception("Нельзя менять состояние шпинделя во время этой задачи.");
            }
            if (IsFault() || !IsConnected())
            {
                throw new Exception("Неверное состояние станка.");
            }
            if (machineIP.Length > 0)
            {
                throw new Exception("У этого станка нет шпинделя.");
            }
            if (spindleActive)
            {
                SetSpindleRpm(0);
            } else
            {
                SetSpindleRpm(4000);
            }
        }
        public void PrintCalibPattern()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintCalPattern;
            PrintDialog pdi = new PrintDialog();
            pdi.Document = pd;
            if (pdi.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void Pd_PrintCalPattern(object sender, PrintPageEventArgs e)
        {
            // Start
            e.HasMorePages = false;
            float oneMmInPrintUnits = 100.0f / 25.4f;

            // Calculate
            float fullSizeWMm = calibDistanceBetweenCentersMmW + (calibCellSizeMm * calibSubpatternCols);
            float fullSizeHMm = calibDistanceBetweenCentersMmH + (calibCellSizeMm * calibSubpatternRows);
            for (int aq = 0; aq < 4; aq++)
            {
                for (int nx = 0; nx < calibSubpatternCols; nx++)
                {
                    for (int ny = 0; ny < calibSubpatternRows; ny++)
                    {
                        bool isWhite = false;
                        if (nx % 2 == 0)
                        {
                            isWhite = ny % 2 == 0;
                        }
                        else
                        {
                            isWhite = ny % 2 != 0;
                        }
                        float offsetX = 0.0f;
                        float offsetY = 0.0f;
                        if (aq <= 0)
                        {
                            offsetX = 0.0f;
                            offsetY = 0.0f;
                        } else if (aq == 1)
                        {
                            offsetX = 1.0f;
                            offsetY = 0.0f;
                        } else if (aq == 2)
                        {
                            offsetX = 0.0f;
                            offsetY = 1.0f;
                        } else 
                        {
                            offsetX = 1.0f;
                            offsetY = 1.0f;
                        }
                        float leftPU = e.PageBounds.Left + (e.PageBounds.Width / 2.0f) - ((fullSizeWMm / 2.0f) * oneMmInPrintUnits) + (offsetX * calibDistanceBetweenCentersMmW * oneMmInPrintUnits) + (nx * calibCellSizeMm * oneMmInPrintUnits);
                        float topPU = e.PageBounds.Top + (e.PageBounds.Height / 2.0f) - ((fullSizeHMm / 2.0f) * oneMmInPrintUnits) + (offsetY * calibDistanceBetweenCentersMmH * oneMmInPrintUnits) + (ny * calibCellSizeMm * oneMmInPrintUnits);
                        e.Graphics?.FillRectangle(new SolidBrush((isWhite) ? Color.White : Color.Black), new RectangleF(leftPU, topPU, oneMmInPrintUnits * calibCellSizeMm, oneMmInPrintUnits * calibCellSizeMm));
                    }
                }
            }
        }

        public void Disconnect()
        {
            if (IsConnectPending())
            {
                throw new Exception("Дождитесь пока завершится подключение.");
            }
            if (IsUIInitiatedMovementPending())
            {
                throw new Exception("Дождитесь пока завершится перемещение.");
            }
            if (IsBusyWithLongTask())
            {
                throw new Exception("Дождитесь пока завершится задача или отмените её.");
            }
            ForceDisconnect();
        }

        private void ForceDisconnect() {
            try
            {
                grblPort?.Close();
            }
            catch (Exception excpt) { }
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
            for (int i = 0; i < cameras.Count; i++)
            {
                try
                {
                    if (cameras[i].cncCamCapture != null) cameras[i].cncCamCapture?.Stop();
                }
                catch (Exception excpt) { }
            }
            try
            {
                spindleActive = false;
                cncConnected = false;
                busyWithLongTask = false;
                movePending = false;
                connectPending = false;
                lightOn = false;
                cncFault = false;
            }
            catch (Exception excpt) { }
            StateChanged.Invoke(this, new EventArgs());
        }

        public void CancelOp()
        {
            shouldCancelOp = true;
            StateChanged.Invoke(this, new EventArgs());
        }

        public float GrblProbeCurrentLocation()
        {
            // Test
            if (!IsConnected() || IsFault())
            {
                throw new Exception("Нет связи со станком или он в состоянии Alarm.");
            }

            // Узнать текущий уровень Z в системе координат станка
            Stopwatch sw = new Stopwatch();
            sw.Start();
            grblCommandCmpl = 0;
            cncMachineZMm = 0f;
            GrblSendLine("?");
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            int toc = 0;
            while (cncMachineZMm == 0f && toc < 500)
            {
                Thread.Sleep(10);
                toc++;
            }
            if (toc >= 500 && cncMachineZMm == 0f)
            {
                throw new Exception("Таймаут при команде статуса (?)");
            }
            if (grblCommandCmpl >= 2)
            {
                throw new Exception("Ошибка ЧПУ станка при перемещении к отверстию.");
            }
            sw.Stop();

            // Выполнить измерение и получить результат
            sw.Reset();
            sw.Start();
            grblCommandCmpl = 0;
            probeResultZMm = 0f;
            GrblSendLine(GetGCodeCommand("G38.3", 0f, 0f, -6f, 60f));
            while (grblCommandCmpl < 1)
            {
                Thread.Sleep(10);
            }
            toc = 0;
            while (probeResultZMm == 0f && toc < 500)
            {
                Thread.Sleep(10);
                toc++;
            }
            if (toc >= 500 && probeResultZMm == 0f)
            {
                throw new Exception("Таймаут при G38.3");
            }
            if (grblCommandCmpl >= 2)
            {
                throw new Exception("Ошибка ЧПУ станка при перемещении к отверстию.");
            }
            sw.Stop();

            // Записать результат
            float deltaZMm = probeResultZMm - cncMachineZMm;
            absMmZ += deltaZMm;
            StateChanged.Invoke(this, new EventArgs());
            float fResult = absMmZ + 0.0f;

            // Вернуться к безопасному Z
            MoveRelativeAndWaitForCompletion(false, 0f, 0f, -deltaZMm, -1.0f);

            // Проверить результат
            if (deltaZMm < -5.85f || deltaZMm > 5.85f)
            {
                throw new Exception("Неверный результат GRBL Probe: похоже между концом инструмента и проводящей заготовкой более 6 мм.");
            }
            else if ((deltaZMm < 0.1f && deltaZMm >= 0.0f) || (deltaZMm > -0.1f && deltaZMm <= 0.0f))
            {
                throw new Exception("Неверный результат GRBL Probe: он близок к 0 (" + deltaZMm.ToString("0.00") + ").");
            }
            return fResult;
        }

        public float GetCamToToolDistX()
        {
            return camToToolDistX;
        }

        public float GetCamToToolDistY()
        {
            return camToToolDistY;
        }

        public int GetStepsPerFullCTurn()
        {
            return stepsPerFullCTurn;
        }

        public float GetCAddAngle()
        {
            return cAddAngle;
        }

        public void SetStepsPerFullCTurn(int steps)
        {
            stepsPerFullCTurn = steps;
            SaveSettingsToFile();
        }

        public void SetCAddAngle(float angleDeg)
        {
            cAddAngle = angleDeg;
            SaveSettingsToFile();
        }

        public bool GetCPositiveClockwise()
        {
            return cPositiveClockwise;
        }

        public void SetCPositiveClockwise(bool flag)
        {
            cPositiveClockwise = flag;
            SaveSettingsToFile();
        }

        private void MoveToZ(float tgtZMm)
        {
            if (machineIP.Length <= 0)
            {
                throw new Exception("Неверный вызов.");
            }
            MoveToAbsolutePositionAndWaitForCompletion(false, null, null, tgtZMm, -1.0f);
        }

        protected void MoveToPlaceZ()
        {
            MoveToZ(placeHeightMm);
        }

        protected void MoveToSafeZ()
        {
            MoveToZ(safeHeightMm);
        }

        protected String TcpSendPlace()
        {
            if (machineIP.Length <= 0)
            {
                throw new Exception("Неверный вызов.");
            }
            MoveToPlaceZ();
            String res = TcpSendAndPerformJson("vacoff", 0);
            Thread.Sleep(300);
            MoveToSafeZ();
            return res;
        }

        protected String TcpSendPick()
        {
            if (machineIP.Length <= 0)
            {
                throw new Exception("Неверный вызов.");
            }
            MoveToPlaceZ();
            String res = TcpSendAndPerformJson("vacon", 0);
            Thread.Sleep(300);
            Thread.Sleep(300);
            MoveToSafeZ();
            return res;
        }

        protected String TcpSendRotate(int angleSteps)
        {
            return TcpSendAndPerformJson("crotate", (cPositiveClockwise) ? angleSteps : -angleSteps);
        }

        protected String TcpSendAndPerformJson(String command, int arg)
        {
            JObject reqObj = new JObject();
            reqObj.Add("command", command);
            reqObj.Add("argument", arg);
            return TcpSendAndPerformJsonRaw(reqObj.ToString());
        }

        private String TcpSendAndPerformJsonRaw(String json)
        {
            if (machineIP.Length <= 0)
            {
                throw new Exception("Неверный вызов.");
            }
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
            cncFault = true;
            StateChanged.Invoke(this, new EventArgs());
            throw new Exception("Превышено количество попыток TCP-связи со станком.");
        }

        public String GetMachineIP()
        {
            return machineIP;
        }

        public void PictureBoxClick(int x, int y)
        {
            // Check Connected And Have Image?
            if (!IsConnected() || IsFault())
            {
                return;
            }
            if (cameras.Count <= displayCamIdx)
            {
                return;
            }

            // Crop
            Rectangle previewCropRect = Rectangle.Empty;
            Vector2 previewClickLoc = Vector2.Zero;
            int imgW = cameras[displayCamIdx].curRawFrame.Cols;
            int imgH = cameras[displayCamIdx].curRawFrame.Rows;
            Vector2 centerLoc = new Vector2(((float)imgW) / 2.0f, ((float)imgH) / 2.0f);
            float imgAspectRadio = (float)imgW / (float)imgH;
            float pbAspectRatio = (float)previewBoxWidth / (float)previewBoxHeight;
            float aspCoeff = 0.0f;
            if (imgAspectRadio > pbAspectRatio)
            {
                // Stripes Cut-Off Will Be Vertical
                aspCoeff = pbAspectRatio / imgAspectRadio;
                previewCropRect = new Rectangle((int)(imgW * 0.5f * (1.0f - aspCoeff)), 0, (int)(imgW * aspCoeff), imgH);
            } else
            {
                // Stripes Cut-Off Will Be Horizontal
                aspCoeff = imgAspectRadio / pbAspectRatio;
                previewCropRect = new Rectangle(0, (int)(imgH * 0.5f * (1.0f - aspCoeff)), imgW, (int)(imgH * aspCoeff));
            }

            // Zoom
            if (zoomFactor < 1.0f)
            {
                throw new Exception("Неверный уровень зума.");
            } else if (zoomFactor > 1.0f)
            {
                int remWidth = (int)(previewCropRect.Width / zoomFactor);
                int remHeight = (int)(previewCropRect.Height / zoomFactor);
                float infW = ((float)(remWidth - previewCropRect.Width)) / 2.0f;
                float infH = ((float)(remHeight - previewCropRect.Height)) / 2.0f;
                previewCropRect.Inflate((int)infW, (int)infH);
            }

            // Rotate
            if (rotateCamImage || hFlip || vFlip)
            {
                throw new Exception("Функция не поддерживается при повернутом изображении.");
            }

            // FOV
            if (fovXFromPercent != 0 || fovXToPercent != 100 || fovYFromPercent != 0 || fovYToPercent != 100)
            {
                throw new Exception("Функция не поддерживается при ограниченном FOV.");
            }

            // Click Location
            float propX = ((float)x) / ((float)previewBoxWidth);
            float propY = ((float)y) / ((float)previewBoxHeight);
            previewClickLoc = new Vector2((propX * previewCropRect.Width) + previewCropRect.Left, (propY * previewCropRect.Height) + previewCropRect.Top);

            // Check Z
            if (Math.Abs(absMmZ - calibZMm) > 0.1f)
            {
                throw new Exception("Высота Z не калибровочная.");
            }

            // Calculate Offset
            float effImagePxPerMm = imagePxPerMm * (((float)imgW) / ((float)previewBoxWidth));
            SizeF offsetInMm = new SizeF(((previewClickLoc.X - centerLoc.X) / effImagePxPerMm), ((previewClickLoc.Y - centerLoc.Y) / effImagePxPerMm));
            if (Math.Abs(offsetInMm.Width) > 5.0f || Math.Abs(offsetInMm.Height) > 5.0f)
            {
                throw new Exception("Слишком большое перемещение выбрано.");
            }
            StartMoveManual(offsetInMm.Width, offsetInMm.Height, 0.0f);
        }
    }
}