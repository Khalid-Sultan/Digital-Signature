using System;
using System.Collections.Generic;
using System.Text;

namespace Digital_Signature_Verification
{
    [Serializable]
    public class Message
    {
        public Category Category { get; set; }

        public string content{ get; set; }
        public string Sender_Username { get; set; }
        public string Receiver_Username { get; set; }

        public bool Is_Encrypted { get; set; } = true;

        public string File_Name { get; set; }
        public string Hash { get; set; }

        public Message(string content, string Sender_Username, string Receiver_Username, string Hash, Category Category, string File_Name)
        {
            this.content = content;
            this.Sender_Username = Sender_Username;
            this.Receiver_Username = Receiver_Username;
            this.Hash = Hash;
            this.Category = Category;
            this.File_Name = File_Name;
        }
    }
    public enum Category
    {
        Message,
        Naming
    }
}
