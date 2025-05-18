using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ChatForm : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private string nickname;
        private Image defaultProfileImage;

        public ChatForm(string nickname)
        {
            InitializeComponent();
            this.nickname = nickname;
            this.Text = $"채팅 - {nickname}";
            defaultProfileImage = Image.FromFile("프로필.jpg");
            ConnectToServer();
        }

        private void ChatForm_Resize(object sender, EventArgs e)
        {
            chatLayout.Invalidate();
            chatLayout.PerformLayout();
        }

        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                btnSend_Click(sender, e);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            string fullMsg = $"[{nickname}] {msg}";
            byte[] data = Encoding.UTF8.GetBytes(fullMsg);
            stream.Write(data, 0, data.Length);

            AppendMessage(fullMsg);
            txtMessage.Clear();
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 9000);
                stream = client.GetStream();

                receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch
            {
                MessageBox.Show("서버에 연결할 수 없습니다.");
                Close();
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int len = stream.Read(buffer, 0, buffer.Length);
                    if (len == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, len);
                    AppendMessage(msg);
                }
                catch
                {
                    AppendMessage("연결이 끊겼습니다.");
                    break;
                }
            }
        }

        private void AppendMessage(string msg)
        {
            if (InvokeRequired)
                Invoke(new Action(() => AppendMessage(msg)));
            else
            {
                if (msg.StartsWith($"[{nickname}]"))
                    AddMyMessage(msg);
                else
                    AddOtherMessage(msg);
            }
        }

        private void AddMyMessage(string message)
        {
            string content = message.Substring(message.IndexOf("]") + 1).Trim();

            Label msgLabel = new Label
            {
                Text = content,
                AutoSize = true,
                Font = new Font("맑은 고딕", 10),
                BackColor = Color.Gold,
                Padding = new Padding(10),
                MaximumSize = new Size(250, 0),
                Margin = new Padding(0)
            };

            Label timeLabel = new Label
            {
                Text = DateTime.Now.ToString("tt h:mm"),
                Font = new Font("맑은 고딕", 7),
                ForeColor = Color.Gray,
                AutoSize = true,
                Margin = new Padding(0, 2, 0, 0)
            };

            FlowLayoutPanel stack = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false,
                Anchor = AnchorStyles.Right,
                Margin = new Padding(5)
            };

            stack.Controls.Add(msgLabel);
            stack.Controls.Add(timeLabel);

            chatLayout.Controls.Add(new Panel(), 0, chatLayout.RowCount);
            chatLayout.Controls.Add(stack, 1, chatLayout.RowCount++);
            chatLayout.ScrollControlIntoView(stack);
        }

        private void AddOtherMessage(string message)
        {
            int idx = message.IndexOf("]");
            string senderName = message.Substring(1, idx - 1);
            string content = message.Substring(idx + 1).Trim();

            PictureBox profile = new PictureBox
            {
                Image = defaultProfileImage,
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Margin = new Padding(5, 0, 5, 0),
                Cursor = Cursors.Hand
            };

            Label nameLabel = new Label
            {
                Text = senderName,
                Font = new Font("맑은 고딕", 9, FontStyle.Bold),
                AutoSize = true
            };

            Label msgLabel = new Label
            {
                Text = content,
                AutoSize = true,
                Font = new Font("맑은 고딕", 10),
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10),
                MaximumSize = new Size(250, 0),
                Margin = new Padding(0)
            };

            Label timeLabel = new Label
            {
                Text = DateTime.Now.ToString("tt h:mm"),
                Font = new Font("맑은 고딕", 7),
                ForeColor = Color.Gray,
                AutoSize = true,
                Margin = new Padding(0, 2, 0, 0)
            };

            FlowLayoutPanel msgStack = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false
            };
            msgStack.Controls.Add(nameLabel);
            msgStack.Controls.Add(msgLabel);
            msgStack.Controls.Add(timeLabel);

            FlowLayoutPanel wrapper = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };
            wrapper.Controls.Add(profile);
            wrapper.Controls.Add(msgStack);

            chatLayout.Controls.Add(wrapper, 0, chatLayout.RowCount++);
            chatLayout.Controls.Add(new Panel(), 1, chatLayout.RowCount++);
            chatLayout.ScrollControlIntoView(wrapper);
        }
    }
}
