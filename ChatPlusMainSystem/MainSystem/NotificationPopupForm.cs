using System;
using System.Drawing;
using System.Windows.Forms;

namespace MainSystem
{
    internal partial class NotificationPopupForm : Form
    {
        private NotificationInfo notification;

        public event Action<bool> NotificationProcessed;

        internal NotificationPopupForm(NotificationInfo notificationInfo)
        {
            InitializeComponent();
            notification = notificationInfo;
            InitializeUI();
            SetupNotificationContent();
        }

        private void InitializeUI()
        {
            this.BackColor = Color.FromArgb(200, 202, 224);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 자동 닫기 타이머 (30초)
            Timer autoCloseTimer = new Timer();
            autoCloseTimer.Interval = 30000;
            autoCloseTimer.Tick += (sender, args) =>  // 변수명 변경
            {
                autoCloseTimer.Stop();
                if (this.Visible)
                {
                    NotificationProcessed?.Invoke(false); // 거절로 처리
                    this.Close();
                }
            };
            autoCloseTimer.Start();
        }

        private void SetupNotificationContent()
        {
            if (notification.Inform_Kind == "friend_request")
            {
                this.Text = "친구 요청";
                lblTitle.Text = "친구 요청";
                lblMessage.Text = notification.Inform_Str;
                btnAccept.Text = "수락";
                btnReject.Text = "거절";

                // 아이콘 설정
                try
                {
                    Bitmap friendIcon = new Bitmap(64, 64);
                    using (Graphics g = Graphics.FromImage(friendIcon))
                    {
                        g.Clear(Color.FromArgb(41, 47, 102));
                        g.DrawString("👤", new Font("Segoe UI Emoji", 24), Brushes.White, new PointF(8, 8));
                    }
                    picIcon.Image = friendIcon;
                }
                catch
                {
                    picIcon.BackColor = Color.FromArgb(41, 47, 102);
                }
            }
            else if (notification.Inform_Kind == "invite")
            {
                this.Text = "채팅방 초대";
                lblTitle.Text = "채팅방 초대";
                lblMessage.Text = notification.Inform_Str;
                btnAccept.Text = "참여";
                btnReject.Text = "거절";

                // 아이콘 설정
                try
                {
                    Bitmap chatIcon = new Bitmap(64, 64);
                    using (Graphics g = Graphics.FromImage(chatIcon))
                    {
                        g.Clear(Color.FromArgb(41, 47, 102));
                        g.DrawString("💬", new Font("Segoe UI Emoji", 24), Brushes.White, new PointF(8, 8));
                    }
                    picIcon.Image = chatIcon;
                }
                catch
                {
                    picIcon.BackColor = Color.FromArgb(41, 47, 102);
                }
            }
            else
            {
                this.Text = "알림";
                lblTitle.Text = "알림";
                lblMessage.Text = notification.Inform_Str;
                btnAccept.Text = "확인";
                btnReject.Text = "닫기";

                picIcon.BackColor = Color.Gray;
            }

            // 날짜 표시
            if (notification.Inform_Date.Length >= 8)
            {
                string dateStr = $"{notification.Inform_Date.Substring(0, 4)}-{notification.Inform_Date.Substring(4, 2)}-{notification.Inform_Date.Substring(6, 2)}";
                lblDate.Text = dateStr;
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            NotificationProcessed?.Invoke(true);
            this.Close();
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            NotificationProcessed?.Invoke(false);
            this.Close();
        }

        private void NotificationPopupForm_Load(object sender, EventArgs e)
        {
            // 화면 우측 하단에 위치
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(
                workingArea.Right - this.Width - 20,
                workingArea.Bottom - this.Height - 20
            );

            // 슬라이드 인 애니메이션 효과
            this.Opacity = 0;
            Timer fadeTimer = new Timer();
            fadeTimer.Interval = 50;
            fadeTimer.Tick += (timerSender, timerArgs) =>  // 변수명 변경
            {
                this.Opacity += 0.1;
                if (this.Opacity >= 1.0)
                {
                    fadeTimer.Stop();
                }
            };
            fadeTimer.Start();
        }

        private void NotificationPopupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 폼이 닫힐 때 정리 작업
        }
    }
}