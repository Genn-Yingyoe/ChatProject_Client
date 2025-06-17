using System;
using System.Drawing;
using System.Windows.Forms;

namespace MainSystem
{
    partial class FriendSelectForm
    {
        private System.ComponentModel.IContainer components = null;
        private FlowLayoutPanel flpFriends;
        private Button btnOK;
        private Button btnCancel;
        private Label lblTitle;

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
            this.flpFriends = new FlowLayoutPanel();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.Location = new Point(12, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(180, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "채팅에 초대할 친구를 선택하세요";

            // 
            // flpFriends
            // 
            this.flpFriends.AutoScroll = true;
            this.flpFriends.BackColor = Color.White;
            this.flpFriends.BorderStyle = BorderStyle.FixedSingle;
            this.flpFriends.FlowDirection = FlowDirection.TopDown;
            this.flpFriends.Location = new Point(12, 50);
            this.flpFriends.Name = "flpFriends";
            this.flpFriends.Size = new Size(310, 300);
            this.flpFriends.TabIndex = 1;
            this.flpFriends.WrapContents = false;

            // 
            // btnOK
            // 
            this.btnOK.BackColor = Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.btnOK.Enabled = false;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.Font = new Font("맑은 고딕", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ForeColor = Color.White;
            this.btnOK.Location = new Point(167, 370);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(75, 30);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "확인";
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
            this.btnCancel.Location = new Point(247, 370);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // 
            // FriendSelectForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(334, 411);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.flpFriends);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FriendSelectForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "친구 선택";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}