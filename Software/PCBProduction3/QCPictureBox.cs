using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    public class QCPictureBox : PictureBox
    {
        public Color TopColor;
        public Color BottomColor;
        public Color[] InnerColors;
        public Bitmap? HoleFailureMap;
        public bool ShowErrors;
        public List<Bitmap> ImagesForEachLayer;
        public QCNet? SelectedNet;

        public QCPictureBox()
        {
            TopColor = Color.Red;
            BottomColor = Color.Blue;
            InnerColors = new Color[] { Color.Brown, Color.Yellow, Color.Orange, Color.Violet, Color.Gray, Color.Fuchsia };
            ImagesForEachLayer = new List<Bitmap>();
            HoleFailureMap = null;
            ShowErrors = false;
        }

        private Color GetCurLayerColorByIndex(int i)
        {
            Color curLayerColor = Color.White;
            if (i == 0)
            {
                curLayerColor = TopColor;
            }
            else if (i >= (ImagesForEachLayer.Count - 1))
            {
                curLayerColor = BottomColor;
            }
            else
            {
                try
                {
                    curLayerColor = InnerColors[i - 1];
                }
                catch { }
            }
            return curLayerColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ImagesForEachLayer == null) return;
            if (ImagesForEachLayer.Count < 1) return;
            e.Graphics.Clear(Color.Black);
            float scale = Math.Min((float)this.Width / (float)ImagesForEachLayer[0].Width, (float)this.Height / (float)ImagesForEachLayer[0].Height);
            int offsetX = (int)((this.Width / 2f) - (ImagesForEachLayer[0].Width * scale / 2f));
            int offsetY = (int)((this.Height / 2f) - (ImagesForEachLayer[0].Height * scale / 2f));
            for (int i = ImagesForEachLayer.Count - 1; i >= 0; i--)
            {
                ImageAttributes attr = new ImageAttributes();
                Color curLayerColor = GetCurLayerColorByIndex(i);
                var mxCur = new float[][]
                {
                    new float[] { (curLayerColor.R / 255.0f),  0,  0,  0,  0},
                    new float[] { 0,  (curLayerColor.G / 255.0f),  0,  0,  0},
                    new float[] { 0,  0,  (curLayerColor.B / 255.0f),  0,  0},
                    new float[] { 0,  0,  0,  0.5f,  0},
                    new float[] { 0,  0,  0,  0,  0}
                };
                attr.SetColorMatrix(new ColorMatrix(mxCur));
                e.Graphics.DrawImage(ImagesForEachLayer[i], new Rectangle(offsetX, offsetY, (int)(ImagesForEachLayer[i].Width * scale), (int)(ImagesForEachLayer[i].Height * scale)), 0, 0, ImagesForEachLayer[i].Width, ImagesForEachLayer[i].Height, GraphicsUnit.Pixel, attr);
            }
            if (SelectedNet != null)
            {
                for (int i = ImagesForEachLayer.Count - 1; i >= 0; i--)
                {
                    Brush brush = new SolidBrush(GetCurLayerColorByIndex(i));
                    List<Rectangle> rects = new List<Rectangle>();
                    for (int k = 0; k < SelectedNet.trackByLayer[i].Count; k++)
                    {
                        Point pt = new Point((int)((SelectedNet.trackByLayer[i][k].X * scale) + offsetX), (int)((SelectedNet.trackByLayer[i][k].Y * scale) + offsetY));
                        rects.Add(new Rectangle(pt.X, pt.Y, 1, 1));
                    }
                    if (rects.Count > 0)
                    {
                        e.Graphics.FillRectangles(brush, rects.ToArray());
                    }
                }
            }
            if (ShowErrors)
            {
                if (HoleFailureMap != null)
                {
                    e.Graphics.DrawImage(HoleFailureMap, new Rectangle(offsetX, offsetY, (int)(HoleFailureMap.Width * scale), (int)(HoleFailureMap.Height * scale)), 0, 0, HoleFailureMap.Width, HoleFailureMap.Height, GraphicsUnit.Pixel);
                }
            }
        }
    }
}
