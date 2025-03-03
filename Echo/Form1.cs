using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Echo
{
    public partial class Form1 : Form
    {
        Client client; 

        public Form1()
        {
            InitializeComponent();
            client = new Client(13000);
            client.PacketReceived += OnPacketReceived;
            client.Disconnected += OnDisconnected;
            FormClosed += (s, e) => client.Disconnect();
        }

        string username = "";
        Color user_color = Color.AliceBlue;

        private void chat_box_KeyDown(object sender, KeyEventArgs args)
        {
            if (args.KeyCode == Keys.Enter)
            {
                args.SuppressKeyPress = true;
                string message = chat_box.Text;
                client.SendPacket(message, PacketType.Text);
            }
        }

        private void AddMessage(string message, Color color)
        {

            RichTextBox message_box = new RichTextBox();
            message_box.ReadOnly = true;
            message_box.Width = message_panel.Width - 30;
            message_box.Font = new Font("Arial", 12);
            message_box.BorderStyle = BorderStyle.None;
            message_box.BackColor = Color.White;
            message_box.Text = message;
            message_box.Height = (int)message_box.CreateGraphics().MeasureString(message, message_box.Font, message_box.Width).Height + 10;
            message_box.Margin = new Padding(0, 0, 0, 0);

            message_panel.Controls.Add(message_box);
            message_panel.ScrollControlIntoView(message_box);

            chat_box.Clear();
        }

        private void connectbtn_Click(object sender, EventArgs e)
        {
            _ = Task.Run( () =>
            {
                connectbtn.Invoke(new MethodInvoker(delegate { connectbtn.Enabled = false; }));
                if (client.Connected)
                {
                    client.Disconnect();
                }
                else
                {
                    if (usernametxt.Text.Length > 0)
                    {
                        pickColorBtn.Invoke(new MethodInvoker(delegate { pickColorBtn.Enabled = false; }));
                        username = usernametxt.Text;
                        var succes = Task.Run(async () => await client.StartClient(username));
                        if (succes.Result)
                        {
                            connectbtn.Invoke(new MethodInvoker(delegate { connectbtn.Text = "Disconnect"; }));
                        }
                        else
                        {
                            pickColorBtn.Invoke(new MethodInvoker(delegate { pickColorBtn.Enabled = true; }));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                connectbtn.Invoke(new MethodInvoker(delegate { connectbtn.Enabled = true; }));
            });
        }

        void OnPacketReceived(object source, PacketArgs args)
        {
            if(args.packetType == PacketType.Text)
            {
                message_panel.Invoke(new MethodInvoker( delegate { 
                    AddMessage(client.users[args.fromId].username + ": " + args.data, Color.AliceBlue); 
                }));
            }
        }

        void OnDisconnected(object source, EventArgs args)
        {
            if(connectbtn.InvokeRequired)
                connectbtn.Invoke(new MethodInvoker(delegate { connectbtn.Text = "Connect"; pickColorBtn.Enabled = true;  }));
            else
            {
                pickColorBtn.Enabled = true;
                connectbtn.Text = "Connect";
            }
        }

        private void pickColorBtn_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            user_color = colorDialog1.Color;
        }
    }
}
