using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using ChatMoa_DataBaseServer;
using System.Text;

namespace MainSystem
{
    public partial class ChatForm : Form
    {
        private string currentUserId;
        private string currentUserName;
        private string roomId;
        private int lastReadMsgId = -1;
        private string lastReadDate = "";
        private Timer refreshTimer;
        private bool isLoading = false;

        // 닉네임 캐시 (User_Id -> Nickname)
        private Dictionary<string, string> nicknameCache = new Dictionary<string, string>();

        // Room ID에 접근할 수 있는 공개 속성
        public string RoomId => roomId;

        public ChatForm()
        {
            InitializeComponent();
            InitializeUI();
            InitializeTimer();
        }

        private void InitializeUI()
        {
            this.BackColor = Color.FromArgb(200, 202, 224);

            // 채팅 입력 영역 스타일
            txtMessage.BackColor = Color.White;
            txtMessage.BorderStyle = BorderStyle.FixedSingle;
            txtMessage.Font = new Font("맑은 고딕", 10);

            // 전송 버튼 스타일
            btnSend.BackColor = Color.FromArgb(41, 47, 102);
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Font = new Font("맑은 고딕", 10, FontStyle.Bold);

            // 초대 버튼 스타일
            btnInvite.BackColor = Color.FromArgb(220, 120, 50);
            btnInvite.ForeColor = Color.White;
            btnInvite.FlatStyle = FlatStyle.Flat;
            btnInvite.FlatAppearance.BorderSize = 0;
            btnInvite.Font = new Font("맑은 고딕", 9, FontStyle.Bold);

            // 채팅 목록 영역 스타일
            flpChatList.BackColor = Color.White;
            flpChatList.BorderStyle = BorderStyle.FixedSingle;
            flpChatList.AutoScroll = true;
            flpChatList.FlowDirection = FlowDirection.TopDown;
            flpChatList.WrapContents = false;
        }

        private void InitializeTimer()
        {
            refreshTimer = new Timer();
            refreshTimer.Interval = 2000; // 2초마다 새 메시지 확인
            refreshTimer.Tick += async (s, e) => await LoadNewMessages();
            refreshTimer.Start();
        }

        public void InitializeChat(string userId, string userName, string chatRoomId)
        {
            currentUserId = userId;
            currentUserName = userName;
            roomId = chatRoomId;

            this.Text = $"채팅방 - {roomId}";
            lblRoomInfo.Text = $"채팅방 ID: {roomId}";

            // 초기 채팅 로드
            LoadInitialMessages();
        }

        // 닉네임 가져오기 및 캐시 - DCM 사용하도록 수정
        private async Task<string> GetUserNicknameAsync(string userId)
        {
            // 관리자 메시지는 "시스템"으로 표시
            if (userId == "000000")
            {
                return "시스템";
            }

            // 자신의 메시지는 캐시하지 않고 바로 반환
            if (userId == currentUserId)
            {
                return currentUserName;
            }

            // 캐시에서 먼저 확인
            if (nicknameCache.ContainsKey(userId))
            {
                return nicknameCache[userId];
            }

            // DCM을 사용하여 서버에서 닉네임 조회
            try
            {
                // opcode 13: user_id_search
                List<string> items = new List<string> { userId };
                var result = await LoginForm.GlobalDCM.db_request_data(13, items);

                if (result.Key && result.Value.Item2.Count >= 2)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        var friendInfo = LoginForm.DeserializeGlobalDCMJson<FriendInfo>(key, indexes[0]);
                        if (!string.IsNullOrEmpty(friendInfo.Friend_Id))
                        {
                            // 캐시에 저장
                            nicknameCache[userId] = friendInfo.Nickname;
                            LoginForm.ClearGlobalDCMReceivedData(key);
                            return friendInfo.Nickname;
                        }
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"닉네임 조회 오류: {ex.Message}");
            }

