using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MainSystem
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        public void InitializeAfterLogin(string code)
        {
            string loggedInCode = code;
            LoadFriendList(loggedInCode, flpMain);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void rdbFriend_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn1_Click(object sender, EventArgs e)
        {

        }

        private const string UserDBPath = "UserDB.txt";

        void LoadFriendList(string loggedInUserCode, FlowLayoutPanel flowLayoutPanel)
        {
            string filePath = loggedInUserCode + ".txt";

            // [1] 코드 기반 파일이 없으면 생성
            if (!File.Exists(filePath))
            {
                File.Create(filePath);

                // [2] UserDB.txt에서 유저 정보 검색
                if (!File.Exists(UserDBPath))
                {
                    MessageBox.Show("UserDB가 없습니다. 파일이 필요합니다.");
                    return;
                }

                var userLines = File.ReadAllLines(UserDBPath);
                string matchedLine = userLines.FirstOrDefault(line =>
                {
                    var parts = line.Split(',');
                    return parts.Length >= 6 && parts[3] == loggedInUserCode;
                });

                if (matchedLine != null)
                {
                    File.WriteAllText(filePath, matchedLine + Environment.NewLine);
                }
                else
                {
                    return;
                }
            }

            // [3] 파일 읽기
            var allLines = File.ReadAllLines(filePath).ToList();
            if (allLines.Count == 0) return;

            // [4] 닉네임 매핑
            Dictionary<string, string> idToNick = new Dictionary<string, string>();

            // [5] 친구 목록
            var friendIds = allLines.ToList();

            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.AutoScroll = true;

            foreach (var friendId in friendIds)
            {
                string friendNick = "(정보 없음)";
                string friendImagePath = "default_profile.png"; // 기본 프로필 경로 (이미지 없을 경우)

                // 닉네임 찾기
                foreach (var line in File.ReadAllLines(UserDBPath))
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 6 && parts[0] == friendId)
                    {
                        friendNick = parts[5];
                        break;
                    }
                }

                // 하나의 패널에 PictureBox + Button 넣기
                Panel friendPanel = new Panel
                {
                    Width = flowLayoutPanel.ClientSize.Width - 30, // 스크롤바 여백 감안
                    Height = 50,
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.FixedSingle
                };

                // 이미지 박스
                PictureBox pic = new PictureBox
                {
                    Size = new Size(50, 50),
                    Location = new Point(0, 0),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = Image.FromFile(friendImagePath)
                };

                // 버튼
                Button btn = new Button
                {
                    Text = friendNick,
                    Font = new Font("맑은 고딕", 12),
                    Width = friendPanel.Width - 55,
                    Height = 40,
                    Location = new Point(50, 5),
                    BackColor = Color.LightBlue,
                    Tag = friendNick
                };

                btn.Click += (sender, e) =>
                {
                    var clickedButton = sender as Button;
                    string selectedFriendNick = clickedButton.Tag.ToString();
                    MessageBox.Show($"'{clickedButton.Text}' 클릭됨 (NickName: {selectedFriendNick})");
                };

                // 구성요소 패널에 추가
                friendPanel.Controls.Add(pic);
                friendPanel.Controls.Add(btn);

                // 전체 패널에 추가
                flowLayoutPanel.Controls.Add(friendPanel);
            }


        }

        private void flpMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
