using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    public class RedGroupBox : GroupBox
    {

        public RedGroupBox()
        {
            this.Paint += this.BorderedGroupBox_Paint;
        }

        private void BorderedGroupBox_Paint(object sender, PaintEventArgs e) =>
            DrawGroupBox(e.Graphics);

        private void DrawGroupBox(Graphics g)
        {
            Brush textBrush = new SolidBrush(this.ForeColor);
            SizeF strSize = g.MeasureString(this.Text, this.Font);

            Brush borderBrush = new SolidBrush(Color.Red);
            Pen borderPen = new Pen(borderBrush, 1);
            Rectangle rect = new Rectangle(this.ClientRectangle.X,
                                            this.ClientRectangle.Y + (int)(strSize.Height / 2),
                                            this.ClientRectangle.Width - 1,
                                            this.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

            Brush labelBrush = new SolidBrush(this.BackColor);

            // Clear text and border
            g.Clear(this.BackColor);
            g.DrawRectangle(borderPen, new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1));

            // Draw text
            if (this.Text.Length > 0)
            {
                g.FillRectangle(labelBrush, 10, 0, strSize.Width, strSize.Height);
                g.DrawString(this.Text, this.Font, textBrush, 10, 0);
            }
        }
    }
}
