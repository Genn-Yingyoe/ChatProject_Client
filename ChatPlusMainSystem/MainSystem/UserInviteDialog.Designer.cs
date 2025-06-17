using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MainSystem
{
    partial class UserInviteDialog
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblUserId;
        private TextBox txtUserId;
        private Button btnOK;
        private Button btnCancel;

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
            this.lblTitle = new Label();
            this.lblUserId = new Label();
            this.txtUserId = new TextBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.Location = new Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(180, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "채팅방에 초대할 사용자 ID를 입력하세요";

            // 
            // lblUserId
            // 
            this.lblUserId.AutoSize = true;
            this.lblUserId.Font = new Font("맑은 고딕", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            this.lblUserId.Location = new Point(20, 60);
            this.lblUserId.Name = "lblUserId";
            this.lblUserId.Size = new Size(100, 17);
            this.lblUserId.TabIndex = 1;
            this.lblUserId.Text = "사용자 ID (6자리):";

            // 
            // txtUserId
            // 
            this.txtUserId.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            this.txtUserId.Location = new Point(20, 85);
            this.txtUserId.MaxLength = 6;
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new Size(300, 29);
            this.txtUserId.TabIndex = 2;
            this.txtUserId.TextAlign = HorizontalAlignment.Center;
            this.txtUserId.KeyDown += new KeyEventHandler(this.txtUserId_KeyDown);

            // 
            // btnOK
            // 
            this.btnOK.BackColor = Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.Font = new Font("맑은 고딕", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ForeColor = Color.White;
            this.btnOK.Location = new Point(165, 130);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(75, 30);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "초대";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);

            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = Color.Gray;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Font = new Font("맑은 고딕", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.Location = new Point(245, 130);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 30);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // 
            // UserInviteDialog
            // 
            this.AutoScaleDimensions = new SizeF(7F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(202)))), ((int)(((byte)(224)))));
            this.ClientSize = new Size(340, 175);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtUserId);
            this.Controls.Add(this.lblUserId);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserInviteDialog";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "사용자 초대";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}