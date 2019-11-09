using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Digital_Signature_Verification
{
    public class RSA
    {
        public string Sender_Username { get; set; }
        public string Receiver_Username { get; set; }
        public string Public_Key { get; set; }
        private string Private_Key { get; set; }
        private SHA512 SHA_Hasher { get; set; }

        private RSACryptoServiceProvider provider { get; set; }

        public RSA(string Sender_Username, string Receiver_Username)
        {
            this.Sender_Username = Sender_Username;
            this.Receiver_Username = Receiver_Username;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            this.Public_Key = rsa.ToXmlString(false);
            this.Private_Key = rsa.ToXmlString(true);
            this.provider = rsa;

            SHA512 SHA_Hasher = SHA512.Create();
            this.SHA_Hasher = SHA_Hasher;
        }

        public byte[] EncryptText(string text)
        {
            byte[] dataToEncrypt = StringHelper.convertToByteArray(text);

            SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
            ICryptoTransform ct = symmetricAlgorithm.CreateEncryptor();
            byte[] encryptedData = ct.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            RSAPKCS1KeyExchangeFormatter formatter = new RSAPKCS1KeyExchangeFormatter(provider);
            byte[] keyExchange = formatter.CreateKeyExchange(symmetricAlgorithm.Key);
            byte[] result = new byte[keyExchange.Length + symmetricAlgorithm.IV.Length + encryptedData.Length];
            Buffer.BlockCopy(keyExchange, 0, result, 0, keyExchange.Length);
            Buffer.BlockCopy(symmetricAlgorithm.IV, 0, result, keyExchange.Length, symmetricAlgorithm.IV.Length);
            Buffer.BlockCopy(encryptedData, 0, result, keyExchange.Length+symmetricAlgorithm.IV.Length, encryptedData.Length);
            return result;
        }
        public byte[] DecryptData(string text)
        {
            byte[] dataToDecrypt = StringHelper.convertToByteArray(text);

            SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
            byte[] keyExchange = new byte[provider.KeySize >> 3];
            Buffer.BlockCopy(dataToDecrypt, 0, keyExchange, 0, keyExchange.Length);

            RSAPKCS1KeyExchangeDeformatter def = new RSAPKCS1KeyExchangeDeformatter(provider);
            byte[] key = def.DecryptKeyExchange(keyExchange);

            byte[] iv = new byte[symmetricAlgorithm.IV.Length];
            Buffer.BlockCopy(dataToDecrypt, keyExchange.Length, iv, 0, iv.Length);

            ICryptoTransform ct = symmetricAlgorithm.CreateDecryptor(key, iv);
            byte[] decrypt = ct.TransformFinalBlock(dataToDecrypt, keyExchange.Length + iv.Length, dataToDecrypt.Length - (keyExchange.Length + iv.Length));
            return decrypt;
        }

        public string GetHash(byte[] dataToHash)
        { 
            byte[] data = SHA_Hasher.ComputeHash(dataToHash);
            StringBuilder builder = new StringBuilder();
            for(int i=0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString());
            }
            string hash = builder.ToString();
            return hash;
        }
        public bool VerifyHash(string text, string hash)
        {
            byte[] dataToHash = StringHelper.convertToByteArray(text);

            byte[] HashOfInput = SHA_Hasher.ComputeHash(dataToHash);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < HashOfInput.Length; i++)
            {
                builder.Append(HashOfInput[i].ToString());
            }
            string cleanedHash = builder.ToString();
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if(comparer.Compare(cleanedHash, hash)==0)
            {
                return true;
            }
            return false;
        }

    }
}
