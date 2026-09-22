namespace PCBProduction3
{
    partial class MainForm
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
            btnCNCPre = new Button();
            lblGerberPath = new Label();
            tbGerberPath = new TextBox();
            btnExposure = new Button();
            btnQC = new Button();
            btnPNP = new Button();
            btnCNCPost = new Button();
            btnElectroplating = new Button();
            SuspendLayout();
            // 
            // btnCNCPre
            // 
            btnCNCPre.Location = new Point(12, 56);
            btnCNCPre.Name = "btnCNCPre";
            btnCNCPre.Size = new Size(466, 29);
            btnCNCPre.TabIndex = 0;
            btnCNCPre.Text = "ЧПУ (сверление, вырезание)";
            btnCNCPre.UseVisualStyleBackColor = true;
            btnCNCPre.Click += btnCNCPre_Click;
            // 
            // lblGerberPath
            // 
            lblGerberPath.AutoSize = true;
            lblGerberPath.Location = new Point(12, 9);
            lblGerberPath.Name = "lblGerberPath";
            lblGerberPath.Size = new Size(200, 15);
            lblGerberPath.TabIndex = 1;
            lblGerberPath.Text = "Папка с файлами проекта (Gerber):";
            // 
            // tbGerberPath
            // 
            tbGerberPath.Location = new Point(12, 27);
            tbGerberPath.Name = "tbGerberPath";
            tbGerberPath.Size = new Size(466, 23);
            tbGerberPath.TabIndex = 2;
            tbGerberPath.Text = "C:\\Users\\ArtemChip\\Documents\\EAGLE\\projects\\MetalTest\\GerberFiles";
            // 
            // btnExposure
            // 
            btnExposure.Location = new Point(12, 161);
            btnExposure.Name = "btnExposure";
            btnExposure.Size = new Size(466, 29);
            btnExposure.TabIndex = 4;
            btnExposure.Text = "Засветка фоторезиста/паяльной маски";
            btnExposure.UseVisualStyleBackColor = true;
            btnExposure.Click += btnExposure_Click;
            // 
            // btnQC
            // 
            btnQC.Location = new Point(9, 196);
            btnQC.Name = "btnQC";
            btnQC.Size = new Size(469, 29);
            btnQC.TabIndex = 5;
            btnQC.Text = "Контроль качества";
            btnQC.UseVisualStyleBackColor = true;
            btnQC.Click += btnQC_Click;
            // 
            // btnPNP
            // 
            btnPNP.Location = new Point(9, 231);
            btnPNP.Name = "btnPNP";
            btnPNP.Size = new Size(469, 29);
            btnPNP.TabIndex = 6;
            btnPNP.Text = "Pick&Place";
            btnPNP.UseVisualStyleBackColor = true;
            btnPNP.Click += btnPNP_Click;
            // 
            // btnCNCPost
            // 
            btnCNCPost.Location = new Point(12, 91);
            btnCNCPost.Name = "btnCNCPost";
            btnCNCPost.Size = new Size(466, 29);
            btnCNCPost.TabIndex = 7;
            btnCNCPost.Text = "ЧПУ (сверление прессованного пакета, депанелизация)";
            btnCNCPost.UseVisualStyleBackColor = true;
            btnCNCPost.Click += btnCNCPost_Click;
            // 
            // btnElectroplating
            // 
            btnElectroplating.Location = new Point(12, 126);
            btnElectroplating.Name = "btnElectroplating";
            btnElectroplating.Size = new Size(466, 29);
            btnElectroplating.TabIndex = 8;
            btnElectroplating.Text = "Металлизация переходных отверстий";
            btnElectroplating.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(487, 273);
            Controls.Add(btnElectroplating);
            Controls.Add(btnCNCPost);
            Controls.Add(btnPNP);
            Controls.Add(btnQC);
            Controls.Add(btnExposure);
            Controls.Add(tbGerberPath);
            Controls.Add(lblGerberPath);
            Controls.Add(btnCNCPre);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PCBProduction3";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCNCPre;
        private Label lblGerberPath;
        private TextBox tbGerberPath;
        private Button btnExposure;
        private Button btnQC;
        private Button btnPNP;
        private Button btnCNCPost;
        private Button btnElectroplating;
    }
}
