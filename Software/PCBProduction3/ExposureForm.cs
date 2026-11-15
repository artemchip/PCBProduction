using DirectShowLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Reg;
using Emgu.CV.Structure;
using GerberVS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBProduction3
{
    public partial class ExposureForm : Form
    {
        // Gerber
        public LibGerberVS gerberVS = new LibGerberVS();

        // Params
        public bool isPhotoresistPositive = false;
        public int renderDpi = 300;
        public String topText = "Top";
        public String bottomText = "Bottom";

        public ushort[,] oldGamma;
        public bool gbError = false;

        // Прямая засветка
        public LCDParamsForm? lcdParamsForm;
        public LCDForm? lcdForm;

        public ExposureForm()
        {
            InitializeComponent();
        }

        private void ExposureForm_Shown(object sender, EventArgs e)
        {
            try
            {
                GerberFolderParser.LoadLayers(MainForm.gerberFilesDir);
                clbLayers.Items.Clear();
                foreach (ELayer el in GerberFolderParser.loadedLayers)
                {
                    clbLayers.Items.Add(Path.GetFileName(el.fileName));
                }
                clbLayers.SelectedIndex = 0;
            }
            catch (Exception exc)
            {
                MessageBox.Show(this, exc.Message);
                Close();
                return;
            }
            this.FormBorderStyle = FormBorderStyle.None;
            this.Location = new Point(0, 0);
            this.Size = Screen.FromControl(this).Bounds.Size;
            this.mainGroupBox.Location = new Point((int)(((float)this.Size.Width / 2f) - ((float)this.mainGroupBox.Size.Width / 2f)), (int)(((float)this.Size.Height / 2f) - ((float)this.mainGroupBox.Size.Height / 2f)));
            tbBottomText.Cursor = this.Cursor;
            tbTopText.Cursor = this.Cursor;
            nudRenderDpi.Cursor = this.Cursor;
            RefreshUIDisplay();
        }

        private void cbPositiveExposure_CheckedChanged(object sender, EventArgs e)
        {
            RefreshUIDisplay();
        }

        private void nudRenderDpi_ValueChanged(object sender, EventArgs e)
        {
            RefreshUIDisplay();
        }

        public Bitmap? RedOnly(Bitmap? srcBmp)
        {
            if (srcBmp == null) return null;
            Bitmap resBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            var mxRed = new float[][]
            {
                new float[] { 1,  0,  0,  0,  0},
                new float[] { 1,  0,  0,  0,  0},
                new float[] { 0,  0,  0,  0,  0},
                new float[] { 0,  0,  0,  1,  0},
                new float[] { 0,  0,  0,  0,  0}
            };
            using (Graphics g = Graphics.FromImage(resBmp))
            {
                using (var attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(new ColorMatrix(mxRed));
                    var rect = new Rectangle(Point.Empty, srcBmp.Size);
                    g.DrawImage(srcBmp, rect, 0, 0, srcBmp.Width, srcBmp.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            srcBmp?.Dispose();
            return resBmp;
        }

        public void RefreshUIDisplay()
        {
            isPhotoresistPositive = cbPositiveExposure.Checked;
            renderDpi = (int)nudRenderDpi.Value;
            topText = tbTopText.Text;
            bottomText = tbBottomText.Text;
            int selLayerIdx = clbLayers.SelectedIndex;
            if (selLayerIdx > (GerberFolderParser.loadedLayers.Count - 1)) selLayerIdx = GerberFolderParser.loadedLayers.Count - 1;
            if (selLayerIdx < 0) selLayerIdx = 0;
            if (GerberFolderParser.loadedLayers.Count > 0)
            {
                try
                {
                    pbPreview.Image?.Dispose();
                }
                catch { }
                Bitmap? bitmap = null;
                try
                {
                    bitmap = GerberFolderParser.DrawPanelFromGbrLayerFile(GerberFolderParser.loadedLayers[selLayerIdx].fileName, GerberFolderParser.gbrProfileFile, isPhotoresistPositive, !GerberFolderParser.loadedLayers[selLayerIdx].shouldMirror, (GerberFolderParser.loadedLayers[selLayerIdx].shouldMirror) ? bottomText : topText, renderDpi);
                }
                catch { }
                Bitmap? bmpAtIdx = RedOnly(bitmap);
                if (bmpAtIdx == null)
                {
                    pbPreview.Image = null;
                    lblWhiteCount.Text = "?";
                }
                else
                {
                    pbPreview.Height = (int)((float)pbPreview.Width * ((float)bmpAtIdx.Height / (float)bmpAtIdx.Width));
                    pbPreview.Image = bmpAtIdx;
                    int whitePixels = 0;
                    for (int x = 0; x < bmpAtIdx.Width; x++)
                    {
                        for (int y = 0; y < bmpAtIdx.Height; y++)
                        {
                            Color clr = bmpAtIdx.GetPixel(x, y);
                            if (clr.R > 200)
                            {
                                whitePixels++;
                            }
                        }
                    }
                    float whPercent = ((float)whitePixels * 100.0f) / (float)(bmpAtIdx.Width * bmpAtIdx.Height);
                    lblWhiteCount.Text = whPercent.ToString("0.##") + "% белых пикселей";
                }
            }
            else
            {
                pbPreview.Image = null;
            }
        }

        private void clbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUIDisplay();
        }

        private void tbBottomText1_Leave(object sender, EventArgs e)
        {
            RefreshUIDisplay();
        }

        private void tbTopText1_Leave(object sender, EventArgs e)
        {
            RefreshUIDisplay();
        }

        private void btnCloseWin_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void redButton1_Click(object sender, EventArgs e)
        {
            // Выбранный слой
            if (GerberFolderParser.loadedLayers.Count < 1)
            {
                MessageBox.Show(this, "Слои не загружены.");
                return;
            }
            if (clbLayers.SelectedIndices.Count < 1)
            {
                MessageBox.Show(this, "Ничего не выбрано для сохранения.");
                return;
            }
            int selLayerIdx = clbLayers.SelectedIndex;
            if (selLayerIdx > (GerberFolderParser.loadedLayers.Count - 1)) selLayerIdx = GerberFolderParser.loadedLayers.Count - 1;
            if (selLayerIdx < 0) selLayerIdx = 0;

            // Размеры пикселя указать
            lcdParamsForm = new LCDParamsForm();
            if (lcdParamsForm?.ShowDialog() == DialogResult.OK)
            {
                // Render And Open
                int minUm = Math.Min(lcdParamsForm.pxWidthUm, lcdParamsForm.pxHeightUm);
                int rndDPI = (int)(25400f / minUm);
                lcdForm = new LCDForm();
                lcdForm.panelBitmapAtHighDPI = GerberFolderParser.DrawPanelFromGbrLayerFile(GerberFolderParser.loadedLayers[selLayerIdx].fileName, GerberFolderParser.gbrProfileFile, isPhotoresistPositive, !GerberFolderParser.loadedLayers[selLayerIdx].shouldMirror, (GerberFolderParser.loadedLayers[selLayerIdx].shouldMirror) ? bottomText : topText, rndDPI);
                lcdForm.pxWidthUm = lcdParamsForm.pxWidthUm;
                lcdForm.pxHeightUm = lcdParamsForm.pxHeightUm;
                String clFName = Path.GetFileName(GerberFolderParser.loadedLayers[selLayerIdx].fileName);
                lcdForm.stencilMode = clFName.ToLower().Contains("solderpaste") || clFName.ToLower().EndsWith(".gtp") || clFName.ToLower().EndsWith(".gbp");
                lcdForm?.ShowDialog();
            }
        }

        private void ExposureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            pbPreview.Image?.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            imgSaveDialog.FileName = "";
            if (imgSaveDialog.ShowDialog() == DialogResult.OK)
            {
                pbPreview.Image?.Save(imgSaveDialog.FileName);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (GerberFolderParser.loadedLayers.Count < 1)
            {
                MessageBox.Show(this, "Слои не загружены.");
                return;
            }
            if (clbLayers.CheckedIndices.Count < 1)
            {
                MessageBox.Show(this, "Ничего не выбрано для печати.");
                return;
            }
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintFilmPage;
            PrintDialog pdi = new PrintDialog();
            pdi.Document = pd;
            if (pdi.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void Pd_PrintFilmPage(object sender, PrintPageEventArgs e)
        {
            e.HasMorePages = false;

            // Предобработка Bitmapов и поворот так чтобы ширина была больше высоты
            List<Bitmap> bitmapsToRender = new List<Bitmap>();
            Bitmap? totalGridImage = null;
            try
            {
                int idx = 0;
                foreach (ELayer el in GerberFolderParser.loadedLayers)
                {
                    if (clbLayers.CheckedIndices.Contains(idx))
                    {
                        Bitmap bmpCopy = GerberFolderParser.DrawPanelFromGbrLayerFile(el.fileName, GerberFolderParser.gbrProfileFile, isPhotoresistPositive, !el.shouldMirror, (el.shouldMirror) ? bottomText : topText, renderDpi);
                        bmpCopy.SetResolution(1f, 1f);
                        if (bmpCopy.Width < bmpCopy.Height)
                        {
                            bmpCopy.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        }
                        bitmapsToRender.Add(bmpCopy);
                    }
                    idx++;
                }

                // Stack
                int widthLimitPx = (int)Math.Floor((((float)Math.Max(e.PageBounds.Width, e.PageBounds.Height)) / 100.0f) * renderDpi);
                int firstBmpWidthPx = 1;
                int firstBmpHeightPx = 1;
                try
                {
                    firstBmpWidthPx = bitmapsToRender.First().Width;
                    firstBmpHeightPx = bitmapsToRender.First().Height;
                }
                catch { }
                int colCount = (int)Math.Floor((float)widthLimitPx / (float)firstBmpWidthPx);
                int rowCount = (int)Math.Ceiling((float)bitmapsToRender.Count / (float)colCount);
                if (colCount > bitmapsToRender.Count)
                {
                    colCount = bitmapsToRender.Count;
                }
                int gIdx = 0;
                totalGridImage = new Bitmap(colCount * firstBmpWidthPx, rowCount * firstBmpHeightPx);
                totalGridImage.SetResolution(1f, 1f);
                using (Graphics g = Graphics.FromImage(totalGridImage))
                {
                    for (int x = 0; x < colCount; x++)
                    {
                        for (int y = 0; y < rowCount; y++)
                        {
                            g.DrawImage(bitmapsToRender[gIdx], new Point(x * firstBmpWidthPx, y * firstBmpHeightPx));
                            gIdx++;
                        }
                    }
                }

                // Рендер общей картинки на страницу
                bool isMaskWide = totalGridImage.Width > totalGridImage.Height;
                bool isPaperWide = e.PageBounds.Width > e.PageBounds.Height;
                if (isMaskWide != isPaperWide)
                {
                    totalGridImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                float widthU = totalGridImage.Width * 100.0f / renderDpi;
                float heightU = totalGridImage.Height * 100.0f / renderDpi;
                float left = e.PageBounds.Left + (e.PageBounds.Width / 2.0f) - (widthU / 2.0f);
                float top = e.PageBounds.Top + (e.PageBounds.Height / 2.0f) - (heightU / 2.0f);
                if (left < e.PageBounds.Left) left = e.PageBounds.Left;
                if (top < e.PageBounds.Top) top = e.PageBounds.Top;
                e.Graphics?.DrawImage(totalGridImage, left, top, widthU, heightU);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } finally
            {
                // Освободить ресурсы
                for (int i = 0; i < bitmapsToRender.Count; i++)
                {
                    bitmapsToRender[i].Dispose();
                }
                totalGridImage?.Dispose();
            }
        }
    }
}