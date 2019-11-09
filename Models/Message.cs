using System;
using System.Collections.Generic;
using System.Text;

namespace Digital_Signature_Verification
{
    [Serializable]
    public class Message
    {
        public byte[] Bytes { get; set; }
        public string Sender_Username { get; set; }
        public string Receiver_Username { get; set; }

        public bool Is_Encrypted { get; set; } = true;

        public string Hash { get; set; }

        public Message(byte[] Bytes, string Sender_Username, string Receiver_Username, string Hash)
        {
            this.Bytes = Bytes;
            this.Sender_Username = Sender_Username;
            this.Receiver_Username = Receiver_Username;
            this.Hash = Hash;
        }
    }
}
