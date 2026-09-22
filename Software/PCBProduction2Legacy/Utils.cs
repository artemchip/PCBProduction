using GerberVS;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PCBProduction2.CNCForm;

namespace PCBProduction2
{
    class Utils
    {
        static decimal CustomParseDecimal(string xinput)
        {
            String input = xinput.Replace("\r\n", "").Replace("\n", "");
            long n = 0;
            int decimalPosition = input.Length;
            for (int k = 0; k < input.Length; k++)
            {
                char c = input[k];
                if (c == '.' || c == ',')
                    decimalPosition = k + 1;
                else if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9')
                    n = (n * 10) + (int)(c - '0');
                else throw new Exception("Неверный формат.");
            }
            return new decimal((int)n, (int)(n >> 32), 0, false, (byte)(input.Length - decimalPosition));
        }
        public static double StrToDouble(String str)
        {
            return (double)(CustomParseDecimal(str));
        }
        public static Bitmap TransformInvert(Bitmap source)
        {
            Bitmap newBitmap = new Bitmap(source.Width, source.Height);
            newBitmap.SetResolution(source.HorizontalResolution, source.VerticalResolution);
            Graphics g = Graphics.FromImage(newBitmap);
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
            {
                new float[] {-1, 0, 0, 0, 0},
                new float[] {0, -1, 0, 0, 0},
                new float[] {0, 0, -1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {1, 1, 1, 0, 1}
            });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
            g.Dispose();
            return newBitmap;
        }

        public static Bitmap Dilation(Bitmap image, int se_dim, bool prPositive)
        {
            int w = image.Width;
            int h = image.Height;
            BitmapData image_data = image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int bytes = image_data.Stride * image_data.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];

            Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
            image.UnlockBits(image_data);

            if (prPositive)
            {
                se_dim = -se_dim;
            }

            bool inv = (se_dim < 0);
            if (se_dim < 0)
            {
                for (int i = 0; i < bytes; i++) buffer[i] = (byte)(255 - buffer[i]);
                se_dim = -se_dim;
            }

            int o = (se_dim - 1) / 2;
            for (int i = o; i < w - o; i++)
            {
                for (int j = o; j < h - o; j++)
                {
                    int position = i * 3 + j * image_data.Stride;
                    for (int k = -o; k <= o; k++)
                    {
                        for (int l = -o; l <= o; l++)
                        {
                            int se_pos = position + k * 3 + l * image_data.Stride;
                            for (int c = 0; c < 3; c++)
                            {
                                result[se_pos + c] = Math.Max(result[se_pos + c], buffer[position]);
                            }
                        }
                    }
                }
            }

            if (inv)
            {
                for (int i = 0; i < bytes; i++) result[i] = (byte)(255 - result[i]);
            }

            Bitmap res_img = new Bitmap(w, h);
            res_img.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            BitmapData res_data = res_img.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(result, 0, res_data.Scan0, bytes);
            res_img.UnlockBits(res_data);
            return res_img;
        }

        public static RectangleF FindBoundsOfProfileImage(GerberImage image)
        {
            double gMinX = 10000.0f;
            double gMinY = 10000.0f;
            double gMaxX = -10000.0f;
            double gMaxY = -10000.0f;
            foreach (GerberNet gnet in image.GerberNetList)
            {
                if (gnet.ApertureState == GerberApertureState.On)
                {
                    if (gnet.StartX > gMaxX)
                    {
                        gMaxX = gnet.StartX;
                    }
                    if (gnet.EndX > gMaxX)
                    {
                        gMaxX = gnet.EndX;
                    }
                    if (gnet.StartY > gMaxY)
                    {
                        gMaxY = gnet.StartY;
                    }
                    if (gnet.EndY > gMaxY)
                    {
                        gMaxY = gnet.EndY;
                    }

                    if (gnet.StartX < gMinX)
                    {
                        gMinX = gnet.StartX;
                    }
                    if (gnet.EndX < gMinX)
                    {
                        gMinX = gnet.EndX;
                    }
                    if (gnet.StartY < gMinY)
                    {
                        gMinY = gnet.StartY;
                    }
                    if (gnet.EndY < gMinY)
                    {
                        gMinY = gnet.EndY;
                    }
                }
            }

            return new RectangleF((float)gMinX, (float)gMinY, (float)(gMaxX - gMinX), (float)(gMaxY - gMinY));
        }

