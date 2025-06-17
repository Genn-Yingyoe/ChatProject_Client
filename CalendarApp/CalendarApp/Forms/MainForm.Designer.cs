namespace CalendarApp
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
            this.listViewSchedules = new System.Windows.Forms.ListView();
            this.IsComplete = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Category = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddSchedule = new System.Windows.Forms.Button();
            this.btnDeleteSchedule = new System.Windows.Forms.Button();
            this.btnTempLogin = new System.Windows.Forms.Button();
            this.txtScheduleContent = new System.Windows.Forms.TextBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // listViewSchedules
            // 
            this.listViewSchedules.CheckBoxes = true;
            this.listViewSchedules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IsComplete,
            this.Category,
            this.Title});
            this.listViewSchedules.FullRowSelect = true;
            this.listViewSchedules.HideSelection = false;
            this.listViewSchedules.Location = new System.Drawing.Point(11, 320);
            this.listViewSchedules.Name = "listViewSchedules";
            this.listViewSchedules.Size = new System.Drawing.Size(460, 360);
            this.listViewSchedules.TabIndex = 0;
            this.listViewSchedules.UseCompatibleStateImageBehavior = false;
            this.listViewSchedules.View = System.Windows.Forms.View.Details;
            this.listViewSchedules.SelectedIndexChanged += new System.EventHandler(this.listViewSchedules_SelectedIndexChanged);
            this.listViewSchedules.DoubleClick += new System.EventHandler(this.listViewSchedules_DoubleClick);
            // 
            // IsComplete
            // 
            this.IsComplete.Text = "완료";
            this.IsComplete.Width = 80;
            // 
            // Category
            // 
            this.Category.Text = "카테고리";
            this.Category.Width = 100;
            // 
            // Title
            // 
            this.Title.Text = "제목";
            this.Title.Width = 355;
            // 
            // btnAddSchedule
            // 
            this.btnAddSchedule.Location = new System.Drawing.Point(273, 693);
            this.btnAddSchedule.Name = "btnAddSchedule";
            this.btnAddSchedule.Size = new System.Drawing.Size(85, 46);
            this.btnAddSchedule.TabIndex = 1;
            this.btnAddSchedule.Text = "일정 추가";
            this.btnAddSchedule.UseVisualStyleBackColor = true;
            this.btnAddSchedule.Click += new System.EventHandler(this.btnAddSchedule_Click);
            // 
            // btnDeleteSchedule
            // 
            this.btnDeleteSchedule.Location = new System.Drawing.Point(374, 693);
            this.btnDeleteSchedule.Name = "btnDeleteSchedule";
            this.btnDeleteSchedule.Size = new System.Drawing.Size(85, 46);
            this.btnDeleteSchedule.TabIndex = 2;
            this.btnDeleteSchedule.Text = "일정 삭제";
            this.btnDeleteSchedule.UseVisualStyleBackColor = true;
            this.btnDeleteSchedule.Click += new System.EventHandler(this.btnDeleteSchedule_Click);
            // 
            // btnTempLogin
            // 
            this.btnTempLogin.Location = new System.Drawing.Point(33, 692);
            this.btnTempLogin.Name = "btnTempLogin";
            this.btnTempLogin.Size = new System.Drawing.Size(126, 23);
            this.btnTempLogin.TabIndex = 3;
            this.btnTempLogin.Text = "임시 로그인";
            this.btnTempLogin.UseVisualStyleBackColor = true;
            //this.btnTempLogin.Click += new System.EventHandler(this.btnTempLogin_Click);
            // 
            // txtScheduleContent
            // 
            this.txtScheduleContent.Location = new System.Drawing.Point(11, 530);
            this.txtScheduleContent.Multiline = true;
            this.txtScheduleContent.Name = "txtScheduleContent";
            this.txtScheduleContent.ReadOnly = true;
            this.txtScheduleContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtScheduleContent.Size = new System.Drawing.Size(460, 150);
            this.txtScheduleContent.TabIndex = 7;
            this.txtScheduleContent.Visible = false;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(18, 295);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(94, 19);
            this.chkSelectAll.TabIndex = 8;
            this.chkSelectAll.Text = "전체 선택";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 753);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.txtScheduleContent);
            this.Controls.Add(this.btnTempLogin);
            this.Controls.Add(this.btnDeleteSchedule);
            this.Controls.Add(this.btnAddSchedule);
            this.Controls.Add(this.listViewSchedules);
            this.Name = "MainForm";
            this.Text = "캘린더";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewSchedules;
        private System.Windows.Forms.Button btnAddSchedule;
        private System.Windows.Forms.Button btnDeleteSchedule;
        private System.Windows.Forms.ColumnHeader IsComplete;
        private System.Windows.Forms.ColumnHeader Category;
        private System.Windows.Forms.ColumnHeader Title;
        private System.Windows.Forms.Button btnTempLogin;
        private System.Windows.Forms.TextBox txtScheduleContent;
        private System.Windows.Forms.CheckBox chkSelectAll;
    }
}