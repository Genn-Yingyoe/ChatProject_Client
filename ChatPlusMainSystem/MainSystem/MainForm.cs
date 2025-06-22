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

namespace MainSystem
{
    public partial class MainForm : Form
    {
        private string currentUserId;
        private string currentUserName;
        private string currentUserNickname;
        private List<FriendInfo> friendList = new List<FriendInfo>();
        private List<string> chatRoomList = new List<string>();
        private bool isLoadingData = false;

        // 알림 관련
        private Timer notificationTimer;
        private List<NotificationInfo> pendingNotifications = new List<NotificationInfo>();
        private bool isCheckingNotifications = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // UI 초기 설정
            this.BackColor = Color.FromArgb(200, 202, 224);

            // 버튼 스타일 설정
            btnNewChat.BackColor = Color.FromArgb(41, 47, 102);
            btnNewChat.ForeColor = Color.White;
            btnNewChat.FlatStyle = FlatStyle.Flat;
            btnNewChat.FlatAppearance.BorderSize = 0;

            button2.BackColor = Color.FromArgb(41, 47, 102);
            button2.ForeColor = Color.White;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;

            button3.BackColor = Color.FromArgb(41, 47, 102);
            button3.ForeColor = Color.White;
            button3.FlatStyle = FlatStyle.Flat;
            button3.FlatAppearance.BorderSize = 0;

            button4.BackColor = Color.FromArgb(41, 47, 102);
            button4.ForeColor = Color.White;
            button4.FlatStyle = FlatStyle.Flat;
            button4.FlatAppearance.BorderSize = 0;

            // FlowLayoutPanel 스타일 설정
            flpMain.BackColor = Color.White;
            flpMain.BorderStyle = BorderStyle.FixedSingle;

            // RadioButton 이벤트 설정
            rdbChatroom.AutoCheck = true;
        }

        // 알림 타이머 초기화
        private void InitializeNotificationTimer()
        {
            notificationTimer = new Timer();
            notificationTimer.Interval = 10000; // 10초마다
            notificationTimer.Tick += async (s, e) => await CheckNotifications();
            notificationTimer.Start();
        }

        // LoginForm에서 호출되는 초기화 메서드 - 수정됨
        public void InitializeAfterLogin(string userId, string userName)
        {
            currentUserId = userId;
            currentUserName = userName;

            // 사용자 닉네임도 저장 (ChatForm에서 사용)
            LoadUserNickname();

            // UI 업데이트 - User ID도 함께 표시
            this.Text = $"ChatMoa - {userName}({userId})";

            // 알림 타이머 시작
            InitializeNotificationTimer();

            // 서버에서 데이터 로드
            LoadUserDataFromServer();
        }

        // 사용자 닉네임 로드 - DCM 사용하도록 수정
        private async void LoadUserNickname()
        {
            try
            {
                // opcode 13: user_id_search로 자신의 닉네임 조회
                List<string> items = new List<string> { currentUserId };
                var result = await LoginForm.GlobalDCM.db_request_data(13, items);

                if (result.Key && result.Value.Item2.Count >= 2)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        var userInfo = LoginForm.DeserializeGlobalDCMJson<FriendInfo>(key, indexes[0]);
                        if (!string.IsNullOrEmpty(userInfo.Nickname))
                        {
                            currentUserNickname = userInfo.Nickname;
                            // 닉네임 로드 후 타이틀 업데이트
                            this.Text = $"ChatMoa - {currentUserNickname}({currentUserId})";
                        }
                        else
                        {
                            currentUserNickname = currentUserName; // 기본값
                        }
                    }
                    else
                    {
                        currentUserNickname = currentUserName; // 기본값
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
                else
                {
                    currentUserNickname = currentUserName; // 기본값
                }
            }
            catch (Exception)
            {
                currentUserNickname = currentUserName; // 오류 시 기본값
            }
        }

