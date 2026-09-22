using System;
using System.Drawing;
using System.Windows.Forms;

namespace PCBProduction2
{
    public struct ProTransformation
    {
        public Point Translation { get { return _translation; } }
        public double Scale { get { return _scale; } }
        private readonly Point _translation;
        private readonly double _scale;

        public ProTransformation(Point translation, double scale)
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

        public ProTransformation SetTranslate(Point p)
        {
            return new ProTransformation(p, _scale);
        }

        public ProTransformation AddTranslate(Point p)
        {
            return SetTranslate(new Point(p.X + _translation.X, p.Y + _translation.Y));
        }

        public ProTransformation SetScale(double scale)
        {
            return new ProTransformation(_translation, scale);
        }
    }

    public class ProPictureBox : PictureBox
    {
        public Pen pen = new Pen(Brushes.Red);
        private Point? _clickedPoint;
        private ProTransformation _transformation;
        public ProTransformation Transformation
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

        public ProPictureBox()
        {
            VisibleCenterPoint = new Point(0, 0);
            _transformation = new ProTransformation(new Point(100, 0), .5f);
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            Resize += OnResize;
        }

        private ProTransformation FixTranslation(ProTransformation value)
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
            return value;
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
            var imRect = Transformation.ConvertToIm(ClientRectangle);
            e.Graphics.DrawImage(Image, ClientRectangle, imRect, GraphicsUnit.Pixel);

            Point cursorClientPoint = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
            e.Graphics.DrawLine(pen, new Point(cursorClientPoint.X, 0), new Point(cursorClientPoint.X, ClientRectangle.Height));
            e.Graphics.DrawLine(pen, new Point(0, cursorClientPoint.Y), new Point(ClientRectangle.Width, cursorClientPoint.Y));

            VisibleCenterPoint = new Point((int)(imRect.Left + ((float)imRect.Width / 2f)), (int)(imRect.Top + ((float)imRect.Height / 2f)));
            e.Graphics.DrawString("Крестик на " + VisibleCenterPoint.X.ToString() + ", " + VisibleCenterPoint.Y.ToString(), new Font(FontFamily.GenericSansSerif, 10f), Brushes.Red, new PointF(10, 10));
        }

        public void DecideInitialTransformation()
        {
            Transformation = new ProTransformation(Point.Empty, int.MaxValue);
        }
    }
}