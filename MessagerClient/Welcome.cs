using System;
using System.Windows.Forms;

namespace MessagerClient
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
        }

        public String Username = "";
        public String IP = "";

        private void EnterName_Click(object sender, EventArgs e)
        {
            Username = UsernameBox.Text;
            IP = ipBox.Text;
        }
    }
}
