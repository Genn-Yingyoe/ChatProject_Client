using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MainSystem
{
    public partial class UserInviteDialog : Form
    {
        public string UserIdToInvite { get; private set; }

        public UserInviteDialog()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "사용자 초대";
            this.Size = new Size(350, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(200, 202, 224);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string userId = txtUserId.Text.Trim();

            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("사용자 ID를 입력해주세요.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserId.Focus();
                return;
            }

            if (userId.Length != 6)
            {
                MessageBox.Show("사용자 ID는 6자리여야 합니다.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserId.Focus();
                return;
            }

            // 숫자만 체크
            if (!userId.All(char.IsDigit))
            {
                MessageBox.Show("사용자 ID는 숫자만 입력할 수 있습니다.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserId.Focus();
                return;
            }

            UserIdToInvite = userId;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtUserId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(sender, e);
            }
        }
    }
}