using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TokenManager.tmsclient;

namespace TokenManager.common
{
    class CertificateRenew : ObserverAble
    {
        private static readonly log4net.ILog _LOG = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<Observer> _observers = new List<Observer>();

        private CustomerInfo _info;
        private string _userPin;

        public const string OTP_TYPE_EMAIL = "EMAIL";
        public const string OTP_TYPE_PHONE = "PHONE";

        public CertificateRenew()
        {
            //Default constructor
        }

        public CertificateRenew(Observer obj)
        {
            if(obj != null)
            {
                attach(obj);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPin"></param>
        public void QueryRenew(string userPin)
        {
            _userPin = userPin;
            ThreadStart childThread = new ThreadStart(HandleQueryRenew);
            Thread thread = new Thread(childThread);
            thread.Start();
            notify("Đang gửi yêu cầu gia hạn...", CommonMessage.MESSAGE_TYPE_ACTION_WITH_LOADING, null);
        }


        /// <summary>
        /// 
        /// </summary>
        private void HandleQueryRenew()
        {
            MainWindow main = (MainWindow)_observers.ElementAt(0);

            //Lay thong tin token
            _info = null;
            try
            {
                _info = Pkcs11Util.GetCustomerTokenInfo();
            }
            catch (TokenManagerException ex)
            {
                _LOG.Error("HandleQueryRenew: " + ex.Message);
                if (ex.Message.Contains("SLOT NULL"))
                {
                    main.InvokeErrorDialog("Cắm VNPT CA Token để tiếp tục");
                    return;
                }
                else
                {
                    main.InvokeErrorDialog(ex.Message);
                    return;
                }
            }

            //Goi TMS service kiem tra xe co cert moi chua
            int resultCode = TMSClient.QueryRenewCert(_info);
            if (resultCode != TMSClient.SUCCESSFULL)
            {
                _LOG.Error("HandleQueryRenew: " + TMSClient.GetErrorMessage(resultCode));
                string errorMessage = TMSClient.GetErrorMessage(resultCode);
                main.InvokeErrorDialog(errorMessage);
                return;
            }

            main.InvokeConfirmDialog("Đã có certificate mới trên hệ thống. Bạn có muốn cập nhật ngay?", CommonMessage.CONFIRM_RENEW_CERT_OK);
        }


        /// <summary>
        /// Thuc hien lay pkcs12 data tu TMSClient, 
        /// import vao token, cap nhat ds cert len TMS
        /// </summary>
        public void ImportP12Data()
        {
            ThreadStart childThread = new ThreadStart(HandleUpdateData);
            Thread thread = new Thread(childThread);
            thread.Start();
            notify("Đang cập nhật chứng thư số...", 
                CommonMessage.MESSAGE_TYPE_ACTION_WITH_LOADING, null);
        }


        /// <summary>
        /// Ham thuc thi trong thread con.
        /// Thuc hien lay pkcs12 data tu TMSClient, 
        /// import vao token, cap nhat ds cert len TMS
        /// </summary>
        private void HandleUpdateData()
        {
            MainWindow main = (MainWindow)_observers.ElementAt(0);

            //Get token information
            _info = null;
            try
            {
                _info = Pkcs11Util.GetCustomerTokenInfo();
            }
            catch (TokenManagerException ex)
            {
                if (ex.Message.Contains("SLOT NULL"))
                {
                    main.InvokeErrorDialog("Cắm VNPT CA Token để tiếp tục");
                    return;
                }
                else
                {
                    main.InvokeErrorDialog(ex.Message);
                    return;
                }
            }

            //Download pkcs12 data from TMS
            int resultCode = TMSClient.DownloadP12Data(_info);
            if (resultCode != TMSClient.SUCCESSFULL)
            {
                string errorMessage = TMSClient.GetErrorMessage(resultCode);
                _LOG.Error("HandleUpdateData: Download pkcs12 data error=" + errorMessage);
                main.InvokeErrorDialog(errorMessage);
                return;
            }

            //Get cached pkcs12 data after download from TMS
            RenewCertMeta p12Data = TMSClient.GetRenewData();
            if(p12Data == null)
            {
                //Da kiem tra
            }

            int code = Pkcs11Util.ImportP12Data(p12Data.keystore, p12Data.keystorePassword, _userPin);
            if(Pkcs11Util.SUCCESSFULL == code)
            {
                _LOG.Info("HandleUpdateData: Import pkcs12 data successfull.");
                main.InvokeMessageDialog("Cập nhật chứng thư số thành công");
                main.InvokeActionMessage(CommonMessage.RELOAD_CERT_LIST, null);

                //Update danh sach token chua cap nhat danh sach cert len TMS
                string newValue = Properties.Settings.Default.HAS_UPDATED_CERT_LIST +
                    _info.TokenSerialNumber + ",";
                Properties.Settings.Default.HAS_UPDATED_CERT_LIST = newValue;
                Properties.Settings.Default.Save();

                //Update danh sach cert len TMS
                UpdateCertList();
            }
            else
            {
                main.InvokeErrorDialog(Pkcs11Util.GetErrorMessage(code));
            }
        }


        /// <summary>
        /// Check xem da cap nhat danh sach cert len TMS chua.
        /// Neu chua thuc hien gui danh sach len TMS va cap nhat trang thai
        /// </summary>
        public void UpdateCertList()
        {
            string updateCerList = Properties.Settings.Default.HAS_UPDATED_CERT_LIST;
            _LOG.Info("UpdateCertList: Check and update certificate list to TMS. List token hasn't updated cert list=" + updateCerList);
            if (String.IsNullOrEmpty(updateCerList))
            {
                return;
            }

            //Get token information
            _info = null;
            try
            {
                _info = Pkcs11Util.GetCustomerTokenInfo();
            }
            catch (TokenManagerException ex)
            {
                _LOG.Error("UpdateCertList: " + ex.Message);
                return;
            }

            if (String.IsNullOrEmpty(_info.TokenSerialNumber))
            {
                _LOG.Error("UpdateCertList: Cannot read token serial number");
                return;
            }
            if (!updateCerList.Contains(_info.TokenSerialNumber))
            {
                return;
            }
            _LOG.Info("UpdateCertList: Update certificate list to TMS for token " +
                _info.TokenSerialNumber + ". Cert list=" + _info.GetCertListString());

            int resultCode = TMSClient.UpdateTokenProfile(_info);
            if (resultCode != TMSClient.SUCCESSFULL)
            {
                //Background task, no user notify message here
                _LOG.Error("UpdateCertList: Cannot update cert list. TMS response=" + 
                    TMSClient.GetErrorMessage(resultCode));
                return;
            }
            else
            {
                _LOG.Info("UpdateCertList: Successfull update cert list for token " + 
                    _info.TokenSerialNumber);
                string newValue = updateCerList.Replace(_info.TokenSerialNumber + ",", "");
                Properties.Settings.Default.HAS_UPDATED_CERT_LIST = newValue;
                Properties.Settings.Default.Save();
            }
        }


        public void attach(Observer obj)
        {
            this._observers.Add(obj);
        }


        public void detach(Observer obj)
        {
            this._observers.Remove(obj);
        }


        public void notify(string message, int messageType, object[] param)
        {
            foreach(Observer obj in this._observers)
            {
                if(obj == null)
                {
                    continue;
                }
                obj.Update(message, messageType, param);
            }
        }
    }
}
