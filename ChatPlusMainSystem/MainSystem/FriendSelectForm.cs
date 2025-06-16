using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MainSystem
{
    public partial class FriendSelectForm : Form
    {
        private List<FriendInfo> friendList;
        private List<FriendInfo> selectedFriends = new List<FriendInfo>();

        // internal로 접근성 일치
        internal List<FriendInfo> SelectedFriends => selectedFriends;

        // 생성자 접근자를 internal로 변경하여 FriendInfo와 접근성 일치
        internal FriendSelectForm(List<FriendInfo> friends)
        {
            InitializeComponent();
            friendList = friends;
            InitializeUI();
            LoadFriends();
        }

        private void InitializeUI()
        {
            this.Text = "친구 선택";
            this.Size = new Size(350, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(200, 202, 224);
        }

        private void LoadFriends()
        {
            flpFriends.Controls.Clear();

            foreach (var friend in friendList)
            {
                CheckBox chkFriend = new CheckBox
                {
                    Text = $"{friend.Nickname} (ID: {friend.Friend_Id})",
                    Tag = friend,
                    Size = new Size(flpFriends.Width - 30, 25),
                    Margin = new Padding(5),
                    Font = new Font("맑은 고딕", 10),
                    BackColor = Color.Transparent
                };

                chkFriend.CheckedChanged += (s, e) =>
                {
                    if (chkFriend.Checked)
                    {
                        if (!selectedFriends.Contains(friend))
                            selectedFriends.Add(friend);
                    }
                    else
                    {
                        selectedFriends.Remove(friend);
                    }

                    btnOK.Enabled = selectedFriends.Count > 0;
                };

                flpFriends.Controls.Add(chkFriend);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (selectedFriends.Count == 0)
            {
                MessageBox.Show("최소 한 명의 친구를 선택해주세요.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
