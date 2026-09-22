using DirectShowLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
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
    }
}