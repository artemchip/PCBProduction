using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    public class BmpShape
    {
        public List<Point> allPoints = new List<Point>();
        public Point centerPoint = Point.Empty;

        public BmpShape(List<Point> allPoints, Point centerPoint)
        {
            this.allPoints = allPoints;
            this.centerPoint = centerPoint;
        }
    }

    public class MyBitmapData
    {
        public bool[] data = { };
        public int width = 0;
        public int height = 0;

        public int GetIndexByCoord(int x, int y)
        {
            return x + (y * width);
        }

        private MyBitmapData()
        {

        }

        public unsafe MyBitmapData(Bitmap bmp)
        {
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int pixelSize = Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
            byte* imgStartPointer = (byte*)bitmapData.Scan0;
            data = new bool[bmp.Width * bmp.Height];
            width = bmp.Width;
            height = bmp.Height;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    int pxindex = ((y * bitmapData.Stride) + (x * pixelSize)) + 2;
                    data[x + (y * width)] = imgStartPointer[pxindex] > 200;
                }
            }
            bmp.UnlockBits(bitmapData);
        }

        public List<Point> GetShapeAndTurnBlackFlood(Point pf)
        {
            List<Point> curShape = new List<Point>();
            Stack<Point> pixels = new Stack<Point>();
            pixels.Push(pf);
            while (pixels.Count > 0)
            {
                Point a = pixels.Pop();
                if (a.X < width && a.X > -1 && a.Y < height && a.Y > -1)
                {
                    if (data[a.X + (a.Y * width)])
                    {
                        data[a.X + (a.Y * width)] = false;
                        curShape.Add(a);
                        pixels.Push(new Point(a.X - 1, a.Y));
                        pixels.Push(new Point(a.X + 1, a.Y));
                        pixels.Push(new Point(a.X, a.Y - 1));
                        pixels.Push(new Point(a.X, a.Y + 1));
                        pixels.Push(new Point(a.X - 1, a.Y - 1));
                        pixels.Push(new Point(a.X + 1, a.Y + 1));
                        pixels.Push(new Point(a.X + 1, a.Y - 1));
                        pixels.Push(new Point(a.X - 1, a.Y + 1));
                    }
                }
            }
            return curShape;
        }


        public List<BmpShape> GetAllShapes()
        {
            List<BmpShape> list = new List<BmpShape>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (data[i + (j * width)])
                    {
                        // Текущая фигура
                        List<Point> curShape = GetShapeAndTurnBlackFlood(new Point(i, j));

                        // Добавить центр текущей фигуры к списку
                        Point math_avg = new Point((int)curShape.Average((e) => e.X), (int)curShape.Average((e) => e.Y));
                        Point ex_avg = curShape.MinBy(e => Math.Pow(e.X - math_avg.X, 2) + Math.Pow(e.Y - math_avg.Y, 2));
                        list.Add(new BmpShape(curShape, ex_avg));
                    }
                }
            }
            return list;
        }

        public MyBitmapData Copy()
        {
            MyBitmapData res = new MyBitmapData();
            res.width = width;
            res.height = height;
            res.data = new bool[width * height];
            for (int i = 0; i < data.Length; i++)
            {
                res.data[i] = data[i];
            }
            return res;
        }

        public MyBitmapData Inverted()
        {
            MyBitmapData res = new MyBitmapData();
            res.width = width;
            res.height = height;
            res.data = new bool[width * height];
            for (int i = 0; i < data.Length; i++)
            {
                res.data[i] = !data[i];
            }
            return res;
        }

        public unsafe Bitmap ToNormalBitmap()
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int pixelSize = Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
            byte* imgStartPointer = (byte*)bitmapData.Scan0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    imgStartPointer[((y * bitmapData.Stride) + (x * pixelSize)) + 0] = (byte)((data[x + (y * width)]) ? 255 : 0);
                    imgStartPointer[((y * bitmapData.Stride) + (x * pixelSize)) + 1] = (byte)((data[x + (y * width)]) ? 255 : 0);
                    imgStartPointer[((y * bitmapData.Stride) + (x * pixelSize)) + 2] = (byte)((data[x + (y * width)]) ? 255 : 0);
                }
            }
            bmp.UnlockBits(bitmapData);
            return bmp;
        }
    }
}
