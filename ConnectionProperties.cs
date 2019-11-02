using System;
using System.Collections.Generic;
using System.Text;

namespace Digital_Signature_Verification
{
    class ConnectionProperties
    {
        public string ip_address { get; set; }
        public int port_number { get; set; }

        public ConnectionProperties(int port_number)
        {
            this.port_number = port_number;
        }
        public ConnectionProperties(string ip_address, int port_number)
        {
            this.ip_address = ip_address;
            this.port_number = port_number;
        }
    }
}
