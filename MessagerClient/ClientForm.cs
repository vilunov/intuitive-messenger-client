using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.IO;

namespace MessagerClient
{
    public partial class Client : Form
    {
        class ChatMessage
        {
            public ChatMessage(String ntype, String nname, String nfilename, String ndate, String ntext)
            {
                type = ntype;
                name = nname;
                filename = nfilename;
                date = ndate;
                text = ntext;
            }

            public String type { get; set; }
            public String name { get; set; }
            public String filename { get; set; }
            public String date { get; set; }
            public String text { get; set; }
        }

        private String Name;
        private String[] FilesListBox = new String[0];
        private TcpClient client;
        private List List;
        public const string SERVER_IP = "localhost";
        public const int SERVER_PORT = 8080;

        public Client()
        {
            // закоментил подключение к серверу
            client = new TcpClient();
            client.Connect(SERVER_IP, SERVER_PORT);
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
            sendFiles();
            FilesListBox = new String[0];
            /*History.SelectionFont = new Font(History.SelectionFont, FontStyle.Bold);
            History.AppendText(DateTime.Now.ToLongTimeString() + "\t" + Name + ": ");
            History.SelectionFont = new Font(History.SelectionFont, FontStyle.Regular);
            History.AppendText(Message.Text + "\n");*/

            // for compression testing
            //byte[] arr = Compressor.Compress(new byte[] { 1, 3, 3, 7 });
            //foreach(var n in arr)
            //History.AppendText(n.ToString());

            //Нахуй пустые сообщения
            if (!String.IsNullOrWhiteSpace(Message.Text))
            {
                History.AppendText(DateTime.Now.ToLongTimeString() + "  " + Name + ": " + Message.Text + "\n");
                History.ScrollToCaret();
                String messg = JsonConvert.SerializeObject(new ChatMessage("Text", Name, "", DateTime.Now.ToLongTimeString(), Message.Text));
                byte[] data = System.Text.Encoding.UTF8.GetBytes(messg);
                sendBytes(data);
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
                    if (client.Connected == false)
                    {
                        client = new TcpClient();
                        client.Connect(SERVER_IP, SERVER_PORT);
                    }

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

                //Оповещение о загруженном файле для всех клиентов
                if (messg != "" && message.type == "Notification")
                {
                    History.AppendText(message.text);
                    History.ScrollToCaret();
                    History.Refresh();
                }

                //Ответ сервера на запрос о файле
                if (messg != "" && message.type == "File")
                {
                    History.AppendText(messg);
                    File.WriteAllBytes(message.filename, System.Convert.FromBase64String(message.text));
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
            FilesListBox = new String[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                FilesListBox[i] = files[i];
            }
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
            this.Enabled = true;
        }

        private void sendFiles()
        {
            if (FilesListBox.Length != 0)
            {
                for (int i = 0; i < FilesListBox.Length; i++)
                {
                    List.AddFileToList(FilesListBox[i]);
                }

                History.AppendText(DateTime.Now.ToLongTimeString() + "  " + Name + ": Send these files to server: ");
                for (int i = 0; i < FilesListBox.Length; i++)
                {
                    History.AppendText(FilesListBox[i]);
                    string file = System.Convert.ToBase64String(System.IO.File.ReadAllBytes(FilesListBox[i]));
                    string filename = FilesListBox[i].Split('\\')[FilesListBox[i].Split('\\').Length - 1];
                    String messg = JsonConvert.SerializeObject(new ChatMessage("File", Name, filename, DateTime.Now.ToLongTimeString(), file));
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(messg);
                    sendBytes(data);
                }
                History.AppendText("\n");
            }
        }

        private void sendBytes(byte[] bytes)
        {
            try
            {
                while (client.Connected == false)
                {
                    client = new TcpClient();
                    client.Connect(SERVER_IP, SERVER_PORT);
                }

                NetworkStream stream = client.GetStream();

                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();

                byte[] data = System.Text.Encoding.UTF8.GetBytes("END");
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
        }

        private byte[] recieveBytes()
        {
            return null;
        }
    }
}