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
using Microsoft.Win32;

namespace Digital_Signature_Verification
{
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        ChatServer cs;
        private string filepath = null;
        public ServerWindow()
        {
            InitializeComponent();

            lbActiveClients.SelectionChanged += (_s, _e) =>
            {
                if (lbActiveClients.SelectedValue == null)
                    return;
                if (lbActiveClients.SelectedValue is Client)
                {
                    tbTargetUsername.Text = (lbActiveClients.SelectedValue as Client).Username;
                    foreach(KeyTracker keyTracker in KeysManifestController.KeysManifest)
                    {
                        if ((keyTracker.receiver_id == tbTargetUsername.Text ||
                            keyTracker.sender_id == tbTargetUsername.Text) &&
                            (keyTracker.receiver_id == cs.Username ||
                            keyTracker.sender_id     == cs.Username)
                        )
                        {
                            KeyStatus.Text = $"Keys Exchanged :)";
                            bExchange.IsEnabled = false;
                            bSend.IsEnabled = true;
                            break;
                        }
                    }
                    KeyStatus.Text = $"No Keys Exchanged Yet :(";
                    bExchange.IsEnabled = true;
                    bSend.IsEnabled = false;
                }

            };


            this.DataContext = cs = new ChatServer();
        }

        private void bSwitchServerState_Click(object sender, RoutedEventArgs e)
        {
            cs.SwitchServerState();
        }

        private void bSend_Click(object sender, RoutedEventArgs e)
        {
            cs.SendMessage(tbTargetUsername.Text, filepath);
        }

        private void bExchange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cs.ExchangeKeys(tbTargetUsername.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".txt";
            Nullable<bool> openedFile = openFile.ShowDialog();
            if (openedFile.HasValue)
            {
                filepath = openFile.FileName;
                tbMessage.Content = "Browse: Selected " + openFile.SafeFileName;
            }
        }
    }
}
