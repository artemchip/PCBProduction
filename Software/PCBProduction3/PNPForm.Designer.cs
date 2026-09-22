namespace PCBProduction3
{
    partial class PNPForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PNPForm));
            nudFOVYTo = new NumericUpDown();
            nudFOVYFrom = new NumericUpDown();
            lblFOVY = new Label();
            nudFOVXTo = new NumericUpDown();
            nudFOVXFrom = new NumericUpDown();
            lblFOVX = new Label();
            btnCamNext = new Button();
            btnCamPrev = new Button();
            cbShowRect = new CheckBox();
            lblPxPerMm = new Label();
            nudImagePxPerMm = new NumericUpDown();
            lblMm = new Label();
            lblXRect = new Label();
            nudRectHeightMm = new NumericUpDown();
            nudRectWidthMm = new NumericUpDown();
            cbImageShowShapes = new CheckBox();
            cbImageShowAngleGrid = new CheckBox();
            cbImageFlipV = new CheckBox();
            nudImageAngleDeg = new NumericUpDown();
            cbImageFlipH = new CheckBox();
            lblCalibGridAngleDeg = new Label();
            btnSpindle = new Button();
            btnZNeg = new Button();
            btnZPos = new Button();
            lblStatus = new Label();
            lblMoveAmount = new Label();
            tbMoveAmount = new TrackBar();
            btnYNeg = new Button();
            btnYPos = new Button();
            btnXPos = new Button();
            btnXNeg = new Button();
            tbZoom = new TrackBar();
            pnMotionIndicator = new Panel();
            btnMachineConnect = new Button();
            pbCamPreview = new PictureBox();
            panel1 = new Panel();
            pnpTab = new TabPage();
            btnLarge = new Button();
            btnUFeeder5 = new Button();
            btnUFeeder4 = new Button();
            btnUFeeder3 = new Button();
            btnUFeeder2 = new Button();
            btnUFeeder1 = new Button();
            btnSFeeder5 = new Button();
            btnSFeeder4 = new Button();
            btnSFeeder3 = new Button();
            btnSFeeder2 = new Button();
            btnSFeeder1 = new Button();
            lblSelComp = new Label();
            gbProcess = new GroupBox();
            cbDemoPlaced = new CheckBox();
            nudLyingDeg = new NumericUpDown();
            rbPNP = new RadioButton();
            rbPanelSpecify = new RadioButton();
            btnRun = new Button();
            lvComponents = new ListView();
            rbSideBottom = new RadioButton();
            rbSideTop = new RadioButton();
            lblPInstr1 = new Label();
            gbDrillUserInput = new GroupBox();
            btnDInstr6 = new Button();
            btnDInstr5 = new Button();
            btnDInstr4 = new Button();
            btnDInstr3 = new Button();
            lblDInstr = new Label();
            btnDInstr2 = new Button();
            btnDInstr1 = new Button();
            calibTab = new TabPage();
            cbCAxisPositive = new CheckBox();
            lblAddAngle = new Label();
            nudAddAngle = new NumericUpDown();
            lblCalibInstr5 = new Label();
            nudStepsFullTurn = new NumericUpDown();
            lblStepsPerTurn = new Label();
            lblSteps = new Label();
            nudCCalSteps = new NumericUpDown();
            lblCalibInstr4 = new Label();
            rbCalCAxis = new RadioButton();
            rbCalCamOffset = new RadioButton();
            rbCalSteps = new RadioButton();
            lblCalibInstr3 = new Label();
            lblCalibInstr6 = new Label();
            btnContinueAfterCalib = new Button();
            gbCalibUserInput = new GroupBox();
            lblCInstr = new Label();
            btnInstr2 = new Button();
            btnInstr1 = new Button();
            btnCalib = new Button();
            lblCalibInstr2 = new Label();
            btnPrintCalib = new Button();
            lblCalibInstr1 = new Label();
            noConnectionTab = new TabPage();
            lblAlarmStatus = new Label();
            procTabControl = new TabControl();
            tbMachineIP = new TextBox();
            lblMachineIP = new Label();
            ((System.ComponentModel.ISupportInitialize)nudFOVYTo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFOVYFrom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFOVXTo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFOVXFrom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudImagePxPerMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRectHeightMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRectWidthMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudImageAngleDeg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbMoveAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbZoom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbCamPreview).BeginInit();
            pnpTab.SuspendLayout();
            gbProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudLyingDeg).BeginInit();
            gbDrillUserInput.SuspendLayout();
            calibTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudAddAngle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudStepsFullTurn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCCalSteps).BeginInit();
            gbCalibUserInput.SuspendLayout();
            noConnectionTab.SuspendLayout();
            procTabControl.SuspendLayout();
            SuspendLayout();
            // 
            // nudFOVYTo
            // 
            nudFOVYTo.Location = new Point(195, 454);
            nudFOVYTo.Name = "nudFOVYTo";
            nudFOVYTo.Size = new Size(66, 23);
            nudFOVYTo.TabIndex = 122;
            nudFOVYTo.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudFOVYTo.ValueChanged += nudFOVYTo_ValueChanged;
            // 
            // nudFOVYFrom
            // 
            nudFOVYFrom.Location = new Point(123, 454);
            nudFOVYFrom.Name = "nudFOVYFrom";
            nudFOVYFrom.Size = new Size(66, 23);
            nudFOVYFrom.TabIndex = 121;
            nudFOVYFrom.ValueChanged += nudFOVYFrom_ValueChanged;
            // 
            // lblFOVY
            // 
            lblFOVY.AutoSize = true;
            lblFOVY.Location = new Point(11, 456);
            lblFOVY.Name = "lblFOVY";
            lblFOVY.Size = new Size(103, 15);
            lblFOVY.TabIndex = 120;
            lblFOVY.Text = "Поле зрения, Y%:";
            // 
            // nudFOVXTo
            // 
            nudFOVXTo.Location = new Point(195, 423);
            nudFOVXTo.Name = "nudFOVXTo";
            nudFOVXTo.Size = new Size(66, 23);
            nudFOVXTo.TabIndex = 119;
            nudFOVXTo.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudFOVXTo.ValueChanged += nudFOVXTo_ValueChanged;
            // 
            // nudFOVXFrom
            // 
            nudFOVXFrom.Location = new Point(123, 423);
            nudFOVXFrom.Name = "nudFOVXFrom";
            nudFOVXFrom.Size = new Size(66, 23);
            nudFOVXFrom.TabIndex = 118;
            nudFOVXFrom.ValueChanged += nudFOVXFrom_ValueChanged;
            // 
            // lblFOVX
            // 
            lblFOVX.AutoSize = true;
            lblFOVX.Location = new Point(11, 425);
            lblFOVX.Name = "lblFOVX";
            lblFOVX.Size = new Size(103, 15);
            lblFOVX.TabIndex = 117;
            lblFOVX.Text = "Поле зрения, X%:";
            // 
            // btnCamNext
            // 
            btnCamNext.Location = new Point(619, 342);
            btnCamNext.Name = "btnCamNext";
            btnCamNext.Size = new Size(34, 23);
            btnCamNext.TabIndex = 116;
            btnCamNext.Text = ">";
            btnCamNext.UseVisualStyleBackColor = true;
            btnCamNext.Click += btnCamNext_Click;
            // 
            // btnCamPrev
            // 
            btnCamPrev.Location = new Point(579, 342);
            btnCamPrev.Name = "btnCamPrev";
            btnCamPrev.Size = new Size(34, 23);
            btnCamPrev.TabIndex = 115;
            btnCamPrev.Text = "<";
            btnCamPrev.UseVisualStyleBackColor = true;
            btnCamPrev.Click += btnCamPrev_Click;
            // 
            // cbShowRect
            // 
            cbShowRect.AutoSize = true;
            cbShowRect.Location = new Point(11, 393);
            cbShowRect.Name = "cbShowRect";
            cbShowRect.Size = new Size(115, 19);
            cbShowRect.TabIndex = 114;
            cbShowRect.Text = "Прямоугольник";
            cbShowRect.UseVisualStyleBackColor = true;
            cbShowRect.CheckedChanged += cbShowRect_CheckedChanged;
            // 
            // lblPxPerMm
            // 
            lblPxPerMm.AutoSize = true;
            lblPxPerMm.Location = new Point(419, 393);
            lblPxPerMm.Name = "lblPxPerMm";
            lblPxPerMm.Size = new Size(96, 15);
            lblPxPerMm.TabIndex = 113;
            lblPxPerMm.Text = "пикселей на мм";
            // 
            // nudImagePxPerMm
            // 
            nudImagePxPerMm.DecimalPlaces = 2;
            nudImagePxPerMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudImagePxPerMm.Location = new Point(330, 391);
            nudImagePxPerMm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudImagePxPerMm.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            nudImagePxPerMm.Name = "nudImagePxPerMm";
            nudImagePxPerMm.Size = new Size(83, 23);
            nudImagePxPerMm.TabIndex = 112;
            nudImagePxPerMm.Value = new decimal(new int[] { 10, 0, 0, 0 });
            nudImagePxPerMm.ValueChanged += nudImagePxPerMm_ValueChanged;
            // 
            // lblMm
            // 
            lblMm.AutoSize = true;
            lblMm.Location = new Point(299, 393);
            lblMm.Name = "lblMm";
            lblMm.Size = new Size(25, 15);
            lblMm.TabIndex = 111;
            lblMm.Text = "мм";
            // 
            // lblXRect
            // 
            lblXRect.AutoSize = true;
            lblXRect.Location = new Point(208, 393);
            lblXRect.Name = "lblXRect";
            lblXRect.Size = new Size(14, 15);
            lblXRect.TabIndex = 110;
            lblXRect.Text = "X";
            // 
            // nudRectHeightMm
            // 
            nudRectHeightMm.DecimalPlaces = 2;
            nudRectHeightMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudRectHeightMm.Location = new Point(227, 391);
            nudRectHeightMm.Name = "nudRectHeightMm";
            nudRectHeightMm.Size = new Size(66, 23);
            nudRectHeightMm.TabIndex = 109;
            nudRectHeightMm.ValueChanged += nudRectHeightMm_ValueChanged;
            // 
            // nudRectWidthMm
            // 
            nudRectWidthMm.DecimalPlaces = 2;
            nudRectWidthMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudRectWidthMm.Location = new Point(132, 391);
            nudRectWidthMm.Name = "nudRectWidthMm";
            nudRectWidthMm.Size = new Size(70, 23);
            nudRectWidthMm.TabIndex = 108;
            nudRectWidthMm.ValueChanged += nudRectWidthMm_ValueChanged;
            // 
            // cbImageShowShapes
            // 
            cbImageShowShapes.AutoSize = true;
            cbImageShowShapes.Location = new Point(184, 342);
            cbImageShowShapes.Name = "cbImageShowShapes";
            cbImageShowShapes.Size = new Size(122, 19);
            cbImageShowShapes.TabIndex = 107;
            cbImageShowShapes.Text = "Показать фигуры";
            cbImageShowShapes.UseVisualStyleBackColor = true;
            cbImageShowShapes.CheckedChanged += cbImageShowShapes_CheckedChanged;
            // 
            // cbImageShowAngleGrid
            // 
            cbImageShowAngleGrid.AutoSize = true;
            cbImageShowAngleGrid.Location = new Point(184, 363);
            cbImageShowAngleGrid.Name = "cbImageShowAngleGrid";
            cbImageShowAngleGrid.Size = new Size(154, 19);
            cbImageShowAngleGrid.TabIndex = 106;
            cbImageShowAngleGrid.Text = "Повернуть картинку на";
            cbImageShowAngleGrid.UseVisualStyleBackColor = true;
            cbImageShowAngleGrid.CheckedChanged += cbImageShowAngleGrid_CheckedChanged;
            // 
            // cbImageFlipV
            // 
            cbImageFlipV.AutoSize = true;
            cbImageFlipV.Location = new Point(12, 363);
            cbImageFlipV.Name = "cbImageFlipV";
            cbImageFlipV.Size = new Size(153, 19);
            cbImageFlipV.TabIndex = 105;
            cbImageFlipV.Text = "Отразить по вертикали";
            cbImageFlipV.UseVisualStyleBackColor = true;
            cbImageFlipV.CheckedChanged += cbImageFlipV_CheckedChanged;
            // 
            // nudImageAngleDeg
            // 
            nudImageAngleDeg.DecimalPlaces = 2;
            nudImageAngleDeg.Location = new Point(343, 362);
            nudImageAngleDeg.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            nudImageAngleDeg.Minimum = new decimal(new int[] { 180, 0, 0, int.MinValue });
            nudImageAngleDeg.Name = "nudImageAngleDeg";
            nudImageAngleDeg.Size = new Size(82, 23);
            nudImageAngleDeg.TabIndex = 87;
            nudImageAngleDeg.ValueChanged += nudImageAngleDeg_ValueChanged;
            // 
            // cbImageFlipH
            // 
            cbImageFlipH.AutoSize = true;
            cbImageFlipH.Location = new Point(12, 342);
            cbImageFlipH.Name = "cbImageFlipH";
            cbImageFlipH.Size = new Size(166, 19);
            cbImageFlipH.TabIndex = 104;
            cbImageFlipH.Text = "Отразить по горизонтали";
            cbImageFlipH.UseVisualStyleBackColor = true;
            cbImageFlipH.CheckedChanged += cbImageFlipH_CheckedChanged;
            // 
            // lblCalibGridAngleDeg
            // 
            lblCalibGridAngleDeg.AutoSize = true;
            lblCalibGridAngleDeg.Location = new Point(431, 364);
            lblCalibGridAngleDeg.Name = "lblCalibGridAngleDeg";
            lblCalibGridAngleDeg.Size = new Size(56, 15);
            lblCalibGridAngleDeg.TabIndex = 86;
            lblCalibGridAngleDeg.Text = "градусов";
            // 
            // btnSpindle
            // 
            btnSpindle.Location = new Point(165, 627);
            btnSpindle.Margin = new Padding(3, 2, 3, 2);
            btnSpindle.Name = "btnSpindle";
            btnSpindle.Size = new Size(36, 34);
            btnSpindle.TabIndex = 102;
            btnSpindle.Text = "◯";
            btnSpindle.UseVisualStyleBackColor = true;
            btnSpindle.Click += btnLight_Click;
            // 
            // btnZNeg
            // 
            btnZNeg.Location = new Point(165, 664);
            btnZNeg.Name = "btnZNeg";
            btnZNeg.Size = new Size(36, 38);
            btnZNeg.TabIndex = 101;
            btnZNeg.Tag = "5";
            btnZNeg.Text = "▼";
            btnZNeg.UseVisualStyleBackColor = true;
            btnZNeg.Click += btnMove_Click;
            // 
            // btnZPos
            // 
            btnZPos.Location = new Point(165, 583);
            btnZPos.Name = "btnZPos";
            btnZPos.Size = new Size(36, 38);
            btnZPos.TabIndex = 100;
            btnZPos.Tag = "4";
            btnZPos.Text = "▲";
            btnZPos.UseVisualStyleBackColor = true;
            btnZPos.Click += btnMove_Click;
            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(342, 566);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(310, 151);
            lblStatus.TabIndex = 99;
            // 
            // lblMoveAmount
            // 
            lblMoveAmount.Location = new Point(280, 574);
            lblMoveAmount.Name = "lblMoveAmount";
            lblMoveAmount.Size = new Size(57, 135);
            lblMoveAmount.TabIndex = 97;
            lblMoveAmount.Text = "100 мм\r\n\r\n10 мм\r\n\r\n1 мм\r\n\r\n0.1 мм\r\n\r\n0.01 мм";
            // 
            // tbMoveAmount
            // 
            tbMoveAmount.Location = new Point(226, 566);
            tbMoveAmount.Maximum = 2;
            tbMoveAmount.Minimum = -2;
            tbMoveAmount.Name = "tbMoveAmount";
            tbMoveAmount.Orientation = Orientation.Vertical;
            tbMoveAmount.Size = new Size(45, 144);
            tbMoveAmount.TabIndex = 96;
            tbMoveAmount.TickStyle = TickStyle.Both;
            // 
            // btnYNeg
            // 
            btnYNeg.Location = new Point(49, 664);
            btnYNeg.Name = "btnYNeg";
            btnYNeg.Size = new Size(36, 38);
            btnYNeg.TabIndex = 95;
            btnYNeg.Tag = "3";
            btnYNeg.Text = "▼";
            btnYNeg.UseVisualStyleBackColor = true;
            btnYNeg.Click += btnMove_Click;
            // 
            // btnYPos
            // 
            btnYPos.Location = new Point(49, 583);
            btnYPos.Name = "btnYPos";
            btnYPos.Size = new Size(36, 38);
            btnYPos.TabIndex = 94;
            btnYPos.Tag = "2";
            btnYPos.Text = "▲";
            btnYPos.UseVisualStyleBackColor = true;
            btnYPos.Click += btnMove_Click;
            // 
            // btnXPos
            // 
            btnXPos.Location = new Point(89, 623);
            btnXPos.Name = "btnXPos";
            btnXPos.Size = new Size(36, 38);
            btnXPos.TabIndex = 93;
            btnXPos.Tag = "1";
            btnXPos.Text = "▶";
            btnXPos.UseVisualStyleBackColor = true;
            btnXPos.Click += btnMove_Click;
            // 
            // btnXNeg
            // 
            btnXNeg.Location = new Point(11, 623);
            btnXNeg.Name = "btnXNeg";
            btnXNeg.Size = new Size(36, 38);
            btnXNeg.TabIndex = 92;
            btnXNeg.Tag = "0";
            btnXNeg.Text = "◀";
            btnXNeg.UseVisualStyleBackColor = true;
            btnXNeg.Click += btnMove_Click;
            // 
            // tbZoom
            // 
            tbZoom.Location = new Point(245, 515);
            tbZoom.Minimum = 1;
            tbZoom.Name = "tbZoom";
            tbZoom.Size = new Size(407, 45);
            tbZoom.TabIndex = 91;
            tbZoom.TickFrequency = 10;
            tbZoom.TickStyle = TickStyle.Both;
            tbZoom.Value = 1;
            tbZoom.ValueChanged += tbZoom_ValueChanged;
            // 
            // pnMotionIndicator
            // 
            pnMotionIndicator.Location = new Point(183, 512);
            pnMotionIndicator.Name = "pnMotionIndicator";
            pnMotionIndicator.Size = new Size(57, 51);
            pnMotionIndicator.TabIndex = 90;
            // 
            // btnMachineConnect
            // 
            btnMachineConnect.Location = new Point(11, 512);
            btnMachineConnect.Name = "btnMachineConnect";
            btnMachineConnect.Size = new Size(166, 51);
            btnMachineConnect.TabIndex = 89;
            btnMachineConnect.Text = "Подключиться к станку (не используйте USB-hub!)";
            btnMachineConnect.UseVisualStyleBackColor = true;
            btnMachineConnect.Click += btnMachineConnect_Click;
            // 
            // pbCamPreview
            // 
            pbCamPreview.Location = new Point(12, 12);
            pbCamPreview.Name = "pbCamPreview";
            pbCamPreview.Size = new Size(641, 324);
            pbCamPreview.SizeMode = PictureBoxSizeMode.CenterImage;
            pbCamPreview.TabIndex = 88;
            pbCamPreview.TabStop = false;
            pbCamPreview.MouseClick += pbCamPreview_MouseClick;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Black;
            panel1.Location = new Point(659, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(5, 722);
            panel1.TabIndex = 123;
            // 
            // pnpTab
            // 
            pnpTab.Controls.Add(btnLarge);
            pnpTab.Controls.Add(btnUFeeder5);
            pnpTab.Controls.Add(btnUFeeder4);
            pnpTab.Controls.Add(btnUFeeder3);
            pnpTab.Controls.Add(btnUFeeder2);
            pnpTab.Controls.Add(btnUFeeder1);
            pnpTab.Controls.Add(btnSFeeder5);
            pnpTab.Controls.Add(btnSFeeder4);
            pnpTab.Controls.Add(btnSFeeder3);
            pnpTab.Controls.Add(btnSFeeder2);
            pnpTab.Controls.Add(btnSFeeder1);
            pnpTab.Controls.Add(lblSelComp);
            pnpTab.Controls.Add(gbProcess);
            pnpTab.Controls.Add(lvComponents);
            pnpTab.Controls.Add(rbSideBottom);
            pnpTab.Controls.Add(rbSideTop);
            pnpTab.Controls.Add(lblPInstr1);
            pnpTab.Controls.Add(gbDrillUserInput);
            pnpTab.Location = new Point(4, 24);
            pnpTab.Name = "pnpTab";
            pnpTab.Size = new Size(585, 677);
            pnpTab.TabIndex = 2;
            pnpTab.Text = "Расстановка компонентов";
            pnpTab.UseVisualStyleBackColor = true;
            // 
            // btnLarge
            // 
            btnLarge.Location = new Point(414, 605);
            btnLarge.Name = "btnLarge";
            btnLarge.Size = new Size(162, 52);
            btnLarge.TabIndex = 27;
            btnLarge.Tag = "11";
            btnLarge.Text = "LargeChip";
            btnLarge.UseVisualStyleBackColor = true;
            btnLarge.Click += btnQuickMove_Click;
            // 
            // btnUFeeder5
            // 
            btnUFeeder5.Location = new Point(333, 634);
            btnUFeeder5.Name = "btnUFeeder5";
            btnUFeeder5.Size = new Size(75, 23);
            btnUFeeder5.TabIndex = 26;
            btnUFeeder5.Tag = "10";
            btnUFeeder5.Text = "UFeeder5";
            btnUFeeder5.UseVisualStyleBackColor = true;
            btnUFeeder5.Click += btnQuickMove_Click;
            // 
            // btnUFeeder4
            // 
            btnUFeeder4.Location = new Point(252, 634);
            btnUFeeder4.Name = "btnUFeeder4";
            btnUFeeder4.Size = new Size(75, 23);
            btnUFeeder4.TabIndex = 25;
            btnUFeeder4.Tag = "9";
            btnUFeeder4.Text = "UFeeder4";
            btnUFeeder4.UseVisualStyleBackColor = true;
            btnUFeeder4.Click += btnQuickMove_Click;
            // 
            // btnUFeeder3
            // 
            btnUFeeder3.Location = new Point(171, 634);
            btnUFeeder3.Name = "btnUFeeder3";
            btnUFeeder3.Size = new Size(75, 23);
            btnUFeeder3.TabIndex = 24;
            btnUFeeder3.Tag = "8";
            btnUFeeder3.Text = "UFeeder3";
            btnUFeeder3.UseVisualStyleBackColor = true;
            btnUFeeder3.Click += btnQuickMove_Click;
            // 
            // btnUFeeder2
            // 
            btnUFeeder2.Location = new Point(90, 634);
            btnUFeeder2.Name = "btnUFeeder2";
            btnUFeeder2.Size = new Size(75, 23);
            btnUFeeder2.TabIndex = 23;
            btnUFeeder2.Tag = "7";
            btnUFeeder2.Text = "UFeeder2";
            btnUFeeder2.UseVisualStyleBackColor = true;
            btnUFeeder2.Click += btnQuickMove_Click;
            // 
            // btnUFeeder1
            // 
            btnUFeeder1.Location = new Point(9, 634);
            btnUFeeder1.Name = "btnUFeeder1";
            btnUFeeder1.Size = new Size(75, 23);
            btnUFeeder1.TabIndex = 22;
            btnUFeeder1.Tag = "6";
            btnUFeeder1.Text = "UFeeder1";
            btnUFeeder1.UseVisualStyleBackColor = true;
            btnUFeeder1.Click += btnQuickMove_Click;
            // 
            // btnSFeeder5
            // 
            btnSFeeder5.Location = new Point(333, 605);
            btnSFeeder5.Name = "btnSFeeder5";
            btnSFeeder5.Size = new Size(75, 23);
            btnSFeeder5.TabIndex = 21;
            btnSFeeder5.Tag = "5";
            btnSFeeder5.Text = "SFeeder5";
            btnSFeeder5.UseVisualStyleBackColor = true;
            btnSFeeder5.Click += btnQuickMove_Click;
            // 
            // btnSFeeder4
            // 
            btnSFeeder4.Location = new Point(252, 605);
            btnSFeeder4.Name = "btnSFeeder4";
            btnSFeeder4.Size = new Size(75, 23);
            btnSFeeder4.TabIndex = 20;
            btnSFeeder4.Tag = "4";
            btnSFeeder4.Text = "SFeeder4";
            btnSFeeder4.UseVisualStyleBackColor = true;
            btnSFeeder4.Click += btnQuickMove_Click;
            // 
            // btnSFeeder3
            // 
            btnSFeeder3.Location = new Point(171, 605);
            btnSFeeder3.Name = "btnSFeeder3";
            btnSFeeder3.Size = new Size(75, 23);
            btnSFeeder3.TabIndex = 19;
            btnSFeeder3.Tag = "3";
            btnSFeeder3.Text = "SFeeder3";
            btnSFeeder3.UseVisualStyleBackColor = true;
            btnSFeeder3.Click += btnQuickMove_Click;
            // 
            // btnSFeeder2
            // 
            btnSFeeder2.Location = new Point(90, 605);
            btnSFeeder2.Name = "btnSFeeder2";
            btnSFeeder2.Size = new Size(75, 23);
            btnSFeeder2.TabIndex = 18;
            btnSFeeder2.Tag = "2";
            btnSFeeder2.Text = "SFeeder2";
            btnSFeeder2.UseVisualStyleBackColor = true;
            btnSFeeder2.Click += btnQuickMove_Click;
            // 
            // btnSFeeder1
            // 
            btnSFeeder1.Location = new Point(9, 605);
            btnSFeeder1.Name = "btnSFeeder1";
            btnSFeeder1.Size = new Size(75, 23);
            btnSFeeder1.TabIndex = 17;
            btnSFeeder1.Tag = "1";
            btnSFeeder1.Text = "SFeeder1";
            btnSFeeder1.UseVisualStyleBackColor = true;
            btnSFeeder1.Click += btnQuickMove_Click;
            // 
            // lblSelComp
            // 
            lblSelComp.Location = new Point(9, 334);
            lblSelComp.Name = "lblSelComp";
            lblSelComp.Size = new Size(567, 38);
            lblSelComp.TabIndex = 16;
            // 
            // gbProcess
            // 
            gbProcess.Controls.Add(cbDemoPlaced);
            gbProcess.Controls.Add(nudLyingDeg);
            gbProcess.Controls.Add(rbPNP);
            gbProcess.Controls.Add(rbPanelSpecify);
            gbProcess.Controls.Add(btnRun);
            gbProcess.Location = new Point(9, 377);
            gbProcess.Name = "gbProcess";
            gbProcess.Size = new Size(567, 127);
            gbProcess.TabIndex = 15;
            gbProcess.TabStop = false;
            gbProcess.Text = "Процесс";
            // 
            // cbDemoPlaced
            // 
            cbDemoPlaced.AutoSize = true;
            cbDemoPlaced.Checked = true;
            cbDemoPlaced.CheckState = CheckState.Checked;
            cbDemoPlaced.Location = new Point(6, 72);
            cbDemoPlaced.Name = "cbDemoPlaced";
            cbDemoPlaced.Size = new Size(278, 19);
            cbDemoPlaced.TabIndex = 5;
            cbDemoPlaced.Text = "Демонстрировать установленный компонент";
            cbDemoPlaced.UseVisualStyleBackColor = true;
            // 
            // nudLyingDeg
            // 
            nudLyingDeg.DecimalPlaces = 2;
            nudLyingDeg.Location = new Point(326, 47);
            nudLyingDeg.Maximum = new decimal(new int[] { 180, 0, 0, 0 });
            nudLyingDeg.Minimum = new decimal(new int[] { 180, 0, 0, int.MinValue });
            nudLyingDeg.Name = "nudLyingDeg";
            nudLyingDeg.Size = new Size(79, 23);
            nudLyingDeg.TabIndex = 4;
            nudLyingDeg.Value = new decimal(new int[] { 90, 0, 0, 0 });
            // 
            // rbPNP
            // 
            rbPNP.AutoSize = true;
            rbPNP.Location = new Point(6, 47);
            rbPNP.Name = "rbPNP";
            rbPNP.Size = new Size(314, 19);
            rbPNP.TabIndex = 1;
            rbPNP.TabStop = true;
            rbPNP.Text = "Поставить компонент, он лежит под углом, градусы";
            rbPNP.UseVisualStyleBackColor = true;
            rbPNP.CheckedChanged += rbPNP_CheckedChanged;
            // 
            // rbPanelSpecify
            // 
            rbPanelSpecify.AutoSize = true;
            rbPanelSpecify.Checked = true;
            rbPanelSpecify.Location = new Point(6, 22);
            rbPanelSpecify.Name = "rbPanelSpecify";
            rbPanelSpecify.Size = new Size(158, 19);
            rbPanelSpecify.TabIndex = 0;
            rbPanelSpecify.TabStop = true;
            rbPanelSpecify.Text = "Выбрать начало панели";
            rbPanelSpecify.UseVisualStyleBackColor = true;
            rbPanelSpecify.CheckedChanged += rbPanelSpecify_CheckedChanged;
            // 
            // btnRun
            // 
            btnRun.Location = new Point(6, 98);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(89, 23);
            btnRun.TabIndex = 3;
            btnRun.Text = "Выполнить";
            btnRun.UseVisualStyleBackColor = true;
            btnRun.Click += btnDrill_Click;
            // 
            // lvComponents
            // 
            lvComponents.FullRowSelect = true;
            lvComponents.Location = new Point(9, 74);
            lvComponents.Name = "lvComponents";
            lvComponents.Size = new Size(567, 251);
            lvComponents.TabIndex = 14;
            lvComponents.UseCompatibleStateImageBehavior = false;
            lvComponents.ItemSelectionChanged += lvComponents_ItemSelectionChanged;
            lvComponents.DoubleClick += lvComponents_DoubleClick;
            // 
            // rbSideBottom
            // 
            rbSideBottom.AutoSize = true;
            rbSideBottom.Location = new Point(60, 49);
            rbSideBottom.Name = "rbSideBottom";
            rbSideBottom.Size = new Size(65, 19);
            rbSideBottom.TabIndex = 13;
            rbSideBottom.Text = "Bottom";
            rbSideBottom.UseVisualStyleBackColor = true;
            rbSideBottom.CheckedChanged += rbSideBottom_CheckedChanged;
            // 
            // rbSideTop
            // 
            rbSideTop.AutoSize = true;
            rbSideTop.Checked = true;
            rbSideTop.Location = new Point(9, 49);
            rbSideTop.Name = "rbSideTop";
            rbSideTop.Size = new Size(45, 19);
            rbSideTop.TabIndex = 12;
            rbSideTop.TabStop = true;
            rbSideTop.Text = "Top";
            rbSideTop.UseVisualStyleBackColor = true;
            rbSideTop.CheckedChanged += rbSideTop_CheckedChanged;
            // 
            // lblPInstr1
            // 
            lblPInstr1.Location = new Point(9, 12);
            lblPInstr1.Name = "lblPInstr1";
            lblPInstr1.Size = new Size(573, 34);
            lblPInstr1.TabIndex = 11;
            lblPInstr1.Text = "1. Сначала выберите начало панели. Затем переместите каретку так, чтобы центр изображения камеры показывал на компонент. Выберите этот компонент из списка и выполните процесс. Повторите для каждого.";
            // 
            // gbDrillUserInput
            // 
            gbDrillUserInput.Controls.Add(btnDInstr6);
            gbDrillUserInput.Controls.Add(btnDInstr5);
            gbDrillUserInput.Controls.Add(btnDInstr4);
            gbDrillUserInput.Controls.Add(btnDInstr3);
            gbDrillUserInput.Controls.Add(lblDInstr);
            gbDrillUserInput.Controls.Add(btnDInstr2);
            gbDrillUserInput.Controls.Add(btnDInstr1);
            gbDrillUserInput.Location = new Point(9, 510);
            gbDrillUserInput.Name = "gbDrillUserInput";
            gbDrillUserInput.Size = new Size(567, 89);
            gbDrillUserInput.TabIndex = 5;
            gbDrillUserInput.TabStop = false;
            gbDrillUserInput.Text = "Инструкция";
            // 
            // btnDInstr6
            // 
            btnDInstr6.Location = new Point(411, 60);
            btnDInstr6.Name = "btnDInstr6";
            btnDInstr6.Size = new Size(75, 23);
            btnDInstr6.TabIndex = 6;
            btnDInstr6.Tag = "5";
            btnDInstr6.Text = "Кнопка 6";
            btnDInstr6.UseVisualStyleBackColor = true;
            btnDInstr6.Click += btnUserInput_Click;
            // 
            // btnDInstr5
            // 
            btnDInstr5.Location = new Point(330, 60);
            btnDInstr5.Name = "btnDInstr5";
            btnDInstr5.Size = new Size(75, 23);
            btnDInstr5.TabIndex = 5;
            btnDInstr5.Tag = "4";
            btnDInstr5.Text = "Кнопка 5";
            btnDInstr5.UseVisualStyleBackColor = true;
            btnDInstr5.Click += btnUserInput_Click;
            // 
            // btnDInstr4
            // 
            btnDInstr4.Location = new Point(249, 60);
            btnDInstr4.Name = "btnDInstr4";
            btnDInstr4.Size = new Size(75, 23);
            btnDInstr4.TabIndex = 4;
            btnDInstr4.Tag = "3";
            btnDInstr4.Text = "Кнопка 4";
            btnDInstr4.UseVisualStyleBackColor = true;
            btnDInstr4.Click += btnUserInput_Click;
            // 
            // btnDInstr3
            // 
            btnDInstr3.Location = new Point(168, 60);
            btnDInstr3.Name = "btnDInstr3";
            btnDInstr3.Size = new Size(75, 23);
            btnDInstr3.TabIndex = 3;
            btnDInstr3.Tag = "2";
            btnDInstr3.Text = "Кнопка 3";
            btnDInstr3.UseVisualStyleBackColor = true;
            btnDInstr3.Click += btnUserInput_Click;
            // 
            // lblDInstr
            // 
            lblDInstr.Location = new Point(6, 19);
            lblDInstr.Name = "lblDInstr";
            lblDInstr.Size = new Size(567, 38);
            lblDInstr.TabIndex = 2;
            lblDInstr.Text = "Её подробности";
            // 
            // btnDInstr2
            // 
            btnDInstr2.Location = new Point(87, 60);
            btnDInstr2.Name = "btnDInstr2";
            btnDInstr2.Size = new Size(75, 23);
            btnDInstr2.TabIndex = 1;
            btnDInstr2.Tag = "2";
            btnDInstr2.Text = "Кнопка 2";
            btnDInstr2.UseVisualStyleBackColor = true;
            btnDInstr2.Click += btnUserInput_Click;
            // 
            // btnDInstr1
            // 
            btnDInstr1.Location = new Point(6, 60);
            btnDInstr1.Name = "btnDInstr1";
            btnDInstr1.Size = new Size(75, 23);
            btnDInstr1.TabIndex = 0;
            btnDInstr1.Tag = "1";
            btnDInstr1.Text = "Кнопка 1";
            btnDInstr1.UseVisualStyleBackColor = true;
            btnDInstr1.Click += btnUserInput_Click;
            // 
            // calibTab
            // 
            calibTab.Controls.Add(cbCAxisPositive);
            calibTab.Controls.Add(lblAddAngle);
            calibTab.Controls.Add(nudAddAngle);
            calibTab.Controls.Add(lblCalibInstr5);
            calibTab.Controls.Add(nudStepsFullTurn);
            calibTab.Controls.Add(lblStepsPerTurn);
            calibTab.Controls.Add(lblSteps);
            calibTab.Controls.Add(nudCCalSteps);
            calibTab.Controls.Add(lblCalibInstr4);
            calibTab.Controls.Add(rbCalCAxis);
            calibTab.Controls.Add(rbCalCamOffset);
            calibTab.Controls.Add(rbCalSteps);
            calibTab.Controls.Add(lblCalibInstr3);
            calibTab.Controls.Add(lblCalibInstr6);
            calibTab.Controls.Add(btnContinueAfterCalib);
            calibTab.Controls.Add(gbCalibUserInput);
            calibTab.Controls.Add(btnCalib);
            calibTab.Controls.Add(lblCalibInstr2);
            calibTab.Controls.Add(btnPrintCalib);
            calibTab.Controls.Add(lblCalibInstr1);
            calibTab.Location = new Point(4, 24);
            calibTab.Name = "calibTab";
            calibTab.Size = new Size(585, 677);
            calibTab.TabIndex = 1;
            calibTab.Text = "Калибровка";
            calibTab.UseVisualStyleBackColor = true;
            // 
            // cbCAxisPositive
            // 
            cbCAxisPositive.AutoSize = true;
            cbCAxisPositive.Location = new Point(3, 281);
            cbCAxisPositive.Name = "cbCAxisPositive";
            cbCAxisPositive.Size = new Size(415, 19);
            cbCAxisPositive.TabIndex = 20;
            cbCAxisPositive.Text = "Ось C крутится по часовой стрелке при положительном направлении";
            cbCAxisPositive.UseVisualStyleBackColor = true;
            cbCAxisPositive.CheckedChanged += cbCAxisPositive_CheckedChanged;
            // 
            // lblAddAngle
            // 
            lblAddAngle.AutoSize = true;
            lblAddAngle.Location = new Point(3, 371);
            lblAddAngle.Name = "lblAddAngle";
            lblAddAngle.Size = new Size(160, 15);
            lblAddAngle.TabIndex = 19;
            lblAddAngle.Text = "Добавочный угол, градусы:";
            // 
            // nudAddAngle
            // 
            nudAddAngle.DecimalPlaces = 2;
            nudAddAngle.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudAddAngle.Location = new Point(169, 369);
            nudAddAngle.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            nudAddAngle.Name = "nudAddAngle";
            nudAddAngle.Size = new Size(95, 23);
            nudAddAngle.TabIndex = 18;
            nudAddAngle.ValueChanged += nudAddAngle_ValueChanged;
            // 
            // lblCalibInstr5
            // 
            lblCalibInstr5.Location = new Point(3, 310);
            lblCalibInstr5.Name = "lblCalibInstr5";
            lblCalibInstr5.Size = new Size(579, 50);
            lblCalibInstr5.TabIndex = 17;
            lblCalibInstr5.Text = "5. Камера не всегда установлена на станке ровно. В идеале ось X изображения должна быть параллельна оси X движения, и с Y тоже самое, но по факту не совсем. Экспериментально определите отклонение.";
            // 
            // nudStepsFullTurn
            // 
            nudStepsFullTurn.Location = new Point(160, 256);
            nudStepsFullTurn.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudStepsFullTurn.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
            nudStepsFullTurn.Name = "nudStepsFullTurn";
            nudStepsFullTurn.Size = new Size(100, 23);
            nudStepsFullTurn.TabIndex = 16;
            nudStepsFullTurn.Value = new decimal(new int[] { 3200, 0, 0, 0 });
            nudStepsFullTurn.ValueChanged += nudStepsFullTurn_ValueChanged;
            // 
            // lblStepsPerTurn
            // 
            lblStepsPerTurn.AutoSize = true;
            lblStepsPerTurn.Location = new Point(3, 258);
            lblStepsPerTurn.Name = "lblStepsPerTurn";
            lblStepsPerTurn.Size = new Size(151, 15);
            lblStepsPerTurn.TabIndex = 15;
            lblStepsPerTurn.Text = "Шагов на полный оборот:";
            // 
            // lblSteps
            // 
            lblSteps.AutoSize = true;
            lblSteps.Location = new Point(218, 455);
            lblSteps.Name = "lblSteps";
            lblSteps.Size = new Size(42, 15);
            lblSteps.TabIndex = 14;
            lblSteps.Text = "шагов";
            // 
            // nudCCalSteps
            // 
            nudCCalSteps.Location = new Point(119, 451);
            nudCCalSteps.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudCCalSteps.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudCCalSteps.Name = "nudCCalSteps";
            nudCCalSteps.Size = new Size(93, 23);
            nudCCalSteps.TabIndex = 13;
            nudCCalSteps.Value = new decimal(new int[] { 3200, 0, 0, 0 });
            // 
            // lblCalibInstr4
            // 
            lblCalibInstr4.Location = new Point(3, 170);
            lblCalibInstr4.Name = "lblCalibInstr4";
            lblCalibInstr4.Size = new Size(579, 78);
            lblCalibInstr4.TabIndex = 12;
            lblCalibInstr4.Text = resources.GetString("lblCalibInstr4.Text");
            // 
            // rbCalCAxis
            // 
            rbCalCAxis.AutoSize = true;
            rbCalCAxis.Location = new Point(3, 451);
            rbCalCAxis.Name = "rbCalCAxis";
            rbCalCAxis.Size = new Size(110, 19);
            rbCalCAxis.TabIndex = 10;
            rbCalCAxis.TabStop = true;
            rbCalCAxis.Text = "Ось C - сделать";
            rbCalCAxis.UseVisualStyleBackColor = true;
            rbCalCAxis.CheckedChanged += rbCalCAxis_CheckedChanged;
            // 
            // rbCalCamOffset
            // 
            rbCalCamOffset.AutoSize = true;
            rbCalCamOffset.Location = new Point(3, 426);
            rbCalCamOffset.Name = "rbCalCamOffset";
            rbCalCamOffset.Size = new Size(236, 19);
            rbCalCamOffset.TabIndex = 9;
            rbCalCamOffset.TabStop = true;
            rbCalCamOffset.Text = "Смещение от камеры до инструмента";
            rbCalCamOffset.UseVisualStyleBackColor = true;
            rbCalCamOffset.CheckedChanged += rbCalCamOffset_CheckedChanged;
            // 
            // rbCalSteps
            // 
            rbCalSteps.AutoSize = true;
            rbCalSteps.Checked = true;
            rbCalSteps.Location = new Point(3, 401);
            rbCalSteps.Name = "rbCalSteps";
            rbCalSteps.Size = new Size(92, 19);
            rbCalSteps.TabIndex = 8;
            rbCalSteps.TabStop = true;
            rbCalSteps.Text = "Шаги и угол";
            rbCalSteps.UseVisualStyleBackColor = true;
            rbCalSteps.CheckedChanged += rbCalSteps_CheckedChanged;
            // 
            // lblCalibInstr3
            // 
            lblCalibInstr3.Location = new Point(3, 121);
            lblCalibInstr3.Name = "lblCalibInstr3";
            lblCalibInstr3.Size = new Size(579, 49);
            lblCalibInstr3.TabIndex = 7;
            lblCalibInstr3.Text = resources.GetString("lblCalibInstr3.Text");
            // 
            // lblCalibInstr6
            // 
            lblCalibInstr6.Location = new Point(0, 596);
            lblCalibInstr6.Name = "lblCalibInstr6";
            lblCalibInstr6.Size = new Size(579, 17);
            lblCalibInstr6.TabIndex = 6;
            lblCalibInstr6.Text = "4. Нажмите кнопку продолжить для перехода к техпроцессу.";
            // 
            // btnContinueAfterCalib
            // 
            btnContinueAfterCalib.Location = new Point(485, 615);
            btnContinueAfterCalib.Name = "btnContinueAfterCalib";
            btnContinueAfterCalib.Size = new Size(97, 23);
            btnContinueAfterCalib.TabIndex = 5;
            btnContinueAfterCalib.Text = "Продолжить";
            btnContinueAfterCalib.UseVisualStyleBackColor = true;
            btnContinueAfterCalib.Click += btnContinueAfterCalib_Click;
            // 
            // gbCalibUserInput
            // 
            gbCalibUserInput.Controls.Add(lblCInstr);
            gbCalibUserInput.Controls.Add(btnInstr2);
            gbCalibUserInput.Controls.Add(btnInstr1);
            gbCalibUserInput.Location = new Point(3, 505);
            gbCalibUserInput.Name = "gbCalibUserInput";
            gbCalibUserInput.Size = new Size(579, 89);
            gbCalibUserInput.TabIndex = 4;
            gbCalibUserInput.TabStop = false;
            gbCalibUserInput.Text = "Инструкция";
            // 
            // lblCInstr
            // 
            lblCInstr.Location = new Point(6, 19);
            lblCInstr.Name = "lblCInstr";
            lblCInstr.Size = new Size(567, 38);
            lblCInstr.TabIndex = 2;
            lblCInstr.Text = "Её подробности";
            // 
            // btnInstr2
            // 
            btnInstr2.Location = new Point(87, 60);
            btnInstr2.Name = "btnInstr2";
            btnInstr2.Size = new Size(75, 23);
            btnInstr2.TabIndex = 1;
            btnInstr2.Tag = "2";
            btnInstr2.Text = "Кнопка 2";
            btnInstr2.UseVisualStyleBackColor = true;
            btnInstr2.Click += btnUserInput_Click;
            // 
            // btnInstr1
            // 
            btnInstr1.Location = new Point(6, 60);
            btnInstr1.Name = "btnInstr1";
            btnInstr1.Size = new Size(75, 23);
            btnInstr1.TabIndex = 0;
            btnInstr1.Tag = "1";
            btnInstr1.Text = "Кнопка 1";
            btnInstr1.UseVisualStyleBackColor = true;
            btnInstr1.Click += btnUserInput_Click;
            // 
            // btnCalib
            // 
            btnCalib.Location = new Point(3, 476);
            btnCalib.Name = "btnCalib";
            btnCalib.Size = new Size(89, 23);
            btnCalib.TabIndex = 3;
            btnCalib.Text = "Калибровать";
            btnCalib.UseVisualStyleBackColor = true;
            btnCalib.Click += btnCalib_Click;
            // 
            // lblCalibInstr2
            // 
            lblCalibInstr2.Location = new Point(3, 72);
            lblCalibInstr2.Name = "lblCalibInstr2";
            lblCalibInstr2.Size = new Size(579, 49);
            lblCalibInstr2.TabIndex = 2;
            lblCalibInstr2.Text = resources.GetString("lblCalibInstr2.Text");
            // 
            // btnPrintCalib
            // 
            btnPrintCalib.Location = new Point(3, 46);
            btnPrintCalib.Name = "btnPrintCalib";
            btnPrintCalib.Size = new Size(75, 23);
            btnPrintCalib.TabIndex = 1;
            btnPrintCalib.Text = "Печать";
            btnPrintCalib.UseVisualStyleBackColor = true;
            btnPrintCalib.Click += btnPrintCalib_Click;
            // 
            // lblCalibInstr1
            // 
            lblCalibInstr1.Location = new Point(3, 11);
            lblCalibInstr1.Name = "lblCalibInstr1";
            lblCalibInstr1.Size = new Size(579, 32);
            lblCalibInstr1.TabIndex = 0;
            lblCalibInstr1.Text = "1. Распечатайте в высоком разрешении калибровочный узор на бумаге и положите его на рабочее поле станка по его центру, закрепив его, чтобы он лежал ровно и не изгибался.";
            // 
            // noConnectionTab
            // 
            noConnectionTab.Controls.Add(lblAlarmStatus);
            noConnectionTab.Location = new Point(4, 24);
            noConnectionTab.Name = "noConnectionTab";
            noConnectionTab.Size = new Size(585, 677);
            noConnectionTab.TabIndex = 0;
            noConnectionTab.Text = "Ошибка";
            noConnectionTab.UseVisualStyleBackColor = true;
            // 
            // lblAlarmStatus
            // 
            lblAlarmStatus.AutoSize = true;
            lblAlarmStatus.Location = new Point(3, 9);
            lblAlarmStatus.Name = "lblAlarmStatus";
            lblAlarmStatus.Size = new Size(318, 15);
            lblAlarmStatus.TabIndex = 0;
            lblAlarmStatus.Text = "Станок в состоянии Alarm или нет подключения к нему.";
            // 
            // procTabControl
            // 
            procTabControl.Controls.Add(noConnectionTab);
            procTabControl.Controls.Add(calibTab);
            procTabControl.Controls.Add(pnpTab);
            procTabControl.Location = new Point(670, 12);
            procTabControl.Name = "procTabControl";
            procTabControl.SelectedIndex = 0;
            procTabControl.Size = new Size(593, 705);
            procTabControl.TabIndex = 124;
            procTabControl.Selecting += procTabControl_Selecting;
            // 
            // tbMachineIP
            // 
            tbMachineIP.Location = new Point(123, 483);
            tbMachineIP.Name = "tbMachineIP";
            tbMachineIP.Size = new Size(138, 23);
            tbMachineIP.TabIndex = 125;
            // 
            // lblMachineIP
            // 
            lblMachineIP.AutoSize = true;
            lblMachineIP.Location = new Point(12, 486);
            lblMachineIP.Name = "lblMachineIP";
            lblMachineIP.Size = new Size(95, 15);
            lblMachineIP.TabIndex = 126;
            lblMachineIP.Text = "IP-адрес станка:";
            // 
            // PNPForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1265, 719);
            Controls.Add(lblMachineIP);
            Controls.Add(tbMachineIP);
            Controls.Add(procTabControl);
            Controls.Add(panel1);
            Controls.Add(nudFOVYTo);
            Controls.Add(nudFOVYFrom);
            Controls.Add(lblFOVY);
            Controls.Add(nudFOVXTo);
            Controls.Add(nudFOVXFrom);
            Controls.Add(lblFOVX);
            Controls.Add(btnCamNext);
            Controls.Add(btnCamPrev);
            Controls.Add(cbShowRect);
            Controls.Add(lblPxPerMm);
            Controls.Add(nudImagePxPerMm);
            Controls.Add(lblMm);
            Controls.Add(lblXRect);
            Controls.Add(nudRectHeightMm);
            Controls.Add(nudRectWidthMm);
            Controls.Add(cbImageShowShapes);
            Controls.Add(cbImageShowAngleGrid);
            Controls.Add(cbImageFlipV);
            Controls.Add(nudImageAngleDeg);
            Controls.Add(cbImageFlipH);
            Controls.Add(lblCalibGridAngleDeg);
            Controls.Add(btnSpindle);
            Controls.Add(btnZNeg);
            Controls.Add(btnZPos);
            Controls.Add(lblStatus);
            Controls.Add(lblMoveAmount);
            Controls.Add(tbMoveAmount);
            Controls.Add(btnYNeg);
            Controls.Add(btnYPos);
            Controls.Add(btnXPos);
            Controls.Add(btnXNeg);
            Controls.Add(tbZoom);
            Controls.Add(pnMotionIndicator);
            Controls.Add(btnMachineConnect);
            Controls.Add(pbCamPreview);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "PNPForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Pick&Place";
            FormClosing += CNCForm_FormClosing;
            Load += CNCForm_Load;
            ((System.ComponentModel.ISupportInitialize)nudFOVYTo).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudFOVYFrom).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudFOVXTo).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudFOVXFrom).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudImagePxPerMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRectHeightMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRectWidthMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudImageAngleDeg).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbMoveAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbZoom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbCamPreview).EndInit();
            pnpTab.ResumeLayout(false);
            pnpTab.PerformLayout();
            gbProcess.ResumeLayout(false);
            gbProcess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudLyingDeg).EndInit();
            gbDrillUserInput.ResumeLayout(false);
            calibTab.ResumeLayout(false);
            calibTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudAddAngle).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudStepsFullTurn).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCCalSteps).EndInit();
            gbCalibUserInput.ResumeLayout(false);
            noConnectionTab.ResumeLayout(false);
            noConnectionTab.PerformLayout();
            procTabControl.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown nudFOVYTo;
        private NumericUpDown nudFOVYFrom;
        private Label lblFOVY;
        private NumericUpDown nudFOVXTo;
        private NumericUpDown nudFOVXFrom;
        private Label lblFOVX;
        private Button btnCamNext;
        private Button btnCamPrev;
        private CheckBox cbShowRect;
        private Label lblPxPerMm;
        private NumericUpDown nudImagePxPerMm;
        private Label lblMm;
        private Label lblXRect;
        private NumericUpDown nudRectHeightMm;
        private NumericUpDown nudRectWidthMm;
        private CheckBox cbImageShowShapes;
        private CheckBox cbImageShowAngleGrid;
        private CheckBox cbImageFlipV;
        private NumericUpDown nudImageAngleDeg;
        private CheckBox cbImageFlipH;
        private Label lblCalibGridAngleDeg;
        private Button btnSpindle;
        private Button btnZNeg;
        private Button btnZPos;
        private Label lblStatus;
        private Label lblMoveAmount;
        private TrackBar tbMoveAmount;
        private Button btnYNeg;
        private Button btnYPos;
        private Button btnXPos;
        private Button btnXNeg;
        private TrackBar tbZoom;
        private Panel pnMotionIndicator;
        private Button btnMachineConnect;
        private PictureBox pbCamPreview;
        private Panel panel1;
        private TabPage pnpTab;
        private GroupBox gbDrillUserInput;
        private Button btnDInstr3;
        private Label lblDInstr;
        private Button btnDInstr2;
        private Button btnDInstr1;
        private Button btnRun;
        private TabPage calibTab;
        private RadioButton rbCalCamOffset;
        private RadioButton rbCalSteps;
        private Label lblCalibInstr3;
        private Label lblCalibInstr6;
        private Button btnContinueAfterCalib;
        private GroupBox gbCalibUserInput;
        private Label lblCInstr;
        private Button btnInstr2;
        private Button btnInstr1;
        private Button btnCalib;
        private Label lblCalibInstr2;
        private Button btnPrintCalib;
        private Label lblCalibInstr1;
        private TabPage noConnectionTab;
        private Label lblAlarmStatus;
        private TabControl procTabControl;
        private RadioButton rbCalCAxis;
        private Label lblCalibInstr4;
        private NumericUpDown nudCCalSteps;
        private Label lblSteps;
        private Label lblStepsPerTurn;
        private NumericUpDown nudStepsFullTurn;
        private Label lblCalibInstr5;
        private NumericUpDown nudAddAngle;
        private Label lblAddAngle;
        private CheckBox cbCAxisPositive;
        private TextBox tbMachineIP;
        private Label lblMachineIP;
        private Label lblPInstr1;
        private RadioButton rbSideTop;
        private RadioButton rbSideBottom;
        private ListView lvComponents;
        private GroupBox gbProcess;
        private RadioButton rbPanelSpecify;
        private RadioButton rbPNP;
        private Button btnDInstr4;
        private Button btnDInstr6;
        private Button btnDInstr5;
        private Label lblSelComp;
        private NumericUpDown nudLyingDeg;
        private CheckBox cbDemoPlaced;
        private Button btnSFeeder1;
        private Button btnSFeeder2;
        private Button btnSFeeder3;
        private Button btnSFeeder4;
        private Button btnSFeeder5;
        private Button btnUFeeder5;
        private Button btnUFeeder4;
        private Button btnUFeeder3;
        private Button btnUFeeder2;
        private Button btnUFeeder1;
        private Button btnLarge;
    }
}