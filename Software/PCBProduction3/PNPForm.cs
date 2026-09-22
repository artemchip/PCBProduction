using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PCBProduction3.PNPMachine;

namespace PCBProduction3
{
    public partial class PNPForm : Form
    {
        private PNPMachine ourMachine;
        private int curStep = 0;
        private bool selectingTab = false;
        private UserInputRequiredArgs? curUserInput;
        public int selectedComponentId = -1;

        // Константы моего станка
        private Vector2 sfeeder1 = new Vector2(131.40f, 58.6f);
        private Vector2 sfeeder2 = new Vector2(131.40f, 73.0f);
        private Vector2 sfeeder3 = new Vector2(131.40f, 87.4f);
        private Vector2 sfeeder4 = new Vector2(131.40f, 101.8f);
        private Vector2 sfeeder5 = new Vector2(131.40f, 116.2f);
        private Vector2 ufeeder1 = new Vector2(156.01f, 41.10f);
        private Vector2 ufeeder2 = new Vector2(180.51f, 41.10f);
        private Vector2 ufeeder3 = new Vector2(205.01f, 41.10f);
        private Vector2 ufeeder4 = new Vector2(229.51f, 41.10f);
        private Vector2 ufeeder5 = new Vector2(254.01f, 41.10f);
        private Vector2 lcfeeder = new Vector2(290.51f, 136.60f);

        public PNPForm()
        {
            InitializeComponent();
        }

