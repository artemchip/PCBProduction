using Emgu.CV.Structure;
using Emgu.CV;
using GerberVS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV.Util;
using System.Windows.Forms.VisualStyles;
using Emgu.CV.CvEnum;
using System.Numerics;
using Newtonsoft.Json;

namespace PCBProduction2
{
    public partial class DrillQCForm : Form
    {
        // Gerber
        public LibGerberVS gerberVS = new LibGerberVS();

        // Все отверстия координаты
        public List<CHole> totalHoleList = new List<CHole>();

        public List<CHole> toolList = new List<CHole>();

        public Brush drilledBrush = new SolidBrush(Color.Green);

        public List<PointF> photoMaskPoints = new List<PointF>();

        public DrillQCForm()
        {
            InitializeComponent();
        }

        public void DoLoadEverything()
        {
            String gbrProfileFile = "";
            String gbrDrillFile = "";
            List<String> allGerberFiles = Directory.GetFiles(MainForm.gerberFilePath).ToList();
            allGerberFiles.Sort((a, b) => File.ReadAllText(a).Length.CompareTo(File.ReadAllText(b).Length));
            foreach (String pFile in allGerberFiles)
            {
                if (pFile.ToLower().EndsWith(".gko") || pFile.ToLower().Contains("outline") || pFile.ToLower().Contains("profile"))
                {
                    gbrProfileFile = pFile;
                }
                else if (pFile.ToLower().EndsWith(".xln") || pFile.ToLower().EndsWith(".drl"))
                {
                    gbrDrillFile = pFile;
                }
            }
            ProcessDrills(gbrDrillFile, gbrProfileFile);
        }

        private void ProcessDrills(String inputFile, String profileFile)
        {
            GerberProject proj = gerberVS.CreateNewProject();
            gerberVS.OpenLayerFromFileName(proj, inputFile);
            gerberVS.OpenLayerFromFileName(proj, profileFile);
            List<CHole> allHoles = new List<CHole>();
            Aperture[] apertures = proj.FileInfo[0].Image.ApertureArray();
            RectangleF bounds = Utils.FindBoundsOfProfileImage(proj.FileInfo[1].Image);
            foreach (GerberNet net in proj.FileInfo[0].Image.GerberNetList)
            {
                if (net.ApertureState == GerberApertureState.Flash)
                {
                    double cxInFileUnits = (net.StartX - bounds.Left);
                    double cyInFileUnits = (bounds.Bottom - net.StartY);
                    double diameterInFileUnits = apertures[net.Aperture].Parameters()[0];
                    allHoles.Add(
                        new CHole(
                            false,
                            (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cxInFileUnits * 25.4f) : cxInFileUnits),
                            (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cyInFileUnits * 25.4f) : cyInFileUnits),
                            (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (diameterInFileUnits * 25.4f) : diameterInFileUnits)
                        )
                    );
                }
            }

            // Stack images
            SizeF workImagePhysicalSizeInMm = new SizeF(MainForm.workImagePhysWidthMm, MainForm.workImagePhysHeightMm);
            SizeF simagePhysicalSizeInMm = new SizeF((float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (bounds.Width * 25.4f) : bounds.Width), (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (bounds.Height * 25.4f) : bounds.Height));
            int possibleCols = (int)Math.Ceiling((double)workImagePhysicalSizeInMm.Width / (double)simagePhysicalSizeInMm.Width);
            int possibleRows = (int)Math.Ceiling((double)workImagePhysicalSizeInMm.Height / (double)simagePhysicalSizeInMm.Height);
            if (possibleCols < 1 || possibleRows < 1)
            {
                throw new Exception("PCB too large.");
            }
            totalHoleList.Clear();
            foreach (CHole hole in allHoles)
            {
                for (int ix = 0; ix < possibleCols; ix++)
                {
                    for (int iy = 0; iy < possibleRows; iy++)
                    {
                        CHole newHole = new CHole(
                            false,
                            MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + MainForm.gapBetweenPCBsMm + hole.holeCenterMmX + ((simagePhysicalSizeInMm.Width + (MainForm.gapBetweenPCBsMm * 2.0f)) * ix),
                            MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + MainForm.gapBetweenPCBsMm + hole.holeCenterMmY + ((simagePhysicalSizeInMm.Height + (MainForm.gapBetweenPCBsMm * 2.0f)) * iy),
                            (float)Math.Round(hole.diameterMm, 1)
                        );
                        if ((newHole.holeCenterMmX + hole.diameterMm) > workImagePhysicalSizeInMm.Width)
                        {
                            continue;
                        }
                        if ((newHole.holeCenterMmY + hole.diameterMm) > workImagePhysicalSizeInMm.Height)
                        {
                            continue;
                        }
                        totalHoleList.Add(newHole);
                    }
                }
            }

