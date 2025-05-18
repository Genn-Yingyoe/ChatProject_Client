using System.Drawing;
using System.Windows.Forms;

namespace Client
{
    partial class ChatForm
    {
        private System.ComponentModel.IContainer components = null;

        // UI 필드
        private TableLayoutPanel chatLayout;
        private Panel pnlInput;
        private TextBox txtMessage;
        private Button btnSend;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.chatLayout = new TableLayoutPanel();
            this.pnlInput = new Panel();
            this.txtMessage = new TextBox();
            this.btnSend = new Button();

            // chatLayout
            this.chatLayout.ColumnCount = 2;
            this.chatLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.chatLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.chatLayout.Dock = DockStyle.Fill;
            this.chatLayout.AutoScroll = true;
            this.chatLayout.BackColor = Color.White;
            this.chatLayout.Padding = new Padding(10);
            this.chatLayout.Location = new Point(0, 0);
            this.chatLayout.Name = "chatLayout";
            this.chatLayout.RowCount = 1;
            this.chatLayout.RowStyles.Add(new RowStyle());

            // pnlInput
            this.pnlInput.Dock = DockStyle.Bottom;
            this.pnlInput.Height = 75;
            this.pnlInput.BackColor = Color.LightGray;
            this.pnlInput.Padding = new Padding(10);
            this.pnlInput.Name = "pnlInput";

            // txtMessage
            this.txtMessage.Multiline = true;
            this.txtMessage.Font = new Font("맑은 고딕", 10);
            this.txtMessage.Dock = DockStyle.Fill;
            this.txtMessage.ScrollBars = ScrollBars.Vertical;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.KeyDown += new KeyEventHandler(this.TxtMessage_KeyDown);

            // btnSend
            this.btnSend.Text = "전송";
            this.btnSend.Width = 80;
            this.btnSend.Dock = DockStyle.Right;
            this.btnSend.Name = "btnSend";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);

            // 조립
            this.pnlInput.Controls.Add(this.txtMessage);
            this.pnlInput.Controls.Add(this.btnSend);
            this.Controls.Add(this.chatLayout);
            this.Controls.Add(this.pnlInput);

            // ChatForm
            this.Text = "채팅 클라이언트";
            this.BackColor = Color.White;
            this.ClientSize = new Size(500, 600);
            this.Resize += new System.EventHandler(this.ChatForm_Resize);
        }
    }
}
