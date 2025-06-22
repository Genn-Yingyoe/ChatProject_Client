using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainSystem
{
    public partial class FriendProfileForm : Form
    {
        private string friendId;
        private string friendName;
        private readonly string defaultImagePath;

        public FriendProfileForm(string friendId, string nick)
        {
            InitializeComponent();
            this.friendId = friendId;
            this.friendName = nick;

            // 기본 이미지 경로(Application 실행 폴더)
            defaultImagePath = Path.Combine(Application.StartupPath, "default_profile.png");

            label1.Text = friendName;
            label2.Text = friendId;

            // 생성 시점에 기본 이미지 세팅
            if (File.Exists(defaultImagePath))
            {
                pictureBoxFriend.Image = Image.FromFile(defaultImagePath);
            }
            pictureBoxFriend.SizeMode = PictureBoxSizeMode.Zoom;

            // Load 이벤트 연결
            this.Load += FriendProfileForm_Load;

            // FriendProfileForm.cs – 생성자 또는 Load 이벤트 이후에 추가
            btnDelete.Click += async (sender, e) =>
            {
                btnDelete.Enabled = false;
                // opcode 7: friend_delete(친구 삭제)
                var items = new List<string> { friendId };
                var result = await LoginForm.GlobalDCM.db_request_data(7, items); // :contentReference[oaicite:0]{index=0}

                if (result.Key && result.Value.Item2.Count > 0)
                {
                    int key = result.Value.Item1;
                    var indexes = result.Value.Item2;
                    string response = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());
                    LoginForm.ClearGlobalDCMReceivedData(key);

                    if (response == "1")
                    {
                        MessageBox.Show("친구가 삭제되었습니다.", "삭제 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("친구 삭제에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("서버와 통신에 실패했습니다.", "통신 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                btnDelete.Enabled = true;
            };

        }

        private async void FriendProfileForm_Load(object sender, EventArgs e)
        {
            await LoadFriendImageAsync(friendId);
        }


        private async Task LoadFriendImageAsync(string friendId)
        {
            string imageDir = Path.Combine(Application.StartupPath, "FriendImages");
            if (!Directory.Exists(imageDir))
                Directory.CreateDirectory(imageDir);
            string localPath = Path.Combine(imageDir, $"{friendId}.jpg");

            // 서버에서 이미지 다운로드 시도
            var items = new List<string> { "0", friendId, localPath };
            var result = await LoginForm.GlobalDCM.db_request_data(15, items);

            if (result.Key && File.Exists(localPath))
            {
                // 성공적으로 불러왔으면 실제 이미지로 교체
                pictureBoxFriend.Image = Image.FromFile(localPath);
                pictureBoxFriend.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                // 실패 시 기본 이미지를 유지 (이미 생성자에서 세팅됨)
                pictureBoxFriend.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }
        private void pictureBoxFriend_Click(object sender, EventArgs e)
        {

        }


    }
}
