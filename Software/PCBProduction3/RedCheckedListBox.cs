using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    public class RedCheckedListBox : CheckedListBox
    {
        public RedCheckedListBox()
        {
            DoubleBuffered = true;
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index > (Items.Count - 1))
            {
                return;
            }
            e.Graphics.FillRectangle((SelectedIndex == e.Index) ? Brushes.Red : Brushes.Black, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
            e.Graphics.DrawRectangle(Pens.Red, e.Bounds.X, e.Bounds.Y, e.Bounds.Height - 1, e.Bounds.Height - 1);
            e.Graphics.FillRectangle(Brushes.Black, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Height - 2, e.Bounds.Height - 2);
            if (GetItemChecked(e.Index))
            {
                e.Graphics.FillRectangle(Brushes.Red, e.Bounds.X + 5, e.Bounds.Y + 5, e.Bounds.Height - 10, e.Bounds.Height - 10);
            }
            using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center })
            {
                using (Brush brush = new SolidBrush((SelectedIndex == e.Index) ? Color.Black : Color.Red))
                {
                    e.Graphics.DrawString(Items[e.Index].ToString(), Font, brush, new Rectangle(e.Bounds.Height, e.Bounds.Top, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height), sf);
                }
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSelectedIndexChanged(e);
        }
    }
}
