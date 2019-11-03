﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Digital_Signature_Verification
{
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        Socket listener;
        public ServerWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int port_number_server = 0;
            if (!int.TryParse(Port_Number_Server.Text.ToString(), out port_number_server))
            {
                MessageBox.Show("Invalid IP Address Given");
                return;
            }
            ConnectionProperties connectionProperties = new ConnectionProperties(port_number: port_number_server);
            StartServer(connectionProperties);
        }

        private void StartServer(ConnectionProperties connectionProperties)
        {
            //Establish Local EndPoint for the socket
            IPHostEntry iPHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = iPHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, connectionProperties.port_number);

            //Create TCP/IP Socket using Sockets
            listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //Using Binding method, associate the network address to the socket
                listener.Bind(localEndPoint);

                //Using Listening method, set the maximum list of clients to accept
                listener.Listen(5);

                Server_Start.IsEnabled = false;
                Server_Stop.IsEnabled = true;

                ListenServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private async void ListenServer()
        {
            while (true)
            {
                Server_Status.Content = "Waiting For Connection";
                Socket client = await listener.AcceptAsync();
                Connected_Clients.Content = "Some clients are Connected";

                byte[] bytes = new byte[1024];
                string data = null;

                while (true)
                {
                    int numByte = client.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, numByte);
                    if (data.IndexOf("<EOF>") > -1) break;
                }
                MessageBox.Show("File Received");
                Label label = new Label();
                label.Content = "Received Message from Client";
                Received_Files.Children.Add(label);

                byte[] message = Encoding.ASCII.GetBytes("File Received By Server");

                //Send an acknowledgment message back to the client
                client.Send(message);
            }
        }

        private void Server_Stop_Click(object sender, RoutedEventArgs e)
        {
            Server_Start.IsEnabled = true;
            Server_Stop.IsEnabled = false;
            MessageBox.Show("Server has been Shut Down");
            Connected_Clients.Content = "No clients are Connected";
            Server_Status.Content = "Not Started";
            listener.Close();
        }
    }
}
