using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MessagerClient
{
    public partial class Client : Form
    {

        private byte[] appendBytes(byte[] bt1, byte[] bt2)
        {
            byte[] result = new byte[bt1.Length + bt2.Length];
            for (int i = 0; i < bt1.Length + bt2.Length; i++)
                if (i < bt1.Length)
                    result[i] = bt1[i];
                else
                    result[i] = bt2[i - bt1.Length];

            return result;
        }

        private byte[] generateChatMessage(String type, String name, String filename, String text)
        {
            byte[] message = new byte[1];
            switch (type)
            {
                case "Text":
                    message[0] = 0;
                    break;
                case "File":
                    message[0] = 1;
                    break;
                case "File Request":
                    message[0] = 2;
                    break;
            }

            byte[] uname = System.Text.Encoding.UTF8.GetBytes(name);
            byte[] fname = System.Text.Encoding.UTF8.GetBytes(filename);
            byte[] txt = System.Text.Encoding.UTF8.GetBytes(text + "END");

            message = appendBytes(message, new byte[] { (byte)uname.Length });
            message = appendBytes(message, new byte[] { (byte)fname.Length });

            message = appendBytes(message, uname);
            message = appendBytes(message, fname);
            message = appendBytes(message, txt);

            return message;
        }

        private String[] extractChatMessage(byte[] messg)
        {
            String[] message = new String[4];
            switch (messg[0])
            {
                case 0:
                    message[0] = "Text";
                    break;
                case 1:
                    message[0] = "File";
                    break;
                case 2:
                    message[0] = "Notification";
                    break;
                case 3:
                    message[0] = "Table info";
                    break;
            }

            int namesize = (int)messg[1];
            int filenamesize = (int)messg[2];

            if (namesize != 0)
            {
                byte[] uname = new byte[namesize];
                for (int i = 3; i < 3 + namesize; i++)
                    uname[i - 3] = messg[i];
                message[1] = System.Text.Encoding.UTF8.GetString(uname);
            }
            else
                message[1] = "";

            if (filenamesize != 0)
            {
                byte[] fname = new byte[filenamesize];
                for (int i = 3 + namesize; i < 3 + namesize + filenamesize; i++)
                    fname[i - 3 - namesize] = messg[i];
                message[2] = System.Text.Encoding.UTF8.GetString(fname);
            }
            else
                message[2] = "";
            
            if (3+namesize+filenamesize != messg.Length)
            {
                byte[] txt = new byte[messg.Length - 3 - namesize - filenamesize];
                for (int i = 3 + namesize + filenamesize; i < messg.Length; i++)
                    txt[i - 3 - namesize - filenamesize] = messg[i];
                if (message[0] == "File")
                    message[3] = System.Text.Encoding.UTF8.GetString(txt);
                else
                    message[3] = System.Text.Encoding.UTF8.GetString(txt);
            }
            else
                message[3] = "";
            return message;
        }

        private String Name;
        private String File = "";
        private TcpClient client;
        private List List;
        public static string SERVER_IP;
        public const int SERVER_PORT = 8080;
        public const string FILE_DIR = "files\\";

        public Client()
        {
            // закоментил подключение к серверу
            InitializeComponent();
            this.Enabled = false;
            Welcome Welcome = new Welcome();
            Welcome.Activate();
            Welcome.Show();
            Welcome.Refresh();
            while (Welcome.Username == "" && Welcome.IP == "")
            {
                Application.DoEvents();
            }
            Name = Welcome.Username;
            SERVER_IP = Welcome.IP;
            client = new TcpClient();
            client.Connect(SERVER_IP, SERVER_PORT);
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
            SendFile();

            // for compression testing
            //byte[] arr = Compressor.Compress(new byte[] { 1, 3, 3, 7 });
            //foreach(var n in arr)
            //History.AppendText(n.ToString());

            //Фикс отправки пробельчиков
            if (!String.IsNullOrWhiteSpace(Message.Text))
            {
                History.AppendText(Name + ": " + Message.Text + "\n");
                History.ScrollToCaret();
                byte[] data = generateChatMessage("Text", Name, "", Message.Text);
                SendBytes(data);
            }

            Message.Clear();
        }

        private void ServerSays()
        {
            //WARNING THIS IS KOCTb|/\b look at Program.cs
            while (!this.IsDisposed)
            {
                Application.DoEvents();

                string messg = GetResponse();

                if (messg == "")
                    continue;

                if (messg.Split('#').Length > 1)
                {
                    string[] messages = messg.Split('#');

                    String[] message1 = extractChatMessage(System.Text.Encoding.UTF8.GetBytes(messages[0]));
                    String[] message2 = extractChatMessage(System.Text.Encoding.UTF8.GetBytes(messages[1]));

                    //Оповещение о загруженном файле для клиентов
                    if (message1[1] != Name)
                    {
                        History.SelectionFont = new Font(History.SelectionFont, FontStyle.Bold);
                        History.AppendText(message1[3]);
                        History.SelectionFont = new Font(History.SelectionFont, FontStyle.Regular);
                        History.ScrollToCaret();
                    }

                    //Таблица должна грузиться рилтайм при загрузке нового файла. Ахуенно, да?
                    List.removeAllFiles();
                    string[] files = message2[3].Split(';');
                    foreach (string file in files)
                        List.AddFileToList(file);

                    continue;
                }

                String[] message = extractChatMessage(System.Text.Encoding.UTF8.GetBytes(messg));

                if (message[0] == "Table info")
                {
                    List.removeAllFiles();
                    string[] files = message[3].Split(';');
                    foreach (string file in files)
                        List.AddFileToList(file);
                }

                if (message[0] == "Text" && message[1] != Name)
                {
                    History.AppendText(message[1] + ": " + message[3] + "\n");
                    History.ScrollToCaret();
                }

                //Ответ сервера на запрос о файле
                if (message[0] == "File")
                {
                    if (Directory.Exists(FILE_DIR))
                        System.IO.File.WriteAllBytes(FILE_DIR + message[2], System.Convert.FromBase64String(message[3]));
                    else
                    {
                        System.IO.Directory.CreateDirectory(FILE_DIR);
                        System.IO.File.WriteAllBytes(FILE_DIR + message[2], System.Convert.FromBase64String(message[3]));
                    }

                    History.SelectionFont = new Font(History.SelectionFont, FontStyle.Bold);
                    History.AppendText(" You successfully downloaded " + message[2] + "\n");
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

        private void Attach_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK) // if user clicked OK
            {
                File = dialog.FileName; // get name of file
                Send.PerformClick();
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
                    byte[] data = generateChatMessage("File Request", Name, Selected, "");
                    SendBytes(data);
                }

                List.IsDownload = false;
            }

            this.Enabled = true;
            this.Focus();
        }

        //Льет текущее значение File на сервер и обнуляет его
        private void SendFile()
        {
            if (File != "")
            {
                string file = System.Convert.ToBase64String(System.IO.File.ReadAllBytes(File));
                string filename = File.Split('\\')[File.Split('\\').Length - 1];
                byte[] data = generateChatMessage("File", Name, filename, file);
                SendBytes(data);
                File = "";

                //Жырная обводачка
                History.SelectionFont = new Font(History.SelectionFont, FontStyle.Bold);
                History.AppendText( "File " + filename + " was successuflly sent to the server \n");
                History.SelectionFont = new Font(History.SelectionFont, FontStyle.Regular);
                History.ScrollToCaret();
                History.Refresh();
            }
        }

        //Отправляет байтики отформатированного жсон стринга на сервер
        private void SendBytes(byte[] bytes)
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
        private string GetResponse()
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
                if (!stream.DataAvailable)
                    return response;

                byte[] data = new byte[client.ReceiveBufferSize];

                while (!response.EndsWith("END"))
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    response += Encoding.UTF8.GetString(data, 0, bytes);
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

            return response.Substring(0, response.Length - 3);
        }
    }
}