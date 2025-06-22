using System;
using System.Drawing;
using System.Windows.Forms;

namespace MainSystem
{
    partial class ChatForm
    {
        private System.ComponentModel.IContainer components = null;
        private FlowLayoutPanel flpChatList;
        private TextBox txtMessage;
        private Button btnSend;
        private Button btnInvite;  // 초대 버튼
        private Button btnExit;    // 나가기 버튼 추가
        private Label lblRoomInfo;
        private Panel pnlBottom;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblRoomInfo = new Label();
            this.btnExit = new Button();        // 나가기 버튼 추가
            this.flpChatList = new FlowLayoutPanel();
            this.pnlBottom = new Panel();
            this.txtMessage = new TextBox();
            this.btnSend = new Button();
            this.btnInvite = new Button();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();

            // 
            // lblRoomInfo
            // 
            this.lblRoomInfo.BackColor = Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.lblRoomInfo.Dock = DockStyle.Top;
            this.lblRoomInfo.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.lblRoomInfo.ForeColor = Color.White;
            this.lblRoomInfo.Location = new Point(0, 0);
            this.lblRoomInfo.Name = "lblRoomInfo";
            this.lblRoomInfo.Padding = new Padding(10, 8, 10, 8);
            this.lblRoomInfo.Size = new Size(800, 40);
            this.lblRoomInfo.TabIndex = 0;
            this.lblRoomInfo.Text = "채팅방";
            this.lblRoomInfo.TextAlign = ContentAlignment.MiddleLeft;

            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.btnExit.BackColor = Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = FlatStyle.Flat;
            this.btnExit.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.btnExit.ForeColor = Color.White;
            this.btnExit.Location = new Point(720, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new Size(70, 30);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "나가기";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new EventHandler(this.btnExit_Click);

            // 
            // flpChatList
            // 
            this.flpChatList.AutoScroll = true;
            this.flpChatList.BackColor = Color.White;
            this.flpChatList.BorderStyle = BorderStyle.FixedSingle;
            this.flpChatList.Dock = DockStyle.Fill;
            this.flpChatList.FlowDirection = FlowDirection.TopDown;
            this.flpChatList.Location = new Point(0, 40);
            this.flpChatList.Name = "flpChatList";
            this.flpChatList.Padding = new Padding(5);
            this.flpChatList.Size = new Size(800, 460);
            this.flpChatList.TabIndex = 1;
            this.flpChatList.WrapContents = false;
            this.flpChatList.Resize += new EventHandler(this.flpChatList_Resize);

            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlBottom.Controls.Add(this.btnInvite);
            this.pnlBottom.Controls.Add(this.btnSend);
            this.pnlBottom.Controls.Add(this.txtMessage);
            this.pnlBottom.Dock = DockStyle.Bottom;
            this.pnlBottom.Location = new Point(0, 500);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Padding = new Padding(10);
            this.pnlBottom.Size = new Size(800, 60);
            this.pnlBottom.TabIndex = 2;

            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.txtMessage.BorderStyle = BorderStyle.FixedSingle;
            this.txtMessage.Font = new Font("맑은 고딕", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            this.txtMessage.Location = new Point(10, 15);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new Size(610, 30);
            this.txtMessage.TabIndex = 0;
            this.txtMessage.KeyDown += new KeyEventHandler(this.txtMessage_KeyDown);

            // 
            // btnInvite
            // 
            this.btnInvite.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.btnInvite.BackColor = Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(120)))), ((int)(((byte)(50)))));
            this.btnInvite.FlatAppearance.BorderSize = 0;
            this.btnInvite.FlatStyle = FlatStyle.Flat;
            this.btnInvite.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.btnInvite.ForeColor = Color.White;
            this.btnInvite.Location = new Point(630, 15);
            this.btnInvite.Name = "btnInvite";
            this.btnInvite.Size = new Size(75, 30);
            this.btnInvite.TabIndex = 2;
            this.btnInvite.Text = "초대";
            this.btnInvite.UseVisualStyleBackColor = false;
            this.btnInvite.Click += new EventHandler(this.btnInvite_Click);

            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.btnSend.BackColor = Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle = FlatStyle.Flat;
            this.btnSend.Font = new Font("맑은 고딕", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.btnSend.ForeColor = Color.White;
            this.btnSend.Location = new Point(710, 15);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new Size(75, 30);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "전송";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new EventHandler(this.btnSend_Click);

            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(202)))), ((int)(((byte)(224)))));
            this.ClientSize = new Size(800, 560);
            this.Controls.Add(this.btnExit);      // 나가기 버튼을 폼에 추가
            this.Controls.Add(this.flpChatList);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.lblRoomInfo);
            this.MinimumSize = new Size(600, 400);
            this.Name = "ChatForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "ChatForm";
            this.FormClosed += new FormClosedEventHandler(this.ChatForm_FormClosed);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}