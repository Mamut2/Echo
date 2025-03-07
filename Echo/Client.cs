using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using Echo;
using System.ComponentModel;

namespace Echo
{
    public enum PacketType
    {
        Empty = 0,
        Text = 1,
        Disconnect = 2,
        Connect = 3,
        AssignId = 4
    }

    class User
    {
        public string username = "";
    }

    class Client
    {
        const int MAX_USERS = 5;
        TcpClient tcp;
        int port, id;
        public User[] users = new User[MAX_USERS];

        public bool Connected
        {
            get { return tcp.Connected; }
        }

        public event EventHandler<PacketArgs> PacketReceived;
        public event EventHandler Disconnected;

        public Client(int port)
        {
            for (int i = 0; i < MAX_USERS; i++) users[i] = new User();
            this.port = port;
            tcp = new TcpClient();
        }

        public Task<bool> StartClient(string username)
        {
            tcp = new TcpClient();
            var result = tcp.BeginConnect(Dns.GetHostAddresses("0.tcp.eu.ngrok.io"), 12308, null, null);
            var succes = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

            if (!succes || !tcp.Connected)
            {
                Console.WriteLine("Connection failed");
                succes = false;
                tcp.Close();
                tcp = new TcpClient();
            }
            else
            {
                tcp.EndConnect(result);
                SendPacket(username, PacketType.Connect);
                _ = Task.Run(() => ReceivePackets());
            }
            return Task.FromResult(succes);
        }

        public void SendPacket(string data, PacketType packetType)
        {
            try
            {
                if (Connected)
                {
                    NetworkStream stream = tcp.GetStream();

                    string pt = ((int)packetType).ToString();

                    data = pt + '|' + id + '|' + data;
                    Byte[] bytes = Encoding.ASCII.GetBytes(data);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); Disconnect(); }
        }

        void ReceivePackets()
        {
            NetworkStream stream = tcp.GetStream();
            Byte[] bytes = new Byte[256];
            string data = null;
            while (Connected)
            {
                if (tcp.Available > 0)
                {
                    int i = stream.Read(bytes, 0, bytes.Length), fromId;
                    PacketType packetType = PacketType.Empty;
                    data = Encoding.ASCII.GetString(bytes, 0, i);

                    string[] p = data.Split('|');
                    packetType = (PacketType)int.Parse(p[0]);
                    fromId = int.Parse(p[1]);
                    data = "";
                    for (int k = 2; k < p.Length; k++) data += p[k];

                    switch (packetType)
                    {
                        case PacketType.Text:
                            break;
                        case PacketType.Disconnect:
                            Disconnect();
                            break;
                        case PacketType.Connect:
                            users[fromId].username = data;
                            break;
                        case PacketType.AssignId:
                            this.id = fromId;
                            break;
                    }
                    OnPacketReceived(new PacketArgs(packetType, data, fromId));
                }
            }
        }

        public void Disconnect()
        {
            SendPacket(null, PacketType.Disconnect);
            tcp.Close();
            tcp = new TcpClient();
            OnDisconnected();
        }

        void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        void OnPacketReceived(PacketArgs args)
        {
            PacketReceived?.Invoke(this, args);
        }
    }
}

public class PacketArgs : EventArgs
{
    public PacketType packetType;
    public string data;
    public int fromId;

    public PacketArgs(PacketType packetType, string data, int fromId)
    {
        this.packetType = packetType;
        this.data = data;
        this.fromId = fromId;
    }
}
