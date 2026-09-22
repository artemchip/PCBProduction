using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBProduction2
{
    public partial class PreviewForm : Form
    {
        public List<CHole> holesToPreview = new List<CHole>();
        public List<PointF[]> contoursToPreview = new List<PointF[]>();
        public float pixelsPerMm = 5f;
        public Pen contourPen = new Pen(new SolidBrush(Color.Yellow));
        public Brush drilledBrush = new SolidBrush(Color.Green);
        public Brush undrilledBrush = new SolidBrush(Color.Red);
        public float contourWidthMm = MainForm.cuttingToolDiameterMm;
        public bool stencilMode = false;

        public PreviewForm()
        {
            InitializeComponent();
        }

        private void PreviewForm_Paint(object sender, PaintEventArgs e)
        {
            contourPen.Width = pixelsPerMm * contourWidthMm;
            contourPen.SetLineCap(System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.DashCap.Round);
            e.Graphics.Clear(Color.Black);
            if (holesToPreview.Count > 0)
            {
                for (int i = 0; i < holesToPreview.Count; i++)
                {
                    Rectangle holeRect = new Rectangle(
                        (int)((holesToPreview[i].holeCenterMmX * pixelsPerMm) - ((holesToPreview[i].diameterMm * pixelsPerMm) / 2f)),
                        (int)((holesToPreview[i].holeCenterMmY * pixelsPerMm) - ((holesToPreview[i].diameterMm * pixelsPerMm) / 2f)),
                        (int)(holesToPreview[i].diameterMm * pixelsPerMm),
                        (int)(holesToPreview[i].diameterMm * pixelsPerMm)
                    );
                    e.Graphics.FillEllipse(holesToPreview[i].isDrilled ? drilledBrush : undrilledBrush, holeRect);
                }
            }
            if (contoursToPreview.Count > 0)
            {
                for (int i = 0; i < contoursToPreview.Count; i++)
                {
                    List<PointF> gPoints = new List<PointF>();
                    PointF prevPoint = new PointF(-1f, -1f);
                    if (contoursToPreview[i].Length == 1)
                    {
                        float xdiameterMm = MainForm.cuttingToolDiameterMm;
                        Rectangle holeRect = new Rectangle(
                            (int)((contoursToPreview[i][0].X * pixelsPerMm) - ((xdiameterMm * pixelsPerMm) / 2f)),
                            (int)((contoursToPreview[i][0].Y * pixelsPerMm) - ((xdiameterMm * pixelsPerMm) / 2f)),
                            (int)(xdiameterMm * pixelsPerMm),
                            (int)(xdiameterMm * pixelsPerMm)
                        );
                        e.Graphics.FillEllipse(Brushes.Yellow, holeRect);
                    }
                    else
                    {
                        for (int j = 0; j < contoursToPreview[i].Length; j++)
                        {
                            PointF curPoint = new PointF(contoursToPreview[i][j].X * pixelsPerMm, contoursToPreview[i][j].Y * pixelsPerMm);
                            if (prevPoint.X != -1f && prevPoint.Y != -1f)
                            {
                                e.Graphics.DrawLine(contourPen, prevPoint, curPoint);
                            }
                            prevPoint = curPoint;
                        }
                    }
                }
            }
        }

        private void PreviewForm_Shown(object sender, EventArgs e)
        {
            float stencilAdvanceMm = (stencilMode) ? 20.0f : 0.0f;
            float panelSizeWidthMm = stencilAdvanceMm + MainForm.workImagePhysWidthMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
            float panelSizeHeightMm = MainForm.workImagePhysHeightMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
            ClientSize = new Size((int)(panelSizeWidthMm * pixelsPerMm), (int)(panelSizeHeightMm * pixelsPerMm));
        }
    }
}
