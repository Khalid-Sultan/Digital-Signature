using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Digital_Signature_Verification
{
    class Sender : INotifyPropertyChanged
    {
        #region Properties
        private string _username;

        private Thread _thread;
        private Socket _socket;
        private Dispatcher _dispatcher;

        private IPAddress _ipAddress;
        private int _port;
        private IPEndPoint _ipEndPoint => new IPEndPoint(_ipAddress, _port);

        private bool _isClientConnected;
        public bool IsClientDisconnected => !this.IsClientConnected;

        public BindingList<Message> Messages { get; set; }


        public bool IsClientConnected {
            get {
                return _isClientConnected;
            }
            private set {
                this._isClientConnected = value;
                this.NotifyPropertyChanged("IsClientConnected");
                this.NotifyPropertyChanged("IsClientDisconnected");
            }
        }

        public string Username {
            get {
                return _username;
            }
            set {
                this._username = value;
                if (this.IsClientConnected) {
                    this.SetUsername(value);
                }
            }
        }
        private void SetUsername(string newUsername)
        {
            string cmd = string.Format("/setname {0}", newUsername);
            try
            {
                this._socket.Send(Encoding.Unicode.GetBytes(cmd), SocketFlags.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Client Name Error : {ex.Message.ToString()}");
            }
        }

        public string IpAddress{
            get {
                return _ipAddress.ToString();
            }
            set {
                if (this.IsClientConnected)
                    throw new Exception("Can't change this property when server is active");
                _ipAddress = IPAddress.Parse(value);
            }
        }
        public int Port {
            get {
                return _port;
            }
            set {
                if (this.IsClientConnected)
                    throw new Exception("Can't change this property when server is active");
                this._port = value;
            }
        }


        #endregion

        #region Constructor
        public Sender()
        {
            this._dispatcher = Dispatcher.CurrentDispatcher;
            this.Messages = new BindingList<Message>();
            this.IpAddress = "127.0.0.1";
            this.Port = 6000;
            this.Username = "Client" + new Random().Next(0, 99).ToString();
        }
        #endregion

        #region Connection Methods
        public static bool IsSocketConnected(Socket s){
            if (!s.Connected) return false;
            if (s.Available == 0)
                if (s.Poll(1500, SelectMode.SelectRead))
                    return false;
            return true;
        }
        public void SwitchClientState()
        {
            if (!this.IsClientConnected) this.Connect();
            else this.Disconnect();
        }
        private void Connect() {
            if (this.IsClientConnected) return;

            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socket.Connect(this._ipEndPoint);
            SetUsername(this.Username);
            this._thread = new Thread(() => this.ReceiveMessages());
            this._thread.Start();
            this.IsClientConnected = true;
        }
        private void Disconnect(){
            if (!this.IsClientConnected) return;
            if (this._socket != null && this._thread != null){
                this._socket.Shutdown(SocketShutdown.Both);
                this._socket.Dispose();
                this._socket = null;
                this._thread = null;
            }
            this.Messages.Clear();
            this.IsClientConnected = false;
        }
        public void ExchangeKeys(string targetUsername){
            string cmd = string.Format("/keys {0}\n", targetUsername);
            try {
                this._socket.Send(Encoding.Unicode.GetBytes(cmd), SocketFlags.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Client : {ex.Message.ToString()}");
            }
        }

        #endregion

        #region Messaging Methods
        public void ReceiveMessages(){
            while (true) {
                byte[] inf = new byte[4096];
                try {
                    if (!IsSocketConnected(this._socket)) {
                        this._dispatcher.Invoke(new Action(() =>
                        {
                            this.Disconnect();
                        }));
                        return;
                    }
                    int x = this._socket.Receive(inf);
                    if (x > 0) {
                        string fileName = $"ENCRYPTED - {new Random(Seed: 151).Next(500000)}.txt";
                        File.WriteAllBytes(fileName, inf);

                        this._dispatcher.Invoke(new Action(() =>
                        {
                            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                            Message message = (Message)formatter.Deserialize(stream);
                            stream.Close();
                            this.Messages.Add(message);
                        }));
                    }
                }
                catch (Exception ex){
                    this._dispatcher.Invoke(new Action(() => {
                        MessageBox.Show($"Client Receiving Messages Error: {ex.Message.ToString()}");
                        this.Disconnect();
                    }));
                    return;
                }
            }

        }
        public void SendMessageTo(string targetUsername, string path)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(file);
            byte[] data = reader.ReadBytes((int)file.Length);
            reader.Close();
            file.Close();

            foreach (DsaTracker key in KeysManifestController.KeysManifest)
            {
                if ((key.receiver_id == targetUsername ||
                    key.sender_id == targetUsername) &&
                    (key.receiver_id == this.Username ||
                    key.sender_id == this.Username)
                )
                {
                    byte[] signedData = key.SignData(data);
                    string cmd = $"/msgto {targetUsername}:{key.ConvertBytesToString(data)}:{key.ConvertBytesToString(signedData)}";
                    try
                    {
                        this._socket.Send(Encoding.Unicode.GetBytes(cmd), SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Client : {ex.Message.ToString()}");
                    }
                    return;
                }
            }
        }
        #endregion

        #region Interfaces implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion

    }
}
