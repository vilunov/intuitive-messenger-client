using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MessagerClient
{
    public partial class List : Form
    {
        //HashMap, you can take path offile by its name
        public List<String> ListOfFiles = new List<String>();
        public bool IsDownload;

        public List()
        {
            InitializeComponent();
        }

        public void AddFileToList(String File)
        {
            if (!ListOfFiles.Contains(File)) {
                ListOfFiles.Add(File); }
        }

        public void removeAllFiles()
        {
            ListOfFiles.Clear();
            Files.Items.Clear();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            this.Visible = false;
        }

        private void Download_Click(object sender, EventArgs e)
        {
            String Selected = "";
            foreach (string file in Files.Items)
            {
                if (Files.GetSelected(Files.Items.IndexOf(file)))
                {
                    Selected = file;
                    break;
                }
            }
                //код для скачивания файлов с сервера
            if (Selected != "")
            { 
                IsDownload = true;
            }
            this.Enabled = false;
            this.Visible = false;
        }
    }
}
