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

namespace Login___Signup
{
    public partial class LoginForm : Form
    {
        private string version = "0.0.1";   // 버전 정보
        //0.0 -> 기본 로그인 기능 및 회원가입 기능
        //0.1 -> 메인 대화창 연결 및 데이터 %appdata% 경로로 이동
        //0.2 -> 로그인 및 회원가입 기능 개선
        //0.3 -> Server 연결

        public string code = string.Empty;

        private const string UserDBPath = "UserDB.txt";
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            SignupForm signupForm = new SignupForm();
            signupForm.ShowDialog();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string inputID = txtID.Text.Trim();
            string inputPW = txtPW.Text.Trim();

            if (!File.Exists(UserDBPath))
            {
                MessageBox.Show("UserDB.txt 파일이 없습니다.");
                return;
            }

            var lines = File.ReadAllLines(UserDBPath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length < 6) continue; // 데이터가 부족한 경우 무시

                if (parts[0] == inputID && parts[1] == inputPW) // 여기에 메인 대화창을 띄울 코드를 넣을것
                {
                    로그인 성공 시 메인 대화창을 띄우는 코드
                    MainChatForm mainForm = new MainForm();
                    MainChatForm.Show();
                    this.Hide();
                }

                MessageBox.Show("로그인 실패. 아이디 또는 비밀번호를 확인하세요.");
            }
        }
    }
}

