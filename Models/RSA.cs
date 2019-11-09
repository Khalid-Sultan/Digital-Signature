using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Digital_Signature_Verification
{
    public class RSA
    {
        private bool _optimalAsymmetricEncryptionPadding = false;

        public string Encrypt(string text, string publicKeyXml, int keySize)
        {
            var encrypted = EncryptByteArray(Encoding.UTF8.GetBytes(text), publicKeyXml, keySize);
            return Convert.ToBase64String(encrypted);
        }
        public string Decrypt(string text, string publicAndPrivateKeyXml, int keySize)
        {
            var decrypted = DecryptByteArray(Convert.FromBase64String(text), publicAndPrivateKeyXml, keySize);
            return Encoding.UTF8.GetString(decrypted);
        }

        private byte[] EncryptByteArray(byte[] data, string publicKeyXml, int keySize)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data are empty", "data");
            }
            int maxLength = GetMaxDataLength(keySize);
            if (data.Length > maxLength)
            {
                throw new ArgumentException(String.Format("Maximum data length is {0}", maxLength), "data");
            }
            if (!IsKeySizeValid(keySize))
            {
                throw new ArgumentException("Key size is not valid", "keySize");
            }
            if (String.IsNullOrEmpty(publicKeyXml))
            {
                throw new ArgumentException("Key is null or empty", "publicKeyXml");
            }
            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicKeyXml);
                return provider.Encrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }
        private byte[] DecryptByteArray(byte[] data, string publicAndPrivateKeyXml, int keySize)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data are empty", "data");
            }
            if (!IsKeySizeValid(keySize))
            {
                throw new ArgumentException("Key size is not valid", "keySize");
            }
            if (String.IsNullOrEmpty(publicAndPrivateKeyXml))
            {
                throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");
            }
            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicAndPrivateKeyXml);
                return provider.Decrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        public int GetMaxDataLength(int keySize)
        {
            if (_optimalAsymmetricEncryptionPadding)
            {
                return ((keySize - 384) / 8) + 7;
            }
            return ((keySize - 384) / 8) + 37;
        }

        public bool IsKeySizeValid(int keySize)
        {
            return keySize >= 384 && keySize <= 16384 && keySize % 8 == 0;
        }


    }

}

