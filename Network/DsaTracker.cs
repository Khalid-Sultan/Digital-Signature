using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Digital_Signature_Verification
{
    class DsaTracker
    {

        public string sender_id { get; set; }
        public string receiver_id { get; set; }
        public DSAParameters public_key { get; set; }
        private DSAParameters private_key { get; set; }

        public DsaTracker()
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
            this.public_key = dsa.ExportParameters(true);
            this.private_key = dsa.ExportParameters(false);
        }

        public string ConvertBytesToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        public byte[] ConvertStringToBytes(string text)
        {
            return Convert.FromBase64String(text);
        }
        public string ConvertKeyToString(DSAParameters key)
        {
            var sw = new System.IO.StringWriter();
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(DSAParameters));
            xs.Serialize(sw, key);
            return sw.ToString();
        }
        public DSAParameters ConvertStringToKey(string string_key)
        {
            var sr = new System.IO.StringReader(string_key);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(DSAParameters));
            var key = (DSAParameters)xs.Deserialize(sr);
            return key;
        }
        public byte[] SignData(byte[] bytes)
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
            dsa.ImportParameters(this.public_key);
            var encrypted_bytes = dsa.SignData(bytes);
            return encrypted_bytes;
        }
        public bool VerifyData(byte[] bytes, byte[] signature)
        {
            DSACryptoServiceProvider verifier = new DSACryptoServiceProvider();
            verifier.ImportParameters(this.private_key);
            return verifier.VerifyData(bytes, signature);
        }
        public void a(string path)
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(file);
            byte[] data = reader.ReadBytes((int)file.Length);

            byte[] signature = dsa.SignData(data);
            string public_key = dsa.ToXmlString(false);
            reader.Close();
            file.Close();

            DSACryptoServiceProvider verifier = new DSACryptoServiceProvider();
            verifier.FromXmlString(public_key);
            FileStream file2 = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader reader2 = new BinaryReader(file2);
            byte[] data2 = reader2.ReadBytes((int)file2.Length);
             

            reader2.Close();
            file2.Close();


        }



    }
}
