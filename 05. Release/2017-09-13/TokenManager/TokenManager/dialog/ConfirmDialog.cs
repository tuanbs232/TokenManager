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

    /// <summary>
    /// Form hien thi confirm dialog, nhan vao 1 MESSAGE_TYPE_ACTION va 1 confirm message
    /// notify message ve cho MainWindow ne nhan chon OK
    /// </summary>
    public partial class ConfirmDialog : Form, ObserverAble
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

        private List<Observer> observer = new List<Observer>();
        private string actionCode;
        
        public ConfirmDialog(string message, string actionCode, Object obj)
        {
            InitializeComponent();
            confirmLabel.Text = message;
            this.actionCode = actionCode;
            observer.Add((Observer)obj);
            header.BackColor = MainWindow.HeaderBack;
        }
        

        public void notify(string message, int messageType, object[] param)
        {
            this.Visible = false;
            this.Close();
            foreach (Observer obj in observer)
            {
                obj.Update(message, messageType, param);
            }
        }

        void ObserverAble.attach(Observer obj)
        {
            throw new NotImplementedException();
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

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void ObserverAble.detach(Observer obj)
        {
            throw new NotImplementedException();
        }

        void ObserverAble.notify(string message, int messageType, object[] param)
        {
            foreach(Observer obj in observer)
            {
                obj.Update(message, messageType, param);
            }
        }

        private void ok_Click(object sender, EventArgs e)
        {
            notify(actionCode, CommonMessage.MESSAGE_TYPE_ACTION, null);
        }
    }
}
