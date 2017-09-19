using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.common
{

    /// <summary>
    /// 
    /// </summary>
    class CommonMessage
    {
        //Message types----------------------------------------------------------
        public const int MESSAGE_TYPE_ACTION = 1;
        public const int MESSAGE_TYPE_INFO = 2;
        public const int MESSAGE_TYPE_ERROR = 3;
        public const int MESSAGE_TYPE_CONFIRM = 4;
        public const int MESSAGE_TYPE_ACTION_WITH_LOADING = 5;
        //-----------------------------------------------------------------------

        public const string UNLOCK_USER_PIN_ACTION = "UNLOCK_PIN_ACTION";
        public const string RELOAD_CERT_LIST = "RELOAD_CERT_LIST";
        public const string UNLOCK_PIN_OTP_ENTERED = "UNLOCK_PIN_OTP_ENTERED";

        public const string RENEW_CERT_ACTION = "RENEW_CERT_ACTION";

        //Message khi chon xong hinh thuc gui OTP, tiep tuc voi action cha.
        public const string OTP_TYPE_SELECTED = "OTP_TYPE_SELECTED";

        //Message khi query unlock pin len TMS tra ve ket qua thanh cong (da gui OTP);
        public const string UNLOCK_PIN_OTP_SENT = "UNLOCK_PIN_OTP_SENT";

        //Message khi query Renew cert len TMS tra ve ket qua thanh cong (da gui OTP);
        public const string RENEW_CERT_OTP_SENT = "RENEW_CERT_OTP_SENT";

        //Message khi yeu cau xacs thuc ma pin thanh cong
        public const string CONFIRM_USER_PIN_SUCCESS = "CONFIRM_USER_PIN_SUCCESS";

        //Da co cts moi, nguoi dung dong y cap nhat
        public const string CONFIRM_RENEW_CERT_OK = "CONFIRM_RENEW_CERT_OK";
    }
}
