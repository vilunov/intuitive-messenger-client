using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MessagerClient
{
    public partial class List : Form
    {
        //HashMap, you can take path offile by its name
        public Dictionary<String, String> ListOfFiles = new Dictionary<String, String>();
        public bool IsDownload;

        public List()
        {
            InitializeComponent();
        }

        public void AddFileToList(String File)
        {
            //Короче я хуй знает зачем это нужно было, но мне нужен был просто список файлов на сервере
            //if (!ListOfFiles.ContainsKey(System.IO.Path.GetFileName(File)))
            //    ListOfFiles.Add(System.IO.Path.GetFileName(File), File);
            //Files.Items.Add(System.IO.Path.GetFileName(File));
            if (!ListOfFiles.ContainsKey(File))
                ListOfFiles.Add(File, File);
            Files.Items.Add(File);
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
            if (Selected != "")
            { 
                //ListOfFiles[Selected]
                // скачать
            }

                //код для скачивания файлов с сервера
            IsDownload = true;
            this.Enabled = false;
            this.Visible = false;
        }
    }
}
