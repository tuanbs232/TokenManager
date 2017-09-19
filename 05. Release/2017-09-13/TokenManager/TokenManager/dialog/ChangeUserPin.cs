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
    public partial class ChangeUserPin : Form
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
        private int minPinLength = Pkcs11Util.MIN_USER_PIN_LENGHT;
        public ChangeUserPin()
        {
            InitializeComponent();
            minPinLength = Pkcs11Util.GetMinPinLength();
            CurentPinTxt.Focus();
            header.BackColor = MainWindow.HeaderBack;
        }

        List<Observer> Observers = new List<Observer>();
        public ChangeUserPin(Object ObserverInstance)
        {
            InitializeComponent();
            if(ObserverInstance != null)
            {
                Observers.Add((Observer)ObserverInstance);
            }
            CurentPinTxt.Focus();
            this.ActiveControl = CurentPinTxt;
            header.BackColor = MainWindow.HeaderBack;
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

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            handleSetPin();
        }

        private void handleSetPin()
        {
            if (!IsValidInput())
            {
                return;
            }
            this.Visible = false;
            this.Close();
            int result = Pkcs11Util.ResetUserPin(CurentPinTxt.Text, NewPinConfirmTxt.Text);

            if (result == Pkcs11Util.SUCCESSFULL)
            {
                notify("Thay đổi User PIN thành công!", CommonMessage.MESSAGE_TYPE_INFO, null);
            }
            else
            {
                string errorMsg = Pkcs11Util.GetErrorMessage(result);
                notify("Lỗi khi thay đổi User PIN. " + errorMsg, CommonMessage.MESSAGE_TYPE_ERROR, null);
            }
        }

        private bool IsValidInput()
        {
            currentPinError.Text = "";
            newPinError.Text = "";
            newPinConfirmError.Text = "";

            string CurrentPinValue = CurentPinTxt.Text;
            if(CurrentPinValue == null || "".Equals(CurrentPinValue))
            {
                currentPinError.Text = "Nhập mã PIN hiễn tại để tiếp tục";
                return false;
            }

            int TryLogin = Pkcs11Util.TryLogin(CurrentPinValue);
            if(Pkcs11Util.USER_PIN_INVALID == TryLogin)
            {
                currentPinError.Text = "Mã PIN không chính xác";
                return false;
            } 
            else if(Pkcs11Util.TOKEN_UNPLUG == TryLogin)
            {
                currentPinError.Text = "Cắm VNPT-CA Token để tiếp tục";
                return false;
            }

            string NewPinValue = NewPinTxt.Text;
            if(NewPinValue == null || "".Equals(NewPinValue))
            {
                newPinError.Text = "Nhập mã PIN mới để tiếp tục";
                return false;
            }
            if(NewPinValue.Length < minPinLength)
            {
                newPinError.Text = "Mã PIN có độ dài tối thiểu " + minPinLength + " ký tự";
                return false;
            }

            string NewPinConfirmValue = NewPinConfirmTxt.Text;
            if (NewPinConfirmValue == null || "".Equals(NewPinConfirmValue))
            {
                newPinConfirmError.Text = "Nhập lại mã PIN mới để tiếp tục";
                return false;
            }
            if (!NewPinConfirmValue.Equals(NewPinValue))
            {
                newPinConfirmError.Text = "Xác nhận mã PIN không khớp";
                return false;
            }

            return true;
        }

        public void notify(string Message, int MessageType, Object[] Param)
        {
            foreach(Observer Obj in this.Observers)
            {
                if(Obj == null)
                {
                    continue;
                }
                Obj.Update(Message, MessageType, Param);
            }
        }

        private void NewPinConfirmTxt_KeyDown(object sender, KeyEventArgs e)
        {
            currentPinError.Text = "";
            newPinError.Text = "";
            newPinConfirmError.Text = "";
            if (e.KeyCode == Keys.Enter)
            {
                handleSetPin();
            }
        }

        private void CurentPinTxt_KeyDown(object sender, KeyEventArgs e)
        {
            currentPinError.Text = "";
            newPinError.Text = "";
            newPinConfirmError.Text = "";
        }

        private void NewPinTxt_KeyDown(object sender, KeyEventArgs e)
        {
            currentPinError.Text = "";
            newPinError.Text = "";
            newPinConfirmError.Text = "";
        }
    }
}