        // 이전 버전과의 호환성을 위한 오버로드
        public void InitializeAfterLogin(string code)
        {
            currentUserId = code;
            currentUserName = "사용자";
            currentUserNickname = "사용자";

            this.Text = $"ChatMoa - {currentUserName}({currentUserId})";

            // 알림 타이머 시작
            InitializeNotificationTimer();

            LoadUserDataFromServer();
        }

        // 알림 확인 메서드 - DCM 사용하도록 수정
        private async Task CheckNotifications()
        {
            if (string.IsNullOrEmpty(currentUserId) || isCheckingNotifications)
                return;

            isCheckingNotifications = true;

            try
            {
                // opcode 11: 알림 전체 읽기
                List<string> items = new List<string>();
                var result = await LoginForm.GlobalDCM.db_request_data(11, items);

                if (result.Key && result.Value.Item2.Count > 0)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    List<NotificationInfo> newNotifications = new List<NotificationInfo>();

                    for (int i = 0; i < indexes.Count; i++)
                    {
                        try
                        {
                            string responseData = LoginForm.GetGlobalDCMResponseData(key, indexes[i]);

                            if (responseData.StartsWith("{") && responseData.EndsWith("}"))
                            {
                                var notification = LoginForm.DeserializeGlobalDCMJson<NotificationInfo>(key, indexes[i]);

                                if (!notification.Inform_Checked &&
                                    !pendingNotifications.Any(n => n.Inform_Id == notification.Inform_Id))
                                {
                                    newNotifications.Add(notification);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // 파싱 오류는 무시
                        }
                    }

                    if (newNotifications.Count > 0)
                    {
                        ProcessNewNotifications(newNotifications);
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
            }
            catch (Exception)
            {
                // 네트워크 오류는 조용히 처리
            }
            finally
            {
                isCheckingNotifications = false;
            }
        }

        // 새 알림 처리
        private void ProcessNewNotifications(List<NotificationInfo> notifications)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    ProcessNewNotificationsInternal(notifications);
                });
            }
            else
            {
                ProcessNewNotificationsInternal(notifications);
            }
        }

        private void ProcessNewNotificationsInternal(List<NotificationInfo> notifications)
        {
            try
            {
                foreach (var notification in notifications)
                {
                    if (!pendingNotifications.Any(n => n.Inform_Id == notification.Inform_Id))
                    {
                        pendingNotifications.Add(notification);
                        ShowNotificationPopup(notification);
                    }
                }
            }
            catch (Exception)
            {
                // 오류는 무시
            }
        }

        // 알림 팝업 표시
        private void ShowNotificationPopup(NotificationInfo notification)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ShowNotificationPopupInternal(notification);
                });
            }
            else
            {
                ShowNotificationPopupInternal(notification);
            }
        }

        private void ShowNotificationPopupInternal(NotificationInfo notification)
        {
            try
            {
                pendingNotifications.RemoveAll(n => n.Inform_Id == notification.Inform_Id);

                NotificationPopupForm popupForm = new NotificationPopupForm(notification);

                popupForm.NotificationProcessed += async (accepted) =>
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await ProcessNotificationResponse(notification, accepted);
                        }
                        catch (Exception)
                        {
                            // 오류는 무시
                        }
                    });
                };

                popupForm.Show();
                popupForm.BringToFront();
                popupForm.Activate();
            }
            catch (Exception)
            {
                // 팝업 폼 생성 실패 시 간단한 MessageBox 사용
                string message = $"알림: {notification.Inform_Str}\n\n수락하시겠습니까?";
                string title = notification.Inform_Kind == "friend_request" ? "친구 요청" :
                              notification.Inform_Kind == "invite" ? "채팅방 초대" : "알림";

                var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                bool accepted = (result == DialogResult.Yes);

                Task.Run(async () =>
                {
                    try
                    {
                        await ProcessNotificationResponse(notification, accepted);
                    }
                    catch (Exception)
                    {
                        // 오류는 무시
                    }
                });
            }
        }

        // 알림 응답 처리 - DCM 사용하도록 수정
        private async Task ProcessNotificationResponse(NotificationInfo notification, bool accepted)
        {
            try
            {
                // opcode 8: 알림 확인/수락
                List<string> items = new List<string>
                {
                    notification.Inform_Id.ToString(),
                    accepted ? "1" : "0"
                };

                var result = await LoginForm.GlobalDCM.db_request_data(8, items);

                if (result.Key && result.Value.Item2.Count > 0)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        if (accepted && notification.Inform_Kind == "friend_request")
                        {
                            await Task.Delay(1500);
                            UpdateUIAfterFriendAccepted();
                        }
                        else if (accepted && notification.Inform_Kind == "invite")
                        {
                            if (notification.need_items != null && notification.need_items.Count > 0)
                            {
                                string roomId = notification.need_items[0];

                                // 서버 처리 시간을 고려하여 잠시 대기
                                await Task.Delay(2000);

                                UpdateUIAfterChatRoomAccepted(roomId);
                            }
                        }
                        else if (!accepted)
                        {
                            ShowNotificationMessage("알림을 거절했습니다.", "알림", MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        ShowNotificationMessage("알림 처리에 실패했습니다.", "오류", MessageBoxIcon.Error);
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
                else
                {
                    ShowNotificationMessage("알림 처리에 실패했습니다.", "오류", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowNotificationMessage($"알림 처리 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxIcon.Error);
            }
        }

        private async void rdbChatroom_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbChatroom.Checked)
            {
                // 채팅방 목록 강제 새로고침
                if (!isLoadingData)
                {
                    isLoadingData = true;
                    try
                    {
                        await LoadChatRoomListFromServer();
                        DisplayChatRoomList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"채팅방 목록 로드 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        isLoadingData = false;
                    }
                }
            }
        }

        // UI 업데이트 메서드들
        private void UpdateUIAfterFriendAccepted()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    UpdateUIAfterFriendAcceptedInternal();
                });
            }
            else
            {
                UpdateUIAfterFriendAcceptedInternal();
            }
        }

        private async void UpdateUIAfterFriendAcceptedInternal()
        {
            try
            {
                await LoadFriendListFromServer();

                if (rdbFriend.Checked)
                {
                    DisplayFriendList();
                }

                ShowNotificationMessage("친구가 추가되었습니다!", "알림", MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                ShowNotificationMessage("친구가 추가되었지만 목록 새로고침에 실패했습니다.", "알림", MessageBoxIcon.Warning);
            }
        }

        private void UpdateUIAfterChatRoomAccepted(string roomId)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    UpdateUIAfterChatRoomAcceptedInternal(roomId);
                });
            }
            else
            {
                UpdateUIAfterChatRoomAcceptedInternal(roomId);
            }
        }

        private async void UpdateUIAfterChatRoomAcceptedInternal(string roomId)
        {
            try
            {
                // 채팅방 목록을 먼저 새로고침
                await LoadChatRoomListFromServer();

                // 채팅방 탭이 선택되어 있다면 UI 업데이트
                if (rdbChatroom.Checked)
                {
                    DisplayChatRoomList();
                }

                // 채팅방 열기
                OpenChatRoom(roomId);

                ShowNotificationMessage($"채팅방에 입장했습니다!", "알림", MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                ShowNotificationMessage("채팅방 입장에 실패했습니다.", "오류", MessageBoxIcon.Error);
            }
        }

        private async void RefreshChatRoomList()
        {
            try
            {
                await LoadChatRoomListFromServer();

                // 현재 채팅방 탭이 선택되어 있다면 UI 업데이트
                if (rdbChatroom.Checked)
                {
                    DisplayChatRoomList();
                }
            }
            catch (Exception)
            {
                // 채팅방 목록 새로고침 실패는 조용히 처리
            }
        }

        private void ShowNotificationMessage(string message, string title, MessageBoxIcon icon)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
                });
            }
            else
            {
                MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
            }
        }

        // 채팅방 열기 - 수정됨: 닉네임도 전달
        private void OpenChatRoom(string roomId)
        {
            // 이미 열린 채팅방이 있는지 확인
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is ChatForm chatForm && chatForm.RoomId == roomId)
                {
                    chatForm.BringToFront();
                    chatForm.Activate();
                    return;
                }
            }

            // 새 채팅방 열기 - 닉네임도 함께 전달
            ChatForm newChatForm = new ChatForm();
            string displayName = !string.IsNullOrEmpty(currentUserNickname) ? currentUserNickname : currentUserName;
            newChatForm.InitializeChat(currentUserId, displayName, roomId);
            newChatForm.Show();
        }

        private async void LoadUserDataFromServer()
        {
            if (isLoadingData) return;
            isLoadingData = true;

            try
            {
                if (rdbFriend.Checked)
                {
                    await LoadFriendListFromServer();
                    DisplayFriendList();
                }
                else
                {
                    await LoadChatRoomListFromServer();
                    DisplayChatRoomList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 로드 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoadingData = false;
            }
        }

        // 서버에서 친구 목록 로드 - DCM 사용하도록 수정
        private async Task LoadFriendListFromServer()
        {
            try
            {
                // opcode 10: 친구 목록 읽기
                List<string> items = new List<string>();
                var result = await LoginForm.GlobalDCM.db_request_data(10, items);

                friendList.Clear();
                HashSet<string> addedFriendIds = new HashSet<string>(); // 중복 제거용

                if (result.Key && result.Value.Item2.Count > 0)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    // 마지막 응답이 "1"이면 성공
                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        for (int i = 0; i < indexes.Count - 1; i++)
                        {
                            try
                            {
                                string friendData = LoginForm.GetGlobalDCMResponseData(key, indexes[i]);

                                if (friendData.StartsWith("{") && friendData.EndsWith("}"))
                                {
                                    var friendInfo = LoginForm.DeserializeGlobalDCMJson<FriendInfo>(key, indexes[i]);

                                    // 중복 체크 및 유효성 검사
                                    if (!string.IsNullOrEmpty(friendInfo.Friend_Id) &&
                                        friendInfo.Friend_Id != "0" &&
                                        !addedFriendIds.Contains(friendInfo.Friend_Id))
                                    {
                                        friendList.Add(friendInfo);
                                        addedFriendIds.Add(friendInfo.Friend_Id);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                // 파싱 오류는 무시
                            }
                        }
                    }
                    else if (indexes.Count == 1 && LoginForm.GetGlobalDCMResponseData(key, indexes[0]) == "0")
                    {
                        // 친구 목록이 비어있음
                    }
                    else
                    {
                        // 모든 데이터를 친구 정보로 처리
                        for (int i = 0; i < indexes.Count; i++)
                        {
                            try
                            {
                                string friendData = LoginForm.GetGlobalDCMResponseData(key, indexes[i]);

                                if (friendData.StartsWith("{") && friendData.EndsWith("}"))
                                {
                                    var friendInfo = LoginForm.DeserializeGlobalDCMJson<FriendInfo>(key, indexes[i]);

                                    // 중복 체크 및 유효성 검사
                                    if (!string.IsNullOrEmpty(friendInfo.Friend_Id) &&
                                        friendInfo.Friend_Id != "0" &&
                                        !addedFriendIds.Contains(friendInfo.Friend_Id))
                                    {
                                        friendList.Add(friendInfo);
                                        addedFriendIds.Add(friendInfo.Friend_Id);
                                    }
                                }
                                else if (friendData == "1" || friendData == "0")
                                {
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                // 파싱 오류는 무시
                            }
                        }
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // 서버에서 채팅방 목록 로드 - DCM 사용하도록 수정
        private async Task LoadChatRoomListFromServer()
        {
            try
            {
                // opcode 38: 내 채팅방 목록 읽기
                List<string> items = new List<string>();
                var result = await LoginForm.GlobalDCM.db_request_data(38, items);

                chatRoomList.Clear();

                if (result.Key && result.Value.Item2.Count > 0)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        for (int i = 0; i < indexes.Count - 1; i++)
                        {
                            string roomData = LoginForm.GetGlobalDCMResponseData(key, indexes[i]);

                            // 구분자가 있는 경우 제거 (서버가 아직 구분자를 보내는 경우 대비)
                            if (roomData.Contains("|"))
                            {
                                string[] parts = roomData.Split('|');
                                if (parts.Length > 0)
                                {
                                    string roomId = parts[0];
                                    if (!string.IsNullOrEmpty(roomId))
                                    {
                                        chatRoomList.Add(roomId);
                                    }
                                }
                            }
                            else
                            {
                                // 순수한 Room ID인 경우
                                chatRoomList.Add(roomData);
                            }
                        }
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // 친구 목록 표시
        private void DisplayFriendList()
        {
            flpMain.Controls.Clear();
            flpMain.AutoScroll = true;

            foreach (var friend in friendList)
            {
                Panel friendPanel = new Panel
                {
                    Width = flpMain.ClientSize.Width - 30,
                    Height = 60,
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.None,
                    BackColor = Color.White
                };

                PictureBox pic = new PictureBox
                {
                    Size = new Size(50, 50),
                    Location = new Point(5, 5),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.LightGray,
                    BorderStyle = BorderStyle.FixedSingle
                };

                EventHandler showProfile = (sender, e) =>
                {
                    // sender로부터 친구 ID 가져오기
                    string friendId = friend.Friend_Id;

                    // 프로필 폼 띄우기 (모달)
                    using (var profileForm = new FriendProfileForm(friendId,friend.Nickname))
                    {
                        profileForm.ShowDialog();
                    }
                };

                try
                {
                    string imagePath = Path.Combine(Application.StartupPath, "default_profile.png");
                    if (File.Exists(imagePath))
                    {
                        pic.Image = Image.FromFile(imagePath);
                    }
                    else
                    {
                        Bitmap defaultImage = new Bitmap(50, 50);
                        using (Graphics g = Graphics.FromImage(defaultImage))
                        {
                            g.Clear(Color.LightGray);
                            g.DrawString(friend.Nickname.Substring(0, 1), new Font("맑은 고딕", 20),
                                Brushes.DarkGray, new PointF(12, 10));
                        }
                        pic.Image = defaultImage;
                    }
                }
                catch { }

                pic.Tag = friend.Friend_Id;
                pic.Cursor = Cursors.Hand;

                Label lblNickname = new Label
                {
                    Text = friend.Nickname,
                    Font = new Font("맑은 고딕", 12, FontStyle.Bold),
                    Location = new Point(65, 10),
                    Size = new Size(friendPanel.Width - 70, 20),
                    Tag = friend.Friend_Id
                };

                Label lblId = new Label
                {
                    Text = $"ID: {friend.Friend_Id}",
                    Font = new Font("맑은 고딕", 9),
                    ForeColor = Color.Gray,
                    Location = new Point(65, 32),
                    Size = new Size(friendPanel.Width - 70, 20)
                };

                // 친구 클릭 이벤트 수정 - 1:1 채팅방 생성
                EventHandler friendClick = async (sender, e) =>
                {
                    //await CreateDirectChatWithFriend(friend);
                };

                friendPanel.Click += friendClick;
                pic.Click += friendClick;
                lblNickname.Click += friendClick;
                lblId.Click += friendClick;

                friendPanel.MouseEnter += (s, e) => friendPanel.BackColor = Color.FromArgb(240, 240, 240);
                friendPanel.MouseLeave += (s, e) => friendPanel.BackColor = Color.White;

                friendPanel.Controls.Add(pic);
                friendPanel.Controls.Add(lblNickname);
                friendPanel.Controls.Add(lblId);

                friendPanel.Click += showProfile;
                pic.Click += showProfile;
                lblNickname.Click += showProfile;
                lblId.Click += showProfile;

                flpMain.Controls.Add(friendPanel);
            }

            if (friendList.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "친구 목록이 비어있습니다.\n친구를 추가해보세요!",
                    Font = new Font("맑은 고딕", 10),
                    ForeColor = Color.Gray,
                    Size = new Size(flpMain.Width - 20, 60),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(10, 50, 10, 10)
                };
                flpMain.Controls.Add(lblEmpty);
            }
        }

        private async Task CreateDirectChatWithFriend(FriendInfo friend)
        {
            try
            {
                // 로딩 표시
                this.Cursor = Cursors.WaitCursor;

                // 새 1:1 채팅방 생성
                string roomId = await CreateNewDirectChatRoom(friend);

                if (!string.IsNullOrEmpty(roomId))
                {
                    // 채팅방 목록 새로고침
                    await LoadChatRoomListFromServer();

                    // 채팅방 탭이 선택되어 있다면 UI 업데이트
                    if (rdbChatroom.Checked)
                    {
                        DisplayChatRoomList();
                    }

                    // 채팅방 열기
                    OpenChatRoom(roomId);
                }
                else
                {
                    MessageBox.Show($"{friend.Nickname}님과의 채팅방 생성에 실패했습니다.", "오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"채팅방 생성 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async Task<string> CreateNewDirectChatRoom(FriendInfo friend)
        {
            try
            {
                // opcode 32: 채팅방 만들기 (친구 1명만 초대)
                List<string> items = new List<string>
        {
            friend.Friend_Id
        };

                var result = await LoginForm.GlobalDCM.db_request_data(32, items);

                if (result.Key && result.Value.Item2.Count >= 2)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        string roomId = LoginForm.GetGlobalDCMResponseData(key, indexes[indexes.Count - 2]);
                        LoginForm.ClearGlobalDCMReceivedData(key);
                        return roomId;
                    }
                    else
                    {
                        LoginForm.ClearGlobalDCMReceivedData(key);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"1:1 채팅방 생성 오류: {ex.Message}");
                throw;
            }
        }

        // 채팅방 목록 표시
        private void DisplayChatRoomList()
        {
            flpMain.Controls.Clear();
            flpMain.AutoScroll = true;

            foreach (var roomId in chatRoomList)
            {
                Panel roomPanel = new Panel
                {
                    Width = flpMain.ClientSize.Width - 30,
                    Height = 60,
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.None,
                    BackColor = Color.White
                };

                PictureBox pic = new PictureBox
                {
                    Size = new Size(50, 50),
                    Location = new Point(5, 5),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.FromArgb(41, 47, 102),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Bitmap roomIcon = new Bitmap(50, 50);
                using (Graphics g = Graphics.FromImage(roomIcon))
                {
                    g.Clear(Color.FromArgb(41, 47, 102));
                    g.DrawString("채팅", new Font("맑은 고딕", 12), Brushes.White, new PointF(8, 15));
                }
                pic.Image = roomIcon;

                Label lblRoomId = new Label
                {
                    Text = $"채팅방 {roomId}",
                    Font = new Font("맑은 고딕", 12, FontStyle.Bold),
                    Location = new Point(65, 20),
                    Size = new Size(roomPanel.Width - 70, 20),
                    Tag = roomId
                };

                EventHandler roomClick = (sender, e) =>
                {
                    OpenChatRoom(roomId);
                };

                roomPanel.Click += roomClick;
                pic.Click += roomClick;
                lblRoomId.Click += roomClick;

                roomPanel.MouseEnter += (s, e) => roomPanel.BackColor = Color.FromArgb(240, 240, 240);
                roomPanel.MouseLeave += (s, e) => roomPanel.BackColor = Color.White;

                roomPanel.Controls.Add(pic);
                roomPanel.Controls.Add(lblRoomId);

                flpMain.Controls.Add(roomPanel);
            }

            if (chatRoomList.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "참여중인 채팅방이 없습니다.\n새 채팅을 시작해보세요!",
                    Font = new Font("맑은 고딕", 10),
                    ForeColor = Color.Gray,
                    Size = new Size(flpMain.Width - 20, 60),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(10, 50, 10, 10)
                };
                flpMain.Controls.Add(lblEmpty);
            }
        }

        // 이벤트 핸들러들
        private void btn1_Click(object sender, EventArgs e)
        {
            if (friendList.Count == 0)
            {
                MessageBox.Show("친구 목록이 비어있습니다.\n먼저 친구를 추가해주세요.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            FriendSelectForm selectForm = new FriendSelectForm(friendList);
            if (selectForm.ShowDialog() == DialogResult.OK)
            {
                var selectedFriends = selectForm.SelectedFriends;
                if (selectedFriends.Count > 0)
                {
                    CreateNewChatRoom(selectedFriends);
                }
            }
        }

        // CreateNewChatRoom 메서드 - DCM 사용하도록 수정
        private async void CreateNewChatRoom(List<FriendInfo> selectedFriends)
        {
            try
            {
                // opcode 32: 채팅방 만들기
                List<string> items = new List<string>();

                foreach (var friend in selectedFriends)
                {
                    items.Add(friend.Friend_Id);
                }

                var result = await LoginForm.GlobalDCM.db_request_data(32, items);

                if (result.Key && result.Value.Item2.Count >= 2)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    string lastResponse = LoginForm.GetGlobalDCMResponseData(key, indexes.Last());

                    if (lastResponse == "1")
                    {
                        string roomId = LoginForm.GetGlobalDCMResponseData(key, indexes[indexes.Count - 2]);

                        // 채팅방 목록 즉시 새로고침
                        await LoadChatRoomListFromServer();

                        // 채팅방 탭이 선택되어 있다면 UI 업데이트
                        if (rdbChatroom.Checked)
                        {
                            DisplayChatRoomList();
                        }

                        // 채팅방 열기
                        OpenChatRoom(roomId);

                        MessageBox.Show($"그룹 채팅방이 생성되었습니다.\nRoom ID: {roomId}", "성공",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("채팅방 생성에 실패했습니다.", "오류",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    LoginForm.ClearGlobalDCMReceivedData(key);
                }
                else
                {
                    MessageBox.Show("채팅방 생성에 실패했습니다.", "오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"채팅방 생성 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            FriendInviteForm inviteForm = new FriendInviteForm();
            inviteForm.CurrentUserId = currentUserId;
            if (inviteForm.ShowDialog() == DialogResult.OK)
            {
                LoadUserDataFromServer();
            }
        }

        private void rdbFriend_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbFriend.Checked)
            {
                LoadUserDataFromServer();
            }
        }

        private void flpMain_Paint(object sender, PaintEventArgs e)
        {
            // FlowLayoutPanel 페인트 이벤트
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            rdbFriend.Checked = true;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            notificationTimer?.Stop();
            notificationTimer?.Dispose();
            Application.Exit();
        }

        // 추가 기능 버튼들 (필요에 따라 활용)
        private void button3_Click(object sender, EventArgs e)
        {
            CalendarApp.MainForm calendarForm = new CalendarApp.MainForm();
            calendarForm.UserLoggedIn(currentUserId);  // 현재 사용자 ID 넘겨주기
            calendarForm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (var configForm = new ConfigForm(currentUserId,currentUserNickname))
            {
                configForm.ShowDialog();
            }
        }
    }

    // 모든 데이터 구조들을 MainForm.cs에 통합
    [DataContract]
    internal class FriendInfo
    {
        [DataMember] internal string Friend_Id;
        [DataMember] internal string Nickname;
    }

    [DataContract]
    internal class NotificationInfo
    {
        [DataMember] internal int Inform_Id;
        [DataMember] internal string Inform_Date;
        [DataMember] internal string Inform_Kind;
        [DataMember] internal string Inform_Str;
        [DataMember] internal List<string> need_items;
        [DataMember] internal bool Inform_Checked;
    }

    // ChatForm에서 사용하는 데이터 클래스도 여기에 추가
    [DataContract]
    internal class ChatMessage
    {
        [DataMember] internal int Msg_Id;
        [DataMember] internal string User_Id;
        [DataMember] internal int Msg_Kind;
        [DataMember] internal string Date;
        [DataMember] internal string Msg_Str;
    }
}