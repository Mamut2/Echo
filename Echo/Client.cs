using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Echo
{
    public enum PacketType
    {
        Text = 0,
        Disconnect = 1,
        Connect = 2
    }

    class Client
    {

        TcpClient tcp;
        int port;

        public bool Connected
        {
            get { return tcp.Connected; }
        }

        public event EventHandler PacketReceived;

        public Client(int port)
        {
            this.port = port;
            tcp = new TcpClient();
        }

        public Task<bool> StartClient(string username)
        {
            tcp = new TcpClient();
            var result = tcp.BeginConnect(IPAddress.Loopback, port, null, null);
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
                _ = Task.Run(async () => await ReceivePackets());
            }
            return Task.FromResult(succes);
        }

        public void SendPacket(string data, PacketType type)
        {
            if (Connected)
            {
                NetworkStream stream = tcp.GetStream();
                Byte[] bytes = Encoding.ASCII.GetBytes(((int)type).ToString() + data);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        async Task ReceivePackets()
        {
            NetworkStream stream = tcp.GetStream();
            StreamReader reader = new StreamReader(stream);
            while (Connected)
            {
                if (tcp.Available > 0)
                {
                    string message = await reader.ReadLineAsync();
                    OnPacketReceived();
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        public void Disconnect()
        {
            if(Connected)
            {
                SendPacket(null, PacketType.Disconnect);
                tcp.Close();
                tcp = new TcpClient();
            }
        }

        protected virtual void OnPacketReceived()
        {
            PacketReceived?.Invoke(this, EventArgs.Empty);
        }
    }
}
