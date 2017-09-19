using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TokenManager.common
{
    class Pkcs11Util : ObserverAble
    {
        //Logger for this class
        private static readonly log4net.ILog _LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const int MIN_USER_PIN_LENGHT = 8;

        public const int SUCCESSFULL = 0;
        public const int USER_PIN_LOCKED = -1;
        public const int USER_PIN_INVALID = -2;
        public const int TOKEN_UNPLUG = -3;

        private const int CANNOT_DECRYPT_DATA = -4;
        private const int P12_DATA_INVALID = -5;
        public const int CANNOT_IMPORT_PRIVATE_KEY = -6;
        private const int UNSUPPORTED_KEY_TYPE = -7;
        public const int CANNOT_IMPORT_CERT = -8;
        public const int CERT_NOT_MATCH_ANY_KEY = -9;
        public const int CAN_NOT_IMPORT_PUBLIC_KEY = -10;

        private List<Observer> observers = new List<Observer>();


        /// <summary>
        /// Decrypt p12 data from TMS and import to token
        /// </summary>
        /// <param name="p12Base64">Encrypted base64 string p12 keystore data</param>
        /// <param name="passBase64">Encrypted base64 string p12 keystore password</param>
        public static int ImportP12Data(string p12Base64, string passBase64, string userPin)
        {
            _LOG.Info("ImportP12Data: start import p12 data to token");
            byte[] Pkcs12Data = null;
            string Password = null;
            _LOG.Info("ImportP12Data: Decrypt pkcs12 data and password");
            try
            {
                Pkcs12Data = AES.DecryptToBytes(p12Base64);
                Password = AES.DecryptToString(passBase64);
            }
            catch (CryptographicException ex)
            {
                _LOG.Info("ImportP12Data: Cannot decrypt p12 data. " + ex.Message);
                return CANNOT_DECRYPT_DATA;
            }
            catch (FormatException ex)
            {
                _LOG.Info("ImportP12Data: Cannot decrypt p12 data. " + ex.Message);
                return CANNOT_DECRYPT_DATA;
            }

            //Verify pkcs12 data
            int checkP12 = _checkPkcs12Data(Pkcs12Data, Password);
            if(SUCCESSFULL != checkP12)
            {
                return checkP12;
            }
            
            Pkcs11Connector connector = Pkcs11Connector.GetInstance();
            if (connector == null)
            {
                Pkcs11Connector.Destroy();
                return TOKEN_UNPLUG;
            }
            Session session = connector.OpenReadWriteSession(userPin);

            if (session == null)
            {
                Pkcs11Connector.Destroy();
                return USER_PIN_INVALID;
            }

            X509Certificate2 x509Cert = null;
            try
            {
                x509Cert = new X509Certificate2(Pkcs12Data, Password, 
                    X509KeyStorageFlags.Exportable);
            }
            catch(CryptographicException ex)
            {
                _LOG.Error("ImportP12Data: " + ex);
                return P12_DATA_INVALID;
            }
            string label = x509Cert.GetNameInfo(System.Security.Cryptography.X509Certificates.X509NameType.DnsName, false) + "'s VNPT Certification Authority ID";

            SHA1 sha1 = SHA1.Create();
            byte[] modulus = Pkcs12Importer.GetPrivateKeyModulus(x509Cert);
            if(modulus == null)
            {
                modulus = session.GenerateRandom(20);
            }
            byte[] cka_id = sha1.ComputeHash(modulus);

            //Create private key object
            int importPriv = Pkcs12Importer.ImportPrivateKey(session, x509Cert, label, cka_id);
            if(Pkcs12Importer.SUCCESSFULL != importPriv)
            {
                _LOG.Error("ImportP12Data: Import private key failed");
                return CANNOT_IMPORT_PRIVATE_KEY;
            }
            else
            {
                _LOG.Info("ImportP12Data: Import private key successfull");
            }

            //Create public key object
            int importPub = Pkcs12Importer.ImportPublicKey(session, x509Cert, label, cka_id);
            if(Pkcs12Importer.SUCCESSFULL != importPub)
            {
                _LOG.Info("ImportP12Data: Import public key failed");
                Pkcs12Importer.RollbackState(session);
                return CAN_NOT_IMPORT_PUBLIC_KEY;
            }else
            {
                _LOG.Info("ImportP12Data: Import public key successfull");
            }

            //Create certificate object
            int importCert = Pkcs12Importer.ImportCertificate(session, x509Cert, label, cka_id);
            if (Pkcs12Importer.SUCCESSFULL != importCert)
            {
                _LOG.Info("ImportP12Data: Import user certificate failed");
                Pkcs12Importer.RollbackState(session);
                return importCert;
            }else
            {
                _LOG.Info("ImportP12Data: Import user certificate successfull");
            }

            _LOG.Info("Pkcs12 keystore has been imported");
            return SUCCESSFULL;
        }

        /// <summary>
        /// Verify pkcs12 data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        private static int _checkPkcs12Data(byte[] data, string pass)
        {
            _LOG.Info("_checkPkcs12Data: Verify pkcs12 data befor import to token.");
            //Load pkcs12 data to store.
            MemoryStream Stream = new MemoryStream(data);
            Pkcs12Store Store = null;
            _LOG.Info("_checkPkcs12Data: Load pkcs12 data to store");
            try
            {
                Store = new Pkcs12Store(Stream, pass.ToCharArray());
            }
            catch (Exception ex)
            {
                _LOG.Error("_checkPkcs12Data: Cannot load pkcs12 store. " + ex.Message);
                return P12_DATA_INVALID;
            }

            if (Store == null || Store.Aliases == null)
            {
                _LOG.Error("_checkPkcs12Data: P12Store null or no aliases in store");
                return P12_DATA_INVALID;
            }

            string Alias = null;

            //Only get first Alias;
            IEnumerable Aliases = Store.Aliases;
            foreach (object obj in Aliases)
            {
                if (Store.IsKeyEntry("" + obj))
                {
                    Alias = "" + obj;
                    break;
                }
            }
            _LOG.Info("_checkPkcs12Data: Found key alias=" + Alias);
            if (Alias == null)
            {
                _LOG.Error("_checkPkcs12Data: No key alias found in p12 data");
                return P12_DATA_INVALID;
            }

            AsymmetricKeyEntry keyEntry = Store.GetKey(Alias);
            if (keyEntry == null || keyEntry.Key == null)
            {
                _LOG.Error("_checkPkcs12Data: Key entry not found for this alias");
                return P12_DATA_INVALID;
            }

            RsaPrivateCrtKeyParameters privateKey = null;
            if (keyEntry.Key.IsPrivate)
            {
                privateKey = keyEntry.Key as RsaPrivateCrtKeyParameters;
            }
            if (privateKey == null)
            {
                _LOG.Error("_checkPkcs12Data: Private key not found in key entry");
                return P12_DATA_INVALID;
            }
            _LOG.Info("_checkPkcs12Data: Private key successfull loaded");


            X509CertificateEntry CertEntry = Store.GetCertificate(Alias);
            if (CertEntry == null || CertEntry.Certificate == null)
            {
                _LOG.Error("_checkPkcs12Data: User certificate not found");
                return P12_DATA_INVALID;
            }
            _LOG.Info("_checkPkcs12Data: Pkcs12 data has been verified.");
            return SUCCESSFULL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="privateKey"></param>
        /// <param name="cert"></param>
        /// <param name="label"></param>
        /// <param name="cka_id"></param>
        /// <returns></returns>
        private static int ImportPrivateKey(Session session, RsaPrivateCrtKeyParameters privateKey, Org.BouncyCastle.X509.X509Certificate cert, string label, byte[]cka_id)
        {
            _LOG.Info("ImportPrivateKey: Start import private key");
            X509Certificate2 x509Cert = new X509Certificate2();
            x509Cert.Import(cert.GetEncoded());

            List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_RSA));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE, true));
            //Set to true so object will in token after session logout
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, label));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SUBJECT, cert.SubjectDN.GetDerEncoded()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ID, cka_id));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SENSITIVE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SENSITIVE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODIFIABLE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXTRACTABLE, false));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODULUS, privateKey.Modulus.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, privateKey.PublicExponent.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE_EXPONENT, privateKey.Exponent.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_1, privateKey.P.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_2, privateKey.Q.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_1, privateKey.DP.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_2, privateKey.DQ.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_COEFFICIENT, privateKey.QInv.ToByteArray()));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_DECRYPT, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SIGN, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SIGN_RECOVER, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_UNWRAP, true));

            try
            {
                ObjectHandle objectHandle = session.CreateObject(objectAttributes);
                _LOG.Info("ImportPrivateKey: Private key imported with handle " + objectHandle.ObjectId);
                return SUCCESSFULL;
            }
            catch (Pkcs11Exception ex)
            {
                _LOG.Error("ImportPrivateKey: " + ex.Message);
                return CANNOT_IMPORT_PRIVATE_KEY;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cert"></param>
        /// <param name="label"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int ImportPublicKey(Session session, Org.BouncyCastle.X509.X509Certificate cert, string label, byte[] id)
        {
            _LOG.Info("ImportPublicKey: Start import public key");
            AsymmetricKeyParameter pubKeyParams = cert.GetPublicKey();
            if (!(pubKeyParams is RsaKeyParameters))
            {
                _LOG.Error("ImportPublicKey: Public key is not a Rsa Public Key");
                return UNSUPPORTED_KEY_TYPE;
            }
            RsaKeyParameters rsaPubKeyParams = (RsaKeyParameters)pubKeyParams;

            List<ObjectAttribute> publicKeyAttributes = new List<ObjectAttribute>();
            publicKeyAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PUBLIC_KEY));
            publicKeyAttributes.Add(new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_RSA));
            publicKeyAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));

            publicKeyAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, label));
            publicKeyAttributes.Add(new ObjectAttribute(CKA.CKA_SUBJECT, cert.SubjectDN.GetDerEncoded()));
            publicKeyAttributes.Add(new ObjectAttribute(CKA.CKA_ID, id));
            publicKeyAttributes.Add(new ObjectAttribute(CKA.CKA_ENCRYPT, true));
            publicKeyAttributes.Add(new ObjectAttribute(CKA.CKA_VERIFY, true));

            publicKeyAttributes.Add(new ObjectAttribute(CKA.CKA_MODULUS, rsaPubKeyParams.Modulus.ToByteArray()));
            publicKeyAttributes.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, rsaPubKeyParams.Exponent.ToByteArray()));

            // Create public key object
            try
            {
                session.CreateObject(publicKeyAttributes);
                return SUCCESSFULL;
            }
            catch (Pkcs11Exception e)
            {
                _LOG.Error("ImportPublicKey: " + e.Message);
                return CANNOT_IMPORT_CERT;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cert"></param>
        /// <param name="label"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int ImportCertficate(Session session, Org.BouncyCastle.X509.X509Certificate cert, string label, byte[] id)
        {
            _LOG.Info("ImportCertficate: Start import certificate");
            AsymmetricKeyParameter pubKeyParams = cert.GetPublicKey();
            if (!(pubKeyParams is RsaKeyParameters))
            {
                return UNSUPPORTED_KEY_TYPE;
            }
            RsaKeyParameters rsaPubKeyParams = (RsaKeyParameters)pubKeyParams;

            // Find corresponding private key
            List<ObjectAttribute> privKeySearchTemplate = new List<ObjectAttribute>();
            privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY));
            privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_RSA));
            privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_MODULUS, rsaPubKeyParams.Modulus.ToByteArray()));
            privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));
            //privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, rsaPubKeyParams.Exponent.ToByteArray()));

            List<ObjectHandle> foundObjects = session.FindAllObjects(privKeySearchTemplate);
            if (foundObjects.Count < 1)
            {
                _LOG.Error("ImportCertficate: No private key match user certificate.");
                return CERT_NOT_MATCH_ANY_KEY;
            }

            ObjectHandle privKeyObjectHandle = foundObjects[0];

            // Read CKA_LABEL and CKA_ID attributes of private key
            List<CKA> privKeyAttrsToRead = new List<CKA>();
            privKeyAttrsToRead.Add(CKA.CKA_LABEL);
            privKeyAttrsToRead.Add(CKA.CKA_ID);
            privKeyAttrsToRead.Add(CKA.CKA_MODULUS);

            List<ObjectAttribute> privKeyAttributes = session.GetAttributeValue(privKeyObjectHandle, privKeyAttrsToRead);

            // Define attributes of new certificate object
            List<ObjectAttribute> certificateAttributes = new List<ObjectAttribute>();
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));
            //certificateAttributes.Add(new ObjectAttribute(CKA.CKA_MODULUS, privKeyAttributes[2].GetValueAsByteArray()));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE, false));
            //certificateAttributes.Add(new ObjectAttribute(CKA.CKA_MODIFIABLE, true));
            //certificateAttributes.Add(new ObjectAttribute(CKA.CKA_TRUSTED, false));

            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, label));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_SUBJECT, cert.SubjectDN.GetDerEncoded()));

            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_ID, privKeyAttributes[0].GetValueAsByteArray()));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_ISSUER, cert.IssuerDN.ToString()));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_SERIAL_NUMBER, new DerInteger(cert.SerialNumber).GetDerEncoded()));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_VALUE, cert.GetEncoded()));

            // Create certificate object
            try
            {
                session.CreateObject(certificateAttributes);
                return SUCCESSFULL;
            }
            catch(Pkcs11Exception e)
            {
                _LOG.Error("ImportCertficate: " + e.Message);
                return CANNOT_IMPORT_CERT;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetErrorMessage(int code)
        {
            LanguageUtil lang = LanguageUtil.GetInstance();
            switch (code)
            {
                case USER_PIN_LOCKED:
                    return lang.GetValue(LanguageUtil.Key.PKCS11_UTIL_TOKEN_LOCKED);
                case USER_PIN_INVALID:
                    return lang.GetValue(LanguageUtil.Key.CHANGEPIN_CHECK_PIN_INCORRECT);
                case TOKEN_UNPLUG:
                    return lang.GetValue(LanguageUtil.Key.CHANGEPIN_CHECK_PLUG_TOKEN);
                case CANNOT_DECRYPT_DATA:
                    //See below case
                case P12_DATA_INVALID:
                    return lang.GetValue(LanguageUtil.Key.PKCS11_UTIL_P12_DATA_INVALID);
                case CANNOT_IMPORT_PRIVATE_KEY:
                    return lang.GetValue(LanguageUtil.Key.PKCS11_UTIL_CANNOT_IMPORT_PRIVATEKEY);
                case UNSUPPORTED_KEY_TYPE:
                    return lang.GetValue(LanguageUtil.Key.PKCS11_UTIL_UNSUPPORT_KEY_TYPE);
                case CANNOT_IMPORT_CERT:
                    return lang.GetValue(LanguageUtil.Key.PKCS11_UTIL_CANNOT_UPDATE_CERT);
                case CERT_NOT_MATCH_ANY_KEY:
                    return lang.GetValue(LanguageUtil.Key.PKCS11_UTIL_CERT_KEY_NOT_MATCH);
                default:
                    return lang.GetValue(LanguageUtil.Key.PKCS11_UTIL_UNDEFINED_ERROR);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static CustomerInfo GetCustomerTokenInfo()
        {
            Pkcs11Connector connector = Pkcs11Connector.GetInstance();
            if (connector == null)
            {
                Pkcs11Connector.Destroy();
                throw new TokenManagerException("GetCustomerTokenInfo: Cannot connect token. SLOT NULL");
            }
            CustomerInfo result = new CustomerInfo();

            Slot slot = connector.GetTokenSlot();
            if(slot == null || slot.GetTokenInfo() == null)
            {
                throw new TokenManagerException("GetCustomerTokenInfo: Cannot get token info");
            }
            TokenInfo info = slot.GetTokenInfo();
            result.TokenSerialNumber = info.SerialNumber;
            result.TokenModelName = info.Label;

            Session session = connector.OpenReadOnlySession();
            if(session == null)
            {
                throw new TokenManagerException("GetCustomerTokenInfo: Cannot open session.");
            }

            List<X509Certificate2> certs = ListAllCertificate(session);
            List<string> certSerialList = new List<string>();
            foreach(X509Certificate2 cert in certs)
            {
                if(cert == null)
                {
                    continue;
                }
                certSerialList.Add(cert.SerialNumber);
            }
            result.CertSerialList = certSerialList;

            Pkcs11Connector.Destroy();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private static List<X509Certificate2> ListAllCertificate(Session session)
        {
            _LOG.Info("ListAllCertificate: Start get all certificate in token slot.");
            List<X509Certificate2> result = new List<X509Certificate2>();

            if(session == null)
            {
                _LOG.Error("ListAllCertificate: Opp!!! Get error Session NULL");
                return result;
            }

            List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE));

            session.FindObjectsInit(objectAttributes);
            List<ObjectHandle> certs = session.FindObjects(10);

            foreach (ObjectHandle cert in certs)
            {
                // Prepare list of object attributes we want to read
                List<CKA> attributes = new List<CKA>();
                attributes.Add(CKA.CKA_VALUE);

                // Get value of specified attributes
                List<ObjectAttribute> attr = session.GetAttributeValue(cert, attributes);

                // CKA_VALUE contains raw certificate data
                byte[] certData = attr[0].GetValueAsByteArray();

                // Use raw certificate data to construct X509Certificate2 object
                X509Certificate2 x509Cert = new X509Certificate2(certData);
                result.Add(x509Cert);
            }

            return result;
        }


        /// <summary>
        /// Open RO session and read all certificate in slot
        /// </summary>
        /// <returns></returns>
        public static List<X509Certificate2> ListAllCertificate()
        {
            Pkcs11Connector connector = Pkcs11Connector.GetInstance();
            if(connector == null || connector.OpenReadOnlySession() == null)
            {
                Pkcs11Connector.Destroy();
                throw new TokenManagerException("ListAllCertificate: TOKEN_UNPLUGED");
            }

            Session session = connector.OpenReadOnlySession();

            List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE));

            List<X509Certificate2> result = new List<X509Certificate2>();
            try
            {
                session.FindObjectsInit(objectAttributes);
            }
            catch(Pkcs11Exception e)
            {
                _LOG.Error(e.Message);
                return result;
            }
            List<ObjectHandle> certs = session.FindObjects(10);


            foreach (ObjectHandle cert in certs)
            {
                // Prepare list of object attributes we want to read
                List<CKA> attributes = new List<CKA>();
                attributes.Add(CKA.CKA_VALUE);

                // Get value of specified attributes
                List<ObjectAttribute> attr = session.GetAttributeValue(cert, attributes);

                // CKA_VALUE contains raw certificate data
                byte[] certData = attr[0].GetValueAsByteArray();

                // Use raw certificate data to construct X509Certificate2 object
                X509Certificate2 x509Certificate2 = new X509Certificate2(certData);
                result.Add(x509Certificate2);
            }
            Pkcs11Connector.Destroy();
            return result;
        }

        /// <summary>
        /// Initial user PIN again using SO PIN from TMS
        /// </summary>
        /// <param name="soPin"></param>
        public static void UnlockPin(string soPin)
        {
            _LOG.Info("UnlockPin: Start unlock User Pin");
            //Try connect to token
            Pkcs11Connector connector = Pkcs11Connector.GetInstance();
            if (connector == null)
            {
                Pkcs11Connector.Destroy();
                _LOG.Error("UnlockPin: Cannot connect to token.");
                throw new TokenManagerException("Cannot connect token. SLOT NULL");
            }

            //Open R/W session and login with SO PIN
            Session session = connector.OpenSOSession(soPin);
            if(session == null)
            {
                Pkcs11Connector.Destroy();
                _LOG.Error("UnlockPin: Cannot connect to token.");
                throw new TokenManagerException("Cannot connect token. SESSION NULL");
            }

            string defaultPin = null;
            try
            {
                defaultPin = AES.DecryptToString(TokenManagerConstants.DEFAULT_USER_PIN);
            }
            catch(CryptographicException ex)
            {
                _LOG.Error("UnlockPin: Cannot decrypt user pin " + TokenManagerConstants.DEFAULT_USER_PIN + ". " + ex.Message);
                throw new TokenManagerException("Cannot decrypt user pin");
            }

            //Init new user Pin
            session.InitPin(Encoding.ASCII.GetBytes(defaultPin));
            _LOG.Info("UnlockPin: User pin already reset to default PIN.");
            Pkcs11Connector.Destroy();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserPin"></param>
        /// <returns></returns>
        public static int TryLogin(string UserPin)
        {
            _LOG.Info("TryLogin: Try call User Login to smartcard to check User Pin");
            Pkcs11Connector connector = Pkcs11Connector.GetInstance();
            if(connector == null)
            {
                Pkcs11Connector.Destroy();
                return TOKEN_UNPLUG;
            }

            Session session = connector.OpenReadWriteSession(UserPin);
            if(session == null)
            {
                Pkcs11Connector.Destroy();
                return USER_PIN_INVALID;
            }
            Pkcs11Connector.Destroy();
            _LOG.Info("TryLogin: Test user login successfull");
            return SUCCESSFULL;
        }


        /// <summary>
        /// Reset User Pin
        /// </summary>
        /// <param name="OldPin"></param>
        /// <param name="NewPin"></param>
        /// <returns></returns>
        public static int ResetUserPin(string OldPin, string NewPin)
        {
            _LOG.Info("ResetUserPin: Start reset user PIN");
            Pkcs11Connector connector = Pkcs11Connector.GetInstance();
            if(connector == null)
            {
                Pkcs11Connector.Destroy();
                return TOKEN_UNPLUG;
            }

            Session session = connector.OpenReadWriteSession(OldPin);
            if(session == null)
            {
                Pkcs11Connector.Destroy();
                return USER_PIN_INVALID;
            }

            byte[] OldPinBytes = Encoding.ASCII.GetBytes(OldPin);
            byte[] NewPinBytes = Encoding.ASCII.GetBytes(NewPin);
            try
            {
                session.SetPin(OldPinBytes, NewPinBytes);
            }
            catch (Pkcs11Exception e)
            {
                _LOG.Error("ResetUserPin: User login failed. " + e.Message);
                if (e.Message.Contains("CKR_PIN_LEN_RANGE"))
                {
                    return USER_PIN_INVALID;
                }

                if (e.Message.Contains("CKR_PIN_INCORRECT"))
                {
                    return USER_PIN_INVALID;
                }
                session.Logout();
            }
            session.Logout();
            return SUCCESSFULL;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool CheckTokenLocked()
        {
            Pkcs11Connector connector = Pkcs11Connector.GetInstance();
            if (connector == null)
            {
                Pkcs11Connector.Destroy();
                return false;
            }
            try
            {
                TokenFlags flag = connector.GetTokenSlot().GetTokenInfo().TokenFlags;
                
                return flag.UserPinLocked;
            }
            catch (Exception ex)
            {
                _LOG.Error("CheckTokenLocked: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetMinPinLength()
        {
            _LOG.Info("GetMinPinLength: Called");
            Pkcs11Connector connector = Pkcs11Connector.GetInstance();
            if (connector == null)
            {
                _LOG.Error("GetMinPinLength: Pkcs11Connector null");
                Pkcs11Connector.Destroy();
                return MIN_USER_PIN_LENGHT;
            }
            try
            {
                return (int) connector.GetTokenSlot().GetTokenInfo().MinPinLen;
            }
            catch (Exception ex)
            {
                _LOG.Error("GetMinPinLength: " + ex.Message);
                return MIN_USER_PIN_LENGHT;
            }
        }


        public void attach(Observer obj)
        {
            this.observers.Add(obj);
        }


        public void detach(Observer obj)
        {
            this.observers.Remove(obj);
        }


        public void notify(string message, int messageType, object[] param)
        {
            foreach (Observer obj in this.observers)
            {
                obj.Update(message, messageType, param);
            }
        }
    }
}
