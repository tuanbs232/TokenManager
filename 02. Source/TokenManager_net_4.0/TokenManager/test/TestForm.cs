using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using TokenManager.common;
using TokenManager.dialog;
using System.Runtime.InteropServices;
using Org.BouncyCastle.Math;
using TokenManager.test;
using System.Threading;

namespace TokenManager
{
    public partial class TestForm : Form
    {
        [DllImport("CspInteractive.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int LoadAllCertToStore(StringBuilder name);

        [DllImport("CspInteractive.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int UnloadAllCertificate(StringBuilder provider);

        public TestForm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == UsbNotification.WmDevicechange)
            {
                switch ((int)m.WParam)
                {
                    case UsbNotification.DbtDeviceremovecomplete:
                        Usb_DeviceRemoved(); // this is where you do your magic
                        break;
                    case UsbNotification.DbtDevicearrival:
                        Usb_DeviceAdded(); // this is where you do your magic
                        break;
                }
            }
        }

        private void Usb_DeviceRemoved()
        {
            checkToken();
            Console.WriteLine("Token unpluged");
        }

        private void Usb_DeviceAdded()
        {
            checkToken();
            Console.WriteLine("Token pluged in");
            SystemUtil.ShowNotify("token pluged in");
        }

        private void checkToken()
        {
            const string ModulePath = @"S:\WORK\2016\03-2016\BkavCA_Token_Config\BkavCA.dll";
            Pkcs11 pkcs11 = new Pkcs11(ModulePath, AppType.MultiThreaded);
            List<Slot> slots = pkcs11.GetSlotList(SlotsType.WithTokenPresent);
            Console.WriteLine(slots.Count);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<ObjectAttribute> attr = DetectPrivateKey();
            Session session = Pkcs11Connector.GetInstance().OpenReadWriteSession("123123qwe");


            List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_RSA));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));
            //objectAttributes.Add(new ObjectAttribute(CKA.CKA_HW_FEATURE_TYPE, CKO.CKO_HW_FEATURE));
            //objectAttributes.Add(new ObjectAttribute(CKA.CKA_APPLICATION, Encoding.ASCII.GetBytes("VNPT Token Manager")));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, "TuanBs"));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ID, session.GenerateRandom(20)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SENSITIVE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_DECRYPT, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SIGN, true));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODULUS, attr[2].GetValueAsByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, attr[3].GetValueAsByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE_EXPONENT, attr[4].GetValueAsByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_1, attr[5].GetValueAsByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_2, attr[6].GetValueAsByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_1, attr[7].GetValueAsByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_2, attr[8].GetValueAsByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_COEFFICIENT, attr[9].GetValueAsByteArray()));

