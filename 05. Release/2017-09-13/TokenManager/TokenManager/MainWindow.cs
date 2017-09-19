﻿using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TokenManager.common;
using TokenManager.dialog;
using TokenManager.test;

namespace TokenManager
{
    public partial class MainWindow : Form, Observer
    {
        private const int CS_DROPSHADOW = 0x20000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
        //Logger for this class
        private static readonly log4net.ILog _LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //Certificates list
        private List<X509Certificate2> _certList = null;
        //Instance renew certificate
        private CertificateRenew _renew = null;
        //Row index of certificate
        private int _rowIndex = -1;
        //Thread manage linkLabel text
        private Thread _linkLabelThread = null;

        

        public MainWindow()
        {
            InitializeComponent();

            //Init certificate table with header row
            _initCertTable();
            //Init side menu state
            _initSideMenu();

            _renew = new CertificateRenew(this);

            //Load all cert in token slot and show in cert table
            int result = _checkTokenLockedAndLoadCert();
            UsbNotification.RegisterUsbDeviceNotification(this.Handle);
            //
            if (TOKEN_PLUGED == result)
            {
                loaded = true;
                unloaded = false;

                //Check and update certificate list to TMS
                ThreadStart child = new ThreadStart(updateCertList);
                Thread parent = new Thread(child);
                parent.Start();
            }
            //Install CA certificate chain if not exist in Window store
            _checkInstallCA();

            _initSkin();
        }

        /// <summary>
        /// Check and install CA certificate chain to Window store.
        /// </summary>
        private void _checkInstallCA()
        {
            ThreadStart child = new ThreadStart(CspUtil.InstallCAChain);
            Thread parentThread = new Thread(child);
            parentThread.Start();
        }

