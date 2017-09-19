using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TokenManager.test
{
    public partial class PasswordInput : Form
    {
        public PasswordInput()
        {
            InitializeComponent();
        }

        public string Password { get; set; }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                string pin = textBox1.Text;
                Password = pin;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            string pin = textBox1.Text;
            Password = pin;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        internal DialogResult ShowDialog(TestForm testForm, string v)
        {
            label1.Text = v;
            textBox1.Text = "";
            this.ShowDialog(testForm);
            return this.DialogResult;
        }
    }
}
