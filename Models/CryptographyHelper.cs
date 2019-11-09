using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Digital_Signature_Verification
{
    class CryptographyHelper
    {
        /**
            Encryption:

                i. Generate a random key of the length required for symmetrical encryption technique such as AES/Rijndael or similar.

                ii. Encrypt your data using AES/Rijndael using that random key generated in part i.

                iii. Use RSA encryption to asymmetrically encrypt the random key generated in part i.

            Publish (eg write to a file) the outputs from parts ii. and iii.: the AES-encrypted data and the RSA-encrypted random key.

            Decryption:

                i. Decrypt the AES random key using your private RSA key.

                ii. Decrypt the original data using the decrypted AES random key

         * 
         */

        private string private_key_rsa_encryption { get; set; }
        private string public_key_aes_encryption { get; set; }

        private AES AES { get; set; }
        private RSA RSA { get; set; }

        private string encrypted_content { get; set; }
        private string decrypted_content { get; set; }

        private string rsa_public_xml { get; set; }
        private string rsa_private_xml { get; set; }

        private SHA512 SHA_Hasher { get; set; }

        public string Receiver_Username { get; set; }
        public string Sender_Username { get; set; }


        public CryptographyHelper(string Sender_Username, string Receiver_Username)
        {
            this.AES = new AES();
            this.RSA = new RSA();
            this.Sender_Username = Sender_Username;
            this.Receiver_Username = Receiver_Username;

            RSACryptoServiceProvider rSACryptoService = new RSACryptoServiceProvider(2048);
            this.rsa_public_xml = rSACryptoService.ToXmlString(false);
            this.rsa_private_xml = rSACryptoService.ToXmlString(true);
        }

        public string EncryptContent(string text)
        {
            string encryptedContent = this.AES.Encrypt(text);
            this.private_key_rsa_encryption = this.AES.GetRandomKeyText();

            RSACryptoServiceProvider rSACryptoService = new RSACryptoServiceProvider(2048);
            rSACryptoService.FromXmlString(rsa_private_xml);
            this.public_key_aes_encryption = this.AES.GetString(rSACryptoService.Encrypt(this.AES.GetBytes(this.private_key_rsa_encryption),true));

            SHA512 SHA_Hasher = SHA512.Create();
            this.SHA_Hasher = SHA_Hasher;

            return encryptedContent;
        }
        public string DecryptContent(string text)
        {
            RSACryptoServiceProvider rSACryptoService = new RSACryptoServiceProvider(2048);
            rSACryptoService.FromXmlString(rsa_private_xml);
            string randomKeyText = this.AES.GetString(rSACryptoService.Decrypt(this.AES.GetBytes(this.public_key_aes_encryption), true));
            string decryptedContent = this.AES.Decrypt(text, randomKeyText);
            return decryptedContent;
        }
        public string GetHash(string text)
        {
            byte[] dataToDecrypt = this.AES.GetBytes(text);
            byte[] data = SHA_Hasher.ComputeHash(dataToDecrypt);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString());
            }
            string hash = builder.ToString();
            return hash;
        }
        public bool VerifyHash(string text, string hash)
        {
            byte[] dataToHash = this.AES.GetBytes(EncryptContent(text));
            byte[] HashOfInput = SHA_Hasher.ComputeHash(dataToHash);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < HashOfInput.Length; i++)
            {
                builder.Append(HashOfInput[i].ToString());
            }
            string cleanedHash = builder.ToString();
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (comparer.Compare(cleanedHash, hash) == 0)
            {
                return true;
            }
            return false;
        }

    }
}
