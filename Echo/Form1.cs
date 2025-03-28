using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Collections.Concurrent;

namespace Echo
{
    public partial class Form1 : Form
    {
        class ChatMessage
        {
            public string Sender { get; }
            public string SenderUsername { get; }
            public string Text { get; }
            public DateTime TimeStamp { get; }

            public ChatMessage(string sender, string senderUsername, string text)
            {
                Sender = sender;
                SenderUsername = senderUsername;
                Text = text;
                TimeStamp = DateTime.Now;
            }
        }

        public Form1()
        {
            InitializeComponent();
            SetupUI();
        }

        enum PacketType { UserInfo = 0, Message = 1, Disconnect = 2 }

        class ClientInfo
        {
            public string username;
            public byte[] avatar;

            public ClientInfo(string username, byte[] avatar)
            {
                this.username = username;
                this.avatar = avatar;
            }
        }

        TcpClient client;
        NetworkStream stream;
        string localUsername;
        byte[] localAvatar;
        bool isConnected;
        const int port = 19083;
        ConcurrentDictionary<string, ClientInfo> clients = new ConcurrentDictionary<string, ClientInfo>();

        VoipNode voip = new VoipNode(port + 1);

        void SetupUI()
        {
            lstMessages.DrawMode = DrawMode.OwnerDrawVariable;
            lstMessages.DrawItem += DrawMessageItem;
            lstMessages.MeasureItem += (s, e) => e.ItemHeight = 60;
        }

        private void connectbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(usernametxt.Text))
            {
                MessageBox.Show("Username required");
                return;
            }

            if (pfpImage.Image == null)
            {
                MessageBox.Show("Profile image required");
                return;
            }

            try
            {
                client = new TcpClient();
                client.Connect(Dns.GetHostAddresses("0.tcp.eu.ngrok.io"), port);
                stream = client.GetStream();
                isConnected = true;
                localUsername = usernametxt.Text;

                using (MemoryStream ms = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(localUsername);
                    writer.Write(localAvatar.Length);
                    writer.Write(localAvatar);
                    SendPacket(PacketType.UserInfo, ms.ToArray());
                }


                AddSystemMessage("Connected to server");
                new Thread(ReceiveLoop) { IsBackground = true }.Start();
                UpdateUI(() => connectbtn.Enabled = false);

                voip.Start();
            }
            catch
            {
                Disconnect();
            }
        }

        void ReceiveLoop()
        {
            try
            {
                while (isConnected)
                {
                    byte[] header = new byte[8];
                    int bytesRead = stream.Read(header, 0, 8);
                    if (bytesRead != 8) break;

                    int type = BitConverter.ToInt32(header, 0);
                    int length = BitConverter.ToInt32(header, 4);

                    byte[] data = new byte[length];
                    int totalRead = 0;
                    while (totalRead < length)
                    {
                        int read = stream.Read(data, totalRead, length - totalRead);
                        if (read == 0) break;
                        totalRead += read;
                    }

                    ProcessPacket((PacketType)type, data);
                }
            }
            catch { }
            finally { 
                Disconnect(); 
            }
        }

        void ProcessPacket(PacketType type, byte[] data)
        {
            switch (type)
            {
                case PacketType.UserInfo:
                    using (MemoryStream ms = new MemoryStream(data))
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                        string senderId = reader.ReadString();
                        string username = reader.ReadString();
                        byte[] avatar = reader.ReadBytes(reader.ReadInt32());
                        string ip = reader.ReadString();
                        clients[senderId] = new ClientInfo(username, avatar);

                        voip.ConnectToPeer(IPAddress.Parse(ip), port + 1);
                    }
                    break;

                case PacketType.Message:
                    using (MemoryStream ms = new MemoryStream(data))
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                        string senderId = reader.ReadString();
                        string message = reader.ReadString();
                        AddChatMessage(senderId, message);
                    }
                    break;
                case PacketType.Disconnect:
                    using (MemoryStream ms = new MemoryStream(data))
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                        string senderId = reader.ReadString();
                        OnClientDisconnected(senderId);
                    }
                    break;
            }
        }

        void SendPacket(PacketType type, byte[] data)
        {
            byte[] header = new byte[8];
            BitConverter.GetBytes((int)type).CopyTo(header, 0);
            BitConverter.GetBytes(data.Length).CopyTo(header, 4);

            stream.Write(header, 0, 8);
            stream.Write(data, 0, data.Length);
        }

        private void change_pfp_btn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Images|*.jpg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Image image = Image.FromFile(ofd.FileName))
                        using (Bitmap resized = new Bitmap(image, new Size(64, 64)))
                        using (Bitmap resizedUI = new Bitmap(image, new Size(200, 200)))
                        using (MemoryStream ms = new MemoryStream())
                        {
                            resized.Save(ms, ImageFormat.Png);
                            localAvatar = ms.ToArray();
                            pfpImage.Image = new Bitmap(resizedUI);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Invalid image file");
                    }
                }
            }
        }

        void UpdateUI(Action action)
        {
            if (InvokeRequired) Invoke(action);
            else action();
        }

        void OnClientDisconnected(string senderId)
        {
            AddSystemMessage($"{clients[senderId].username} disconnected");
            //clients.TryRemove(senderId, out ClientInfo _);
        }

        void Disconnect()
        {
            if (!isConnected) return;

            isConnected = false;
            UpdateUI(() =>
            {
                connectbtn.Enabled = true;
                AddSystemMessage("Disconnected from server");
            });

            try
            {
                stream?.Close();
                client?.Close();
            }
            catch { }
        }

        void DrawMessageItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();
            var msg = (ChatMessage)lstMessages.Items[e.Index];

            // Draw avatar
            if (clients.TryGetValue(msg.Sender, out ClientInfo client))
            {
                using (MemoryStream ms = new MemoryStream(client.avatar))
                using (Image image = Image.FromStream(ms))
                {
                    e.Graphics.DrawImage(image, e.Bounds.Left + 5, e.Bounds.Top + 5, 50, 50);
                }
            }

            // Draw text
            using (var usernameFont = new Font("Segoe UI", 12))
            using (var font = new Font("Segoe UI", 9))
            {
                e.Graphics.DrawString(msg.SenderUsername, usernameFont, Brushes.Navy, e.Bounds.Left + 60, e.Bounds.Top);
                e.Graphics.DrawString(msg.Text, font, Brushes.Black, e.Bounds.Left + 60, e.Bounds.Top + 25);
            }

            e.DrawFocusRectangle();
        }

        void AddChatMessage(string sender, string message)
        {
            UpdateUI(() =>
            {
                lstMessages.Items.Insert(lstMessages.Items.Count, new ChatMessage(sender, clients[sender].username, message));
                lstMessages.TopIndex = lstMessages.Items.Count - 1;
            });
        }

        void AddSystemMessage(string message)
        {
            UpdateUI(() => 
            {
                lstMessages.Items.Insert(lstMessages.Items.Count, new ChatMessage("System", "System", message));
                lstMessages.TopIndex = lstMessages.Items.Count - 1;
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Disconnect();
            base.OnFormClosing(e);
        }

        private void chat_box_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (!isConnected || string.IsNullOrEmpty(chat_box.Text.Trim())) return;

                try
                {
                    SendPacket(PacketType.Message, Encoding.UTF8.GetBytes(chat_box.Text.Trim()));
                    chat_box.Clear();
                }
                catch
                {
                    Disconnect();
                }
            }
        }
    }
}
