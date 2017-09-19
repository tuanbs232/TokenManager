using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TokenManager.common
{
    class Pkcs11Connector
    {
        //Logger for this class
        private static readonly log4net.ILog _LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Pkcs11Connector Instance = null;
        
        //Pkcs11 module (dll) name
        private string _pkcs11Provider;
        //CSP provider name
        public static string CspProvider;

        private Pkcs11 _pkcs11Module = null;
        private Slot _pkcs11Slot = null;
        private Session _pkcs11Session = null;

        
        private Pkcs11Connector()
        {
            // Private connector, user static method GetInstance() instead of
        }

        /// <summary>
        /// Get a Pkcs11Connector instance match pluged token with supported module
        /// </summary>
        /// <returns></returns>
        public static Pkcs11Connector GetInstance()
        {
            if(Instance == null)
            {
                Init();
            }
            return Instance;
        }

        /// <summary>
        /// Find match pkcs11 module and init neccessary attribute
        /// </summary>
        private static void Init()
        {
            //Destroy all Instance before init
            Destroy();

            string cachedModule = "" + Properties.Settings.Default[TokenManagerConstants.CACHED_PKCS11_MODULE_KEY];
            _LOG.Info("Init: Try connect using cached module '" + cachedModule + "'");
            if (Connect(cachedModule))
            {
                return;
            }

            _LOG.Info("Init: Cached module not match token, try other supported modules.");
            string AllowedModules = "" + Properties.Settings.Default[TokenManagerConstants.ALLOWED_PKCS11_MODULE_KEY];
            if ("".Equals(AllowedModules))
            {
                _LOG.Error("Init: No supported modules setting found");
                return;
            }

            string[] Modules = AllowedModules.Split(",".ToCharArray());
            if(Modules == null || Modules.Count() < 1)
            {
                _LOG.Error("Init: Supported modules setting empty or null");
                return;
            }
            _LOG.Info("Init: All supported modules: " + AllowedModules);

            bool ConnectSuccess = false;
            foreach(string Module in Modules)
            {
                if (cachedModule.Equals(Module))
                {
                    continue;
                }

                _LOG.Info("Init: Try connect using module " + Module);
                if (!Connect(Module))
                {
                    continue;
                }
                else
                {
                    ConnectSuccess = true;
                    break;
                }
            }

            if (!ConnectSuccess)
            {
                _LOG.Error("Init: Token unpluged or no allowed module match");
            }
        }

        /// <summary>
        /// Get match CSP provider name for current pkcs11 module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        private static string _getCspProvider(string module)
        {
            if (TokenManagerConstants.PKCS11_MODULE_BKAVCA.Equals(module))
            {
                return TokenManagerConstants.CSP_PROVIDER_BKAVCA;
            }
            if (TokenManagerConstants.PKCS11_MODULE_VNPT_V6.Equals(module))
            {
                return TokenManagerConstants.CSP_PROVIDER_VNPT_V6;
            }
            if (TokenManagerConstants.PKCS11_MODULE_VNPT_PKI.Equals(module))
            {
                return TokenManagerConstants.CSP_PROVIDER_VNPT_PKI;
            }
            return "";
        }

        /// <summary>
        /// Connect to token using match pkcs11 module
        /// </summary>
        /// <param name="Module"></param>
        /// <returns></returns>
        private static bool Connect(string Module)
        {
            if(Module == null || "".Equals(Module))
            {
                _LOG.Error("Connect: Module file name null or empty");
                return false;
            }
            Pkcs11 Pkcs11Module = null;
            try
            {
                Pkcs11Module = new Pkcs11(Module, AppType.MultiThreaded);
            }
            catch(Exception ex)
            {
                _LOG.Error("Connect: " + ex.Message);
            }

            if (Pkcs11Module == null)
            {
                return false;
            }

            List<Slot> Slots = Pkcs11Module.GetSlotList(SlotsType.WithTokenPresent);
            if (Slots == null || Slots.Count == 0)
            {
                _LOG.Error("Connect: Opp! Wrong module. try other");
                return false;
            }

            Slot TokenSlot = Slots.ElementAt(0);
            if (TokenSlot == null)
            {
                _LOG.Error("Connect: Slot not useable");
                return false; 
            }
            try
            {
                TokenInfo info = TokenSlot.GetTokenInfo();
            }
            catch(Pkcs11Exception ex)
            {
                if (ex.Message.Contains("CKR_TOKEN_NOT_RECOGNIZED"))
                {
                    //Tokens does not exist or 
                    _LOG.Error("Connect: Get token info failed. Wrong token or corrupted");
                }
                return false;
            }

            _LOG.Info("Connect: Connect successfull using module " + Module);
            Properties.Settings.Default[TokenManagerConstants.CACHED_PKCS11_MODULE_KEY] = Module;
            Properties.Settings.Default.Save();

            Instance = new Pkcs11Connector();
            Instance._pkcs11Module = Pkcs11Module;
            Instance._pkcs11Slot = TokenSlot;

            Instance._pkcs11Provider = Module;
            CspProvider = _getCspProvider(Module);

            return true;
        }

        /// <summary>
        /// Used when token unplug, exit program, ...
        /// </summary>
        public static void Destroy()
        {
            _LOG.Info("Destroy: Destroy all Pkcs11Connector instance and close all pkcs11 session");
            if(Instance != null)
            {
                if(Instance._pkcs11Session != null)
                {
                    _LOG.Info("Destroy: Destroy current session");
                    try
                    {
                        Instance._pkcs11Session.Logout();
                    } 
                    catch(Pkcs11Exception ex)
                    {
                        _LOG.Info("Destroy: " + ex.Message);
                    }
                    Instance._pkcs11Session = null;
                }

                if (Instance._pkcs11Slot != null)
                {
                    Instance._pkcs11Slot.CloseAllSessions();
                    Instance._pkcs11Slot = null;
                }

                if (Instance._pkcs11Module != null)
                {
                    Instance._pkcs11Module = null;
                }

                Instance = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserPin"></param>
        /// <returns></returns>
        public Session OpenReadWriteSession(string UserPin)
        {
            TryLogoutSession();

            if (_pkcs11Slot == null)
            {
                _LOG.Error("OpenReadWriteSession: Cannot open readonly session (Slot null)");
                return null;
            }

            try
            {
                _pkcs11Slot.CloseAllSessions();
                this._pkcs11Session = _pkcs11Slot.OpenSession(SessionType.ReadWrite);
                this._pkcs11Session.Login(CKU.CKU_USER, Encoding.ASCII.GetBytes(UserPin));
                _LOG.Info("OpenReadWriteSession: Open new R/W session successfull");
            }
            catch (Pkcs11Exception ex)
            {
                this._pkcs11Session = null;
                _LOG.Error("OpenReadWriteSession: " + ex.Message);
            }

            return this._pkcs11Session;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SoPin"></param>
        /// <returns></returns>
        public Session OpenSOSession(string SoPin)
        {
            TryLogoutSession();

            if (_pkcs11Slot == null)
            {
                _LOG.Error("OpenSOSession: Cannot open readonly session (Slot null)");
                return null;
            }

            try
            {
                _pkcs11Slot.CloseAllSessions();
                this._pkcs11Session = _pkcs11Slot.OpenSession(SessionType.ReadWrite);
                this._pkcs11Session.Login(CKU.CKU_SO, Encoding.ASCII.GetBytes(SoPin));
                _LOG.Info("OpenSOSession: Open new SO session successfull");
            }
            catch (Pkcs11Exception ex)
            {
                this._pkcs11Session = null;
                _LOG.Error("OpenSOSession: " + ex.Message);
            }

            return this._pkcs11Session;
        }

        /// <summary>
        /// 
        /// </summary>
        private void TryLogoutSession()
        {
            if (this._pkcs11Session != null)
            {
                try
                {
                    this._pkcs11Session.Logout();
                }
                catch (Pkcs11Exception ex)
                {
                    _LOG.Error("TryLogoutSession: " + ex.Message);
                }
            }
        }

        public Pkcs11 GetPkcs11()
        {
            return this._pkcs11Module;
        }

        public Slot GetTokenSlot()
        {
            return this._pkcs11Slot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Session OpenReadOnlySession()
        {
            TryLogoutSession();

            if(_pkcs11Slot == null)
            {
                _LOG.Error("OpenReadOnlySession: Cannot open readonly session (Slot null)");
                return null;
            }

            try
            {
                _pkcs11Slot.CloseAllSessions();
                this._pkcs11Session = _pkcs11Slot.OpenSession(SessionType.ReadOnly);
                _LOG.Info("OpenReadOnlySession: Open new R/O session successfull)");
            }
            catch (Pkcs11Exception ex)
            {
                _LOG.Error("OpenReadOnlySession: " + ex.Message);
            }

            return this._pkcs11Session;
        }
    }
}
