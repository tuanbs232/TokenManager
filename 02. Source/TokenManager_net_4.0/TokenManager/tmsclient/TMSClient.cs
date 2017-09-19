using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TokenManager.common;
using TokenManager.Properties;

namespace TokenManager.tmsclient
{
    class TMSClient
    {
        //TEST-------------------------------------------------------------------------------
        private static bool LocalTest = false;
        private static bool FakeSerialList = false;
        private const int PROCESS_TIME = 500;
        //TEST-------------------------------------------------------------------------------


        //Logger for this class
        private static readonly log4net.ILog _LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Base URI
        const string ENPOINT_URL = @"https://123.31.10.17:4443/tms/";
        //Link yeu cau unlock pin
        const string QUERY_UNLOCK_PIN_RESOURCE = "restapi/token/codeunlockuerpin";
        //Link xac nhan otp va unlock pin
        const string UNLOCK_PIN_RESOURCE = "restapi/token/confirmunlockuerpin";
        //Link gia han CTS: kiem tra cert moi, download p12, update danh sach cert (Xem REQUEST_)
        const string RENEW_CERT_RESOURCE = "restapi/certificate/p12";
        //Link cap nhat thong tin khach hang (email, phone)
        const string UPDATE_PROFILE_RESOURCE = "restapi/token/tokenmanager/check/version/";

        //Constant requestType kiểm tra xem có cert mới không
        private const string REQUEST_CHECK_NEW_CERT = "CHECK_IMPORT";
        //Constant requestType download p12 data từ TMS
        private const string REQUEST_GET_P12_DATA = "GET_P12";
        //Constant requestType cập nhật danh sách cert lên TMS
        private const string REQUEST_UPDATE_CERT_LIST = "UPDATE_PROFILE";


        //Luu message loi tu TMS tra ve
        private static string _tmsErrorMsg = "";
        //Luu p12 data & password encrypted
        private static RenewCertMeta _renewData;
        //Luu SO PIN tu TMS
        private static string _soPinBase64 = "";
        //Luu link download phiem ban moi
        private static UpdateMeta _newVersionLink;


