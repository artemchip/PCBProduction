using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBProduction2
{
    public partial class GalvForm : Form
    {
        public Thread? stirThread;
        public bool shouldCancelStir = false;
        public bool stirCmpl = false;
        public SerialPort? grblPort;

        public GalvForm()
        {
            InitializeComponent();
        }

        private void btnStir_Click(object sender, EventArgs e)
        {
            if (btnStir.Tag == null)
            {
                stirThread = new Thread(StirProcess);
                stirThread.Start();
                btnStir.Tag = 1;
                btnStir.Text = "Стоп";
                rbGalv.Enabled = false;
                rbChemical.Enabled = false;
            }
            else
            {
                btnStir.Enabled = false;
                shouldCancelStir = true;
            }
        }

        private void GrblPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            grblPort?.ReadExisting();
            stirCmpl = true;
        }

        public void StirProcess()
        {
            shouldCancelStir = false;
            try
            {
                string[] ports = SerialPort.GetPortNames();
                if (ports.Length < 1)
                {
                    throw new Exception("Grbl не найден.");
                }
                grblPort = null;
                int pIdx = 0;
                while (!shouldCancelStir)
                {
                    grblPort = new SerialPort(ports[pIdx], 115200);
                    grblPort.Open();
                    grblPort.WriteTimeout = 3000;
                    grblPort.ReadTimeout = 3000;
                    grblPort.Write("?");
                    String fLine = grblPort.ReadLine();
                    if (fLine.ToLower().StartsWith("<"))
                    {
                        break;
                    }
                    pIdx++;
                    if (pIdx >= ports.Length)
                    {
                        throw new Exception("Grbl не найден.");
                    }
                }
                if (shouldCancelStir)
                {
                    return;
                }
                grblPort!.DataReceived += GrblPort_DataReceived;
                stirCmpl = false;
                grblPort.WriteLine("$X");
                while (!stirCmpl)
                {
                    Thread.Sleep(10);
                }
                grblPort.WriteLine("$110=2000");
                while (!stirCmpl)
                {
                    Thread.Sleep(10);
                }
                grblPort.WriteLine("$100=320");
                while (!stirCmpl)
                {
                    Thread.Sleep(10);
                }
                grblPort.WriteLine("$120=500");
                while (!stirCmpl)
                {
                    Thread.Sleep(10);
                }
                grblPort.WriteLine("$130=5000");
                while (!stirCmpl)
                {
                    Thread.Sleep(10);
                }
                grblPort.WriteLine("$102=800");
                while (!stirCmpl)
                {
                    Thread.Sleep(10);
                }
                grblPort.WriteLine("$112=5000");
                while (!stirCmpl)
                {
                    Thread.Sleep(10);
                }
                grblPort.WriteLine("G91");
                while (!stirCmpl)
                {
                    Thread.Sleep(10);
                }
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (!shouldCancelStir)
                {
                    if (IsLinearDrive()) {
                        stirCmpl = false;
                        grblPort.WriteLine("G0 X14");
                        while (!stirCmpl)
                        {
                            Thread.Sleep(10);
                        }
                        stirCmpl = false;
                        grblPort.WriteLine("G0 X-14");
                        while (!stirCmpl)
                        {
                            Thread.Sleep(10);
                        }
                    } else
                    {
                        stirCmpl = false;
                        grblPort.WriteLine("G1 Z5 F250");
                        while (!stirCmpl)
                        {
                            Thread.Sleep(10);
                        }
                    }
                    SetStirLabelText(sw.Elapsed.ToString());
                }
                sw.Stop();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }
            finally
            {
                SetStirLabelText("");
                SetStirButtonToInitial();
                grblPort?.Close();
            }
        }

        private void SetStirButtonToInitial()
        {
            if (btnStir.InvokeRequired)
            {
                btnStir.Invoke(SetStirButtonToInitial);
            }
            else
            {
                btnStir.Enabled = true;
                btnStir.Tag = null;
                btnStir.Text = "Перемешивать";
                rbChemical.Enabled = true;
                rbGalv.Enabled = true;
            }
        }

        private void SetStirLabelText(String text)
        {
            if (lblStirTime.InvokeRequired)
            {
                lblStirTime.Invoke(SetStirLabelText, text);
            }
            else
            {
                lblStirTime.Text = text;
            }
        }

        private void btnGenFile_Click(object sender, EventArgs e)
        {
            List<String> gcodeLines = new List<string>();
            gcodeLines.Add("$X");
            gcodeLines.Add("$110=2000");
            gcodeLines.Add("$100=320");
            gcodeLines.Add("$120=500");
            gcodeLines.Add("$130=5000");
            gcodeLines.Add("$102=800");
            gcodeLines.Add("$112=5000");
            gcodeLines.Add("G91");
            int secondCount = (int)(60 * nudTimeMinutes.Value);
            if (IsLinearDrive())
            {
                int cycleCount = (int)((float)secondCount * 1.025f);
                for (int i = 0; i < cycleCount; i++)
                {
                    gcodeLines.Add("G0 X14");
                    gcodeLines.Add("G0 X-14");
                }
            } else
            {
                int cycleCount = (int)((float)secondCount * 0.84f);
                for (int i = 0; i < cycleCount; i++)
                {
                    gcodeLines.Add("G1 Z5 F250");
                }
            }
            gcodeSaveDialog.Reset();
            gcodeSaveDialog.DefaultExt = "nc";
            gcodeSaveDialog.Filter = "GCode|*.nc";
            gcodeSaveDialog.Title = "GCode";
            if (gcodeSaveDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(gcodeSaveDialog.FileName, gcodeLines);
            }
        }

        public bool IsLinearDrive()
        {
            if (InvokeRequired)
            {
                return this.Invoke(IsLinearDrive);
            } else
            {
                return rbChemical.Checked;
            }
        }
    }
}