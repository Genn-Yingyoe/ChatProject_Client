using ChatMoa_DataBaseServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainSystem
{
    public partial class ConfigForm : Form
    {
        private string userId;
        private string nick;

        public ConfigForm(string userId, string nick)
        {
            InitializeComponent();
            this.userId = userId;
            this.nick = nick;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var profileForm = new ProfileForm(this.userId, this.nick))
            {
                profileForm.ShowDialog();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoginForm.LoggedInUserId = "";
            LoginForm.LoggedInUserName = "";
            LoginForm.LoggedInUserNickname = "";  // 필요하다면 추가로

            // 2) DCM 내부 상태 리셋 (private Logdout 호출) :contentReference[oaicite:0]{index=0}
            var logoutM = typeof(DCM)
                          .GetMethod("Logdout", BindingFlags.NonPublic | BindingFlags.Instance);
            logoutM?.Invoke(LoginForm.GlobalDCM, null);

            // 3) 로그인 폼 띄우기
            var loginForm = new LoginForm();
            loginForm.Show();

            // 4) 나머지 모든 폼 닫기 (ConfigForm 포함)
            foreach (Form frm in Application.OpenForms.Cast<Form>().ToList())
            {
                if (frm != loginForm)
                    frm.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
        "테마 기능은 현재 개발 중입니다.",   // 메시지
        "개발 중",                        // 제목
        MessageBoxButtons.OK,             // 버튼
        MessageBoxIcon.Information        // 아이콘
    );
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var lost = new LostPW(LoginForm.login_ID))
            {
                if (lost.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("비밀번호가 변경되었습니다.", "완료",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
