using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for VerificationWindow.xaml
    /// </summary>
    public partial class VerificationWindow : Window
    {
        private Message message;
 
        public VerificationWindow(Message message)
        {
            InitializeComponent();

            this.message = message;
            string text = StringHelper.convertToString(message.Bytes);

            FileContents.Text = text;
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RSA key in Ledger.KeysManifest)
            {
                if ((key.Receiver_Username == message.Receiver_Username ||
                    key.Sender_Username == message.Receiver_Username) &&
                    (key.Receiver_Username == message.Sender_Username ||
                    key.Sender_Username == message.Sender_Username)
                )
                {
                    byte[] decryptedBytes = key.DecryptData(FileContents.Text);
                    string decryptedText = StringHelper.convertToString(decryptedBytes);

                    FileContents.Text = decryptedText;
                    return;
                }
            }
        }
        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RSA key in Ledger.KeysManifest)
            {
                if ((key.Receiver_Username == message.Receiver_Username ||
                    key.Sender_Username == message.Receiver_Username) &&
                    (key.Receiver_Username == message.Sender_Username ||
                    key.Sender_Username == message.Sender_Username)
                )
                {
                    bool value = key.VerifyHash(FileContents.Text, message.Hash);
                    if (value)
                    {
                        MessageBox.Show("File is Verified and Unchanged", "SUCCESS");
                        return;
                    }
                    MessageBox.Show("File isn't Verified and Has Been Tampered With.", "ERROR");
                    return;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            Nullable<bool> savedFile = saveFileDialog.ShowDialog();
            try
            {
                if (savedFile.HasValue)
                {
                    string SelectedFilePath = saveFileDialog.FileName;
                    File.WriteAllText(SelectedFilePath, FileContents.Text);
                    MessageBox.Show($"File Saved at {SelectedFilePath}");
                    return;
                }
                MessageBox.Show("No File Name Given");
                return;
            }
            catch
            {
                MessageBox.Show("No File Name Given");
                return;
            }
        }
    }
}
