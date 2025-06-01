namespace Schedular_UI
{
    partial class CalendarForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button deleteButton;


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
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.SuspendLayout();
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(31, 16);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 0;
            // 
            this.listBox = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();

            // ListBox 위치 및 크기 설정
            this.listBox.Location = new System.Drawing.Point(20, 50);
            this.listBox.Size = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.listBox); // 폼에 추가

            // AddButton 설정
            this.addButton.Location = new System.Drawing.Point(20, 360);
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.Text = "일정 추가";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            this.Controls.Add(this.addButton);

            // DeleteButton 설정
            this.deleteButton.Location = new System.Drawing.Point(120, 360);
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.Text = "일정 삭제";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            this.Controls.Add(this.deleteButton);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.monthCalendar1);
            this.Name = "Form1";
            this.Text = "Calendar";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);



        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.ListBox listBox1;
    }
}

