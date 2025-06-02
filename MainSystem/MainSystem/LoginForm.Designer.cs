namespace MainSystem
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtPW = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnSignup = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pbApp = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbApp)).BeginInit();
            this.SuspendLayout();
            // 
            // txtID
            // 
            this.txtID.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtID.Font = new System.Drawing.Font("돋움", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtID.Location = new System.Drawing.Point(16, 300);
            this.txtID.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.txtID.MaxLength = 30;
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(312, 31);
            this.txtID.TabIndex = 0;
            this.txtID.WordWrap = false;
            this.txtID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtID_KeyDown);
            // 
            // txtPW
            // 
            this.txtPW.AcceptsReturn = true;
            this.txtPW.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPW.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPW.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPW.Font = new System.Drawing.Font("돋움", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtPW.Location = new System.Drawing.Point(16, 362);
            this.txtPW.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPW.MaxLength = 30;
            this.txtPW.Name = "txtPW";
            this.txtPW.Size = new System.Drawing.Size(312, 31);
            this.txtPW.TabIndex = 1;
            this.txtPW.UseSystemPasswordChar = true;
            this.txtPW.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPW_KeyDown);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.btnLogin.Font = new System.Drawing.Font("돋움", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLogin.ForeColor = System.Drawing.SystemColors.Control;
            this.btnLogin.Location = new System.Drawing.Point(42, 399);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(90, 40);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnSignup
            // 
            this.btnSignup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.btnSignup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSignup.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(202)))), ((int)(((byte)(224)))));
            this.btnSignup.Font = new System.Drawing.Font("돋움", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSignup.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnSignup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSignup.Location = new System.Drawing.Point(214, 399);
            this.btnSignup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSignup.Name = "btnSignup";
            this.btnSignup.Size = new System.Drawing.Size(90, 40);
            this.btnSignup.TabIndex = 3;
            this.btnSignup.Text = "SignUp";
            this.btnSignup.UseVisualStyleBackColor = false;
            this.btnSignup.Click += new System.EventHandler(this.btnSignup_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(102)))));
            this.label1.Location = new System.Drawing.Point(16, 278);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = " ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(102)))));
            this.label2.Location = new System.Drawing.Point(18, 340);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "PW";
            // 
            // pbApp
            // 
            this.pbApp.BackgroundImage = global::MainSystem.Properties.Resources.main;
            this.pbApp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbApp.ErrorImage = null;
            this.pbApp.Image = global::MainSystem.Properties.Resources.main;
            this.pbApp.ImageLocation = "C:\\Users\\admin\\Desktop\\ChatProject_Client\\MainSystem\\MainSystem\\bin\\Debug\\main.pn" +
    "g";
            this.pbApp.Location = new System.Drawing.Point(16, 15);
            this.pbApp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pbApp.Name = "pbApp";
            this.pbApp.Size = new System.Drawing.Size(312, 278);
            this.pbApp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbApp.TabIndex = 4;
            this.pbApp.TabStop = false;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(202)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(342, 452);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSignup);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPW);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.pbApp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "LoginForm";
            this.Text = "ChatMoa";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LoginForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pbApp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtPW;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnSignup;
        private System.Windows.Forms.PictureBox pbApp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

