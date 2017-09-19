using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security.Certificates;
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

        static X509Certificate2 _cert = null;
        static MainWindow _mainWindow = null;
        public static void CheckOcsp(X509Certificate2 cert, MainWindow mainWindow)
        {
            _cert = cert;
            _mainWindow = mainWindow;
            ThreadStart child = new ThreadStart(HandleCheckOcsp);
            Thread parent = new Thread(child);
            parent.Start();
            string message = LanguageUtil.GetInstance().GetValue(LanguageUtil.Key.CERT_UTIL_OCSP_CHECKING);
            mainWindow.Update(message, CommonMessage.MESSAGE_TYPE_ACTION_WITH_LOADING, null);
        }

        private static void HandleCheckOcsp()
        {
            LanguageUtil lang = LanguageUtil.GetInstance();
            if (_cert == null)
            {
                _LOG.Error("Certificate null");
                return;
            }

            _LOG.Info("CheckOcsp: Get revoccation information for cert=" + _cert.GetName());
            X509Certificate2 ca = getCACertificate(_cert);
            if (ca == null)
            {
                _mainWindow.InvokeErrorDialog(lang.GetValue(LanguageUtil.Key.CERT_UTIL_OCSP_NO_CA));
                return;
            }

            //string ocspUrl = GetOcspUrl(_cert);
            string ocspUrl = "http://123.31.10.17/ocsp";
            if (ocspUrl == null)
            {
                _mainWindow.InvokeErrorDialog(lang.GetValue(LanguageUtil.Key.CERT_UTIL_OCSP_NO_CA));
                return;
            }

            BigInteger serialNumber = new BigInteger(_cert.SerialNumber, 16);
            Org.BouncyCastle.X509.X509CertificateParser certParser = new Org.BouncyCastle.X509.X509CertificateParser();
            Org.BouncyCastle.X509.X509Certificate issuer = certParser.ReadCertificate(ca.RawData);
            int resultCode = OcspValidator.check(serialNumber, issuer, ocspUrl);
            _mainWindow.InvokeMessageDialog(lang.GetValue(LanguageUtil.Key.CERT_UTIL_OCSP_CERT_STATUS) + ": " + OcspValidator.GetMessage(resultCode));
            return;
        }

        private static X509Certificate2 getCACertificate(X509Certificate2 cert)
        {
            if(cert == null)
            {
                return null;
            }

            X509Certificate2 caCert = null;
            try
            {
                byte[] caCertData = Convert.FromBase64String(TokenManagerConstants.CA_CERTIFICATE);
                caCert = new X509Certificate2(caCertData);
            }
            catch(Exception ex)
            {
                _LOG.Error("getCACertificate: Cannot get ca certificate data. " + ex.Message);
                return null;
            }

            if (!cert.Issuer.ToLower().Equals(caCert.Subject.ToLower()))
            {
                return null;
            }
            return caCert;
            return null;
        }

        public static string GetOcspUrl(X509Certificate2 certificate)
        {
            X509Extensions extensions = null;
            try
            {
                extensions = OcspValidator.GetX509Extensions(certificate);
            }
            catch(Exception exx)
            {
                _LOG.Error("GetOcspUrl: Cannot get extensions. " + exx.Message);
                return null;
            }
            if(extensions == null)
            {
                return null;
            }

            Org.BouncyCastle.Asn1.X509.X509Extension ex = extensions.GetExtension(X509Extensions.AuthorityInfoAccess);

            AccessDescription[] authorityInformationAccess = AuthorityInformationAccess.GetInstance(ex).GetAccessDescriptions();
            if (authorityInformationAccess == null)
            {
                _LOG.Error("GetOcspUrl: Could not find ocsp url for certificate");
                return null;
            }

            var ocspUrl = OcspValidator.GetAccessDescriptionUrlForOid(AccessDescription.IdADOcsp, authorityInformationAccess);

            if (ocspUrl == null)
            {
                _LOG.Error("GetOcspUrl: Could not find ocsp url for certificate");
                return null;
            }
            return ocspUrl;
        }

    }
}
