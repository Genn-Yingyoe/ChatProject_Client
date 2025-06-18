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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Reflection;
using ChatMoa_DataBaseServer;  // DCM 네임스페이스 추가

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
        internal static DCM GlobalDCM;  // internal로 변경하여 DCM과 액세스 가능성 일치

        public LoginForm()
        {
            InitializeComponent();
            InitializeForm();
            GlobalDCM = new DCM();  // DCM 인스턴스 생성
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

                // DCM을 사용하여 로그인 요청
                var loginResult = await LoginToServerWithDCM(inputID, inputPW);

                if (loginResult.success)
                {
                    // 로그인 성공
                    MessageBox.Show($"{loginResult.userName}님 환영합니다!", "로그인 성공",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 전역 변수에 사용자 정보 저장
                    LoggedInUserId = loginResult.userId;
                    LoggedInUserName = loginResult.userName;

                    // DCM에 사용자 ID 설정 (리플렉션 사용)
                    SetDCMUserId(GlobalDCM, loginResult.userId);

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

        // DCM을 사용한 로그인 메서드
        private async Task<(bool success, string userId, string userName)> LoginToServerWithDCM(string id, string password)
        {
            try
            {
                // 임시 DCM 인스턴스로 로그인 요청 (아직 user_id가 없으므로)
                DCM tempDcm = new DCM();
                SetDCMUserId(tempDcm, "000000");  // 임시 ID

                // opcode 2: 로그인
                List<string> items = new List<string> { id, password };
                var result = await tempDcm.db_request_data(2, items);

                if (result.Key && result.Value.Item2.Count > 0)
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    // 마지막 응답이 성공("1")인지 확인
                    string lastResponse = GetDCMResponseData(tempDcm, key, indexes.Last());

                    if (lastResponse == "1" && indexes.Count >= 2)
                    {
                        // 첫 번째 응답은 User_Info 데이터
                        var userInfo = DeserializeDCMJson<UserInfo>(tempDcm, key, indexes[0]);

                        // 정리
                        ClearDCMReceivedData(tempDcm, key);

                        return (true, userInfo.User_Id, userInfo.Name);
                    }
                    else
                    {
                        ClearDCMReceivedData(tempDcm, key);
                        return (false, "", "");
                    }
                }
                else
                {
                    return (false, "", "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"로그인 오류: {ex.Message}");
                throw;
            }
        }

        // DCM의 private 메서드들에 접근하기 위한 리플렉션 헬퍼 메서드들
        private void SetDCMUserId(DCM dcm, string userId)
        {
            try
            {
                var loginMethod = dcm.GetType().GetMethod("Login",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                loginMethod?.Invoke(dcm, new object[] { userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DCM Login 설정 오류: {ex.Message}");
            }
        }

        private string GetDCMResponseData(DCM dcm, int key, int index)
        {
            try
            {
                var receivedDataField = dcm.GetType().GetField("received_data",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (receivedDataField != null)
                {
                    var receivedData = receivedDataField.GetValue(dcm) as Dictionary<int, List<string>>;
                    if (receivedData != null && receivedData.ContainsKey(key))
                    {
                        var dataList = receivedData[key];
                        if (index < dataList.Count)
                        {
                            return dataList[index];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DCM 응답 데이터 조회 오류: {ex.Message}");
            }

            return "0"; // 기본값: 실패
        }

        private void ClearDCMReceivedData(DCM dcm, int key)
        {
            try
            {
                var clearMethod = dcm.GetType().GetMethod("Clear_receive_data",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                clearMethod?.Invoke(dcm, new object[] { key });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DCM 데이터 정리 오류: {ex.Message}");
            }
        }

        private T DeserializeDCMJson<T>(DCM dcm, int key, int index)
        {
            try
            {
                var deserializeMethod = dcm.GetType().GetMethod("DeSerializeJson",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (deserializeMethod != null)
                {
                    var genericMethod = deserializeMethod.MakeGenericMethod(typeof(T));
                    return (T)genericMethod.Invoke(dcm, new object[] { key, index });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DCM JSON 역직렬화 오류: {ex.Message}");
            }

            return default(T);
        }

        // 전역 DCM 헬퍼 메서드들 - 다른 폼에서 사용
        public static string GetGlobalDCMResponseData(int key, int index)
        {
            try
            {
                var receivedDataField = GlobalDCM.GetType().GetField("received_data",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (receivedDataField != null)
                {
                    var receivedData = receivedDataField.GetValue(GlobalDCM) as Dictionary<int, List<string>>;
                    if (receivedData != null && receivedData.ContainsKey(key))
                    {
                        var dataList = receivedData[key];
                        if (index < dataList.Count)
                        {
                            return dataList[index];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"전역 DCM 응답 데이터 조회 오류: {ex.Message}");
            }

            return "0";
        }

        public static void ClearGlobalDCMReceivedData(int key)
        {
            try
            {
                var clearMethod = GlobalDCM.GetType().GetMethod("Clear_receive_data",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                clearMethod?.Invoke(GlobalDCM, new object[] { key });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"전역 DCM 데이터 정리 오류: {ex.Message}");
            }
        }

        public static T DeserializeGlobalDCMJson<T>(int key, int index)
        {
            try
            {
                var deserializeMethod = GlobalDCM.GetType().GetMethod("DeSerializeJson",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (deserializeMethod != null)
                {
                    var genericMethod = deserializeMethod.MakeGenericMethod(typeof(T));
                    return (T)genericMethod.Invoke(GlobalDCM, new object[] { key, index });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"전역 DCM JSON 역직렬화 오류: {ex.Message}");
            }

            return default(T);
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