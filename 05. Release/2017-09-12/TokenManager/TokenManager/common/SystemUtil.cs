using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.common
{
    class SystemUtil
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void ShowNotify(string message)
        {
            var notification = new System.Windows.Forms.NotifyIcon()
            {
                Visible = true,
                Icon = Properties.Resources.Logo_VNPT,
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