        public static float Lerp(float min, float max, float factor)
        {
            if (factor >= 1.0f)
            {
                return max;
            }
            else if (factor <= 0.0f)
            {
                return min;
            }
            else
            {
                return min + ((max - min) * factor);
            }
        }

        public static int MillisecToExecuteAxisMove(float axisDeltaMm, float axisFeedRateMmPerMin, float axisAccMmPerSecSq)
        {
            float distPassedMm = 0f;
            int msecRes = 0;
            float curVelocityMmPerSec = 0f;
            while (distPassedMm < Math.Abs(axisDeltaMm / 2f))
            {
                if (curVelocityMmPerSec < (axisFeedRateMmPerMin / 60f)) {
                    curVelocityMmPerSec += axisAccMmPerSecSq * 0.01f;
                    if (curVelocityMmPerSec > (axisFeedRateMmPerMin / 60f))
                    {
                        curVelocityMmPerSec = (axisFeedRateMmPerMin / 60f);
                    }
                }
                distPassedMm += curVelocityMmPerSec * 0.01f;
                msecRes += 20;
            }
            return msecRes;
        }

        public static List<CHole> SortHoles(List<CHole> input)
        {
            List<CHole> inTemp = new List<CHole>();
            List<CHole> output = new List<CHole>();
            inTemp.AddRange(input.DistinctBy((e) => e.holeCenterMmX.ToString("0.0000") + e.holeCenterMmY.ToString("0.0000")));
            while (inTemp.Count > 0)
            {
                int closestIndex = 0;
                if (output.Count > 0)
                {
                    Vector2 lastHoleVec = new Vector2(output[output.Count - 1].holeCenterMmX, output[output.Count - 1].holeCenterMmY);
                    float minDist = 1000000.0f;
                    for (int i = 0; i < inTemp.Count; i++)
                    {
                        Vector2 thisVec = new Vector2(inTemp[i].holeCenterMmX, inTemp[i].holeCenterMmY);
                        float thisDist = Vector2.Distance(lastHoleVec, thisVec);
                        if (thisDist < minDist)
                        {
                            minDist = thisDist;
                            closestIndex = i;
                        }
                    }
                }
                output.Add(inTemp[closestIndex]);
                inTemp.RemoveAt(closestIndex);
            }
            return output;
        }

        public static float GetContourSize(PointF[] contour)
        {
            float xMax = float.MinValue;
            float xMin = float.MaxValue;
            float yMax = float.MinValue;
            float yMin = float.MaxValue;
            for (int i = 0; i < contour.Length; i++)
            {
                xMax = Math.Max(xMax, contour[i].X);
                xMin = Math.Min(xMin, contour[i].X);
                yMax = Math.Max(yMax, contour[i].Y);
                yMin = Math.Min(yMin, contour[i].Y);
            }
            return (xMax - xMin) * (yMax - yMin);
        }

        public static double RadToDeg(double rad)
        {
            return rad * (180.0 / Math.PI);
        }

