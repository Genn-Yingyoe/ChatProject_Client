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
    public partial class ImageUploadForm : Form
    {
        public string SelectedImagePath { get; private set; }

        public ImageUploadForm()
        {
            InitializeComponent();
            this.AcceptButton = btnOK;

            btnOK.BackColor = Color.FromArgb(41, 47, 102);
            btnOK.ForeColor = Color.White;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.FlatAppearance.BorderSize = 0;

            btnBrowse.BackColor = Color.FromArgb(41, 47, 102);
            btnBrowse.ForeColor = Color.White;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderSize = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click_1(object sender, EventArgs e)
        {

        }
    }
}
