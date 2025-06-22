using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainSystem
{
    public partial class ConfigForm : Form
    {
        private string userId;
        private string nick;

        public ConfigForm(string userId, string nick)
        {
            InitializeComponent();
            this.userId = userId;
            this.nick = nick;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var profileForm = new ProfileForm(this.userId, this.nick))
            {
                profileForm.ShowDialog();
            }
        }
    }
}
