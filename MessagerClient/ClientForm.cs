﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;

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
        private String File = "";
        private TcpClient client;
        private List List;
        public const string SERVER_IP = "192.168.0.107";
        public const int SERVER_PORT = 8080;
        public const string FILE_DIR = "files\\";

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
            sendFile();

            /*History.SelectionFont = new Font(History.SelectionFont, FontStyle.Bold);
            History.AppendText(DateTime.Now.ToLongTimeString() + "\t" + Name + ": ");
            History.SelectionFont = new Font(History.SelectionFont, FontStyle.Regular);
            History.AppendText(Message.Text + "\n");*/

            // for compression testing
            //byte[] arr = Compressor.Compress(new byte[] { 1, 3, 3, 7 });
            //foreach(var n in arr)
            //History.AppendText(n.ToString());

            //Фикс отправки пробельчиков
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
                Application.DoEvents();

                string messg = getResponse();

                if (messg == "")
                    continue;

                if (messg.Split('#').Length > 1)
                {
                    string[] messages = messg.Split('#');

                    ChatMessage message1 = JsonConvert.DeserializeObject<ChatMessage>(messages[0]);
                    ChatMessage message2 = JsonConvert.DeserializeObject<ChatMessage>(messages[1]);

                    //Оповещение о загруженном файле для клиентов
                    if (message1.name != Name)
                    {
                        History.SelectionFont = new Font(History.SelectionFont, FontStyle.Bold);
                        History.AppendText(message1.text);
                        History.SelectionFont = new Font(History.SelectionFont, FontStyle.Regular);
                        History.ScrollToCaret();
                    }

                    //Таблица должна грузиться рилтайм при загрузке нового файла. Ахуенно, да?
                    List.removeAllFiles();
                    string[] files = message2.text.Split(';');
                    foreach (string file in files)
                    List.AddFileToList(file);

                    continue;
                }

                ChatMessage message = JsonConvert.DeserializeObject<ChatMessage>(messg);

                if (message.type == "Table info")
                {
                    List.removeAllFiles();
                    string[] files = message.text.Split(';');
                    foreach(string file in files)
                    List.AddFileToList(file);
                }

                if (message.type == "Text" && message.name != Name)
                {
                    History.AppendText(message.date+": "+message.date + "  " + message.name + ": " + message.text + "\n");
                    History.ScrollToCaret();
                }

                //Ответ сервера на запрос о файле
                if (message.type == "File")
                {
                    if(Directory.Exists(FILE_DIR))
                        System.IO.File.WriteAllBytes(FILE_DIR+message.filename, System.Convert.FromBase64String(message.text));
                    else
                    {
                        System.IO.Directory.CreateDirectory(FILE_DIR);
                        System.IO.File.WriteAllBytes(FILE_DIR + message.filename, System.Convert.FromBase64String(message.text));
                    }
                        
                    History.SelectionFont = new Font(History.SelectionFont, FontStyle.Bold);
                    History.AppendText(DateTime.Now.ToLongTimeString() + " You successfully downloaded " + message.filename + "\n");
                    History.SelectionFont = new Font(History.SelectionFont, FontStyle.Regular);
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
                {
                    //Жалобное прошение выделенного файла
                    String messg = JsonConvert.SerializeObject(new ChatMessage("File Request", Name, Selected, DateTime.Now.ToLongTimeString(),""));
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(messg);
                    sendBytes(data);
                }

                List.IsDownload = false;
            }

            this.Enabled = true;
            this.Focus();
        }

        //Льет текущее значение File на сервер и обнуляет его
        private void sendFile()
        {
            if (File != "")
            {
                //Жырная обводачка
                History.SelectionFont = new Font(History.SelectionFont, FontStyle.Bold);
                History.AppendText(DateTime.Now.ToLongTimeString()+ " File " + System.IO.Path.GetFileName(File) + " was successuflly sent to the server \n");
                History.SelectionFont = new Font(History.SelectionFont, FontStyle.Regular);
                History.ScrollToCaret();
                History.Refresh();

                string file = System.Convert.ToBase64String(System.IO.File.ReadAllBytes(File));
                string filename = File.Split('\\')[File.Split('\\').Length - 1];
                String messg = JsonConvert.SerializeObject(new ChatMessage("File", Name, filename, DateTime.Now.ToLongTimeString(), file));
                byte[] data = System.Text.Encoding.UTF8.GetBytes(messg);
                sendBytes(data);
                File = "";
            }
        }

        //Отправляет байтики отформатированного жсон стринга на сервер
        private void sendBytes(byte[] bytes)
        {
            try
            {
                //Реконнект при обрыве
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

        //Пиздеж ответа сервера
        private string getResponse()
        {
            string response = "";
            try
            {
                while (client.Connected == false)
                {
                    client = new TcpClient();
                    client.Connect(SERVER_IP, SERVER_PORT);
                }

                NetworkStream stream = client.GetStream();

                byte[] data = new byte[256];
                while (stream.DataAvailable)
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    response += Encoding.UTF8.GetString(data, 0, bytes).ToString();
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

            return response;
        }
    }
}