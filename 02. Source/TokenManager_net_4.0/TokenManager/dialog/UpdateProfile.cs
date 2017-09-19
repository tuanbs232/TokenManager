using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TokenManager.common;
using TokenManager.tmsclient;

namespace TokenManager.dialog
{
    public partial class UpdateProfile : Form
    {
        //Logger for this class
        private static readonly log4net.ILog LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<Observer> _observer = new List<Observer>();
        private LanguageUtil _lang;

        public UpdateProfile()
        {
            InitializeComponent();
            EmailTextBox.Focus();
            this.ActiveControl = EmailTextBox;
            header.BackColor = MainWindow.HeaderBack;
        }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Obj">MainWindows instance</param>
        public UpdateProfile(Object Obj)
        {
            InitializeComponent();
            this._observer.Add((Observer)Obj);
            EmailTextBox.Focus();
            this.ActiveControl = EmailTextBox;
            header.BackColor = MainWindow.HeaderBack;
            _lang = LanguageUtil.GetInstance();

            title.Text = _lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_TITLE);
            phoneTitle.Text = _lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_PHONE);
            pinTitle.Text = _lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_PIN);

            okBtn.Text = _lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_OK_BTN);
            cancelBtn.Text = _lang.GetValue(LanguageUtil.Key.BTN_CLOSE);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsValidInputValue()
        {
            string PhoneNumber = PhoneTextBox.Text;
            string Email = EmailTextBox.Text;
            if (Email == null || "".Equals(Email))
            {
                EmailError.Text = _lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_ENTER_EMAIL);
                return false;
            }

            bool isValidEmail = false;
            try
            {
                isValidEmail = Regex.IsMatch(Email, TokenManagerConstants.EMAIL_REGEX, RegexOptions.IgnoreCase);
            }
            catch (Exception ex)
            {
                LOG.Error("Cannot check email address regex: " + ex.Message);
                isValidEmail = false;
            }
            if (!isValidEmail)
            {
                EmailError.Text = _lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_EMAIL_INVALID);
                return false;
            }


            if (PhoneNumber == null || "".Equals(PhoneNumber))
            {
                PhoneError.Text = _lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_ENTER_PHONE);
                return false;
            }

            bool isValidPhone = false;
            try
            {
                isValidPhone = Regex.IsMatch(PhoneNumber, @"^\d+$");
            }
            catch (Exception ex)
            {
                LOG.Error("Cannot check phone address regex: " + ex.Message);
                isValidPhone = false;
            }
            if (!isValidPhone)
            {
                PhoneError.Text = _lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_PHONE_INVALID);
                return false;
            }

            return CheckUserPin();
        }

        private bool CheckUserPin()
        {
            string userPin = PinTxt.Text;
            if(userPin == null || "".Equals(userPin))
            {
                PinError.Text = _lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_ENTER_PIN);
                return false;
            }

            int resultCode = Pkcs11Util.TryLogin(userPin);
            if(Pkcs11Util.TOKEN_UNPLUG == resultCode)
            {
                PinError.Text = _lang.GetValue(LanguageUtil.Key.CHANGEPIN_CHECK_PLUG_TOKEN);
                return false;
            }
            if(Pkcs11Util.USER_PIN_INVALID == resultCode)
            {
                PinError.Text = _lang.GetValue(LanguageUtil.Key.CHANGEPIN_CHECK_PIN_INCORRECT);
                return false;
            }
            return true;
        }

        private CustomerInfo info;
        private void HandleUpdateProfile()
        {
            this.Visible = false;
            this.Close();
            info = null;
            try
            {
                info = Pkcs11Util.GetCustomerTokenInfo();
            }
            catch (TokenManagerException ex)
            {
                if (ex.Message.Contains("SLOT NULL"))
                {
                    Notify(_lang.GetValue(LanguageUtil.Key.CHANGEPIN_CHECK_PLUG_TOKEN), CommonMessage.MESSAGE_TYPE_ERROR, null);
                    return;
                }
                else
                {
                    Notify(ex.Message, CommonMessage.MESSAGE_TYPE_ERROR, null);
                    return;
                }
            }

            info.Email = EmailTextBox.Text;
            info.PhoneNumber = PhoneTextBox.Text;

            ThreadStart childThread = new ThreadStart(HandleRequest);
            Thread thread = new Thread(childThread);
            thread.Start();
            Notify(_lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_PROCESSING), CommonMessage.MESSAGE_TYPE_ACTION_WITH_LOADING, null);
        }

        private void HandleRequest()
        {
            int resultCode = TMSClient.UpdateProfile(info);

            MainWindow main = (MainWindow)_observer.ElementAt(0);

            if (resultCode != TMSClient.SUCCESSFULL)
            {
                string errorMessage = TMSClient.GetErrorMessage(resultCode);
                main.InvokeErrorDialog(errorMessage);
                return;
            }

            main.InvokeMessageDialog(_lang.GetValue(LanguageUtil.Key.DIALOG_UPDATE_PROFILE_SUCCESSFULL));
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuImageButton1_MouseEnter(object sender, EventArgs e)
        {
            bunifuImageButton1.BackColor = Color.FromArgb(204, 81, 20);
        }

        private void bunifuImageButton1_MouseLeave(object sender, EventArgs e)
        {
            bunifuImageButton1.BackColor = header.BackColor;
        }


        private void okBtn_Click(object sender, EventArgs e)
        {
            LOG.Info("Start update customer profile.");
            if (!IsValidInputValue())
            {
                LOG.Error("Input value invalid");
                return;
            }

            HandleUpdateProfile();
        }

        private void PinTxt_KeyDown(object sender, KeyEventArgs e)
        {
            EmailError.Text = "";
            PhoneError.Text = "";
            PinError.Text = "";
            if (e.KeyCode == Keys.Enter)
            {
                if (!IsValidInputValue())
                {
                    LOG.Error("Input value invalid");
                    return;
                }

                HandleUpdateProfile();
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Notify(string message, int messageType, Object[] param)
        {
            foreach(Observer Obj in this._observer)
            {
                if(Obj == null)
                {
                    continue;
                }
                Obj.Update(message, messageType, param);
            }
        }

        private void EmailTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            EmailError.Text = "";
            PhoneError.Text = "";
            PinError.Text = "";
        }

        private void PhoneTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            EmailError.Text = "";
            PhoneError.Text = "";
            PinError.Text = "";
        }
    }
}
