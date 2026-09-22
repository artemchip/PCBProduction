namespace PCBProduction2
{
    partial class MainForm
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
            btnCNC = new Button();
            btnGalv = new Button();
            btnExposure = new Button();
            btnSolderpaste = new Button();
            lblGerberPath = new Label();
            tbGerberPath = new TextBox();
            btnBrowseGerber = new Button();
            lblWorkImageSize = new Label();
            nudWorkImagePhysWidthMm = new NumericUpDown();
            nudWorkImagePhysHeightMm = new NumericUpDown();
            lblX = new Label();
            nudCuttingToolSizeMm = new NumericUpDown();
            lblCuttingToolDiameter = new Label();
            lblHolderHolesIndent = new Label();
            nudHolderHolesIndentMm = new NumericUpDown();
            nudSpaceOuterMm = new NumericUpDown();
            lblSpaceOuter = new Label();
            nudGapBetweenPCBsMm = new NumericUpDown();
            lblGapBetweenPCBs = new Label();
            btnDrillQC = new Button();
            btnQC = new Button();
            label1 = new Label();
            nudAnchorHoleDiameterMm = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)nudWorkImagePhysWidthMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWorkImagePhysHeightMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCuttingToolSizeMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudHolderHolesIndentMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudSpaceOuterMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGapBetweenPCBsMm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAnchorHoleDiameterMm).BeginInit();
            SuspendLayout();
            // 
            // btnCNC
            // 
            btnCNC.Location = new Point(15, 420);
            btnCNC.Name = "btnCNC";
            btnCNC.Size = new Size(460, 32);
            btnCNC.TabIndex = 0;
            btnCNC.Text = "ЧПУ станок (начальная обработка, конечная депанелизация)";
            btnCNC.UseVisualStyleBackColor = true;
            btnCNC.Click += btnCNC_Click;
            // 
            // btnGalv
            // 
            btnGalv.Location = new Point(15, 498);
            btnGalv.Name = "btnGalv";
            btnGalv.Size = new Size(460, 32);
            btnGalv.TabIndex = 1;
            btnGalv.Text = "Металлизация переходных отверстий (перемешивание растворов)";
            btnGalv.UseVisualStyleBackColor = true;
            btnGalv.Click += btnGalv_Click;
            // 
            // btnExposure
            // 
            btnExposure.Location = new Point(15, 536);
            btnExposure.Name = "btnExposure";
            btnExposure.Size = new Size(460, 38);
            btnExposure.TabIndex = 2;
            btnExposure.Text = "Засветка фоторезиста, паяльной макси, шелкографии (фотошаблоны)";
            btnExposure.UseVisualStyleBackColor = true;
            btnExposure.Click += btnExposure_Click;
            // 
            // btnSolderpaste
            // 
            btnSolderpaste.Location = new Point(15, 580);
            btnSolderpaste.Name = "btnSolderpaste";
            btnSolderpaste.Size = new Size(460, 32);
            btnSolderpaste.TabIndex = 4;
            btnSolderpaste.Text = "Печать паяльной пасты и установка компонентов";
            btnSolderpaste.UseVisualStyleBackColor = true;
            btnSolderpaste.Click += btnSolderpaste_Click;
            // 
            // lblGerberPath
            // 
            lblGerberPath.AutoSize = true;
            lblGerberPath.Location = new Point(10, 10);
            lblGerberPath.Name = "lblGerberPath";
            lblGerberPath.Size = new Size(132, 15);
            lblGerberPath.TabIndex = 5;
            lblGerberPath.Text = "Путь к Gerber-файлам:";
            // 
            // tbGerberPath
            // 
            tbGerberPath.Location = new Point(12, 27);
            tbGerberPath.Name = "tbGerberPath";
            tbGerberPath.Size = new Size(384, 23);
            tbGerberPath.TabIndex = 6;
            tbGerberPath.Text = "D:\\Electronics\\CurrentSW\\GokeDevBoard\\GerberFiles";
            // 
            // btnBrowseGerber
            // 
            btnBrowseGerber.Location = new Point(402, 27);
            btnBrowseGerber.Name = "btnBrowseGerber";
            btnBrowseGerber.Size = new Size(75, 23);
            btnBrowseGerber.TabIndex = 7;
            btnBrowseGerber.Text = "Обзор...";
            btnBrowseGerber.UseVisualStyleBackColor = true;
            btnBrowseGerber.Click += btnBrowseGerber_Click;
            // 
            // lblWorkImageSize
            // 
            lblWorkImageSize.AutoSize = true;
            lblWorkImageSize.Location = new Point(10, 63);
            lblWorkImageSize.Name = "lblWorkImageSize";
            lblWorkImageSize.Size = new Size(279, 15);
            lblWorkImageSize.TabIndex = 8;
            lblWorkImageSize.Text = "Размер рабочей (полезной) области панели, мм:";
            // 
            // nudWorkImagePhysWidthMm
            // 
            nudWorkImagePhysWidthMm.Location = new Point(12, 80);
            nudWorkImagePhysWidthMm.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudWorkImagePhysWidthMm.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudWorkImagePhysWidthMm.Name = "nudWorkImagePhysWidthMm";
            nudWorkImagePhysWidthMm.Size = new Size(66, 23);
            nudWorkImagePhysWidthMm.TabIndex = 9;
            nudWorkImagePhysWidthMm.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // nudWorkImagePhysHeightMm
            // 
            nudWorkImagePhysHeightMm.Location = new Point(104, 80);
            nudWorkImagePhysHeightMm.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudWorkImagePhysHeightMm.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudWorkImagePhysHeightMm.Name = "nudWorkImagePhysHeightMm";
            nudWorkImagePhysHeightMm.Size = new Size(66, 23);
            nudWorkImagePhysHeightMm.TabIndex = 10;
            nudWorkImagePhysHeightMm.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // lblX
            // 
            lblX.AutoSize = true;
            lblX.Location = new Point(84, 82);
            lblX.Name = "lblX";
            lblX.Size = new Size(14, 15);
            lblX.TabIndex = 11;
            lblX.Text = "X";
            // 
            // nudCuttingToolSizeMm
            // 
            nudCuttingToolSizeMm.DecimalPlaces = 1;
            nudCuttingToolSizeMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudCuttingToolSizeMm.Location = new Point(12, 131);
            nudCuttingToolSizeMm.Maximum = new decimal(new int[] { 25, 0, 0, 65536 });
            nudCuttingToolSizeMm.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudCuttingToolSizeMm.Name = "nudCuttingToolSizeMm";
            nudCuttingToolSizeMm.Size = new Size(66, 23);
            nudCuttingToolSizeMm.TabIndex = 12;
            nudCuttingToolSizeMm.Value = new decimal(new int[] { 15, 0, 0, 65536 });
            // 
            // lblCuttingToolDiameter
            // 
            lblCuttingToolDiameter.AutoSize = true;
            lblCuttingToolDiameter.Location = new Point(10, 114);
            lblCuttingToolDiameter.Name = "lblCuttingToolDiameter";
            lblCuttingToolDiameter.Size = new Size(196, 15);
            lblCuttingToolDiameter.TabIndex = 13;
            lblCuttingToolDiameter.Text = "Размер фрезы (ширина реза), мм:";
            // 
            // lblHolderHolesIndent
            // 
            lblHolderHolesIndent.AutoSize = true;
            lblHolderHolesIndent.Location = new Point(10, 219);
            lblHolderHolesIndent.Name = "lblHolderHolesIndent";
            lblHolderHolesIndent.Size = new Size(316, 15);
            lblHolderHolesIndent.TabIndex = 14;
            lblHolderHolesIndent.Text = "От края полезной области до реперного отверстия, мм:";
            // 
            // nudHolderHolesIndentMm
            // 
            nudHolderHolesIndentMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudHolderHolesIndentMm.Location = new Point(13, 237);
            nudHolderHolesIndentMm.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            nudHolderHolesIndentMm.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            nudHolderHolesIndentMm.Name = "nudHolderHolesIndentMm";
            nudHolderHolesIndentMm.Size = new Size(66, 23);
            nudHolderHolesIndentMm.TabIndex = 15;
            nudHolderHolesIndentMm.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // nudSpaceOuterMm
            // 
            nudSpaceOuterMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudSpaceOuterMm.Location = new Point(13, 290);
            nudSpaceOuterMm.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            nudSpaceOuterMm.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            nudSpaceOuterMm.Name = "nudSpaceOuterMm";
            nudSpaceOuterMm.Size = new Size(66, 23);
            nudSpaceOuterMm.TabIndex = 17;
            nudSpaceOuterMm.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // lblSpaceOuter
            // 
            lblSpaceOuter.AutoSize = true;
            lblSpaceOuter.Location = new Point(11, 273);
            lblSpaceOuter.Name = "lblSpaceOuter";
            lblSpaceOuter.Size = new Size(255, 15);
            lblSpaceOuter.TabIndex = 16;
            lblSpaceOuter.Text = "От реперного отверстия до края панели, мм:";
            // 
            // nudGapBetweenPCBsMm
            // 
            nudGapBetweenPCBsMm.DecimalPlaces = 1;
            nudGapBetweenPCBsMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudGapBetweenPCBsMm.Location = new Point(14, 343);
            nudGapBetweenPCBsMm.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            nudGapBetweenPCBsMm.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            nudGapBetweenPCBsMm.Name = "nudGapBetweenPCBsMm";
            nudGapBetweenPCBsMm.Size = new Size(66, 23);
            nudGapBetweenPCBsMm.TabIndex = 19;
            nudGapBetweenPCBsMm.Value = new decimal(new int[] { 25, 0, 0, 65536 });
            // 
            // lblGapBetweenPCBs
            // 
            lblGapBetweenPCBs.AutoSize = true;
            lblGapBetweenPCBs.Location = new Point(12, 326);
            lblGapBetweenPCBs.Name = "lblGapBetweenPCBs";
            lblGapBetweenPCBs.Size = new Size(207, 15);
            lblGapBetweenPCBs.TabIndex = 18;
            lblGapBetweenPCBs.Text = "Зазор между платами в панели, мм:";
            // 
            // btnDrillQC
            // 
            btnDrillQC.Location = new Point(15, 458);
            btnDrillQC.Name = "btnDrillQC";
            btnDrillQC.Size = new Size(460, 32);
            btnDrillQC.TabIndex = 20;
            btnDrillQC.Text = "Контроль качества сверления";
            btnDrillQC.UseVisualStyleBackColor = true;
            btnDrillQC.Click += btnDrillQC_Click;
            // 
            // btnQC
            // 
            btnQC.Location = new Point(15, 382);
            btnQC.Name = "btnQC";
            btnQC.Size = new Size(460, 32);
            btnQC.TabIndex = 21;
            btnQC.Text = "Просмотр цепей и характеристик рисунка";
            btnQC.UseVisualStyleBackColor = true;
            btnQC.Click += btnQC_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 167);
            label1.Name = "label1";
            label1.Size = new Size(202, 15);
            label1.TabIndex = 23;
            label1.Text = "Диаметр реперного отверстия, мм:";
            // 
            // nudAnchorHoleDiameterMm
            // 
            nudAnchorHoleDiameterMm.DecimalPlaces = 1;
            nudAnchorHoleDiameterMm.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudAnchorHoleDiameterMm.Location = new Point(12, 184);
            nudAnchorHoleDiameterMm.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            nudAnchorHoleDiameterMm.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            nudAnchorHoleDiameterMm.Name = "nudAnchorHoleDiameterMm";
            nudAnchorHoleDiameterMm.Size = new Size(66, 23);
            nudAnchorHoleDiameterMm.TabIndex = 22;
            nudAnchorHoleDiameterMm.Value = new decimal(new int[] { 6, 0, 0, 65536 });
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(484, 624);
            Controls.Add(label1);
            Controls.Add(nudAnchorHoleDiameterMm);
            Controls.Add(btnQC);
            Controls.Add(btnDrillQC);
            Controls.Add(nudGapBetweenPCBsMm);
            Controls.Add(lblGapBetweenPCBs);
            Controls.Add(nudSpaceOuterMm);
            Controls.Add(lblSpaceOuter);
            Controls.Add(nudHolderHolesIndentMm);
            Controls.Add(lblHolderHolesIndent);
            Controls.Add(lblCuttingToolDiameter);
            Controls.Add(nudCuttingToolSizeMm);
            Controls.Add(lblX);
            Controls.Add(nudWorkImagePhysHeightMm);
            Controls.Add(nudWorkImagePhysWidthMm);
            Controls.Add(lblWorkImageSize);
            Controls.Add(btnBrowseGerber);
            Controls.Add(tbGerberPath);
            Controls.Add(lblGerberPath);
            Controls.Add(btnSolderpaste);
            Controls.Add(btnExposure);
            Controls.Add(btnGalv);
            Controls.Add(btnCNC);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PCBProduction2";
            FormClosing += MainForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)nudWorkImagePhysWidthMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWorkImagePhysHeightMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCuttingToolSizeMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudHolderHolesIndentMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudSpaceOuterMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGapBetweenPCBsMm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAnchorHoleDiameterMm).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCNC;
        private Button btnGalv;
        private Button btnExposure;
        private Button btnSolderpaste;
        private Label lblGerberPath;
        private TextBox tbGerberPath;
        private Button btnBrowseGerber;
        private Label lblWorkImageSize;
        private NumericUpDown nudWorkImagePhysWidthMm;
        private NumericUpDown nudWorkImagePhysHeightMm;
        private Label lblX;
        private NumericUpDown nudCuttingToolSizeMm;
        private Label lblCuttingToolDiameter;
        private Label lblHolderHolesIndent;
        private NumericUpDown nudHolderHolesIndentMm;
        private NumericUpDown nudSpaceOuterMm;
        private Label lblSpaceOuter;
        private NumericUpDown nudGapBetweenPCBsMm;
        private Label lblGapBetweenPCBs;
        private Button btnDrillQC;
        private Button btnQC;
        private Label label1;
        private NumericUpDown nudAnchorHoleDiameterMm;
    }
}