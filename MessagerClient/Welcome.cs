using System;
using System.Windows.Forms;

namespace MessagerClient
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
            Compression.Items.Add(Compressor.Compression.HUFFMAN);
            Compression.Items.Add(Compressor.Compression.SHANNON_FANO);
            Compression.Items.Add(Compressor.Compression.ARITHMETIC);
            Encoding.Items.Add(Compressor.Encoding.HAMMING);
            Encoding.Items.Add(Compressor.Encoding.PARITY_CHECK);
            Encoding.Items.Add(Compressor.Encoding.REPETITION);
            Compression.SelectedItem = Compression.Items[0];
            Encoding.SelectedItem = Encoding.Items[0];
        }

        public String Username = "";
        public String IP = "";
        public double NoiseRate = 0;

        private void EnterName_Click(object sender, EventArgs e)
        {
            Username = UsernameBox.Text;
            IP = ipBox.Text;
            NoiseRate = Convert.ToDouble(Noise.Text);
            Compressor.CurrCompression = (Compressor.Compression) Compression.SelectedItem;
            Compressor.CurrEncoding = (Compressor.Encoding ) Encoding.SelectedItem;
        }
    }
}
