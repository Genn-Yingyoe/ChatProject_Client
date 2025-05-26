using MainSystem;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;

namespace MainSystem
{
    public partial class LoginForm : Form
    {
        public const int EM_SETRECT = 0xB3;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref RECT lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private void SetTextBoxPadding(System.Windows.Forms.TextBox tb, int left, int top, int right, int bottom)
        {
            RECT rect = new RECT
            {
                Left = left,
                Top = top,
                Right = tb.ClientSize.Width - right,
                Bottom = tb.ClientSize.Height - bottom
            };

            SendMessage(tb.Handle, EM_SETRECT, IntPtr.Zero, ref rect);
        }

        public static string code = "";

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
                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                    this.Hide();

                    mainForm.InitializeAfterLogin(parts[2]); // 로그인 후 초기화 호출
                }
                else if (parts[0] == inputID && !(parts[1] == inputPW))
                {
                    MessageBox.Show("로그인 실패. 아이디 또는 비밀번호를 확인하세요.");
                }
                else
                {

                }
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void txtID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                btnLogin.PerformClick();
            }
        }

        private void txtPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                btnLogin.PerformClick();
            }
        }

        private void txtPW_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}

