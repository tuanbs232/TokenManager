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
        private static readonly log4net.ILog LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Load all certificate in token to Window-MY using current token CSP provider
        /// </summary>
        /// <param name="cspProviderName"></param>
        public static void LoadAllCertToStore(string cspProviderName)
        {
            LOG.Info("LoadAllCertToStore: Load all certificates to Window-MY with csp " + cspProviderName);
            try
            {
                int result = CspWrapper.C_LoadAllCertToStore(new StringBuilder(cspProviderName));
                //int result = 0;
                if(result == 0)
                {
                    LOG.Info("LoadAllCertToStore: All certificates already imported.");
                }
                else
                {
                    LOG.Error("LoadAllCertToStore: " + CspWrapper.GetErrorMessage(result));
                }
            }
            catch(Exception e)
            {
                LOG.Error("LoadAllCertToStore: " + e.Message);
            }
        }


        /// <summary>
        /// Remove all certificate in Window-MY with current token CSP provider
        /// </summary>
        /// <param name="cspProviderName"></param>
        public static void UnloadAllCertificate(string cspProviderName)
        {
            LOG.Info("UnloadAllCertificate: Unload all certificates from Window-MY with csp " + cspProviderName);
            try
            {
                int result = CspWrapper.C_UnloadAllCertificate(new StringBuilder(cspProviderName));
                
                if (result == 0)
                {
                    LOG.Info("UnloadAllCertificate: All certificates already removed.");
                }
                else
                {
                    LOG.Error("UnloadAllCertificate: " + CspWrapper.GetErrorMessage(result));
                }
            }
            catch (Exception e)
            {
                LOG.Error("UnloadAllCertificate: " + e.Message);
            }
        }
    }
}
