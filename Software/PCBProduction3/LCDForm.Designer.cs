namespace PCBProduction3
{
    partial class LCDForm
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
            components = new System.ComponentModel.Container();
            pbExpImg = new PictureBox();
            timerDisplayResolution = new System.Windows.Forms.Timer(components);
            gbControls = new RedGroupBox();
            lblAutoAnchors = new Label();
            pbPreview4 = new CamPictureBox();
            pbPreview3 = new CamPictureBox();
            lblAnchors = new Label();
            lblHotkeys = new Label();
            lblExpTime = new Label();
            lblTimerTitle = new Label();
            pbPreview2 = new CamPictureBox();
            pbPreview1 = new CamPictureBox();
            expTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pbExpImg).BeginInit();
            gbControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbPreview4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbPreview3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbPreview2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbPreview1).BeginInit();
            SuspendLayout();
            // 
            // pbExpImg
            // 
            pbExpImg.Location = new Point(814, 222);
            pbExpImg.Name = "pbExpImg";
            pbExpImg.Size = new Size(100, 50);
            pbExpImg.SizeMode = PictureBoxSizeMode.StretchImage;
            pbExpImg.TabIndex = 0;
            pbExpImg.TabStop = false;
            // 
            // timerDisplayResolution
            // 
            timerDisplayResolution.Enabled = true;
            timerDisplayResolution.Interval = 1000;
            timerDisplayResolution.Tick += timerDisplayResolution_Tick;
            // 
            // gbControls
            // 
            gbControls.Controls.Add(lblAutoAnchors);
            gbControls.Controls.Add(pbPreview4);
            gbControls.Controls.Add(pbPreview3);
            gbControls.Controls.Add(lblAnchors);
            gbControls.Controls.Add(lblHotkeys);
            gbControls.Controls.Add(lblExpTime);
            gbControls.Controls.Add(lblTimerTitle);
            gbControls.Controls.Add(pbPreview2);
            gbControls.Controls.Add(pbPreview1);
            gbControls.ForeColor = Color.Red;
            gbControls.Location = new Point(12, 12);
            gbControls.Name = "gbControls";
            gbControls.Size = new Size(1283, 1004);
            gbControls.TabIndex = 1;
            gbControls.TabStop = false;
            gbControls.Text = "Управление";
            // 
            // lblAutoAnchors
            // 
            lblAutoAnchors.Location = new Point(640, 844);
            lblAutoAnchors.Name = "lblAutoAnchors";
            lblAutoAnchors.Size = new Size(628, 144);
            lblAutoAnchors.TabIndex = 43;
            lblAutoAnchors.Text = "AutoAnchors";
            // 
            // pbPreview4
            // 
            pbPreview4.Location = new Point(640, 376);
            pbPreview4.Name = "pbPreview4";
            pbPreview4.Size = new Size(628, 334);
            pbPreview4.SizeMode = PictureBoxSizeMode.CenterImage;
            pbPreview4.TabIndex = 42;
            pbPreview4.TabStop = false;
            pbPreview4.Tag = "4";
            // 
            // pbPreview3
            // 
            pbPreview3.Location = new Point(640, 22);
            pbPreview3.Name = "pbPreview3";
            pbPreview3.Size = new Size(628, 348);
            pbPreview3.SizeMode = PictureBoxSizeMode.CenterImage;
            pbPreview3.TabIndex = 41;
            pbPreview3.TabStop = false;
            pbPreview3.Tag = "3";
            // 
            // lblAnchors
            // 
            lblAnchors.Location = new Point(6, 844);
            lblAnchors.Name = "lblAnchors";
            lblAnchors.Size = new Size(628, 157);
            lblAnchors.TabIndex = 40;
            lblAnchors.Text = "Anchors";
            // 
            // lblHotkeys
            // 
            lblHotkeys.Location = new Point(6, 713);
            lblHotkeys.Name = "lblHotkeys";
            lblHotkeys.Size = new Size(1262, 70);
            lblHotkeys.TabIndex = 39;
            lblHotkeys.Text = "Нажмите 1,2,3 или 4 чтобы выбрать реперное отверстие.\r\nНажимайте WASD чтобы перемещать его вручную.\r\nНажмите 5 чтобы запустить/остановить таймер засветки.\r\nНажмите 6 чтобы закрыть это окно.";
            // 
            // lblExpTime
            // 
            lblExpTime.AutoSize = true;
            lblExpTime.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblExpTime.ForeColor = Color.Red;
            lblExpTime.Location = new Point(6, 809);
            lblExpTime.Name = "lblExpTime";
            lblExpTime.Size = new Size(71, 32);
            lblExpTime.TabIndex = 24;
            lblExpTime.Text = "00:00";
            // 
            // lblTimerTitle
            // 
            lblTimerTitle.AutoSize = true;
            lblTimerTitle.ForeColor = Color.Red;
            lblTimerTitle.Location = new Point(6, 794);
            lblTimerTitle.Name = "lblTimerTitle";
            lblTimerTitle.Size = new Size(102, 15);
            lblTimerTitle.TabIndex = 23;
            lblTimerTitle.Text = "Таймер засветки:";
            // 
            // pbPreview2
            // 
            pbPreview2.Location = new Point(6, 376);
            pbPreview2.Name = "pbPreview2";
            pbPreview2.Size = new Size(628, 334);
            pbPreview2.SizeMode = PictureBoxSizeMode.CenterImage;
            pbPreview2.TabIndex = 22;
            pbPreview2.TabStop = false;
            pbPreview2.Tag = "2";
            // 
            // pbPreview1
            // 
            pbPreview1.Location = new Point(6, 22);
            pbPreview1.Name = "pbPreview1";
            pbPreview1.Size = new Size(628, 348);
            pbPreview1.SizeMode = PictureBoxSizeMode.CenterImage;
            pbPreview1.TabIndex = 20;
            pbPreview1.TabStop = false;
            pbPreview1.Tag = "1";
            // 
            // expTimer
            // 
            expTimer.Interval = 1000;
            expTimer.Tick += expTimer_Tick;
            // 
            // LCDForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Black;
            ClientSize = new Size(1307, 1028);
            Controls.Add(gbControls);
            Controls.Add(pbExpImg);
            FormBorderStyle = FormBorderStyle.None;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LCDForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "LCDForm";
            FormClosing += LCDForm_FormClosing;
            Shown += LCDForm_Shown;
            KeyDown += LCDForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)pbExpImg).EndInit();
            gbControls.ResumeLayout(false);
            gbControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbPreview4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbPreview3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbPreview2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbPreview1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pbExpImg;
        private System.Windows.Forms.Timer timerDisplayResolution;
        private RedGroupBox gbControls;
        private Label lblExpTime;
        private Label lblTimerTitle;
        private CamPictureBox pbPreview2;
        private CamPictureBox pbPreview1;
        private System.Windows.Forms.Timer expTimer;
        private Label lblHotkeys;
        private Label lblAnchors;
        private CamPictureBox pbPreview4;
        private CamPictureBox pbPreview3;
        private Label lblAutoAnchors;
    }
}