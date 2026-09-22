using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBProduction2
{
    public partial class MainForm : Form
    {
        public CNCForm? cncForm;
        public DrillQCForm? drillQCForm;
        public GalvForm? galvForm;
        public ExposureForm? exposureForm;
        public PNPForm? pnpForm;
        public QCForm? qcForm;

        public static String gerberFilePath = "";
        public static float workImagePhysWidthMm = 100.0f;
        public static float workImagePhysHeightMm = 100.0f;
        public static float holderHolesIndentMm = 5.0f;
        public static float spaceOuterMm = 6.0f;
        public static float gapBetweenPCBsMm = 2.5f;
        public static float cuttingToolDiameterMm = 1.5f;
        public static float anchorHoleDiameterMm = 0.6f;

        public MainForm()
        {
            InitializeComponent();
            CheckAndApplyParams();
        }

        public bool CheckAndApplyParams()
        {
            gerberFilePath = tbGerberPath.Text;
            if (!Directory.Exists(gerberFilePath))
            {
                MessageBox.Show(this, "Папки с Gerber-файлами не существует.");
                return false;
            }
            workImagePhysWidthMm = (float)nudWorkImagePhysWidthMm.Value;
            workImagePhysHeightMm = (float)nudWorkImagePhysHeightMm.Value;
            holderHolesIndentMm = (float)nudHolderHolesIndentMm.Value;
            spaceOuterMm = (float)nudSpaceOuterMm.Value;
            gapBetweenPCBsMm = (float)nudGapBetweenPCBsMm.Value;
            cuttingToolDiameterMm = (float)nudCuttingToolSizeMm.Value;
            anchorHoleDiameterMm = (float)nudAnchorHoleDiameterMm.Value;
            if (workImagePhysHeightMm < 10 || workImagePhysWidthMm < 10)
            {
                MessageBox.Show(this, "Размер пленки или рабочая её область слишком мала.");
                return false;
            }
            if (holderHolesIndentMm < 1 || spaceOuterMm < 1)
            {
                MessageBox.Show(this, "Слишком маленькие поля заготовки.");
                return false;
            }
            if ((cuttingToolDiameterMm * 2f) >= (holderHolesIndentMm + spaceOuterMm))
            {
                MessageBox.Show(this, "Размер полей недостаточен чтобы вместить реперные отверстия.");
                return false;
            }
            if (cuttingToolDiameterMm < 0.5f || cuttingToolDiameterMm > 4.0f)
            {
                MessageBox.Show(this, "Диаметр фрезы слишком мал или велик.");
                return false;
            }
            if (anchorHoleDiameterMm < 0.1f || anchorHoleDiameterMm > 1.0f)
            {
                MessageBox.Show(this, "Диаметр реперного отверстия слишком мал или велик.");
                return false;
            }
            if (gapBetweenPCBsMm < (1.4f * cuttingToolDiameterMm) || gapBetweenPCBsMm > 15f)
            {
                MessageBox.Show(this, "Поля между отдельными платами слишком малы или велики.");
                return false;
            }
            return true;
        }

        private void btnCNC_Click(object sender, EventArgs e)
        {
            if (CheckAndApplyParams())
            {
                cncForm = new CNCForm();
                cncForm?.ShowDialog();
                cncForm = null;
            }
        }

        private void btnGalv_Click(object sender, EventArgs e)
        {
            if (CheckAndApplyParams())
            {
                galvForm = new GalvForm();
                galvForm?.ShowDialog();
                galvForm = null;
            }
        }

        private void btnExposure_Click(object sender, EventArgs e)
        {
            if (CheckAndApplyParams())
            {
                exposureForm = new ExposureForm();
                exposureForm?.ShowDialog();
                exposureForm = null;
            }
        }

        private void btnBrowseGerber_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    tbGerberPath.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnDrillQC_Click(object sender, EventArgs e)
        {
            if (CheckAndApplyParams())
            {
                drillQCForm = new DrillQCForm();
                drillQCForm?.ShowDialog();
                drillQCForm = null;
            }
        }

        private void btnSolderpaste_Click(object sender, EventArgs e)
        {
            if (CheckAndApplyParams())
            {
                pnpForm = new PNPForm();
                pnpForm?.ShowDialog();
                pnpForm = null;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            Process.GetCurrentProcess().Kill();
        }

        private void btnQC_Click(object sender, EventArgs e)
        {
            if (CheckAndApplyParams())
            {
                qcForm = new QCForm();
                qcForm?.ShowDialog();
                qcForm = null;
            }
        }
    }
}