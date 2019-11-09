using Microsoft.Win32;
using System;
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
    /// Interaction logic for CoordinatorWindow.xaml
    /// </summary>
    public partial class CoordinatorWindow : Window
    {
        Coordinator Coordinator;
        private string SelectedFilePath = null;

        public CoordinatorWindow()
        {
            InitializeComponent();

            lbActiveClients.SelectionChanged += (_s, _e) =>
            {
                if (lbActiveClients.SelectedValue == null)
                    return;
                if (lbActiveClients.SelectedValue is User)
                {
                    tbTargetUsername.Text = (lbActiveClients.SelectedValue as User).Username;
                    foreach (RSA key in Ledger.KeysManifest)
                    {
                        if ((key.Receiver_Username == tbTargetUsername.Text ||
                            key.Sender_Username == tbTargetUsername.Text) &&
                            (key.Receiver_Username == Coordinator.Username ||
                            key.Sender_Username == Coordinator.Username)
                        )
                        {
                            KeyStatus.Text = $"Keys Exchanged :)";
                            bExchange.IsEnabled = false;
                            bSend.IsEnabled = true;
                            return;
                        }
                    }
                    KeyStatus.Text = $"No Keys Exchanged Yet :(";
                    bExchange.IsEnabled = true;
                    bSend.IsEnabled = false;
                }

            };


            this.DataContext = Coordinator = new Coordinator();
        }



        private void bSwitchServerState_Click(object sender, RoutedEventArgs e)
        {
            Coordinator.SwitchServerState();
        }

        private void bSend_Click(object sender, RoutedEventArgs e)
        {
            Coordinator.ServerSendMessages(tbTargetUsername.Text, SelectedFilePath);
        }

        private void bExchange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Coordinator.ExchangeKeysWithServer(tbTargetUsername.Text);
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
            openFile.Filter = "Text files (*.txt)|*.txt";
            Nullable<bool> openedFile = openFile.ShowDialog();
            if (openedFile.HasValue)
            {
                SelectedFilePath = openFile.FileName;
                tbMessage.Content = "Browse: Selected " + openFile.SafeFileName;
            }
        }
    }
}
