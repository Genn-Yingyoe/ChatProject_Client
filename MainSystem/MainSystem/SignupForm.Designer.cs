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
            this.lblNewID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPWQ = new System.Windows.Forms.TextBox();
            this.txtPWA = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtNewID
            // 
            this.txtNewID.Location = new System.Drawing.Point(14, 29);
            this.txtNewID.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNewID.Name = "txtNewID";
            this.txtNewID.Size = new System.Drawing.Size(212, 25);
            this.txtNewID.TabIndex = 1;
            this.txtNewID.TextChanged += new System.EventHandler(this.txtNewID_TextChanged);
            // 
            // txtNewPW
            // 
            this.txtNewPW.Location = new System.Drawing.Point(14, 75);
            this.txtNewPW.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNewPW.Name = "txtNewPW";
            this.txtNewPW.Size = new System.Drawing.Size(212, 25);
            this.txtNewPW.TabIndex = 2;
            this.txtNewPW.UseSystemPasswordChar = true;
            this.txtNewPW.TextChanged += new System.EventHandler(this.txtNewPW_TextChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(14, 260);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(212, 25);
            this.txtName.TabIndex = 3;
            // 
            // txtNick
            // 
            this.txtNick.Location = new System.Drawing.Point(16, 306);
            this.txtNick.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNick.Name = "txtNick";
            this.txtNick.Size = new System.Drawing.Size(210, 25);
            this.txtNick.TabIndex = 4;
            // 
            // btnRegister
            // 
            this.btnRegister.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.btnRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRegister.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRegister.Location = new System.Drawing.Point(1, 371);
            this.btnRegister.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(344, 79);
            this.btnRegister.TabIndex = 5;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // lblNewID
            // 
            this.lblNewID.AutoSize = true;
            this.lblNewID.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblNewID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.lblNewID.Location = new System.Drawing.Point(14, 11);
            this.lblNewID.Name = "lblNewID";
            this.lblNewID.Size = new System.Drawing.Size(59, 15);
            this.lblNewID.TabIndex = 6;
            this.lblNewID.Text = "New ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.label2.Location = new System.Drawing.Point(14, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "New PW";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.label3.Location = new System.Drawing.Point(14, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Repeat your password";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.label4.Location = new System.Drawing.Point(14, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(211, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Account password question";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.label5.Location = new System.Drawing.Point(14, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "Answer of your question";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.label6.Location = new System.Drawing.Point(14, 242);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "Name";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(14, 121);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(212, 25);
            this.textBox1.TabIndex = 12;
            this.textBox1.UseSystemPasswordChar = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.label7.Location = new System.Drawing.Point(14, 289);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "NickName";
            // 
            // txtPWQ
            // 
            this.txtPWQ.Location = new System.Drawing.Point(14, 168);
            this.txtPWQ.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPWQ.Name = "txtPWQ";
            this.txtPWQ.Size = new System.Drawing.Size(212, 25);
            this.txtPWQ.TabIndex = 14;
            // 
            // txtPWA
            // 
            this.txtPWA.Location = new System.Drawing.Point(14, 214);
            this.txtPWA.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPWA.Name = "txtPWA";
            this.txtPWA.Size = new System.Drawing.Size(212, 25);
            this.txtPWA.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(233, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "label1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(233, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 15);
            this.label8.TabIndex = 17;
            this.label8.Text = "label8";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(233, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 15);
            this.label9.TabIndex = 18;
            this.label9.Text = "label9";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(233, 171);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 15);
            this.label10.TabIndex = 19;
            this.label10.Text = "label10";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(233, 218);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 15);
            this.label11.TabIndex = 20;
            this.label11.Text = "label11";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(233, 264);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 15);
            this.label12.TabIndex = 21;
            this.label12.Text = "label12";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(233, 310);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 15);
            this.label13.TabIndex = 22;
            this.label13.Text = "label13";
            // 
            // SignupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(202)))), ((int)(((byte)(224)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(344, 450);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPWA);
            this.Controls.Add(this.txtPWQ);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblNewID);
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
        private System.Windows.Forms.Label lblNewID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPWQ;
        private System.Windows.Forms.TextBox txtPWA;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
    }
}