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
    public partial class LoadingDialog : Form
    {
        private static LoadingDialog _instance;
        private LoadingDialog()
        {
            InitializeComponent();
        }
        public LoadingDialog(string message)
        {
            InitializeComponent();
            this.Message.Text = message;
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


        public static void Show(Form parent, string message)
        {
            _instance = new LoadingDialog();
            _instance.Message.Text = message;
            _instance.ShowDialog(parent);
        }

        public static  void HideMe()
        {
            if(_instance != null)
            {
                _instance.Visible = false;
                _instance.Close();
            }
        }
    }
}
