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
using MainSystem;

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

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string inputID = txtID.Text.Trim();
            string inputPW = txtPW.Text.Trim();

            var dcm = new DCM();

            // 서버에 로그인 요청
            var requestBody = new List<string> { inputID, inputPW };
            var result = await dcm.db_request_data(0x01, requestBody);  // 예: opcode 0x01 = 로그인

            bool success = result.Key;
            int key = result.Value.Item1;
            List<int> indices = result.Value.Item2;

            if (success)
            {
                // 서버 응답에서 첫 번째 문자열을 꺼내기
                string responseStr = dcm.DeSerializeJson<string>(key, indices[0]);

                if (responseStr == "1")  // 로그인 성공
                {
                    dcm.Login(inputID);  // DCM 인스턴스에 유저 정보 저장

                    MainForm mainForm = new MainForm(dcm);  // dcm 전달
                    mainForm.Show();
                    this.Hide();

                    mainForm.InitializeAfterLogin(inputID);
                }
                else
                {
                    MessageBox.Show("로그인 실패. 아이디 또는 비밀번호를 확인하세요.");
                }
            }
            else
            {
                MessageBox.Show("서버와 통신에 실패했습니다.");
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

        private void pbApp_Click(object sender, EventArgs e)
        {

        }
    }
}

