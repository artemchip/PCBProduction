namespace PCBProduction2
{
    partial class DrillQCForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DrillQCForm));
            btnCalculate = new Button();
            pbPreview = new ProPictureBox();
            lblInstr1 = new Label();
            tbScanLocation = new TextBox();
            btnBrowse = new Button();
            lblInstr2 = new Label();
            lblInstr3 = new Label();
            label1 = new Label();
            label6 = new Label();
            label7 = new Label();
            nudTopLeftX = new NumericUpDown();
            nudTopLeftY = new NumericUpDown();
            nudTopRightY = new NumericUpDown();
            nudTopRightX = new NumericUpDown();
            nudBottomLeftY = new NumericUpDown();
            nudBottomLeftX = new NumericUpDown();
            nudBottomRightY = new NumericUpDown();
            nudBottomRightX = new NumericUpDown();
            btnTopLeftCursor = new Button();
            btnTopRightCursor = new Button();
            btnBottomLeftCursor = new Button();
            btnBottomRightCursor = new Button();
            label2 = new Label();
            lblDPI = new Label();
            nudDPI = new NumericUpDown();
            lblCalcResult = new Label();
            btnSaveMatrix = new Button();
            ofDialog = new OpenFileDialog();
            sfDialog = new SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)pbPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudTopLeftX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudTopLeftY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudTopRightY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudTopRightX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudBottomLeftY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudBottomLeftX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudBottomRightY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudBottomRightX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDPI).BeginInit();
            SuspendLayout();
            // 
            // btnCalculate
            // 
            btnCalculate.Location = new Point(516, 470);
            btnCalculate.Name = "btnCalculate";
            btnCalculate.Size = new Size(75, 23);
            btnCalculate.TabIndex = 0;
            btnCalculate.Text = "Расчет";
            btnCalculate.UseVisualStyleBackColor = true;
            btnCalculate.Click += button1_Click;
            // 
            // pbPreview
            // 
            pbPreview.Location = new Point(13, 107);
            pbPreview.Name = "pbPreview";
            pbPreview.Size = new Size(498, 384);
            pbPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            pbPreview.TabIndex = 1;
            pbPreview.TabStop = false;
            // 
            // lblInstr1
            // 
            lblInstr1.Location = new Point(12, 9);
            lblInstr1.Name = "lblInstr1";
            lblInstr1.Size = new Size(807, 38);
            lblInstr1.TabIndex = 2;
            lblInstr1.Text = "1. Отсканируйте заготовку после сверления. Укажите получившийся файл и его DPI. Убедитесь что ориентация паттерна та же что и в превью сверления.";
            // 
            // tbScanLocation
            // 
            tbScanLocation.Location = new Point(12, 50);
            tbScanLocation.Name = "tbScanLocation";
            tbScanLocation.ReadOnly = true;
            tbScanLocation.Size = new Size(726, 23);
            tbScanLocation.TabIndex = 3;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(744, 50);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 23);
            btnBrowse.TabIndex = 4;
            btnBrowse.Text = "Обзор...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // lblInstr2
            // 
            lblInstr2.Location = new Point(516, 107);
            lblInstr2.Name = "lblInstr2";
            lblInstr2.Size = new Size(310, 33);
            lblInstr2.TabIndex = 5;
            lblInstr2.Text = "2. Найдите реперные отверстия и укажите координаты их центров на изображении.";
            // 
            // lblInstr3
            // 
            lblInstr3.AutoSize = true;
            lblInstr3.Location = new Point(516, 149);
            lblInstr3.Name = "lblInstr3";
            lblInstr3.Size = new Size(90, 15);
            lblInstr3.TabIndex = 6;
            lblInstr3.Text = "Левое верхнее:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(516, 206);
            label1.Name = "label1";
            label1.Size = new Size(98, 15);
            label1.TabIndex = 7;
            label1.Text = "Правое верхнее:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(516, 318);
            label6.Name = "label6";
            label6.Size = new Size(96, 15);
            label6.TabIndex = 11;
            label6.Text = "Правое нижнее:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(516, 261);
            label7.Name = "label7";
            label7.Size = new Size(88, 15);
            label7.TabIndex = 10;
            label7.Text = "Левое нижнее:";
            // 
            // nudTopLeftX
            // 
            nudTopLeftX.Location = new Point(516, 167);
            nudTopLeftX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudTopLeftX.Name = "nudTopLeftX";
            nudTopLeftX.Size = new Size(84, 23);
            nudTopLeftX.TabIndex = 12;
            // 
            // nudTopLeftY
            // 
            nudTopLeftY.Location = new Point(606, 167);
            nudTopLeftY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudTopLeftY.Name = "nudTopLeftY";
            nudTopLeftY.Size = new Size(82, 23);
            nudTopLeftY.TabIndex = 13;
            // 
            // nudTopRightY
            // 
            nudTopRightY.Location = new Point(606, 224);
            nudTopRightY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudTopRightY.Name = "nudTopRightY";
            nudTopRightY.Size = new Size(82, 23);
            nudTopRightY.TabIndex = 15;
            // 
            // nudTopRightX
            // 
            nudTopRightX.Location = new Point(516, 224);
            nudTopRightX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudTopRightX.Name = "nudTopRightX";
            nudTopRightX.Size = new Size(84, 23);
            nudTopRightX.TabIndex = 14;
            // 
            // nudBottomLeftY
            // 
            nudBottomLeftY.Location = new Point(606, 279);
            nudBottomLeftY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudBottomLeftY.Name = "nudBottomLeftY";
            nudBottomLeftY.Size = new Size(82, 23);
            nudBottomLeftY.TabIndex = 17;
            // 
            // nudBottomLeftX
            // 
            nudBottomLeftX.Location = new Point(516, 279);
            nudBottomLeftX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudBottomLeftX.Name = "nudBottomLeftX";
            nudBottomLeftX.Size = new Size(84, 23);
            nudBottomLeftX.TabIndex = 16;
            // 
            // nudBottomRightY
            // 
            nudBottomRightY.Location = new Point(606, 336);
            nudBottomRightY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudBottomRightY.Name = "nudBottomRightY";
            nudBottomRightY.Size = new Size(82, 23);
            nudBottomRightY.TabIndex = 19;
            // 
            // nudBottomRightX
            // 
            nudBottomRightX.Location = new Point(516, 336);
            nudBottomRightX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudBottomRightX.Name = "nudBottomRightX";
            nudBottomRightX.Size = new Size(84, 23);
            nudBottomRightX.TabIndex = 18;
            // 
            // btnTopLeftCursor
            // 
            btnTopLeftCursor.Location = new Point(694, 167);
            btnTopLeftCursor.Name = "btnTopLeftCursor";
            btnTopLeftCursor.Size = new Size(132, 23);
            btnTopLeftCursor.TabIndex = 20;
            btnTopLeftCursor.Text = "По курсору";
            btnTopLeftCursor.UseVisualStyleBackColor = true;
            btnTopLeftCursor.Click += btnTopLeftCursor_Click;
            // 
            // btnTopRightCursor
            // 
            btnTopRightCursor.Location = new Point(694, 224);
            btnTopRightCursor.Name = "btnTopRightCursor";
            btnTopRightCursor.Size = new Size(132, 23);
            btnTopRightCursor.TabIndex = 21;
            btnTopRightCursor.Text = "По курсору";
            btnTopRightCursor.UseVisualStyleBackColor = true;
            btnTopRightCursor.Click += btnTopRightCursor_Click;
            // 
            // btnBottomLeftCursor
            // 
            btnBottomLeftCursor.Location = new Point(694, 279);
            btnBottomLeftCursor.Name = "btnBottomLeftCursor";
            btnBottomLeftCursor.Size = new Size(132, 23);
            btnBottomLeftCursor.TabIndex = 22;
            btnBottomLeftCursor.Text = "По курсору";
            btnBottomLeftCursor.UseVisualStyleBackColor = true;
            btnBottomLeftCursor.Click += btnBottomLeftCursor_Click;
            // 
            // btnBottomRightCursor
            // 
            btnBottomRightCursor.Location = new Point(694, 336);
            btnBottomRightCursor.Name = "btnBottomRightCursor";
            btnBottomRightCursor.Size = new Size(132, 23);
            btnBottomRightCursor.TabIndex = 23;
            btnBottomRightCursor.Text = "По курсору";
            btnBottomRightCursor.UseVisualStyleBackColor = true;
            btnBottomRightCursor.Click += btnBottomRightCursor_Click;
            // 
            // label2
            // 
            label2.Location = new Point(516, 372);
            label2.Name = "label2";
            label2.Size = new Size(310, 95);
            label2.TabIndex = 24;
            label2.Text = resources.GetString("label2.Text");
            // 
            // lblDPI
            // 
            lblDPI.AutoSize = true;
            lblDPI.Location = new Point(13, 81);
            lblDPI.Name = "lblDPI";
            lblDPI.Size = new Size(28, 15);
            lblDPI.TabIndex = 25;
            lblDPI.Text = "DPI:";
            // 
            // nudDPI
            // 
            nudDPI.Location = new Point(47, 79);
            nudDPI.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudDPI.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            nudDPI.Name = "nudDPI";
            nudDPI.Size = new Size(73, 23);
            nudDPI.TabIndex = 26;
            nudDPI.Value = new decimal(new int[] { 600, 0, 0, 0 });
            // 
            // lblCalcResult
            // 
            lblCalcResult.Location = new Point(12, 494);
            lblCalcResult.Name = "lblCalcResult";
            lblCalcResult.Size = new Size(814, 168);
            lblCalcResult.TabIndex = 27;
            // 
            // btnSaveMatrix
            // 
            btnSaveMatrix.Location = new Point(12, 665);
            btnSaveMatrix.Name = "btnSaveMatrix";
            btnSaveMatrix.Size = new Size(150, 23);
            btnSaveMatrix.TabIndex = 28;
            btnSaveMatrix.Text = "Сохранить матрицу";
            btnSaveMatrix.UseVisualStyleBackColor = true;
            btnSaveMatrix.Click += btnSaveMatrix_Click;
            // 
            // ofDialog
            // 
            ofDialog.FileName = "openFileDialog1";
            // 
            // DrillQCForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(831, 695);
            Controls.Add(btnSaveMatrix);
            Controls.Add(lblCalcResult);
            Controls.Add(nudDPI);
            Controls.Add(lblDPI);
            Controls.Add(label2);
            Controls.Add(btnBottomRightCursor);
            Controls.Add(btnBottomLeftCursor);
            Controls.Add(btnTopRightCursor);
            Controls.Add(btnTopLeftCursor);
            Controls.Add(nudBottomRightY);
            Controls.Add(nudBottomRightX);
            Controls.Add(nudBottomLeftY);
            Controls.Add(nudBottomLeftX);
            Controls.Add(nudTopRightY);
            Controls.Add(nudTopRightX);
            Controls.Add(nudTopLeftY);
            Controls.Add(nudTopLeftX);
            Controls.Add(label6);
            Controls.Add(label7);
            Controls.Add(label1);
            Controls.Add(lblInstr3);
            Controls.Add(lblInstr2);
            Controls.Add(btnBrowse);
            Controls.Add(tbScanLocation);
            Controls.Add(lblInstr1);
            Controls.Add(pbPreview);
            Controls.Add(btnCalculate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "DrillQCForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Контроль качества сверления";
            Load += DrillQCForm_Load;
            ((System.ComponentModel.ISupportInitialize)pbPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudTopLeftX).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudTopLeftY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudTopRightY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudTopRightX).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudBottomLeftY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudBottomLeftX).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudBottomRightY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudBottomRightX).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDPI).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCalculate;
        private ProPictureBox pbPreview;
        private Label lblInstr1;
        private TextBox tbScanLocation;
        private Button btnBrowse;
        private Label lblInstr2;
        private Label lblInstr3;
        private Label label1;
        private Label label6;
        private Label label7;
        private NumericUpDown nudTopLeftX;
        private NumericUpDown nudTopLeftY;
        private NumericUpDown nudTopRightY;
        private NumericUpDown nudTopRightX;
        private NumericUpDown nudBottomLeftY;
        private NumericUpDown nudBottomLeftX;
        private NumericUpDown nudBottomRightY;
        private NumericUpDown nudBottomRightX;
        private Button btnTopLeftCursor;
        private Button btnTopRightCursor;
        private Button btnBottomLeftCursor;
        private Button btnBottomRightCursor;
        private Label label2;
        private Label lblDPI;
        private NumericUpDown nudDPI;
        private Label lblCalcResult;
        private Button btnSaveMatrix;
        private OpenFileDialog ofDialog;
        private SaveFileDialog sfDialog;
    }
}