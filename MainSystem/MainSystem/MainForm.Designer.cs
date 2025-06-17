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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnChat = new System.Windows.Forms.Button();
            this.btnInv = new System.Windows.Forms.Button();
            this.flpMain = new System.Windows.Forms.FlowLayoutPanel();
            this.rdbFriend = new System.Windows.Forms.RadioButton();
            this.rdbChatroom = new System.Windows.Forms.RadioButton();
            this.btnCal = new System.Windows.Forms.Button();
            this.btnCon = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnChat
            // 
            this.btnChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChat.Location = new System.Drawing.Point(54, 12);
            this.btnChat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnChat.Name = "btnChat";
            this.btnChat.Size = new System.Drawing.Size(49, 41);
            this.btnChat.TabIndex = 0;
            this.btnChat.Text = "NewChat";
            this.btnChat.UseVisualStyleBackColor = true;
            this.btnChat.Click += new System.EventHandler(this.btn1_Click);
            // 
            // btnInv
            // 
            this.btnInv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInv.Location = new System.Drawing.Point(109, 12);
            this.btnInv.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnInv.Name = "btnInv";
            this.btnInv.Size = new System.Drawing.Size(75, 41);
            this.btnInv.TabIndex = 1;
            this.btnInv.Text = "Friend Invitation";
            this.btnInv.UseVisualStyleBackColor = true;
            this.btnInv.Click += new System.EventHandler(this.button2_Click);
            // 
            // flpMain
            // 
            this.flpMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpMain.AutoScroll = true;
            this.flpMain.Location = new System.Drawing.Point(3, 145);
            this.flpMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flpMain.Name = "flpMain";
            this.flpMain.Size = new System.Drawing.Size(335, 304);
            this.flpMain.TabIndex = 2;
            this.flpMain.Paint += new System.Windows.Forms.PaintEventHandler(this.flpMain_Paint);
            // 
            // rdbFriend
            // 
            this.rdbFriend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rdbFriend.AutoSize = true;
            this.rdbFriend.Checked = true;
            this.rdbFriend.Location = new System.Drawing.Point(75, 84);
            this.rdbFriend.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbFriend.Name = "rdbFriend";
            this.rdbFriend.Size = new System.Drawing.Size(58, 19);
            this.rdbFriend.TabIndex = 3;
            this.rdbFriend.TabStop = true;
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
            this.rdbChatroom.Location = new System.Drawing.Point(191, 84);
            this.rdbChatroom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbChatroom.Name = "rdbChatroom";
            this.rdbChatroom.Size = new System.Drawing.Size(73, 19);
            this.rdbChatroom.TabIndex = 4;
            this.rdbChatroom.Text = "채팅방";
            this.rdbChatroom.UseVisualStyleBackColor = true;
            this.rdbChatroom.CheckedChanged += new System.EventHandler(this.rdbChatroom_CheckedChanged);
            // 
            // btnCal
            // 
            this.btnCal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCal.Location = new System.Drawing.Point(191, 12);
            this.btnCal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCal.Name = "btnCal";
            this.btnCal.Size = new System.Drawing.Size(81, 41);
            this.btnCal.TabIndex = 5;
            this.btnCal.Text = "Calendar";
            this.btnCal.UseVisualStyleBackColor = true;
            // 
            // btnCon
            // 
            this.btnCon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCon.Location = new System.Drawing.Point(278, 12);
            this.btnCon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCon.Name = "btnCon";
            this.btnCon.Size = new System.Drawing.Size(59, 41);
            this.btnCon.TabIndex = 6;
            this.btnCon.Text = "Config";
            this.btnCon.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 452);
            this.Controls.Add(this.btnCon);
            this.Controls.Add(this.btnCal);
            this.Controls.Add(this.rdbChatroom);
            this.Controls.Add(this.rdbFriend);
            this.Controls.Add(this.flpMain);
            this.Controls.Add(this.btnInv);
            this.Controls.Add(this.btnChat);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "ChatMoa";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnChat;
        private System.Windows.Forms.Button btnInv;
        private System.Windows.Forms.FlowLayoutPanel flpMain;
        private System.Windows.Forms.RadioButton rdbFriend;
        private System.Windows.Forms.RadioButton rdbChatroom;
        private System.Windows.Forms.Button btnCal;
        private System.Windows.Forms.Button btnCon;
    }
}

