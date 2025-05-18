namespace Client
{
    partial class LoginForm
    {
        private System.Windows.Forms.TextBox txtNickname;
        private System.Windows.Forms.Button btnOK;

        private void InitializeComponent()
        {
            this.txtNickname = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // txtNickname
            this.txtNickname.Location = new System.Drawing.Point(30, 30);
            this.txtNickname.Size = new System.Drawing.Size(200, 25);

            // btnOK
            this.btnOK.Text = "확인";
            this.btnOK.Location = new System.Drawing.Point(240, 30);
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);

            // LoginForm
            this.Text = "닉네임 입력";
            this.ClientSize = new System.Drawing.Size(350, 100);
            this.Controls.Add(this.txtNickname);
            this.Controls.Add(this.btnOK);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}