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
using System.Reflection;
using ChatMoa_DataBaseServer;

namespace MainSystem
{
    public partial class LostPW : Form
    {
        private readonly DCM dcm = new DCM();   // DB 통신 모듈

        public LostPW()
        {
            InitializeComponent();
            txtPW.UseSystemPasswordChar = true;
            txtQ.ReadOnly = true;   // (id 입력 후 자동 채움용)
            // 비밀번호 질문을 textBox2에 표시 (첫 번째 질문으로 기본 설정)
            txtQ.Text = passwordQuestions[0];
            txtQ.Tag = 0; // 질문 인덱스 저장
            txtQ.ReadOnly = true;
            txtQ.BackColor = SystemColors.Control;

            // 질문 선택 버튼 추가 (textBox2 옆에)
            Button btnSelectQuestion = new Button();
            btnSelectQuestion.Text = "▼";
            btnSelectQuestion.Size = new Size(25, txtQ.Height);
            btnSelectQuestion.Location = new Point(txtQ.Right - 25, txtQ.Top);
            btnSelectQuestion.Click += BtnSelectQuestion_Click;
            this.Controls.Add(btnSelectQuestion);
            btnSelectQuestion.BringToFront();
        }

        private void BtnSelectQuestion_Click(object sender, EventArgs e)
        {
            // 간단한 질문 선택 다이얼로그
            using (Form questionForm = new Form())
            {
                questionForm.Text = "비밀번호 찾기 질문 선택";
                questionForm.Size = new Size(350, 200);
                questionForm.StartPosition = FormStartPosition.CenterParent;

                ListBox listBox = new ListBox();
                listBox.Dock = DockStyle.Top;
                listBox.Height = 120;
                listBox.Items.AddRange(passwordQuestions);
                listBox.SelectedIndex = (int)(txtQ.Tag ?? 0);

                Button btnOK = new Button();
                btnOK.Text = "확인";
                btnOK.DialogResult = DialogResult.OK;
                btnOK.Location = new Point(135, 130);

                questionForm.Controls.Add(listBox);
                questionForm.Controls.Add(btnOK);

                if (questionForm.ShowDialog() == DialogResult.OK && listBox.SelectedIndex >= 0)
                {
                    txtQ.Text = passwordQuestions[listBox.SelectedIndex];
                    txtQ.Tag = listBox.SelectedIndex;
                }
            }
        }

        private string[] passwordQuestions = new string[]
        {
            "가장 좋아하는 음식은?",
            "첫 번째 애완동물의 이름은?",
            "어머니의 성함은?",
            "출신 초등학교는?",
            "가장 좋아하는 색깔은?"
        };


        private async void btnOK_Click(object sender, EventArgs e)
        {
            // -------- (1) 입력값 수집 & 1차 검증 --------
            string id = txtID.Text.Trim();
            string qIndex = txtQ.Tag.ToString().Trim();
            string answer = txtA.Text.Trim();
            string newPW = txtPW.Text.Trim();

            // -------- (2) 서버 요청 준비 --------
            btnOK.Enabled = false; btnOK.Text = "Processing...";
            var items = new List<string> { id, qIndex.ToString(), answer, newPW };

            try
            {
                bool success = await ChangePasswordWithDCM(items);
                if (success)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                btnOK.Enabled = true; btnOK.Text = "OK";
            }
        }

        private async Task<bool> ChangePasswordWithDCM(List<string> items)
        {
            // opcode 3 : change_password { id, ps_question_index, ps_question_answer, new_ps }
            var result = await dcm.db_request_data(3, items);

            if (result.Key && result.Value.Item2.Count > 0)
            {
                int key = result.Value.Item1;
                var lastIdx = result.Value.Item2.Last();
                string response = GetDCMResponseData(dcm, key, lastIdx);
                ClearDCMReceivedData(dcm, key);
                return response == "1";
            }
            return false;
        }

        private string GetDCMResponseData(DCM d, int key, int idx)
        {
            var f = d.GetType().GetField("received_data",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            if (f?.GetValue(d) is Dictionary<int, List<string>> dic &&
                dic.TryGetValue(key, out var list) && idx < list.Count)
                return list[idx];
            return "0";
        }
        private void ClearDCMReceivedData(DCM d, int key)
        {
            var m = d.GetType().GetMethod("Clear_receive_data",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            m?.Invoke(d, new object[] { key });
        }
    }
}


