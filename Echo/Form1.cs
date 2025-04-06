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
using System.Threading.Tasks;

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

        class SearchResult
        {
            public string Title { get; }
            public string Url { get; }
            public byte[] Thumbnail { get; }

            public SearchResult(string title, string url, byte[] thumbnail)
            {
                Title = title;
                Url = url;
                Thumbnail = thumbnail;
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
        const int port = 13000;
        ConcurrentDictionary<string, ClientInfo> clients = new ConcurrentDictionary<string, ClientInfo>();

        VoipNode voip;

        void SetupUI()
        {
            // Tab control
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(0, 1);
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.SelectedTab = loginTabPage;

            // Message list
            lstMessages.DrawMode = DrawMode.OwnerDrawVariable;
            lstMessages.DrawItem += DrawMessageItem;
            lstMessages.MeasureItem += (s, e) => e.ItemHeight = 60;

            // Users list
            lstUsers.DrawMode = DrawMode.OwnerDrawVariable;
            lstUsers.DrawItem += DrawUserItem;
            lstUsers.MeasureItem += (s, e) => e.ItemHeight = 60;

            // YtPlayer
            lstYtSearchResults.DrawMode = DrawMode.OwnerDrawVariable;
            lstYtSearchResults.DrawItem += DrawSearchResultItem;
            lstYtSearchResults.MeasureItem += (s, e) => { e.ItemHeight = 160; };
            YtPlayer.OnVideoInfoReceived += (string title, string url, byte[] thumbnailData) => {
                using (MemoryStream ms = new MemoryStream(thumbnailData))
                using (Bitmap resized = new Bitmap(Image.FromStream(ms), 150 * 16/9, 150))
                using (MemoryStream resizedMs = new MemoryStream())
                {
                    resized.Save(resizedMs, ImageFormat.Png);
                    AddSearchResult(title, url, resizedMs.ToArray());
                }
            };
        }

        private void connectbtn_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                Disconnect();
            }
            else
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

                if (string.IsNullOrEmpty(ipInput.Text))
                {
                    MessageBox.Show("Server ip required");
                    return;
                }

                try
                {
                    client = new TcpClient();
                    client.Connect(IPAddress.Parse(ipInput.Text), port);
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
                    UpdateUI(() => connectbtn.Text = "Disconnect");

                    voip = new VoipNode(port + 1);
                    voip.LogMessage += (string msg) => { Console.WriteLine(msg); };
                    voip.Start();
                }
                catch
                {
                    Disconnect();
                }
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
                        bool isMine = reader.ReadBoolean();

                        clients[senderId] = new ClientInfo(username, avatar);
                        if(!isMine) voip.ConnectToPeer(IPAddress.Parse(ip), port + 1);

                        AddUserToList(senderId);
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
            RemoveUserFromList(senderId);
            //clients.TryRemove(senderId, out ClientInfo _);
        }

        void Disconnect()
        {
            if (!isConnected) return;

            isConnected = false;

            UpdateUI(() =>
            {
                lstUsers.Items.Clear();
                connectbtn.Text = "Connect";
                AddSystemMessage("Disconnected from server");
            });

            try
            {
                stream?.Close();
                client?.Close();
                voip.Dispose();
            }
            catch { }
        }

        void DrawSearchResultItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();
            var result = (SearchResult)lstYtSearchResults.Items[e.Index];

            // Draw thumbnail
            using (MemoryStream ms = new MemoryStream(result.Thumbnail))
            using (Image image = Image.FromStream(ms))
            {
                e.Graphics.DrawImage(image, e.Bounds.Left + 5, e.Bounds.Top + 5, 150 * 16/9, 150);
            }

            // Draw text
            using (var font = new Font("Segoe UI", 12))
            {
                e.Graphics.DrawString(result.Title, font, Brushes.Navy, e.Bounds.Left + 300, e.Bounds.Top + 10);
            }

            e.DrawFocusRectangle();
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
                Rectangle rect = new Rectangle(e.Bounds.Left + 60, e.Bounds.Top + 25, e.Bounds.Width - 60, e.Bounds.Height - 25);
                e.Graphics.DrawString(msg.SenderUsername, usernameFont, Brushes.Navy, e.Bounds.Left + 60, e.Bounds.Top);
                e.Graphics.DrawString(msg.Text, font, Brushes.Black, rect);
            }

            e.DrawFocusRectangle();
        }

        void DrawUserItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();
            string userId = (string)lstUsers.Items[e.Index];

            // Draw avatar
            if (clients.TryGetValue(userId, out ClientInfo client))
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
                e.Graphics.DrawString(clients[userId].username, usernameFont, Brushes.Navy, e.Bounds.Left + 60, e.Bounds.Top + 15);
            }

            e.DrawFocusRectangle();
        }

        void AddSearchResult(string title, string url, byte[] thumbnailData)
        {
            UpdateUI(() =>
            {
                using (MemoryStream ms = new MemoryStream(thumbnailData))
                using (Image image = Image.FromStream(ms))
                {
                    lstYtSearchResults.Items.Add(new SearchResult(title, url, thumbnailData));
                }
            });
        }

        void AddUserToList(string userId)
        {
            UpdateUI(() =>
            {
                lstUsers.Items.Insert(lstUsers.Items.Count, userId);
            });
        }

        void RemoveUserFromList(string userId)
        {
            UpdateUI(() =>
            {
                lstUsers.Items.Remove(userId);
            });
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

        private void messagingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = messagingTabPage;
        }

        private void youtubePlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = ytTabPage;
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = loginTabPage;
        }

        private void btnSearchYt_Click(object sender, EventArgs e)
        {
            UpdateUI(() => lstYtSearchResults.Items.Clear());
            Task.Run(async () =>
            {
                await YtPlayer.SearchAsync(searchYtTextBox.Text.Trim());
            });
        }

        private void lstYtSearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            var result = (SearchResult)lstYtSearchResults.SelectedItem;
            Task.Run(async () =>
            {
                YtPlayer.StopPlayback();
                await YtPlayer.PlayAsync(result.Url);
            });
        }
    }
}