            // Calculate anchor holes
            float holderHoleRectTop = MainForm.spaceOuterMm;
            float holderHoleRectLeft = MainForm.spaceOuterMm;
            float holderHoleRectRight = MainForm.spaceOuterMm + workImagePhysicalSizeInMm.Width + (MainForm.holderHolesIndentMm * 2f);
            float holderHoleRectBottom = MainForm.spaceOuterMm + workImagePhysicalSizeInMm.Height + (MainForm.holderHolesIndentMm * 2f);
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.5f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 1.0f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.5f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 1.0f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 1.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 1.0f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 1.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.5f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 1.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.5f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), MainForm.anchorHoleDiameterMm));

            // Tools
            toolList = totalHoleList.DistinctBy((e) => e.diameterMm).OrderByDescending((e) => e.diameterMm * ((e.isAnchor) ? 100f : 1f)).ToList();

            // Free resources
            gerberVS.UnloadAllLayers(proj);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Отчет
            String report = "";

            // Исходные данные
            int scanDpi = (int)nudDPI.Value;
            Point anchorTL = new Point((int)nudTopLeftX.Value, (int)nudTopLeftY.Value);
            Point anchorTR = new Point((int)nudTopRightX.Value, (int)nudTopRightY.Value);
            Point anchorBL = new Point((int)nudBottomLeftX.Value, (int)nudBottomLeftY.Value);
            Point anchorBR = new Point((int)nudBottomRightX.Value, (int)nudBottomRightY.Value);
            if (anchorTL.X < 1 || anchorTL.Y < 1 || anchorTR.X < 1 || anchorTR.Y < 1 || anchorBL.X < 1 || anchorBL.Y < 1 || anchorBR.X < 1 || anchorBR.Y < 1)
            {
                MessageBox.Show("Заполните все координаты.");
                return;
            }

            // Диагонали и угол под которым заготовка лежит на сканере
            Vector2 diagonal1 = Vector2.Subtract(new Vector2(anchorBR.X, anchorBR.Y), new Vector2(anchorTL.X, anchorTL.Y));
            float rotAngleD1 = (float)(Math.Atan(diagonal1.Y / diagonal1.X) - Utils.RadToDeg(45));
            Vector2 diagonal2 = Vector2.Subtract(new Vector2(anchorBL.X, anchorBL.Y), new Vector2(anchorTR.X, anchorTR.Y));
            float rotAngleD2 = (float)(-Math.Atan(Math.Abs(diagonal2.Y / diagonal2.X)) + Utils.RadToDeg(45));
            report += "Диагональ 1: " + diagonal1.Length().ToString("0.#########") + " пикселей, " + ((25.4f * diagonal1.Length()) / (float)scanDpi).ToString("0.#########") + " мм\n";
            report += "Диагональ 2: " + diagonal2.Length().ToString("0.#########") + " пикселей, " + ((25.4f * diagonal2.Length()) / (float)scanDpi).ToString("0.#########") + " мм\n";
            float rotAngleDX = (rotAngleD1 + rotAngleD2) / 2f;
            report += "Они должны совпадать, в противном случае оси XY вашего станка не очень перпендикулярны\n";
            report += "Заготовка лежит на сканере под углом " + Utils.RadToDeg(rotAngleDX).ToString("0.#########") + " градусов\n";

            // Вектора сторон
            Vector2 lineX1 = Vector2.Subtract(new Vector2(anchorTR.X, anchorTR.Y), new Vector2(anchorTL.X, anchorTL.Y));
            Vector2 lineX2 = Vector2.Subtract(new Vector2(anchorBR.X, anchorBR.Y), new Vector2(anchorBL.X, anchorBL.Y));
            Vector2 lineY1 = Vector2.Subtract(new Vector2(anchorBL.X, anchorBL.Y), new Vector2(anchorTL.X, anchorTL.Y));
            Vector2 lineY2 = Vector2.Subtract(new Vector2(anchorBR.X, anchorBR.Y), new Vector2(anchorTR.X, anchorTR.Y));
            Vector2 lineA = Vector2.Subtract(new Vector2(anchorBR.X, anchorBR.Y), new Vector2(anchorTL.X, anchorTL.Y));

            // Повернутые ровно
            Vector2 lineXT1 = Vector2.Transform(lineX1, Matrix3x2.CreateRotation(-rotAngleDX));
            Vector2 lineXT2 = Vector2.Transform(lineX2, Matrix3x2.CreateRotation(-rotAngleDX));
            Vector2 lineYT1 = Vector2.Transform(lineY1, Matrix3x2.CreateRotation(-rotAngleDX));
            Vector2 lineYT2 = Vector2.Transform(lineY2, Matrix3x2.CreateRotation(-rotAngleDX));
            Vector2 lineAT = Vector2.Transform(lineA, Matrix3x2.CreateRotation(-rotAngleDX));

            // Усредняем
            Vector2 lineXT = new Vector2((lineXT1.X + lineXT2.X) / 2f, (lineXT1.Y + lineXT2.Y) / 2f);
            Vector2 lineYT = new Vector2((lineYT1.X + lineYT2.X) / 2f, (lineYT1.Y + lineYT2.Y) / 2f);

            // Считаем длины
            float shouldBeLengthXPx = ((float)scanDpi / 25.4f) * (MainForm.workImagePhysWidthMm + (MainForm.holderHolesIndentMm * 2f));
            float shouldBeLengthYPx = ((float)scanDpi / 25.4f) * (MainForm.workImagePhysHeightMm + (MainForm.holderHolesIndentMm * 2f));
            report += "Длина X: " + lineXT.Length() + " (" + ((25.4f * lineXT.Length()) / (float)scanDpi).ToString("0.#########") + " мм), а должна быть " + shouldBeLengthXPx.ToString("0.#########") + " пикселей (" + ((25.4f * shouldBeLengthXPx) / (float)scanDpi).ToString("0.#########") + " мм)\n";
            report += "Длина Y: " + lineYT.Length() + " (" + ((25.4f * lineYT.Length()) / (float)scanDpi).ToString("0.#########") + " мм), а должна быть " + shouldBeLengthYPx.ToString("0.#########") + " пикселей (" + ((25.4f * shouldBeLengthYPx) / (float)scanDpi).ToString("0.#########") + " мм)\n";
            report += "Необходимо увеличить у ЧПУ станка количество шагов на мм по X в " + (shouldBeLengthXPx / lineXT.Length()).ToString("0.#########") + " раз, по Y в " + (shouldBeLengthYPx / lineYT.Length()).ToString("0.#########") + " раз (по сравнению с теми при которых заготовка делалась)\n";

            // Skew
            float compensateXCoeff = -lineXT.Y / lineXT.X;
            float compensateYCoeff = -lineYT.X / lineYT.Y;
            report += "Для исправления неперпендикулярности осей нужно добавить к коэффициенту по X число " + compensateXCoeff.ToString("0.#########") + ", по Y " + compensateYCoeff.ToString("0.#########") + " (к тем при которых заготовка делалась)\n";

            // Matrix
            photoMaskPoints.Clear();
            photoMaskPoints.Add(new PointF(MainForm.spaceOuterMm, MainForm.spaceOuterMm));
            photoMaskPoints.Add(new PointF(MainForm.spaceOuterMm + (25.4f * lineXT1.X / (float)scanDpi), MainForm.spaceOuterMm + (25.4f * lineXT1.Y / (float)scanDpi)));
            photoMaskPoints.Add(new PointF(MainForm.spaceOuterMm + (25.4f * lineYT1.X / (float)scanDpi), MainForm.spaceOuterMm + (25.4f * lineYT1.Y / (float)scanDpi)));
            photoMaskPoints.Add(new PointF(MainForm.spaceOuterMm + (25.4f * lineAT.X / (float)scanDpi), MainForm.spaceOuterMm + (25.4f * lineAT.Y / (float)scanDpi)));

            // Результат
            lblCalcResult.Text = report;
        }

        private void DrillQCForm_Load(object sender, EventArgs e)
        {
            DoLoadEverything();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            ofDialog.Reset();
            ofDialog.Filter = "PNG файлы|*.png";
            ofDialog.Multiselect = false;
            ofDialog.Title = "Скан";
            ofDialog.FilterIndex = 0;
            ofDialog.CheckFileExists = true;
            ofDialog.ValidateNames = true;
            ofDialog.DefaultExt = "*.png";
            ofDialog.RestoreDirectory = true;
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                tbScanLocation.Text = ofDialog.FileName;
                pbPreview.Image = new Bitmap(ofDialog.FileName);
            }
        }

        private void btnTopLeftCursor_Click(object sender, EventArgs e)
        {
            nudTopLeftX.Value = pbPreview.VisibleCenterPoint.X;
            nudTopLeftY.Value = pbPreview.VisibleCenterPoint.Y;
        }

        private void btnTopRightCursor_Click(object sender, EventArgs e)
        {
            nudTopRightX.Value = pbPreview.VisibleCenterPoint.X;
            nudTopRightY.Value = pbPreview.VisibleCenterPoint.Y;
        }

        private void btnBottomLeftCursor_Click(object sender, EventArgs e)
        {
            nudBottomLeftX.Value = pbPreview.VisibleCenterPoint.X;
            nudBottomLeftY.Value = pbPreview.VisibleCenterPoint.Y;
        }

        private void btnBottomRightCursor_Click(object sender, EventArgs e)
        {
            nudBottomRightX.Value = pbPreview.VisibleCenterPoint.X;
            nudBottomRightY.Value = pbPreview.VisibleCenterPoint.Y;
        }

        private void btnSaveMatrix_Click(object sender, EventArgs e)
        {
            if (photoMaskPoints.Count < 4)
            {
                MessageBox.Show("Сначала выполните расчет.");
                return;
            }
            float[] arrayToSerialize = { photoMaskPoints[0].X, photoMaskPoints[0].Y, photoMaskPoints[1].X, photoMaskPoints[1].Y, photoMaskPoints[2].X, photoMaskPoints[2].Y, photoMaskPoints[3].X, photoMaskPoints[3].Y };
            string output = JsonConvert.SerializeObject(arrayToSerialize);
            sfDialog.Reset();
            sfDialog.RestoreDirectory = true;
            sfDialog.ValidateNames = true;
            sfDialog.CheckPathExists = true;
            sfDialog.Title = "Сохранить матрицу";
            sfDialog.DefaultExt = "*.json";
            sfDialog.AddExtension = true;
            sfDialog.SupportMultiDottedExtensions = false;
            sfDialog.OverwritePrompt = true;
            sfDialog.Filter = "JSON файлы|*.json";
            sfDialog.FilterIndex = 0;
            if (sfDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfDialog.FileName, output);
            }
        }
    }
}
// nulls in coord by default and file select