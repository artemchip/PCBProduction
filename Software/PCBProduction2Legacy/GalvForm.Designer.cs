namespace PCBProduction2
{
    partial class GalvForm
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
            gbOnline = new GroupBox();
            lblStirTime = new Label();
            btnStir = new Button();
            gbOffline = new GroupBox();
            btnGenFile = new Button();
            nudTimeMinutes = new NumericUpDown();
            lblTimeMin = new Label();
            gcodeSaveDialog = new SaveFileDialog();
            gbWhatToStir = new GroupBox();
            rbGalv = new RadioButton();
            rbChemical = new RadioButton();
            label1 = new Label();
            gbOnline.SuspendLayout();
            gbOffline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudTimeMinutes).BeginInit();
            gbWhatToStir.SuspendLayout();
            SuspendLayout();
            // 
            // gbOnline
            // 
            gbOnline.Controls.Add(lblStirTime);
            gbOnline.Controls.Add(btnStir);
            gbOnline.Location = new Point(12, 99);
            gbOnline.Name = "gbOnline";
            gbOnline.Size = new Size(381, 54);
            gbOnline.TabIndex = 0;
            gbOnline.TabStop = false;
            gbOnline.Text = "Мешалка подключена (онлайн)";
            // 
            // lblStirTime
            // 
            lblStirTime.Location = new Point(120, 24);
            lblStirTime.Name = "lblStirTime";
            lblStirTime.Size = new Size(255, 19);
            lblStirTime.TabIndex = 1;
            // 
            // btnStir
            // 
            btnStir.Location = new Point(6, 22);
            btnStir.Name = "btnStir";
            btnStir.Size = new Size(108, 23);
            btnStir.TabIndex = 0;
            btnStir.Text = "Перемешивать";
            btnStir.UseVisualStyleBackColor = true;
            btnStir.Click += btnStir_Click;
            // 
            // gbOffline
            // 
            gbOffline.Controls.Add(btnGenFile);
            gbOffline.Controls.Add(nudTimeMinutes);
            gbOffline.Controls.Add(lblTimeMin);
            gbOffline.Location = new Point(12, 159);
            gbOffline.Name = "gbOffline";
            gbOffline.Size = new Size(381, 63);
            gbOffline.TabIndex = 1;
            gbOffline.TabStop = false;
            gbOffline.Text = "Мешалка работает автономно";
            // 
            // btnGenFile
            // 
            btnGenFile.Location = new Point(175, 23);
            btnGenFile.Name = "btnGenFile";
            btnGenFile.Size = new Size(192, 23);
            btnGenFile.TabIndex = 2;
            btnGenFile.Text = "Сгенерировать файл GCode";
            btnGenFile.UseVisualStyleBackColor = true;
            btnGenFile.Click += btnGenFile_Click;
            // 
            // nudTimeMinutes
            // 
            nudTimeMinutes.Location = new Point(106, 23);
            nudTimeMinutes.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            nudTimeMinutes.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudTimeMinutes.Name = "nudTimeMinutes";
            nudTimeMinutes.Size = new Size(54, 23);
            nudTimeMinutes.TabIndex = 1;
            nudTimeMinutes.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblTimeMin
            // 
            lblTimeMin.AutoSize = true;
            lblTimeMin.Location = new Point(6, 25);
            lblTimeMin.Name = "lblTimeMin";
            lblTimeMin.Size = new Size(94, 15);
            lblTimeMin.TabIndex = 0;
            lblTimeMin.Text = "Время, минуты:";
            // 
            // gcodeSaveDialog
            // 
            gcodeSaveDialog.DefaultExt = "nc";
            gcodeSaveDialog.Filter = "GCode|*.nc";
            gcodeSaveDialog.Title = "GCode";
            // 
            // gbWhatToStir
            // 
            gbWhatToStir.Controls.Add(rbGalv);
            gbWhatToStir.Controls.Add(rbChemical);
            gbWhatToStir.Controls.Add(label1);
            gbWhatToStir.Location = new Point(12, 12);
            gbWhatToStir.Name = "gbWhatToStir";
            gbWhatToStir.Size = new Size(381, 81);
            gbWhatToStir.TabIndex = 2;
            gbWhatToStir.TabStop = false;
            gbWhatToStir.Text = "Что перемешиваем?";
            // 
            // rbGalv
            // 
            rbGalv.AutoSize = true;
            rbGalv.Location = new Point(6, 47);
            rbGalv.Name = "rbGalv";
            rbGalv.Size = new Size(281, 19);
            rbGalv.TabIndex = 89;
            rbGalv.Text = "Гальванические ванны (маятниковый привод)";
            rbGalv.UseVisualStyleBackColor = true;
            // 
            // rbChemical
            // 
            rbChemical.AutoSize = true;
            rbChemical.Checked = true;
            rbChemical.Location = new Point(6, 22);
            rbChemical.Name = "rbChemical";
            rbChemical.Size = new Size(242, 19);
            rbChemical.TabIndex = 88;
            rbChemical.TabStop = true;
            rbChemical.Text = "Химические ванны (линейный привод)";
            rbChemical.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.Location = new Point(120, 24);
            label1.Name = "label1";
            label1.Size = new Size(255, 19);
            label1.TabIndex = 1;
            // 
            // GalvForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(403, 231);
            Controls.Add(gbWhatToStir);
            Controls.Add(gbOffline);
            Controls.Add(gbOnline);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "GalvForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Металлизация отверстий";
            gbOnline.ResumeLayout(false);
            gbOffline.ResumeLayout(false);
            gbOffline.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudTimeMinutes).EndInit();
            gbWhatToStir.ResumeLayout(false);
            gbWhatToStir.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbOnline;
        private GroupBox gbOffline;
        private Label lblTimeMin;
        private Button btnGenFile;
        private NumericUpDown nudTimeMinutes;
        private Button btnStir;
        private Label lblStirTime;
        private SaveFileDialog gcodeSaveDialog;
        private GroupBox gbWhatToStir;
        private Label label1;
        private RadioButton rbGalv;
        private RadioButton rbChemical;
    }
}