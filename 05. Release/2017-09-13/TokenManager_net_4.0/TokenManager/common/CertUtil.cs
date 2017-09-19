using Org.BouncyCastle.Pkcs;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using TokenManager.dialog;

namespace TokenManager.common
{
    /// <summary>
    /// Certificate utility methods
    /// </summary>
    class CertUtil
    {
        //Logger for this class
        private static readonly log4net.ILog _LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static string PathToRemove = "";

        public static Pkcs12Store ConvertBase64ToKeystore(string Base64String)
        {
            _LOG.Info("Start parse pkcs12 data get from TMS service");
            return null;
        }


        /// <summary>
        /// Call default program to view selected certificate
        /// </summary>
        /// <param name="cert">X509Certificate2 object to view</param>
        public static void viewCert(X509Certificate2 cert)
        {
            _LOG.Info("viewCert: Start view selected certificate.");
            if (cert == null)
            {
                _LOG.Info("viewCert: Certificate data null. Return to main program.");
                return;
            }

            try
            {
                PathToRemove = "\\" + DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + ".cer";
                string path = System.IO.Path.GetTempPath();
                PathToRemove = path + PathToRemove;

                //Write certificate content to system tmp folder
                File.WriteAllBytes(PathToRemove, cert.Export(X509ContentType.Cert));
                System.Diagnostics.Process.Start(PathToRemove);

                //Create thread to remove tmp certificate file after 10seconds
                ThreadStart t = new ThreadStart(removeTmpCertFile);
                Thread thread = new Thread(t);
                thread.Start();
            }
            catch(Exception ex)
            {
                _LOG.Error("viewCert: " + ex.Message);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cert"></param>
        public static void viewSerialNumber(X509Certificate2 cert)
        {
            _LOG.Info("Start view certificate's serial number");
            if(cert == null)
            {
                _LOG.Error("Certificate parameter is NULL. Return to main program");
                return;
            }
            _LOG.Info("Selected certificate's serial number is " + cert.SerialNumber);
            SerialDialog dialog = new SerialDialog(cert.SerialNumber);
            dialog.Show();
            return;
        }


        /// <summary>
        /// Remove tmp certificate file
        /// </summary>
        private static void removeTmpCertFile()
        {
            _LOG.Info("Start thread to remove tmp certificate file at " + PathToRemove);
            if (!File.Exists(PathToRemove))
            {
                _LOG.Error("File at " + PathToRemove + " doesn't exist.");
                return;
            }

            try
            {
                Thread.Sleep(10000);
                File.Delete(PathToRemove);
                _LOG.Info("Remove tmp certificate file successfull.");
            }
            catch (Exception ex)
            {
                _LOG.Error("removeTmpCertFile: " + ex.Message);
            }
        }
    }
}
