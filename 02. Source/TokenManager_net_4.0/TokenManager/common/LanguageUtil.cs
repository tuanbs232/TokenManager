using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TokenManager.common
{
    class LanguageUtil
    {
        private static log4net.ILog _LOG = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);
        public static readonly string VI = "vi";
        public static readonly string EN = "en";

        //TODO Next release or ...
        private static LanguageUtil _instance;

        private ExternalConfig _prop = null;
        private string _lang = VI;

        public string GetValue(string key)
        {
            return _prop.get(key);
        }

        private LanguageUtil()
        {
            //Private constructor
            string lang = _getRegLanguage();
            if (!lang.Equals(_lang) || _prop == null)
            {
                _lang = lang;
                _prop = new ExternalConfig();
                _prop.reload(lang + ".lg");
            }
        }

        public bool IsVietnamese()
        {
            return VI.Equals(_lang);
        }

        public static void SetLanguage(string lan)
        {
            if(!(VI.Equals(lan) || EN.Equals(lan)))
            {
                return;
            }
            RegistryKey key = null;
            try
            {
                key = Registry.LocalMachine.OpenSubKey(TokenManagerConstants.REG_APP_SUBKEY, true);
                key.SetValue(TokenManagerConstants.REG_LANG, lan);
            }
            catch (Exception ex)
            {
                _LOG.Error("SetLanguage: " + ex.Message);
                return;
            }
        }

        public static LanguageUtil GetInstance()
        {
            _instance = new LanguageUtil();
            return _instance;
        }


        private static string _getRegLanguage()
        {
            String lang = VI;
            RegistryKey key = null;
            try
            {
                key = Registry.LocalMachine.OpenSubKey(TokenManagerConstants.REG_APP_SUBKEY);
            }
            catch (Exception ex)
            {
                _LOG.Error("GetInstance: " + ex.Message);
            }
            try
            {
                lang = "" + key.GetValue(TokenManagerConstants.REG_LANG);
                if (lang.Contains(EN))
                {
                    lang = EN;
                }
                else
                {
                    lang = VI;
                }
            }
            catch (Exception ex)
            {
                _LOG.Error("GetInstance: " + ex.Message);
            }

            return lang;
        }


        public static class Key
        {
            public static readonly string MENU_CERTIFICATE = "MENU_CERTIFICATE";
            public static readonly string MENU_TOOL = "MENU_TOOL";
            public static readonly string MENU_CONFIG = "MENU_CONFIG";
            public static readonly string MENU_SUPPORT = "MENU_SUPPORT";

            public static readonly string TITLE_CERTIFICATE = "TITLE_CERTIFICATE";
            public static readonly string TITLE_TOOL = "TITLE_TOOL";
            public static readonly string TITLE_CONFIG = "TITLE_CONFIG";

            public static readonly string TABLE_COLUMN_CN = "TABLE_COLUMN_CN";
            public static readonly string TABLE_COLUMN_VALID_FROM = "TABLE_COLUMN_VALID_FROM";
            public static readonly string TABLE_COLUMN_VALID_TO = "TABLE_COLUMN_VALID_TO";

            public static readonly string TABLE_VIEW_CERT_TOOL_TRIP = "TABLE_VIEW_CERT_TOOL_TRIP";
            public static readonly string TABLE_VIEW_SERIAL_TOOL_TRIP = "TABLE_VIEW_SERIAL_TOOL_TRIP";
            public static readonly string TABLE_CHECK_OCSP_TOOL_TRIP = "TABLE_CHECK_OCSP_TOOL_TRIP";

            public static readonly string TOOL_UNLOCK_TITLE = "TOOL_UNLOCK_TITLE";
            public static readonly string TOOL_UNLOCK_DES = "TOOL_UNLOCK_DES";
            public static readonly string TOOL_UNLOCK_BTN = "TOOL_UNLOCK_BTN";
            public static readonly string TOOL_CHANGEPIN_TITLE = "TOOL_CHANGEPIN_TITLE";
            public static readonly string TOOL_CHANGEPIN_DES = "TOOL_CHANGEPIN_DES";
            public static readonly string TOOL_CHANGEPIN_BTN = "TOOL_CHANGEPIN_BTN";
            public static readonly string TOOL_RENEW_TITLE = "TOOL_RENEW_TITLE";
            public static readonly string TOOL_RENEW_DES = "TOOL_RENEW_DES";
            public static readonly string TOOL_RENEW_BTN = "TOOL_RENEW_BTN";
            public static readonly string TOOL_PROFILE_TITLE = "TOOL_PROFILE_TITLE";
            public static readonly string TOOL_PROFILE_DES = "TOOL_PROFILE_DES";
            public static readonly string TOOL_PROFILE_BTN = "TOOL_PROFILE_BTN";

            public static readonly string CONFIG_CHANGESKIN_TITLE = "CONFIG_CHANGESKIN_TITLE";
            public static readonly string CONFIG_CHANGELANGUAGE_TITLE = "CONFIG_CHANGELANGUAGE_TITLE";

            public static readonly string NOTIFY_TOKEN_UNPLUGED = "NOTIFY_TOKEN_UNPLUGED";
            public static readonly string NOTIFY_TOKEN_PLUGED = "NOTIFY_TOKEN_PLUGED";

            public static readonly string SUPPORT_ABOUT_BTN = "SUPPORT_ABOUT_BTN";
            public static readonly string SUPPORT_HELP_BTN = "SUPPORT_HELP_BTN";
            public static readonly string SUPPORT_CONTACT_TITLE = "SUPPORT_CONTACT_TITLE";
            public static readonly string SUPPORT_PHONE = "SUPPORT_PHONE";
            public static readonly string SUPPORT_HELP_DOWNTHUE_TITLE = "SUPPORT_HELP_DOWNTHUE_TITLE";
            public static readonly string SUPPORT_HELP_DOWNTHUE_DES = "SUPPORT_HELP_DOWNTHUE_DES";
            public static readonly string SUPPORT_HELP_DOWNTHUE_BTN = "SUPPORT_HELP_DOWNTHUE_BTN";
            public static readonly string SUPPORT_HELP_DOWNBHXH_TITLE = "SUPPORT_HELP_DOWNBHXH_TITLE";
            public static readonly string SUPPORT_HELP_DOWNBHXH_DES = "SUPPORT_HELP_DOWNBHXH_DES";
            public static readonly string SUPPORT_HELP_DOWNBHXH_BTN = "SUPPORT_HELP_DOWNBHXH_BTN";

            public static readonly string MESSAGE_UNSUPPORT_IN_CURRENT_VERSION = "MESSAGE_UNSUPPORT_IN_CURRENT_VERSION";
            public static readonly string MESSAGE_TOKEN_UNPLUGED = "MESSAGE_TOKEN_UNPLUGED";
            public static readonly string MESSAGE_TOKEN_LOCKED = "MESSAGE_TOKEN_LOCKED";
            public static readonly string MESSAGE_UNDEFINED_ERROR = "MESSAGE_UNDEFINED_ERROR";

            public static readonly string CONTEXT_MENU_SHOW = "CONTEXT_MENU_SHOW";
            public static readonly string CONTEXT_MENU_TOOLS = "CONTEXT_MENU_TOOLS";
            public static readonly string CONTEXT_MENU_UPDATE = "CONTEXT_MENU_UPDATE";
            public static readonly string CONTEXT_MENU_ABOUT = "CONTEXT_MENU_ABOUT";
            public static readonly string CONTEXT_MENU_EXIT = "CONTEXT_MENU_EXIT";

            public static readonly string BTN_CLOSE = "BTN_CLOSE";
            public static readonly string BTN_OK = "BTN_OK";

            public static readonly string CHANGEPIN_CURENT_PIN_TITLE = "CHANGEPIN_CURENT_PIN_TITLE";
            public static readonly string CHANGEPIN_NEW_PIN_TITLE = "CHANGEPIN_NEW_PIN_TITLE";
            public static readonly string CHANGEPIN_NEW_PIN_AGAIN_TITLE = "CHANGEPIN_NEW_PIN_AGAIN_TITLE";
            public static readonly string CHANGEPIN_OK_BTN = "CHANGEPIN_OK_BTN";
            public static readonly string CHANGEPIN_MESSAGE_SUCCESSFULL = "CHANGEPIN_MESSAGE_SUCCESSFULL";
            public static readonly string CHANGEPIN_MESSAGE_ERROR = "CHANGEPIN_MESSAGE_ERROR";
            public static readonly string CHANGEPIN_CHECK_ENTER_CURENT_PIN = "CHANGEPIN_CHECK_ENTER_CURENT_PIN";
            public static readonly string CHANGEPIN_CHECK_PIN_INCORRECT = "CHANGEPIN_CHECK_PIN_INCORRECT";
            public static readonly string CHANGEPIN_CHECK_PLUG_TOKEN = "CHANGEPIN_CHECK_PLUG_TOKEN";
            public static readonly string CHANGEPIN_CHECK_ENTER_NEW_PIN = "CHANGEPIN_CHECK_ENTER_NEW_PIN";
            public static readonly string CHANGEPIN_CHECK_MIN_PIN_LENGTH = "CHANGEPIN_CHECK_MIN_PIN_LENGTH";
            public static readonly string CHANGEPIN_CHECK_ENTER_NEW_PIN_AGAIN = "CHANGEPIN_CHECK_ENTER_NEW_PIN_AGAIN";
            public static readonly string CHANGEPIN_CHECK_PIN_AGAIN_INVALID = "CHANGEPIN_CHECK_PIN_AGAIN_INVALID";
            public static readonly string LINK_LABEL_ENTER_CODE = "LINK_LABEL_ENTER_CODE";

            public static readonly string DIALOG_CONFIRM_TITLE = "DIALOG_CONFIRM_TITLE";

            public static readonly string DIALOG_OTP_CONFIRM_TITLE = "DIALOG_OTP_CONFIRM_TITLE";
            public static readonly string DIALOG_OTP_CONFIRM_LABEL = "DIALOG_OTP_CONFIRM_LABEL";
            public static readonly string DIALOG_OTP_CONFIRM_ERROR = "DIALOG_OTP_CONFIRM_ERROR";

            public static readonly string DIALOG_SELECT_OTP_TITLE = "DIALOG_SELECT_OTP_TITLE";

            public static readonly string DIALOG_PIN_CONFIRM_TITLE = "DIALOG_PIN_CONFIRM_TITLE";
            public static readonly string DIALOG_PIN_CONFIRM_LABEL = "DIALOG_PIN_CONFIRM_LABEL";

            public static readonly string DIALOG_ERROR_TITLE = "DIALOG_ERROR_TITLE";
            public static readonly string DIALOG_MESSAGE_TITLE = "DIALOG_MESSAGE_TITLE";

            public static readonly string DIALOG_SERIAL_TITLE = "DIALOG_SERIAL_TITLE";
            public static readonly string DIALOG_SERIAL_LABEL = "DIALOG_SERIAL_LABEL";
            public static readonly string DIALOG_SERIAL_LINK = "DIALOG_SERIAL_LINK";
            public static readonly string DIALOG_SERIAL_MESSAGE = "DIALOG_SERIAL_MESSAGE";

            public static readonly string DIALOG_UPDATE_PROFILE_TITLE = "DIALOG_UPDATE_PROFILE_TITLE";
            public static readonly string DIALOG_UPDATE_PROFILE_PHONE = "DIALOG_UPDATE_PROFILE_PHONE";
            public static readonly string DIALOG_UPDATE_PROFILE_PIN = "DIALOG_UPDATE_PROFILE_PIN";
            public static readonly string DIALOG_UPDATE_PROFILE_OK_BTN = "DIALOG_UPDATE_PROFILE_OK_BTN";
            public static readonly string DIALOG_UPDATE_PROFILE_PROCESSING = "DIALOG_UPDATE_PROFILE_PROCESSING";
            public static readonly string DIALOG_UPDATE_PROFILE_SUCCESSFULL = "DIALOG_UPDATE_PROFILE_SUCCESSFULL";
            public static readonly string DIALOG_UPDATE_PROFILE_ENTER_EMAIL = "DIALOG_UPDATE_PROFILE_ENTER_EMAIL";
            public static readonly string DIALOG_UPDATE_PROFILE_EMAIL_INVALID = "DIALOG_UPDATE_PROFILE_EMAIL_INVALID";
            public static readonly string DIALOG_UPDATE_PROFILE_ENTER_PHONE = "DIALOG_UPDATE_PROFILE_ENTER_PHONE";
            public static readonly string DIALOG_UPDATE_PROFILE_PHONE_INVALID = "DIALOG_UPDATE_PROFILE_PHONE_INVALID";
            public static readonly string DIALOG_UPDATE_PROFILE_ENTER_PIN = "DIALOG_UPDATE_PROFILE_ENTER_PIN";

            public static readonly string RENEW_CERT_QUERY_RENEW = "RENEW_CERT_QUERY_RENEW";
            public static readonly string RENEW_CERT_HAVE_NEW_CERT = "RENEW_CERT_HAVE_NEW_CERT";
            public static readonly string RENEW_CERT_UPDATING = "RENEW_CERT_UPDATING";
            public static readonly string RENEW_CERT_SUCCESSFULL = "RENEW_CERT_SUCCESSFULL";

            public static readonly string CERT_UTIL_OCSP_CHECKING = "CERT_UTIL_OCSP_CHECKING";
            public static readonly string CERT_UTIL_OCSP_NO_CA = "CERT_UTIL_OCSP_NO_CA";
            public static readonly string CERT_UTIL_OCSP_CERT_STATUS = "CERT_UTIL_OCSP_CERT_STATUS";

            public static readonly string PKCS11_UTIL_TOKEN_LOCKED = "PKCS11_UTIL_TOKEN_LOCKED";
            public static readonly string PKCS11_UTIL_P12_DATA_INVALID = "PKCS11_UTIL_P12_DATA_INVALID";
            public static readonly string PKCS11_UTIL_CANNOT_IMPORT_PRIVATEKEY = "PKCS11_UTIL_CANNOT_IMPORT_PRIVATEKEY";
            public static readonly string PKCS11_UTIL_UNSUPPORT_KEY_TYPE = "PKCS11_UTIL_UNSUPPORT_KEY_TYPE";
            public static readonly string PKCS11_UTIL_CANNOT_UPDATE_CERT = "PKCS11_UTIL_CANNOT_UPDATE_CERT";
            public static readonly string PKCS11_UTIL_CERT_KEY_NOT_MATCH = "PKCS11_UTIL_CERT_KEY_NOT_MATCH";
            public static readonly string PKCS11_UTIL_UNDEFINED_ERROR = "PKCS11_UTIL_UNDEFINED_ERROR";

            public static readonly string PIN_UNLOCKER_PROCESSING = "PIN_UNLOCKER_PROCESSING";
            public static readonly string PIN_UNLOCKER_UNLOCKING = "PIN_UNLOCKER_UNLOCKING";
            public static readonly string PIN_UNLOCKER_RESETED = "PIN_UNLOCKER_RESETED";
            public static readonly string PIN_UNLOCKER_LET_CHANGE = "PIN_UNLOCKER_LET_CHANGE";
            public static readonly string TMS_CANNOT_DECRYPT_DATA = "TMS_CANNOT_DECRYPT_DATA";
            public static readonly string TMS_DATA_MALFORMED = "TMS_DATA_MALFORMED";

            public static readonly string TMS_CUSTOMER_INFO_NULL = "TMS_CUSTOMER_INFO_NULL";
            public static readonly string TMS_CANNOT_CONNECT_TO_TMS = "TMS_CANNOT_CONNECT_TO_TMS";
            public static readonly string TMS_SERVER_NOT_RESPONSE = "TMS_SERVER_NOT_RESPONSE";
            public static readonly string TMS_CANNOT_PARSE_TMS_RESPONSE = "TMS_CANNOT_PARSE_TMS_RESPONSE";
            public static readonly string TMS_CANNOT_CHECK_FOR_UPDATE = "TMS_CANNOT_CHECK_FOR_UPDATE";

            public static readonly string UPDATE_CHECKING_NEW_VERSION = "UPDATE_CHECKING_NEW_VERSION";
            public static readonly string UPDATE_NO_NEW_VERSION = "UPDATE_NO_NEW_VERSION";
            public static readonly string UPDATE_HAVE_NEW_VERSION = "UPDATE_HAVE_NEW_VERSION";
            public static readonly string UPDATE_CANCEL_BUTTON = "UPDATE_CANCEL_BUTTON";
            public static readonly string UPDATE_DOWNLOADING_UPDATE_PACKAGE = "UPDATE_DOWNLOADING_UPDATE_PACKAGE";
            public static readonly string UPDATE_TITLE = "UPDATE_TITLE";
            public static readonly string UPDATE_CANNOT_DOWNLOAD_PACKAGE = "UPDATE_CANNOT_DOWNLOAD_PACKAGE";
            public static readonly string UPDATE_UPDATE_NOW_BTN = "UPDATE_UPDATE_NOW_BTN";
            public static readonly string UPDATE_COMPLETE_UPDATE_PACKAGE = "UPDATE_COMPLETE_UPDATE_PACKAGE";
        }

    }
}
