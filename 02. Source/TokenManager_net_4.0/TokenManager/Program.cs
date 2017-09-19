using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TokenManager.common;
using TokenManager.dialog;
using TokenManager.test;

namespace TokenManager
{
    static class Program
    {
        //For run admin tool --------------------------------------------------
        static bool runAdminTool = false;
        //For run admin tool --------------------------------------------------


        public static ProcessIcon pi;
        static Mutex mutex = new Mutex(true, "VNPT-CA-TokenManager");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            if (!runAdminTool)
            {
                _runTokenManager();
            }
            else
            {
                Application.Run(new TestForm());
            }
        }

        private static void _runTokenManager()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                MainWindow main = new MainWindow();
                pi = new ProcessIcon(main);

                pi.Display();
                Application.Run(main);
                mutex.ReleaseMutex();
            }
            else
            {
                SingleInstanceApp.PostMessage(
                    (IntPtr)SingleInstanceApp.HWND_BROADCAST,
                    SingleInstanceApp.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            if (pi != null)
            {
                pi.Dispose();
            }
        }
    }
}
