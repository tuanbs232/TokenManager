using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using TokenManager.common;

namespace TokenManager.dialog
{
    public partial class UpdateDialog : Form
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

        private WebClient _client;
        private MainWindow _mainWindow;
        public UpdateDialog(MainWindow main)
        {
            InitializeComponent();
            header.BackColor = MainWindow.HeaderBack;
            LanguageUtil lang = LanguageUtil.GetInstance();
            title.Text = lang.GetValue(LanguageUtil.Key.UPDATE_TITLE);
            label1.Text = lang.GetValue(LanguageUtil.Key.UPDATE_DOWNLOADING_UPDATE_PACKAGE);
            updateBtn.Text = lang.GetValue(LanguageUtil.Key.UPDATE_UPDATE_NOW_BTN);
            _mainWindow = main;
            downloadProcessBar.Value = 0;

            _client = new WebClient();
            common.Updater.DownloadUpdatePackage(_client, this);
        }

        public void InvokeChangeProgressBar(int value, long total, long received)
        {
            MethodInvoker inv = delegate
            {
                downloadProcessBar.Value = value;
                double total1 = total * 1.0 / 1024;
                double received1 = received * 1.0 / 1024;
                sizeValue.Text = "" + received1.ToString("#,###") + "/" + total1.ToString("#,###") + " KBs";
                percentage.Text = value + " %";
            };

            this.Invoke(inv);
        }

        public void InvokeErrorMessage(string message)
        {
            this.Dispose();
            _mainWindow.InvokeErrorDialog(message);
        }

        public void InvokeDownloadComplete()
        {
            label1.Text = LanguageUtil.GetInstance().GetValue(LanguageUtil.Key.UPDATE_COMPLETE_UPDATE_PACKAGE);
            updateBtn.Enabled = true;
        }

        public void Cancel()
        {
            if(_client != null)
            {
                _client.CancelAsync();
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            if(_client != null)
            {
                _client.CancelAsync();
            }

            this.Dispose();
        }

        private void bunifuImageButton1_MouseEnter(object sender, EventArgs e)
        {
            closeBtn.BackColor = Color.FromArgb(204, 81, 20);
        }

        private void bunifuImageButton1_MouseLeave(object sender, EventArgs e)
        {
            closeBtn.BackColor = header.BackColor;
        }

        private void cancel_Click(object sender, EventArgs e)
        {

        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
            string tmpPath = System.IO.Path.GetTempPath() + "\\" + TokenManagerConstants.UPDATE_PACKAGE_TMP_PATH;
            System.Diagnostics.Process.Start(tmpPath);
            _mainWindow.CloseApp();
        }
    }
}
