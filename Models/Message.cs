using System;
using System.Collections.Generic;
using System.Text;

namespace Digital_Signature_Verification
{
    [Serializable]
    public class Message
    {
        public string Filename { get; set; }
        public string Sender_Username { get; set; }
        public string Receiver_Username { get; set; }
        public bool Is_Encrypted { get; set; } = true;
        public int Hash { get; set; }

        public Message(string Filename, string Sender_Username, string Receiver_Username, int Hash)
        {
                this.Filename = Filename;
                this.Sender_Username = Sender_Username;
                this.Receiver_Username = Receiver_Username;
                this.Hash = Hash;
        }
    }
}
