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
        private readonly string friendId;
        private readonly string friendName;
        private readonly string friendImagePath;
        private readonly string defaultImagePath;

        public FriendProfileForm(string friendId, string nick, string imagePath)
        {
            InitializeComponent();
            this.friendId = friendId;
            this.friendName = nick;
            this.friendImagePath = imagePath;
            this.defaultImagePath = Path.Combine(Application.StartupPath, "default_profile.png");

            label1.Text = friendName;
            label2.Text = friendId;

            // 생성 시점에 기본 이미지 세팅
            if (!string.IsNullOrEmpty(friendImagePath) && File.Exists(friendImagePath))
                pictureBoxFriend.Image = Image.FromFile(friendImagePath);
            else if (File.Exists(defaultImagePath))
                pictureBoxFriend.Image = Image.FromFile(defaultImagePath);

            pictureBoxFriend.SizeMode = PictureBoxSizeMode.Zoom;

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

            // 기존 파일 삭제해서 항상 최신 이미지 받기
            if (File.Exists(localPath))
            {
                try
                {
                    File.Delete(localPath);
                }
                catch { /* 삭제 못해도 계속 진행 */ }
            }

            // 서버에서 이미지 다운로드 시도
            var items = new List<string> { "0", friendId, localPath };
            var result = await LoginForm.GlobalDCM.db_request_data(15, items);

            if (result.Key && File.Exists(localPath))
            {
                // 이전에 PictureBox에 로드된 이미지 메모리 해제
                if (pictureBoxFriend.Image != null)
                {
                    pictureBoxFriend.Image.Dispose();
                    pictureBoxFriend.Image = null;
                }

                // 스트림으로 로드해서 파일 잠금 방지
                using (var fs = new FileStream(localPath, FileMode.Open, FileAccess.Read))
                {
                    pictureBoxFriend.Image = Image.FromStream(fs);
                }
                pictureBoxFriend.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                // 다운로드 실패 시 기본 이미지 유지
                pictureBoxFriend.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }


        private void pictureBoxFriend_Click(object sender, EventArgs e)
        {

        }
        private async void btnChat_Click(object sender, EventArgs e)
        {
            try
            {
                // 버튼 비활성화 및 로딩 표시
                btnChat.Enabled = false;
                btnChat.Text = "채팅방 생성 중...";
                this.Cursor = Cursors.WaitCursor;

                // opcode 32: 채팅방 만들기 (친구 1명 초대)
                List<string> items = new List<string> { friendId };
                var result = await LoginForm.GlobalDCM.db_request_data(32, items);

                if (result.Key && result.Value.Item2.Count >= 2)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        // 채팅방 ID 가져오기
                        string roomId = LoginForm.GetGlobalDCMResponseData(key, indexes[indexes.Count - 2]);

                        // 성공 메시지
                        MessageBox.Show($"{friendName}님과의 채팅방이 생성되었습니다!\n채팅방으로 이동합니다.", "성공",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 채팅방 열기
                        OpenChatRoom(roomId);

                        // 프로필 창 닫기
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"{friendName}님과의 채팅방 생성에 실패했습니다.", "오류",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
                else
                {
                    MessageBox.Show("서버와 통신에 실패했습니다.", "통신 오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"채팅방 생성 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 버튼 복원
                btnChat.Enabled = true;
                btnChat.Text = "채팅하기";
                this.Cursor = Cursors.Default;
            }
        }

        private void OpenChatRoom(string roomId)
        {
            try
         {
                // 이미 열린 채팅방이 있는지 확인
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm is ChatForm chatForm && chatForm.RoomId == roomId)
                    {
                        chatForm.BringToFront();
                        chatForm.Activate();
                        return;
                    }
                }

                // 새 채팅방 열기
                ChatForm newChatForm = new ChatForm();

                // 현재 사용자 정보 가져오기
                string currentUserId = LoginForm.LoggedInUserId;
                string currentUserName = LoginForm.LoggedInUserName;

                // 닉네임이 없으면 이름 사용
                string displayName = !string.IsNullOrEmpty(currentUserName) ? currentUserName : currentUserId;
        
                // 채팅방 초기화 및 열기
                newChatForm.InitializeChat(currentUserId, displayName, roomId);
                newChatForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"채팅방 열기 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
