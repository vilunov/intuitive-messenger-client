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
    public partial class List : Form
    {
        public String[] ListOfFiles;

        public List()
        {
            InitializeComponent();
        }

        public void AddFileToList(String File)
        {
            Files.Items.Add(File);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            this.Visible = false;
        }

        private void Download_Click(object sender, EventArgs e)
        {
            //код для скачивания файлов с сервера
        }
    }
}
