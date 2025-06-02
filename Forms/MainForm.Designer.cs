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
            this.IsImportant = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IsCompleted = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Category = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddSchedule = new System.Windows.Forms.Button();
            this.btnDeleteSchedule = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewSchedules
            // 
            this.listViewSchedules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IsImportant,
            this.IsCompleted,
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
            this.listViewSchedules.DoubleClick += new System.EventHandler(this.listViewSchedules_DoubleClick);
            // 
            // IsImportant
            // 
            this.IsImportant.Text = "중요";
            this.IsImportant.Width = 50;
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 753);
            this.Controls.Add(this.btnDeleteSchedule);
            this.Controls.Add(this.btnAddSchedule);
            this.Controls.Add(this.listViewSchedules);
            this.Name = "MainForm";
            this.Text = " ";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewSchedules;
        private System.Windows.Forms.Button btnAddSchedule;
        private System.Windows.Forms.Button btnDeleteSchedule;
        private System.Windows.Forms.ColumnHeader IsImportant;
        private System.Windows.Forms.ColumnHeader IsCompleted;
        private System.Windows.Forms.ColumnHeader Category;
        private System.Windows.Forms.ColumnHeader Title;
    }
}

