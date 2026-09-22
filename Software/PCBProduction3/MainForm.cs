using System.Windows.Forms;

namespace PCBProduction3
{
    public partial class MainForm : Form
    {
        // Константы размера панели и расстояний
        public static String gerberFilesDir = "";
        public static float workImagePhysWidthMm = 100.0f;
        public static float workImagePhysHeightMm = 100.0f;
        public static float holderHolesIndentMm = 5.0f;
        public static float spaceOuterMm = 6.0f;
        public static float gapBetweenPCBsMm = 2.5f;
        public static float cuttingToolDiameterMm = 1.5f;
        public static float anchorHoleDiameterMm = 0.6f;

        private CNCForm? cncForm;
        private PNPForm? pnpForm;
        private QCForm? qcForm;
        private ExposureForm? exposureForm;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnCNCPre_Click(object sender, EventArgs e)
        {
            gerberFilesDir = tbGerberPath.Text;
            cncForm = new CNCForm();
            cncForm.XMode = 0;
            cncForm?.ShowDialog();
            cncForm?.Dispose();
        }

        private void btnCNCPost_Click(object sender, EventArgs e)
        {
            gerberFilesDir = tbGerberPath.Text;
            cncForm = new CNCForm();
            cncForm.XMode = 1;
            cncForm?.ShowDialog();
            cncForm?.Dispose();
        }

        private void btnQC_Click(object sender, EventArgs e)
        {
            gerberFilesDir = tbGerberPath.Text;
            qcForm = new QCForm();
            qcForm?.ShowDialog();
            qcForm?.Dispose();
        }

        private void btnPNP_Click(object sender, EventArgs e)
        {
            gerberFilesDir = tbGerberPath.Text;
            pnpForm = new PNPForm();
            pnpForm?.ShowDialog();
            pnpForm?.Dispose();
        }

        private void btnExposure_Click(object sender, EventArgs e)
        {
            gerberFilesDir = tbGerberPath.Text;
            exposureForm = new ExposureForm();
            exposureForm?.ShowDialog();
            exposureForm?.Dispose();
        }
    }
}