            // 실패 시 User ID 반환 (기본값)
            return $"User {userId}";
        }

        // LoadInitialMessages 메서드 - DCM 사용하도록 수정
        private async void LoadInitialMessages()
        {
            if (isLoading) return;
            isLoading = true;

            try
            {
                // opcode 37: 채팅 읽기 (초기 로드: 최근 3일치)
                List<string> items = new List<string>
                {
                    roomId,
                    "-1", // 초기 로드
                    DateTime.Now.ToString("yyyyMMdd") // 오늘 날짜
                };

                var result = await LoginForm.GlobalDCM.db_request_data(37, items);

                if (result.Key)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    List<string> responses = new List<string>();

                    for (int i = 0; i < indexes.Count; i++)
                    {
                        responses.Add(LoginForm.GetGlobalDCMResponseData(key, indexes[i]));
                    }

                    await ProcessChatMessages(responses);
                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"메시지 로드 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoading = false;
            }
        }

        // LoadNewMessages 메서드 - DCM 사용하도록 수정
        private async Task LoadNewMessages()
        {
            if (isLoading || lastReadMsgId == -1) return;
            isLoading = true;

            try
            {
                // opcode 37: 채팅 읽기 (이어서 읽기)
                List<string> items = new List<string>
                {
                    roomId,
                    lastReadMsgId.ToString(),
                    lastReadDate
                };

                var result = await LoginForm.GlobalDCM.db_request_data(37, items);

                if (result.Key)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    List<string> responses = new List<string>();

                    for (int i = 0; i < indexes.Count; i++)
                    {
                        responses.Add(LoginForm.GetGlobalDCMResponseData(key, indexes[i]));
                    }

                    await ProcessChatMessages(responses);
                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
            }
            catch (Exception ex)
            {
                // 타이머에서 발생하는 오류는 조용히 처리
                Console.WriteLine($"메시지 새로고침 오류: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task ProcessChatMessages(List<string> responses)
        {
            if (responses.Count == 0) return;

            string lastResponse = responses.Last();
            if (lastResponse == "0") return; // 실패
            if (lastResponse == "1" && responses.Count == 1) return; // 새 메시지 없음

            // "00000000"이면 읽을 메시지가 없음
            if (responses.Count == 2 && responses[0] == "00000000") return;

            // 메시지 처리
            string currentDate = "";
            for (int i = 0; i < responses.Count - 1; i++)
            {
                string response = responses[i];

                // 날짜인지 확인 (8자리 숫자)
                if (response.Length == 8 && response.All(char.IsDigit))
                {
                    currentDate = response;
                    continue;
                }

                // 메시지 데이터 처리
                try
                {
                    var chatMessage = DeserializeChatMessage(response);
                    await DisplayChatMessage(chatMessage, currentDate);

                    // 마지막 읽은 메시지 정보 업데이트
                    if (chatMessage.Msg_Id > lastReadMsgId)
                    {
                        lastReadMsgId = chatMessage.Msg_Id;
                        lastReadDate = currentDate;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"메시지 파싱 오류: {ex.Message}");
                }
            }

            // 스크롤을 맨 아래로
            if (flpChatList.Controls.Count > 0)
            {
                flpChatList.ScrollControlIntoView(flpChatList.Controls[flpChatList.Controls.Count - 1]);
            }
        }

        private async Task DisplayChatMessage(ChatMessage message, string date)
        {
            // 중복 메시지 체크 (이미 표시된 메시지인지 확인)
            foreach (Control control in flpChatList.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == message.Msg_Id.ToString())
                {
                    return; // 이미 표시된 메시지
                }
            }

            Panel messagePanel = new Panel
            {
                Width = flpChatList.ClientSize.Width - 20,
                Margin = new Padding(5),
                BackColor = Color.Transparent,
                Tag = message.Msg_Id
            };

            bool isMyMessage = message.User_Id == currentUserId;
            bool isSystemMessage = message.Msg_Kind == 0;

            if (isSystemMessage)
            {
                // 시스템 메시지 (가운데 정렬)
                Label lblSystem = new Label
                {
                    Text = message.Msg_Str,
                    Font = new Font("맑은 고딕", 9, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Height = 25
                };
                messagePanel.Height = 25;
                messagePanel.Controls.Add(lblSystem);
            }
            else
            {
                // 일반 메시지
                string timeStr = "";
                if (message.Date.Length >= 14)
                {
                    timeStr = $"{message.Date.Substring(8, 2)}:{message.Date.Substring(10, 2)}";
                }

                if (isMyMessage)
                {
                    // 내 메시지 (오른쪽 정렬)
                    Panel msgBubble = new Panel
                    {
                        BackColor = Color.FromArgb(41, 47, 102),
                        Padding = new Padding(10),
                        Margin = new Padding(0),
                        MaximumSize = new Size(messagePanel.Width - 100, 0),
                        AutoSize = true
                    };

                    Label lblMsg = new Label
                    {
                        Text = message.Msg_Str,
                        Font = new Font("맑은 고딕", 10),
                        ForeColor = Color.White,
                        AutoSize = true,
                        MaximumSize = new Size(msgBubble.MaximumSize.Width - 20, 0)
                    };

                    msgBubble.Controls.Add(lblMsg);

                    Label lblTime = new Label
                    {
                        Text = timeStr,
                        Font = new Font("맑은 고딕", 8),
                        ForeColor = Color.Gray,
                        AutoSize = true
                    };

                    int bubbleHeight = Math.Max(40, lblMsg.PreferredHeight + 20);
                    msgBubble.Height = bubbleHeight;

                    msgBubble.Location = new Point(messagePanel.Width - msgBubble.Width - 10, 5);
                    lblTime.Location = new Point(msgBubble.Left - lblTime.Width - 5,
                        msgBubble.Bottom - lblTime.Height);

                    messagePanel.Height = bubbleHeight + 15;
                    messagePanel.Controls.Add(msgBubble);
                    messagePanel.Controls.Add(lblTime);
                }
                else
                {
                    // 상대방 메시지 (왼쪽 정렬) - 닉네임 표시
                    string nickname = await GetUserNicknameAsync(message.User_Id);

                    Label lblSender = new Label
                    {
                        Text = nickname, // User ID 대신 닉네임 표시
                        Font = new Font("맑은 고딕", 8),
                        ForeColor = Color.Gray,
                        AutoSize = true,
                        Location = new Point(10, 5)
                    };

                    Panel msgBubble = new Panel
                    {
                        BackColor = Color.FromArgb(240, 240, 240),
                        Padding = new Padding(10),
                        Margin = new Padding(0),
                        MaximumSize = new Size(messagePanel.Width - 100, 0),
                        AutoSize = true,
                        Location = new Point(10, 25)
                    };

                    Label lblMsg = new Label
                    {
                        Text = message.Msg_Str,
                        Font = new Font("맑은 고딕", 10),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        MaximumSize = new Size(msgBubble.MaximumSize.Width - 20, 0)
                    };

                    msgBubble.Controls.Add(lblMsg);

                    Label lblTime = new Label
                    {
                        Text = timeStr,
                        Font = new Font("맑은 고딕", 8),
                        ForeColor = Color.Gray,
                        AutoSize = true
                    };

                    int bubbleHeight = Math.Max(40, lblMsg.PreferredHeight + 20);
                    msgBubble.Height = bubbleHeight;

                    lblTime.Location = new Point(msgBubble.Right + 5,
                        msgBubble.Bottom - lblTime.Height);

                    messagePanel.Height = bubbleHeight + 35;
                    messagePanel.Controls.Add(lblSender);
                    messagePanel.Controls.Add(msgBubble);
                    messagePanel.Controls.Add(lblTime);
                }
            }

            // UI 스레드에서 실행
            if (flpChatList.InvokeRequired)
            {
                flpChatList.Invoke((MethodInvoker)delegate
                {
                    flpChatList.Controls.Add(messagePanel);
                });
            }
            else
            {
                flpChatList.Controls.Add(messagePanel);
            }
        }

        // 초대 버튼 클릭 이벤트
        private async void btnInvite_Click(object sender, EventArgs e)
        {
            UserInviteDialog inviteDialog = new UserInviteDialog();
            if (inviteDialog.ShowDialog() == DialogResult.OK)
            {
                string userIdToInvite = inviteDialog.UserIdToInvite;
                await InviteUserToChatRoom(userIdToInvite);
            }
        }

        // 채팅방에 사용자 초대 - DCM 사용하도록 수정
        private async Task InviteUserToChatRoom(string userIdToInvite)
        {
            try
            {
                // opcode 33: 채팅방 초대
                List<string> items = new List<string>
                {
                    roomId,
                    userIdToInvite
                };

                var result = await LoginForm.GlobalDCM.db_request_data(33, items);

                if (result.Key && result.Value.Item2.Count > 0)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        MessageBox.Show($"사용자 {userIdToInvite}를 채팅방에 초대했습니다!", "성공",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 새 메시지 로드 (초대 메시지 표시)
                        await LoadNewMessages();
                    }
                    else
                    {
                        MessageBox.Show("사용자 초대에 실패했습니다.\n존재하지 않는 사용자 ID이거나 서버 오류일 수 있습니다.", "오류",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
                else
                {
                    MessageBox.Show("사용자 초대에 실패했습니다.\n존재하지 않는 사용자 ID이거나 서버 오류일 수 있습니다.", "오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"초대 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            await SendMessage();
        }

        private async void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                await SendMessage();
            }
        }

        // SendMessage 메서드 - DCM 사용하도록 수정
        private async Task SendMessage()
        {
            string message = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            try
            {
                // opcode 36: 채팅 쓰기
                List<string> items = new List<string>
                {
                    roomId,
                    "1", // 일반 채팅
                    message
                };

                var result = await LoginForm.GlobalDCM.db_request_data(36, items);

                if (result.Key && result.Value.Item2.Count > 0)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        txtMessage.Clear();
                        // 메시지 전송 후 새 메시지 로드
                        await LoadNewMessages();
                    }
                    else
                    {
                        MessageBox.Show("메시지 전송에 실패했습니다.", "오류",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
                else
                {
                    MessageBox.Show("메시지 전송에 실패했습니다.", "오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"메시지 전송 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ChatMessage 역직렬화
        private ChatMessage DeserializeChatMessage(string json)
        {
            var ser = new DataContractJsonSerializer(typeof(ChatMessage));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (ChatMessage)ser.ReadObject(ms);
            }
        }

        // FriendInfo 역직렬화
        private FriendInfo DeserializeFriendInfo(string json)
        {
            var ser = new DataContractJsonSerializer(typeof(FriendInfo));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (FriendInfo)ser.ReadObject(ms);
            }
        }

        private void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            refreshTimer?.Stop();
            refreshTimer?.Dispose();
        }

        private void flpChatList_Resize(object sender, EventArgs e)
        {
            // 채팅 패널들의 너비 조정
            foreach (Panel panel in flpChatList.Controls.OfType<Panel>())
            {
                panel.Width = flpChatList.ClientSize.Width - 20;
            }
        }

        // 로컬 데이터 클래스들
        [DataContract]
        private class ChatMessage
        {
            [DataMember] internal int Msg_Id;
            [DataMember] internal string User_Id;
            [DataMember] internal int Msg_Kind;
            [DataMember] internal string Date;
            [DataMember] internal string Msg_Str;
        }

        [DataContract]
        private class FriendInfo
        {
            [DataMember] internal string Friend_Id;
            [DataMember] internal string Nickname;
        }
    }
}