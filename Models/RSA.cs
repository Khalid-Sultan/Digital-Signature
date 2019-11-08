using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Digital_Signature_Verification.Models
{
    class RSA
    {
        public string Sender_Username { get; set; }
        public string Receiver_Username { get; set; }
        public string Public_Key { get; set; }
        private string Private_Key { get; set; }

        RSA(string Sender_Username, string Receiver_Username)
        {
            this.Sender_Username = Sender_Username;
            this.Receiver_Username = Receiver_Username;
            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            this.Public_Key = rsa.ToXmlString(false);
            this.Private_Key = rsa.ToXmlString(true);
        }

        string EncryptText(string text)
        {
            UnicodeEncoding byteConverter = new UnicodeEncoding(); 
            byte[] dataToEncrypt = byteConverter.GetBytes(text);
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(Public_Key);
                encryptedData = rsa.Encrypt(dataToEncrypt, false);
            }
            string fileName = $"ENCRYPTED - {new Random(Seed:151).Next(500000)}.txt";
            File.WriteAllBytes(fileName, encryptedData);
            return fileName;
        }

        string DecryptData(string filename)
        {
            byte[] dataToDecrypt = File.ReadAllBytes(filename);
            byte[] decryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(Private_Key);
                decryptedData = rsa.Decrypt(dataToDecrypt, false);
            }
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            string fileName = $"DECRYPTED - {new Random(Seed: 151).Next(500000)}.txt";
            return byteConverter.GetString(decryptedData);
        }
    }
}
