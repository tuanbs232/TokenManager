﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TokenManager.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string CACHED_PKCS11_MODULE_KEY {
            get {
                return ((string)(this["CACHED_PKCS11_MODULE_KEY"]));
            }
            set {
                this["CACHED_PKCS11_MODULE_KEY"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("vnptca_p11_v6,BkavCA,vnpt-ca_csp11")]
        public string ALLOWED_PKCS11_MODULE {
            get {
                return ((string)(this["ALLOWED_PKCS11_MODULE"]));
            }
            set {
                this["ALLOWED_PKCS11_MODULE"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("OPEN")]
        public string SIDE_MENU_STATE {
            get {
                return ((string)(this["SIDE_MENU_STATE"]));
            }
            set {
                this["SIDE_MENU_STATE"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0410131413111210,")]
        public string HAS_UPDATED_CERT_LIST {
            get {
                return ((string)(this["HAS_UPDATED_CERT_LIST"]));
            }
            set {
                this["HAS_UPDATED_CERT_LIST"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("LIGHT")]
        public string SKIN {
            get {
                return ((string)(this["SKIN"]));
            }
            set {
                this["SKIN"] = value;
            }
        }
    }
}
