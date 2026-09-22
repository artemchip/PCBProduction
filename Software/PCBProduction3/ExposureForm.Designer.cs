namespace PCBProduction3
{
    partial class ExposureForm
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
            pbPreview = new PictureBox();
            cbPositiveExposure = new RedCheckBox();
            lblRenderDpi = new Label();
            nudRenderDpi = new RedUpDown();
            gbControls = new RedGroupBox();
            btnExpose = new RedButton();
            lblWhiteCount = new Label();
            tbBottomText = new RedTextBox();
            lblBotText = new Label();
            lblTopText = new Label();
            tbTopText = new RedTextBox();
            clbLayers = new RedCheckedListBox();
            ofDialog = new OpenFileDialog();
            btnCloseWin = new RedButton();
            mainGroupBox = new RedGroupBox();
            imgSaveDialog = new SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)pbPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRenderDpi).BeginInit();
            gbControls.SuspendLayout();
            mainGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // pbPreview
            // 
            pbPreview.Location = new Point(6, 11);
            pbPreview.Name = "pbPreview";
            pbPreview.Size = new Size(600, 600);
            pbPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            pbPreview.TabIndex = 0;
            pbPreview.TabStop = false;
            // 
            // cbPositiveExposure
            // 
            cbPositiveExposure.AutoSize = true;
            cbPositiveExposure.BackColor = Color.Black;
            cbPositiveExposure.FlatStyle = FlatStyle.Flat;
            cbPositiveExposure.ForeColor = Color.Red;
            cbPositiveExposure.Location = new Point(5, 543);
            cbPositiveExposure.Name = "cbPositiveExposure";
            cbPositiveExposure.Size = new Size(137, 19);
            cbPositiveExposure.TabIndex = 2;
            cbPositiveExposure.Text = "Позитивная засветка";
            cbPositiveExposure.UseVisualStyleBackColor = true;
            cbPositiveExposure.CheckedChanged += cbPositiveExposure_CheckedChanged;
            // 
            // lblRenderDpi
            // 
            lblRenderDpi.AutoSize = true;
            lblRenderDpi.Location = new Point(5, 382);
            lblRenderDpi.Name = "lblRenderDpi";
            lblRenderDpi.Size = new Size(102, 15);
            lblRenderDpi.TabIndex = 7;
            lblRenderDpi.Text = "Разрешение, DPI:";
            // 
            // nudRenderDpi
            // 
            nudRenderDpi.BackColor = Color.Black;
            nudRenderDpi.ForeColor = Color.Red;
            nudRenderDpi.Location = new Point(5, 400);
            nudRenderDpi.Maximum = new decimal(new int[] { 6000, 0, 0, 0 });
            nudRenderDpi.Minimum = new decimal(new int[] { 300, 0, 0, 0 });
            nudRenderDpi.Name = "nudRenderDpi";
            nudRenderDpi.Size = new Size(78, 23);
            nudRenderDpi.TabIndex = 6;
            nudRenderDpi.Value = new decimal(new int[] { 300, 0, 0, 0 });
            nudRenderDpi.ValueChanged += nudRenderDpi_ValueChanged;
            // 
            // gbControls
            // 
            gbControls.Controls.Add(btnExpose);
            gbControls.Controls.Add(lblWhiteCount);
            gbControls.Controls.Add(tbBottomText);
            gbControls.Controls.Add(lblBotText);
            gbControls.Controls.Add(lblTopText);
            gbControls.Controls.Add(tbTopText);
            gbControls.Controls.Add(clbLayers);
            gbControls.Controls.Add(lblRenderDpi);
            gbControls.Controls.Add(nudRenderDpi);
            gbControls.Controls.Add(cbPositiveExposure);
            gbControls.ForeColor = Color.Red;
            gbControls.Location = new Point(612, 11);
            gbControls.Name = "gbControls";
            gbControls.Size = new Size(268, 600);
            gbControls.TabIndex = 8;
            gbControls.TabStop = false;
            gbControls.Text = "Параметры";
            // 
            // btnExpose
            // 
            btnExpose.BackColor = Color.Black;
            btnExpose.FlatStyle = FlatStyle.Flat;
            btnExpose.ForeColor = Color.Red;
            btnExpose.Location = new Point(5, 569);
            btnExpose.Name = "btnExpose";
            btnExpose.Size = new Size(256, 23);
            btnExpose.TabIndex = 21;
            btnExpose.Text = "Засветить...";
            btnExpose.UseVisualStyleBackColor = true;
            btnExpose.Click += redButton1_Click;
            // 
            // lblWhiteCount
            // 
            lblWhiteCount.Location = new Point(6, 345);
            lblWhiteCount.Name = "lblWhiteCount";
            lblWhiteCount.Size = new Size(255, 28);
            lblWhiteCount.TabIndex = 19;
            // 
            // tbBottomText
            // 
            tbBottomText.BackColor = Color.Black;
            tbBottomText.BorderStyle = BorderStyle.FixedSingle;
            tbBottomText.ForeColor = Color.Red;
            tbBottomText.Location = new Point(5, 514);
            tbBottomText.Name = "tbBottomText";
            tbBottomText.Size = new Size(256, 23);
            tbBottomText.TabIndex = 13;
            tbBottomText.Text = "Bottom";
            tbBottomText.Leave += tbBottomText1_Leave;
            // 
            // lblBotText
            // 
            lblBotText.AutoSize = true;
            lblBotText.Location = new Point(5, 496);
            lblBotText.Name = "lblBotText";
            lblBotText.Size = new Size(91, 15);
            lblBotText.TabIndex = 12;
            lblBotText.Text = "Текст BOTTOM:";
            // 
            // lblTopText
            // 
            lblTopText.AutoSize = true;
            lblTopText.Location = new Point(5, 440);
            lblTopText.Name = "lblTopText";
            lblTopText.Size = new Size(65, 15);
            lblTopText.TabIndex = 11;
            lblTopText.Text = "Текст TOP:";
            // 
            // tbTopText
            // 
            tbTopText.BackColor = Color.Black;
            tbTopText.BorderStyle = BorderStyle.FixedSingle;
            tbTopText.ForeColor = Color.Red;
            tbTopText.Location = new Point(5, 458);
            tbTopText.Name = "tbTopText";
            tbTopText.Size = new Size(256, 23);
            tbTopText.TabIndex = 9;
            tbTopText.Text = "Top";
            tbTopText.Leave += tbTopText1_Leave;
            // 
            // clbLayers
            // 
            clbLayers.BackColor = Color.Black;
            clbLayers.BorderStyle = BorderStyle.None;
            clbLayers.ForeColor = Color.Red;
            clbLayers.FormattingEnabled = true;
            clbLayers.Location = new Point(6, 22);
            clbLayers.Name = "clbLayers";
            clbLayers.Size = new Size(256, 324);
            clbLayers.TabIndex = 8;
            clbLayers.SelectedIndexChanged += clbLayers_SelectedIndexChanged;
            // 
            // ofDialog
            // 
            ofDialog.FileName = "openFileDialog1";
            // 
            // btnCloseWin
            // 
            btnCloseWin.BackColor = Color.Black;
            btnCloseWin.FlatStyle = FlatStyle.Flat;
            btnCloseWin.ForeColor = Color.Red;
            btnCloseWin.Location = new Point(886, 11);
            btnCloseWin.Name = "btnCloseWin";
            btnCloseWin.Size = new Size(33, 33);
            btnCloseWin.TabIndex = 12;
            btnCloseWin.Text = "X";
            btnCloseWin.UseVisualStyleBackColor = true;
            btnCloseWin.Click += btnCloseWin_Click;
            // 
            // mainGroupBox
            // 
            mainGroupBox.Controls.Add(btnCloseWin);
            mainGroupBox.Controls.Add(pbPreview);
            mainGroupBox.Controls.Add(gbControls);
            mainGroupBox.ForeColor = Color.Red;
            mainGroupBox.Location = new Point(3, 1);
            mainGroupBox.Name = "mainGroupBox";
            mainGroupBox.Size = new Size(931, 627);
            mainGroupBox.TabIndex = 19;
            mainGroupBox.TabStop = false;
            // 
            // imgSaveDialog
            // 
            imgSaveDialog.DefaultExt = "png";
            imgSaveDialog.Filter = "Файлы PNG|*.png";
            imgSaveDialog.RestoreDirectory = true;
            imgSaveDialog.Title = "Сохранить фотошаблон";
            // 
            // ExposureForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Black;
            ClientSize = new Size(937, 631);
            Controls.Add(mainGroupBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "ExposureForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Засветка фоторезиста и маски";
            FormClosing += ExposureForm_FormClosing;
            Shown += ExposureForm_Shown;
            ((System.ComponentModel.ISupportInitialize)pbPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRenderDpi).EndInit();
            gbControls.ResumeLayout(false);
            gbControls.PerformLayout();
            mainGroupBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pbPreview;
        private RedCheckBox cbPositiveExposure;
        private Label lblRenderDpi;
        private RedUpDown nudRenderDpi;
        private RedGroupBox gbControls;
        private RedCheckedListBox clbLayers;
        private RedTextBox tbTopText;
        private Label lblTopText;
        private Label lblBotText;
        private RedTextBox tbBottomText;
        private OpenFileDialog ofDialog;
        private RedButton btnCloseWin;
        private RedGroupBox mainGroupBox;
        private Label lblWhiteCount;
        private SaveFileDialog imgSaveDialog;
        private RedButton btnExpose;
    }
}