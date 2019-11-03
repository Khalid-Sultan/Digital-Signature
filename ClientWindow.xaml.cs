using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        Socket client;

        public ClientWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int port_number_server = 0;
            if (!int.TryParse(Port_Number_Client.Text.ToString(), out port_number_server))
            {
                MessageBox.Show("Invalid IP Address Given");
                return;
            }
            ConnectionProperties connectionProperties = new ConnectionProperties(port_number: port_number_server);
            ConnectToServer(connectionProperties);
        }
        private void ConnectToServer(ConnectionProperties connectionProperties)
        {
            //Establish Local EndPoint for the socket
            IPHostEntry iPHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = iPHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, connectionProperties.port_number);

            //Create Socket using Sockets
            client = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //Connect Socket to the remote end-point
                client.Connect(localEndPoint);
                Client_Status.Content = "Connected To Server";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                //Create Message to be Sent
                byte[] sentFile = Encoding.ASCII.GetBytes("Client Sent Message");
                int byteSent = client.Send(sentFile);

                //Data Buffer
                byte[] messageReceived = new byte[1024];

                //Receive message using Receive method
                int received_bytes = client.Receive(messageReceived);
                MessageBox.Show("Received Message from Server");

                Label label = new Label();
                label.Content = "Received Message from Server";
                Received_Files.Children.Add(label);  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Client_Start.IsEnabled = true;
            Client_Stop.IsEnabled = false;
            MessageBox.Show("Server has been Shut Down"); 
            Client_Status.Content = "Not Started";
            //Close Socket using Close method
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}
