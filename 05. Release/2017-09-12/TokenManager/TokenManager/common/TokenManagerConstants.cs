using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.common
{
    class TokenManagerConstants
    {
        //App Setting cached lai module vua ket noi
        public const string CACHED_PKCS11_MODULE_KEY = "CACHED_PKCS11_MODULE_KEY";
        //App Setting danh sach module duoc ho tro
        public const string ALLOWED_PKCS11_MODULE_KEY = "ALLOWED_PKCS11_MODULE";
        //App Setting trang thai side menu
        public const string SIDE_MENU_STATE = "SIDE_MENU_STATE";
        //Khoa bi mat cho AES
        public const string AES_SECRET_KEY = "dGhhbmdudHRAdm5wdC52bg==";
        //Mau kiem tra email
        public const string EMAIL_REGEX = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        //Default User PIN to reset
        public const string DEFAULT_USER_PIN = "Lm32TNnFQJnRMpxAbYINzg==";

        public const string PKCS11_MODULE_BKAVCA = "BkavCA";
        public const string CSP_PROVIDER_BKAVCA = "Bkav-CA-CTDT Token CSP";

        public const string PKCS11_MODULE_VNPT_V6 = "vnptca_p11_v6";
        public const string CSP_PROVIDER_VNPT_V6 = "VNPT-CA-CTDT V6-Token CSP";

        public const string PKCS11_MODULE_VNPT_PKI = "vnpt-ca_csp11";
        public const string CSP_PROVIDER_VNPT_PKI = "VNPT-CA-CTDT PKI-Token CSP";
    }
}
