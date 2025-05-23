namespace MainSystem
{
    partial class SignupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignupForm));
            this.txtNewID = new System.Windows.Forms.TextBox();
            this.txtNewPW = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtNick = new System.Windows.Forms.TextBox();
            this.btnRegister = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtNewID
            // 
            this.txtNewID.Location = new System.Drawing.Point(12, 142);
            this.txtNewID.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNewID.Name = "txtNewID";
            this.txtNewID.Size = new System.Drawing.Size(263, 21);
            this.txtNewID.TabIndex = 1;
            this.txtNewID.Text = "NewID\r\n";
            this.txtNewID.TextChanged += new System.EventHandler(this.txtNewID_TextChanged);
            // 
            // txtNewPW
            // 
            this.txtNewPW.Location = new System.Drawing.Point(12, 184);
            this.txtNewPW.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNewPW.Name = "txtNewPW";
            this.txtNewPW.Size = new System.Drawing.Size(263, 21);
            this.txtNewPW.TabIndex = 2;
            this.txtNewPW.Text = "NewPW";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(12, 221);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(263, 21);
            this.txtName.TabIndex = 3;
            this.txtName.Text = "Name";
            // 
            // txtNick
            // 
            this.txtNick.Location = new System.Drawing.Point(12, 259);
            this.txtNick.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNick.Name = "txtNick";
            this.txtNick.Size = new System.Drawing.Size(263, 21);
            this.txtNick.TabIndex = 4;
            this.txtNick.Text = "Nick";
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(0, 303);
            this.btnRegister.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(301, 55);
            this.btnRegister.TabIndex = 5;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // SignupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(202)))), ((int)(((byte)(224)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(301, 360);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.txtNick);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtNewPW);
            this.Controls.Add(this.txtNewID);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SignupForm";
            this.Text = "ChatMoa_SignUp";
            this.Load += new System.EventHandler(this.SignupForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtNewID;
        private System.Windows.Forms.TextBox txtNewPW;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtNick;
        private System.Windows.Forms.Button btnRegister;
    }
}