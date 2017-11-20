using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MessagerClient
{
    public partial class Client : Form
    {

        private byte[] generateChatMessage(String type, String name, String filename, byte[] data)
        {
            List<byte> list = new List<byte>();
            switch (type)
            {
                case "Text":
                    list.Add(0);
                    break;
                case "File":
                    list.Add(1);
                    break;
                case "File Request":
                    list.Add(2);
                    break;
            }

            byte[] uname = Encoding.UTF8.GetBytes(name);
            byte[] fname = Encoding.UTF8.GetBytes(filename);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            list.Add((byte)uname.Length);
            list.Add((byte)fname.Length);
            list.AddRange(BitConverter.IsLittleEndian ? dataLength : dataLength.Reverse());
            
            list.AddRange(uname);
            list.AddRange(fname);
            list.AddRange(data);

            return list.ToArray();
        }

        private Tuple<string[], byte[]> extractChatMessage(byte[] messg)
        {
            string[] message = new string[3];
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

            int namesize = messg[1];
            int filenamesize = messg[2];
            byte[] contentSize = {messg[3], messg[4], messg[5], messg[6]};
            int contentSizeC = BitConverter.ToInt32(BitConverter.IsLittleEndian ? contentSize : contentSize.Reverse().ToArray(), 0);

            int i = 7;
            if (namesize != 0)
            {
                byte[] uname = new byte[namesize];
                for (int j = 0; j < namesize; i++, j++)
                    uname[j] = messg[i];
                message[1] = Encoding.UTF8.GetString(uname);
            }
            else
                message[1] = "";

            if (filenamesize != 0)
            {
                byte[] fname = new byte[filenamesize];
                for (int j = 0; j < filenamesize; i++, j++)
                    fname[j] = messg[i];
                message[2] = Encoding.UTF8.GetString(fname);
            }
            else
                message[2] = "";

            if (3 + namesize + filenamesize == messg.Length)
                return new Tuple<string[], byte[]>(message, null);
            
            List<byte> data = new List<byte>(messg.Length - 3 - namesize - filenamesize);
            for (int j = 0; j < contentSizeC; i++, j++)
                data.Add(messg[i]);
        
            return new Tuple<string[], byte[]>(message, data.ToArray());
        }

        private String Name;
        private String File = "";
        private TcpClient client;
        private List List;
        public readonly string SERVER_IP;
        public const int SERVER_PORT = 8080;
        public const string FILE_DIR = "files";

        private readonly Font FONT_BOLD;
        private readonly Font FONT_NORMAL;

        public Client()
        {
            InitializeComponent();
            FONT_BOLD = new Font(History.SelectionFont, FontStyle.Bold);
            FONT_NORMAL = new Font(History.SelectionFont, FontStyle.Regular);
            
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

            //Фикс отправки пробельчиков
            if (!String.IsNullOrWhiteSpace(Message.Text))
            {
                History.AppendText(Name + ": " + Message.Text + "\n");
                History.ScrollToCaret();
                byte[] data = generateChatMessage("Text", Name, "", Encoding.UTF8.GetBytes(Message.Text));
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

                byte[] messg = GetResponse();

                if (messg == null)
                    continue;

                var message = extractChatMessage(messg);
                string[] strings = message.Item1;
                byte[] content = message.Item2;

                switch (strings[0])
                {
                    case "Table info":
                        List.removeAllFiles();
                        string[] files = Encoding.UTF8.GetString(content).Split(';');
                        foreach (string file in files)
                            List.AddFileToList(file);
                        break;
                    case "Text":
                        if (strings[1] != Name || content == null) break;
                        Console.WriteLine(strings[1]);
                        Console.WriteLine(Name);
                        History.AppendText(strings[1] + ": " + Encoding.UTF8.GetString(content) + "\n");
                        History.ScrollToCaret();
                        break;
                    case "File":
                        if (!Directory.Exists(FILE_DIR))
                            Directory.CreateDirectory(FILE_DIR);
                        System.IO.File.WriteAllBytes(Path.Combine(FILE_DIR, strings[2]), content);

                        History.SelectionFont = FONT_BOLD;
                        History.AppendText(" You successfully downloaded " + strings[2] + "\n");
                        History.SelectionFont = FONT_NORMAL;
                        History.ScrollToCaret();
                        History.Refresh();
                        break;
                }

                //Ответ сервера на запрос о файле
            }
        }

        private void FilesListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
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
            OpenFileDialog dialog = new OpenFileDialog {Multiselect = false};
            if (dialog.ShowDialog() != DialogResult.OK) return;
            File = dialog.FileName; // get name of file
            Send.PerformClick();
        }

        private void FilesList_Click(object sender, EventArgs e)
        {
            List.Enabled = true;
            List.Show();
            this.Enabled = false;
            while (List.Enabled)
            {
                Application.DoEvents();
            }

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
                    byte[] data = generateChatMessage("File Request", Name, Selected, null);
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
            if (File == "") return;
            byte[] fileContents = System.IO.File.ReadAllBytes(File);
            string filename = Path.GetFileName(File);
            byte[] data = generateChatMessage("File", Name, filename, fileContents);
            SendBytes(data);
            File = "";

            //Жырная обводачка
            History.SelectionFont = FONT_BOLD;
            History.AppendText( "File " + filename + " was successuflly sent to the server \n");
            History.SelectionFont = FONT_NORMAL;
            History.ScrollToCaret();
            History.Refresh();
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
        private byte[] GetResponse()
        {
            try
            {
                while (!client.Connected)
                {
                    client = new TcpClient();
                    client.Connect(SERVER_IP, SERVER_PORT);
                }

                NetworkStream stream = client.GetStream();
                if (!stream.DataAvailable)
                    return null;

                byte[] data = new byte[client.ReceiveBufferSize];

                int bytes = stream.Read(data, 0, data.Length);
                if (bytes == 0)
                    return null;
                
                List<byte> output = new List<byte>();
                for(int i = 0; i < bytes; i++)
                    output.Add(data[i]);
                
                while (stream.DataAvailable && (bytes = stream.Read(data, 0, data.Length)) != 0)
                {
                    for(int i = 0; i < bytes; i++)
                        output.Add(data[i]);
                }

                stream.Flush();
                return output.ToArray();
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
            return null;
        }
    }
}