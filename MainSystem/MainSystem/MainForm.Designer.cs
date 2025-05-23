namespace MainSystem
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnNewChat = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.flpMain = new System.Windows.Forms.FlowLayoutPanel();
            this.rdbFriend = new System.Windows.Forms.RadioButton();
            this.rdbChatroom = new System.Windows.Forms.RadioButton();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnNewChat
            // 
            this.btnNewChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewChat.Location = new System.Drawing.Point(47, 10);
            this.btnNewChat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnNewChat.Name = "btnNewChat";
            this.btnNewChat.Size = new System.Drawing.Size(43, 33);
            this.btnNewChat.TabIndex = 0;
            this.btnNewChat.Text = "NewChat";
            this.btnNewChat.UseVisualStyleBackColor = true;
            this.btnNewChat.Click += new System.EventHandler(this.btn1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(95, 10);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(66, 33);
            this.button2.TabIndex = 1;
            this.button2.Text = "Friend Invitation";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // flpMain
            // 
            this.flpMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpMain.AutoScroll = true;
            this.flpMain.Location = new System.Drawing.Point(3, 116);
            this.flpMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flpMain.Name = "flpMain";
            this.flpMain.Size = new System.Drawing.Size(293, 243);
            this.flpMain.TabIndex = 2;
            this.flpMain.Paint += new System.Windows.Forms.PaintEventHandler(this.flpMain_Paint);
            // 
            // rdbFriend
            // 
            this.rdbFriend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rdbFriend.AutoSize = true;
            this.rdbFriend.Location = new System.Drawing.Point(66, 67);
            this.rdbFriend.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbFriend.Name = "rdbFriend";
            this.rdbFriend.Size = new System.Drawing.Size(47, 16);
            this.rdbFriend.TabIndex = 3;
            this.rdbFriend.Text = "친구";
            this.rdbFriend.UseVisualStyleBackColor = true;
            this.rdbFriend.CheckedChanged += new System.EventHandler(this.rdbFriend_CheckedChanged);
            // 
            // rdbChatroom
            // 
            this.rdbChatroom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rdbChatroom.AutoCheck = false;
            this.rdbChatroom.AutoSize = true;
            this.rdbChatroom.Location = new System.Drawing.Point(167, 67);
            this.rdbChatroom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbChatroom.Name = "rdbChatroom";
            this.rdbChatroom.Size = new System.Drawing.Size(59, 16);
            this.rdbChatroom.TabIndex = 4;
            this.rdbChatroom.Text = "채팅방";
            this.rdbChatroom.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(167, 10);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(71, 33);
            this.button3.TabIndex = 5;
            this.button3.Text = "Calendar";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(243, 10);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(52, 33);
            this.button4.TabIndex = 6;
            this.button4.Text = "Config";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 362);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.rdbChatroom);
            this.Controls.Add(this.rdbFriend);
            this.Controls.Add(this.flpMain);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnNewChat);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "ChatMoa";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNewChat;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.FlowLayoutPanel flpMain;
        private System.Windows.Forms.RadioButton rdbFriend;
        private System.Windows.Forms.RadioButton rdbChatroom;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}

