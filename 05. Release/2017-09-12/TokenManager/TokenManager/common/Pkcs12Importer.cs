using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TokenManager.common
{
    class Pkcs12Importer
    {
        public const int SUCCESSFULL = 0;

        private const string CKA_MODULUS = "Modulus";
        private const string CKA_PUBLIC_EXPONENT = "Exponent";
        private const string CKA_PRIVATE_EXPONENT = "D";
        private const string CKA_PRIME_1 = "P";
        private const string CKA_PRIME_2 = "Q";
        private const string CKA_EXPONENT_1 = "DP";
        private const string CKA_EXPONENT_2 = "DQ";
        private const string CKA_COEFFICIENT = "InverseQ";

        static ObjectHandle _privateKeyHandle = null;
        static ObjectHandle _publicKeyHandle = null;
        static ObjectHandle _certHandle = null;

        //Logger for this class
        private static readonly log4net.ILog _LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get private key attribute value from xml string by tag name
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static byte[] GetValueByTagName(string data, string tag)
        {
            if (String.IsNullOrEmpty(data) || String.IsNullOrEmpty(tag))
            {
                _LOG.Error("GetValueByTagName: Private data null or empty");
                return null;
            }

            XmlDocument xmlDocument = new XmlDocument();
            XmlNodeList nodeList;
            string value = null;
            try
            {
                xmlDocument.LoadXml(data);
                nodeList = xmlDocument.GetElementsByTagName(tag);
                if (nodeList != null && nodeList.Count == 1)
                    value = ((XmlNode)nodeList.Item(0)).InnerXml;

                return Convert.FromBase64String(value);
            }
            catch (XmlException e)
            {
                _LOG.Error("GetValueByTagName: " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                _LOG.Error("GetValueByTagName: " + e.Message);
            }
            catch (FormatException e)
            {
                _LOG.Error("GetValueByTagName: " + e.Message);
            }

            return null;
        }

        /// <summary>
        /// Import user certificate into token
        /// </summary>
        /// <param name="session">R/W user session</param>
        /// <param name="x509Cert">X509Certificate2 instance</param>
        /// <param name="label">Specify label for certificate object</param>
        /// <param name="ckaId">Specify cka_id for certificate object</param>
        /// <returns></returns>
        public static int ImportCertificate(Session session, X509Certificate2 x509Cert, String label, byte[] ckaId)
        {
            _LOG.Info("ImportCertificate: Start import user certificate");
            List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));
            //objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODIFIABLE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE, false));

            //objectAttributes.Add(new ObjectAttribute(CKA.CKA_TRUSTED, true));
            //objectAttributes.Add(new ObjectAttribute(CKA.CKA_SERIAL_NUMBER, x509Cert.GetSerialNumber()));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, label));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SUBJECT, x509Cert.SubjectName.RawData));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ID, ckaId));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ISSUER, x509Cert.IssuerName.RawData));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SERIAL_NUMBER, ParseAsBytes(x509Cert.SerialNumber)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_VALUE, x509Cert.RawData));
            try
            {
                _certHandle = session.CreateObject(objectAttributes);
                _LOG.Info("User certificate was imported with handle " + _certHandle.ObjectId);
                return SUCCESSFULL;
            }
            catch (Pkcs11Exception ex)
            {
                _LOG.Error("ImportCertificate: " + ex.Message);
                return Pkcs11Util.CANNOT_IMPORT_CERT;
            }
        }

        private static byte[] ParseAsBytes(string s)
        {
            return Enumerable.Range(0, s.Length / 2)
                             .Select(i => byte.Parse(s.Substring(i* 2, 2), 
                                                     NumberStyles.AllowHexSpecifier))
                             .ToArray();
        }

        /// <summary>
    /// Create private object in token
    /// </summary>
    /// <param name="session"></param>
    /// <param name="x509Cert"></param>
    /// <param name="label"></param>
    /// <param name="ckaId"></param>
    /// <returns></returns>
        public static int ImportPrivateKey(Session session, X509Certificate2 x509Cert, String label, byte[] ckaId)
        {
            _LOG.Info("ImportPrivateKey: Start import private key");
            if (x509Cert == null)
            {
                //Already checked
            }

            string xmlKey = x509Cert.PrivateKey.ToXmlString(true);

            //Create attributes template for private object
            List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_RSA));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, label));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ID, ckaId));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE, true));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SIGN_RECOVER, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SIGN, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_DECRYPT, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SENSITIVE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_UNWRAP, true));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXTRACTABLE, false));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_DERIVE, false));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_LOCAL, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ALWAYS_SENSITIVE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_NEVER_EXTRACTABLE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ALWAYS_AUTHENTICATE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_WRAP_WITH_TRUSTED, false));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODULUS, GetValueByTagName(xmlKey, CKA_MODULUS)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, GetValueByTagName(xmlKey, CKA_PUBLIC_EXPONENT)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE_EXPONENT, GetValueByTagName(xmlKey, CKA_PRIVATE_EXPONENT)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_1, GetValueByTagName(xmlKey, CKA_PRIME_1)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_2, GetValueByTagName(xmlKey, CKA_PRIME_2)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_1, GetValueByTagName(xmlKey, CKA_EXPONENT_1)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_2, GetValueByTagName(xmlKey, CKA_EXPONENT_2)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_COEFFICIENT, GetValueByTagName(xmlKey, CKA_COEFFICIENT)));

            try
            {
                _privateKeyHandle = session.CreateObject(objectAttributes);
                _LOG.Info("ImportPrivateKey: Private key imported with handle " + _privateKeyHandle.ObjectId);
                return SUCCESSFULL;
            }
            catch (Pkcs11Exception ex)
            {
                _LOG.Info("ImportPrivateKey: " + ex.Message);
                return Pkcs11Util.CANNOT_IMPORT_PRIVATE_KEY;
            }
        }

        /// <summary>
        /// Create public key object in token
        /// </summary>
        /// <param name="session"></param>
        /// <param name="x509Cert"></param>
        /// <param name="label"></param>
        /// <param name="ckaId"></param>
        /// <returns></returns>
        public static int ImportPublicKey(Session session, X509Certificate2 x509Cert, String label, byte[] ckaId)
        {
            _LOG.Info("ImportPublicKey: Start import public key");
            if(x509Cert == null)
            {
                //Already checked
            }
            RSACryptoServiceProvider key = x509Cert.PublicKey.Key as RSACryptoServiceProvider;
            byte[] expoenet = null;
            byte[] modulus = null;
            if (key != null)
            {
                RSAParameters parameters = key.ExportParameters(false);
                expoenet = parameters.Exponent;
                modulus = parameters.Modulus;
            }

            List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PUBLIC_KEY));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_RSA));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, label));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ID, ckaId));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE, false));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODIFIABLE, true));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ENCRYPT, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_VERIFY, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_VERIFY_RECOVER, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_WRAP, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_TRUSTED, true));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_DERIVE, false));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_LOCAL, true));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODULUS, modulus));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, expoenet));

            try
            {
                _publicKeyHandle = session.CreateObject(objectAttributes);
                _LOG.Info("ImportPublicKey: Public key imported with handle " + _publicKeyHandle.ObjectId);
                return SUCCESSFULL;
            }
            catch (Pkcs11Exception ex)
            {
                _LOG.Info("ImportPublicKey: " + ex.Message);
                return Pkcs11Util.CAN_NOT_IMPORT_PUBLIC_KEY;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public static void RollbackState(Session session)
        {
            //Destroy private key object
            if(_privateKeyHandle != null)
            {
                _LOG.Info("RollbackState: Destroy private key with handle " + _privateKeyHandle.ObjectId);
                try
                {
                    session.DestroyObject(_privateKeyHandle);
                }
                catch(Pkcs11Exception ex)
                {
                    _LOG.Error("RollbackState: Destroy private key failed. " + ex.Message);
                }
                _privateKeyHandle = null;
            }

            //Destroy public key object
            if(_publicKeyHandle != null)
            {
                _LOG.Info("RollbackState: Destroy public key with handle " + _publicKeyHandle.ObjectId);
                try
                {
                    session.DestroyObject(_publicKeyHandle);
                }
                catch (Pkcs11Exception ex)
                {
                    _LOG.Error("RollbackState: Destroy public key failed. " + ex.Message);
                }
                _publicKeyHandle = null;
            }

            //Destroy user certificate object
            if (_certHandle != null)
            {
                _LOG.Info("RollbackState: Destroy user certificate with handle " + _certHandle.ObjectId);
                try
                {
                    session.DestroyObject(_certHandle);
                }
                catch (Pkcs11Exception ex)
                {
                    _LOG.Error("RollbackState: Destroy user certificate failed. " + ex.Message);
                }
                _certHandle = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x509Cert"></param>
        /// <returns></returns>
        public static byte[] GetPrivateKeyModulus(X509Certificate2 x509Cert)
        {
            if (x509Cert == null)
            {
                //Already checked
            }

            string xmlKey = x509Cert.PrivateKey.ToXmlString(true);
            byte[] result = GetValueByTagName(xmlKey, CKA_MODULUS);
            return result;
        }

    }
}
