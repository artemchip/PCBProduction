namespace PCBProduction3
{
    partial class LCDParamsForm
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
            btnOK = new RedButton();
            btnCancel = new RedButton();
            nudPxWidthUm = new RedUpDown();
            nudPxHeightUm = new RedUpDown();
            lblPxWidth = new Label();
            lblPxHeight = new Label();
            ((System.ComponentModel.ISupportInitialize)nudPxWidthUm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPxHeightUm).BeginInit();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.BackColor = Color.Black;
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.ForeColor = Color.Red;
            btnOK.Location = new Point(12, 83);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(96, 26);
            btnOK.TabIndex = 0;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = false;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Black;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.ForeColor = Color.Red;
            btnCancel.Location = new Point(157, 83);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(96, 26);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // nudPxWidthUm
            // 
            nudPxWidthUm.BackColor = Color.Black;
            nudPxWidthUm.ForeColor = Color.Red;
            nudPxWidthUm.Location = new Point(161, 12);
            nudPxWidthUm.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            nudPxWidthUm.Name = "nudPxWidthUm";
            nudPxWidthUm.Size = new Size(92, 23);
            nudPxWidthUm.TabIndex = 2;
            nudPxWidthUm.Value = new decimal(new int[] { 19, 0, 0, 0 });
            // 
            // nudPxHeightUm
            // 
            nudPxHeightUm.BackColor = Color.Black;
            nudPxHeightUm.ForeColor = Color.Red;
            nudPxHeightUm.Location = new Point(161, 41);
            nudPxHeightUm.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            nudPxHeightUm.Name = "nudPxHeightUm";
            nudPxHeightUm.Size = new Size(92, 23);
            nudPxHeightUm.TabIndex = 3;
            nudPxHeightUm.Value = new decimal(new int[] { 24, 0, 0, 0 });
            // 
            // lblPxWidth
            // 
            lblPxWidth.AutoSize = true;
            lblPxWidth.ForeColor = Color.Red;
            lblPxWidth.Location = new Point(12, 14);
            lblPxWidth.Name = "lblPxWidth";
            lblPxWidth.Size = new Size(133, 15);
            lblPxWidth.TabIndex = 4;
            lblPxWidth.Text = "Ширина пикселя, мкм:";
            // 
            // lblPxHeight
            // 
            lblPxHeight.AutoSize = true;
            lblPxHeight.ForeColor = Color.Red;
            lblPxHeight.Location = new Point(12, 43);
            lblPxHeight.Name = "lblPxHeight";
            lblPxHeight.Size = new Size(128, 15);
            lblPxHeight.TabIndex = 5;
            lblPxHeight.Text = "Высота пикселя, мкм:";
            // 
            // LCDParamsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(272, 121);
            Controls.Add(lblPxHeight);
            Controls.Add(lblPxWidth);
            Controls.Add(nudPxHeightUm);
            Controls.Add(nudPxWidthUm);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LCDParamsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Параметры LCD матрицы";
            FormClosing += LCDParamsForm_FormClosing;
            Shown += LCDParamsForm_Shown;
            ((System.ComponentModel.ISupportInitialize)nudPxWidthUm).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPxHeightUm).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RedButton btnOK;
        private RedButton btnCancel;
        private RedUpDown nudPxWidthUm;
        private RedUpDown nudPxHeightUm;
        private Label lblPxWidth;
        private Label lblPxHeight;
    }
}