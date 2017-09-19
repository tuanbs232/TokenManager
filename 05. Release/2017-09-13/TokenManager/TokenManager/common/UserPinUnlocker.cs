using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TokenManager.tmsclient;

namespace TokenManager.common
{
    class UserPinUnlocker : ObserverAble
    {
        public static int SUCCESSFULL = 0;
        public static int UNDEFINED_ERROR = -1;

        private List<Observer> _observers = new List<Observer>();

        public UserPinUnlocker()
        {

        }

        public UserPinUnlocker(Observer obj)
        {
            this._observers.Add(obj);
        }

        /// <summary>
        /// Gửi hình thức xác thực OTP lên TMS. 
        /// Khởi tạo 1 thread để tránh bị crash
        /// Invoke về MainWindow khi request xong
        /// </summary>
        /// <returns></returns>
        private CustomerInfo info;
        public void queryUnlockPin(string otpType)
        {
            info = null;
            try
            {
                info = Pkcs11Util.GetCustomerTokenInfo();
            }
            catch (TokenManagerException ex)
            {
                if (ex.Message.Contains("SLOT NULL"))
                {
                    notify("Cắm VNPT-CA Token để tiếp tục.", CommonMessage.MESSAGE_TYPE_ERROR, null);
                    return;
                }
                else
                {
                    notify(ex.Message, CommonMessage.MESSAGE_TYPE_ERROR, null);
                    return;
                }
            }
            info.OtpType = otpType;
            ThreadStart childThread = new ThreadStart(HandleRequest);
            Thread thread = new Thread(childThread);
            thread.Start();
            notify("Đang gửi yêu cầu unlock pin...", CommonMessage.MESSAGE_TYPE_ACTION_WITH_LOADING, null);
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleRequest()
        {
            int resultCode = TMSClient.QueryUnlockPin(info);

            MainWindow main = (MainWindow)_observers.ElementAt(0);

            if (resultCode != TMSClient.SUCCESSFULL)
            {
                string errorMessage = TMSClient.GetErrorMessage(resultCode);
                main.InvokeErrorDialog(errorMessage);
                return;
            }
            
            main.InvokeActionMessage(CommonMessage.UNLOCK_PIN_OTP_SENT, null);
        }


        /// <summary>
        /// Được gọi sau khi form nhập OTP hiển thị
        /// Gọi TMS gửi OTP và nhận SO PIN về
        /// Thực hiện Unlock
        /// Invoke về MainWindow để thông báo
        /// </summary>
        /// <param name="otp"></param>
        public void ConfirmOTP(string otp)
        {
            info = null;
            try
            {
                info = Pkcs11Util.GetCustomerTokenInfo();
            }
            catch (TokenManagerException ex)
            {
                if (ex.Message.Contains("SLOT NULL"))
                {
                    notify("Cắm VNPT-CA Token để tiếp tục.", CommonMessage.MESSAGE_TYPE_ERROR, null);
                    return;
                }
                else
                {
                    notify(ex.Message, CommonMessage.MESSAGE_TYPE_ERROR, null);
                    return;
                }
            }
            info.OtpValue = otp;


            ThreadStart childThread = new ThreadStart(HandleUnlockPin);
            Thread thread = new Thread(childThread);
            thread.Start();
            notify("Đang kiểm tra mã xác nhận và mở khóa mã PIN...", CommonMessage.MESSAGE_TYPE_ACTION_WITH_LOADING, null);
        }

        /// <summary>
        /// Thuc thi trong thread con
        /// Goi TMS lay soPIN, reset User PIN va gui notify message
        /// </summary>
        private void HandleUnlockPin()
        {
            int resultCode = TMSClient.UnlockPin(info);
            
            MainWindow main = (MainWindow)_observers.ElementAt(0);

            if (resultCode != TMSClient.SUCCESSFULL)
            {
                string errorMessage = TMSClient.GetErrorMessage(resultCode);
                main.InvokeErrorDialog(errorMessage);
                return;
            }

            //Get SO PIn Bas64 value if request success.
            string SopinBase64 = TMSClient.GetSoPinBase64();

            string SoPin = null;
            try
            {
                SoPin = AES.DecryptToString(SopinBase64);
            }
            catch (CryptographicException)
            {
                main.InvokeErrorDialog("Không thể giải mã dữ liệu từ TMS");
                return;
            }
            catch (FormatException)
            {
                main.InvokeErrorDialog("Dữ liệu TMS không đúng định dạng");
                return;
            }

            //Handle unlock userpin
            try
            {
                Pkcs11Util.UnlockPin(SoPin);
            }
            catch (TokenManagerException ex)
            {
                main.InvokeErrorDialog(ex.Message);
                return;
            }
            string defaultPin = "12345678";
            try
            {
                defaultPin = AES.DecryptToString(TokenManagerConstants.DEFAULT_USER_PIN);
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine(ex.Message);
            }
            main.InvokeUnlockSuccess();
            main.InvokeMessageDialog("Đã mở khóa mã PIN và chuyển về mặc định " + 
                defaultPin + ". Hãy chọn chức năng reset pin để đổi lại.");
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
