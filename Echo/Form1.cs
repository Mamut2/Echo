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
            FormClosed += (s, e) => client.Disconnect();
        }

        string username = "User";
        Color user_color = Color.Blue;

        private void chat_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string message = chat_box.Text;
                AddMessage(username + ":" + message, user_color);
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
            message_box.SelectionStart = 0;
            message_box.SelectionLength = username.Length;
            message_box.SelectionColor = color;
            message_box.Height = (int)message_box.CreateGraphics().MeasureString(message, message_box.Font, message_box.Width).Height + 10;
            message_box.Margin = new Padding(0, 0, 0, 0);

            message_panel.Controls.Add(message_box);

            chat_box.Clear();
        }

        void OnPacketReceived(object source, EventArgs args)
        {
            
        }

        private void connectbtn_Click(object sender, EventArgs e)
        {
            Task.Run( () =>
            {
                connectbtn.Invoke(new MethodInvoker(delegate { connectbtn.Enabled = false;  }));
                if (client.Connected)
                {
                    client.Disconnect();
                    connectbtn.Text = "Connect";
                }
                else
                {
                    if (usernametxt.Text.Length > 0)
                    {
                        username = usernametxt.Text;
                        var succes = Task.Run(async () => await client.StartClient(username));
                        if (succes.Result)
                            connectbtn.Text = "Disconnect";
                    }
                    else
                    {
                        MessageBox.Show("Invalid username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                connectbtn.Invoke(new MethodInvoker(delegate { connectbtn.Enabled = true; }));
            });
        }
    }
}
