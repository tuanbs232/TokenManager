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
    public partial class MessageDialog : Form
    {
        private static MessageDialog instance = null;
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
        public static void Show(string Message, Form Container)
        {
            if(instance == null)
            {
                instance = new MessageDialog();
            }
            instance.MessageLabel.Text = Message;
            instance.StartPosition = FormStartPosition.CenterParent;
            instance.ShowDialog(Container);
            instance.BackColor = MainWindow.HeaderBack;
        }

        private MessageDialog()
        {
            InitializeComponent();
            header.BackColor = MainWindow.HeaderBack;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void bunifuImageButton1_MouseEnter(object sender, EventArgs e)
        {
            bunifuImageButton1.BackColor = Color.FromArgb(204, 81, 20);
        }

        private void bunifuImageButton1_MouseLeave(object sender, EventArgs e)
        {
            bunifuImageButton1.BackColor = header.BackColor;
        }
    }
}