        //Ma loi------------------------------------------------------------
        public const int SUCCESSFULL = 0;
        public const int CUSTOMER_INFO_NULL = -1;
        public const int CANNOT_CONNECT_TO_TMS = -2;
        public const int SERVER_NOT_RESPONSE = -3;
        public const int CANNOT_PARSE_TMS_RESPONSE = -4;
        public const int SERVER_RESPONSE_ERROR_CODE = -5;
        public const int NO_P12_DATA_FROM_TMS = -6;
        public const int CANNOT_CHECK_FOR_UDPATE = -7;
        public const int NO_NEW_VERSION_FOUND = -8;
        //End Ma loi---------------------------------------------------------


        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static int QueryRenewCert(CustomerInfo info)
        {
            _LOG.Info("QueryRenewCert: Start query renew certificate to TMS service");

            //-----------------------------------------------------
            if (LocalTest)
            {
                _LOG.Info("QueryRenewCert: Running local testcase with LocalTest=True");
                Thread.Sleep(PROCESS_TIME);
                return SUCCESSFULL;
            }
            //-----------------------------------------------------


            if (info == null)
            {
                _LOG.Error("QueryRenewCert: CustomerInfo null");
                return CUSTOMER_INFO_NULL;
            }
            RenewCertTemplate template = new RenewCertTemplate();
            template.tokenSerialNumber = info.TokenSerialNumber;
            template.tokenModelName = info.TokenModelName;

            //-----------------------------------------------------
            if (FakeSerialList)
            {
                _LOG.Info("QueryRenewCert: Using fake serial number with FakeSerialList=True");
                template.certificateSerialNumberList = "10595a00d288f419";
            }
            else
            {
                template.certificateSerialNumberList = info.GetCertListString();
            }
            //-----------------------------------------------------


            template.typeRequest = REQUEST_CHECK_NEW_CERT;

            string cusInfo = "Customer information=tokenSerialNumber="
                + info.TokenSerialNumber + ". tokenModelName=" + info.TokenModelName
                + ". certificateSerialNumberList=" + info.GetCertListString() + ". typeRequest="
                + REQUEST_CHECK_NEW_CERT;
            _LOG.Info("QueryRenewCert: " + cusInfo);

            string pramJson = JsonConvert.SerializeObject(template);

            string Content = null;
            try
            {
                Content = HandleQuery(pramJson, RENEW_CERT_RESOURCE, Method.PUT);
            }
            catch (TokenManagerException ex)
            {
                _LOG.Error("QueryRenewCert: " + ex.Message);
                if (ex.Message.Equals("" + CANNOT_CONNECT_TO_TMS))
                {
                    return CANNOT_CONNECT_TO_TMS;
                }
                if (ex.Message.Equals("" + SERVER_NOT_RESPONSE))
                {
                    return SERVER_NOT_RESPONSE;
                }
            }

            RenewCertResponse tmsResponse = null;
            try
            {
                tmsResponse = JsonConvert.DeserializeObject<RenewCertResponse>(Content);
            }
            catch (Exception e)
            {
                _LOG.Error("QueryRenewCert: " + e.Message);
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse == null)
            {
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse.code != null && tmsResponse.code.StartsWith("E_"))
            {
                _tmsErrorMsg = tmsResponse.descrition;
                return SERVER_RESPONSE_ERROR_CODE;
            }
            //Khong co cert moi de gia han
            if ("INFO_101".Equals(tmsResponse.code))
            {
                _tmsErrorMsg = tmsResponse.descrition;
                return SERVER_RESPONSE_ERROR_CODE;
            }
            _LOG.Info("QueryRenewCert: There is new cert on TMS. Call download p12 next");
            return SUCCESSFULL;
        }


        public static int UpdateTokenProfile(CustomerInfo info)
        {
            _LOG.Info("UpdateTokenProfile: Start update token profile to TMS service");
            if (info == null)
            {
                _LOG.Error("UpdateTokenProfile: CustomerInfo null");
                return CUSTOMER_INFO_NULL;
            }

            //-----------------------------------------------------
            if (LocalTest)
            {
                Thread.Sleep(PROCESS_TIME);
                return SUCCESSFULL;
            }
            //-----------------------------------------------------

            if (info == null)
            {
                _LOG.Error("UpdateTokenProfile: CustomerInfo null");
                return CUSTOMER_INFO_NULL;
            }
            RenewCertTemplate template = new RenewCertTemplate();
            template.tokenSerialNumber = info.TokenSerialNumber;
            template.tokenModelName = info.TokenModelName;

            //-----------------------------------------------------
            if (LocalTest && FakeSerialList)
            {
                _LOG.Info("UpdateTokenProfile: Using fake serial number with FakeSerialList=True");
                template.certificateSerialNumberList = "10595a00d288f419";
            }
            else
            {
                template.certificateSerialNumberList = info.GetCertListString();
            }
            //-----------------------------------------------------


            template.typeRequest = REQUEST_UPDATE_CERT_LIST;

            string cusInfo = "Customer information=tokenSerialNumber="
                + info.TokenSerialNumber + ". tokenModelName=" + info.TokenModelName
                + ". certificateSerialNumberList=" + info.GetCertListString() + ". typeRequest="
                + REQUEST_UPDATE_CERT_LIST;
            _LOG.Info("UpdateTokenProfile: " + cusInfo);

            string pramJson = JsonConvert.SerializeObject(template);

            string Content = null;
            try
            {
                Content = HandleQuery(pramJson, RENEW_CERT_RESOURCE, Method.PUT);
            }
            catch (TokenManagerException ex)
            {
                _LOG.Error("UpdateTokenProfile: " + ex.Message);
                if (ex.Message.Equals("" + CANNOT_CONNECT_TO_TMS))
                {
                    return CANNOT_CONNECT_TO_TMS;
                }
                if (ex.Message.Equals("" + SERVER_NOT_RESPONSE))
                {
                    return SERVER_NOT_RESPONSE;
                }
            }

            RenewCertResponse tmsResponse = null;
            try
            {
                tmsResponse = JsonConvert.DeserializeObject<RenewCertResponse>(Content);
            }
            catch (Exception e)
            {
                _LOG.Error("UpdateTokenProfile: " + e.Message);
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse == null)
            {
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse.code != null && tmsResponse.code.StartsWith("E_"))
            {
                _tmsErrorMsg = tmsResponse.descrition;
                return SERVER_RESPONSE_ERROR_CODE;
            }
            _LOG.Info("UpdateTokenProfile: Update certificate list information successfull.");
            return SUCCESSFULL;
        }


