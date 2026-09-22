using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBProduction3
{
    public partial class LCDParamsForm : Form
    {
        public int pxWidthUm = 19;
        public int pxHeightUm = 24;
        public bool gotResult = false;

        public LCDParamsForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            pxWidthUm = (int)nudPxWidthUm.Value;
            pxHeightUm = (int)nudPxHeightUm.Value;
            if (((float)pxWidthUm / (float)pxHeightUm) < 0.5f || ((float)pxWidthUm / (float)pxHeightUm) > 2f)
            {
                MessageBox.Show(this, "У вашей матрицы слишком прямоугольные пиксели.");
                return;
            }
            gotResult = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void LCDParamsForm_Shown(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            gotResult = false;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void LCDParamsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!gotResult)
            {
                DialogResult = DialogResult.Cancel;
            }
        }
    }
}
