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

        private RSACryptoServiceProvider provider { get; set; }

        public RSA(string Sender_Username, string Receiver_Username)
        {
            this.Sender_Username = Sender_Username;
            this.Receiver_Username = Receiver_Username;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            this.Public_Key = rsa.ToXmlString(false);
            this.Private_Key = rsa.ToXmlString(true);
            this.provider = rsa;
        }

        public byte[] EncryptText(string text)
        {
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = byteConverter.GetBytes(text);
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
        public string DecryptData(string filename)
        {
            byte[] dataToDecrypt = File.ReadAllBytes(filename);
            byte[] decryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(Private_Key);
                decryptedData = rsa.Decrypt(dataToDecrypt, false);
            } 
            string decryptedFileName = $"DECRYPTED - {new Random(Seed: 151).Next(500000)}.txt";
            return decryptedFileName;
        }
        public string GetHash(string text)
        {
            SHA512 SHA_Hasher = SHA512.Create();            
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            byte[] dataToHash = byteConverter.GetBytes(text);
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
            string HashOfInput = GetHash(text);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if(0==comparer.Compare(HashOfInput, hash))
            {
                return true;
            }
            return false;
        }

    }
}
