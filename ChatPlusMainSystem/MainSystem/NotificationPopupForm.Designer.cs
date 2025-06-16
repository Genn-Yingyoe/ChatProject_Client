using System;
using System.Drawing;
using System.Windows.Forms;

namespace MainSystem
{
    internal partial class NotificationPopupForm  // internal로 변경
    {
        private System.ComponentModel.IContainer components = null;
        private PictureBox picIcon;
        private Label lblTitle;
        private Label lblMessage;
        private Label lblDate;
        private Button btnAccept;
        private Button btnReject;

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
            this.picIcon = new PictureBox();
            this.lblTitle = new Label();
            this.lblMessage = new Label();
            this.lblDate = new Label();
            this.btnAccept = new Button();
            this.btnReject = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            this.SuspendLayout();

            // 
            // picIcon
            // 
            this.picIcon.BackColor = Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.picIcon.BorderStyle = BorderStyle.FixedSingle;
            this.picIcon.Location = new Point(15, 15);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new Size(64, 64);
            this.picIcon.SizeMode = PictureBoxSizeMode.Zoom;
            this.picIcon.TabIndex = 0;
            this.picIcon.TabStop = false;

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("맑은 고딕", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.Location = new Point(95, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(50, 25);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "알림";

            // 
            // lblMessage
            // 
            this.lblMessage.Font = new Font("맑은 고딕", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            this.lblMessage.Location = new Point(95, 45);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new Size(280, 40);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "알림 메시지";

            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new Font("맑은 고딕", 8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            this.lblDate.ForeColor = Color.Gray;
            this.lblDate.Location = new Point(95, 90);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new Size(57, 13);
            this.lblDate.TabIndex = 3;
            this.lblDate.Text = "2024-01-01";

            // 
            // btnAccept
            // 
            this.btnAccept.BackColor = Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(47)))), ((int)(((byte)(102)))));
            this.btnAccept.FlatAppearance.BorderSize = 0;
            this.btnAccept.FlatStyle = FlatStyle.Flat;
            this.btnAccept.Font = new Font("맑은 고딕", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.btnAccept.ForeColor = Color.White;
            this.btnAccept.Location = new Point(220, 120);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new Size(75, 30);
            this.btnAccept.TabIndex = 4;
            this.btnAccept.Text = "수락";
            this.btnAccept.UseVisualStyleBackColor = false;
            this.btnAccept.Click += new EventHandler(this.btnAccept_Click);

            // 
            // btnReject
            // 
            this.btnReject.BackColor = Color.Gray;
            this.btnReject.FlatAppearance.BorderSize = 0;
            this.btnReject.FlatStyle = FlatStyle.Flat;
            this.btnReject.Font = new Font("맑은 고딕", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            this.btnReject.ForeColor = Color.White;
            this.btnReject.Location = new Point(300, 120);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new Size(75, 30);
            this.btnReject.TabIndex = 5;
            this.btnReject.Text = "거절";
            this.btnReject.UseVisualStyleBackColor = false;
            this.btnReject.Click += new EventHandler(this.btnReject_Click);

            // 
            // NotificationPopupForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(202)))), ((int)(((byte)(224)))));
            this.ClientSize = new Size(390, 165);
            this.Controls.Add(this.btnReject);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.picIcon);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NotificationPopupForm";
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Text = "알림";
            this.TopMost = true;
            this.Load += new EventHandler(this.NotificationPopupForm_Load);
            this.FormClosed += new FormClosedEventHandler(this.NotificationPopupForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}