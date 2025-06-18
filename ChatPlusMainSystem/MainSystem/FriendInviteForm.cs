using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using ChatMoa_DataBaseServer;

namespace MainSystem
{
    public partial class FriendInviteForm : Form
    {
        public string CurrentUserId { get; set; }

        private TextBox txtFriendId;
        private Button btnSearch;
        private Button btnInvite;
        private Label lblResult;
        private Panel pnlResult;
        private Label lblFriendInfo;

        public FriendInviteForm()
        {
            InitializeCustomComponents(); // 메서드 이름 변경
        }

        private void InitializeCustomComponents() // 메서드 이름 변경
        {
            this.Text = "친구 초대";
            this.Size = new Size(350, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(200, 202, 224);

            // 제목 라벨
            Label lblTitle = new Label
            {
                Text = "친구 초대",
                Font = new Font("맑은 고딕", 16, FontStyle.Bold),
                Size = new Size(200, 30),
                Location = new Point(75, 20),
                ForeColor = Color.FromArgb(41, 47, 102)
            };

            // 친구 ID 입력 라벨
            Label lblFriendId = new Label
            {
                Text = "친구 ID:",
                Font = new Font("맑은 고딕", 10),
                Size = new Size(60, 20),
                Location = new Point(30, 70),
                ForeColor = Color.FromArgb(41, 47, 102)
            };

            // 친구 ID 입력 텍스트박스
            txtFriendId = new TextBox
            {
                Font = new Font("맑은 고딕", 10),
                Size = new Size(150, 25),
                Location = new Point(95, 68),
                MaxLength = 6
            };

            // 검색 버튼
            btnSearch = new Button
            {
                Text = "검색",
                Font = new Font("맑은 고딕", 9),
                Size = new Size(60, 27),
                Location = new Point(250, 67),
                BackColor = Color.FromArgb(41, 47, 102),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;

            // 결과 패널
            pnlResult = new Panel
            {
                Size = new Size(280, 60),
                Location = new Point(30, 105),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                BackColor = Color.White
            };

            // 친구 정보 라벨
            lblFriendInfo = new Label
            {
                Font = new Font("맑은 고딕", 10),
                Size = new Size(270, 50),
                Location = new Point(5, 5),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlResult.Controls.Add(lblFriendInfo);

            // 초대 버튼
            btnInvite = new Button
            {
                Text = "친구 초대",
                Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                Size = new Size(100, 35),
                Location = new Point(125, 175),
                BackColor = Color.FromArgb(41, 47, 102),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            btnInvite.FlatAppearance.BorderSize = 0;
            btnInvite.Click += BtnInvite_Click;

            // 컨트롤 추가
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblFriendId);
            this.Controls.Add(txtFriendId);
            this.Controls.Add(btnSearch);
            this.Controls.Add(pnlResult);
            this.Controls.Add(btnInvite);

            // Enter 키 이벤트
            txtFriendId.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.PerformClick();
                }
            };
        }

        private string searchedFriendId = "";
        private string searchedFriendNickname = "";

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string friendId = txtFriendId.Text.Trim();

            if (string.IsNullOrEmpty(friendId))
            {
                MessageBox.Show("친구 ID를 입력해주세요.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (friendId.Length != 6 || !friendId.All(char.IsDigit))
            {
                MessageBox.Show("친구 ID는 6자리 숫자여야 합니다.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnSearch.Enabled = false;
                btnSearch.Text = "검색 중...";

                var result = await SearchUser(friendId);

                if (result.found)
                {
                    searchedFriendId = result.userId;
                    searchedFriendNickname = result.nickname;

                    lblFriendInfo.Text = $"ID: {result.userId}\n닉네임: {result.nickname}";
                    pnlResult.Visible = true;
                    btnInvite.Visible = true;
                }
                else
                {
                    MessageBox.Show("존재하지 않는 사용자입니다.", "검색 결과",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    pnlResult.Visible = false;
                    btnInvite.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"검색 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSearch.Enabled = true;
                btnSearch.Text = "검색";
            }
        }

        // SearchUser 메서드 - DCM 사용하도록 수정
        private async Task<(bool found, string userId, string nickname)> SearchUser(string friendId)
        {
            try
            {
                // opcode 13: user_id_search
                List<string> items = new List<string> { friendId };
                var result = await LoginForm.GlobalDCM.db_request_data(13, items);

                if (result.Key && result.Value.Item2.Count >= 2)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        // 첫 번째 응답은 친구 정보
                        var friendInfo = LoginForm.DeserializeGlobalDCMJson<FriendInfo>(key, indexes[0]);
                        if (!string.IsNullOrEmpty(friendInfo.Friend_Id))
                        {
                            LoginForm.ClearGlobalDCMReceivedData(key);
                            return (true, friendInfo.Friend_Id, friendInfo.Nickname);
                        }
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }

                return (false, "", "");
            }
            catch
            {
                throw;
            }
        }

        private async void BtnInvite_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchedFriendId))
            {
                MessageBox.Show("먼저 친구를 검색해주세요.", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (searchedFriendId == CurrentUserId)
            {
                MessageBox.Show("자기 자신은 친구로 추가할 수 없습니다.", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnInvite.Enabled = false;
                btnInvite.Text = "초대 중...";

                bool success = await SendFriendRequest(searchedFriendId);

                if (success)
                {
                    MessageBox.Show($"{searchedFriendNickname}님에게 친구 요청을 보냈습니다.", "성공",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("친구 요청 전송에 실패했습니다.", "오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"친구 초대 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnInvite.Enabled = true;
                btnInvite.Text = "친구 초대";
            }
        }

        // SendFriendRequest 메서드 - DCM 사용하도록 수정
        private async Task<bool> SendFriendRequest(string friendId)
        {
            try
            {
                // opcode 6: friend_request
                List<string> items = new List<string> { friendId };
                var result = await LoginForm.GlobalDCM.db_request_data(6, items);

                if (result.Key && result.Value.Item2.Count > 0)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string response = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());
                    LoginForm.ClearGlobalDCMReceivedData(key);

                    return response == "1";
                }

                return false;
            }
            catch
            {
                throw;
            }
        }

        // FriendInfo 데이터 구조
        [DataContract]
        private class FriendInfo
        {
            [DataMember] internal string Friend_Id;
            [DataMember] internal string Nickname;
        }
    }
}