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
    public partial class ConfirmUserPin : Form
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
        private List<Observer> _observers = new List<Observer>();
        private string _parentAction;
        public ConfirmUserPin()
        {
            InitializeComponent();
            PinError.Text = "";
            PinTxt.Focus();
            this.ActiveControl = PinTxt;
            header.BackColor = MainWindow.HeaderBack;
        }

        public ConfirmUserPin(Object obj, string parentAction)
        {
            InitializeComponent();
            PinError.Text = "";
            _observers.Add((Observer)obj);
            _parentAction = parentAction;
            PinError.Text = "";
            PinTxt.Focus();
            this.ActiveControl = PinTxt;
            header.BackColor = MainWindow.HeaderBack;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
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

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Close();
        }

        private void PinTxt_KeyDown(object sender, KeyEventArgs e)
        {
            PinError.Text = "";
            if(e.KeyCode == Keys.Enter)
            {
                HandlePinEntered();
            }
        }

        private void HandlePinEntered()
        {
            string PinValue = PinTxt.Text;
            if(PinValue == null || "".Equals(PinValue))
            {
                PinError.Text = "Nhập mã PIN để tiếp tục";
                return;
            }

            int result = Pkcs11Util.TryLogin(PinValue);
            if(Pkcs11Util.TOKEN_UNPLUG == result)
            {
                PinError.Text = "Cắm VNPT CA Token để tiếp tục";
                return;
            }
            if (Pkcs11Util.USER_PIN_INVALID == result)
            {
                PinError.Text = "Mã PIn không chính xác";
                return;
            }
            this.Visible = false;
            this.Close();

            foreach(Observer obj in _observers)
            {
                if(obj == null)
                {
                    continue;
                }
                string[] param = { _parentAction, PinValue };
                obj.Update(CommonMessage.CONFIRM_USER_PIN_SUCCESS, CommonMessage.MESSAGE_TYPE_ACTION, param);
            }
        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            HandlePinEntered();
        }

        private void PinTxt_Enter(object sender, EventArgs e)
        {
            //HandlePinEntered();
        }
    }
}