            try
            {
                ObjectHandle objectHandle = session.CreateObject(objectAttributes);
                Console.WriteLine(objectHandle.ObjectId);
                MessageBox.Show("Import successfull");
            }
            catch (Pkcs11Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Cannot import private key");
            }
            session.Logout();
        }

        private void findPrivateKey()
        {
            Session session = Pkcs11Connector.GetInstance().OpenReadWriteSession("123123qwe");
            const string CertFile = @"C:\Users\TUANBS\Desktop\TuanBS.cer";

            System.Security.Cryptography.X509Certificates.X509Certificate2 X509Cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(CertFile);

            X509CertificateParser x509CertificateParser = new X509CertificateParser();

            X509Certificate cert = x509CertificateParser.ReadCertificate(File.ReadAllBytes(CertFile));
            AsymmetricKeyParameter pubKeyParams = cert.GetPublicKey();
            
            if (!(pubKeyParams is RsaKeyParameters))
            {
                MessageBox.Show("Not supported key type");
                return;
            }
            RsaKeyParameters rsaPubKeyParams = (RsaKeyParameters)pubKeyParams;
            Console.WriteLine(new BigInteger(rsaPubKeyParams.Modulus.ToByteArray()).ToString(16));

            // Find corresponding private key
            List<ObjectAttribute> privKeySearchTemplate = new List<ObjectAttribute>();
            privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY));
            privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_RSA));
            //privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_MODULUS, rsaPubKeyParams.Modulus.ToByteArray()));
            //privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));
            //privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, rsaPubKeyParams.Exponent.ToByteArray()));

            List<ObjectHandle> foundObjects = session.FindAllObjects(privKeySearchTemplate);
            if (foundObjects.Count < 1)
            {
                MessageBox.Show("No private key found");
                return;
            }

            foreach(ObjectHandle obj in foundObjects)
            {
                ObjectHandle privKeyObjectHandle = obj;

                // Read CKA_LABEL and CKA_ID attributes of private key
                List<CKA> privKeyAttrsToRead = new List<CKA>();
                privKeyAttrsToRead.Add(CKA.CKA_LABEL);
                privKeyAttrsToRead.Add(CKA.CKA_ID);
                privKeyAttrsToRead.Add(CKA.CKA_MODULUS);

                List<ObjectAttribute> privKeyAttributes = session.GetAttributeValue(privKeyObjectHandle, privKeyAttrsToRead);
                Console.WriteLine(new BigInteger(privKeyAttributes[2].GetValueAsByteArray()).ToString(16));
            }
        }

        private RsaPrivateCrtKeyParameters readPrivateKey()
        {
            const string P12Base64 = "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";
            const string KeystorePass = @"d3jpWqicAsRZ";

            byte[] KeystoreBytes = Convert.FromBase64String(P12Base64);
            Pkcs12Store store = new Pkcs12Store(new MemoryStream(KeystoreBytes), KeystorePass.ToCharArray());
            foreach (string n in store.Aliases)
            {
                if (store.IsKeyEntry(n))
                {
                    AsymmetricKeyEntry key = store.GetKey(n);

                    if (key.Key.IsPrivate)
                    {
                        RsaPrivateCrtKeyParameters parameters = key.Key as RsaPrivateCrtKeyParameters;
                        return parameters;
                    }
                    break;
                }
            }
            return null;
        }

        private void ImportCertficiate()
        {
            Session session = Pkcs11Connector.GetInstance().OpenReadWriteSession("123123qwe");
            const string CertFile = @"C:\Users\TUANBS\Desktop\TuanBS.cer";

            System.Security.Cryptography.X509Certificates.X509Certificate2 X509Cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(CertFile);

            X509CertificateParser x509CertificateParser = new X509CertificateParser();

            X509Certificate cert = x509CertificateParser.ReadCertificate(File.ReadAllBytes(CertFile));
            int result = Pkcs11Util.ImportCertficate(session, cert, "TuanBS", session.GenerateRandom(20));
            Console.WriteLine(result);
        }


        private void ImportPrivateKey(RsaPrivateCrtKeyParameters parameters)
        {
            Pkcs12Store store = new Pkcs12Store();
            Session session = Pkcs11Connector.GetInstance().OpenReadWriteSession("123123qwe");
            const string CertFile = @"C:\Users\TUANBS\Desktop\TuanBS.cer";

            System.Security.Cryptography.X509Certificates.X509Certificate2 X509Cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(CertFile);

            X509CertificateParser x509CertificateParser = new X509CertificateParser();

            X509Certificate cert = x509CertificateParser.ReadCertificate(File.ReadAllBytes(CertFile));
            string subject = cert.SubjectDN.ToString();

            //session.Login(CKU.CKU_SO, "BkavCAToken");
            //session.Login(CKU.CKU_USER, "12345678");

            List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_RSA));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));
            //objectAttributes.Add(new ObjectAttribute(CKA.CKA_HW_FEATURE_TYPE, CKO.CKO_HW_FEATURE));
            //objectAttributes.Add(new ObjectAttribute(CKA.CKA_APPLICATION, Encoding.ASCII.GetBytes("VNPT Token Manager")));
            
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, X509Cert.GetNameInfo(System.Security.Cryptography.X509Certificates.X509NameType.DnsName, false)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SUBJECT, cert.SubjectDN.GetDerEncoded()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_ID, session.GenerateRandom(20)));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SENSITIVE, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_DECRYPT, true));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_SIGN, true));

            objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODULUS, parameters.Modulus.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, parameters.PublicExponent.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE_EXPONENT, parameters.Exponent.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_1, parameters.P.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_2, parameters.Q.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_1, parameters.DP.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_2, parameters.DQ.ToByteArray()));
            objectAttributes.Add(new ObjectAttribute(CKA.CKA_COEFFICIENT, parameters.QInv.ToByteArray()));

            try
            {
                ObjectHandle objectHandle = session.CreateObject(objectAttributes);
                Console.WriteLine(objectHandle.ObjectId);
                MessageBox.Show("Import successfull");
            }
            catch(Pkcs11Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Cannot import private key");
            }
            session.Logout();
        }

        private List<ObjectAttribute> DetectPrivateKey()
        {
            Pkcs11Connector connector = Pkcs11Connector.GetInstance();
            Session session = connector.OpenReadWriteSession("123123qwe");

            //RsaPrivateCrtKeyParameters parameters = readPrivateKey();

            // Find corresponding private key
            List<ObjectAttribute> privKeySearchTemplate = new List<ObjectAttribute>();
            privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY));
            privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_RSA));
            //privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_MODULUS, parameters.Modulus.ToByteArrayUnsigned()));
            //privKeySearchTemplate.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, parameters.Exponent.ToByteArrayUnsigned()));

            List<ObjectHandle> foundObjects = session.FindAllObjects(privKeySearchTemplate);
            if (foundObjects.Count < 1)
            {
                MessageBox.Show("No private found");
                return null;
            }


            ObjectHandle privKeyObjectHandle = foundObjects[0];
            
            // Read CKA_LABEL and CKA_ID attributes of private key
            List<CKA> privKeyAttrsToRead = new List<CKA>();
            privKeyAttrsToRead.Add(CKA.CKA_LABEL);
            privKeyAttrsToRead.Add(CKA.CKA_ID);
            privKeyAttrsToRead.Add(CKA.CKA_MODULUS);
            privKeyAttrsToRead.Add(CKA.CKA_PUBLIC_EXPONENT);
            privKeyAttrsToRead.Add(CKA.CKA_PRIVATE_EXPONENT);
            privKeyAttrsToRead.Add(CKA.CKA_PRIME_1);
            privKeyAttrsToRead.Add(CKA.CKA_PRIME_2);
            privKeyAttrsToRead.Add(CKA.CKA_EXPONENT_1);
            privKeyAttrsToRead.Add(CKA.CKA_EXPONENT_2);
            privKeyAttrsToRead.Add(CKA.CKA_COEFFICIENT);



            List<ObjectAttribute> privKeyAttributes = session.GetAttributeValue(privKeyObjectHandle, privKeyAttrsToRead);

            Console.WriteLine(BitConverter.ToString(SHA1.Create().ComputeHash(privKeyAttributes[2].GetValueAsByteArray())));

            session.Logout();
            return privKeyAttributes;
        }

        private void importP12(object sender, EventArgs e)
        {
            string p12Path = "";
            OpenFileDialog fileDig = new OpenFileDialog();
            fileDig.Filter = "Personal Exchange (*.p12)|*.p12";
            if(fileDig.ShowDialog() == DialogResult.OK)
            {
                p12Path = fileDig.FileName;
            }

            if (!File.Exists(p12Path))
            {
                MessageBox.Show("File not found");
                return;
            }

            PasswordInput pass = new PasswordInput();
            if(pass.ShowDialog(this, "Enter Pkcs#12 password") != DialogResult.OK)
            {
                return;
            }
            
            byte[] p12Data = File.ReadAllBytes(p12Path);
            string p12Base64 = AES.EncryptToBase64(p12Data);
            string password = pass.Password;
            string p12Pass = AES.EncryptToBase64(password);
            string userPin = "";
            if(pass.ShowDialog(this, "Enter Token user pin") == DialogResult.OK)
            {
                userPin = pass.Password;
                //Pkcs11Util.ImportP12Data(p12Base64, p12Pass, userPin);
            }
            
            Pkcs11Connector connector = Pkcs11Connector.GetInstance();
            Session session = connector.OpenReadWriteSession(userPin);
            if(session == null)
            {
                Console.WriteLine("Session null");
                return;
            }

            System.Security.Cryptography.X509Certificates.X509Certificate2 cert = null;
            try
            {
                cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(p12Data, password, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable);
            }
            catch(CryptographicException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            byte[] caId = session.GenerateRandom(20);
            ImportP12Test.ImportPrivateKey(session, cert, "TuanBS", caId);
            ImportP12Test.ImportPublicKey(session, cert, "TuanBS", caId);
            ImportP12Test.ImportCertificate(session, cert, "TuanBS", caId);

        }


        private void importKeyPair(System.Security.Cryptography.X509Certificates.X509Certificate2 cert, Session session)
        {
            AsymmetricAlgorithm keyPair = cert.PrivateKey;
            if(keyPair is RSA)
            {
                RSAParameters keyParams = ((RSA)keyPair).ExportParameters(true);
                List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_RSA));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_SUBJECT, cert.SubjectName.RawData));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_ID, "TuanBS_ID"));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, "TuanBS"));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));


                objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODULUS, keyParams.Modulus));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_PUBLIC_EXPONENT, keyParams.Exponent));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE_EXPONENT, keyParams.D));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIVATE, true));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_MODIFIABLE, true));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXTRACTABLE, false));
                /*
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_1, privateKey.P.ToByteArray()));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_PRIME_2, privateKey.Q.ToByteArray()));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_1, privateKey.DP.ToByteArray()));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_EXPONENT_2, privateKey.DQ.ToByteArray()));
                objectAttributes.Add(new ObjectAttribute(CKA.CKA_COEFFICIENT, privateKey.QInv.ToByteArray()));
                */
                try
                {
                    session.CreateObject(objectAttributes);
                    return;
                }
                catch (Pkcs11Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }

        private void importCertificate(System.Security.Cryptography.X509Certificates.X509Certificate2 cert, Session session)
        {
            List<ObjectAttribute> certificateAttributes = new List<ObjectAttribute>();
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_SUBJECT, cert.SubjectName.RawData));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_ISSUER, cert.Issuer));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_SERIAL_NUMBER, cert.SerialNumber));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_ID, "TuanBS_ID"));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_LABEL, "TuanBS"));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_TOKEN, true));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_VALUE, cert.RawData));
            certificateAttributes.Add(new ObjectAttribute(CKA.CKA_MODIFIABLE, true));
            try
            {
                session.CreateObject(certificateAttributes);
                return;
            }
            catch (Pkcs11Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PasswordInput pass = new PasswordInput();
            
            if (pass.ShowDialog(this, "Enter encrypted base64 string") != DialogResult.OK)
            {
                return;
            }
            //string Base64 = @"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";
            //string KeystorePass = @"mHHJIvil3hgNb9Fah7Ig9w==";
            string base64Encrypted = pass.Password;
            byte[] decryptedBytes = AES.DecryptToBytes(base64Encrypted);
            string base64 = Convert.ToBase64String(decryptedBytes);
            Console.WriteLine(base64);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ThreadStart child = new ThreadStart(updatetokenInfo);
            Thread parent = new Thread(child);
            parent.Start();
        }

        private void updatetokenInfo()
        {
            string updateCerList = Properties.Settings.Default.HAS_UPDATED_CERT_LIST;
            string newValue = updateCerList + "0410131413111210,";
            Properties.Settings.Default.HAS_UPDATED_CERT_LIST = newValue;
            Properties.Settings.Default.Save();
            CertificateRenew renew = new CertificateRenew();
            renew.UpdateCertList();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string Base64 = @"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";
            string KeystorePass = @"mHHJIvil3hgNb9Fah7Ig9w==";
            int code = Pkcs11Util.ImportP12Data(Base64, KeystorePass, "123456a@A");
            Console.WriteLine(Pkcs11Util.GetErrorMessage(code));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Pkcs11Connector.GetInstance().GetTokenSlot().CloseAllSessions();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CspUtil.LoadAllCertToStore("VNPT-CA-CTDT V6-Token CSP");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            CspUtil.UnloadAllCertificate("VNPT-CA Token CSP v6");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            LanguageUtil.SetLanguage(LanguageUtil.VI);
            Console.WriteLine(LanguageUtil.GetInstance().GetValue("TOOL_UNLOCK_DES"));
        }

        private void button10_Click(object sender, EventArgs e)
        {
            LanguageUtil.SetLanguage(LanguageUtil.EN);
            Console.WriteLine(LanguageUtil.GetInstance().GetValue("TOOL_UNLOCK_DES"));
        }
    }
}
