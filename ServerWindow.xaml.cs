using System;
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
        ChatServer cs;
        public ServerWindow()
        {
            InitializeComponent();

            lbActiveClients.SelectionChanged += (_s, _e) =>
            {
                if (lbActiveClients.SelectedValue == null)
                    return;
                if (lbActiveClients.SelectedValue is Client)
                    tbTargetUsername.Text = (lbActiveClients.SelectedValue as Client).Username;
            };


            this.DataContext = cs = new ChatServer();
        }

        private void bSwitchServerState_Click(object sender, RoutedEventArgs e)
        {
            cs.SwitchServerState();
        }

        private void bSend_Click(object sender, RoutedEventArgs e)
        {
            cs.SendMessage(tbTargetUsername.Text, tbMessage.Text);
        }
    }
}
