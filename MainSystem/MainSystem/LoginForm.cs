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

            var requestBody = new List<string> { inputID, inputPW };
            var result = await dcm.db_request_data(0x02, requestBody); // 로그인 부분

            bool success = result.Key;
            int key = result.Value.Item1;
            List<int> indices = result.Value.Item2;

            if (success)
            {
                string responseStr;

                responseStr = GetRawResponse(dcm, key, indices[0]);

                if (responseStr == "0")
                {
                    MessageBox.Show("로그인 실패. 아이디 또는 비밀번호를 확인하세요.");
                    
                }
                else
                {
                    MainForm mainForm = new MainForm(dcm);
                    mainForm.Show();
                    this.Hide();

                    mainForm.InitializeAfterLogin(inputID);
                }
            }
            else
            {
                MessageBox.Show("서버와 통신에 실패했습니다.");
            }
        }

        private string GetRawResponse(DCM dcm, int key, int index)
        {
            var field = typeof(DCM).GetField("received_data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var data = field?.GetValue(dcm) as Dictionary<int, List<string>>;
            return data?[key][index];
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
    }
}

