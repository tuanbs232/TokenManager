using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TokenManager.dialog
{
    public partial class AboutDialog : Form
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
        public AboutDialog()
        {
            InitializeComponent();
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

        private void mailLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string mail = "mailto:" + mailLink.Text;
            try
            {
                System.Diagnostics.Process.Start(mail);
            }
            catch (Exception ex)
            {
                _LOG.Error("mailLink_LinkClicked" + ex.Message);
            }
        }

        private void webLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string web = "http://" + webLink.Text;
            try
            {
                System.Diagnostics.Process.Start(web);
            }
            catch (Exception ex)
            {
                _LOG.Error("webLink_LinkClicked" + ex.Message);
            }
        }
    }
}