        /// <summary>
        /// Call TMS service to get pkcs12 data for renew method
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static int DownloadP12Data(CustomerInfo info)
        {
            _LOG.Info("DownloadP12Data: Start download p12 data from TMS service");

            //-----------------------------------------------------
            if (LocalTest)
            {
                _LOG.Info("DownloadP12Data: Running local testcase with LocalTest=True");
                Thread.Sleep(PROCESS_TIME);
                string Base64 = @"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";
                string KeystorePass = @"mHHJIvil3hgNb9Fah7Ig9w==";

                //string Base64 = @"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";
                //string KeystorePass = @"Lm32TNnFQJnRMpxAbYINzg==";

                _renewData = new RenewCertMeta();
                _renewData.keystore = Base64;
                _renewData.keystorePassword = KeystorePass;

                return SUCCESSFULL;
            }
            //-----------------------------------------------------

            if (info == null)
            {
                _LOG.Error("DownloadP12Data: CustomerInfo null");
                return CUSTOMER_INFO_NULL;
            }
            RenewCertTemplate template = new RenewCertTemplate();
            template.tokenSerialNumber = info.TokenSerialNumber;
            template.tokenModelName = info.TokenModelName;
            template.typeRequest = REQUEST_GET_P12_DATA;

            //-----------------------------------------------------
            if (FakeSerialList)
            {
                _LOG.Info("DownloadP12Data: Using fake serial number with FakeSerialList=True");
                template.certificateSerialNumberList = "10595a00d288f419";
            }
            else
            {
                template.certificateSerialNumberList = info.GetCertListString();
            }
            //-----------------------------------------------------

            string cusInfo = "Customer information=tokenSerialNumber=" + info.TokenSerialNumber
                + ". tokenModelName=" + info.TokenModelName + ". certificateSerialNumberList="
                + info.GetCertListString() + ". typeRequest=" + REQUEST_GET_P12_DATA;
            _LOG.Info("DownloadP12Data: " + cusInfo);

            string pramJson = JsonConvert.SerializeObject(template);


            string Content = null;
            try
            {
                Content = HandleQuery(pramJson, RENEW_CERT_RESOURCE, Method.PUT);
            }
            catch (TokenManagerException ex)
            {
                _LOG.Error("DownloadP12Data: " + ex.Message);
                if (ex.Message.Equals("" + CANNOT_CONNECT_TO_TMS))
                {
                    return CANNOT_CONNECT_TO_TMS;
                }
                if (ex.Message.Equals("" + SERVER_NOT_RESPONSE))
                {
                    return SERVER_NOT_RESPONSE;
                }
            }

            _LOG.Info("DownloadP12Data: TMS response=" + Content);

            RenewCertResponse tmsResponse = null;
            try
            {
                tmsResponse = JsonConvert.DeserializeObject<RenewCertResponse>(Content);
            }
            catch (Exception e)
            {
                _LOG.Error("DownloadP12Data: " + e.Message);
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse == null)
            {
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse.code != null && tmsResponse.code.StartsWith("E_"))
            {
                _LOG.Info("DownloadP12Data: TMS service return error message, show in main window");
                _tmsErrorMsg = tmsResponse.descrition;
                return SERVER_RESPONSE_ERROR_CODE;
            }

            if (tmsResponse.metaData == null)
            {
                _LOG.Info("DownloadP12Data: No pkcs12 data in TMS response");
                return NO_P12_DATA_FROM_TMS;
            }
            _renewData = tmsResponse.metaData;
            _LOG.Info("DownloadP12Data: Pkcs12 data downloaded");
            return SUCCESSFULL;
        }


