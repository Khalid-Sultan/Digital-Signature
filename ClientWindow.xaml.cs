using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
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
        private ChatClient cc;
        private string filepath = null;

        public ClientWindow()
        {
            InitializeComponent();
            cc = new ChatClient();
            this.DataContext = cc;
        }

        private void bSwitchClientState_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cc.SwitchClientState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bSend_Click(object sender, RoutedEventArgs e)
        {
            cc.SendMessageTo(tbTargetUsername.Text, filepath);
        }

        private void bExchange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cc.ExchangeKeys(tbTargetUsername.Text);
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
            if (openedFile.HasValue){
                filepath = openFile.FileName;
                tbMessage.Content = "Browse: Selected " + openFile.SafeFileName;
            }
        }
    }
}
