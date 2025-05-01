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

namespace Login___Signup
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

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string newID = txtNewID.Text.Trim();
            string newPW = txtNewPW.Text.Trim();
            string newName = txtName.Text.Trim();
            string newNick = txtNick.Text.Trim();

            if (string.IsNullOrEmpty(newID) || string.IsNullOrEmpty(newPW) || string.IsNullOrEmpty(newName) || string.IsNullOrEmpty(newNick))
            {
                MessageBox.Show("모든 항목을 입력해주세요.");
                return;
            }

            if (!File.Exists(UserDBPath))
            {
                File.Create(UserDBPath).Close();
            }

            var lines = File.ReadAllLines(UserDBPath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts[0] == newID)
                {
                    MessageBox.Show("중복된 아이디입니다.");
                    return;
                }
                if (parts[5] == newNick)
                {
                    MessageBox.Show("이미 존재하는 닉네임입니다.");
                    return;
                }
            }

            string newHashTag = GenerateUniqueHashTag(lines);
            string newCode = GenerateUniqueCode(lines);

            string newUserLine = $"{newID},{newPW},{newHashTag},{newCode},{newName},{newNick}"; //txt파일 schema
            File.AppendAllText(UserDBPath, newUserLine + Environment.NewLine);

            MessageBox.Show("회원가입 성공!");
            this.Close();
        }

        private string GenerateUniqueHashTag(string[] existingUsers)
        {
            while (true)
            {
                string tag = random.Next(000000, 999999).ToString();
                if (!existingUsers.Any(u => u.Split(',')[2] == tag))
                    return tag;
            }
        }

        private string GenerateUniqueCode(string[] existingUsers)
        {
            while (true)
            {
                string code = random.Next(0x00000000, 0x7FFFFFFF).ToString("X8");
                if (!existingUsers.Any(u => u.Split(',')[3] == code))
                    return code;
            }
        }
    }
}
