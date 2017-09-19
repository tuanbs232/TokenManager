using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TokenManager.common;

namespace TokenManager.dialog
{
    public partial class ConfirmOTP : Form
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
        private static readonly log4net.ILog LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Observer ParentWindow;

        public ConfirmOTP()
        {
            InitializeComponent();
            ErrorMessageLabel.Text = "";
            OtpCodeTextBox.Focus();
            this.ActiveControl = OtpCodeTextBox;
            header.BackColor = MainWindow.HeaderBack;
            _setLanguage();
        }

        private void _setLanguage()
        {
            LanguageUtil lang = LanguageUtil.GetInstance();
            title.Text = lang.GetValue(LanguageUtil.Key.DIALOG_OTP_CONFIRM_TITLE);
            label.Text = lang.GetValue(LanguageUtil.Key.DIALOG_OTP_CONFIRM_LABEL);
            confirmBtn.Text = lang.GetValue(LanguageUtil.Key.BTN_OK);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">Observer instance object</param>
        public ConfirmOTP(Object obj)
        {
            InitializeComponent();
            ErrorMessageLabel.Text = "";
            this.ParentWindow = (Observer)obj;
            OtpCodeTextBox.Focus();
            this.ActiveControl = OtpCodeTextBox;
            header.BackColor = MainWindow.HeaderBack;
            _setLanguage();
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
            bunifuImageButton1.BackColor = MainWindow.HeaderBack;
        }

        private void confirmBtn_Click(object sender, EventArgs e)
        {
            ErrorMessageLabel.Text = "";
            HanleOTPEntered();
        }

        private void otpCode_Enter(object sender, EventArgs e)
        {
            ErrorMessageLabel.Text = "";
            HanleOTPEntered();
        }

        private void otpCode_KeyDown(object sender, KeyEventArgs e)
        {
            ErrorMessageLabel.Text = "";
            if(e.KeyCode == Keys.Enter)
            {
                HanleOTPEntered();
            }
        }
        
        private void HanleOTPEntered()
        {
            string OtpCodeValue = OtpCodeTextBox.Text;
            LOG.Info("OTP code entered: " + OtpCodeValue);
            if(OtpCodeValue == null || "".Equals(OtpCodeValue))
            {
                ErrorMessageLabel.Text = LanguageUtil.GetInstance().GetValue(LanguageUtil.Key.DIALOG_OTP_CONFIRM_ERROR);
                return;
            }
            this.Visible = false;


            LOG.Info("Notify to MainWindow that OTP entered");
            if(this.ParentWindow == null)
            {
                LOG.Error("No MainWindow instance parameter");
                return;
            }

            string[] param = { OtpCodeValue };

            ParentWindow.Update(CommonMessage.UNLOCK_PIN_OTP_ENTERED, 
                CommonMessage.MESSAGE_TYPE_ACTION, param);
        }
    }
}