        /// <summary>
        /// Gui yeu cau unlock pin len TMS de sinh OTP
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static int QueryUnlockPin(CustomerInfo info)
        {
            _LOG.Info("QueryUnlockPin: Start query unlock pin to TMS service");
            if (info == null)
            {
                _LOG.Error("QueryUnlockPin: CustomerInfo null");
                return CUSTOMER_INFO_NULL;
            }

            //-----------------------------------------------------
            if (LocalTest)
            {
                _LOG.Info("QueryUnlockPin: Running local testcase with LocalTest=True");
                Thread.Sleep(PROCESS_TIME);
                return SUCCESSFULL;
            }
            //-----------------------------------------------------
            string cusInfo = "Customer information(tokenSerialNumber="
                + info.TokenSerialNumber + ". tokenModelName=" + info.TokenModelName
                + ". certificateSerialNumberList=" + info.GetCertListString()
                + ". typeNotification=" + info.OtpType + ")";
            _LOG.Info("QueryUnlockPin: " + cusInfo);

            QueryUnlockTemplate template = new QueryUnlockTemplate();
            template.tokenSerialNumber = info.TokenSerialNumber;
            template.tokenModelName = info.TokenModelName;
            template.typeNotification = info.OtpType;

            //-----------------------------------------------------
            if (FakeSerialList)
            {
                _LOG.Info("QueryUnlockPin: Using fake serial number with FakeSerialList=True");
                template.certificateSerialNumberList = "10595a00d288f419";
            }
            else
            {
                template.certificateSerialNumberList = info.GetCertListString();
            }
            //-----------------------------------------------------

            string pramJson = JsonConvert.SerializeObject(template);


            string Content = null;
            try
            {
                Content = HandleQuery(pramJson, QUERY_UNLOCK_PIN_RESOURCE, Method.PUT);
            }
            catch (TokenManagerException ex)
            {
                _LOG.Info("QueryUnlockPin: " + ex.Message);
                if (ex.Equals("" + CANNOT_CONNECT_TO_TMS))
                {
                    return CANNOT_CONNECT_TO_TMS;
                }
                if (ex.Equals("" + SERVER_NOT_RESPONSE))
                {
                    return SERVER_NOT_RESPONSE;
                }
                return CANNOT_CONNECT_TO_TMS;
            }

            _LOG.Info("QueryUnlockPin: TMS response=" + Content);

            RootObject tmsResponse = null;
            try
            {
                tmsResponse = JsonConvert.DeserializeObject<RootObject>(Content);
            }
            catch (Exception e)
            {
                _LOG.Error("QueryUnlockPin: " + e.Message);
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse == null)
            {
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse.code != null && tmsResponse.code.StartsWith("E_"))
            {
                _LOG.Info("QueryUnlockPin: TMS service return error message, show in main window");
                _tmsErrorMsg = tmsResponse.descrition;
                return SERVER_RESPONSE_ERROR_CODE;
            }
            _LOG.Info("QueryUnlockPin: Query unlock pin and sent otp code");
            return SUCCESSFULL;
        }


