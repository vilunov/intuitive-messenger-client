using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

        private MessageHeader extractHeader(byte[] messg)
        {
            byte[] contentSize = {messg[3], messg[4], messg[5], messg[6]};
            int contentSizeC =
                BitConverter.ToInt32(BitConverter.IsLittleEndian ? contentSize : contentSize.Reverse().ToArray(), 0);
            return new MessageHeader
            {
                type = messg[0],
                name = messg[1],
                fname = messg[2],
                datalength = contentSizeC
            };
        }

        private Tuple<string[], byte[]> extractChatMessage(byte[] messg)
        {
            MessageHeader header = extractHeader(messg);
            string[] message = new string[3];
            switch (header.type)
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
            int i = 7;
            if (header.name != 0)
            {
                byte[] uname = new byte[header.name];
                for (int j = 0; j < header.name; i++, j++)
                    uname[j] = messg[i];
                message[1] = Encoding.UTF8.GetString(uname);
            }
            else
                message[1] = "";

            if (header.fname != 0)
            {
                byte[] fname = new byte[header.fname];
                for (int j = 0; j < header.fname; i++, j++)
                    fname[j] = messg[i];
                message[2] = Encoding.UTF8.GetString(fname);
            }
            else
                message[2] = "";

            if (3 + header.name + header.fname == messg.Length)
                return new Tuple<string[], byte[]>(message, null);
            
            List<byte> data = new List<byte>(messg.Length - 3 - header.name - header.datalength);
            for (int j = 0; j < header.datalength; i++, j++)
                data.Add(messg[i]);
        
            return new Tuple<string[], byte[]>(message, data.ToArray());
        }

        private String Name;
        private String File = "";
        private TcpClient client;
        private List List;
        public readonly string SERVER_IP;
        private Thread t;
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
            t = new Thread(NetworkThread);
            t.Start();
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

        private void ProcessServerResponse(byte[] messg)
        {
            if (messg == null)
                return;

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
                    byte[] data = generateChatMessage("File Request", Name, Selected, new byte[0]);
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


        private void NetworkThread()
        {
            while (!this.IsDisposed)
            {
                byte[] response = GetResponse();
                if (response != null)
                    ProcessServerResponse(response);
                Thread.Sleep(500);
            }
        }
        
        private byte[] GetResponse()
        {
            List<byte> output = new List<byte>();
            
            try
            {
                while (!client.Connected)
                {
                    client = new TcpClient();
                    client.Connect(SERVER_IP, SERVER_PORT);
                }

                NetworkStream stream = client.GetStream();

                byte[] data = new byte[client.ReceiveBufferSize];

                if (!stream.DataAvailable) return null;
                int bytes = stream.Read(data, 0, data.Length);
                if (bytes == 0)
                    return null;
                MessageHeader header = extractHeader(data);
                for(int i = 0; i < bytes; i++)
                    output.Add(data[i]);

                if (output.Count >= header.name + header.fname + header.datalength + 7)
                    return output.ToArray();
                while (true)
                {
                    Thread.Sleep(50);
                    if (!stream.DataAvailable)
                        break;
                    bytes = stream.Read(data, 0, data.Length);
                    if (bytes == 0) break;
                    if (output.Count >= header.name + header.fname + header.datalength + 7) break;
                    for(int i = 0; i < bytes; i++)
                        output.Add(data[i]);
                }
                stream.Flush();
                return output.Count >= header.name + header.fname + header.datalength + 7 ? output.ToArray() : null;
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