using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Digital_Signature_Verification
{
    class User : IDisposable, INotifyPropertyChanged
    {
        //Properties
        private int _id;
        private string _username;
        public int ID {
            get {
                return _id;
            }
            set {
                this._id = value;
                this.NotifyPropertyChanged("ID");
            }
        }
        public string Username {
            get {
                return _username;
            }
            set {
                this._username = value;
                this.NotifyPropertyChanged("Username");
            }
        }

        //Connection Properties
        public Socket Socket { get; set; }
        public Thread Thread { get; set; }

        //Check Connection
        public bool IsSocketConnected(){
            if (!Socket.Connected) return false;
            return true;
        }

        //Send Messages
        public void SendMessage(byte[] message){
            try{
                Socket.Send(message);
            }
            catch(Exception ex){
                MessageBox.Show($"User: {ex.Message.ToString()}");
            }
        }




        //Interface Implementation
        //Handle When Properties are Changed
        private void NotifyPropertyChanged(string propName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        public event PropertyChangedEventHandler PropertyChanged;
        //Handle When Connection Is Need To Be Disposed and Discarded
        private bool _isDisposed = false;
        public void Dispose() {
            if (!_isDisposed) {
                if (this.Socket != null) {
                    this.Socket.Shutdown(SocketShutdown.Both);
                    this.Socket.Dispose();
                    this.Socket = null;
                }
                if (this.Thread != null) this.Thread = null;
                _isDisposed = true;
            }
        }
    }
}
