using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Digital_Signature_Verification
{
    class KeyTracker
    {
        public string receiver_id { get; set; }
        public RSAParameters public_key { get; set; }
        private RSAParameters private_key { get; set; }

        public KeyTracker()
        {
            var csp = new RSACryptoServiceProvider(2048);
            this.public_key = csp.ExportParameters(true);
            this.private_key = csp.ExportParameters(false);
        }

        public string ConvertBytesToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        public byte[] ConvertStringToBytes(string text)
        {
            return Convert.FromBase64String(text);
        }
        public string ConvertKeyToString(RSAParameters key)
        {
            var sw = new System.IO.StringWriter();
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, key);
            return sw.ToString();
        }
        public RSAParameters ConvertStringToKey(string string_key)
        {
            var sr = new System.IO.StringReader(string_key);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            var key = (RSAParameters)xs.Deserialize(sr);
            return key;
        }
        public byte[] EncryptBytes(RSAParameters public_key, byte[] bytes)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(public_key);
            var encrypted_bytes = rsa.Encrypt(bytes, false);
            return encrypted_bytes;
        }

        public byte[] DecryptBytes(RSAParameters private_key, byte[] bytes)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(private_key);
            var decrypted_bytes = rsa.Decrypt(bytes, false);
            return decrypted_bytes;
        }
    }
}