        private void _initSkin()
        {
            skinLightSelected.Size = skinLightBtn.Size;
            skinDarkSelected.Size = skinDarkBtn.Size;
            skinLightSelected.Visible = false;
            skinDarkSelected.Visible = false;


            string skin = Properties.Settings.Default.SKIN;
            if ("LIGHT".Equals(skin))
            {
                var bm = new Bitmap(skinLightSelected.BackgroundImage, new Size(skinLightSelected.Width -3, skinLightSelected.Height - 3));
                skinLightSelected.BackgroundImage = bm;
                skinLightSelected.Location = skinLightBtn.Location;
                skinLightSelected.Visible = true;

                cusTomeSkin();
            }
            else
            {
                var bm = new Bitmap(skinDarkSelected.BackgroundImage, new Size(skinDarkSelected.Width, skinDarkSelected.Height));
                skinDarkSelected.BackgroundImage = bm;
                skinDarkSelected.Location = skinDarkBtn.Location;
                skinDarkSelected.Visible = true;

                darkSkin();
            }

        }
        private void panelpanelbordercolor1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Red,
            e.ClipRectangle.Left,
            e.ClipRectangle.Top,
            e.ClipRectangle.Width - 1,
            e.ClipRectangle.Height - 1);
            base.OnPaint(e);
        }
        private Color TextColor = Color.Gray;
        public static Color HeaderBack = Color.FromArgb(27, 36, 46);
        private void darkSkin()
        {
            Color backColor = Color.FromArgb(41, 56, 71);
            Color HeaderTableColor = Color.FromArgb(18, 24, 31);
            TextColor = Color.Silver;
            Color DesText = Color.FromArgb(122, 125, 150);
            Color TableBack = backColor;
            HeaderBack = Color.FromArgb(27, 36, 46);

            Color sideMenuBack = Color.FromArgb(79, 171, 217);

            BackColor = backColor;
            this.panel3.BackColor = backColor;
            this.ErrorLbl.BackColor = backColor;
            this.certGrid.BackgroundColor = backColor;
            this.certTab.BackColor = backColor;
            this.certPane.BackColor = backColor;
            this.certTabTitlePanel.BackColor = backColor;
            this.contentPanel.BackColor = backColor;

            bunifuImageButton1.BackColor = HeaderBack;

            certGrid.ColumnHeadersDefaultCellStyle.BackColor = HeaderTableColor;
            certGrid.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
            certGrid.DefaultCellStyle.BackColor = TableBack;
            certGrid.DefaultCellStyle.ForeColor = TextColor;
            certGrid.DefaultCellStyle.SelectionBackColor = HeaderBack;

            certPanelTitle.ForeColor = TextColor;

            header.BackColor = HeaderBack;
            appTitle.ForeColor = Color.White;

            toolTab.BackColor = BackColor;
            toolTabTitle.ForeColor = TextColor;
            unlockBox.BackColor = HeaderTableColor;
            unlockBoxTitle.ForeColor = TextColor;
            unlockBoxDex.ForeColor = DesText;

            changePinBox.BackColor = Color.FromArgb(33, 44, 56);
            changePinBoxTitle.ForeColor = TextColor;
            changePinBoxDes.ForeColor = DesText;


            renewCertBox.BackColor = HeaderTableColor;
            renewCertBoxTitle.ForeColor = TextColor;
            renewCertBoxDes.ForeColor = DesText;

            updateProfileBox.BackColor = Color.FromArgb(33, 44, 56);
            updateProfileBoxTitle.ForeColor = TextColor;
            updateProfileBoxDes.ForeColor = DesText;

            panel2.Height = 4 * unlockBox.Height;

            configTab.BackColor = BackColor;
            configTabTitle.ForeColor = TextColor;
            changeSkinLbl.ForeColor = TextColor;

            helpTab.BackColor = BackColor;
            tabPage1.BackColor = BackColor;
            tabPage2.BackColor = BackColor;
            aboutAppName.ForeColor = TextColor;
            contactTitle.ForeColor = TextColor;
            phoneTitle.ForeColor = TextColor;
            phoneValue.ForeColor = TextColor;
            emailTitle.ForeColor = TextColor;
            mailLink.ForeColor = TextColor;
            webTitle.ForeColor = TextColor;
            webLink.ForeColor = TextColor;

            aboutBtn.DisabledColor = BackColor;
            if (!aboutBtn.Enabled)
            {
                aboutBtn.Textcolor = TextColor;
                aboutBtn.BackColor = BackColor;
                aboutBtn.Activecolor = BackColor;
            }
            else
            {
                aboutBtn.Textcolor = Color.White;
            }
            helpTabBtn.DisabledColor = BackColor;
            if (!helpTabBtn.Enabled)
            {
                helpTabBtn.Textcolor = TextColor;
                helpTabBtn.BackColor = BackColor;
            }
            else
            {
                helpTabBtn.Textcolor = Color.White;
            }

            downThueBox.ForeColor = TextColor;
            downBHXHBox.ForeColor = TextColor;
        }

        private void cusTomeSkin()
        {
            Color backColor = Color.FromArgb(225, 238, 245);
            TextColor = Color.FromArgb(64, 65, 66);
            Color HeaderTableColor = Color.FromArgb(242, 242, 242);
            Color DesText = Color.FromArgb(122, 125, 150);
            Color TableBack = Color.FromArgb(250, 250, 250);
            HeaderBack = Color.FromArgb(20, 143, 204);

            Color sideMenuBack = Color.FromArgb(79, 171, 217);

            bunifuImageButton1.BackColor = HeaderBack;

            BackColor = backColor;
            this.panel3.BackColor = backColor;
            this.ErrorLbl.BackColor = backColor;
            this.certGrid.BackgroundColor = backColor;
            this.certTab.BackColor = backColor;
            this.certPane.BackColor = backColor;
            this.certTabTitlePanel.BackColor = backColor;
            this.contentPanel.BackColor = backColor;

            certGrid.ColumnHeadersDefaultCellStyle.BackColor = HeaderTableColor;
            certGrid.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
            certGrid.DefaultCellStyle.BackColor = TableBack;
            certGrid.DefaultCellStyle.ForeColor = TextColor;
            certGrid.DefaultCellStyle.SelectionBackColor = HeaderBack;

            certPanelTitle.ForeColor = TextColor;

            header.BackColor = HeaderBack;
            appTitle.ForeColor = Color.White;

            toolTab.BackColor = BackColor;
            toolTabTitle.ForeColor = TextColor;
            unlockBox.BackColor = Color.FromArgb(243, 243, 243);
            unlockBoxTitle.ForeColor = TextColor;
            unlockBoxDex.ForeColor = DesText;

            changePinBox.BackColor = TableBack;
            changePinBoxTitle.ForeColor = TextColor;
            changePinBoxDes.ForeColor = DesText;

            
            renewCertBox.BackColor = Color.FromArgb(243, 243, 243);
            renewCertBoxTitle.ForeColor = TextColor;
            renewCertBoxDes.ForeColor = DesText;

            updateProfileBox.BackColor = TableBack;
            updateProfileBoxTitle.ForeColor = TextColor;
            updateProfileBoxDes.ForeColor = DesText;

            panel2.Height = 4 * unlockBox.Height;

            configTab.BackColor = BackColor;
            configTabTitle.ForeColor = TextColor;
            changeSkinLbl.ForeColor = TextColor;

            helpTab.BackColor = BackColor;
            tabPage1.BackColor = BackColor;
            tabPage2.BackColor = BackColor;
            aboutAppName.ForeColor = TextColor;
            contactTitle.ForeColor = TextColor;
            phoneTitle.ForeColor = TextColor;
            phoneValue.ForeColor = TextColor;
            emailTitle.ForeColor = TextColor;
            mailLink.ForeColor = TextColor;
            webTitle.ForeColor = TextColor;
            webLink.ForeColor = TextColor;

            aboutBtn.DisabledColor = BackColor;
            if (!aboutBtn.Enabled)
            {
                aboutBtn.Textcolor = TextColor;
                aboutBtn.BackColor = BackColor;
                aboutBtn.Activecolor = BackColor;
            }
            else
            {
                aboutBtn.Textcolor = Color.White;
            }
            helpTabBtn.DisabledColor = BackColor;
            if (!helpTabBtn.Enabled)
            {
                helpTabBtn.Textcolor = TextColor;
                helpTabBtn.BackColor = BackColor;
            }
            else
            {
                helpTabBtn.Textcolor = Color.White;
            }

            downThueBox.ForeColor = TextColor;
            downBHXHBox.ForeColor = TextColor;
        }

        private int  _checkTokenLockedAndLoadCert()
        {
            if (Pkcs11Util.CheckTokenLocked())
            {
                this._certList = new List<X509Certificate2>();
                ErrorLbl.Text = "VNPT CA Token đã bị khóa.";
                ErrorLbl.Visible = true;
                return -1;
            }
            else
            {
                return ReloadCertTable();
            }
        }

        private void _initCertTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("#", typeof(int));
            table.Columns.Add("Chủ sở hữu", typeof(string));
            table.Columns.Add("Từ ngày", typeof(string));
            table.Columns.Add("Đến ngày", typeof(string));
            certGrid.DataSource = table;
        }

        private void _initSideMenu()
        {
            string sideMenuState = "" + Properties.Settings.Default[TokenManagerConstants.SIDE_MENU_STATE];
            if ("CLOSE".Equals(sideMenuState))
            {
                logo.Visible = false;
                sideMenu.Width = 60;
            }
            else
            {
                logo.Visible = true;
                sideMenu.Width = 220;
            }

            bunifuSeparator3.Width = 650 - bunifuSeparator3.Left;
            helpChildTab.Width = 650 - aboutBtn.Left;
            downThueBox.Width = 650 - aboutBtn.Left - 20;
        }

        //---------------------------------------------------------------------------------
        /// <summary>
        /// Handle USB plug/unplug event
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == UsbNotification.WmDevicechange)
            {
                switch ((int)m.WParam)
                {
                    case UsbNotification.DbtDeviceremovecomplete:
                        if (!unloaded)
                        {
                            Usb_DeviceRemoved();
                        }
                        break;
                    case UsbNotification.DbtDevicearrival:
                        if (!loaded)
                        {
                            Usb_DeviceAdded();
                        }
                        break;
                }
            }
        }

        private void Usb_DeviceRemoved()
        {
            int result = ReloadCertTable();
            if(TOKEN_UNPLUGED == result)
            {
                unloaded = true;
                loaded = false;
                SystemUtil.ShowNotify("VNPT CA Token đã được rút ra");
            }
        }


        bool loaded = false;
        bool unloaded = false;

        private void Usb_DeviceAdded()
        {
            int result = _checkTokenLockedAndLoadCert();

            if(TOKEN_PLUGED == result)
            {
                loaded = true;
                unloaded = false;
                SystemUtil.ShowNotify("VNPT CA Token đã được cắm vào máy");

                //Check and update certificate list to TMS
                ThreadStart child = new ThreadStart(updateCertList);
                Thread parent = new Thread(child);
                parent.Start();
            }
        }
        //---------------------------------------------------------------------------------

        private void updateCertList()
        {
            //Kiem tra va cap nhat danh sach cert trong token len TMS
            _renew.UpdateCertList();
        }

        /// <summary>
        /// Click Close button, kill process to exit program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeBtn_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
            else
            {
                _closeApp();
            }
        }

        private void _closeApp()
        {
            this.Close();
        }

        /// <summary>
        /// Click on side menu button, resize menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bunifuImageButton2_Click_1(object sender, EventArgs e)
        {
            if (sideMenu.Width == 220)
            {
                sideMenu.Visible = false;
                logo.Visible = false;
                sideMenu.Width = 60;
                minimizeSlideMenu.ShowSync(sideMenu);
                Properties.Settings.Default[TokenManagerConstants.SIDE_MENU_STATE] = "CLOSE";
                Properties.Settings.Default.Save();
            }
            else
            {
                sideMenu.Visible = false;
                logo.Visible = false;
                sideMenu.Width = 220;
                SideMenuShow.ShowSync(sideMenu);
                minimizeSlideMenu.ShowSync(logo);
                Properties.Settings.Default[TokenManagerConstants.SIDE_MENU_STATE] = "OPEN";
                Properties.Settings.Default.Save();
            }
        }


        /// <summary>
        /// Handle switch tabs Certificate, Tool, Configuration, Help.
        /// </summary>
        private const int _CERTIFICATE_TAB = 0;
        private const int _TOOL_TAB = 1;
        private const int _CONFIGURATION_TAB = 2;
        private const int _HELP_TAB = 3;
        private readonly int TOKEN_UNPLUGED = 1;
        private readonly int TOKEN_PLUGED = 0;

        /// <summary>
        /// Change tabs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void certBtn_Click(object sender, EventArgs e)
        {
            Bunifu.Framework.UI.BunifuFlatButton btnSource = ((Bunifu.Framework.UI.BunifuFlatButton)sender);
            if (!btnSource.Enabled)
            {
                return;
            }
            bunifuSeparator1.Height = btnSource.Height - 1;
            bunifuSeparator1.Top = btnSource.Top;

            if (btnSource == certBtn)
            {
                tabControl.SelectedIndex = _CERTIFICATE_TAB;
            }
            else if (btnSource == toolBtn)
            {
                tabControl.SelectedIndex = _TOOL_TAB;
            }
            else if (btnSource == configBtn)
            {
                tabControl.SelectedIndex = _CONFIGURATION_TAB;
            }
            else if (btnSource == helpBtn)
            {
                tabControl.SelectedIndex = _HELP_TAB;
            }
        }


        /// <summary>
        /// Mo menustrip khi chuot phai vao row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void certGrid_RowContextMenuStripNeeded(object sender, DataGridViewRowContextMenuStripNeededEventArgs e)
        {
            _rowIndex = e.RowIndex;
            e.ContextMenuStrip = certSelectMenu;
            certGrid.Rows[_rowIndex].Selected = true;
        }


        /// <summary>
        /// Chọn hiển thị chứng thư số từ menustrip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewCertificateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_certList == null || _certList.Count() == 0)
            {
                return;
            }
            X509Certificate2 cert = _certList.ElementAt(_rowIndex);
            if (cert == null)
            {
                return;
            }
            CertUtil.viewCert(cert);
        }


        /// <summary>
        /// Chọn xem số serial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewSerialNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_certList == null || _certList.Count() == 0)
            {
                return;
            }
            X509Certificate2 cert = _certList.ElementAt(_rowIndex);
            SerialDialog dialog = new SerialDialog(cert.SerialNumber);
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }



        /// <summary>
        /// Nhận notify message và chuyển xử lý tùy vào messageType
        /// </summary>
        /// <param name="message">Notify message, có thể là message hoặc là actionCode (Xem CommonMessage)</param>
        /// <param name="messageType"></param>
        /// <param name="param"></param>
        public void Update(string message, int messageType, object[] param)
        {
            switch (messageType)
            {
                //Hanle action notify message
                case CommonMessage.MESSAGE_TYPE_ACTION:
                    HandleActionMessage(message, param);
                    break;

                //Handle action with loading notify message
                case CommonMessage.MESSAGE_TYPE_ACTION_WITH_LOADING:
                    HandleActionMessageWithLoading(message, param);
                    break;

                //Hanle information notify message
                case CommonMessage.MESSAGE_TYPE_INFO:
                    HandleInfoMessage(message);
                    break;

                //Handle error notify message
                case CommonMessage.MESSAGE_TYPE_ERROR:
                    HandleErrorMessage(message);
                    break;

                //Handle confirm notify message
                case CommonMessage.MESSAGE_TYPE_CONFIRM:
                    HandleConfirmMessage(message, param);
                    break;

                default:
                    _LOG.Error("Receive undefined notify message");
                    break;
            }
        }



        /// <summary>
        /// Handle action notify message from child form
        /// </summary>
        /// <param name="actionCode"></param>
        /// <param name="param"></param>
        private void HandleActionMessage(string actionCode, Object[] param)
        {
            LoadingDialog.HideMe();
            //Thuc hien khi otp unlock pin da duoc nhap
            if (CommonMessage.UNLOCK_PIN_OTP_ENTERED.Equals(actionCode))
            {
                _LOG.Info("Received UNLOCK_PIN_OTP_ENTERED notify message");
                HandleConfirmOtp(param);
            }

            //Đã chọn hình thức gửi OTP, thực hiện action tiếp theo
            if (CommonMessage.OTP_TYPE_SELECTED.Equals(actionCode))
            {
                _LOG.Info("Receive notify message OTP_TYPE_SELECTED with param: " + param);
                if (param == null || param.Length != 2)
                {
                    ErrorDialog.Show("Lỗi không xác định", this);
                    return;
                }

                string otpType = CertificateRenew.OTP_TYPE_EMAIL;
                if (SelectOtpType.OTP_TYPE_SMS == (int)param[1])
                {
                    otpType = CertificateRenew.OTP_TYPE_PHONE;
                }


                //1. Chon hinh thuc OTP cho action unlock User Pin
                else if (CommonMessage.UNLOCK_USER_PIN_ACTION.Equals("" + param[0]))
                {
                    new UserPinUnlocker(this).queryUnlockPin(otpType);
                }
                return;
            }

            if (CommonMessage.UNLOCK_PIN_OTP_SENT.Equals(actionCode))
            {
                HandleOtpSent();
            }

            //Yeu cau xac thuc ma pin thanh cong
            if (CommonMessage.CONFIRM_USER_PIN_SUCCESS.Equals(actionCode))
            {
                _LOG.Info("Receive notify message CONFIRM_USER_PIN_SUCCESS with param: " + param);
                if (param == null || param.Length != 2)
                {
                    ErrorDialog.Show("Lỗi không xác định", this);
                    return;
                }

                //1. Thuc hien query gia han cert khi xac thuc ma pin thanh cong
                if (CommonMessage.RENEW_CERT_ACTION.Equals("" + param[0]))
                {
                    _renew.QueryRenew("" + param[1]);
                }
            }

            //Da kiem tra co chung thu moi de gia han, nguoi dung chon cap nhat ngay
            if (CommonMessage.CONFIRM_RENEW_CERT_OK.Equals(actionCode))
            {
                _renew.ImportP12Data();
                return;
            }

            //Yeu cau reload lai cert list
            if (CommonMessage.RELOAD_CERT_LIST.Equals(actionCode))
            {
                ReloadCertTable();
                return;
            }
        }


        /// <summary>
        /// Hiển thị LoadingDialog để chờ notify tiếp theo
        /// </summary>
        /// <param name="actionCode"></param>
        /// <param name="param"></param>
        private void HandleActionMessageWithLoading(string actionCode, Object[] param)
        {
            LoadingDialog.HideMe();
            LoadingDialog.Show(this, actionCode);
        }


        /// <summary>
        /// Hiển thị thông báo khi nhận được info message notify
        /// </summary>
        /// <param name="Message"></param>
        private void HandleInfoMessage(string Message)
        {
            LoadingDialog.HideMe();
            MessageDialog.Show(Message, this);
        }


        /// <summary>
        /// Hiển thị thông báo lỗi khi nhận được error notify
        /// </summary>
        /// <param name="ErrorMessage"></param>
        private void HandleErrorMessage(string ErrorMessage)
        {
            LoadingDialog.HideMe();
            ErrorDialog.Show(ErrorMessage, this);
        }


        /// <summary>
        /// Thực hiện yêu cầu khi nhận được confirm notify
        /// </summary>
        /// <param name="message">Message hiển thị</param>
        /// <param name="param">Tùy vào action mà có param khác nhau</param>
        private void HandleConfirmMessage(string message, Object[] param)
        {
            LoadingDialog.HideMe();
        }


        /// <summary>
        /// Được gọi khi OTP unlock pin được gửi thành công hoặc nhấn vào linkLabel
        /// </summary>
        /// <param name="param"></param>
        private void HandleConfirmOtp(Object[] param)
        {
            if (param == null || param[0] == null)
            {
                _LOG.Error("No OTP parameter from ConfirmOTP form.");
                return;
            }
            string Otp = (string)param[0];
            UserPinUnlocker Unlocker = new UserPinUnlocker(this);
            Unlocker.ConfirmOTP(Otp);
        }



        /// <summary>
        /// Phai su dung invoke khi muon update GUI tu thread khac
        /// </summary>
        /// <param name="message"></param>
        public void InvokeMessageDialog(string message)
        {
            MethodInvoker inv = delegate
            {
                LoadingDialog.HideMe();
                MessageDialog.Show(message, this);
            };

            this.Invoke(inv);
        }



        /// <summary>
        /// Phai su dung invoke khi muon update GUI tu thread khac
        /// </summary>
        /// <param name="message"></param>
        public void InvokeActionMessage(string message, Object[] param)
        {
            MethodInvoker inv = delegate
            {
                HandleActionMessage(message, param);
            };

            this.Invoke(inv);
        }

        /// <summary>
        /// Phai su dung invoke khi muon update GUI tu thread khac
        /// </summary>
        /// <param name="message"></param>
        public void InvokeErrorDialog(string message)
        {
            MethodInvoker inv = delegate
            {
                if (linkLabel1 != null)
                {
                    linkLabel1.Visible = false;
                }
                LoadingDialog.HideMe();
                ErrorDialog.Show(message, this);
            };

            this.Invoke(inv);
        }
        
        
        
        /// <summary>
        /// Phai su dung invoke khi muon update GUI tu thread khac
        /// </summary>
        /// <param name="message"></param>
        public void InvokeConfirmDialog(string message, string actionCode)
        {
            MethodInvoker inv = delegate
            {
                if (linkLabel1 != null)
                {
                    linkLabel1.Visible = false;
                }
                LoadingDialog.HideMe();
                new ConfirmDialog(message, actionCode, this).ShowDialog(this);
            };

            this.Invoke(inv);
        }



        /// <summary>
        /// Được gọi bởi UserPinUnlocker khi unlock pin thành công
        /// </summary>
        public void InvokeUnlockSuccess()
        {
            MethodInvoker inv = delegate
            {
                linkLabel1.Visible = false;
                try
                {
                    _linkLabelThread.Abort();
                }
                catch (Exception ex)
                {
                    _LOG.Error(ex);
                }
            };

            this.Invoke(inv);
        }



        /// <summary>
        /// Reload lại danh sách certificate
        /// </summary>
        private int ReloadCertTable()
        {
            ErrorLbl.Text = "";
            ErrorLbl.Visible = false;
            int result = -1;
            try
            {
                this._certList = Pkcs11Util.ListAllCertificate();
                CspUtil.LoadAllCertToStore(Pkcs11Connector.CspProvider);

                result = TOKEN_PLUGED;
                toolBtn.Enabled = true;
                toolBtn.Cursor = Cursors.Hand;
            }
            catch(TokenManagerException ex)
            {
                //Disable menu Cong cu khi co exception
                toolBtn.Enabled = false;
                toolBtn.Cursor = Cursors.Default;
                tabControl.SelectedIndex = _CERTIFICATE_TAB;
                certBtn.selected = true;
                bunifuSeparator1.Height = certBtn.Height - 1;
                bunifuSeparator1.Top = certBtn.Top;
                //------------------------------------------------------

                //Cap nhat danh sach certificate
                this._certList = new List<X509Certificate2>();

                CspUtil.UnloadAllCertificate(Pkcs11Connector.CspProvider);

                if (ex.Message.Contains("TOKEN_UNPLUGED"))
                {
                    ErrorLbl.Text = "VNPT CA Token chưa được cắm vào máy";
                    ErrorLbl.Visible = true;
                    result = TOKEN_UNPLUGED;
                }
                if (ex.Message.Contains("TOKEN_LOCKED"))
                {
                    ErrorLbl.Text = "VNPT CA Token đã bị khóa.";
                    ErrorLbl.Visible = true;
                    result = 2;
                }
            }
            viewCerts(this._certList);
            return result;
        }



        /// <summary>
        /// Cập nhật bảng hiển thị danh sách certificates
        /// </summary>
        /// <param name="certs"></param>
        private void viewCerts(List<X509Certificate2> certs)
        {
            if(certs == null)
            {
                return;
            }
            DataTable table = new DataTable();
            table.Columns.Add("#", typeof(int));
            table.Columns.Add("Chủ sở hữu", typeof(string));
            table.Columns.Add("Từ ngày", typeof(string));
            table.Columns.Add("Đến ngày", typeof(string));

            int index = 0;
            foreach(X509Certificate2 cert in certs)
            {
                string cn = cert.GetNameInfo(X509NameType.DnsName, false);
                string from = Convert.ToDateTime(cert.NotBefore).ToString("dd/MM/yyyy");
                string to = Convert.ToDateTime(cert.NotAfter).ToString("dd/MM/yyyy");
                table.Rows.Add(new object[] { ++index, cn, from, to });
            }
            certGrid.DataSource = table;
            certGrid.Columns[0].Width = 50;
            certGrid.Columns[1].Width = 200;
        }



        /// <summary>
        /// Chọn chức năng unlock pin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void unlockPinBtn_Click(object sender, EventArgs e)
        {
            new SelectOtpType(this, CommonMessage.UNLOCK_USER_PIN_ACTION).ShowDialog(this);
            return;
        }



        /// <summary>
        /// Chọn chức năng update profile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateProfileBtn_Click(object sender, EventArgs e)
        {
            UpdateProfile update = new UpdateProfile(this);
            update.ShowDialog(this);
        }



        /// <summary>
        /// Chọn chức năng reset pin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePinBtn_Click(object sender, EventArgs e)
        {
            ChangeUserPin ChangePin = new ChangeUserPin(this);
            ChangePin.ShowDialog(this);
        }



        /// <summary>
        /// Chọn chức năng gia hạn CKS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renewCertBtn_Click(object sender, EventArgs e)
        {
            //new SelectOtpType(this, CommonMessage.RENEW_CERT_ACTION).ShowDialog(this);
            new ConfirmUserPin(this, CommonMessage.RENEW_CERT_ACTION).ShowDialog(this);
        }


        /// <summary>
        /// Hien thi ConfirmOtpDialog khi nhan vao linkLabel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new ConfirmOTP(this).ShowDialog(this); ;
        }


        /// <summary>
        /// Khi OTP da gui tu TMS, cap nhat thoi gian hieu luc cua OTP 5minutes
        /// </summary>
        private void HandleOtpSent()
        {
            if(_linkLabelThread != null)
            {
                try
                {
                    _linkLabelThread.Abort();
                }
                catch (Exception ex)
                {
                    _LOG.Error(ex);
                }
            }

            linkLabel1.Visible = true;
            ThreadStart child = new ThreadStart(ChangeEnterOtpStatus);
            _linkLabelThread = new Thread(child);
            _linkLabelThread.Start();
            
            new ConfirmOTP(this).ShowDialog(this);
        }


        /// <summary>
        /// Change linkLabel clock text 
        /// </summary>
        private void ChangeEnterOtpStatus()
        {
            int count = 5*60;
            while(count > 0)
            {
                count--;
                int minute = count / 60;
                int second = count % 60;
                string m = "0" + minute;
                string s = second > 9 ? "" + second : "0" + second;
                MethodInvoker inv = delegate
                {
                    linkLabel1.Text = "Nhập mã (" + m +":" + s +")";
                };

                try
                {
                    this.Invoke(inv);
                }
                catch (Exception)
                {
                }

                Thread.Sleep(1000);
            }

            MethodInvoker inv1 = delegate
            {
                linkLabel1.Visible = false;
            };
            try
            {
                this.Invoke(inv1);
            }
            catch (Exception)
            {
                return;
            }

        }

        private void bunifuFlatButton5_Click(object sender, EventArgs e)
        {
            int maxSize = tabPage2.Width - 140;
            Bunifu.Framework.UI.BunifuFlatButton btnSource = ((Bunifu.Framework.UI.BunifuFlatButton)sender);
            bunifuSeparator2.Width = btnSource.Width - 1;
            bunifuSeparator2.Left = btnSource.Left;
            bunifuSeparator3.Left = btnSource.Right;
            bunifuSeparator3.Width = maxSize - bunifuSeparator3.Left;
            bunifuSeparator4.Left = btnSource.Left;
            bunifuSeparator5.Left = btnSource.Right - 1;
            //helpChildTab.Width = maxSize - aboutBtn.Left;
            downThueBox.Width = maxSize - aboutBtn.Left - 5;
            downBHXHBox.Width = maxSize - aboutBtn.Left - 5;
            
            if (btnSource == aboutBtn)
            {
                helpChildTab.SelectedIndex = 0;
                aboutBtn.Textcolor = TextColor;
                aboutBtn.Enabled = false;
                helpTabBtn.Textcolor = Color.White;
                helpTabBtn.Enabled = true;
            }
            else
            {
                helpChildTab.SelectedIndex = 1;
                aboutBtn.Textcolor = Color.White;
                aboutBtn.Enabled = true;
                helpTabBtn.Textcolor = TextColor;
                helpTabBtn.Enabled = false;
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string mail = "mailto:" + mailLink.Text;
            try
            {
                System.Diagnostics.Process.Start(mail);
            }
            catch (Exception ex)
            {
                _LOG.Error("linkLabel2_LinkClicked: " + ex.Message);
            }
        }

        private void webLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string web = "http://" + webLink.Text;
            try
            {
                System.Diagnostics.Process.Start(web);
            }
            catch(Exception ex)
            {
                _LOG.Error("webLink_LinkClicked: " + ex.Message);
            }
        }

        private void bunifuFlatButton5_Click_1(object sender, EventArgs e)
        {
            MessageDialog.Show("Phiên bản hiện tại chưa hỗ trợ", this);
        }

        private void skinLightBtn_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SKIN = "LIGHT";
            Properties.Settings.Default.Save();
            _initSkin();
        }

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SKIN = "DARK";
            Properties.Settings.Default.Save();
            _initSkin();
        }

        private void bunifuImageButton1_MouseEnter(object sender, EventArgs e)
        {
            bunifuImageButton1.BackColor = Color.FromArgb(204, 81, 20);
        }

        private void bunifuImageButton1_MouseHover(object sender, EventArgs e)
        {
            //bunifuImageButton1.BackColor = Color.Red;
        }

        private void bunifuImageButton1_MouseLeave(object sender, EventArgs e)
        {
            bunifuImageButton1.BackColor = HeaderBack;
        }
    }
}
