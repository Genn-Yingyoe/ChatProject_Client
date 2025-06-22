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

    public partial class NicknameInputForm : Form
    {
        public string NewNickname { get; private set; }

        public NicknameInputForm()
        {
            InitializeComponent();
            this.AcceptButton = btnOK;
            btnOK.Click += BtnOK_Click;
        }

        private void NicknameInputForm_Load(object sender, EventArgs e)
        {

        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string nick = txtNick.Text.Trim();
            if (string.IsNullOrEmpty(nick))
            {
                MessageBox.Show("새 닉네임을 입력해주세요.", "입력 오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            NewNickname = nick;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {

        }
    }
}
