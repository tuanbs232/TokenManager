using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TokenManager.common;
using TokenManager.Properties;

namespace TokenManager.test
{
    class ProcessIcon : IDisposable
    {
        private MainWindow _mainWindow;
        private ContextMenuStrip _contextMenu;
        /// <summary>
        /// The NotifyIcon object.
        /// </summary>
        NotifyIcon ni;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessIcon"/> class.
        /// </summary>
        public ProcessIcon(MainWindow main)
        {
            this._mainWindow = main;
            this._mainWindow.Resize += SetMinimizeState;
            // Instantiate the NotifyIcon object.
            ni = new NotifyIcon();
        }

        /// <summary>
        /// Displays the icon in the system tray.
        /// </summary>
        public void Display()
        {
            // Put the icon in the system tray and allow it react to mouse clicks.			
            ni.MouseClick += new MouseEventHandler(ni_MouseClick);
            ni.MouseDoubleClick += new MouseEventHandler(ni_DoubleClick);
            ni.Icon = Resources.Logo_VNPT;
            ni.Text = "VNPT-CA Token Manager";
            ni.Visible = true;

            // Attach a context menu.
            _contextMenu = new ContextMenus().Create(_mainWindow, this);
            ni.ContextMenuStrip = _contextMenu;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            // When the application closes, this will remove the icon from the system tray immediately.
            ni.Dispose();
        }

        /// <summary>
        /// Handles the MouseClick event of the ni control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void ni_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                // Only context event allowed
                return;
            }
            LanguageUtil lang = LanguageUtil.GetInstance();
            _contextMenu.Items[0].Text = lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_SHOW);
            //_contextMenu.Items[1] is separator
            _contextMenu.Items[2].Text = lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_TOOLS);
            (_contextMenu.Items[2] as ToolStripDropDownItem).DropDownItems[0].Text = lang.GetValue(LanguageUtil.Key.TOOL_UNLOCK_TITLE);
            (_contextMenu.Items[2] as ToolStripDropDownItem).DropDownItems[1].Text = lang.GetValue(LanguageUtil.Key.TOOL_CHANGEPIN_TITLE);
            (_contextMenu.Items[2] as ToolStripDropDownItem).DropDownItems[2].Text = lang.GetValue(LanguageUtil.Key.TOOL_RENEW_TITLE);
            _contextMenu.Items[3].Text = lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_UPDATE);
            _contextMenu.Items[4].Text = lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_ABOUT);
            //_contextMenu.Items[1] is separator
            _contextMenu.Items[6].Text = lang.GetValue(LanguageUtil.Key.CONTEXT_MENU_EXIT);
        }

        /// <summary>
        /// Handles the MouseClick event of the ni control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void ni_DoubleClick(object sender, MouseEventArgs e)
        {
            bool isMinimized = _mainWindow.WindowState == FormWindowState.Minimized;
            if (!isMinimized)
            {
                return;
            }
            _mainWindow.WindowState = (isMinimized) ? FormWindowState.Normal : FormWindowState.Minimized;
        }

        // Toggle state between Normal and Minimized.
        private void ToggleMinimizeState(object sender, MouseEventArgs e)
        {
            bool isMinimized = _mainWindow.WindowState == FormWindowState.Minimized;
            _mainWindow.WindowState = (isMinimized) ? FormWindowState.Normal : FormWindowState.Minimized;
        }

        // Show/Hide window and tray icon to match window state.
        private void SetMinimizeState(object sender, EventArgs e)
        {
            bool isMinimized = _mainWindow.WindowState == FormWindowState.Minimized;

            _mainWindow.ShowInTaskbar = !isMinimized;
            /*systemTrayIcon.Visible = isMinimized;
            if (isMinimized) systemTrayIcon.ShowBalloonTip(500, "Application", "Application minimized to tray.", ToolTipIcon.Info);*/
        }

    }
}
