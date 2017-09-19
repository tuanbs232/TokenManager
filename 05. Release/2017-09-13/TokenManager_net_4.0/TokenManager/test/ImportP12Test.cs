using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TokenManager.test
{
    class ImportP12Test
    {
        //Logger for this class
        private static readonly log4net.ILog _LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static byte[] GetValueByTagName(string data, string tag)
        {
            if (String.IsNullOrEmpty(data) || String.IsNullOrEmpty(tag))
            {
                return null;
            }

            XmlDocument xmlDocument = new XmlDocument();
            XmlNodeList nodeList;
            string value = null;
            try
            {
                xmlDocument.LoadXml(data);
                nodeList = xmlDocument.GetElementsByTagName(tag);
                if (nodeList.Count == 1)
                    value = ((XmlNode)nodeList.Item(0)).InnerXml;
            }
            catch (Exception e)
            {
                _LOG.Error("GetValueByTagName: " + e.Message);
            }
            return Convert.FromBase64String(value);
        }

        public static int ImportCertificate(Session session, X509Certificate2 x509Cert, String label, byte[] ckaId)
        {
            List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODIFIABLE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE, false));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_TRUSTED, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SERIAL_NUMBER, x509Cert.GetSerialNumber()));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, label));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SUBJECT, x509Cert.SubjectName.RawData));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ISSUER, x509Cert.IssuerName.RawData));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SERIAL_NUMBER, x509Cert.SerialNumber));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_VALUE, x509Cert.RawData));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ID, ckaId));
            try
            {
                ObjectHandle objectHandle = session.CreateObject(objectAttributes);
                _LOG.Info("User certificate was imported with handle " + objectHandle.ObjectId);
                return 1;
            }
            catch (Pkcs11Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static int ImportPrivateKey(Session session, X509Certificate2 x509Cert, String label, byte[] ckaId)
        {
            string xmlKey = x509Cert.PrivateKey.ToXmlString(true);

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
            
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODULUS, GetValueByTagName(xmlKey, "Modulus")));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, GetValueByTagName(xmlKey, "Exponent")));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE_EXPONENT, GetValueByTagName(xmlKey, "D")));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_1, GetValueByTagName(xmlKey, "P")));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_2, GetValueByTagName(xmlKey, "Q")));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_1, GetValueByTagName(xmlKey, "DP")));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_2, GetValueByTagName(xmlKey, "DQ")));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_COEFFICIENT, GetValueByTagName(xmlKey, "InverseQ")));
            
            try
            {
                ObjectHandle objectHandle = session.CreateObject(objectAttributes);
                _LOG.Info("Private key imported with handle " + objectHandle.ObjectId);
                return 1;
            }
            catch (Pkcs11Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static int ImportPublicKey(Session session, X509Certificate2 x509Cert, String label, byte[] ckaId)
        {
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
                ObjectHandle objectHandle = session.CreateObject(objectAttributes);
                _LOG.Info("Public key imported with handle " + objectHandle.ObjectId);
                return 1;
            }
            catch (Pkcs11Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
