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
    public partial class ProfileForm : Form
    {
        private string id;
        private string name;
        private readonly string defaultImagePath;

        public ProfileForm(string id, string nick)
        {
            InitializeComponent();
            this.id = id;
            this.name = nick;

            // 기본 이미지 경로(Application 실행 폴더)
            defaultImagePath = Path.Combine(Application.StartupPath, "default_profile.png");

            label1.Text = name;
            label2.Text = id;

            // 생성 시점에 기본 이미지 세팅
            if (File.Exists(defaultImagePath))
            {
                pictureBox1.Image = Image.FromFile(defaultImagePath);
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            // Load 이벤트 연결
            this.Load += FriendProfileForm_Load;

            button1.Click += async (sender, e) =>
            {
                // 새 닉네임 입력 폼 띄우기
                using (var nicknameForm = new NicknameInputForm())
                {
                    if (nicknameForm.ShowDialog() == DialogResult.OK)
                    {
                        string newNick = nicknameForm.NewNickname.Trim();
                        button1.Enabled = false;

                        // opcode 4: change_nickname { new_nickname }
                        var items = new List<string> { newNick };
                        var result = await LoginForm.GlobalDCM.db_request_data(4, items); // :contentReference[oaicite:0]{index=0}

                        if (result.Key && result.Value.Item2.Count > 0)
                        {
                            int key = result.Value.Item1;
                            int lastIdx = result.Value.Item2.Last();
                            string response = LoginForm.GetGlobalDCMResponseData(key, lastIdx);
                            LoginForm.ClearGlobalDCMReceivedData(key);

                            if (response == "1")
                            {
                                MessageBox.Show("닉네임이 성공적으로 변경되었습니다.", "완료",
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                                label1.Text = newNick;
                            }
                            else
                            {
                                MessageBox.Show("닉네임 변경에 실패했습니다.", "오류",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("서버 통신에 실패했습니다.", "통신 오류",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        button1.Enabled = true;
                    }
                }
            };

            // ProfileForm.cs – “이미지 변경” 버튼 클릭 핸들러 추가
            button2.Click += async (sender, e) =>
            {
                // 1) 이미지 선택 폼 띄우기
                using (var uploadForm = new ImageUploadForm())
                {
                    if (uploadForm.ShowDialog() != DialogResult.OK)
                        return;

                    string selectedPath = uploadForm.SelectedImagePath;
                    button2.Enabled = false;

                    // 2) 서버에 이미지 업로드 (opcode 16: upload_image)
                    var items = new List<string> { "0", id, selectedPath };
                    var result = await LoginForm.GlobalDCM.db_request_data(16, items); // :contentReference[oaicite:0]{index=0}

                    if (result.Key && result.Value.Item2.Count > 0)
                    {
                        int keyIndex = result.Value.Item1;
                        int lastIdx = result.Value.Item2.Last();
                        string response = LoginForm.GetGlobalDCMResponseData(keyIndex, lastIdx);
                        LoginForm.ClearGlobalDCMReceivedData(keyIndex);

                        if (response != "0")
                        {
                            MessageBox.Show("프로필 이미지가 성공적으로 업데이트되었습니다.",
                                            "업데이트 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // 3) PictureBox 갱신
                            pictureBox1.Image = Image.FromFile(selectedPath);
                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            MessageBox.Show("이미지 업로드에 실패했습니다.",
                                            "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("서버와 통신에 실패했습니다.",
                                        "통신 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    button2.Enabled = true;
                }
            };

        }

        private async void FriendProfileForm_Load(object sender, EventArgs e)
        {
            await LoadFriendImageAsync(id);
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
                pictureBox1.Image = Image.FromFile(localPath);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                // 실패 시 기본 이미지를 유지 (이미 생성자에서 세팅됨)
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}

