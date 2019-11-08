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
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private Sender Sender;
        private string SelectedFilePath = null;

        public UserWindow()
        {
            InitializeComponent();
            Sender = new Sender();
            this.DataContext = Sender;
        }

        private void bSwitchClientState_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Sender.SwitchClientState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bSend_Click(object sender, RoutedEventArgs e)
        {
            Sender.SendMessageTo(tbTargetUsername.Text, SelectedFilePath);
        }

        private void bExchange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Sender.ExchangeKeys(tbTargetUsername.Text);
                tbMessage.IsEnabled = true;
                bSend.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tbMessage.IsEnabled = false;
                bSend.IsEnabled = false;
            }
        }

        private void bBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".txt";
            Nullable<bool> openedFile = openFile.ShowDialog();
            if (openedFile.HasValue)
            {
                SelectedFilePath = openFile.FileName;
                tbMessage.Content = "Browse: Selected " + openFile.SafeFileName;
            }
        }

    }
}
