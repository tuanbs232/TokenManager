using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TokenManager.dialog;
using TokenManager.test;

namespace TokenManager
{
    static class Program
    {
        static ProcessIcon pi;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            //For run admin tool----------------------------------------
            bool runAdminTool = false;
            //----------------------------------------------------------

            if (!runAdminTool)
            {
                MainWindow main = new MainWindow();
                pi = new ProcessIcon(main);

                pi.Display();
                Application.Run(main);
            }
            else
            {
                Application.Run(new TestForm());
            }
        }
        static void OnProcessExit(object sender, EventArgs e)
        {
            pi.Dispose();
        }
    }
}
