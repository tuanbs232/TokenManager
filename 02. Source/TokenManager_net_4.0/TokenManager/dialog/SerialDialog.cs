using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TokenManager.common;

namespace TokenManager.dialog
{
    public partial class SerialDialog : Form
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
        public SerialDialog()
        {
            InitializeComponent();
            header.BackColor = MainWindow.HeaderBack;
        }
        public SerialDialog(string serial)
        {
            InitializeComponent();
            serialLable.Text = serial;
            header.BackColor = MainWindow.HeaderBack;
            LanguageUtil lang = LanguageUtil.GetInstance();
            title.Text = lang.GetValue(LanguageUtil.Key.DIALOG_SERIAL_TITLE);
            label.Text = lang.GetValue(LanguageUtil.Key.DIALOG_SERIAL_LABEL);
            linkLabel1.Text = lang.GetValue(LanguageUtil.Key.DIALOG_SERIAL_LINK);
            dialogClose.Text = lang.GetValue(LanguageUtil.Key.BTN_CLOSE);
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

        private void dialogClose_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(serialLable.Text);
            string message = LanguageUtil.GetInstance().GetValue(LanguageUtil.Key.DIALOG_SERIAL_MESSAGE);
            showNotify(message);
        }

        private void showNotify(string message)
        {
            var notification = new System.Windows.Forms.NotifyIcon()
            {
                Visible = true,
                Icon = System.Drawing.SystemIcons.Information,
                BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info,
                BalloonTipTitle = "VNPT CA Token Manager",
                BalloonTipText = message,
            };

            // Display for 5 seconds.
            notification.ShowBalloonTip(5000);

            // This will let the balloon close after it's 5 second timeout
            // for demonstration purposes. Comment this out to see what happens
            // when dispose is called while a balloon is still visible.
            //Thread.Sleep(10000);

            // The notification should be disposed when you don't need it anymore,
            // but doing so will immediately close the balloon if it's visible.
            notification.Dispose();
        }
    }
}
