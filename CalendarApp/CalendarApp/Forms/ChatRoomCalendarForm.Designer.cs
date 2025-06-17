namespace CalendarApp.Forms
{
    partial class ChatRoomCalendarForm
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
            this.btnDeleteSchedule = new System.Windows.Forms.Button();
            this.btnAddSchedule = new System.Windows.Forms.Button();
            this.listViewSchedules = new System.Windows.Forms.ListView();
            this.IsCompleted = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Category = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtScheduleContent = new System.Windows.Forms.TextBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnDeleteSchedule
            // 
            this.btnDeleteSchedule.Location = new System.Drawing.Point(374, 693);
            this.btnDeleteSchedule.Name = "btnDeleteSchedule";
            this.btnDeleteSchedule.Size = new System.Drawing.Size(85, 46);
            this.btnDeleteSchedule.TabIndex = 5;
            this.btnDeleteSchedule.Text = "일정 삭제";
            this.btnDeleteSchedule.UseVisualStyleBackColor = true;
            // 
            // btnAddSchedule
            // 
            this.btnAddSchedule.Location = new System.Drawing.Point(273, 693);
            this.btnAddSchedule.Name = "btnAddSchedule";
            this.btnAddSchedule.Size = new System.Drawing.Size(85, 46);
            this.btnAddSchedule.TabIndex = 4;
            this.btnAddSchedule.Text = "일정 추가";
            this.btnAddSchedule.UseVisualStyleBackColor = true;
            // 
            // listViewSchedules
            // 
            this.listViewSchedules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IsCompleted,
            this.Category,
            this.Title});
            this.listViewSchedules.FullRowSelect = true;
            this.listViewSchedules.HideSelection = false;
            this.listViewSchedules.Location = new System.Drawing.Point(11, 320);
            this.listViewSchedules.Name = "listViewSchedules";
            this.listViewSchedules.Size = new System.Drawing.Size(460, 360);
            this.listViewSchedules.TabIndex = 3;
            this.listViewSchedules.UseCompatibleStateImageBehavior = false;
            this.listViewSchedules.View = System.Windows.Forms.View.Details;
            this.listViewSchedules.SelectedIndexChanged += new System.EventHandler(this.listViewSchedules_SelectedIndexChanged);
            // 
            // IsCompleted
            // 
            this.IsCompleted.Text = "완료";
            this.IsCompleted.Width = 50;
            // 
            // Category
            // 
            this.Category.Text = "카테고리";
            this.Category.Width = 80;
            // 
            // Title
            // 
            this.Title.Text = "제목";
            this.Title.Width = 355;
            // 
            // txtScheduleContent
            // 
            this.txtScheduleContent.Location = new System.Drawing.Point(11, 530);
            this.txtScheduleContent.Multiline = true;
            this.txtScheduleContent.Name = "txtScheduleContent";
            this.txtScheduleContent.ReadOnly = true;
            this.txtScheduleContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtScheduleContent.Size = new System.Drawing.Size(460, 150);
            this.txtScheduleContent.TabIndex = 6;
            this.txtScheduleContent.Visible = false;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(18, 295);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(94, 19);
            this.chkSelectAll.TabIndex = 9;
            this.chkSelectAll.Text = "전체 선택";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // ChatRoomCalendarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 753);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.txtScheduleContent);
            this.Controls.Add(this.btnDeleteSchedule);
            this.Controls.Add(this.btnAddSchedule);
            this.Controls.Add(this.listViewSchedules);
            this.Name = "ChatRoomCalendarForm";
            this.Text = "ChatRoomCalendarForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDeleteSchedule;
        private System.Windows.Forms.Button btnAddSchedule;
        private System.Windows.Forms.ListView listViewSchedules;
        private System.Windows.Forms.ColumnHeader IsCompleted;
        private System.Windows.Forms.ColumnHeader Category;
        private System.Windows.Forms.ColumnHeader Title;
        private System.Windows.Forms.TextBox txtScheduleContent;
        private System.Windows.Forms.CheckBox chkSelectAll;
    }
}