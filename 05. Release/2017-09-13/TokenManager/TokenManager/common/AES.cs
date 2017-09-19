using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.common
{

    /// <summary>
    /// 
    /// </summary>
    class AES
    {
        //Logger for this class
        private static readonly log4net.ILog _LOG =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] EncryptToByte(string plainText)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] keyArr = Convert.FromBase64String(TokenManagerConstants.AES_SECRET_KEY);

            // Initialization vector.   
            // No exception here
            byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
            byte[] IVBytes16Value = new byte[16];
            Array.Copy(ivArr, IVBytes16Value, 16);

            aes.Key = keyArr;
            aes.IV = IVBytes16Value;

            ICryptoTransform encryptor = aes.CreateEncryptor();

            try
            {
                byte[] plainTextBytes = Encoding.ASCII.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                return encryptedBytes;
            }
            catch (CryptographicException ex)
            {
                _LOG.Error("EncryptToByte: " + ex.Message);
                throw;
            }
            catch(ArgumentNullException ex)
            {
                _LOG.Error("encryptToByte: " + ex.Message);
                return null;
            }
            catch(EncoderFallbackException ex)
            {
                _LOG.Error("EncryptToByte: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string EncryptToBase64(string plainText)
        {
            try
            {
                byte[] encrypted = EncryptToByte(plainText);
                return Convert.ToBase64String(encrypted);
            }
            catch(CryptographicException e){
                _LOG.Error("EncryptToBase64: " + e.Message);
                throw;
            }
        }


        /// <summary>
        /// Encrypt byte array plain text to base64 string
        /// </summary>
        /// <param name="plainTextBytes"></param>
        /// <returns></returns>
        public static string EncryptToBase64(byte[] plainTextBytes)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] keyArr = Convert.FromBase64String(TokenManagerConstants.AES_SECRET_KEY);

            // Initialization vector.   
            // It could be any value or generated using a random number generator.
            byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
            byte[] IVBytes16Value = new byte[16];
            Array.Copy(ivArr, IVBytes16Value, 16);

            aes.Key = keyArr;
            aes.IV = IVBytes16Value;

            ICryptoTransform encryptor = aes.CreateEncryptor();

            try
            {
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (CryptographicException ex)
            {
                _LOG.Error("EncryptToBase64: " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Decrypt base64 string encrypted to plain string
        /// </summary>
        /// <param name="CipherTextBase64">Base64 string of Pkcs#12 data</param>
        /// <returns></returns>
        public static string DecryptToString(string CipherTextBase64)
        {
            try
            {
                byte[] decryptedData = DecryptToBytes(CipherTextBase64);
                return ASCIIEncoding.UTF8.GetString(decryptedData);
            }
            catch(CryptographicException e)
            {
                _LOG.Error("DecryptToString: " + e.Message);
                throw;
            }
        }


        /// <summary>
        /// Decrypt base64 encrypted string to byte array plain
        /// </summary>
        /// <param name="CipherTextBase64">Base64 string of Pkcs#12 data</param>
        /// <returns></returns>
        public static byte[] DecryptToBytes(string CipherTextBase64)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] keyArr = Convert.FromBase64String(TokenManagerConstants.AES_SECRET_KEY);

            // Initialization vector.   
            // It could be any value or generated using a random number generator.
            byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
            byte[] IVBytes16Value = new byte[16];
            Array.Copy(ivArr, IVBytes16Value, 16);

            aes.Key = keyArr;
            aes.IV = IVBytes16Value;

            ICryptoTransform decrypto = aes.CreateDecryptor();
            byte[] encryptedBytes = null;
            try
            {
                encryptedBytes = Convert.FromBase64CharArray(CipherTextBase64.ToCharArray(), 0, CipherTextBase64.Length);
            }
            catch (FormatException ex)
            {
                _LOG.Error("DecryptToBytes: " + ex.Message);
                throw ex;
            }
        
            try
            {
                byte[] decryptedData = decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return decryptedData;
            }
            catch(CryptographicException ex)
            {
                _LOG.Error("DecryptToBytes: " + ex.Message);
                throw ex;
            }
        }
    }
}