        private void CNCForm_Load(object sender, EventArgs e)
        {
            try
            {
                ourMachine = new PNPMachine(MainForm.gerberFilesDir, "pnp.xml", 3, pbCamPreview.Width, pbCamPreview.Height, delegate
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
                nudStepsFullTurn.Value = (decimal)ourMachine.GetStepsPerFullCTurn();
                nudAddAngle.Value = (decimal)ourMachine.GetCAddAngle();
                cbCAxisPositive.Checked = (bool)ourMachine.GetCPositiveClockwise();
                tbMachineIP.Text = ourMachine.GetMachineIP();
                InitComponentListViewUI();
                RefreshComponentListViewUI();
                UpdateUIState();
            }
            catch (Exception ex)
            {
                ShowMessageBox("Ошибка при инициализации PNPForm, возможно у вас кривые Gerber-файлы.");
                Close();
                return;
            }
        }

        private void InitComponentListViewUI()
        {
            lvComponents.Enabled = true;
            lvComponents.GridLines = true;
            lvComponents.AllowColumnReorder = false;
            lvComponents.FullRowSelect = true;
            lvComponents.View = View.Details;
            lvComponents.Columns.Clear();
            lvComponents.Columns.Add("#");
            lvComponents.Columns.Add("Обозначение");
            lvComponents.Columns.Add("Величина");
            lvComponents.Columns.Add("X");
            lvComponents.Columns.Add("Y");
            lvComponents.Columns.Add("Угол");
            lvComponents.Columns.Add("Статус");
            foreach (ColumnHeader currColumn in lvComponents.Columns)
            {
                currColumn.Width = (int)((float)(lvComponents.Width - 16) / (float)lvComponents.Columns.Count);
            }
        }

        private int GetSelectedComponentIndexQ()
        {
            int selCmpIdx = -1;
            try
            {
                selCmpIdx = (int)lvComponents.FocusedItem.Tag;
                return selCmpIdx;
            }
            catch (Exception eee)
            {
                return -1;
            }
        }

        private void RefreshComponentListViewUI()
        {
            if (InvokeRequired)
            {
                Invoke(RefreshComponentListViewUI);
            }
            else
            {
                int topItemIndex = 0;
                try
                {
                    if (lvComponents.Items.Count > 0)
                    {
                        topItemIndex = lvComponents.TopItem.Index;
                    }
                }
                catch (Exception ex) { }
                lvComponents.BeginUpdate();
                lvComponents.Items.Clear();
                List<PEComponent> cmpList = new List<PEComponent>();
                if (rbSideTop.Checked)
                {
                    cmpList.AddRange(ourMachine.topComponentList);
                }
                else
                {
                    cmpList.AddRange(ourMachine.bottomComponentList);
                }
                for (int i = 0; i < cmpList.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem(i.ToString());
                    String statusStr = "?";
                    if (cmpList[i].placeStatus == 0)
                    {
                        if (selectedComponentId == cmpList[i].id)
                        {
                            statusStr = "Выбран для установки";
                            lvi.ForeColor = Color.Yellow;
                        }
                        else
                        {
                            statusStr = "Не установлен";
                        }
                    }
                    else if (cmpList[i].placeStatus == 1)
                    {
                        statusStr = "В процессе";
                        lvi.ForeColor = Color.Orange;
                    }
                    else if (cmpList[i].placeStatus == 2)
                    {
                        statusStr = "Установлен";
                        lvi.ForeColor = Color.Green;
                    }
                    lvi.SubItems.Add(cmpList[i].designator);
                    lvi.SubItems.Add(cmpList[i].value);
                    lvi.SubItems.Add(cmpList[i].pXMm.ToString("0.00"));
                    lvi.SubItems.Add(cmpList[i].pYMm.ToString("0.00"));
                    lvi.SubItems.Add(cmpList[i].angle.ToString("0"));
                    lvi.SubItems.Add(statusStr);
                    lvi.Tag = i;
                    lvComponents.Items.Add(lvi);
                }
                lvComponents.EndUpdate();
                try
                {
                    lvComponents.TopItem = lvComponents.Items[topItemIndex];
                }
                catch (Exception ex) { }
                if (selectedComponentId < 0)
                {
                    lblSelComp.Text = "Компонент не выбран";
                }
                else
                {
                    PEComponent? pecTop = null;
                    PEComponent? pecBottom = null;
                    try
                    {
                        pecTop = ourMachine.topComponentList.Where((e) => e.id == selectedComponentId).First();
                    }
                    catch { }
                    try
                    {
                        pecBottom = ourMachine.bottomComponentList.Where((e) => e.id == selectedComponentId).First();
                    }
                    catch { }
                    if (pecTop != null)
                    {
                        lblSelComp.Text = "Выбран " + pecTop.designator + " на стороне Top";
                    }
                    else if (pecBottom != null)
                    {
                        lblSelComp.Text = "Выбран " + pecBottom.designator + " на стороне Bottom";
                    }
                    else
                    {
                        lblSelComp.Text = "Выбран неизвестный компонент";
                    }
                }
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
                btnSFeeder1.Enabled = enableMovementButtonsXY;
                btnSFeeder2.Enabled = enableMovementButtonsXY;
                btnSFeeder3.Enabled = enableMovementButtonsXY;
                btnSFeeder4.Enabled = enableMovementButtonsXY;
                btnSFeeder5.Enabled = enableMovementButtonsXY;
                btnUFeeder1.Enabled = enableMovementButtonsXY;
                btnUFeeder2.Enabled = enableMovementButtonsXY;
                btnUFeeder3.Enabled = enableMovementButtonsXY;
                btnUFeeder4.Enabled = enableMovementButtonsXY;
                btnUFeeder5.Enabled = enableMovementButtonsXY;
                btnLarge.Enabled = enableMovementButtonsXY;
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
                rbCalCAxis.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask();
                nudCCalSteps.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask() && rbCalCAxis.Checked;
                nudAddAngle.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask();
                cbCAxisPositive.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask();
                nudStepsFullTurn.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask();
                btnCalib.Enabled = connectedAndNotFault && !ourMachine.IsBusyWithLongTask();
                tbMachineIP.Enabled = !ourMachine.IsConnected() && !ourMachine.IsConnectPending();
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
                btnRun.Enabled = connectedAndNotFault && ourMachine.IsCalibrated() && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                rbPanelSpecify.Enabled = connectedAndNotFault && ourMachine.IsCalibrated() && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                bool pnpNeedsToBeEnabled = connectedAndNotFault && ourMachine.IsCalibrated() && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated() && ourMachine.IsPanelSelected();
                if (!rbPNP.Enabled)
                {
                    if (pnpNeedsToBeEnabled)
                    {
                        rbPNP.Checked = true;
                    }
                }
                rbPNP.Enabled = pnpNeedsToBeEnabled;
                cbDemoPlaced.Enabled = connectedAndNotFault && ourMachine.IsCalibrated() && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated() && ourMachine.IsPanelSelected();
                rbSideTop.Enabled = connectedAndNotFault && ourMachine.IsCalibrated() && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                rbSideBottom.Enabled = connectedAndNotFault && ourMachine.IsCalibrated() && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                lvComponents.Enabled = connectedAndNotFault && ourMachine.IsCalibrated() && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated();
                nudLyingDeg.Enabled = connectedAndNotFault && ourMachine.IsCalibrated() && !ourMachine.IsBusyWithLongTask() && ourMachine.IsCalibrated() && rbPNP.Checked;
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
                        else if (i == 3)
                        {
                            btnDInstr4.Visible = true;
                            btnDInstr4.Text = curUserInput.Buttons[i];
                        }
                        else if (i == 4)
                        {
                            btnDInstr5.Visible = true;
                            btnDInstr5.Text = curUserInput.Buttons[i];
                        }
                        else if (i == 5)
                        {
                            btnDInstr6.Visible = true;
                            btnDInstr6.Text = curUserInput.Buttons[i];
                        }
                    }
                }
                else
                {
                    lblDInstr.Text = "Ждите указаний.";
                    btnDInstr1.Visible = false;
                    btnDInstr2.Visible = false;
                    btnDInstr3.Visible = false;
                    btnDInstr4.Visible = false;
                    btnDInstr5.Visible = false;
                    btnDInstr6.Visible = false;
                }
                RefreshComponentListViewUI();
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
                ourMachine.ToggleLightFromUI();
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void btnMachineConnect_Click(object sender, EventArgs e)
        {
            if (tbMachineIP.Text.Length <= 0)
            {
                ShowMessageBox("Укажите IP станка.");
                return;
            }
            try
            {
                if (((int)btnMachineConnect.Tag!) == 1)
                {
                    ourMachine.Disconnect();
                }
                else if (((int)btnMachineConnect.Tag!) == 3)
                {
                    curStep = 0;
                    ourMachine.StartConnect(tbMachineIP.Text);
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
            if ((ourMachine.IsMotionStepCalibrated() && rbCalSteps.Checked) || (ourMachine.IsCamOffsetCalibrated() && rbCalCamOffset.Checked) || (ourMachine.IsPNPCalibrated() && rbCalCAxis.Checked))
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
                if (rbCalSteps.Checked)
                {
                    ourMachine.StartCalibrationProcess(0, 0);
                }
                else if (rbCalCamOffset.Checked)
                {
                    ourMachine.StartCalibrationProcess(1, 0);
                }
                else if (rbCalCAxis.Checked)
                {
                    ourMachine.StartCalibrationProcess(2, (int)nudCCalSteps.Value);
                }
                else
                {
                    throw new Exception("Неверно указано что калибровать.");
                }
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
                    ourMachine.SubmitUserEntry(2);
                }
                else if (((Button)sender).Tag == btnDInstr4.Tag)
                {
                    curUserInput = null;
                    ourMachine.SubmitUserEntry(3);
                }
                else if (((Button)sender).Tag == btnDInstr5.Tag)
                {
                    curUserInput = null;
                    ourMachine.SubmitUserEntry(4);
                }
                else if (((Button)sender).Tag == btnDInstr6.Tag)
                {
                    curUserInput = null;
                    ourMachine.SubmitUserEntry(5);
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
                curStep = 1;
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
                if (!rbPNP.Checked && !rbPanelSpecify.Checked)
                {
                    throw new Exception("Неверная операция.");
                }
                else if (rbPNP.Checked)
                {
                    if (selectedComponentId < 0)
                    {
                        throw new Exception("Выберите компонент.");
                    }
                }
                else if (rbPanelSpecify.Checked && ourMachine.IsPanelSelected())
                {
                    if (MessageBox.Show(this, "Панель уже указана, переопределить?", "PCBProduction3", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }
                ourMachine.StartPNPProcess(rbPNP.Checked ? selectedComponentId : -1, (float)nudLyingDeg.Value, cbDemoPlaced.Checked);
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void nudStepsFullTurn_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetStepsPerFullCTurn((int)nudStepsFullTurn.Value);
        }

        private void nudAddAngle_ValueChanged(object sender, EventArgs e)
        {
            ourMachine.SetCAddAngle((float)nudAddAngle.Value);
        }

        private void rbCalCAxis_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUIState();
        }

        private void rbCalCamOffset_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUIState();
        }

        private void rbCalSteps_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUIState();
        }

        private void cbCAxisPositive_CheckedChanged(object sender, EventArgs e)
        {
            ourMachine.SetCPositiveClockwise(cbCAxisPositive.Checked);
        }

        private void ActualizeSelection()
        {
            int selCmpIdx = -1;
            try
            {
                if (lvComponents.FocusedItem == null)
                {
                    throw new Exception("Null");
                }
                else selCmpIdx = (int)lvComponents.FocusedItem.Tag;
            }
            catch (Exception eee)
            {
                ShowMessageBox("Компонент не выбран в списке.");
                return;
            }
            try
            {
                if (rbSideTop.Checked)
                {
                    selectedComponentId = ourMachine.topComponentList[selCmpIdx].id;
                }
                else
                {
                    selectedComponentId = ourMachine.bottomComponentList[selCmpIdx].id;
                }
            }
            catch
            {
                ShowMessageBox("Компонент не найден.");
                return;
            }
            UpdateUIState();
        }

        private void lvComponents_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ActualizeSelection();
        }

        private void lvComponents_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (selectedComponentId >= 0 && rbPNP.Checked)
                {
                    ourMachine.StartPNPProcess(selectedComponentId, (float)nudLyingDeg.Value, cbDemoPlaced.Checked);
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void rbPanelSpecify_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUIState();
        }

        private void rbPNP_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUIState();
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

        private void btnQuickMove_Click(object sender, EventArgs e)
        {
            try
            {
                Vector2? tgtPos = null;
                if (((Button)sender).Tag == btnSFeeder1.Tag)
                {
                    tgtPos = sfeeder1;
                }
                else if (((Button)sender).Tag == btnSFeeder2.Tag)
                {
                    tgtPos = sfeeder2;
                }
                else if (((Button)sender).Tag == btnSFeeder3.Tag)
                {
                    tgtPos = sfeeder3;
                }
                else if (((Button)sender).Tag == btnSFeeder4.Tag)
                {
                    tgtPos = sfeeder4;
                }
                else if (((Button)sender).Tag == btnSFeeder5.Tag)
                {
                    tgtPos = sfeeder5;
                }
                else if (((Button)sender).Tag == btnUFeeder1.Tag)
                {
                    tgtPos = ufeeder1;
                }
                else if (((Button)sender).Tag == btnUFeeder2.Tag)
                {
                    tgtPos = ufeeder2;
                }
                else if (((Button)sender).Tag == btnUFeeder3.Tag)
                {
                    tgtPos = ufeeder3;
                }
                else if (((Button)sender).Tag == btnUFeeder4.Tag)
                {
                    tgtPos = ufeeder4;
                }
                else if (((Button)sender).Tag == btnUFeeder5.Tag)
                {
                    tgtPos = ufeeder5;
                }
                else if (((Button)sender).Tag == btnLarge.Tag)
                {
                    tgtPos = lcfeeder;
                }
                if (tgtPos == null)
                {
                    throw new Exception("Неверное место назначения.");
                }
                else
                {
                    float relX = tgtPos!.Value.X - ourMachine.GetAbsXMm();
                    float relY = tgtPos!.Value.Y - ourMachine.GetAbsYMm();
                    ourMachine.StartMoveManual(relX, relY, 0.0f);
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void rbSideBottom_CheckedChanged(object sender, EventArgs e)
        {
            RefreshComponentListViewUI();
        }

        private void rbSideTop_CheckedChanged(object sender, EventArgs e)
        {
            RefreshComponentListViewUI();
        }
    }
}