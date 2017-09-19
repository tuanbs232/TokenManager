using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TokenManager.common;
using TokenManager.dialog;
using TokenManager.Properties;

namespace TokenManager.test
{
    class ContextMenus
    {
        private ToolStripMenuItem showHide;
        private ContextMenuStrip menu;

        private MainWindow _mainWindow;
        private ProcessIcon _pi;
        private LanguageUtil _lang = LanguageUtil.GetInstance();

        private readonly log4net.ILog _LOG = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void HideToolContext()
        {
            try
            {
                (menu.Items[1] as ToolStripDropDownItem).Enabled = false;
            }
            catch(Exception ex)
            {
                _LOG.Error("HideToolContext: " + ex.Message);
            }
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ContextMenuStrip</returns>
        public ContextMenuStrip Create(MainWindow main, ProcessIcon pi)
        {
            _mainWindow = main;
            _pi = pi;

            // Add the default menu options.
            menu = new ContextMenuStrip();
            menu.Renderer = new MyRenderer();

            // Windows Explorer.
            showHide = new ToolStripMenuItem();
            showHide.Text = _lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_SHOW);
            showHide.Click += new EventHandler(Show_hide_Window);
            menu.Items.Add(showHide);

            ToolStripSeparator sep1 = new ToolStripSeparator();
            menu.Items.Add(sep1);


            //Tools dropdown
            ToolStripMenuItem tool = new ToolStripMenuItem();
            tool.Text = "Công cụ";
            menu.Items.Add(tool);
            //Unlock token tool
            ToolStripMenuItem unlock = new ToolStripMenuItem();
            unlock.Text = "Mở khóa token";
            unlock.Click += new EventHandler(Unlock_Token);
            (menu.Items[2] as ToolStripDropDownItem).DropDownItems.Add(unlock);
            //Change PIN tool
            ToolStripMenuItem changePin = new ToolStripMenuItem();
            changePin.Text = "Đổi User PIN";
            changePin.Click += new EventHandler(ChangeUser_Pin);
            (menu.Items[2] as ToolStripDropDownItem).DropDownItems.Add(changePin);
            //Renew certificate tool
            ToolStripMenuItem renew = new ToolStripMenuItem();
            renew.Text = "Gia hạn CTS";
            renew.Click += new EventHandler(Renew_Cert);
            (menu.Items[2] as ToolStripDropDownItem).DropDownItems.Add(renew);

            ToolStripMenuItem item0 = new ToolStripMenuItem();
            item0.Text = _lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_UPDATE);
            item0.Click += new EventHandler(Update_Click);
            menu.Items.Add(item0);

            // About.
            ToolStripMenuItem item1 = new ToolStripMenuItem();
            item1.Text = _lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_ABOUT);
            item1.Click += new EventHandler(About_Click);
            menu.Items.Add(item1);

            // Separator.
            ToolStripSeparator sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            // Exit.
            ToolStripMenuItem item2 = new ToolStripMenuItem();
            item2.Text = _lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_EXIT);
            item2.Click += new System.EventHandler(Exit_Click);
            //item.Image = Resources.close;
            menu.Items.Add(item2);

            return menu;
        }

        private void Update_Click(object sender, EventArgs e)
        {
            if (_mainWindow.WindowState == FormWindowState.Minimized)
            {
                Show_hide_Window(sender, e);
            }
            common.Updater.CheckForUpdate(_mainWindow);
            return;
        }

        /// <summary>
        /// Handles the Click event of the Explorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Show_hide_Window(object sender, EventArgs e)
        {
            if(_mainWindow.WindowState == FormWindowState.Normal)
            {
                showHide.Text = _lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_SHOW);
                _mainWindow.WindowState = FormWindowState.Minimized;
                _mainWindow.ShowInTaskbar = false;
            }
            else
            {
                showHide.Text = _lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_SHOW);
                _mainWindow.WindowState = FormWindowState.Normal;
                _mainWindow.ShowInTaskbar = true;
                bool top = _mainWindow.TopMost;
                // make our form jump to the top of everything
                _mainWindow.TopMost = true;
                // set it back to whatever it was
                _mainWindow.TopMost = top;
            }
        }

        /// <summary>
        /// Handles the Click event of the About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void About_Click(object sender, EventArgs e)
        {
            if (e is MouseEventArgs &&  (e as MouseEventArgs).Button != MouseButtons.Right)
            {
                // Only context event allowed
                return;
            }
            if (_mainWindow.WindowState == FormWindowState.Minimized)
            {
                Show_hide_Window(sender, e);
            }
            AboutDialog dig = new AboutDialog();
            if(_mainWindow.WindowState == FormWindowState.Normal)
            {
                dig.StartPosition = FormStartPosition.CenterParent;
                dig.ShowDialog(_mainWindow);
            }
            else
            {
                dig.StartPosition = FormStartPosition.CenterScreen;
                dig.ShowDialog(_mainWindow);
            }
        }

        /// <summary>
        /// Processes a menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Exit_Click(object sender, EventArgs e)
        {
            CspUtil.UnloadAllCertificate(Pkcs11Connector.CspProvider);

            _pi.Dispose();
            _mainWindow.Close();
            Pkcs11Connector.Destroy();
            Environment.Exit(0);
        }

        void Unlock_Token(object sender, EventArgs e)
        {
            if (_mainWindow.WindowState == FormWindowState.Minimized)
            {
                Show_hide_Window(sender, e);
            }
            new SelectOtpType(_mainWindow, CommonMessage.UNLOCK_USER_PIN_ACTION).ShowDialog(_mainWindow);
            return;
        }

        void Renew_Cert(object sender, EventArgs e)
        {
            if (_mainWindow.WindowState == FormWindowState.Minimized)
            {
                Show_hide_Window(sender, e);
            }
            new ConfirmUserPin(_mainWindow, CommonMessage.RENEW_CERT_ACTION).ShowDialog(_mainWindow);
        }

        void ChangeUser_Pin(object sender, EventArgs e)
        {
            if (_mainWindow.WindowState == FormWindowState.Minimized)
            {
                Show_hide_Window(sender, e);
            }
            ChangeUserPin ChangePin = new ChangeUserPin(_mainWindow);
            ChangePin.ShowDialog(_mainWindow);
        }
    }
}