        public unsafe static Bitmap And(Bitmap imgA, Bitmap imgB)
        {
            Bitmap resBmp = new Bitmap(imgA.Width, imgA.Height, PixelFormat.Format24bppRgb);

            BitmapData image_data_A = imgA.LockBits(new Rectangle(0, 0, imgA.Width, imgA.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData image_data_B = imgB.LockBits(new Rectangle(0, 0, imgB.Width, imgB.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData image_data_R = resBmp.LockBits(new Rectangle(0, 0, resBmp.Width, resBmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte* imgStartPointerA = (byte*)image_data_A.Scan0;
            byte* imgStartPointerB = (byte*)image_data_B.Scan0;
            byte* imgStartPointerR = (byte*)image_data_R.Scan0;
            int bytes_count = image_data_R.Stride * image_data_R.Height;

            for (int i = 0; i < bytes_count; i++)
            {
                byte b1 = imgStartPointerA[i];
                byte b2 = imgStartPointerB[i];
                imgStartPointerR[i] = (byte)((b1 & b2));
            }

            imgA.UnlockBits(image_data_A);
            imgB.UnlockBits(image_data_B);
            resBmp.UnlockBits(image_data_R);

            return resBmp;
        }

        public unsafe static Bitmap AndOr(Bitmap imgA, Bitmap imgB, Bitmap imgC)
        {
            Bitmap resBmp = new Bitmap(imgA.Width, imgA.Height, PixelFormat.Format24bppRgb);

            BitmapData image_data_A = imgA.LockBits(new Rectangle(0, 0, imgA.Width, imgA.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData image_data_B = imgB.LockBits(new Rectangle(0, 0, imgB.Width, imgB.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData image_data_C = imgC.LockBits(new Rectangle(0, 0, imgC.Width, imgC.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData image_data_R = resBmp.LockBits(new Rectangle(0, 0, resBmp.Width, resBmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte* imgStartPointerA = (byte*)image_data_A.Scan0;
            byte* imgStartPointerB = (byte*)image_data_B.Scan0;
            byte* imgStartPointerC = (byte*)image_data_C.Scan0;
            byte* imgStartPointerR = (byte*)image_data_R.Scan0;
            int bytes_count = image_data_R.Stride * image_data_R.Height;

            for (int i = 0; i < bytes_count; i++)
            {
                byte b1 = imgStartPointerA[i];
                byte b2 = imgStartPointerB[i];
                byte b3 = imgStartPointerC[i];
                imgStartPointerR[i] = (byte)((b1 & b2) | b3);
            }

            imgA.UnlockBits(image_data_A);
            imgB.UnlockBits(image_data_B);
            imgC.UnlockBits(image_data_C);
            resBmp.UnlockBits(image_data_R);

            return resBmp;
        }

        public static List<PointF[]> SortContours(List<PointF[]> contours)
        {
            List<PointF[]> inTemp = new List<PointF[]>();
            List<PointF[]> output = new List<PointF[]>();
            inTemp.AddRange(contours.Where((e) => e.Count() >= 2));
            while (inTemp.Count > 0)
            {
                int closestIndex = 0;
                if (output.Count > 0)
                {
                    Vector2 lastHoleVec = new Vector2(output[output.Count - 1].Last().X, output[output.Count - 1].Last().Y);
                    float minDist = 1000000.0f;
                    for (int i = 0; i < inTemp.Count; i++)
                    {
                        Vector2 thisVec = new Vector2(inTemp[i].First().X, inTemp[i].First().Y);
                        float thisDist = Vector2.Distance(lastHoleVec, thisVec);
                        if (thisDist < minDist)
                        {
                            minDist = thisDist;
                            closestIndex = i;
                        }
                    }
                }
                output.Add(inTemp[closestIndex]);
                inTemp.RemoveAt(closestIndex);
            }
            return output;
        }

        public static List<Vector2> SortPoints(List<Vector2> points)
        {
            List<Vector2> inTemp = new List<Vector2>();
            List<Vector2> output = new List<Vector2>();
            inTemp.AddRange(points);
            while (inTemp.Count > 0)
            {
                int closestIndex = 0;
                if (output.Count > 0)
                {
                    Vector2 lastHoleVec = output[output.Count - 1];
                    float minDist = 1000000.0f;
                    for (int i = 0; i < inTemp.Count; i++)
                    {
                        Vector2 thisVec = inTemp[i];
                        float thisDist = Vector2.Distance(lastHoleVec, thisVec);
                        if (thisDist < minDist)
                        {
                            minDist = thisDist;
                            closestIndex = i;
                        }
                    }
                }
                output.Add(inTemp[closestIndex]);
                inTemp.RemoveAt(closestIndex);
            }
            return output;
        }
    }
}
