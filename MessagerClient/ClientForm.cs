﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace MessagerClient
{
    public partial class Client : Form
    {
        class ChatMessage
        {
            public ChatMessage(String ntype, String nname, String ndate, String ntext)
            {
                type = ntype;
                name = nname;
                date = ndate;
                text = ntext;
            }

            public String type { get; set; }
            public String name { get; set; }
            public String date { get; set; }
            public String text { get; set; }
        }

        private String Name;
        private String File = "";
        private TcpClient client;
        private List List;

        public Client()
        {
            // закоментил подключение к серверу
            //client = new TcpClient();
            //client.Connect("localhost", 8080);
            InitializeComponent();
            this.Enabled = false;
            Welcome Welcome = new Welcome();
            Welcome.Activate();
            Welcome.Show();
            Welcome.Refresh();
            while (Welcome.Username == "")
            {
                Application.DoEvents();
            }
            Name = Welcome.Username;
            Welcome.Close();
            this.Enabled = true;
            this.Show();
            List = new List
            {
                Visible = false,
                Enabled = false
            };

            ServerSays();
        }

        private void Send_Click(object sender, EventArgs e)
        {
            if (File != "")
            {
                List.AddFileToList(File);
                History.AppendText(DateTime.Now.ToLongTimeString() + " <b> " + Name + ": Send these file to server: " + System.IO.Path.GetFileName(File) + "</b>\n");
            }
            /*History.SelectionFont = new Font(History.SelectionFont, FontStyle.Bold);
            History.AppendText(DateTime.Now.ToLongTimeString() + "\t" + Name + ": ");
            History.SelectionFont = new Font(History.SelectionFont, FontStyle.Regular);
            History.AppendText(Message.Text + "\n");*/
            if (Message.Text != "")
                History.AppendText(DateTime.Now.ToLongTimeString() + "  " + Name + ": " + Message.Text + "\n");
            History.ScrollToCaret();
            File = "";
            // for compression testing
            //byte[] arr = Compressor.Compress(new byte[] { 1, 3, 3, 7 });
            //foreach(var n in arr)
            //History.AppendText(n.ToString());

            try
            {
                NetworkStream stream = client.GetStream();

                String messg = JsonConvert.SerializeObject(new ChatMessage("Text", Name, DateTime.Now.ToLongTimeString(), Message.Text));
                byte[] data = System.Text.Encoding.UTF8.GetBytes(messg);
                stream.Write(data, 0, data.Length);

                stream.Flush();
            }

            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }

            Message.Clear();
        }

        private void ServerSays()
        {
            while (true)
            {
                string messg = "";
                Application.DoEvents();
                try
                {
                    NetworkStream stream = client.GetStream();


                    byte[] data = new byte[256];
                    while (stream.DataAvailable)
                    {
                        int bytes = stream.Read(data, 0, data.Length);
                        messg += Encoding.UTF8.GetString(data, 0, bytes).ToString();
                    }

                    stream.Flush();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("SocketException: {0}", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                }

                ChatMessage message = JsonConvert.DeserializeObject<ChatMessage>(messg);

                if (messg != "" && message.type == "Text" && message.name != Name)
                {
                    History.AppendText(message.date + "  " + message.name + ": " + message.text + "\n");
                    History.ScrollToCaret();
                    History.Refresh();
                }
            }
        }

        ////При нажате Enter нажимается кнопка
        //private void Client_KeyUp(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar == (char)Keys.Enter)
        //    {
        //        Send.PerformClick();
        //    }
        //}

        private void FilesListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void FilesListBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            File = files[0];
        }

        private void FilesList_Click(object sender, EventArgs e)
        {
            List.Enabled = true;
            List.Show();
            this.Enabled = false;
            while (List.Enabled)
            {
                Application.DoEvents();
            };
            if (List.IsDownload)
            {
                String Selected = "";
                foreach (string file in List.Files.Items)
                {
                    if (List.Files.GetSelected(List.Files.Items.IndexOf(file)))
                    {
                        Selected = file;
                        break;
                    }
                }
                if (Selected != "")
                    History.AppendText(DateTime.Now.ToLongTimeString() + "<b> You successfully download " + Selected + " <b>\n");
                List.IsDownload = false;
            }
            this.Enabled = true;
            this.Focus();
        }
    }
}