        /// <summary>
        /// Gửi OTP lên TMS để lấy SO Pin
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static int UnlockPin(CustomerInfo info)
        {
            _LOG.Info("UnlockPin: Start unlock user pin");
            if (info == null)
            {
                _LOG.Error("UnlockPin: CustomerInfo null");
                return CUSTOMER_INFO_NULL;
            }

            //-----------------------------------------------------
            if (LocalTest)
            {
                _LOG.Info("UnlockPin: Running local testcase with LocalTest=True");
                Thread.Sleep(PROCESS_TIME);
                string module = "" + Properties.Settings.Default[TokenManagerConstants.CACHED_PKCS11_MODULE_KEY];
                if (module.Contains("Bkav"))
                {
                    //Bkav SO PIN
                    _soPinBase64 = @"tYnrFvjgYDKv60rW24HTaA==";
                }
                else
                {
                    //Vnpt SO PIN
                    _soPinBase64 = @"tYnrFvjgYDKv60rW24HTaA==";
                }
                return SUCCESSFULL;
            }
            //-----------------------------------------------------
            string cusInfo = "Customer information (tokenSerialNumber=" +
                info.TokenSerialNumber + ". tokenModelName=" + info.TokenModelName +
                ". certificateSerialNumberList=" + info.GetCertListString() +
                ". codeUnlockUserPin=" + info.OtpValue;
            _LOG.Info("UnlockPin: " + cusInfo);

            UnlockPinTemplate template = new UnlockPinTemplate();
            template.tokenSerialNumber = info.TokenSerialNumber;
            template.tokenModelName = info.TokenModelName;
            template.codeUnlockUserPin = info.OtpValue;

            //-----------------------------------------------------
            if (FakeSerialList)
            {
                template.certificateSerialNumberList = "10595a00d288f419";
            }
            else
            {
                template.certificateSerialNumberList = info.GetCertListString();
            }
            //-----------------------------------------------------

            string pramJson = JsonConvert.SerializeObject(template);

            string Content = null;
            try
            {
                Content = HandleQuery(pramJson, UNLOCK_PIN_RESOURCE, Method.PUT);
            }
            catch (TokenManagerException ex)
            {
                _LOG.Info("UnlockPin: " + ex.Message);
                if (ex.Message.Equals("" + CANNOT_CONNECT_TO_TMS))
                {
                    return CANNOT_CONNECT_TO_TMS;
                }
                if (ex.Message.Equals("" + SERVER_NOT_RESPONSE))
                {
                    return SERVER_NOT_RESPONSE;
                }
            }

            _LOG.Info("UnlockPin: TMS Response=" + Content);

            RootObject tmsResponse = null;
            try
            {
                tmsResponse = JsonConvert.DeserializeObject<RootObject>(Content);
            }
            catch (Exception e)
            {
                _LOG.Info("UnlockPin: " + e.Message);
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse == null)
            {
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse.code != null && tmsResponse.code.StartsWith("E_"))
            {
                _LOG.Info("UnlockPin: TMS service return error message, show in main window");
                _tmsErrorMsg = tmsResponse.descrition;
                return SERVER_RESPONSE_ERROR_CODE;
            }

            //When every condition passed, get soPin to unlock
            _soPinBase64 = tmsResponse.metaData.soPin;
            _LOG.Info("UnlockPin: Query so pin successfull.");
            return SUCCESSFULL;
        }


