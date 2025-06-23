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
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    SelectedImagePath = ofd.FileName;
                    pictureBoxPreview.Image = Image.FromFile(SelectedImagePath);
                    pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedImagePath))
            {
                MessageBox.Show("업로드할 이미지를 선택해주세요.",
                                "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
