namespace PCBProduction2
{
    partial class QCForm
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
            pbPCB = new QCPictureBox();
            lbNets = new ListBox();
            lblHoleTentStatus = new Label();
            cbShowErrors = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pbPCB).BeginInit();
            SuspendLayout();
            // 
            // pbPCB
            // 
            pbPCB.Location = new Point(12, 12);
            pbPCB.Name = "pbPCB";
            pbPCB.Size = new Size(540, 540);
            pbPCB.TabIndex = 0;
            pbPCB.TabStop = false;
            // 
            // lbNets
            // 
            lbNets.FormattingEnabled = true;
            lbNets.ItemHeight = 15;
            lbNets.Location = new Point(558, 12);
            lbNets.Name = "lbNets";
            lbNets.Size = new Size(236, 544);
            lbNets.TabIndex = 1;
            lbNets.SelectedIndexChanged += lbNets_SelectedIndexChanged;
            // 
            // lblHoleTentStatus
            // 
            lblHoleTentStatus.AutoSize = true;
            lblHoleTentStatus.Location = new Point(12, 565);
            lblHoleTentStatus.Name = "lblHoleTentStatus";
            lblHoleTentStatus.Size = new Size(0, 15);
            lblHoleTentStatus.TabIndex = 2;
            // 
            // cbShowErrors
            // 
            cbShowErrors.AutoSize = true;
            cbShowErrors.Location = new Point(12, 583);
            cbShowErrors.Name = "cbShowErrors";
            cbShowErrors.Size = new Size(124, 19);
            cbShowErrors.TabIndex = 3;
            cbShowErrors.Text = "Показать ошибки";
            cbShowErrors.UseVisualStyleBackColor = true;
            cbShowErrors.CheckedChanged += cbShowErrors_CheckedChanged;
            // 
            // QCForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(806, 608);
            Controls.Add(cbShowErrors);
            Controls.Add(lblHoleTentStatus);
            Controls.Add(lbNets);
            Controls.Add(pbPCB);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "QCForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Контроль качества (проводимости)";
            Shown += QCForm_Shown;
            ((System.ComponentModel.ISupportInitialize)pbPCB).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private QCPictureBox pbPCB;
        private ListBox lbNets;
        private Label lblHoleTentStatus;
        private CheckBox cbShowErrors;
    }
}