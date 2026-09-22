using DirectShowLib;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBProduction3
{
    public partial class CNCForm : Form
    {
        private MillingMachine ourMachine;
        private int curStep = 0;
        private bool selectingTab = false;
        private UserInputRequiredArgs? curUserInput;
        public int XMode = 0;

        public CNCForm()
        {
            InitializeComponent();
        }

        private void CNCForm_Load(object sender, EventArgs e)
        {
            try
            {
                ourMachine = new MillingMachine(MainForm.gerberFilesDir, "cnc.xml", 1, pbCamPreview.Width, pbCamPreview.Height, delegate
                {
                    UpdateUIState();
                }, delegate (object? sender, ErrorEventArgs e)
                {
                    UpdateUIState();
                    ShowMessageBox(e.Msg);
                }, delegate (object? sender, NewCameraPreviewImageArgs e)
                {
                    Image? oldIm = null;
                    if (pbCamPreview.Image != null)
                    {
                        oldIm = pbCamPreview.Image;
                    }
                    try
                    {
                        if (e.Img != null)
                        {
                            pbCamPreview.Image = e.Img.ToBitmap();
                            oldIm?.Dispose();
                        }
                    }
                    catch (Exception ex) { }
                }, delegate (object? sender, UserInputRequiredArgs e)
                {
                    curUserInput = e;
                    UpdateUIState();
                });
                cbImageFlipH.Checked = ourMachine.GetImageFlipH();
                cbImageFlipV.Checked = ourMachine.GetImageFlipV();
                cbImageShowShapes.Checked = ourMachine.GetShowCV();
                cbImageShowAngleGrid.Checked = ourMachine.GetRotateImage();
                cbShowRect.Checked = ourMachine.GetShowRect();
                nudImageAngleDeg.Value = (decimal)ourMachine.GetImageAngleDegrees();
                nudRectWidthMm.Value = (decimal)ourMachine.GetRectWidthMm();
                nudRectHeightMm.Value = (decimal)ourMachine.GetRectHeightMm();
                nudImagePxPerMm.Value = (decimal)ourMachine.GetImagePxPerMm();
                nudFOVXFrom.Value = (decimal)ourMachine.GetFOVXFromPercent();
                nudFOVXTo.Value = (decimal)ourMachine.GetFOVXToPercent();
                nudFOVYFrom.Value = (decimal)ourMachine.GetFOVYFromPercent();
                nudFOVYTo.Value = (decimal)ourMachine.GetFOVYToPercent();
                UpdateUIState();
            }
            catch (Exception ex)
            {
                ShowMessageBox("Ошибка при инициализации CNCForm, возможно у вас кривые Gerber-файлы.");
                Close();
                return;
            }
        }
        private void UpdateUIState()
        {
            if (InvokeRequired)
            {
                Invoke(UpdateUIState);
            }
            else
            {
                // Left Panel
                bool connectedAndNotFault = ourMachine.IsConnected() && !ourMachine.IsFault();
                if (ourMachine.IsConnected() || ourMachine.IsFault())
                {
                    btnMachineConnect.Tag = 1;
                    btnMachineConnect.Text = "Отключиться от станка";
                    btnMachineConnect.Enabled = true;
                }
                else if (ourMachine.IsConnectPending())
                {
                    btnMachineConnect.Tag = 2;
                    btnMachineConnect.Text = "Подключение и настройка...";
                    btnMachineConnect.Enabled = false;
                }
                else
                {
                    btnMachineConnect.Tag = 3;
                    btnMachineConnect.Text = "Подключиться к станку (не используйте USB-hub!)";
                    btnMachineConnect.Enabled = true;
                }
                btnCamNext.Enabled = connectedAndNotFault;
                btnCamPrev.Enabled = connectedAndNotFault;
                cbImageFlipH.Enabled = connectedAndNotFault;
                cbImageFlipV.Enabled = connectedAndNotFault;
                cbImageShowAngleGrid.Enabled = connectedAndNotFault;
                cbImageShowShapes.Enabled = connectedAndNotFault;
                cbShowRect.Enabled = connectedAndNotFault;
                nudFOVXFrom.Enabled = connectedAndNotFault;
                nudFOVXTo.Enabled = connectedAndNotFault;
                nudFOVYFrom.Enabled = connectedAndNotFault;
                nudFOVYTo.Enabled = connectedAndNotFault;
                nudImageAngleDeg.Enabled = connectedAndNotFault;
                nudImagePxPerMm.Enabled = connectedAndNotFault;
                nudRectWidthMm.Enabled = connectedAndNotFault;
                nudRectHeightMm.Enabled = connectedAndNotFault;
                tbZoom.Enabled = connectedAndNotFault;
                tbMoveAmount.Enabled = connectedAndNotFault;
                pnMotionIndicator.BackColor = (ourMachine.IsUIInitiatedMovementPending() || ourMachine.IsBusyWithLongTask()) ? Color.Red : Color.Green;
                bool enableMovementButtonsXY = ourMachine.ShouldEnableUIMoveXY();
                bool enableMovementButtonsZ = ourMachine.ShouldEnableUIMoveZ();
                btnSpindle.Enabled = ourMachine.ShouldEnableSpindleToggleBtn();
                btnXNeg.Enabled = enableMovementButtonsXY;
                btnYNeg.Enabled = enableMovementButtonsXY;
                btnZNeg.Enabled = enableMovementButtonsZ;
                btnXPos.Enabled = enableMovementButtonsXY;
                btnYPos.Enabled = enableMovementButtonsXY;
                btnZPos.Enabled = enableMovementButtonsZ;
                if (ourMachine.IsFault())
                {
                    lblStatus.Text = "Станок в состоянии сбоя, требуется переподключение.";
                }
                else if (ourMachine.IsConnectPending())
                {
                    lblStatus.Text = "Идет подключение к станку и его homing.";
                }
                else if (ourMachine.IsConnected())
                {
                    lblStatus.Text = "Подключено, X: " + ourMachine.GetAbsXMm().ToString("0.00") + ", Y: " + ourMachine.GetAbsYMm().ToString("0.00") + ", Z: " + ourMachine.GetAbsZMm().ToString("0.00") + "\nШагов на мм X: " + ourMachine.GetMotionStepsPerMmX().ToString("0.000") + ", Y: " + ourMachine.GetMotionStepsPerMmY().ToString("0.000") + ", skew: " + ourMachine.GetMotionSkewAngleRad().ToString("0.00000") + "\nСмещение камера-инструмент X: " + ourMachine.GetCamToToolDistX().ToString("0.00") + ", Y: " + ourMachine.GetCamToToolDistY().ToString("0.00");
                }
                else
                {
                    lblStatus.Text = "Не подключено.";
                }

                // Right Panel
                selectingTab = true;
                if (connectedAndNotFault)
                {
                    if (curStep == 0)
                    {
                        procTabControl.SelectTab(1);
                    }
                    else if (curStep == 1)
                    {
                        procTabControl.SelectTab(2);
                    }
                    else if (curStep == 2)
                    {
                        procTabControl.SelectTab(3);
                    }
                }
                else
                {
                    procTabControl.SelectTab(0);
                }
                selectingTab = false;

                // Calibration
                rbCalSteps.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask();
                rbCalCamOffset.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask();
                btnCalib.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask();
                if (curUserInput?.Type == 0 || curUserInput?.Type == CNCGeneralMachine.RESERVED_TYPE)
                {
                    lblCInstr.Text = curUserInput!.Text;
                    btnInstr1.Visible = false;
                    btnInstr2.Visible = false;
                    for (int i = 0; i < curUserInput.Buttons.Count; i++)
                    {
                        if (i == 0)
                        {
                            btnInstr1.Visible = true;
                            btnInstr1.Text = curUserInput.Buttons[i];
                        }
                        else if (i == 1)
                        {
                            btnInstr2.Visible = true;
                            btnInstr2.Text = curUserInput.Buttons[i];
                        }
                    }
                }
                else
                {
                    lblCInstr.Text = "Ждите указаний.";
                    btnInstr1.Visible = false;
                    btnInstr2.Visible = false;
                }
                btnContinueAfterCalib.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();

                // Processing 1
                btnDrill.Enabled = connectedAndNotFault && ourMachine.IsCalibrated() && (!(ourMachine.IsCancellationRequested() && ourMachine.IsDrillingPending()));
                if (ourMachine.IsDrillingPending())
                {
                    btnDrill.Text = "Отмена";
                    btnDrill.Tag = 2;
                }
                else
                {
                    btnDrill.Text = "Создать панель";
                    btnDrill.Tag = 1;
                }
                pbDrilling.Value = (ourMachine.IsDrillingPending()) ? ourMachine.GetDrillingProgressPercent() : 0;
                cbDrillMainHoles.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                nudDWorkpieceThicknessMm.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                nudDSafeHeightMm.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                rbPanelPos1.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                rbPanelPos2.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                if (curUserInput?.Type == 1 || curUserInput?.Type == CNCGeneralMachine.RESERVED_TYPE)
                {
                    lblDInstr.Text = curUserInput!.Text;
                    btnDInstr1.Visible = false;
                    btnDInstr2.Visible = false;
                    btnDInstr3.Visible = false;
                    for (int i = 0; i < curUserInput.Buttons.Count; i++)
                    {
                        if (i == 0)
                        {
                            btnDInstr1.Visible = true;
                            btnDInstr1.Text = curUserInput.Buttons[i];
                        }
                        else if (i == 1)
                        {
                            btnDInstr2.Visible = true;
                            btnDInstr2.Text = curUserInput.Buttons[i];
                        }
                        else if (i == 2)
                        {
                            btnDInstr3.Visible = true;
                            btnDInstr3.Text = curUserInput.Buttons[i];
                        }
                    }
                }
                else
                {
                    lblDInstr.Text = "Ждите указаний.";
                    btnDInstr1.Visible = false;
                    btnDInstr2.Visible = false;
                    btnDInstr3.Visible = false;
                }

                // Processing 2
                btnTransform.Enabled = connectedAndNotFault && ourMachine.IsCalibrated() && (!(ourMachine.IsCancellationRequested() && ourMachine.IsTransformPending()));
                btnDepanelizePreview.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                cbInvertX.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                cbInvertY.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                if (ourMachine.IsTransformPending())
                {
                    btnTransform.Text = "Отмена";
                    btnTransform.Tag = 2;
                }
                else
                {
                    btnTransform.Text = "Преобразить панель";
                    btnTransform.Tag = 1;
                }
                pbTransforming.Value = (ourMachine.IsTransformPending()) ? ourMachine.GetTransformProgressPercent() : 0;
                rbDrill.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                rbDepanelize.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                if (curUserInput?.Type == 2 || curUserInput?.Type == CNCGeneralMachine.RESERVED_TYPE)
                {
                    lblTInstr.Text = curUserInput!.Text;
                    btnTInstr1.Visible = false;
                    btnTInstr2.Visible = false;
                    btnTInstr3.Visible = false;
                    for (int i = 0; i < curUserInput.Buttons.Count; i++)
                    {
                        if (i == 0)
                        {
                            btnTInstr1.Visible = true;
                            btnTInstr1.Text = curUserInput.Buttons[i];
                        }
                        else if (i == 1)
                        {
                            btnTInstr2.Visible = true;
                            btnTInstr2.Text = curUserInput.Buttons[i];
                        }
                        else if (i == 2)
                        {
                            btnTInstr3.Visible = true;
                            btnTInstr3.Text = curUserInput.Buttons[i];
                        }
                    }
                }
                else
                {
                    lblTInstr.Text = "Ждите указаний.";
                    btnTInstr1.Visible = false;
                    btnTInstr2.Visible = false;
                    btnTInstr3.Visible = false;
                }
            }
        }

        private void tbZoom_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetZoom(tbZoom.Value);
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            try
            {
                float mmToMove = (float)Math.Pow(10.0f, tbMoveAmount.Value);
                float qX = 0.0f;
                float qY = 0.0f;
                float qZ = 0.0f;
                if (((Button)sender).Tag == btnXPos.Tag)
                {
                    qX = mmToMove;
                }
                else if (((Button)sender).Tag == btnXNeg.Tag)
                {
                    qX = -mmToMove;
                }
                else if (((Button)sender).Tag == btnYPos.Tag)
                {
                    qY = -mmToMove;
                }
                else if (((Button)sender).Tag == btnYNeg.Tag)
                {
                    qY = mmToMove;
                }
                else if (((Button)sender).Tag == btnZPos.Tag)
                {
                    qZ = mmToMove;
                }
                else if (((Button)sender).Tag == btnZNeg.Tag)
                {
                    qZ = -mmToMove;
                }
                else
                {
                    throw new Exception("Непонятное движение.");
                }
                ourMachine.StartMoveManual(qX, qY, qZ);
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void btnLight_Click(object sender, EventArgs e)
        {
            try
            {
                ourMachine.ToggleSpindleFromUI();
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void btnMachineConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (((int)btnMachineConnect.Tag!) == 1)
                {
                    ourMachine.Disconnect();
                }
                else if (((int)btnMachineConnect.Tag!) == 3)
                {
                    curStep = 0;
                    ourMachine.StartConnect("");
                }
                else
                {
                    throw new Exception("Неверное состояние.");
                }
            }
            catch (Exception exc)
            {
                ShowMessageBox(exc.Message);
            }
        }

        private void cbImageFlipH_CheckedChanged(object sender, EventArgs e)
        {
            ourMachine.SetImageFlipH(cbImageFlipH.Checked);
        }

        private void cbImageFlipV_CheckedChanged(object sender, EventArgs e)
        {
            ourMachine.SetImageFlipV(cbImageFlipV.Checked);
        }

        private void cbImageShowShapes_CheckedChanged(object sender, EventArgs e)
        {
            ourMachine.SetShowCV(cbImageShowShapes.Checked);
        }

        private void cbImageShowAngleGrid_CheckedChanged(object sender, EventArgs e)
        {
            ourMachine.SetRotateImage(cbImageShowAngleGrid.Checked);
        }

        private void cbShowRect_CheckedChanged(object sender, EventArgs e)
        {
            ourMachine.SetShowRect(cbShowRect.Checked);
        }

        private void nudImageAngleDeg_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetImageAngleDegrees((float)nudImageAngleDeg.Value);
        }

        private void nudRectWidthMm_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetRectWidthMm((float)nudRectWidthMm.Value);
        }

        private void nudRectHeightMm_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetRectHeightMm((float)nudRectHeightMm.Value);
        }

        private void nudImagePxPerMm_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetImagePxPerMm((float)nudImagePxPerMm.Value);
        }

        private void tbZoom_Scroll(object sender, EventArgs e) { }

        private void nudFOVXFrom_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetFOVXFromPercent((int)nudFOVXFrom.Value);
        }

        private void nudFOVXTo_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetFOVXToPercent((int)nudFOVXTo.Value);
        }

        private void nudFOVYFrom_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetFOVYFromPercent((int)nudFOVYFrom.Value);
        }

        private void nudFOVYTo_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetFOVYToPercent((int)nudFOVYTo.Value);
        }

        private void btnCamPrev_Click(object sender, EventArgs e)
        {
            ourMachine.PrevCameraPreview();
        }

        private void btnCamNext_Click(object sender, EventArgs e)
        {
            ourMachine.NextCameraPreview();
        }

        private void CNCForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ourMachine != null)
            {
                if (ourMachine.IsConnected())
                {
                    ourMachine.Disconnect();
                }
            }
            try
            {
                pbCamPreview?.Image?.Dispose();
            }
            catch { }
        }

        public void ShowMessageBox(String text)
        {
            if (InvokeRequired)
            {
                Invoke(ShowMessageBox, text);
            }
            else
            {
                MessageBox.Show(this, text);
            }
        }

        private void procTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!selectingTab)
            {
                e.Cancel = true;
            }
        }

        private void btnPrintCalib_Click(object sender, EventArgs e)
        {
            try
            {
                ourMachine.PrintCalibPattern();
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void btnCalib_Click(object sender, EventArgs e)
        {
            // Уже откалибровано?
            if ((ourMachine.IsMotionStepCalibrated() && rbCalSteps.Checked) || (ourMachine.IsCamOffsetCalibrated() && rbCalCamOffset.Checked))
            {
                if (MessageBox.Show(this, "Ваш станок уже откалиброван, точно повторить процесс?", "PCBProduction3", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            // Z Safe
            if (MessageBox.Show(this, "В константах программы верно указан безопасный Z (calibZMm)?", "PCBProduction3", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            // Делаем
            try
            {
                if (!rbCalSteps.Checked && !rbCalCamOffset.Checked)
                {
                    throw new Exception("Неверно указано что калибровать.");
                }
                ourMachine.StartCalibrationProcess(rbCalCamOffset.Checked ? 1 : 0, 0);
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void btnUserInput_Click(object sender, EventArgs e)
        {
            try
            {
                if (((Button)sender).Tag == btnInstr1.Tag)
                {
                    curUserInput = null;
                    ourMachine.SubmitUserEntry(0);
                }
                else if (((Button)sender).Tag == btnInstr2.Tag)
                {
                    curUserInput = null;
                    ourMachine.SubmitUserEntry(1);
                }
                else if (((Button)sender).Tag == btnDInstr3.Tag)
                {
                    curUserInput = null;
                    ourMachine.SubmitUserEntry(1);
                }
                else
                {
                    throw new Exception("Неверный ввод.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void btnContinueAfterCalib_Click(object sender, EventArgs e)
        {
            if (ourMachine.IsCalibrated())
            {
                if (XMode <= 0)
                {
                    curStep = 1;
                }
                else
                {
                    curStep = 2;
                }
                UpdateUIState();
            }
            else
            {
                ShowMessageBox("Калибровка не выполнена.");
                return;
            }
        }

        private void btnDrill_Click(object sender, EventArgs e)
        {
            // Начать или наоборот отменить
            try
            {
                if (((int)btnDrill.Tag!) == 1)
                {
                    // Начать, оценить время
                    float totalZDist = (float)(nudDSafeHeightMm.Value + nudDWorkpieceThicknessMm.Value);
                    float timeSec = ourMachine.GetDrillingTimeSeconds(cbDrillMainHoles.Checked, totalZDist);
                    if (MessageBox.Show(this, "Начать создание панели? Процесс займет около " + (timeSec / 60f).ToString("0") + " минут. Убедитесь, что в константах программы указаны правильные скорости подачи, безопасные Z и скорости шпинделя", "PCBProduction3", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        int panelIdx = -1;
                        if (rbPanelPos1.Checked)
                        {
                            panelIdx = 0;
                        }
                        else if (rbPanelPos2.Checked)
                        {
                            panelIdx = 1;
                        }
                        else
                        {
                            ShowMessageBox("Неверное местоположение панели.");
                            return;
                        }
                        ourMachine.StartDrillingProcess(cbDrillMainHoles.Checked, panelIdx, (float)nudDSafeHeightMm.Value, (float)nudDWorkpieceThicknessMm.Value);
                    }
                }
                else if (((int)btnDrill.Tag!) == 2)
                {
                    // Отменить
                    ourMachine.CancelOp();
                    curUserInput = null;
                }
                else
                {
                    throw new Exception("Неверный тэг кнопки.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void btnTransform_Click(object sender, EventArgs e)
        {
            // Начать или наоборот отменить
            try
            {
                if (((int)btnTransform.Tag!) == 1)
                {
                    // Начать, оценить время
                    float timeSec = ourMachine.GetTransformTimeSeconds(rbDepanelize.Checked);
                    if (MessageBox.Show(this, "Начать преображение панели? Процесс займет около " + (timeSec / 60f).ToString("0") + " минут.", "PCBProduction3", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ourMachine.StartTransformProcess(rbDepanelize.Checked);
                    }
                }
                else if (((int)btnTransform.Tag!) == 2)
                {
                    // Отменить
                    ourMachine.CancelOp();
                    curUserInput = null;
                }
                else
                {
                    throw new Exception("Неверный тэг кнопки.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void pbCamPreview_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ourMachine.PictureBoxClick(e.X, e.Y);
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void btnDepanelizePreview_Click(object sender, EventArgs e)
        {
            try
            {
                pbDepanelizePreview.Image?.Dispose();
            }
            catch { }
            try
            {
                ELayer topLayer = GerberFolderParser.loadedLayers.Where((e) => e.isCopperOuter).FirstOrDefault();
                ELayer bottomLayer = GerberFolderParser.loadedLayers.Where((e) => e.isCopperOuter).LastOrDefault();
                Bitmap bmpTool = ourMachine.GetGreenDepanelizeBitmap(600);
                using (Bitmap bmpTop = GerberFolderParser.DrawPanelFromGbrLayerFile(topLayer.fileName, GerberFolderParser.gbrProfileFile, false, false, "", 600))
                {
                    using (Bitmap bmpBottom = GerberFolderParser.DrawPanelFromGbrLayerFile(bottomLayer.fileName, GerberFolderParser.gbrProfileFile, false, false, "", 600))
                    {
                        using (Graphics g = Graphics.FromImage(bmpTool))
                        {
                            using (var attributes = new ImageAttributes())
                            {
                                float[][] mxRed = new float[][]
                                {
                            new float[] { 1, 0, 0, 0, 0 },
                            new float[] { 0, 0, 0, 0, 0 },
                            new float[] { 0, 0, 0, 0, 0 },
                            new float[] { 0, 0, 0, 0.3f, 0 },
                            new float[] { 0, 0, 0, 0, 0 }
                                };
                                attributes.SetColorMatrix(new ColorMatrix(mxRed));
                                g.DrawImage(bmpTop, new Rectangle(new Point(0, 0), bmpTool.Size), 0f, 0f, bmpTop.Width, bmpTop.Height, GraphicsUnit.Pixel, attributes);
                            }
                            using (var attributes = new ImageAttributes())
                            {
                                float[][] mxBlue = new float[][]
                                {
                            new float[] { 0, 0, 0, 0, 0 },
                            new float[] { 0, 0, 0, 0, 0 },
                            new float[] { 0, 0, 1, 0, 0 },
                            new float[] { 0, 0, 0, 0.3f, 0 },
                            new float[] { 0, 0, 0, 0, 0 }
                                };
                                attributes.SetColorMatrix(new ColorMatrix(mxBlue));
                                g.DrawImage(bmpBottom, new Rectangle(new Point(0, 0), bmpTool.Size), 0f, 0f, bmpTop.Width, bmpTop.Height, GraphicsUnit.Pixel, attributes);
                            }
                        }
                    }
                }
                pbDepanelizePreview.Image = bmpTool;
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void cbInvertX_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                pbDepanelizePreview.Image?.Dispose();
            }
            catch { }
            ourMachine.ProcessIndividualCutouts(GerberFolderParser.gbrProfileFile, cbInvertX.Checked, cbInvertY.Checked);
        }

        private void cbInvertY_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                pbDepanelizePreview.Image?.Dispose();
            }
            catch { }
            ourMachine.ProcessIndividualCutouts(GerberFolderParser.gbrProfileFile, cbInvertX.Checked, cbInvertY.Checked);
        }
    }
}