        /// <summary>
        /// Update customer profile
        /// </summary>
        public static int UpdateProfile(CustomerInfo info)
        {
            _LOG.Info("Start update profile to TMS service");
            if (info == null)
            {
                _LOG.Error("UpdateProfile: CustomerInfo null");
                return CUSTOMER_INFO_NULL;
            }

            //-----------------------------------------------------
            if (LocalTest)
            {
                Thread.Sleep(PROCESS_TIME);
                return SUCCESSFULL;
            }
            //-----------------------------------------------------

            string cusInfo = "Customer Information=tokenSerialNumber=" + info.TokenSerialNumber
                + ". tokenModelName=" + info.TokenModelName + ". certificateSerialNumberList="
                + info.GetCertListString() + ". typeNotification=" + info.OtpType;
            _LOG.Info("UpdateProfile: " + cusInfo);

            UpdateProfileTemplate template = new UpdateProfileTemplate();
            template.tokenSerialNumber = info.TokenSerialNumber;
            template.tokenModelName = info.TokenModelName;
            template.emailOwner = info.Email;
            template.phoneNumberOwner = info.PhoneNumber;
            string pramJson = JsonConvert.SerializeObject(template);


            string Content = null;
            try
            {
                Content = HandleQuery(pramJson, UPDATE_PROFILE_RESOURCE, Method.PUT);
            }
            catch (TokenManagerException ex)
            {
                _LOG.Info("UpdateProfile: " + ex.Message);
                if (ex.Message.Equals("" + CANNOT_CONNECT_TO_TMS))
                {
                    return CANNOT_CONNECT_TO_TMS;
                }
                if (ex.Message.Equals("" + SERVER_NOT_RESPONSE))
                {
                    return SERVER_NOT_RESPONSE;
                }
            }

            RootObject tmsResponse = null;
            try
            {
                tmsResponse = JsonConvert.DeserializeObject<RootObject>(Content);
            }
            catch (Exception e)
            {
                _LOG.Info("UpdateProfile: " + e.Message);
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse == null)
            {
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse.code != null && tmsResponse.code.StartsWith("E_"))
            {
                _LOG.Info("UpdateProfile: TMS service return error message, show in main window");
                _tmsErrorMsg = tmsResponse.descrition;
                return SERVER_RESPONSE_ERROR_CODE;
            }

            //When every condition passed, get soPin to unlock
            return SUCCESSFULL;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int CheckForUpdate()
        {
            _LOG.Info("Start check for udpate");

            //-----------------------------------------------------
            if (LocalTest)
            {
                Thread.Sleep(PROCESS_TIME);
                return SUCCESSFULL;
            }
            //-----------------------------------------------------
            String appVersion = "";
            RegistryKey key = null;
            try
            {
                key = Registry.LocalMachine.OpenSubKey(TokenManagerConstants.REG_APP_SUBKEY);
            }
            catch (Exception ex)
            {
                _LOG.Error("CheckForUpdate: " + ex.Message);
            }
            try
            {
                appVersion = "" + key.GetValue(TokenManagerConstants.REG_VERSION);
            }
            catch (Exception ex)
            {
                _LOG.Error("CheckForUpdate: " + ex.Message);
            }

            string Content = null;
            try
            {
                Content = HandleQuery("", UPDATE_PROFILE_RESOURCE + appVersion, Method.GET);
            }
            catch (TokenManagerException ex)
            {
                _LOG.Info("CheckForUpdate: " + ex.Message);
                if (ex.Message.Equals("" + CANNOT_CONNECT_TO_TMS))
                {
                    return CANNOT_CONNECT_TO_TMS;
                }
                if (ex.Message.Equals("" + SERVER_NOT_RESPONSE))
                {
                    return SERVER_NOT_RESPONSE;
                }
            }

            UpdateResponse tmsResponse = null;
            try
            {
                tmsResponse = JsonConvert.DeserializeObject<UpdateResponse>(Content);
            }
            catch (Exception e)
            {
                _LOG.Info("CheckForUpdate: " + e.Message);
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse == null)
            {
                return CANNOT_PARSE_TMS_RESPONSE;
            }

            if (tmsResponse.code != null && tmsResponse.code.StartsWith("E_"))
            {
                _LOG.Info("CheckForUpdate: TMS service return error message, show in main window");
                _tmsErrorMsg = tmsResponse.description;
                return SERVER_RESPONSE_ERROR_CODE;
            }

            if (!tmsResponse.code.Equals("INFO_108") || tmsResponse.metaData == null)
            {
                _LOG.Info("CheckForUpdate: No meta data in response");
                return NO_NEW_VERSION_FOUND;
            }
            _newVersionLink = tmsResponse.metaData;
            _LOG.Info("CheckForUpdate: Link for new version getted");

            return SUCCESSFULL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private static string HandleQuery(string param, string resouce, RestSharp.Method method)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(SslHelper.ValidateRemoteCertificate);
            byte[] sslClientP12 = null;
            string password = "";
            try
            {
                sslClientP12 = Convert.FromBase64String(TokenManagerConstants.SSL_PKCS12_DATA);
                byte[] passData = Convert.FromBase64String(TokenManagerConstants.SSL_PKCS12_PASSWORD);
                password = ASCIIEncoding.UTF8.GetString(passData);
            }
            catch (Exception ex)
            {
                _LOG.Info("HandleQuery: Cannot parse ssl client certificate. " + ex.Message);
                throw new TokenManagerException("" + CANNOT_CONNECT_TO_TMS);
            }
            X509Certificate2 certificates = new X509Certificate2();
            certificates.Import(sslClientP12, password, X509KeyStorageFlags.Exportable);

            _LOG.Info("HandleQuery: Query resouce at '" + ENPOINT_URL + resouce + "'");

            RestClient Client = new RestClient(ENPOINT_URL + resouce);
            Client.ClientCertificates = new X509CertificateCollection() { certificates };

            RestRequest request = new RestRequest(method);
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("application/json", param, ParameterType.RequestBody);


            IRestResponse response = null;
            try
            {
                response = Client.Execute(request);
            }
            catch (Exception ex)
            {
                _LOG.Info("HandleQuery: " + ex.Message);
                throw new TokenManagerException("" + CANNOT_CONNECT_TO_TMS);
            }

            if (response == null || response.ErrorException != null)
            {
                string statusString = string.Format("{0} {1} - {2}", (int)response.StatusCode, response.StatusCode, response.ErrorMessage);
                string errorString = "Response status: " + statusString;
                _LOG.Error("HandleQuery: " + errorString);
                throw new TokenManagerException("" + CANNOT_CONNECT_TO_TMS);
            }
            string Content = response.Content;
            if (Content == null || "".Equals(Content))
            {
                throw new TokenManagerException("" + SERVER_NOT_RESPONSE);
            }

            _LOG.Info("HandleQuery: TMS response=" + Content);
            return Content;
        }

        /// <summary>
        /// Get message for each code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetErrorMessage(int code)
        {
            LanguageUtil lang = LanguageUtil.GetInstance();
            switch (code)
            {
                case TMSClient.CUSTOMER_INFO_NULL:
                    return lang.GetValue(LanguageUtil.Key.TMS_CUSTOMER_INFO_NULL);

                case TMSClient.CANNOT_CONNECT_TO_TMS:
                    return lang.GetValue(LanguageUtil.Key.TMS_CANNOT_CONNECT_TO_TMS);

                case TMSClient.SERVER_NOT_RESPONSE:
                    return lang.GetValue(LanguageUtil.Key.TMS_SERVER_NOT_RESPONSE);

                case TMSClient.CANNOT_PARSE_TMS_RESPONSE:
                    return lang.GetValue(LanguageUtil.Key.TMS_CANNOT_PARSE_TMS_RESPONSE);

                case TMSClient.SERVER_RESPONSE_ERROR_CODE:
                    return _tmsErrorMsg;

                case TMSClient.CANNOT_CHECK_FOR_UDPATE:
                    return lang.GetValue(LanguageUtil.Key.TMS_CANNOT_CHECK_FOR_UPDATE);

            }
            return "UNDEFINED ERROR";
        }

        /// <summary>
        /// Get cached pkcs12 data after download p12 successfull.
        /// </summary>
        /// <returns></returns>
        public static RenewCertMeta GetRenewData()
        {
            return _renewData;
        }

        /// <summary>
        /// Get cached so pin after renew cert successfull.
        /// </summary>
        /// <returns></returns>
        public static string GetSoPinBase64()
        {
            return _soPinBase64;
        }

        public static UpdateMeta GetUpdateData()
        {
            return _newVersionLink;
        }
    }
}
