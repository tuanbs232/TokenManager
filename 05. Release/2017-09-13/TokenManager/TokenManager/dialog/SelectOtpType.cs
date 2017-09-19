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
    public partial class SelectOtpType : Form
    {
        public const int OTP_TYPE_SMS = 1;
        public const int OTP_TYPE_EMAIL = 2;
        public int OtpType { get; set; }

        private List<Observer> _observers = new List<Observer>();
        private string _parentAction;
        public SelectOtpType(Object obj, string parentAction)
        {
            InitializeComponent();
            OtpType = OTP_TYPE_EMAIL;
            this._observers.Add((Observer)obj);
            this._parentAction = parentAction;
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

        private void EmailChx_OnChange(object sender, EventArgs e)
        {
            OtpType = OTP_TYPE_EMAIL;
            EmailChx.Checked = true;
            SmsChx.Checked = false;
        }

        private void SmsChx_OnChange(object sender, EventArgs e)
        {
            OtpType = OTP_TYPE_SMS;
            SmsChx.Checked = true;
            EmailChx.Checked = false;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void notify(string message, int messageType, Object[] param)
        {
            foreach(Observer obj in _observers)
            {
                if(obj == null)
                {
                    continue;
                }
                obj.Update(message, messageType, param);
            }
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Close();
            object[] param = {_parentAction, OtpType };
            notify(CommonMessage.OTP_TYPE_SELECTED, CommonMessage.MESSAGE_TYPE_ACTION, param);
        }
    }
}
