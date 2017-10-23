using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MessagerClient
{
    public partial class Client : Form
    {
        private String Name;
        private String FilesListBox = "";

        public Client()
        {
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
        }

        void History_TextChanged(object sender, EventArgs e)
        {

        }

        private void Send_Click(object sender, EventArgs e)
        {
            if (FilesListBox != "")
                History.AppendText(DateTime.Now.ToLongTimeString() + "  " + Name + ": " + FilesListBox);
            FilesListBox = "";

            /*History.SelectionFont = new Font(History.SelectionFont, FontStyle.Bold);
            History.AppendText(DateTime.Now.ToLongTimeString() + "\t" + Name + ": ");
            History.SelectionFont = new Font(History.SelectionFont, FontStyle.Regular);
            History.AppendText(Message.Text + "\n");*/
            if (Message.Text != "")
                History.AppendText(DateTime.Now.ToLongTimeString() + "  " + Name + ": " + Message.Text + "\n");
            History.ScrollToCaret();
            Message.Clear();
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

            foreach (string file in files)
            {
                FilesListBox += file + "\n";
            }
        }
    }
}