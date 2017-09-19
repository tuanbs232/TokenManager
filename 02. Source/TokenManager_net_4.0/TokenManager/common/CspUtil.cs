using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.common
{
    class CspUtil
    {
        //Logger for this class
        private static readonly log4net.ILog _LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Load all certificate in token to Window-MY using current token CSP provider
        /// </summary>
        /// <param name="cspProviderName"></param>
        public static void LoadAllCertToStore(string cspProviderName)
        {
            _LOG.Info("LoadAllCertToStore: Load all certificates to Window-MY with csp " + cspProviderName);
            try
            {
                int result = CspWrapper.C_LoadAllCertToStore(new StringBuilder(cspProviderName));
                //int result = 0;
                if(result == 0)
                {
                    _LOG.Info("LoadAllCertToStore: All certificates already imported.");
                }
                else
                {
                    _LOG.Error("LoadAllCertToStore: " + CspWrapper.GetErrorMessage(result));
                }
            }
            catch(Exception e)
            {
                _LOG.Error("LoadAllCertToStore: " + e.Message);
            }
        }


        /// <summary>
        /// Remove all certificate in Window-MY with current token CSP provider
        /// </summary>
        /// <param name="cspProviderName"></param>
        public static void UnloadAllCertificate(string cspProviderName)
        {
            _LOG.Info("UnloadAllCertificate: Unload all certificates from Window-MY with csp " + cspProviderName);
            try
            {
                int result = CspWrapper.C_UnloadAllCertificate(new StringBuilder(cspProviderName));
                
                if (result == 0)
                {
                    _LOG.Info("UnloadAllCertificate: All certificates already removed.");
                }
                else
                {
                    _LOG.Error("UnloadAllCertificate: " + CspWrapper.GetErrorMessage(result));
                }
            }
            catch (Exception e)
            {
                _LOG.Error("UnloadAllCertificate: " + e.Message);
            }
        }

        /// <summary>
        /// Check and install CA certificate chain to window store if not
        /// </summary>
        public static void InstallCAChain()
        {
            try
            {
                byte[] rootCertData = Convert.FromBase64String(TokenManagerConstants.ROOT_CA_CERTIFICATE);
                X509Certificate2 rootCA = new X509Certificate2(rootCertData);
                X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);
                if (!store.Certificates.Contains(rootCA))
                {
                    _LOG.Info("InstallCAChain: Import Root CA certificate to Windows store");
                    store.Add(rootCA);
                    _LOG.Info("InstallCAChain: Successfull");
                }
                store.Close();
            }
            catch (Exception ex)
            {
                _LOG.Error("InstallCAChain: Unable install Root CA certificate. " + ex.Message);
            }

            try
            {
                byte[] caCertData = Convert.FromBase64String(TokenManagerConstants.CA_CERTIFICATE);
                X509Certificate2 caCert = new X509Certificate2(caCertData);
                X509Store store = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);
                if (!store.Certificates.Contains(caCert))
                {
                    _LOG.Info("InstallCAChain: Import Intermediate CA certfiicate to Windows store");
                    store.Add(caCert);
                    _LOG.Info("InstallCAChain: Successfull.");
                }
                store.Close();
            }
            catch (Exception ex)
            {
                _LOG.Error("InstallCAChain: Unable install Intermediate CA certificate. " + ex.Message);
            }
        }
    }
}
