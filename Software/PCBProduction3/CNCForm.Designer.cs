namespace PCBProduction3
{
    partial class CNCForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CNCForm));
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
            procTabControl = new TabControl();
            noConnectionTab = new TabPage();
            lblAlarmStatus = new Label();
            calibTab = new TabPage();
            rbCalCamOffset = new RadioButton();
            rbCalSteps = new RadioButton();
            lblCalibInstr3 = new Label();
            lblCalibInstr4 = new Label();
            btnContinueAfterCalib = new Button();
            gbCalibUserInput = new GroupBox();
            lblCInstr = new Label();
            btnInstr2 = new Button();
            btnInstr1 = new Button();
            btnCalib = new Button();
            lblCalibInstr2 = new Label();
            btnPrintCalib = new Button();
            lblCalibInstr1 = new Label();
            newPanelTab = new TabPage();
            nudDWorkpieceThicknessMm = new NumericUpDown();
            nudDSafeHeightMm = new NumericUpDown();
            lblDWorkpieceThickness = new Label();
            lblDSafeHeight = new Label();
            pbDrilling = new ProgressBar();
            gbDrillUserInput = new GroupBox();
            btnDInstr3 = new Button();
            lblDInstr = new Label();
            btnDInstr2 = new Button();
            btnDInstr1 = new Button();
            cbDrillMainHoles = new CheckBox();
            btnDrill = new Button();
            rbPanelPos2 = new RadioButton();
            rbPanelPos1 = new RadioButton();
            lblQInstr1 = new Label();
            existingPanelTab = new TabPage();
            cbInvertY = new CheckBox();
            cbInvertX = new CheckBox();
            pbDepanelizePreview = new PictureBox();
            btnDepanelizePreview = new Button();
            pbTransforming = new ProgressBar();
            gbTransformUserInput = new GroupBox();
            btnTInstr3 = new Button();
            lblTInstr = new Label();
            btnTInstr2 = new Button();
            btnTInstr1 = new Button();
            btnTransform = new Button();
            rbDepanelize = new RadioButton();
            rbDrill = new RadioButton();
            lblTInstr1 = new Label();
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
            procTabControl.SuspendLayout();
            noConnectionTab.SuspendLayout();
            calibTab.SuspendLayout();
            gbCalibUserInput.SuspendLayout();
            newPanelTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudDWorkpieceThicknessMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDSafeHeightMm).BeginInit();
            gbDrillUserInput.SuspendLayout();
            existingPanelTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbDepanelizePreview).BeginInit();
            gbTransformUserInput.SuspendLayout();
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
            btnSpindle.Location = new Point(166, 598);
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
            btnZNeg.Location = new Point(166, 635);
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
            btnZPos.Location = new Point(166, 554);
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
            lblStatus.Location = new Point(343, 537);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(310, 151);
            lblStatus.TabIndex = 99;
            // 
            // lblMoveAmount
            // 
            lblMoveAmount.Location = new Point(281, 545);
            lblMoveAmount.Name = "lblMoveAmount";
            lblMoveAmount.Size = new Size(57, 135);
            lblMoveAmount.TabIndex = 97;
            lblMoveAmount.Text = "100 мм\r\n\r\n10 мм\r\n\r\n1 мм\r\n\r\n0.1 мм\r\n\r\n0.01 мм";
            // 
            // tbMoveAmount
            // 
            tbMoveAmount.Location = new Point(227, 537);
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
            btnYNeg.Location = new Point(50, 635);
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
            btnYPos.Location = new Point(50, 554);
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
            btnXPos.Location = new Point(90, 594);
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
            btnXNeg.Location = new Point(12, 594);
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
            tbZoom.Location = new Point(246, 486);
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
            pnMotionIndicator.Location = new Point(184, 483);
            pnMotionIndicator.Name = "pnMotionIndicator";
            pnMotionIndicator.Size = new Size(57, 51);
            pnMotionIndicator.TabIndex = 90;
            // 
            // btnMachineConnect
            // 
            btnMachineConnect.Location = new Point(12, 483);
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
            panel1.Size = new Size(5, 697);
            panel1.TabIndex = 123;
            // 
            // procTabControl
            // 
            procTabControl.Controls.Add(noConnectionTab);
            procTabControl.Controls.Add(calibTab);
            procTabControl.Controls.Add(newPanelTab);
            procTabControl.Controls.Add(existingPanelTab);
            procTabControl.Location = new Point(670, 12);
            procTabControl.Name = "procTabControl";
            procTabControl.SelectedIndex = 0;
            procTabControl.Size = new Size(593, 676);
            procTabControl.TabIndex = 124;
            procTabControl.Selecting += procTabControl_Selecting;
            // 
            // noConnectionTab
            // 
            noConnectionTab.Controls.Add(lblAlarmStatus);
            noConnectionTab.Location = new Point(4, 24);
            noConnectionTab.Name = "noConnectionTab";
            noConnectionTab.Size = new Size(585, 648);
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
            // calibTab
            // 
            calibTab.Controls.Add(rbCalCamOffset);
            calibTab.Controls.Add(rbCalSteps);
            calibTab.Controls.Add(lblCalibInstr3);
            calibTab.Controls.Add(lblCalibInstr4);
            calibTab.Controls.Add(btnContinueAfterCalib);
            calibTab.Controls.Add(gbCalibUserInput);
            calibTab.Controls.Add(btnCalib);
            calibTab.Controls.Add(lblCalibInstr2);
            calibTab.Controls.Add(btnPrintCalib);
            calibTab.Controls.Add(lblCalibInstr1);
            calibTab.Location = new Point(4, 24);
            calibTab.Name = "calibTab";
            calibTab.Size = new Size(585, 648);
            calibTab.TabIndex = 1;
            calibTab.Text = "Калибровка";
            calibTab.UseVisualStyleBackColor = true;
            // 
            // rbCalCamOffset
            // 
            rbCalCamOffset.AutoSize = true;
            rbCalCamOffset.Location = new Point(196, 177);
            rbCalCamOffset.Name = "rbCalCamOffset";
            rbCalCamOffset.Size = new Size(236, 19);
            rbCalCamOffset.TabIndex = 9;
            rbCalCamOffset.TabStop = true;
            rbCalCamOffset.Text = "Смещение от камеры до инструмента";
            rbCalCamOffset.UseVisualStyleBackColor = true;
            // 
            // rbCalSteps
            // 
            rbCalSteps.AutoSize = true;
            rbCalSteps.Checked = true;
            rbCalSteps.Location = new Point(98, 177);
            rbCalSteps.Name = "rbCalSteps";
            rbCalSteps.Size = new Size(92, 19);
            rbCalSteps.TabIndex = 8;
            rbCalSteps.TabStop = true;
            rbCalSteps.Text = "Шаги и угол";
            rbCalSteps.UseVisualStyleBackColor = true;
            // 
            // lblCalibInstr3
            // 
            lblCalibInstr3.Location = new Point(3, 121);
            lblCalibInstr3.Name = "lblCalibInstr3";
            lblCalibInstr3.Size = new Size(579, 49);
            lblCalibInstr3.TabIndex = 7;
            lblCalibInstr3.Text = resources.GetString("lblCalibInstr3.Text");
            // 
            // lblCalibInstr4
            // 
            lblCalibInstr4.Location = new Point(3, 294);
            lblCalibInstr4.Name = "lblCalibInstr4";
            lblCalibInstr4.Size = new Size(579, 17);
            lblCalibInstr4.TabIndex = 6;
            lblCalibInstr4.Text = "4. Нажмите кнопку продолжить для перехода к техпроцессу.";
            // 
            // btnContinueAfterCalib
            // 
            btnContinueAfterCalib.Location = new Point(479, 614);
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
            gbCalibUserInput.Location = new Point(3, 202);
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
            btnCalib.Location = new Point(3, 173);
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
            // newPanelTab
            // 
            newPanelTab.Controls.Add(nudDWorkpieceThicknessMm);
            newPanelTab.Controls.Add(nudDSafeHeightMm);
            newPanelTab.Controls.Add(lblDWorkpieceThickness);
            newPanelTab.Controls.Add(lblDSafeHeight);
            newPanelTab.Controls.Add(pbDrilling);
            newPanelTab.Controls.Add(gbDrillUserInput);
            newPanelTab.Controls.Add(cbDrillMainHoles);
            newPanelTab.Controls.Add(btnDrill);
            newPanelTab.Controls.Add(rbPanelPos2);
            newPanelTab.Controls.Add(rbPanelPos1);
            newPanelTab.Controls.Add(lblQInstr1);
            newPanelTab.Location = new Point(4, 24);
            newPanelTab.Name = "newPanelTab";
            newPanelTab.Size = new Size(585, 648);
            newPanelTab.TabIndex = 2;
            newPanelTab.Text = "Создание панели";
            newPanelTab.UseVisualStyleBackColor = true;
            // 
            // nudDWorkpieceThicknessMm
            // 
            nudDWorkpieceThicknessMm.DecimalPlaces = 1;
            nudDWorkpieceThicknessMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudDWorkpieceThicknessMm.Location = new Point(226, 169);
            nudDWorkpieceThicknessMm.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            nudDWorkpieceThicknessMm.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDWorkpieceThicknessMm.Name = "nudDWorkpieceThicknessMm";
            nudDWorkpieceThicknessMm.Size = new Size(101, 23);
            nudDWorkpieceThicknessMm.TabIndex = 10;
            nudDWorkpieceThicknessMm.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // nudDSafeHeightMm
            // 
            nudDSafeHeightMm.DecimalPlaces = 1;
            nudDSafeHeightMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudDSafeHeightMm.Location = new Point(226, 140);
            nudDSafeHeightMm.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            nudDSafeHeightMm.Minimum = new decimal(new int[] { 8, 0, 0, 65536 });
            nudDSafeHeightMm.Name = "nudDSafeHeightMm";
            nudDSafeHeightMm.Size = new Size(101, 23);
            nudDSafeHeightMm.TabIndex = 9;
            nudDSafeHeightMm.Value = new decimal(new int[] { 15, 0, 0, 65536 });
            // 
            // lblDWorkpieceThickness
            // 
            lblDWorkpieceThickness.AutoSize = true;
            lblDWorkpieceThickness.Location = new Point(0, 172);
            lblDWorkpieceThickness.Name = "lblDWorkpieceThickness";
            lblDWorkpieceThickness.Size = new Size(143, 15);
            lblDWorkpieceThickness.TabIndex = 8;
            lblDWorkpieceThickness.Text = "Толщина заготовки, мм:";
            // 
            // lblDSafeHeight
            // 
            lblDSafeHeight.AutoSize = true;
            lblDSafeHeight.Location = new Point(0, 142);
            lblDSafeHeight.Name = "lblDSafeHeight";
            lblDSafeHeight.Size = new Size(225, 15);
            lblDSafeHeight.TabIndex = 7;
            lblDSafeHeight.Text = "Безопасная высота над заготовкой, мм:";
            // 
            // pbDrilling
            // 
            pbDrilling.Location = new Point(3, 199);
            pbDrilling.Name = "pbDrilling";
            pbDrilling.Size = new Size(579, 23);
            pbDrilling.TabIndex = 6;
            // 
            // gbDrillUserInput
            // 
            gbDrillUserInput.Controls.Add(btnDInstr3);
            gbDrillUserInput.Controls.Add(lblDInstr);
            gbDrillUserInput.Controls.Add(btnDInstr2);
            gbDrillUserInput.Controls.Add(btnDInstr1);
            gbDrillUserInput.Location = new Point(3, 257);
            gbDrillUserInput.Name = "gbDrillUserInput";
            gbDrillUserInput.Size = new Size(579, 89);
            gbDrillUserInput.TabIndex = 5;
            gbDrillUserInput.TabStop = false;
            gbDrillUserInput.Text = "Инструкция";
            // 
            // btnDInstr3
            // 
            btnDInstr3.Location = new Point(168, 60);
            btnDInstr3.Name = "btnDInstr3";
            btnDInstr3.Size = new Size(75, 23);
            btnDInstr3.TabIndex = 3;
            btnDInstr3.Tag = "3";
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
            // cbDrillMainHoles
            // 
            cbDrillMainHoles.AutoSize = true;
            cbDrillMainHoles.Checked = true;
            cbDrillMainHoles.CheckState = CheckState.Checked;
            cbDrillMainHoles.Location = new Point(3, 111);
            cbDrillMainHoles.Name = "cbDrillMainHoles";
            cbDrillMainHoles.Size = new Size(194, 19);
            cbDrillMainHoles.TabIndex = 4;
            cbDrillMainHoles.Text = "Сверлить основные отверстия";
            cbDrillMainHoles.UseVisualStyleBackColor = true;
            // 
            // btnDrill
            // 
            btnDrill.Location = new Point(3, 228);
            btnDrill.Name = "btnDrill";
            btnDrill.Size = new Size(124, 23);
            btnDrill.TabIndex = 3;
            btnDrill.Text = "Создать панель";
            btnDrill.UseVisualStyleBackColor = true;
            btnDrill.Click += btnDrill_Click;
            // 
            // rbPanelPos2
            // 
            rbPanelPos2.AutoSize = true;
            rbPanelPos2.Location = new Point(3, 86);
            rbPanelPos2.Name = "rbPanelPos2";
            rbPanelPos2.Size = new Size(99, 19);
            rbPanelPos2.TabIndex = 2;
            rbPanelPos2.TabStop = true;
            rbPanelPos2.Text = "Положение 2";
            rbPanelPos2.UseVisualStyleBackColor = true;
            // 
            // rbPanelPos1
            // 
            rbPanelPos1.AutoSize = true;
            rbPanelPos1.Checked = true;
            rbPanelPos1.Location = new Point(3, 61);
            rbPanelPos1.Name = "rbPanelPos1";
            rbPanelPos1.Size = new Size(99, 19);
            rbPanelPos1.TabIndex = 1;
            rbPanelPos1.TabStop = true;
            rbPanelPos1.Text = "Положение 1";
            rbPanelPos1.UseVisualStyleBackColor = true;
            // 
            // lblQInstr1
            // 
            lblQInstr1.Location = new Point(3, 9);
            lblQInstr1.Name = "lblQInstr1";
            lblQInstr1.Size = new Size(579, 49);
            lblQInstr1.TabIndex = 0;
            lblQInstr1.Text = resources.GetString("lblQInstr1.Text");
            // 
            // existingPanelTab
            // 
            existingPanelTab.Controls.Add(cbInvertY);
            existingPanelTab.Controls.Add(cbInvertX);
            existingPanelTab.Controls.Add(pbDepanelizePreview);
            existingPanelTab.Controls.Add(btnDepanelizePreview);
            existingPanelTab.Controls.Add(pbTransforming);
            existingPanelTab.Controls.Add(gbTransformUserInput);
            existingPanelTab.Controls.Add(btnTransform);
            existingPanelTab.Controls.Add(rbDepanelize);
            existingPanelTab.Controls.Add(rbDrill);
            existingPanelTab.Controls.Add(lblTInstr1);
            existingPanelTab.Location = new Point(4, 24);
            existingPanelTab.Name = "existingPanelTab";
            existingPanelTab.Size = new Size(585, 648);
            existingPanelTab.TabIndex = 3;
            existingPanelTab.Text = "Работа с существующей панелью";
            existingPanelTab.UseVisualStyleBackColor = true;
            // 
            // cbInvertY
            // 
            cbInvertY.AutoSize = true;
            cbInvertY.Location = new Point(401, 142);
            cbInvertY.Name = "cbInvertY";
            cbInvertY.Size = new Size(66, 19);
            cbInvertY.TabIndex = 13;
            cbInvertY.Text = "Invert Y";
            cbInvertY.UseVisualStyleBackColor = true;
            cbInvertY.CheckedChanged += cbInvertY_CheckedChanged;
            // 
            // cbInvertX
            // 
            cbInvertX.AutoSize = true;
            cbInvertX.Location = new Point(329, 143);
            cbInvertX.Name = "cbInvertX";
            cbInvertX.Size = new Size(66, 19);
            cbInvertX.TabIndex = 12;
            cbInvertX.Text = "Invert X";
            cbInvertX.UseVisualStyleBackColor = true;
            cbInvertX.CheckedChanged += cbInvertX_CheckedChanged;
            // 
            // pbDepanelizePreview
            // 
            pbDepanelizePreview.Location = new Point(3, 254);
            pbDepanelizePreview.Name = "pbDepanelizePreview";
            pbDepanelizePreview.Size = new Size(390, 390);
            pbDepanelizePreview.SizeMode = PictureBoxSizeMode.StretchImage;
            pbDepanelizePreview.TabIndex = 11;
            pbDepanelizePreview.TabStop = false;
            // 
            // btnDepanelizePreview
            // 
            btnDepanelizePreview.Location = new Point(159, 139);
            btnDepanelizePreview.Name = "btnDepanelizePreview";
            btnDepanelizePreview.Size = new Size(164, 23);
            btnDepanelizePreview.TabIndex = 10;
            btnDepanelizePreview.Text = "Превью депанелизации";
            btnDepanelizePreview.UseVisualStyleBackColor = true;
            btnDepanelizePreview.Click += btnDepanelizePreview_Click;
            // 
            // pbTransforming
            // 
            pbTransforming.Location = new Point(3, 110);
            pbTransforming.Name = "pbTransforming";
            pbTransforming.Size = new Size(579, 23);
            pbTransforming.TabIndex = 9;
            // 
            // gbTransformUserInput
            // 
            gbTransformUserInput.Controls.Add(btnTInstr3);
            gbTransformUserInput.Controls.Add(lblTInstr);
            gbTransformUserInput.Controls.Add(btnTInstr2);
            gbTransformUserInput.Controls.Add(btnTInstr1);
            gbTransformUserInput.Location = new Point(3, 162);
            gbTransformUserInput.Name = "gbTransformUserInput";
            gbTransformUserInput.Size = new Size(579, 86);
            gbTransformUserInput.TabIndex = 8;
            gbTransformUserInput.TabStop = false;
            gbTransformUserInput.Text = "Инструкция";
            // 
            // btnTInstr3
            // 
            btnTInstr3.Location = new Point(168, 60);
            btnTInstr3.Name = "btnTInstr3";
            btnTInstr3.Size = new Size(75, 23);
            btnTInstr3.TabIndex = 3;
            btnTInstr3.Tag = "3";
            btnTInstr3.Text = "Кнопка 3";
            btnTInstr3.UseVisualStyleBackColor = true;
            btnTInstr3.Click += btnUserInput_Click;
            // 
            // lblTInstr
            // 
            lblTInstr.Location = new Point(6, 19);
            lblTInstr.Name = "lblTInstr";
            lblTInstr.Size = new Size(567, 38);
            lblTInstr.TabIndex = 2;
            lblTInstr.Text = "Её подробности";
            // 
            // btnTInstr2
            // 
            btnTInstr2.Location = new Point(87, 60);
            btnTInstr2.Name = "btnTInstr2";
            btnTInstr2.Size = new Size(75, 23);
            btnTInstr2.TabIndex = 1;
            btnTInstr2.Tag = "2";
            btnTInstr2.Text = "Кнопка 2";
            btnTInstr2.UseVisualStyleBackColor = true;
            btnTInstr2.Click += btnUserInput_Click;
            // 
            // btnTInstr1
            // 
            btnTInstr1.Location = new Point(6, 60);
            btnTInstr1.Name = "btnTInstr1";
            btnTInstr1.Size = new Size(75, 23);
            btnTInstr1.TabIndex = 0;
            btnTInstr1.Tag = "1";
            btnTInstr1.Text = "Кнопка 1";
            btnTInstr1.UseVisualStyleBackColor = true;
            btnTInstr1.Click += btnUserInput_Click;
            // 
            // btnTransform
            // 
            btnTransform.Location = new Point(3, 139);
            btnTransform.Name = "btnTransform";
            btnTransform.Size = new Size(150, 23);
            btnTransform.TabIndex = 7;
            btnTransform.Text = "Преобразить панель";
            btnTransform.UseVisualStyleBackColor = true;
            btnTransform.Click += btnTransform_Click;
            // 
            // rbDepanelize
            // 
            rbDepanelize.AutoSize = true;
            rbDepanelize.Location = new Point(3, 85);
            rbDepanelize.Name = "rbDepanelize";
            rbDepanelize.Size = new Size(237, 19);
            rbDepanelize.TabIndex = 2;
            rbDepanelize.TabStop = true;
            rbDepanelize.Text = "Разделить панель на отдельные платы";
            rbDepanelize.UseVisualStyleBackColor = true;
            // 
            // rbDrill
            // 
            rbDrill.AutoSize = true;
            rbDrill.Checked = true;
            rbDrill.Location = new Point(3, 60);
            rbDrill.Name = "rbDrill";
            rbDrill.Size = new Size(233, 19);
            rbDrill.TabIndex = 1;
            rbDrill.TabStop = true;
            rbDrill.Text = "Сверлить пакет многослойной платы";
            rbDrill.UseVisualStyleBackColor = true;
            // 
            // lblTInstr1
            // 
            lblTInstr1.Location = new Point(3, 8);
            lblTInstr1.Name = "lblTInstr1";
            lblTInstr1.Size = new Size(579, 49);
            lblTInstr1.TabIndex = 0;
            lblTInstr1.Text = resources.GetString("lblTInstr1.Text");
            // 
            // CNCForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1275, 697);
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
            Name = "CNCForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Механическая обработка";
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
            procTabControl.ResumeLayout(false);
            noConnectionTab.ResumeLayout(false);
            noConnectionTab.PerformLayout();
            calibTab.ResumeLayout(false);
            calibTab.PerformLayout();
            gbCalibUserInput.ResumeLayout(false);
            newPanelTab.ResumeLayout(false);
            newPanelTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudDWorkpieceThicknessMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDSafeHeightMm).EndInit();
            gbDrillUserInput.ResumeLayout(false);
            existingPanelTab.ResumeLayout(false);
            existingPanelTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbDepanelizePreview).EndInit();
            gbTransformUserInput.ResumeLayout(false);
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
        private TabControl procTabControl;
        private TabPage noConnectionTab;
        private TabPage calibTab;
        private TabPage newPanelTab;
        private Label lblAlarmStatus;
        private Label lblCalibInstr1;
        private Button btnPrintCalib;
        private Label lblCalibInstr2;
        private Button btnCalib;
        private GroupBox gbCalibUserInput;
        private Button btnInstr1;
        private Button btnInstr2;
        private Label lblCInstr;
        private Button btnContinueAfterCalib;
        private Label lblCalibInstr4;
        private TabPage existingPanelTab;
        private Label lblQInstr1;
        private RadioButton rbPanelPos1;
        private RadioButton rbPanelPos2;
        private Button btnDrill;
        private CheckBox cbDrillMainHoles;
        private GroupBox gbDrillUserInput;
        private Label lblDInstr;
        private Button btnDInstr2;
        private Button btnDInstr1;
        private ProgressBar pbDrilling;
        private Button btnDInstr3;
        private Label lblTInstr1;
        private RadioButton rbDrill;
        private RadioButton rbDepanelize;
        private ProgressBar pbTransforming;
        private GroupBox gbTransformUserInput;
        private Button btnTInstr3;
        private Label lblTInstr;
        private Button btnTInstr2;
        private Button btnTInstr1;
        private Button btnTransform;
        private Label lblDSafeHeight;
        private Label lblDWorkpieceThickness;
        private NumericUpDown nudDSafeHeightMm;
        private NumericUpDown nudDWorkpieceThicknessMm;
        private Label lblCalibInstr3;
        private RadioButton rbCalSteps;
        private RadioButton rbCalCamOffset;
        private Button btnDepanelizePreview;
        private PictureBox pbDepanelizePreview;
        private CheckBox cbInvertY;
        private CheckBox cbInvertX;
    }
}