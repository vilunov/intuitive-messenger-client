using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        private double NoiseRate;
        private String File = "";
        private TcpClient client;
        private List List;
        public readonly string SERVER_IP;
        public const int SERVER_PORT = 8080;
        public const string FILE_DIR = "files";
        public static int flipped = 0;

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
            while (Welcome.Username == "" && Welcome.IP == "" && Welcome.NoiseRate <= 0)
            {
                Application.DoEvents();
            }
            Name = Welcome.Username;
            SERVER_IP = Welcome.IP;
            NoiseRate = Welcome.NoiseRate;
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
            while (!this.IsDisposed)
            {
                Application.DoEvents();
                byte[] response = GetResponse();
                if (response != null)
                    ProcessServerResponse(response);
            }
        }

        private void Send_Click(object sender, EventArgs e)
        {
            SendFile();

            if (!String.IsNullOrWhiteSpace(Message.Text))
            {
                History.AppendText(Name + ": " + Message.Text + "\n");
                History.ScrollToCaret();
                byte[] data = generateChatMessage("Text", Name, "", Encoding.UTF8.GetBytes(Message.Text));
                SendBytes(data);
            }

            Message.Clear();
        }

        private void LogMessage(IEnumerable<string> strs)
        {
            History.SelectionFont = FONT_BOLD;
            foreach(var str in strs)
                History.AppendText(str);
            History.SelectionFont = FONT_NORMAL;
            History.ScrollToCaret();
            History.Refresh();
        }
        
        private void LogMessage(string str)
        {
            History.SelectionFont = FONT_BOLD;
            History.AppendText(str);
            History.SelectionFont = FONT_NORMAL;
            History.ScrollToCaret();
            History.Refresh();
        }
        
        /*
         * Handle the message from server
         * Updates the file list if received the list of files
         * Writes the message to the text box if received a text message
         * Decodes, decompresses and saves the file if received a file
         */
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
                    if (strings[1] == Name || strings[1] == "") break;
                        History.SelectionFont = FONT_BOLD;
                        History.AppendText(" User " + strings[1] + " has uploaded new file \n");
                        History.SelectionFont = FONT_NORMAL;
                    break;
                case "Text":
                    if (strings[1] == Name || content == null) break;
                    History.AppendText(strings[1] + ": " + Encoding.UTF8.GetString(content) + "\n");
                    History.ScrollToCaret();
                    break;
                case "File":
                    if (!Directory.Exists(FILE_DIR))
                        Directory.CreateDirectory(FILE_DIR);
                    Stopwatch sw = Stopwatch.StartNew();
                    byte[] withNoise = Noise(NoiseRate, content);
                    byte[] decoded = Compressor.Decode(withNoise); //introdusing some noise
                    sw.Stop();
                    if (decoded == null)
                    {
                        LogMessage($"Failed to decoded {strings[2]}\n");
                        return;
                    }
                    LogMessage(new[]
                    {
                        $"Number of flips: {flipped}\n",
                        $"Noise percentage: {NoiseRate}\n",
                        $"Decoding time: {sw.ElapsedMilliseconds} ms\n",
                        $"Decoded size: {decoded.Length} bytes ({(decoded.Length / 1024)}KB)\n\n"
                    });
                    sw.Reset();
                    sw.Start();
                    byte[] decompressed = Compressor.Decompress(decoded);
                    sw.Stop();
                    if (decompressed == null)
                    {
                        LogMessage($"Failed to decompress {strings[2]}\n");
                        return;
                    }
                    LogMessage(new[]
                    {
                        $"Decompression time: {sw.ElapsedMilliseconds} ms \n",
                        $"Decompression size: {decompressed.Length} bytes ({(decompressed.Length / 1024)}KB)\n\n",
                        $"You successfully downloaded {strings[2]} (file size: {content.Length} bytes)\n"
                    });
                    System.IO.File.WriteAllBytes(Path.Combine(FILE_DIR, strings[2]), decompressed);
                    LogMessage($"Successfully decompressed to {decompressed.Length} bytes\n");
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

        /*
         * Reads and sends the file to the server
         */
        private void SendFile()
        {
            if (File == "") return;
            byte[] fileContents = System.IO.File.ReadAllBytes(File);
            string filename = Path.GetFileName(File);
            byte[] data = generateChatMessage("File", Name, filename, CompressAndEncode(filename, fileContents));
            SendBytes(data);
            File = "";
            LogMessage($"File {filename} was successuflly send to server \n");
        }

        /**
         * Calls the compression and encoding methods
         */
        private byte[] CompressAndEncode(string filename, byte[] data)
        {
            LogMessage(
                $"File {filename} was successuflly compressed from {data.Length} bytes ({(data.Length / 1024)}KB)\n");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            data = Compressor.Compress(data);
            sw.Stop();
            long t = sw.ElapsedMilliseconds;
            LogMessage(new[]
            {
                $"Algorithm: {Compressor.CurrCompression}\n",
                $"Time: {t} ms\n",
                $"Compressed size: {data.Length} bytes ({(data.Length / 1024)}KB)\n\n"
            });
            sw.Reset();
            sw.Start();
            data = Compressor.Encode(data);
            sw.Stop();
            t = sw.ElapsedMilliseconds;
            LogMessage(new[]
            {
                $"Algorithm: {Compressor.CurrEncoding}\n",
                $"Time: {t} ms\n",
                $"Encoded size: {data.Length} bytes ({(data.Length / 1024)}KB)\n\n"
            });
            return data;
        }

        /*
         * Sends the bytes to the server
         */
        private void SendBytes(byte[] bytes)
        {
            try
            {
                //Reconnect when the connection has failed
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
        
        private List<byte> output = new List<byte>(); 
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
                
                byte[] data = new byte[client.ReceiveBufferSize];
                if (output.Count < 7)
                {
                    if (!stream.DataAvailable) return null;
                    int bytes = stream.Read(data, 0, data.Length);
                    if (bytes == 0)
                        return null;
                    for(int i = 0; i < bytes; i++)
                        output.Add(data[i]);
                }
               
                MessageHeader header = extractHeader(output.GetRange(0, 7).ToArray());

                int len = header.name + header.fname + header.datalength + 7;
                if (output.Count >= len)
                {
                    var ret = output.GetRange(0, len).ToArray();
                    output = output.GetRange(len, output.Count - len);
                    return ret.ToArray();
                }
                while (true)
                {
                    Thread.Sleep(50);
                    if (!stream.DataAvailable)
                        break;
                    int bytes = stream.Read(data, 0, data.Length);
                    if (bytes == 0) break;
                    if (output.Count >= len) break;
                    for(int i = 0; i < bytes; i++)
                        output.Add(data[i]);
                }
                stream.Flush();
                if (output.Count >= len)
                {
                    var ret = output.GetRange(0, len).ToArray();
                    output = output.GetRange(len, output.Count - len);
                    return ret.ToArray();
                }
                return null;
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

        /**
         * This method randomly flips some bits in the data, using the percentage as a probability
         */
        private static byte[] Noise(double precentage, byte[] data)
        {
            System.Random rng = new System.Random();
            BitArray bits = new BitArray(data);
            flipped = 0;
            for (int i = 0; i < bits.Count; i++)
            {
                if (rng.NextDouble() * 100 > precentage) continue;
                flipped++;
                bits.Set(i, !bits.Get(i));
            }
 
            return ToByteArray(bits);
        }

        public static byte[] ToByteArray(BitArray bits)
        {
            int numBytes = bits.Count / 8;
            if (bits.Count % 8 != 0) numBytes++;

            byte[] bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    bytes[byteIndex] |= (byte)(1 << bitIndex);
                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }
            return bytes;
        }
    }
}