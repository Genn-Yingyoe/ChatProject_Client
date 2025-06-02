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
using System.Xml.Linq;

namespace MainSystem
{
    public partial class SignupForm : Form
    {

        private const string UserDBPath = "UserDB.txt";
        private Random random = new Random();

        public SignupForm()
        {
            InitializeComponent();
        }

        private void SignupForm_Load(object sender, EventArgs e)
        {
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            string newID = txtNewID.Text.Trim();
            string newPW = txtNewPW.Text.Trim();
            string newName = txtName.Text.Trim();
            string newNick = txtNick.Text.Trim();
            string PWQ = "0";//txtPWQ.Text.Trim();
            string PWA = txtPWA.Text.Trim();

            if (string.IsNullOrEmpty(newID) || string.IsNullOrEmpty(newPW) ||
                string.IsNullOrEmpty(newName) || string.IsNullOrEmpty(newNick))
            {
                MessageBox.Show("모든 항목을 입력해주세요.");
                return;
            }

            // TCP 기반 회원가입 요청
            var dcm = new DCM();

            var requestBody = new List<string> { newID, newPW, PWQ ,PWA, newName, newNick };

            var result = await dcm.db_request_data(0x01, requestBody);

            bool success = result.Key;
            int key = result.Value.Item1;
            List<int> indices = result.Value.Item2;

            if (success)
            {
                string responseStr;

             responseStr = GetRawResponse(dcm, key, indices[0]);

                if (responseStr == "1")
                {
                    MessageBox.Show("회원가입 성공!");
                    this.Close();
                }
                else if (responseStr == "0")
                {
                    MessageBox.Show("회원가입 실패: 중복된 ID 또는 닉네임입니다.");
                }
                else
                {
                    MessageBox.Show("알 수 없는 서버 응답: " + responseStr);
                }
            }
            else
            {
                MessageBox.Show("서버와 통신에 실패했습니다.");
            }
        }

        // DCM의 내부 received_data 접근 (private이므로 리플렉션 사용)
        private string GetRawResponse(DCM dcm, int key, int index)
        {
            var field = typeof(DCM).GetField("received_data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var data = field?.GetValue(dcm) as Dictionary<int, List<string>>;
            return data?[key][index];
        }


        private void txtNewID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNewPW_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
