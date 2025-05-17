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

                // friendId에 해당하는 닉네임 찾기
                foreach (var line in File.ReadAllLines(UserDBPath))
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 6 && parts[0] == friendId)
                    {
                        friendNick = parts[5];
                        break;
                    }
                }

                Label lbl = new Label
                {
                    Text = friendNick,
                    Font = new Font("맑은 고딕", 12),
                    BackColor = Color.LightBlue,
                    Padding = new Padding(5),
                    Margin = new Padding(5),
                    AutoSize = true
                };

                flowLayoutPanel.Controls.Add(lbl);
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
