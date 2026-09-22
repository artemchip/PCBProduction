using System.Windows.Forms;

namespace PCBProduction2
{
    partial class CNCForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CNCForm));
            btnMoveCamToSpindle = new Button();
            btnMoveSpindleToCam = new Button();
            tbSpindleSpeed = new TrackBar();
            btnSpindleActivate = new Button();
            btnZNeg = new Button();
            btnZPos = new Button();
            lblMotionCompens = new Label();
            lblPosition = new Label();
            lblMoveAmount = new Label();
            tbMoveAmount = new TrackBar();
            btnYNeg = new Button();
            btnYPos = new Button();
            btnXPos = new Button();
            btnXNeg = new Button();
            tbZoom = new TrackBar();
            pnMotionIndicator = new Panel();
            btnCncConnect = new Button();
            pbCncCamPreview = new PictureBox();
            mainTabControl = new TabControl();
            primarySetupTabPage = new TabPage();
            lblPInstr3 = new Label();
            gbZDir = new GroupBox();
            rbZTowardsWorkpiece = new RadioButton();
            rbZAwayFromWorkpiece = new RadioButton();
            gbYDir = new GroupBox();
            rbYUp = new RadioButton();
            rbYDown = new RadioButton();
            gbXDir = new GroupBox();
            rbXLeft = new RadioButton();
            rbXRight = new RadioButton();
            lblPInstr2 = new Label();
            plSim = new Panel();
            rbXYBottomLeft = new RadioButton();
            rbXYBottomRight = new RadioButton();
            rbXYTopRight = new RadioButton();
            rbXYTopLeft = new RadioButton();
            lblPInstr1 = new Label();
            calibTabPage = new TabPage();
            btnCalibPanelCalculate = new Button();
            btnCalibPanelBottomRight = new Button();
            btnCalibPanelBottomLeft = new Button();
            btnCalibPanelTopRight = new Button();
            btnCalibPanelTopLeft = new Button();
            btnTestCalib = new Button();
            lblInstr12 = new Label();
            nudSkewAngleRad = new NumericUpDown();
            lblSkewValX = new Label();
            nudStepsPerMmY = new NumericUpDown();
            nudStepsPerMmX = new NumericUpDown();
            lblStepsPerMmYS = new Label();
            lblStepsPerMmXS = new Label();
            lblCamSpindleOffset = new Label();
            btnTakeLocCamera = new Button();
            btnTakeLocSpindle = new Button();
            lblInstr11 = new Label();
            nudBacklashMmY = new NumericUpDown();
            nudBacklashMmX = new NumericUpDown();
            lblBacklashMmY = new Label();
            lblBacklashMmX = new Label();
            lblInstr10 = new Label();
            btnApplyMotion = new Button();
            lblInstr9 = new Label();
            lblInstr1 = new Label();
            settingsPage = new TabPage();
            gbCutting = new GroupBox();
            nudIGapMm = new NumericUpDown();
            nudICutMm = new NumericUpDown();
            lblSettings10 = new Label();
            lblSettings11 = new Label();
            gbMain = new GroupBox();
            nudZUpRateSmallMmPerMin = new NumericUpDown();
            nudZDownRateSmallMmPerMin = new NumericUpDown();
            lblSettings4 = new Label();
            lblSettings5 = new Label();
            nudCuttingRateMmPerMin = new NumericUpDown();
            nudCuttingRpm = new NumericUpDown();
            nudDrillDepthMm = new NumericUpDown();
            nudZUpRateBigMmPerMin = new NumericUpDown();
            nudZDownRateBigMmPerMin = new NumericUpDown();
            nudDrillingRpm = new NumericUpDown();
            lblSettings1 = new Label();
            lblSettings8 = new Label();
            lblSettings2 = new Label();
            lblSettings7 = new Label();
            lblSettings3 = new Label();
            lblSettings6 = new Label();
            panelTabPage = new TabPage();
            groupBox1 = new GroupBox();
            cbALExtended = new CheckBox();
            lblAutoLevelStatus = new Label();
            nudALSafeHeightMm = new NumericUpDown();
            lblALSafeHeightMm = new Label();
            nudPanelThicknessMm = new NumericUpDown();
            lblPanelThickness = new Label();
            btnGrblProbe = new Button();
            rbManualLevel = new RadioButton();
            rbAutoLevel = new RadioButton();
            label1 = new Label();
            cbStencilSideHoles = new CheckBox();
            rbStencil = new RadioButton();
            btnCoordRight = new Button();
            btnCoordLeft = new Button();
            rbExPanel = new RadioButton();
            rbNewPanel = new RadioButton();
            lblPanelStartPos = new Label();
            btnCoordStart = new Button();
            lblMInstr1 = new Label();
            machiningTabPage = new TabPage();
            lblHoleList = new Label();
            btnDrillingPreviewAll = new Button();
            btnDrillingPreview = new Button();
            btnCutting = new Button();
            pbCutting = new ProgressBar();
            lvHoles = new ListView();
            lblMInstr2 = new Label();
            pbDrilling = new ProgressBar();
            btnDrilling = new Button();
            btnResetHoleStatus = new Button();
            cbTools = new ComboBox();
            lblSelectTool = new Label();
            depanelizeTabPage = new TabPage();
            lblDCorners = new Label();
            btnDRightTop = new Button();
            btnDLeftTop = new Button();
            btnDepPreview = new Button();
            btnDepanelize = new Button();
            pbDepanelize = new ProgressBar();
            lblDInstr1 = new Label();
            favLocTabPage = new TabPage();
            btnMoveToLoc4 = new Button();
            nudLoc4YMm = new NumericUpDown();
            nudLoc4XMm = new NumericUpDown();
            lblFavLoc4 = new Label();
            btnMoveToLoc3 = new Button();
            nudLoc3YMm = new NumericUpDown();
            nudLoc3XMm = new NumericUpDown();
            lblFavLoc3 = new Label();
            btnMoveToLoc2 = new Button();
            nudLoc2YMm = new NumericUpDown();
            nudLoc2XMm = new NumericUpDown();
            lblFavLoc2 = new Label();
            btnMoveToLoc1 = new Button();
            nudLoc1YMm = new NumericUpDown();
            nudLoc1XMm = new NumericUpDown();
            lblFavLoc1 = new Label();
            holePopupMenu = new ContextMenuStrip(components);
            showHoleItem = new ToolStripMenuItem();
            resetHoleItem = new ToolStripMenuItem();
            camWatchdogTimer = new System.Windows.Forms.Timer(components);
            tbCamFocus = new TrackBar();
            cbImageFlipV = new CheckBox();
            cbImageFlipH = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)tbSpindleSpeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbMoveAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbZoom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbCncCamPreview).BeginInit();
            mainTabControl.SuspendLayout();
            primarySetupTabPage.SuspendLayout();
            gbZDir.SuspendLayout();
            gbYDir.SuspendLayout();
            gbXDir.SuspendLayout();
            plSim.SuspendLayout();
            calibTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudSkewAngleRad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudStepsPerMmY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudStepsPerMmX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudBacklashMmY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudBacklashMmX).BeginInit();
            settingsPage.SuspendLayout();
            gbCutting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudIGapMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudICutMm).BeginInit();
            gbMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudZUpRateSmallMmPerMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudZDownRateSmallMmPerMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCuttingRateMmPerMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCuttingRpm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDrillDepthMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudZUpRateBigMmPerMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudZDownRateBigMmPerMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDrillingRpm).BeginInit();
            panelTabPage.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudALSafeHeightMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPanelThicknessMm).BeginInit();
            machiningTabPage.SuspendLayout();
            depanelizeTabPage.SuspendLayout();
            favLocTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudLoc4YMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc4XMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc3YMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc3XMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc2YMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc2XMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc1YMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc1XMm).BeginInit();
            holePopupMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbCamFocus).BeginInit();
            SuspendLayout();
            // 
            // btnMoveCamToSpindle
            // 
            btnMoveCamToSpindle.Location = new Point(6, 792);
            btnMoveCamToSpindle.Name = "btnMoveCamToSpindle";
            btnMoveCamToSpindle.Size = new Size(199, 23);
            btnMoveCamToSpindle.TabIndex = 57;
            btnMoveCamToSpindle.Text = "Камера на место шпинделя";
            btnMoveCamToSpindle.UseVisualStyleBackColor = true;
            btnMoveCamToSpindle.Click += btnMoveCamToSpindle_Click;
            // 
            // btnMoveSpindleToCam
            // 
            btnMoveSpindleToCam.Location = new Point(6, 761);
            btnMoveSpindleToCam.Name = "btnMoveSpindleToCam";
            btnMoveSpindleToCam.Size = new Size(199, 23);
            btnMoveSpindleToCam.TabIndex = 56;
            btnMoveSpindleToCam.Text = "Шпиндель на место камеры";
            btnMoveSpindleToCam.UseVisualStyleBackColor = true;
            btnMoveSpindleToCam.Click += btnMoveSpindleToCam_Click;
            // 
            // tbSpindleSpeed
            // 
            tbSpindleSpeed.Location = new Point(387, 619);
            tbSpindleSpeed.Maximum = 10000;
            tbSpindleSpeed.Name = "tbSpindleSpeed";
            tbSpindleSpeed.Size = new Size(260, 45);
            tbSpindleSpeed.TabIndex = 55;
            tbSpindleSpeed.TickFrequency = 100;
            tbSpindleSpeed.TickStyle = TickStyle.Both;
            tbSpindleSpeed.Value = 5000;
            // 
            // btnSpindleActivate
            // 
            btnSpindleActivate.Location = new Point(334, 619);
            btnSpindleActivate.Name = "btnSpindleActivate";
            btnSpindleActivate.Size = new Size(45, 45);
            btnSpindleActivate.TabIndex = 54;
            btnSpindleActivate.Text = "◯";
            btnSpindleActivate.UseVisualStyleBackColor = true;
            btnSpindleActivate.Click += btnSpindleActivate_Click;
            // 
            // btnZNeg
            // 
            btnZNeg.Location = new Point(169, 615);
            btnZNeg.Name = "btnZNeg";
            btnZNeg.Size = new Size(36, 38);
            btnZNeg.TabIndex = 44;
            btnZNeg.Tag = "5";
            btnZNeg.Text = "▼";
            btnZNeg.UseVisualStyleBackColor = true;
            btnZNeg.Click += move_Click;
            // 
            // btnZPos
            // 
            btnZPos.Location = new Point(169, 535);
            btnZPos.Name = "btnZPos";
            btnZPos.Size = new Size(36, 38);
            btnZPos.TabIndex = 43;
            btnZPos.Tag = "4";
            btnZPos.Text = "▲";
            btnZPos.UseVisualStyleBackColor = true;
            btnZPos.Click += move_Click;
            // 
            // lblMotionCompens
            // 
            lblMotionCompens.Location = new Point(334, 526);
            lblMotionCompens.Name = "lblMotionCompens";
            lblMotionCompens.Size = new Size(313, 87);
            lblMotionCompens.TabIndex = 36;
            // 
            // lblPosition
            // 
            lblPosition.Location = new Point(220, 746);
            lblPosition.Name = "lblPosition";
            lblPosition.Size = new Size(427, 63);
            lblPosition.TabIndex = 35;
            lblPosition.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblMoveAmount
            // 
            lblMoveAmount.Location = new Point(271, 526);
            lblMoveAmount.Name = "lblMoveAmount";
            lblMoveAmount.Size = new Size(57, 138);
            lblMoveAmount.TabIndex = 29;
            lblMoveAmount.Text = "100 мм\r\n\r\n10 мм\r\n\r\n1 мм\r\n\r\n0.1 мм\r\n\r\n0.01 мм";
            // 
            // tbMoveAmount
            // 
            tbMoveAmount.Location = new Point(220, 520);
            tbMoveAmount.Maximum = 2;
            tbMoveAmount.Minimum = -2;
            tbMoveAmount.Name = "tbMoveAmount";
            tbMoveAmount.Orientation = Orientation.Vertical;
            tbMoveAmount.Size = new Size(45, 144);
            tbMoveAmount.TabIndex = 28;
            tbMoveAmount.TickStyle = TickStyle.Both;
            // 
            // btnYNeg
            // 
            btnYNeg.Location = new Point(45, 615);
            btnYNeg.Name = "btnYNeg";
            btnYNeg.Size = new Size(36, 38);
            btnYNeg.TabIndex = 27;
            btnYNeg.Tag = "3";
            btnYNeg.Text = "▼";
            btnYNeg.UseVisualStyleBackColor = true;
            btnYNeg.Click += move_Click;
            // 
            // btnYPos
            // 
            btnYPos.Location = new Point(45, 535);
            btnYPos.Name = "btnYPos";
            btnYPos.Size = new Size(36, 38);
            btnYPos.TabIndex = 26;
            btnYPos.Tag = "2";
            btnYPos.Text = "▲";
            btnYPos.UseVisualStyleBackColor = true;
            btnYPos.Click += move_Click;
            // 
            // btnXPos
            // 
            btnXPos.Location = new Point(84, 575);
            btnXPos.Name = "btnXPos";
            btnXPos.Size = new Size(36, 38);
            btnXPos.TabIndex = 25;
            btnXPos.Tag = "1";
            btnXPos.Text = "▶";
            btnXPos.UseVisualStyleBackColor = true;
            btnXPos.Click += move_Click;
            // 
            // btnXNeg
            // 
            btnXNeg.Location = new Point(7, 575);
            btnXNeg.Name = "btnXNeg";
            btnXNeg.Size = new Size(36, 38);
            btnXNeg.TabIndex = 24;
            btnXNeg.Tag = "0";
            btnXNeg.Text = "◀";
            btnXNeg.UseVisualStyleBackColor = true;
            btnXNeg.Click += move_Click;
            // 
            // tbZoom
            // 
            tbZoom.Location = new Point(241, 451);
            tbZoom.Minimum = 1;
            tbZoom.Name = "tbZoom";
            tbZoom.Size = new Size(191, 45);
            tbZoom.TabIndex = 23;
            tbZoom.TickFrequency = 10;
            tbZoom.TickStyle = TickStyle.Both;
            tbZoom.Value = 1;
            tbZoom.ValueChanged += tbZoom_ValueChanged;
            // 
            // pnMotionIndicator
            // 
            pnMotionIndicator.Location = new Point(178, 452);
            pnMotionIndicator.Name = "pnMotionIndicator";
            pnMotionIndicator.Size = new Size(57, 44);
            pnMotionIndicator.TabIndex = 16;
            // 
            // btnCncConnect
            // 
            btnCncConnect.Location = new Point(6, 452);
            btnCncConnect.Name = "btnCncConnect";
            btnCncConnect.Size = new Size(166, 51);
            btnCncConnect.TabIndex = 10;
            btnCncConnect.Text = "Подключиться к станку\r\n(не используйте USB-hub!)";
            btnCncConnect.UseVisualStyleBackColor = true;
            btnCncConnect.Click += btnCncConnect_Click;
            // 
            // pbCncCamPreview
            // 
            pbCncCamPreview.Location = new Point(6, 6);
            pbCncCamPreview.Name = "pbCncCamPreview";
            pbCncCamPreview.Size = new Size(641, 346);
            pbCncCamPreview.SizeMode = PictureBoxSizeMode.CenterImage;
            pbCncCamPreview.TabIndex = 0;
            pbCncCamPreview.TabStop = false;
            // 
            // mainTabControl
            // 
            mainTabControl.Controls.Add(primarySetupTabPage);
            mainTabControl.Controls.Add(calibTabPage);
            mainTabControl.Controls.Add(settingsPage);
            mainTabControl.Controls.Add(panelTabPage);
            mainTabControl.Controls.Add(machiningTabPage);
            mainTabControl.Controls.Add(depanelizeTabPage);
            mainTabControl.Controls.Add(favLocTabPage);
            mainTabControl.Location = new Point(653, 6);
            mainTabControl.Name = "mainTabControl";
            mainTabControl.SelectedIndex = 0;
            mainTabControl.Size = new Size(739, 818);
            mainTabControl.TabIndex = 58;
            // 
            // primarySetupTabPage
            // 
            primarySetupTabPage.Controls.Add(lblPInstr3);
            primarySetupTabPage.Controls.Add(gbZDir);
            primarySetupTabPage.Controls.Add(gbYDir);
            primarySetupTabPage.Controls.Add(gbXDir);
            primarySetupTabPage.Controls.Add(lblPInstr2);
            primarySetupTabPage.Controls.Add(plSim);
            primarySetupTabPage.Controls.Add(lblPInstr1);
            primarySetupTabPage.Location = new Point(4, 24);
            primarySetupTabPage.Name = "primarySetupTabPage";
            primarySetupTabPage.Size = new Size(731, 790);
            primarySetupTabPage.TabIndex = 5;
            primarySetupTabPage.Text = "Первичная настройка";
            primarySetupTabPage.UseVisualStyleBackColor = true;
            // 
            // lblPInstr3
            // 
            lblPInstr3.Location = new Point(6, 402);
            lblPInstr3.Name = "lblPInstr3";
            lblPInstr3.Size = new Size(722, 34);
            lblPInstr3.TabIndex = 6;
            lblPInstr3.Text = "2. Заново подключитесь к станку чтобы изменения вступили в силу и можете переходить к калибровке (на следующую вкладку).";
            // 
            // gbZDir
            // 
            gbZDir.Controls.Add(rbZTowardsWorkpiece);
            gbZDir.Controls.Add(rbZAwayFromWorkpiece);
            gbZDir.Location = new Point(212, 306);
            gbZDir.Name = "gbZDir";
            gbZDir.Size = new Size(314, 58);
            gbZDir.TabIndex = 5;
            gbZDir.TabStop = false;
            gbZDir.Text = "Ось Z";
            // 
            // rbZTowardsWorkpiece
            // 
            rbZTowardsWorkpiece.AutoSize = true;
            rbZTowardsWorkpiece.Location = new Point(184, 22);
            rbZTowardsWorkpiece.Name = "rbZTowardsWorkpiece";
            rbZTowardsWorkpiece.Size = new Size(127, 19);
            rbZTowardsWorkpiece.TabIndex = 1;
            rbZTowardsWorkpiece.TabStop = true;
            rbZTowardsWorkpiece.Text = "К заготовке (в неё)";
            rbZTowardsWorkpiece.UseVisualStyleBackColor = true;
            rbZTowardsWorkpiece.CheckedChanged += rbZTowardsWorkpiece_CheckedChanged;
            // 
            // rbZAwayFromWorkpiece
            // 
            rbZAwayFromWorkpiece.AutoSize = true;
            rbZAwayFromWorkpiece.Location = new Point(6, 22);
            rbZAwayFromWorkpiece.Name = "rbZAwayFromWorkpiece";
            rbZAwayFromWorkpiece.Size = new Size(137, 19);
            rbZAwayFromWorkpiece.TabIndex = 0;
            rbZAwayFromWorkpiece.TabStop = true;
            rbZAwayFromWorkpiece.Text = "От заготовки (вверх)";
            rbZAwayFromWorkpiece.UseVisualStyleBackColor = true;
            rbZAwayFromWorkpiece.CheckedChanged += rbZAwayFromWorkpiece_CheckedChanged;
            // 
            // gbYDir
            // 
            gbYDir.Controls.Add(rbYUp);
            gbYDir.Controls.Add(rbYDown);
            gbYDir.Location = new Point(6, 339);
            gbYDir.Name = "gbYDir";
            gbYDir.Size = new Size(200, 58);
            gbYDir.TabIndex = 4;
            gbYDir.TabStop = false;
            gbYDir.Text = "Ось Y";
            // 
            // rbYUp
            // 
            rbYUp.AutoSize = true;
            rbYUp.Location = new Point(129, 22);
            rbYUp.Name = "rbYUp";
            rbYUp.Size = new Size(56, 19);
            rbYUp.TabIndex = 1;
            rbYUp.TabStop = true;
            rbYUp.Text = "Вверх";
            rbYUp.UseVisualStyleBackColor = true;
            rbYUp.CheckedChanged += rbYUp_CheckedChanged;
            // 
            // rbYDown
            // 
            rbYDown.AutoSize = true;
            rbYDown.Location = new Point(3, 22);
            rbYDown.Name = "rbYDown";
            rbYDown.Size = new Size(51, 19);
            rbYDown.TabIndex = 0;
            rbYDown.TabStop = true;
            rbYDown.Text = "Вниз";
            rbYDown.UseVisualStyleBackColor = true;
            rbYDown.CheckedChanged += rbYDown_CheckedChanged;
            // 
            // gbXDir
            // 
            gbXDir.Controls.Add(rbXLeft);
            gbXDir.Controls.Add(rbXRight);
            gbXDir.Location = new Point(6, 275);
            gbXDir.Name = "gbXDir";
            gbXDir.Size = new Size(200, 58);
            gbXDir.TabIndex = 3;
            gbXDir.TabStop = false;
            gbXDir.Text = "Ось X";
            // 
            // rbXLeft
            // 
            rbXLeft.AutoSize = true;
            rbXLeft.Location = new Point(129, 22);
            rbXLeft.Name = "rbXLeft";
            rbXLeft.Size = new Size(58, 19);
            rbXLeft.TabIndex = 1;
            rbXLeft.TabStop = true;
            rbXLeft.Text = "Влево";
            rbXLeft.UseVisualStyleBackColor = true;
            rbXLeft.CheckedChanged += rbXLeft_CheckedChanged;
            // 
            // rbXRight
            // 
            rbXRight.AutoSize = true;
            rbXRight.Location = new Point(3, 22);
            rbXRight.Name = "rbXRight";
            rbXRight.Size = new Size(65, 19);
            rbXRight.TabIndex = 0;
            rbXRight.TabStop = true;
            rbXRight.Text = "Вправо";
            rbXRight.UseVisualStyleBackColor = true;
            rbXRight.CheckedChanged += rbXRight_CheckedChanged;
            // 
            // lblPInstr2
            // 
            lblPInstr2.Location = new Point(6, 257);
            lblPInstr2.Name = "lblPInstr2";
            lblPInstr2.Size = new Size(722, 25);
            lblPInstr2.TabIndex = 2;
            lblPInstr2.Text = "1. При движении осей в положительную сторону через grblControl, куда движется шпиндель по заготовке.";
            // 
            // plSim
            // 
            plSim.BackColor = Color.FromArgb(255, 128, 128);
            plSim.Controls.Add(rbXYBottomLeft);
            plSim.Controls.Add(rbXYBottomRight);
            plSim.Controls.Add(rbXYTopRight);
            plSim.Controls.Add(rbXYTopLeft);
            plSim.Location = new Point(6, 44);
            plSim.Name = "plSim";
            plSim.Size = new Size(200, 200);
            plSim.TabIndex = 1;
            // 
            // rbXYBottomLeft
            // 
            rbXYBottomLeft.AutoSize = true;
            rbXYBottomLeft.Location = new Point(3, 184);
            rbXYBottomLeft.Name = "rbXYBottomLeft";
            rbXYBottomLeft.Size = new Size(14, 13);
            rbXYBottomLeft.TabIndex = 3;
            rbXYBottomLeft.TabStop = true;
            rbXYBottomLeft.UseVisualStyleBackColor = true;
            rbXYBottomLeft.CheckedChanged += rbXYBottomLeft_CheckedChanged;
            // 
            // rbXYBottomRight
            // 
            rbXYBottomRight.AutoSize = true;
            rbXYBottomRight.Location = new Point(183, 184);
            rbXYBottomRight.Name = "rbXYBottomRight";
            rbXYBottomRight.Size = new Size(14, 13);
            rbXYBottomRight.TabIndex = 2;
            rbXYBottomRight.TabStop = true;
            rbXYBottomRight.UseVisualStyleBackColor = true;
            rbXYBottomRight.CheckedChanged += rbXYBottomRight_CheckedChanged;
            // 
            // rbXYTopRight
            // 
            rbXYTopRight.AutoSize = true;
            rbXYTopRight.Location = new Point(183, 3);
            rbXYTopRight.Name = "rbXYTopRight";
            rbXYTopRight.Size = new Size(14, 13);
            rbXYTopRight.TabIndex = 1;
            rbXYTopRight.TabStop = true;
            rbXYTopRight.UseVisualStyleBackColor = true;
            rbXYTopRight.CheckedChanged += rbXYTopRight_CheckedChanged;
            // 
            // rbXYTopLeft
            // 
            rbXYTopLeft.AutoSize = true;
            rbXYTopLeft.Location = new Point(3, 3);
            rbXYTopLeft.Name = "rbXYTopLeft";
            rbXYTopLeft.Size = new Size(14, 13);
            rbXYTopLeft.TabIndex = 0;
            rbXYTopLeft.TabStop = true;
            rbXYTopLeft.UseVisualStyleBackColor = true;
            rbXYTopLeft.CheckedChanged += rbXYTopLeft_CheckedChanged;
            // 
            // lblPInstr1
            // 
            lblPInstr1.Location = new Point(6, 7);
            lblPInstr1.Name = "lblPInstr1";
            lblPInstr1.Size = new Size(722, 34);
            lblPInstr1.TabIndex = 0;
            lblPInstr1.Text = "0. В каком углу по отношению к лежащей на нем заготовке, если смотреть на неё сверху, находится шпиндель в домашнем положении (home) станка?";
            // 
            // calibTabPage
            // 
            calibTabPage.Controls.Add(btnCalibPanelCalculate);
            calibTabPage.Controls.Add(btnCalibPanelBottomRight);
            calibTabPage.Controls.Add(btnCalibPanelBottomLeft);
            calibTabPage.Controls.Add(btnCalibPanelTopRight);
            calibTabPage.Controls.Add(btnCalibPanelTopLeft);
            calibTabPage.Controls.Add(btnTestCalib);
            calibTabPage.Controls.Add(lblInstr12);
            calibTabPage.Controls.Add(nudSkewAngleRad);
            calibTabPage.Controls.Add(lblSkewValX);
            calibTabPage.Controls.Add(nudStepsPerMmY);
            calibTabPage.Controls.Add(nudStepsPerMmX);
            calibTabPage.Controls.Add(lblStepsPerMmYS);
            calibTabPage.Controls.Add(lblStepsPerMmXS);
            calibTabPage.Controls.Add(lblCamSpindleOffset);
            calibTabPage.Controls.Add(btnTakeLocCamera);
            calibTabPage.Controls.Add(btnTakeLocSpindle);
            calibTabPage.Controls.Add(lblInstr11);
            calibTabPage.Controls.Add(nudBacklashMmY);
            calibTabPage.Controls.Add(nudBacklashMmX);
            calibTabPage.Controls.Add(lblBacklashMmY);
            calibTabPage.Controls.Add(lblBacklashMmX);
            calibTabPage.Controls.Add(lblInstr10);
            calibTabPage.Controls.Add(btnApplyMotion);
            calibTabPage.Controls.Add(lblInstr9);
            calibTabPage.Controls.Add(lblInstr1);
            calibTabPage.Location = new Point(4, 24);
            calibTabPage.Name = "calibTabPage";
            calibTabPage.Padding = new Padding(3);
            calibTabPage.Size = new Size(731, 790);
            calibTabPage.TabIndex = 0;
            calibTabPage.Text = "Калибровка станка";
            calibTabPage.UseVisualStyleBackColor = true;
            // 
            // btnCalibPanelCalculate
            // 
            btnCalibPanelCalculate.Location = new Point(321, 123);
            btnCalibPanelCalculate.Name = "btnCalibPanelCalculate";
            btnCalibPanelCalculate.Size = new Size(75, 23);
            btnCalibPanelCalculate.TabIndex = 87;
            btnCalibPanelCalculate.Text = "Расчет";
            btnCalibPanelCalculate.UseVisualStyleBackColor = true;
            btnCalibPanelCalculate.Click += btnCalibPanelCalculate_Click;
            // 
            // btnCalibPanelBottomRight
            // 
            btnCalibPanelBottomRight.Location = new Point(162, 139);
            btnCalibPanelBottomRight.Name = "btnCalibPanelBottomRight";
            btnCalibPanelBottomRight.Size = new Size(153, 23);
            btnCalibPanelBottomRight.TabIndex = 86;
            btnCalibPanelBottomRight.Text = "Правый нижний угол";
            btnCalibPanelBottomRight.UseVisualStyleBackColor = true;
            btnCalibPanelBottomRight.Click += btnCalibPanelBottomRight_Click;
            // 
            // btnCalibPanelBottomLeft
            // 
            btnCalibPanelBottomLeft.Location = new Point(3, 139);
            btnCalibPanelBottomLeft.Name = "btnCalibPanelBottomLeft";
            btnCalibPanelBottomLeft.Size = new Size(153, 23);
            btnCalibPanelBottomLeft.TabIndex = 85;
            btnCalibPanelBottomLeft.Text = "Левый нижний угол";
            btnCalibPanelBottomLeft.UseVisualStyleBackColor = true;
            btnCalibPanelBottomLeft.Click += btnCalibPanelBottomLeft_Click;
            // 
            // btnCalibPanelTopRight
            // 
            btnCalibPanelTopRight.Location = new Point(162, 110);
            btnCalibPanelTopRight.Name = "btnCalibPanelTopRight";
            btnCalibPanelTopRight.Size = new Size(153, 23);
            btnCalibPanelTopRight.TabIndex = 84;
            btnCalibPanelTopRight.Text = "Правый верхний угол";
            btnCalibPanelTopRight.UseVisualStyleBackColor = true;
            btnCalibPanelTopRight.Click += btnCalibPanelTopRight_Click;
            // 
            // btnCalibPanelTopLeft
            // 
            btnCalibPanelTopLeft.Location = new Point(3, 110);
            btnCalibPanelTopLeft.Name = "btnCalibPanelTopLeft";
            btnCalibPanelTopLeft.Size = new Size(153, 23);
            btnCalibPanelTopLeft.TabIndex = 83;
            btnCalibPanelTopLeft.Text = "Левый верхний угол";
            btnCalibPanelTopLeft.UseVisualStyleBackColor = true;
            btnCalibPanelTopLeft.Click += btnCalibPanelTopLeft_Click;
            // 
            // btnTestCalib
            // 
            btnTestCalib.Location = new Point(6, 533);
            btnTestCalib.Name = "btnTestCalib";
            btnTestCalib.Size = new Size(75, 23);
            btnTestCalib.TabIndex = 82;
            btnTestCalib.Text = "Тест";
            btnTestCalib.UseVisualStyleBackColor = true;
            btnTestCalib.Click += btnTestCalib_Click;
            // 
            // lblInstr12
            // 
            lblInstr12.Location = new Point(6, 496);
            lblInstr12.Name = "lblInstr12";
            lblInstr12.Size = new Size(603, 34);
            lblInstr12.TabIndex = 81;
            lblInstr12.Text = "4. Протестируйте компенсации. При нажатии на кнопку \"Тест\" камера должна поочередно переместиться по всем реперным отверстиям.";
            // 
            // nudSkewAngleRad
            // 
            nudSkewAngleRad.DecimalPlaces = 6;
            nudSkewAngleRad.Increment = new decimal(new int[] { 1, 0, 0, 262144 });
            nudSkewAngleRad.Location = new Point(120, 221);
            nudSkewAngleRad.Maximum = new decimal(new int[] { 1, 0, 0, 65536 });
            nudSkewAngleRad.Minimum = new decimal(new int[] { 1, 0, 0, -2147418112 });
            nudSkewAngleRad.Name = "nudSkewAngleRad";
            nudSkewAngleRad.Size = new Size(82, 23);
            nudSkewAngleRad.TabIndex = 80;
            // 
            // lblSkewValX
            // 
            lblSkewValX.Location = new Point(5, 223);
            lblSkewValX.Name = "lblSkewValX";
            lblSkewValX.Size = new Size(109, 17);
            lblSkewValX.TabIndex = 78;
            lblSkewValX.Text = "Кривизна, рад:";
            // 
            // nudStepsPerMmY
            // 
            nudStepsPerMmY.DecimalPlaces = 3;
            nudStepsPerMmY.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudStepsPerMmY.Location = new Point(120, 194);
            nudStepsPerMmY.Maximum = new decimal(new int[] { 3200, 0, 0, 0 });
            nudStepsPerMmY.Name = "nudStepsPerMmY";
            nudStepsPerMmY.Size = new Size(82, 23);
            nudStepsPerMmY.TabIndex = 77;
            // 
            // nudStepsPerMmX
            // 
            nudStepsPerMmX.DecimalPlaces = 3;
            nudStepsPerMmX.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudStepsPerMmX.Location = new Point(120, 170);
            nudStepsPerMmX.Maximum = new decimal(new int[] { 3200, 0, 0, 0 });
            nudStepsPerMmX.Name = "nudStepsPerMmX";
            nudStepsPerMmX.Size = new Size(82, 23);
            nudStepsPerMmX.TabIndex = 76;
            // 
            // lblStepsPerMmYS
            // 
            lblStepsPerMmYS.Location = new Point(6, 196);
            lblStepsPerMmYS.Name = "lblStepsPerMmYS";
            lblStepsPerMmYS.Size = new Size(93, 17);
            lblStepsPerMmYS.TabIndex = 75;
            lblStepsPerMmYS.Text = "Шаги на мм Y:";
            // 
            // lblStepsPerMmXS
            // 
            lblStepsPerMmXS.Location = new Point(6, 172);
            lblStepsPerMmXS.Name = "lblStepsPerMmXS";
            lblStepsPerMmXS.Size = new Size(97, 17);
            lblStepsPerMmXS.TabIndex = 74;
            lblStepsPerMmXS.Text = "Шаги на мм X:";
            // 
            // lblCamSpindleOffset
            // 
            lblCamSpindleOffset.Location = new Point(6, 466);
            lblCamSpindleOffset.Name = "lblCamSpindleOffset";
            lblCamSpindleOffset.Size = new Size(525, 26);
            lblCamSpindleOffset.TabIndex = 73;
            // 
            // btnTakeLocCamera
            // 
            btnTakeLocCamera.Location = new Point(85, 440);
            btnTakeLocCamera.Name = "btnTakeLocCamera";
            btnTakeLocCamera.Size = new Size(75, 23);
            btnTakeLocCamera.TabIndex = 72;
            btnTakeLocCamera.Text = "Камера";
            btnTakeLocCamera.UseVisualStyleBackColor = true;
            btnTakeLocCamera.Click += btnTakeLocCamera_Click;
            // 
            // btnTakeLocSpindle
            // 
            btnTakeLocSpindle.Location = new Point(6, 440);
            btnTakeLocSpindle.Name = "btnTakeLocSpindle";
            btnTakeLocSpindle.Size = new Size(75, 23);
            btnTakeLocSpindle.TabIndex = 71;
            btnTakeLocSpindle.Text = "Опора";
            btnTakeLocSpindle.UseVisualStyleBackColor = true;
            btnTakeLocSpindle.Click += btnTakeLocSpindle_Click;
            // 
            // lblInstr11
            // 
            lblInstr11.Location = new Point(6, 402);
            lblInstr11.Name = "lblInstr11";
            lblInstr11.Size = new Size(603, 34);
            lblInstr11.TabIndex = 70;
            lblInstr11.Text = "3. Смещение от камеры до шпинделя. Используя управляющие кнопки ниже изображение, просверлите отверстие. Нажмите \"Опора\", затем переместитесь центром вида камеры к ней и нажмите \"Камера\".";
            // 
            // nudBacklashMmY
            // 
            nudBacklashMmY.DecimalPlaces = 3;
            nudBacklashMmY.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
            nudBacklashMmY.Location = new Point(164, 376);
            nudBacklashMmY.Maximum = new decimal(new int[] { 1, 0, 0, 65536 });
            nudBacklashMmY.Name = "nudBacklashMmY";
            nudBacklashMmY.Size = new Size(86, 23);
            nudBacklashMmY.TabIndex = 69;
            nudBacklashMmY.ValueChanged += nudBacklashY_ValueChanged;
            // 
            // nudBacklashMmX
            // 
            nudBacklashMmX.DecimalPlaces = 3;
            nudBacklashMmX.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
            nudBacklashMmX.Location = new Point(164, 335);
            nudBacklashMmX.Maximum = new decimal(new int[] { 1, 0, 0, 65536 });
            nudBacklashMmX.Name = "nudBacklashMmX";
            nudBacklashMmX.Size = new Size(86, 23);
            nudBacklashMmX.TabIndex = 68;
            nudBacklashMmX.ValueChanged += nudBacklashX_ValueChanged;
            // 
            // lblBacklashMmY
            // 
            lblBacklashMmY.AutoSize = true;
            lblBacklashMmY.Location = new Point(6, 379);
            lblBacklashMmY.Name = "lblBacklashMmY";
            lblBacklashMmY.Size = new Size(152, 15);
            lblBacklashMmY.TabIndex = 67;
            lblBacklashMmY.Text = "Величина компенсации Y:";
            // 
            // lblBacklashMmX
            // 
            lblBacklashMmX.AutoSize = true;
            lblBacklashMmX.Location = new Point(8, 337);
            lblBacklashMmX.Name = "lblBacklashMmX";
            lblBacklashMmX.Size = new Size(152, 15);
            lblBacklashMmX.TabIndex = 66;
            lblBacklashMmX.Text = "Величина компенсации X:";
            // 
            // lblInstr10
            // 
            lblInstr10.Location = new Point(6, 271);
            lblInstr10.Name = "lblInstr10";
            lblInstr10.Size = new Size(719, 49);
            lblInstr10.TabIndex = 65;
            lblInstr10.Text = resources.GetString("lblInstr10.Text");
            // 
            // btnApplyMotion
            // 
            btnApplyMotion.Location = new Point(9, 244);
            btnApplyMotion.Name = "btnApplyMotion";
            btnApplyMotion.Size = new Size(96, 23);
            btnApplyMotion.TabIndex = 63;
            btnApplyMotion.Text = "Применить";
            btnApplyMotion.UseVisualStyleBackColor = true;
            btnApplyMotion.Click += btnApplyMotion_Click;
            // 
            // lblInstr9
            // 
            lblInstr9.Location = new Point(3, 56);
            lblInstr9.Name = "lblInstr9";
            lblInstr9.Size = new Size(722, 51);
            lblInstr9.TabIndex = 60;
            lblInstr9.Text = resources.GetString("lblInstr9.Text");
            // 
            // lblInstr1
            // 
            lblInstr1.Location = new Point(3, 3);
            lblInstr1.Name = "lblInstr1";
            lblInstr1.Size = new Size(722, 53);
            lblInstr1.TabIndex = 54;
            lblInstr1.Text = resources.GetString("lblInstr1.Text");
            // 
            // settingsPage
            // 
            settingsPage.Controls.Add(gbCutting);
            settingsPage.Controls.Add(gbMain);
            settingsPage.Location = new Point(4, 24);
            settingsPage.Name = "settingsPage";
            settingsPage.Size = new Size(731, 790);
            settingsPage.TabIndex = 4;
            settingsPage.Text = "Настройки";
            settingsPage.UseVisualStyleBackColor = true;
            // 
            // gbCutting
            // 
            gbCutting.Controls.Add(nudIGapMm);
            gbCutting.Controls.Add(nudICutMm);
            gbCutting.Controls.Add(lblSettings10);
            gbCutting.Controls.Add(lblSettings11);
            gbCutting.Location = new Point(9, 280);
            gbCutting.Name = "gbCutting";
            gbCutting.Size = new Size(590, 79);
            gbCutting.TabIndex = 12;
            gbCutting.TabStop = false;
            gbCutting.Text = "Резка";
            // 
            // nudIGapMm
            // 
            nudIGapMm.DecimalPlaces = 1;
            nudIGapMm.Location = new Point(505, 49);
            nudIGapMm.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            nudIGapMm.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudIGapMm.Name = "nudIGapMm";
            nudIGapMm.Size = new Size(79, 23);
            nudIGapMm.TabIndex = 7;
            nudIGapMm.Value = new decimal(new int[] { 3, 0, 0, 0 });
            nudIGapMm.ValueChanged += nudIGapMm_ValueChanged;
            // 
            // nudICutMm
            // 
            nudICutMm.DecimalPlaces = 1;
            nudICutMm.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            nudICutMm.Location = new Point(505, 19);
            nudICutMm.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            nudICutMm.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            nudICutMm.Name = "nudICutMm";
            nudICutMm.Size = new Size(79, 23);
            nudICutMm.TabIndex = 6;
            nudICutMm.Value = new decimal(new int[] { 18, 0, 0, 0 });
            nudICutMm.ValueChanged += nudICutMm_ValueChanged;
            // 
            // lblSettings10
            // 
            lblSettings10.Location = new Point(11, 21);
            lblSettings10.Name = "lblSettings10";
            lblSettings10.Size = new Size(220, 18);
            lblSettings10.TabIndex = 0;
            lblSettings10.Text = "Длина реза, мм:";
            // 
            // lblSettings11
            // 
            lblSettings11.Location = new Point(11, 48);
            lblSettings11.Name = "lblSettings11";
            lblSettings11.Size = new Size(220, 17);
            lblSettings11.TabIndex = 1;
            lblSettings11.Text = "Длина паузы, мм:";
            // 
            // gbMain
            // 
            gbMain.Controls.Add(nudZUpRateSmallMmPerMin);
            gbMain.Controls.Add(nudZDownRateSmallMmPerMin);
            gbMain.Controls.Add(lblSettings4);
            gbMain.Controls.Add(lblSettings5);
            gbMain.Controls.Add(nudCuttingRateMmPerMin);
            gbMain.Controls.Add(nudCuttingRpm);
            gbMain.Controls.Add(nudDrillDepthMm);
            gbMain.Controls.Add(nudZUpRateBigMmPerMin);
            gbMain.Controls.Add(nudZDownRateBigMmPerMin);
            gbMain.Controls.Add(nudDrillingRpm);
            gbMain.Controls.Add(lblSettings1);
            gbMain.Controls.Add(lblSettings8);
            gbMain.Controls.Add(lblSettings2);
            gbMain.Controls.Add(lblSettings7);
            gbMain.Controls.Add(lblSettings3);
            gbMain.Controls.Add(lblSettings6);
            gbMain.Location = new Point(9, 8);
            gbMain.Name = "gbMain";
            gbMain.Size = new Size(590, 266);
            gbMain.TabIndex = 6;
            gbMain.TabStop = false;
            gbMain.Text = "Основное";
            // 
            // nudZUpRateSmallMmPerMin
            // 
            nudZUpRateSmallMmPerMin.Location = new Point(505, 144);
            nudZUpRateSmallMmPerMin.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudZUpRateSmallMmPerMin.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            nudZUpRateSmallMmPerMin.Name = "nudZUpRateSmallMmPerMin";
            nudZUpRateSmallMmPerMin.Size = new Size(79, 23);
            nudZUpRateSmallMmPerMin.TabIndex = 15;
            nudZUpRateSmallMmPerMin.Value = new decimal(new int[] { 200, 0, 0, 0 });
            nudZUpRateSmallMmPerMin.ValueChanged += nudZUpRateSmallMmPerMin_ValueChanged;
            // 
            // nudZDownRateSmallMmPerMin
            // 
            nudZDownRateSmallMmPerMin.Location = new Point(505, 113);
            nudZDownRateSmallMmPerMin.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudZDownRateSmallMmPerMin.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            nudZDownRateSmallMmPerMin.Name = "nudZDownRateSmallMmPerMin";
            nudZDownRateSmallMmPerMin.Size = new Size(79, 23);
            nudZDownRateSmallMmPerMin.TabIndex = 14;
            nudZDownRateSmallMmPerMin.Value = new decimal(new int[] { 120, 0, 0, 0 });
            nudZDownRateSmallMmPerMin.ValueChanged += nudZDownRateSmallMmPerMin_ValueChanged;
            // 
            // lblSettings4
            // 
            lblSettings4.Location = new Point(11, 115);
            lblSettings4.Name = "lblSettings4";
            lblSettings4.Size = new Size(337, 17);
            lblSettings4.TabIndex = 12;
            lblSettings4.Text = "Скорость опускания сверла, мм/мин (отверстия <0.5мм):";
            // 
            // lblSettings5
            // 
            lblSettings5.Location = new Point(11, 146);
            lblSettings5.Name = "lblSettings5";
            lblSettings5.Size = new Size(337, 17);
            lblSettings5.TabIndex = 13;
            lblSettings5.Text = "Скорость поднятия сверла, мм/мин (отверстия <0.5мм):";
            // 
            // nudCuttingRateMmPerMin
            // 
            nudCuttingRateMmPerMin.Location = new Point(505, 233);
            nudCuttingRateMmPerMin.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudCuttingRateMmPerMin.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            nudCuttingRateMmPerMin.Name = "nudCuttingRateMmPerMin";
            nudCuttingRateMmPerMin.Size = new Size(79, 23);
            nudCuttingRateMmPerMin.TabIndex = 11;
            nudCuttingRateMmPerMin.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudCuttingRateMmPerMin.ValueChanged += nudCuttingRateMmPerMin_ValueChanged;
            // 
            // nudCuttingRpm
            // 
            nudCuttingRpm.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            nudCuttingRpm.Location = new Point(505, 204);
            nudCuttingRpm.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudCuttingRpm.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nudCuttingRpm.Name = "nudCuttingRpm";
            nudCuttingRpm.Size = new Size(79, 23);
            nudCuttingRpm.TabIndex = 10;
            nudCuttingRpm.Value = new decimal(new int[] { 7000, 0, 0, 0 });
            nudCuttingRpm.ValueChanged += nudCuttingRpm_ValueChanged;
            // 
            // nudDrillDepthMm
            // 
            nudDrillDepthMm.DecimalPlaces = 1;
            nudDrillDepthMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudDrillDepthMm.Location = new Point(505, 175);
            nudDrillDepthMm.Maximum = new decimal(new int[] { 6, 0, 0, 0 });
            nudDrillDepthMm.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDrillDepthMm.Name = "nudDrillDepthMm";
            nudDrillDepthMm.Size = new Size(79, 23);
            nudDrillDepthMm.TabIndex = 9;
            nudDrillDepthMm.Value = new decimal(new int[] { 35, 0, 0, 65536 });
            nudDrillDepthMm.ValueChanged += nudDrillDepthMm_ValueChanged;
            // 
            // nudZUpRateBigMmPerMin
            // 
            nudZUpRateBigMmPerMin.Location = new Point(505, 80);
            nudZUpRateBigMmPerMin.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudZUpRateBigMmPerMin.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            nudZUpRateBigMmPerMin.Name = "nudZUpRateBigMmPerMin";
            nudZUpRateBigMmPerMin.Size = new Size(79, 23);
            nudZUpRateBigMmPerMin.TabIndex = 8;
            nudZUpRateBigMmPerMin.Value = new decimal(new int[] { 150, 0, 0, 0 });
            nudZUpRateBigMmPerMin.ValueChanged += nudZUpRateMmPerMin_ValueChanged;
            // 
            // nudZDownRateBigMmPerMin
            // 
            nudZDownRateBigMmPerMin.Location = new Point(505, 49);
            nudZDownRateBigMmPerMin.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudZDownRateBigMmPerMin.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            nudZDownRateBigMmPerMin.Name = "nudZDownRateBigMmPerMin";
            nudZDownRateBigMmPerMin.Size = new Size(79, 23);
            nudZDownRateBigMmPerMin.TabIndex = 7;
            nudZDownRateBigMmPerMin.Value = new decimal(new int[] { 80, 0, 0, 0 });
            nudZDownRateBigMmPerMin.ValueChanged += nudZDownRateMmPerMin_ValueChanged;
            // 
            // nudDrillingRpm
            // 
            nudDrillingRpm.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            nudDrillingRpm.Location = new Point(505, 19);
            nudDrillingRpm.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudDrillingRpm.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nudDrillingRpm.Name = "nudDrillingRpm";
            nudDrillingRpm.Size = new Size(79, 23);
            nudDrillingRpm.TabIndex = 6;
            nudDrillingRpm.Value = new decimal(new int[] { 4000, 0, 0, 0 });
            nudDrillingRpm.ValueChanged += nudDrillingRpm_ValueChanged;
            // 
            // lblSettings1
            // 
            lblSettings1.Location = new Point(11, 21);
            lblSettings1.Name = "lblSettings1";
            lblSettings1.Size = new Size(220, 18);
            lblSettings1.TabIndex = 0;
            lblSettings1.Text = "Обороты шпинделя при сверлении:";
            // 
            // lblSettings8
            // 
            lblSettings8.Location = new Point(11, 235);
            lblSettings8.Name = "lblSettings8";
            lblSettings8.Size = new Size(220, 18);
            lblSettings8.TabIndex = 5;
            lblSettings8.Text = "Скорость подачи резания, мм/мин:";
            // 
            // lblSettings2
            // 
            lblSettings2.Location = new Point(11, 51);
            lblSettings2.Name = "lblSettings2";
            lblSettings2.Size = new Size(220, 17);
            lblSettings2.TabIndex = 1;
            lblSettings2.Text = "Скорость опускания сверла, мм/мин:";
            // 
            // lblSettings7
            // 
            lblSettings7.Location = new Point(11, 206);
            lblSettings7.Name = "lblSettings7";
            lblSettings7.Size = new Size(220, 18);
            lblSettings7.TabIndex = 4;
            lblSettings7.Text = "Обороты шпинделя при резании:";
            // 
            // lblSettings3
            // 
            lblSettings3.Location = new Point(11, 82);
            lblSettings3.Name = "lblSettings3";
            lblSettings3.Size = new Size(220, 17);
            lblSettings3.TabIndex = 2;
            lblSettings3.Text = "Скорость поднятия сверла, мм/мин:";
            // 
            // lblSettings6
            // 
            lblSettings6.Location = new Point(11, 177);
            lblSettings6.Name = "lblSettings6";
            lblSettings6.Size = new Size(220, 17);
            lblSettings6.TabIndex = 3;
            lblSettings6.Text = "Глубина сверления, мм:";
            // 
            // panelTabPage
            // 
            panelTabPage.Controls.Add(groupBox1);
            panelTabPage.Controls.Add(label1);
            panelTabPage.Controls.Add(cbStencilSideHoles);
            panelTabPage.Controls.Add(rbStencil);
            panelTabPage.Controls.Add(btnCoordRight);
            panelTabPage.Controls.Add(btnCoordLeft);
            panelTabPage.Controls.Add(rbExPanel);
            panelTabPage.Controls.Add(rbNewPanel);
            panelTabPage.Controls.Add(lblPanelStartPos);
            panelTabPage.Controls.Add(btnCoordStart);
            panelTabPage.Controls.Add(lblMInstr1);
            panelTabPage.Location = new Point(4, 24);
            panelTabPage.Name = "panelTabPage";
            panelTabPage.Size = new Size(731, 790);
            panelTabPage.TabIndex = 7;
            panelTabPage.Text = "Заготовка";
            panelTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cbALExtended);
            groupBox1.Controls.Add(lblAutoLevelStatus);
            groupBox1.Controls.Add(nudALSafeHeightMm);
            groupBox1.Controls.Add(lblALSafeHeightMm);
            groupBox1.Controls.Add(nudPanelThicknessMm);
            groupBox1.Controls.Add(lblPanelThickness);
            groupBox1.Controls.Add(btnGrblProbe);
            groupBox1.Controls.Add(rbManualLevel);
            groupBox1.Controls.Add(rbAutoLevel);
            groupBox1.Location = new Point(7, 299);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(718, 152);
            groupBox1.TabIndex = 102;
            groupBox1.TabStop = false;
            groupBox1.Text = "Параметры";
            // 
            // cbALExtended
            // 
            cbALExtended.AutoSize = true;
            cbALExtended.Location = new Point(277, 69);
            cbALExtended.Name = "cbALExtended";
            cbALExtended.Size = new Size(155, 19);
            cbALExtended.TabIndex = 104;
            cbALExtended.Text = "Расширенная точность";
            cbALExtended.UseVisualStyleBackColor = true;
            cbALExtended.CheckedChanged += cbALExtended_CheckedChanged;
            // 
            // lblAutoLevelStatus
            // 
            lblAutoLevelStatus.Location = new Point(0, 104);
            lblAutoLevelStatus.Name = "lblAutoLevelStatus";
            lblAutoLevelStatus.Size = new Size(712, 45);
            lblAutoLevelStatus.TabIndex = 103;
            lblAutoLevelStatus.Text = "Автоуровень не измерен.";
            // 
            // nudALSafeHeightMm
            // 
            nudALSafeHeightMm.DecimalPlaces = 1;
            nudALSafeHeightMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudALSafeHeightMm.Location = new Point(514, 46);
            nudALSafeHeightMm.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            nudALSafeHeightMm.Minimum = new decimal(new int[] { 5, 0, 0, 65536 });
            nudALSafeHeightMm.Name = "nudALSafeHeightMm";
            nudALSafeHeightMm.Size = new Size(79, 23);
            nudALSafeHeightMm.TabIndex = 94;
            nudALSafeHeightMm.Value = new decimal(new int[] { 12, 0, 0, 65536 });
            nudALSafeHeightMm.ValueChanged += nudALSafeHeightMm_ValueChanged;
            // 
            // lblALSafeHeightMm
            // 
            lblALSafeHeightMm.Location = new Point(277, 48);
            lblALSafeHeightMm.Name = "lblALSafeHeightMm";
            lblALSafeHeightMm.Size = new Size(208, 18);
            lblALSafeHeightMm.TabIndex = 93;
            lblALSafeHeightMm.Text = "Безопасная высота над заготовкой:";
            // 
            // nudPanelThicknessMm
            // 
            nudPanelThicknessMm.DecimalPlaces = 1;
            nudPanelThicknessMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudPanelThicknessMm.Location = new Point(514, 20);
            nudPanelThicknessMm.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            nudPanelThicknessMm.Minimum = new decimal(new int[] { 5, 0, 0, 65536 });
            nudPanelThicknessMm.Name = "nudPanelThicknessMm";
            nudPanelThicknessMm.Size = new Size(79, 23);
            nudPanelThicknessMm.TabIndex = 92;
            nudPanelThicknessMm.Value = new decimal(new int[] { 17, 0, 0, 65536 });
            nudPanelThicknessMm.ValueChanged += nudPanelThicknessMm_ValueChanged;
            // 
            // lblPanelThickness
            // 
            lblPanelThickness.Location = new Point(277, 22);
            lblPanelThickness.Name = "lblPanelThickness";
            lblPanelThickness.Size = new Size(148, 21);
            lblPanelThickness.TabIndex = 91;
            lblPanelThickness.Text = "Толщина заготовки, мм:";
            // 
            // btnGrblProbe
            // 
            btnGrblProbe.Location = new Point(177, 20);
            btnGrblProbe.Name = "btnGrblProbe";
            btnGrblProbe.Size = new Size(94, 23);
            btnGrblProbe.TabIndex = 90;
            btnGrblProbe.Text = "GRBL Probe";
            btnGrblProbe.UseVisualStyleBackColor = true;
            btnGrblProbe.Click += btnGrblProbe_Click;
            // 
            // rbManualLevel
            // 
            rbManualLevel.AutoSize = true;
            rbManualLevel.Location = new Point(6, 45);
            rbManualLevel.Name = "rbManualLevel";
            rbManualLevel.Size = new Size(114, 19);
            rbManualLevel.TabIndex = 91;
            rbManualLevel.Text = "Ручной уровень";
            rbManualLevel.UseVisualStyleBackColor = true;
            rbManualLevel.CheckedChanged += rbManualLevel_CheckedChanged;
            // 
            // rbAutoLevel
            // 
            rbAutoLevel.AutoSize = true;
            rbAutoLevel.Checked = true;
            rbAutoLevel.Location = new Point(6, 22);
            rbAutoLevel.Name = "rbAutoLevel";
            rbAutoLevel.Size = new Size(165, 19);
            rbAutoLevel.TabIndex = 90;
            rbAutoLevel.TabStop = true;
            rbAutoLevel.Text = "Автоматический уровень";
            rbAutoLevel.UseVisualStyleBackColor = true;
            rbAutoLevel.CheckedChanged += rbAutoLevel_CheckedChanged;
            // 
            // label1
            // 
            label1.Location = new Point(7, 219);
            label1.Name = "label1";
            label1.Size = new Size(712, 77);
            label1.TabIndex = 101;
            label1.Text = resources.GetString("label1.Text");
            // 
            // cbStencilSideHoles
            // 
            cbStencilSideHoles.AutoSize = true;
            cbStencilSideHoles.Location = new Point(7, 112);
            cbStencilSideHoles.Name = "cbStencilSideHoles";
            cbStencilSideHoles.Size = new Size(208, 19);
            cbStencilSideHoles.TabIndex = 92;
            cbStencilSideHoles.Text = "Боковые отверстия на трафарете";
            cbStencilSideHoles.UseVisualStyleBackColor = true;
            cbStencilSideHoles.CheckedChanged += cbStencilSideHoles_CheckedChanged;
            // 
            // rbStencil
            // 
            rbStencil.AutoSize = true;
            rbStencil.Location = new Point(439, 83);
            rbStencil.Name = "rbStencil";
            rbStencil.Size = new Size(78, 19);
            rbStencil.TabIndex = 100;
            rbStencil.Text = "Трафарет";
            rbStencil.UseVisualStyleBackColor = true;
            rbStencil.CheckedChanged += rbStencil_CheckedChanged;
            // 
            // btnCoordRight
            // 
            btnCoordRight.Location = new Point(306, 137);
            btnCoordRight.Name = "btnCoordRight";
            btnCoordRight.Size = new Size(140, 23);
            btnCoordRight.TabIndex = 99;
            btnCoordRight.Text = "Правый верхний угол";
            btnCoordRight.UseVisualStyleBackColor = true;
            btnCoordRight.Click += btnCoordRight_Click;
            // 
            // btnCoordLeft
            // 
            btnCoordLeft.Location = new Point(157, 137);
            btnCoordLeft.Name = "btnCoordLeft";
            btnCoordLeft.Size = new Size(143, 23);
            btnCoordLeft.TabIndex = 98;
            btnCoordLeft.Text = "Левый верхний угол";
            btnCoordLeft.UseVisualStyleBackColor = true;
            btnCoordLeft.Click += btnCoordLeft_Click;
            // 
            // rbExPanel
            // 
            rbExPanel.AutoSize = true;
            rbExPanel.Location = new Point(150, 83);
            rbExPanel.Name = "rbExPanel";
            rbExPanel.Size = new Size(283, 19);
            rbExPanel.TabIndex = 97;
            rbExPanel.Text = "Существующая панель (пакет многослойный)";
            rbExPanel.UseVisualStyleBackColor = true;
            rbExPanel.CheckedChanged += rbExPanel_CheckedChanged;
            // 
            // rbNewPanel
            // 
            rbNewPanel.AutoSize = true;
            rbNewPanel.Checked = true;
            rbNewPanel.Location = new Point(6, 83);
            rbNewPanel.Name = "rbNewPanel";
            rbNewPanel.Size = new Size(101, 19);
            rbNewPanel.TabIndex = 96;
            rbNewPanel.TabStop = true;
            rbNewPanel.Text = "Новая панель";
            rbNewPanel.UseVisualStyleBackColor = true;
            rbNewPanel.CheckedChanged += rbNewPanel_CheckedChanged;
            // 
            // lblPanelStartPos
            // 
            lblPanelStartPos.Location = new Point(6, 163);
            lblPanelStartPos.Name = "lblPanelStartPos";
            lblPanelStartPos.Size = new Size(713, 45);
            lblPanelStartPos.TabIndex = 95;
            // 
            // btnCoordStart
            // 
            btnCoordStart.Location = new Point(6, 137);
            btnCoordStart.Name = "btnCoordStart";
            btnCoordStart.Size = new Size(145, 23);
            btnCoordStart.TabIndex = 93;
            btnCoordStart.Text = "Задать начало панели";
            btnCoordStart.UseVisualStyleBackColor = true;
            btnCoordStart.Click += btnCoordStart_Click;
            // 
            // lblMInstr1
            // 
            lblMInstr1.Location = new Point(6, 10);
            lblMInstr1.Name = "lblMInstr1";
            lblMInstr1.Size = new Size(722, 68);
            lblMInstr1.TabIndex = 91;
            lblMInstr1.Text = resources.GetString("lblMInstr1.Text");
            // 
            // machiningTabPage
            // 
            machiningTabPage.Controls.Add(lblHoleList);
            machiningTabPage.Controls.Add(btnDrillingPreviewAll);
            machiningTabPage.Controls.Add(btnDrillingPreview);
            machiningTabPage.Controls.Add(btnCutting);
            machiningTabPage.Controls.Add(pbCutting);
            machiningTabPage.Controls.Add(lvHoles);
            machiningTabPage.Controls.Add(lblMInstr2);
            machiningTabPage.Controls.Add(pbDrilling);
            machiningTabPage.Controls.Add(btnDrilling);
            machiningTabPage.Controls.Add(btnResetHoleStatus);
            machiningTabPage.Controls.Add(cbTools);
            machiningTabPage.Controls.Add(lblSelectTool);
            machiningTabPage.Location = new Point(4, 24);
            machiningTabPage.Name = "machiningTabPage";
            machiningTabPage.Padding = new Padding(3);
            machiningTabPage.Size = new Size(731, 790);
            machiningTabPage.TabIndex = 1;
            machiningTabPage.Text = "Пре-обработка";
            machiningTabPage.UseVisualStyleBackColor = true;
            // 
            // lblHoleList
            // 
            lblHoleList.Location = new Point(6, 3);
            lblHoleList.Name = "lblHoleList";
            lblHoleList.Size = new Size(719, 16);
            lblHoleList.TabIndex = 95;
            lblHoleList.Text = "1. Список отверстий:";
            // 
            // btnDrillingPreviewAll
            // 
            btnDrillingPreviewAll.Location = new Point(418, 276);
            btnDrillingPreviewAll.Name = "btnDrillingPreviewAll";
            btnDrillingPreviewAll.Size = new Size(113, 23);
            btnDrillingPreviewAll.TabIndex = 82;
            btnDrillingPreviewAll.Text = "Превью всех";
            btnDrillingPreviewAll.UseVisualStyleBackColor = true;
            btnDrillingPreviewAll.Click += btnDrillingPreviewAll_Click;
            // 
            // btnDrillingPreview
            // 
            btnDrillingPreview.Location = new Point(337, 276);
            btnDrillingPreview.Name = "btnDrillingPreview";
            btnDrillingPreview.Size = new Size(75, 23);
            btnDrillingPreview.TabIndex = 81;
            btnDrillingPreview.Text = "Превью";
            btnDrillingPreview.UseVisualStyleBackColor = true;
            btnDrillingPreview.Click += btnDrillingPreview_Click;
            // 
            // btnCutting
            // 
            btnCutting.Location = new Point(334, 372);
            btnCutting.Name = "btnCutting";
            btnCutting.Size = new Size(75, 23);
            btnCutting.TabIndex = 79;
            btnCutting.Text = "Начать";
            btnCutting.UseVisualStyleBackColor = true;
            btnCutting.Click += btnCutting_Click;
            // 
            // pbCutting
            // 
            pbCutting.Location = new Point(6, 348);
            pbCutting.Name = "pbCutting";
            pbCutting.Size = new Size(719, 18);
            pbCutting.TabIndex = 78;
            // 
            // lvHoles
            // 
            lvHoles.Location = new Point(6, 22);
            lvHoles.Name = "lvHoles";
            lvHoles.Size = new Size(719, 152);
            lvHoles.TabIndex = 69;
            lvHoles.UseCompatibleStateImageBehavior = false;
            lvHoles.MouseClick += lvHoles_MouseClick;
            // 
            // lblMInstr2
            // 
            lblMInstr2.Location = new Point(6, 313);
            lblMInstr2.Name = "lblMInstr2";
            lblMInstr2.Size = new Size(719, 32);
            lblMInstr2.TabIndex = 70;
            lblMInstr2.Text = "2. Вырез панели из листа. Установите на шпиндель фрезу, выставьте Z 1 мм над листом, нажмите начать.\r\nТут автоуровень не применяется.";
            // 
            // pbDrilling
            // 
            pbDrilling.Location = new Point(6, 246);
            pbDrilling.Name = "pbDrilling";
            pbDrilling.Size = new Size(719, 23);
            pbDrilling.TabIndex = 71;
            // 
            // btnDrilling
            // 
            btnDrilling.Location = new Point(6, 275);
            btnDrilling.Name = "btnDrilling";
            btnDrilling.Size = new Size(138, 23);
            btnDrilling.TabIndex = 72;
            btnDrilling.Text = "Начать/продолжить";
            btnDrilling.UseVisualStyleBackColor = true;
            btnDrilling.Click += btnDrilling_Click;
            // 
            // btnResetHoleStatus
            // 
            btnResetHoleStatus.Location = new Point(150, 275);
            btnResetHoleStatus.Name = "btnResetHoleStatus";
            btnResetHoleStatus.Size = new Size(181, 23);
            btnResetHoleStatus.TabIndex = 73;
            btnResetHoleStatus.Text = "Сбросить статус отверстий";
            btnResetHoleStatus.UseVisualStyleBackColor = true;
            btnResetHoleStatus.Click += btnResetHoleStatus_Click;
            // 
            // cbTools
            // 
            cbTools.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTools.FormattingEnabled = true;
            cbTools.Location = new Point(6, 217);
            cbTools.Name = "cbTools";
            cbTools.Size = new Size(719, 23);
            cbTools.TabIndex = 74;
            cbTools.SelectedIndexChanged += cbTools_SelectedIndexChanged;
            // 
            // lblSelectTool
            // 
            lblSelectTool.Location = new Point(6, 177);
            lblSelectTool.Name = "lblSelectTool";
            lblSelectTool.Size = new Size(719, 37);
            lblSelectTool.TabIndex = 75;
            lblSelectTool.Text = "Выберите инструмент, поставьте его на шпиндель, выставьте Z в положение 1 мм над листом текстолита и нажмите начать. Повторите цикл для каждого.";
            // 
            // depanelizeTabPage
            // 
            depanelizeTabPage.Controls.Add(lblDCorners);
            depanelizeTabPage.Controls.Add(btnDRightTop);
            depanelizeTabPage.Controls.Add(btnDLeftTop);
            depanelizeTabPage.Controls.Add(btnDepPreview);
            depanelizeTabPage.Controls.Add(btnDepanelize);
            depanelizeTabPage.Controls.Add(pbDepanelize);
            depanelizeTabPage.Controls.Add(lblDInstr1);
            depanelizeTabPage.Location = new Point(4, 24);
            depanelizeTabPage.Name = "depanelizeTabPage";
            depanelizeTabPage.Size = new Size(731, 790);
            depanelizeTabPage.TabIndex = 3;
            depanelizeTabPage.Text = "Депанелизация";
            depanelizeTabPage.UseVisualStyleBackColor = true;
            // 
            // lblDCorners
            // 
            lblDCorners.Location = new Point(4, 112);
            lblDCorners.Name = "lblDCorners";
            lblDCorners.Size = new Size(724, 47);
            lblDCorners.TabIndex = 86;
            // 
            // btnDRightTop
            // 
            btnDRightTop.Location = new Point(152, 86);
            btnDRightTop.Name = "btnDRightTop";
            btnDRightTop.Size = new Size(142, 23);
            btnDRightTop.TabIndex = 85;
            btnDRightTop.Text = "Правый верхний угол";
            btnDRightTop.UseVisualStyleBackColor = true;
            btnDRightTop.Click += btnDRightTop_Click;
            // 
            // btnDLeftTop
            // 
            btnDLeftTop.Location = new Point(4, 86);
            btnDLeftTop.Name = "btnDLeftTop";
            btnDLeftTop.Size = new Size(142, 23);
            btnDLeftTop.TabIndex = 84;
            btnDLeftTop.Text = "Левый верхний угол";
            btnDLeftTop.UseVisualStyleBackColor = true;
            btnDLeftTop.Click += btnDLeftTop_Click;
            // 
            // btnDepPreview
            // 
            btnDepPreview.Location = new Point(372, 191);
            btnDepPreview.Name = "btnDepPreview";
            btnDepPreview.Size = new Size(75, 23);
            btnDepPreview.TabIndex = 83;
            btnDepPreview.Text = "Превью";
            btnDepPreview.UseVisualStyleBackColor = true;
            btnDepPreview.Click += btnDepPreview_Click;
            // 
            // btnDepanelize
            // 
            btnDepanelize.Location = new Point(291, 191);
            btnDepanelize.Name = "btnDepanelize";
            btnDepanelize.Size = new Size(75, 23);
            btnDepanelize.TabIndex = 82;
            btnDepanelize.Text = "Начать";
            btnDepanelize.UseVisualStyleBackColor = true;
            btnDepanelize.Click += btnDepanelize_Click;
            // 
            // pbDepanelize
            // 
            pbDepanelize.Location = new Point(3, 162);
            pbDepanelize.Name = "pbDepanelize";
            pbDepanelize.Size = new Size(725, 23);
            pbDepanelize.TabIndex = 81;
            // 
            // lblDInstr1
            // 
            lblDInstr1.Location = new Point(4, 4);
            lblDInstr1.Name = "lblDInstr1";
            lblDInstr1.Size = new Size(724, 79);
            lblDInstr1.TabIndex = 80;
            lblDInstr1.Text = resources.GetString("lblDInstr1.Text");
            // 
            // favLocTabPage
            // 
            favLocTabPage.Controls.Add(btnMoveToLoc4);
            favLocTabPage.Controls.Add(nudLoc4YMm);
            favLocTabPage.Controls.Add(nudLoc4XMm);
            favLocTabPage.Controls.Add(lblFavLoc4);
            favLocTabPage.Controls.Add(btnMoveToLoc3);
            favLocTabPage.Controls.Add(nudLoc3YMm);
            favLocTabPage.Controls.Add(nudLoc3XMm);
            favLocTabPage.Controls.Add(lblFavLoc3);
            favLocTabPage.Controls.Add(btnMoveToLoc2);
            favLocTabPage.Controls.Add(nudLoc2YMm);
            favLocTabPage.Controls.Add(nudLoc2XMm);
            favLocTabPage.Controls.Add(lblFavLoc2);
            favLocTabPage.Controls.Add(btnMoveToLoc1);
            favLocTabPage.Controls.Add(nudLoc1YMm);
            favLocTabPage.Controls.Add(nudLoc1XMm);
            favLocTabPage.Controls.Add(lblFavLoc1);
            favLocTabPage.Location = new Point(4, 24);
            favLocTabPage.Name = "favLocTabPage";
            favLocTabPage.Size = new Size(731, 790);
            favLocTabPage.TabIndex = 6;
            favLocTabPage.Text = "Избранное";
            favLocTabPage.UseVisualStyleBackColor = true;
            // 
            // btnMoveToLoc4
            // 
            btnMoveToLoc4.Location = new Point(246, 101);
            btnMoveToLoc4.Name = "btnMoveToLoc4";
            btnMoveToLoc4.Size = new Size(130, 23);
            btnMoveToLoc4.TabIndex = 31;
            btnMoveToLoc4.Tag = "4";
            btnMoveToLoc4.Text = "Переместиться сюда";
            btnMoveToLoc4.UseVisualStyleBackColor = true;
            btnMoveToLoc4.Click += btnMoveToFavLoc_Click;
            // 
            // nudLoc4YMm
            // 
            nudLoc4YMm.DecimalPlaces = 2;
            nudLoc4YMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudLoc4YMm.Location = new Point(159, 101);
            nudLoc4YMm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudLoc4YMm.Name = "nudLoc4YMm";
            nudLoc4YMm.Size = new Size(81, 23);
            nudLoc4YMm.TabIndex = 30;
            nudLoc4YMm.ValueChanged += nudFavLock_ValueChanged;
            // 
            // nudLoc4XMm
            // 
            nudLoc4XMm.DecimalPlaces = 2;
            nudLoc4XMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudLoc4XMm.Location = new Point(72, 101);
            nudLoc4XMm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudLoc4XMm.Name = "nudLoc4XMm";
            nudLoc4XMm.Size = new Size(81, 23);
            nudLoc4XMm.TabIndex = 29;
            nudLoc4XMm.ValueChanged += nudFavLock_ValueChanged;
            // 
            // lblFavLoc4
            // 
            lblFavLoc4.AutoSize = true;
            lblFavLoc4.Location = new Point(12, 103);
            lblFavLoc4.Name = "lblFavLoc4";
            lblFavLoc4.Size = new Size(54, 15);
            lblFavLoc4.TabIndex = 28;
            lblFavLoc4.Text = "Место 4:";
            // 
            // btnMoveToLoc3
            // 
            btnMoveToLoc3.Location = new Point(246, 71);
            btnMoveToLoc3.Name = "btnMoveToLoc3";
            btnMoveToLoc3.Size = new Size(130, 23);
            btnMoveToLoc3.TabIndex = 27;
            btnMoveToLoc3.Tag = "3";
            btnMoveToLoc3.Text = "Переместиться сюда";
            btnMoveToLoc3.UseVisualStyleBackColor = true;
            btnMoveToLoc3.Click += btnMoveToFavLoc_Click;
            // 
            // nudLoc3YMm
            // 
            nudLoc3YMm.DecimalPlaces = 2;
            nudLoc3YMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudLoc3YMm.Location = new Point(159, 71);
            nudLoc3YMm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudLoc3YMm.Name = "nudLoc3YMm";
            nudLoc3YMm.Size = new Size(81, 23);
            nudLoc3YMm.TabIndex = 26;
            nudLoc3YMm.ValueChanged += nudFavLock_ValueChanged;
            // 
            // nudLoc3XMm
            // 
            nudLoc3XMm.DecimalPlaces = 2;
            nudLoc3XMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudLoc3XMm.Location = new Point(72, 71);
            nudLoc3XMm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudLoc3XMm.Name = "nudLoc3XMm";
            nudLoc3XMm.Size = new Size(81, 23);
            nudLoc3XMm.TabIndex = 25;
            nudLoc3XMm.ValueChanged += nudFavLock_ValueChanged;
            // 
            // lblFavLoc3
            // 
            lblFavLoc3.AutoSize = true;
            lblFavLoc3.Location = new Point(12, 73);
            lblFavLoc3.Name = "lblFavLoc3";
            lblFavLoc3.Size = new Size(54, 15);
            lblFavLoc3.TabIndex = 24;
            lblFavLoc3.Text = "Место 3:";
            // 
            // btnMoveToLoc2
            // 
            btnMoveToLoc2.Location = new Point(246, 40);
            btnMoveToLoc2.Name = "btnMoveToLoc2";
            btnMoveToLoc2.Size = new Size(130, 23);
            btnMoveToLoc2.TabIndex = 23;
            btnMoveToLoc2.Tag = "2";
            btnMoveToLoc2.Text = "Переместиться сюда";
            btnMoveToLoc2.UseVisualStyleBackColor = true;
            btnMoveToLoc2.Click += btnMoveToFavLoc_Click;
            // 
            // nudLoc2YMm
            // 
            nudLoc2YMm.DecimalPlaces = 2;
            nudLoc2YMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudLoc2YMm.Location = new Point(159, 40);
            nudLoc2YMm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudLoc2YMm.Name = "nudLoc2YMm";
            nudLoc2YMm.Size = new Size(81, 23);
            nudLoc2YMm.TabIndex = 22;
            nudLoc2YMm.ValueChanged += nudFavLock_ValueChanged;
            // 
            // nudLoc2XMm
            // 
            nudLoc2XMm.DecimalPlaces = 2;
            nudLoc2XMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudLoc2XMm.Location = new Point(72, 40);
            nudLoc2XMm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudLoc2XMm.Name = "nudLoc2XMm";
            nudLoc2XMm.Size = new Size(81, 23);
            nudLoc2XMm.TabIndex = 21;
            nudLoc2XMm.ValueChanged += nudFavLock_ValueChanged;
            // 
            // lblFavLoc2
            // 
            lblFavLoc2.AutoSize = true;
            lblFavLoc2.Location = new Point(12, 42);
            lblFavLoc2.Name = "lblFavLoc2";
            lblFavLoc2.Size = new Size(54, 15);
            lblFavLoc2.TabIndex = 20;
            lblFavLoc2.Text = "Место 2:";
            // 
            // btnMoveToLoc1
            // 
            btnMoveToLoc1.Location = new Point(246, 9);
            btnMoveToLoc1.Name = "btnMoveToLoc1";
            btnMoveToLoc1.Size = new Size(130, 23);
            btnMoveToLoc1.TabIndex = 19;
            btnMoveToLoc1.Tag = "1";
            btnMoveToLoc1.Text = "Переместиться сюда";
            btnMoveToLoc1.UseVisualStyleBackColor = true;
            btnMoveToLoc1.Click += btnMoveToFavLoc_Click;
            // 
            // nudLoc1YMm
            // 
            nudLoc1YMm.DecimalPlaces = 2;
            nudLoc1YMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudLoc1YMm.Location = new Point(159, 9);
            nudLoc1YMm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudLoc1YMm.Name = "nudLoc1YMm";
            nudLoc1YMm.Size = new Size(81, 23);
            nudLoc1YMm.TabIndex = 18;
            nudLoc1YMm.ValueChanged += nudFavLock_ValueChanged;
            // 
            // nudLoc1XMm
            // 
            nudLoc1XMm.DecimalPlaces = 2;
            nudLoc1XMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudLoc1XMm.Location = new Point(72, 9);
            nudLoc1XMm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudLoc1XMm.Name = "nudLoc1XMm";
            nudLoc1XMm.Size = new Size(81, 23);
            nudLoc1XMm.TabIndex = 17;
            nudLoc1XMm.ValueChanged += nudFavLock_ValueChanged;
            // 
            // lblFavLoc1
            // 
            lblFavLoc1.AutoSize = true;
            lblFavLoc1.Location = new Point(12, 11);
            lblFavLoc1.Name = "lblFavLoc1";
            lblFavLoc1.Size = new Size(54, 15);
            lblFavLoc1.TabIndex = 16;
            lblFavLoc1.Text = "Место 1:";
            // 
            // holePopupMenu
            // 
            holePopupMenu.ImageScalingSize = new Size(20, 20);
            holePopupMenu.Items.AddRange(new ToolStripItem[] { showHoleItem, resetHoleItem });
            holePopupMenu.Name = "holePopupMenu";
            holePopupMenu.Size = new Size(128, 48);
            // 
            // showHoleItem
            // 
            showHoleItem.Name = "showHoleItem";
            showHoleItem.Size = new Size(127, 22);
            showHoleItem.Text = "Показать";
            showHoleItem.Click += showHoleItem_Click;
            // 
            // resetHoleItem
            // 
            resetHoleItem.Name = "resetHoleItem";
            resetHoleItem.Size = new Size(127, 22);
            resetHoleItem.Text = "Сбросить";
            resetHoleItem.Click += resetHoleItem_Click;
            // 
            // camWatchdogTimer
            // 
            camWatchdogTimer.Enabled = true;
            camWatchdogTimer.Interval = 5000;
            camWatchdogTimer.Tick += camWatchdogTimer_Tick;
            // 
            // tbCamFocus
            // 
            tbCamFocus.Location = new Point(436, 451);
            tbCamFocus.Maximum = 1023;
            tbCamFocus.Name = "tbCamFocus";
            tbCamFocus.Size = new Size(211, 45);
            tbCamFocus.TabIndex = 66;
            tbCamFocus.TickFrequency = 0;
            tbCamFocus.TickStyle = TickStyle.Both;
            tbCamFocus.Value = 1023;
            tbCamFocus.ValueChanged += tbCamFocus_ValueChanged;
            // 
            // cbImageFlipV
            // 
            cbImageFlipV.AutoSize = true;
            cbImageFlipV.Location = new Point(181, 356);
            cbImageFlipV.Name = "cbImageFlipV";
            cbImageFlipV.Size = new Size(153, 19);
            cbImageFlipV.TabIndex = 69;
            cbImageFlipV.Text = "Отразить по вертикали";
            cbImageFlipV.UseVisualStyleBackColor = true;
            cbImageFlipV.CheckedChanged += cbImageFlipV_CheckedChanged;
            // 
            // cbImageFlipH
            // 
            cbImageFlipH.AutoSize = true;
            cbImageFlipH.Location = new Point(6, 356);
            cbImageFlipH.Name = "cbImageFlipH";
            cbImageFlipH.Size = new Size(166, 19);
            cbImageFlipH.TabIndex = 68;
            cbImageFlipH.Text = "Отразить по горизонтали";
            cbImageFlipH.UseVisualStyleBackColor = true;
            cbImageFlipH.CheckedChanged += cbImageFlipH_CheckedChanged;
            // 
            // CNCForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1404, 825);
            Controls.Add(cbImageFlipV);
            Controls.Add(cbImageFlipH);
            Controls.Add(tbCamFocus);
            Controls.Add(mainTabControl);
            Controls.Add(btnMoveCamToSpindle);
            Controls.Add(btnMoveSpindleToCam);
            Controls.Add(tbSpindleSpeed);
            Controls.Add(btnSpindleActivate);
            Controls.Add(btnZNeg);
            Controls.Add(btnZPos);
            Controls.Add(lblMotionCompens);
            Controls.Add(lblPosition);
            Controls.Add(lblMoveAmount);
            Controls.Add(tbMoveAmount);
            Controls.Add(btnYNeg);
            Controls.Add(btnYPos);
            Controls.Add(btnXPos);
            Controls.Add(btnXNeg);
            Controls.Add(tbZoom);
            Controls.Add(pnMotionIndicator);
            Controls.Add(btnCncConnect);
            Controls.Add(pbCncCamPreview);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "CNCForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ЧПУ обработка";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)tbSpindleSpeed).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbMoveAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbZoom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbCncCamPreview).EndInit();
            mainTabControl.ResumeLayout(false);
            primarySetupTabPage.ResumeLayout(false);
            gbZDir.ResumeLayout(false);
            gbZDir.PerformLayout();
            gbYDir.ResumeLayout(false);
            gbYDir.PerformLayout();
            gbXDir.ResumeLayout(false);
            gbXDir.PerformLayout();
            plSim.ResumeLayout(false);
            plSim.PerformLayout();
            calibTabPage.ResumeLayout(false);
            calibTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudSkewAngleRad).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudStepsPerMmY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudStepsPerMmX).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudBacklashMmY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudBacklashMmX).EndInit();
            settingsPage.ResumeLayout(false);
            gbCutting.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nudIGapMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudICutMm).EndInit();
            gbMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nudZUpRateSmallMmPerMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudZDownRateSmallMmPerMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCuttingRateMmPerMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCuttingRpm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDrillDepthMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudZUpRateBigMmPerMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudZDownRateBigMmPerMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDrillingRpm).EndInit();
            panelTabPage.ResumeLayout(false);
            panelTabPage.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudALSafeHeightMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPanelThicknessMm).EndInit();
            machiningTabPage.ResumeLayout(false);
            depanelizeTabPage.ResumeLayout(false);
            favLocTabPage.ResumeLayout(false);
            favLocTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudLoc4YMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc4XMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc3YMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc3XMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc2YMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc2XMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc1YMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLoc1XMm).EndInit();
            holePopupMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tbCamFocus).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pbCncCamPreview;
        private Button btnCncConnect;
        private Label lblLensCalProgress;
        private Panel pnMotionIndicator;
        private TrackBar tbZoom;
        private Button btnXNeg;
        private Button btnXPos;
        private Button btnYPos;
        private Button btnYNeg;
        private TrackBar tbMoveAmount;
        private Label lblMoveAmount;
        private Label lblPosition;
        private Label lblMotionCompens;
        private Button btnZNeg;
        private Button btnZPos;
        private Button btnMoveSpindleToCam;
        private TrackBar tbSpindleSpeed;
        private Button btnSpindleActivate;
        private Button btnMoveCamToSpindle;
        private TabControl mainTabControl;
        private TabPage calibTabPage;
        private Label lblCamSpindleOffset;
        private Button btnTakeLocCamera;
        private Button btnTakeLocSpindle;
        private Label lblInstr11;
        private NumericUpDown nudBacklashMmY;
        private NumericUpDown nudBacklashMmX;
        private Label lblBacklashMmY;
        private Label lblBacklashMmX;
        private Label lblInstr10;
        private Button btnApplyMotion;
        private Label lblInstr9;
        private Label lblInstr1;
        private TabPage machiningTabPage;
        private ListView lvHoles;
        private Label lblMInstr2;
        private ProgressBar pbDrilling;
        private Button btnDrilling;
        private Button btnResetHoleStatus;
        private ComboBox cbTools;
        private Label lblSelectTool;
        private ProgressBar pbCutting;
        private Button btnCutting;
        private TabPage depanelizeTabPage;
        private TabPage settingsPage;
        private Label lblSettings1;
        private Label lblSettings2;
        private Label lblSettings3;
        private Label lblSettings6;
        private Label lblSettings7;
        private Label lblSettings8;
        private GroupBox gbMain;
        private NumericUpDown nudDrillingRpm;
        private NumericUpDown nudDrillDepthMm;
        private NumericUpDown nudZUpRateBigMmPerMin;
        private NumericUpDown nudZDownRateBigMmPerMin;
        private NumericUpDown nudCuttingRpm;
        private NumericUpDown nudCuttingRateMmPerMin;
        private ContextMenuStrip holePopupMenu;
        private ToolStripMenuItem showHoleItem;
        private ToolStripMenuItem resetHoleItem;
        private TabPage primarySetupTabPage;
        private Label lblPInstr1;
        private Panel plSim;
        private RadioButton rbXYTopLeft;
        private RadioButton rbXYBottomRight;
        private RadioButton rbXYTopRight;
        private RadioButton rbXYBottomLeft;
        private Label lblPInstr2;
        private GroupBox gbXDir;
        private RadioButton rbXRight;
        private RadioButton rbXLeft;
        private GroupBox gbYDir;
        private RadioButton rbYUp;
        private RadioButton rbYDown;
        private GroupBox gbZDir;
        private RadioButton rbZTowardsWorkpiece;
        private RadioButton rbZAwayFromWorkpiece;
        private Label lblPInstr3;
        private Label lblStepsPerMmXS;
        private Label lblStepsPerMmYS;
        private NumericUpDown nudStepsPerMmX;
        private NumericUpDown nudStepsPerMmY;
        private NumericUpDown nudSkewAngleRad;
        private Label lblSkewValX;
        private Button btnDrillingPreview;
        private Button btnDepanelize;
        private ProgressBar pbDepanelize;
        private Label lblDInstr1;
        private Button btnDepPreview;
        private Button btnDLeftTop;
        private Button btnDRightTop;
        private Label lblDCorners;
        private GroupBox gbCutting;
        private NumericUpDown nudIGapMm;
        private NumericUpDown nudICutMm;
        private Label lblSettings10;
        private Label lblSettings11;
        private Button btnDrillingPreviewAll;
        private System.Windows.Forms.Timer camWatchdogTimer;
        private TrackBar tbCamFocus;
        private CheckBox cbImageFlipV;
        private CheckBox cbImageFlipH;
        private Label lblInstr12;
        private Button btnTestCalib;
        private Button btnCalibPanelTopLeft;
        private Button btnCalibPanelBottomRight;
        private Button btnCalibPanelBottomLeft;
        private Button btnCalibPanelTopRight;
        private Button btnCalibPanelCalculate;
        private NumericUpDown nudZUpRateSmallMmPerMin;
        private NumericUpDown nudZDownRateSmallMmPerMin;
        private Label lblSettings4;
        private Label lblSettings5;
        private TabPage favLocTabPage;
        private Button btnMoveToLoc4;
        private NumericUpDown nudLoc4YMm;
        private NumericUpDown nudLoc4XMm;
        private Label lblFavLoc4;
        private Button btnMoveToLoc3;
        private NumericUpDown nudLoc3YMm;
        private NumericUpDown nudLoc3XMm;
        private Label lblFavLoc3;
        private Button btnMoveToLoc2;
        private NumericUpDown nudLoc2YMm;
        private NumericUpDown nudLoc2XMm;
        private Label lblFavLoc2;
        private Button btnMoveToLoc1;
        private NumericUpDown nudLoc1YMm;
        private NumericUpDown nudLoc1XMm;
        private Label lblFavLoc1;
        private TabPage panelTabPage;
        private Label lblAutoLevelStatus;
        private GroupBox groupBox1;
        private NumericUpDown nudALSafeHeightMm;
        private Label lblALSafeHeightMm;
        private NumericUpDown nudPanelThicknessMm;
        private Label lblPanelThickness;
        private Button btnGrblProbe;
        private RadioButton rbManualLevel;
        private RadioButton rbAutoLevel;
        private Label label1;
        private CheckBox cbStencilSideHoles;
        private RadioButton rbStencil;
        private Button btnCoordRight;
        private Button btnCoordLeft;
        private RadioButton rbExPanel;
        private RadioButton rbNewPanel;
        private Label lblPanelStartPos;
        private Button btnCoordStart;
        private Label lblMInstr1;
        private Label lblHoleList;
        private CheckBox cbALExtended;
    }
}
