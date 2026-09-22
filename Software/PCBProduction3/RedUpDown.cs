using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    class RedUpDown : NumericUpDown
    {
        public float[][] mxRed = new float[][]
        {
            new float[] { 1,  0,  0,  0,  0},
            new float[] { 1,  0,  0,  0,  0},
            new float[] { 0,  0,  0,  0,  0},
            new float[] { 0,  0,  0,  1,  0},
            new float[] { 0,  0,  0,  0,  0}
        };
        public ImageAttributes attributes = new ImageAttributes();

        public RedUpDown()
        {
            this.BackColor = Color.Black;
            this.ForeColor = Color.Red;
            attributes.SetColorMatrix(new ColorMatrix(mxRed));
        }

        [DllImport("user32")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);
        private const int WM_NCPAINT = 0x85;
        private const int WM_PAINT = 0x000F;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCPAINT)
            {
                var dc = GetWindowDC(Handle);
                using (Graphics g = Graphics.FromHdc(dc))
                {
                    g.Clear(Color.Black);
                    g.DrawRectangle(Pens.Red, 0, 0, Width - 1, Height - 1);
                }
            } else if (m.Msg == WM_PAINT)
            {
                var dc = GetWindowDC(Handle);
                using (Graphics g = Graphics.FromHdc(dc))
                {
                    g.Clear(Color.Black);
                    g.DrawRectangle(Pens.Red, 0, 0, Width - 1, Height - 1);
                }
            }
        }
    }
}
