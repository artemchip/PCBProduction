using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PCBProduction2
{
    public struct CamTransformation
    {
        public Point Translation { get { return _translation; } }
        public double Scale { get { return _scale; } }
        private readonly Point _translation;
        private readonly double _scale;

        public CamTransformation(Point translation, double scale)
        {
            _translation = translation;
            _scale = scale;
        }

        public Point ConvertToIm(Point p)
        {
            return new Point((int)(p.X * _scale + _translation.X), (int)(p.Y * _scale + _translation.Y));
        }

        public Size ConvertToIm(Size p)
        {
            return new Size((int)(p.Width * _scale), (int)(p.Height * _scale));
        }

        public Rectangle ConvertToIm(Rectangle r)
        {
            return new Rectangle(ConvertToIm(r.Location), ConvertToIm(r.Size));
        }

        public Point ConvertToPb(Point p)
        {
            return new Point((int)((p.X - _translation.X) / _scale), (int)((p.Y - _translation.Y) / _scale));
        }

        public Size ConvertToPb(Size p)
        {
            return new Size((int)(p.Width / _scale), (int)(p.Height / _scale));
        }

        public Rectangle ConvertToPb(Rectangle r)
        {
            return new Rectangle(ConvertToPb(r.Location), ConvertToPb(r.Size));
        }

        public CamTransformation SetTranslate(Point p)
        {
            return new CamTransformation(p, _scale);
        }

        public CamTransformation AddTranslate(Point p)
        {
            return SetTranslate(new Point(p.X + _translation.X, p.Y + _translation.Y));
        }

        public CamTransformation SetScale(double scale)
        {
            return new CamTransformation(_translation, scale);
        }
    }

    public class CamPictureBox : PictureBox
    {
        public Pen pen = new Pen(Brushes.Red);
        private Point? _clickedPoint;
        private CamTransformation _transformation;
        private float[][] mxRed = new float[][]
           {
                new float[] { 1,  0,  0,  0,  0},
                new float[] { 0,  0,  0,  0,  0},
                new float[] { 0,  0,  0,  0,  0},
                new float[] { 0,  0,  0,  1,  0},
                new float[] { 0,  0,  0,  0,  0}
           };
        public CamTransformation Transformation
        {
            set
            {
                _transformation = FixTranslation(value);
                Invalidate();
            }
            get
            {
                return _transformation;
            }
        }
        public Point VisibleCenterPoint = new Point(0, 0);
        public Point ClientCenter = new Point(0, 0);

        public CamPictureBox()
        {
            VisibleCenterPoint = new Point(0, 0);
            _transformation = new CamTransformation(new Point(100, 0), .5f);
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            Resize += OnResize;
        }

        private CamTransformation FixTranslation(CamTransformation value)
        {
            if (Image == null) return value;
            try
            {
                var maxScale = Math.Max((double)Image.Width / ClientRectangle.Width, (double)Image.Height / ClientRectangle.Height);
                if (value.Scale > maxScale)
                    value = value.SetScale(maxScale);
                if (value.Scale < 0.3)
                    value = value.SetScale(0.3);
                var rectSize = value.ConvertToIm(ClientRectangle.Size);
                var max = new Size(Image.Width - rectSize.Width, Image.Height - rectSize.Height);

                value = value.SetTranslate((new Point(Math.Min(value.Translation.X, max.Width), Math.Min(value.Translation.Y, max.Height))));
                if (value.Translation.X < 0 || value.Translation.Y < 0)
                {
                    value = value.SetTranslate(new Point(Math.Max(value.Translation.X, 0), Math.Max(value.Translation.Y, 0)));
                }
            }
            catch { }
            return value;
        }

        public void SetTransformationSoThatCenterIs(PointF desiredCenter)
        {
            if (Image == null) return;
            float curScale = 0.5f;
            var maxScale = Math.Max((double)Image.Width / ClientRectangle.Width, (double)Image.Height / ClientRectangle.Height);
            while (true)
            {
                float translationX = (desiredCenter.X - (ClientRectangle.Size.Width * curScale / 2f) - (ClientRectangle.Location.X * curScale));
                float translationY = (desiredCenter.Y - (ClientRectangle.Size.Height * curScale / 2f) - (ClientRectangle.Location.Y * curScale));
                if (translationX >= 0f && translationY >= 0f)
                {
                    Transformation = new CamTransformation(new Point((int)translationX, (int)translationY), curScale);
                    break;
                }
                curScale *= 0.9f;
                if (curScale < 0.3f)
                {
                    throw new Exception("Не удалось установить Tranformation CamPictureBox");
                }
            }
            Invalidate();
        }

        private void OnResize(object sender, EventArgs eventArgs)
        {
            if (Image == null)
                return;
            Transformation = Transformation;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            var transformation = _transformation;
            var pos1 = transformation.ConvertToIm(e.Location);
            if (e.Delta > 0)
                transformation = (transformation.SetScale(Transformation.Scale / 1.25));
            else
                transformation = (transformation.SetScale(Transformation.Scale * 1.25));
            var pos2 = transformation.ConvertToIm(e.Location);
            transformation = transformation.AddTranslate(pos1 - (Size)pos2);
            if (transformation.Scale < 0.3)
            {
                return;
            }
            Transformation = transformation;
        }

        private void OnMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            _clickedPoint = null;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_clickedPoint == null)
                return;
            var p = _transformation.ConvertToIm((Size)e.Location);
            Transformation = _transformation.SetTranslate(_clickedPoint.Value - p);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Focus();
            _clickedPoint = _transformation.ConvertToIm(e.Location);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image == null) return;
            try
            {
                var imRect = Transformation.ConvertToIm(ClientRectangle);
                using (var attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(new ColorMatrix(mxRed));
                    e.Graphics.DrawImage(Image, ClientRectangle, imRect, GraphicsUnit.Pixel);
                }

                Point cursorClientPoint = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
                e.Graphics.DrawLine(pen, new Point(cursorClientPoint.X, 0), new Point(cursorClientPoint.X, ClientRectangle.Height));
                e.Graphics.DrawLine(pen, new Point(0, cursorClientPoint.Y), new Point(ClientRectangle.Width, cursorClientPoint.Y));
                
                VisibleCenterPoint = new Point((int)(imRect.Left + ((float)imRect.Width / 2f)), (int)(imRect.Top + ((float)imRect.Height / 2f)));
                ClientCenter = new Point((int)(ClientRectangle.Left + (ClientRectangle.Width / 2f)), (int)(ClientRectangle.Top + (ClientRectangle.Height / 2f)));
                e.Graphics.DrawEllipse(pen, new RectangleF(ClientCenter.X - 0.35f * ClientRectangle.Height, ClientCenter.Y - 0.35f * ClientRectangle.Height, ClientRectangle.Height * 0.7f, ClientRectangle.Height * 0.7f));
                e.Graphics.DrawString("Крестик на " + VisibleCenterPoint.X.ToString() + ", " + VisibleCenterPoint.Y.ToString(), new Font(FontFamily.GenericSansSerif, 10f), Brushes.Red, new PointF(10, 10));
            }
            catch { }
        }

        public void DecideInitialTransformation()
        {
            Transformation = new CamTransformation(Point.Empty, int.MaxValue);
        }
    }
}