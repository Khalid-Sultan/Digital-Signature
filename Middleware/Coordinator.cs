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
    class Coordinator : INotifyPropertyChanged
    {
        #region Properties
        private string _username;
        private int _clientIdCounter;

        private Dispatcher _dispatcher { get; set; }
        private Thread _thread;
        private Socket _socket;

        private IPAddress _ipAddress;
        private int _port;
        private IPEndPoint _ipEndPoint => new IPEndPoint(_ipAddress, _port);

        private bool _isServerActive;
        public BindingList<User> UsersList { get; set; }
        public BindingList<Message> MessagesList { get; set; }
        public bool IsServerStopped => !this.IsServerActive;
        public int ActiveUsers => UsersList.Count;

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                if (this.IsServerActive)
                    throw new Exception("Can't change this property when server is active");
                this._port = value;
            }
        }
        public string IpAddress
        {
            get
            {
                return _ipAddress.ToString();
            }
            set
            {
                if (this.IsServerActive)
                    throw new Exception("Can't change this property when server is active");
                _ipAddress = IPAddress.Parse(value);
            }
        }
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                this._username = value;
                if (this.IsServerActive)
                {
                    this.UsersList[0].Username = value;
                }
            }
        }
        public bool IsServerActive
        {
            get
            {
                return _isServerActive;
            }
            private set
            {
                this._isServerActive = value;

                this.NotifyPropertyChanged("IsServerActive");
                this.NotifyPropertyChanged("IsServerStopped");
            }
        }



        #endregion

        #region Constructor
        public Coordinator()
        {
            this._dispatcher = Dispatcher.CurrentDispatcher;
            this.MessagesList = new BindingList<Message>();
            this.UsersList = new BindingList<User>();
            this.UsersList.ListChanged += (_sender, _e) =>
            {
                this.NotifyPropertyChanged("ActiveClients");
            };

            this._clientIdCounter = 0;
            this.IpAddress = "127.0.0.1";
            this.Port = 6000;
            this.Username = "Server";
        }
        #endregion

        #region Connection Methods
        public void StartServer()
        {
            if (this.IsServerActive) return;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(_ipEndPoint);
            _socket.Listen(5);
            _thread = new Thread(new ThreadStart(WaitForConnections));
            _thread.Start();
            UsersList.Add(new User() { ID = 0, Username = this.Username });
            this.IsServerActive = true;
        }
        public void StopServer()
        {
            if (!this.IsServerActive) return;
            UsersList.Clear();
            while (UsersList.Count != 0)
            {
                User user = UsersList[0];
                UsersList.Remove(user);
                user.Dispose();
            }
            _socket.Dispose();
            _socket = null;
            this.IsServerActive = false;
        }
        public void SwitchServerState()
        {
            if (!this.IsServerActive) this.StartServer();
            else this.StopServer();
        }
        void WaitForConnections()
        {
            while (true)
            {
                if (_socket == null) return;
                User user = new User
                {
                    ID = this._clientIdCounter,
                    Username = "NewUser"
                };
                try
                {
                    user.Socket = _socket.Accept();
                    user.Thread = new Thread(() => CoordinateMessages(user));

                    this._dispatcher.Invoke(new Action(() =>
                    {
                        UsersList.Add(user);
                    }), null);

                    user.Thread.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Coordinator : {ex.Message}", "Error");
                }
            }
        }
        public void ExchangeKeysWithServer(string targetUsername)
        {
            foreach (CryptographyHelper key in Ledger.KeysManifest)
            {
                if ((key.Receiver_Username == targetUsername ||
                    key.Sender_Username == targetUsername) &&
                    (key.Receiver_Username == this.UsersList[0].Username ||
                    key.Sender_Username == this.UsersList[0].Username)
                )
                {
                    MessageBox.Show("Coordinator: Keys are already exchanged.");
                    return;
                }

            }
            CryptographyHelper newKey = new CryptographyHelper(
                Sender_Username: this.UsersList[0].Username,
                Receiver_Username: targetUsername
            );
            Ledger.KeysManifest.Add(newKey);
        }
        #endregion


        #region Messaging Methods
        public void ServerSendMessages(string toUsername, string path)
        {
            string text = File.ReadAllText(path);
            foreach (CryptographyHelper key in Ledger.KeysManifest)
            {
                if ((key.Receiver_Username == toUsername ||
                    key.Sender_Username == toUsername) &&
                    (key.Receiver_Username == this.Username ||
                    key.Sender_Username == this.Username)
                )
                {
                    try
                    {
                        string encryptedText = key.EncryptContent(text);
                        string hash = key.GetHash(encryptedText);
                        string fileName = $"S-ENCRYPTED{new Random().Next(50000).ToString()}.txt";
                        Message message = new Message(encryptedText, Username, toUsername, hash, Category.Message,fileName);
                        Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Write);
                        System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        formatter.Serialize(stream, message);
                        stream.Close();
                        SendMessageTo(this.UsersList[0], toUsername, fileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Coordinator: {ex.Message.ToString()}");
                    }
                    return;
                }
            }
        }
        private void SendMessageTo(User from, string toUsername, string fileName)
        {
            foreach (CryptographyHelper key in Ledger.KeysManifest)
            {
                if ((key.Receiver_Username == toUsername ||
                    key.Sender_Username == toUsername) &&
                    (key.Receiver_Username == from.Username ||
                    key.Sender_Username == from.Username)
                )
                {

                    bool isSent = false;
                    try
                    {  
                        System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        Message message = (Message)formatter.Deserialize(stream);
                        stream.Close();
                        this.MessagesList.Add(message);
                        if (toUsername == this.Username)
                        {
                            isSent = true;
                        }
                        else
                        {
                            foreach (User user in UsersList)
                            {
                                if (user.Username == this.Username) continue;
                                if (user.Username == toUsername)
                                {  
                                    user.SendMessage(fileName);
                                    isSent = true;
                                }
                            }
                        }
                        if (!isSent)
                        {
                            MessageBox.Show("Coordinator: Error! Username not found, unable to deliver your message");
                        }
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Coordinator: {ex.Message.ToString()}");
                    }
                    return;
                }
            }
        }
        void CoordinateMessages(User user)
        {
            while (true)
            {
                try
                {
                    if (!user.IsSocketConnected())
                    {
                        this._dispatcher.Invoke(new Action(() =>
                        {
                            UsersList.Remove(user);
                            user.Dispose();
                        }), null);
                        return;
                    }

                    byte[] inf = new byte[4096];
                    int x = user.Socket.Receive(inf, SocketFlags.None);
                    if (x > 0)
                    {
                        string fileName = $"c{new Random().Next(500)}.dat";
                        File.WriteAllBytes(fileName, inf);
                        this._dispatcher.Invoke(new Action(() =>
                        {
                            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                            Message message = (Message)formatter.Deserialize(stream);
                            stream.Close();
                            switch (message.Category)
                            {
                                case Category.Naming:
                                    string newUsername = message.content;
                                    user.Username = newUsername;
                                    File.Delete(fileName);
                                    return;
                                case Category.Message:
                                    SendMessageTo(user, message.Receiver_Username, message.File_Name);
                                    File.Delete(fileName);
                                    return;
                            }
                        }));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Coordinator : {ex.Message.ToString()}");
                    this._dispatcher.Invoke(new Action(() =>
                    {
                        UsersList.Remove(user);
                        user.Dispose();
                    }), null);
                    return;
                }
            }
        }

        #endregion


        #region Interface implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion

    }
}
