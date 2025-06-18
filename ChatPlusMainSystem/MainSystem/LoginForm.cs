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
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MainSystem
{
    public partial class LoginForm : Form
    {
        // TextBox 패딩 설정을 위한 Win32 API
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

        // 로그인한 사용자 정보를 저장할 정적 변수들
        public static string LoggedInUserId = "";
        public static string LoggedInUserName = "";
        public static string LoggedInUserNickname = "";

        public LoginForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // TextBox 패딩 설정 (좌, 상, 우, 하)
            SetTextBoxPadding(txtID, 5, 5, 5, 5);
            SetTextBoxPadding(txtPW, 5, 5, 5, 5);

            // PictureBox 이미지 설정 (상대 경로 사용)
            try
            {
                string imagePath = Path.Combine(Application.StartupPath, "main.png");
                if (File.Exists(imagePath))
                {
                    pbApp.ImageLocation = imagePath;
                }
                else
                {
                    // 이미지가 없을 경우 기본 이미지나 색상으로 대체
                    pbApp.BackColor = Color.FromArgb(200, 202, 224);
                }
            }
            catch { }
        }

        private void SetTextBoxPadding(TextBox tb, int left, int top, int right, int bottom)
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

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // 폼 로드 시 초기화 작업
            txtID.Focus();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string inputID = txtID.Text.Trim();
            string inputPW = txtPW.Text.Trim();

            // 입력값 검증
            if (string.IsNullOrEmpty(inputID) || string.IsNullOrEmpty(inputPW))
            {
                MessageBox.Show("아이디와 비밀번호를 입력해주세요.", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 버튼 비활성화 (중복 클릭 방지)
                btnLogin.Enabled = false;
                btnSignup.Enabled = false;
                btnLogin.Text = "로그인 중...";

                // 서버에 로그인 요청
                var loginResult = await LoginToServer(inputID, inputPW);

                if (loginResult.success)
                {
                    // 로그인 성공
                    MessageBox.Show($"{loginResult.userName}님 환영합니다!", "로그인 성공",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 전역 변수에 사용자 정보 저장
                    LoggedInUserId = loginResult.userId;
                    LoggedInUserName = loginResult.userName;

                    // 메인 폼으로 이동
                    MainForm mainForm = new MainForm();
                    mainForm.InitializeAfterLogin(loginResult.userId, loginResult.userName);
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    // 로그인 실패
                    MessageBox.Show("아이디 또는 비밀번호가 올바르지 않습니다.", "로그인 실패",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPW.Clear();
                    txtPW.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버 연결에 실패했습니다.\n{ex.Message}", "연결 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 버튼 다시 활성화
                btnLogin.Enabled = true;
                btnSignup.Enabled = true;
                btnLogin.Text = "Login";
            }
        }

        // 서버에 로그인 요청
        private async Task<(bool success, string userId, string userName)> LoginToServer(string id, string password)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync("223.194.44.94", 5000);
                    NetworkStream ns = client.GetStream();

                    // 요청 데이터 준비
                    string tempUserId = "000000"; // 로그인 시에는 임시 ID 사용
                    byte opcode = 2; // 로그인 opcode
                    List<string> items = new List<string> { id, password };

                    // 패킷 전송
                    await SendPacketAsync(ns, tempUserId, opcode, items);

                    // 응답 수신
                    var responses = await ReadAllResponsesAsync(ns);

                    if (responses.Count >= 2 && responses[1] == "1") // 로그인 성공
                    {
                        // 첫 번째 응답은 User_Info 데이터
                        var userInfo = DeserializeUserInfo(responses[0]);
                        return (true, userInfo.User_Id, userInfo.Name);
                    }
                    else
                    {
                        return (false, "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"로그인 오류: {ex.Message}");
                throw;
            }
        }

        // User_Info 역직렬화
        private UserInfo DeserializeUserInfo(string json)
        {
            var ser = new DataContractJsonSerializer(typeof(UserInfo));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (UserInfo)ser.ReadObject(ms);
            }
        }

        // 패킷 전송 메서드
        private async Task SendPacketAsync(NetworkStream ns, string userId, byte opcode, List<string> items)
        {
            byte[] userBytes = Encoding.ASCII.GetBytes(userId.PadLeft(6, '0'));
            Encoding utf8 = Encoding.UTF8;
            byte[][] data = items.Select(p => utf8.GetBytes(p)).ToArray();

            int len = 1 + 6 + 1 + (items.Count * 1);
            len += data.Sum(b => b.Length);

            byte[] packet = new byte[len];
            int pos = 0;

            // opcode
            packet[pos++] = opcode;

            // user ID (6 bytes)
            Buffer.BlockCopy(userBytes, 0, packet, pos, 6);
            pos += 6;

            // items count
            packet[pos++] = (byte)items.Count;

            // 각 item의 길이
            foreach (var b in data)
                packet[pos++] = (byte)b.Length;

            // 실제 데이터
            foreach (var b in data)
            {
                Buffer.BlockCopy(b, 0, packet, pos, b.Length);
                pos += b.Length;
            }

            await ns.WriteAsync(packet, 0, packet.Length);
        }

        // 모든 응답 수신 메서드
        private async Task<List<string>> ReadAllResponsesAsync(NetworkStream ns)
        {
            List<string> responses = new List<string>();

            while (true)
            {
                byte[] stateBuf = new byte[1];
                int n = await ns.ReadAsync(stateBuf, 0, 1);

                if (n == 0 || stateBuf[0] == 0) // 에러 또는 연결 종료
                    break;

                byte[] lenBuf = new byte[1];
                await ns.ReadAsync(lenBuf, 0, 1);

                byte[] dataBuf = new byte[lenBuf[0]];
                await ns.ReadAsync(dataBuf, 0, lenBuf[0]);

                string data = Encoding.UTF8.GetString(dataBuf);
                responses.Add(data);

                if (stateBuf[0] == 1) // 마지막 데이터
                    break;
            }

            return responses;
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            SignupForm signupForm = new SignupForm();
            if (signupForm.ShowDialog() == DialogResult.OK)
            {
                // 회원가입 성공 시 메시지 표시
                MessageBox.Show("회원가입이 완료되었습니다. 로그인해주세요.", "회원가입 성공",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (string.IsNullOrEmpty(txtPW.Text))
                {
                    txtPW.Focus();
                }
                else
                {
                    btnLogin.PerformClick();
                }
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
            // 비밀번호 입력 시 처리할 내용
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 로그인 폼이 닫힐 때 애플리케이션 종료
            Application.Exit();
        }
    }

    // User_Info 데이터 구조 (서버의 User_Info와 일치해야 함)
    [DataContract]
    internal class UserInfo
    {
        [DataMember] internal string User_Id;
        [DataMember] internal string Name;
        [DataMember] internal string Nickname;
        [DataMember] internal string Profile_Image_Path;
        [DataMember] internal List<string> Chat_Room_List;
        [DataMember] internal List<string> Waiting_Chat_Room_List;
    }
}