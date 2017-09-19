using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TokenManager.Properties;

namespace TokenManager.test
{
    class ProcessIcon : IDisposable
    {
        private MainWindow _mainWindow;
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
            ni.ContextMenuStrip = new ContextMenus().Create(_mainWindow, this);
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
            // Handle mouse button clicks.
            if (e.Button == MouseButtons.Left)
            {
                // Start Windows Explorer.
            }
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
