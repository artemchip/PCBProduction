using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    class RedCheckBox : CheckBox
    {
        public RedCheckBox()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.Black;
            this.ForeColor = Color.Red;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.Clear(this.BackColor);
            pevent.Graphics.SmoothingMode = SmoothingMode.None;
            using (Pen penBorder = new Pen(Color.Red, 1))
            {
                penBorder.Alignment = PenAlignment.Inset;
                pevent.Graphics.DrawRectangle(penBorder, 0, 0, this.Height - 1, this.Height - 1);
                if (this.Checked)
                {
                    pevent.Graphics.FillRectangle(Brushes.Red, 5, 5, this.Height - 10, this.Height - 10);
                }
                pevent.Graphics.DrawString(this.Text, this.Font, Brushes.Red, this.Height + 3, 0);
            }
        }
    }
}
