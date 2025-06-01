namespace CalendarForm.cs
{
    partial class ScheduleAddForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.categoryComboBox = new System.Windows.Forms.ComboBox();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.contentTextBox = new System.Windows.Forms.TextBox();
            this.startDateLabelstartDateLabel = new System.Windows.Forms.Label();
            this.endDateLabel = new System.Windows.Forms.Label();
            this.startDatePicker = new System.Windows.Forms.DateTimePicker();
            this.endDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dailyRadioButton = new System.Windows.Forms.RadioButton();
            this.weeklyRadioButton = new System.Windows.Forms.RadioButton();
            this.monthlyRadioButton = new System.Windows.Forms.RadioButton();
            this.yearlyRadioButton = new System.Windows.Forms.RadioButton();
            this.repeatRadioPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.alarmComboBox = new System.Windows.Forms.ComboBox();
            this.alarmTimePicker = new System.Windows.Forms.DateTimePicker();
            this.repeatRadioPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "카테고리";
            // 
            // categoryComboBox
            // 
            this.categoryComboBox.FormattingEnabled = true;
            this.categoryComboBox.Items.AddRange(new object[] {
            "개인",
            "가족",
            "친구",
            "학교",
            "기타"});
            this.categoryComboBox.Location = new System.Drawing.Point(15, 27);
            this.categoryComboBox.Name = "categoryComboBox";
            this.categoryComboBox.Size = new System.Drawing.Size(121, 23);
            this.categoryComboBox.TabIndex = 2;
            // 
            // titleTextBox
            // 
            this.titleTextBox.Location = new System.Drawing.Point(15, 70);
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(455, 25);
            this.titleTextBox.TabIndex = 3;
            // 
            // contentTextBox
            // 
            this.contentTextBox.Location = new System.Drawing.Point(15, 101);
            this.contentTextBox.Multiline = true;
            this.contentTextBox.Name = "contentTextBox";
            this.contentTextBox.Size = new System.Drawing.Size(455, 25);
            this.contentTextBox.TabIndex = 4;
            // 
            // startDateLabelstartDateLabel
            // 
            this.startDateLabelstartDateLabel.AutoSize = true;
            this.startDateLabelstartDateLabel.Location = new System.Drawing.Point(84, 155);
            this.startDateLabelstartDateLabel.Name = "startDateLabelstartDateLabel";
            this.startDateLabelstartDateLabel.Size = new System.Drawing.Size(52, 15);
            this.startDateLabelstartDateLabel.TabIndex = 5;
            this.startDateLabelstartDateLabel.Text = "시작일";
            // 
            // endDateLabel
            // 
            this.endDateLabel.AutoSize = true;
            this.endDateLabel.Location = new System.Drawing.Point(348, 155);
            this.endDateLabel.Name = "endDateLabel";
            this.endDateLabel.Size = new System.Drawing.Size(52, 15);
            this.endDateLabel.TabIndex = 6;
            this.endDateLabel.Text = "종료일";
            // 
            // startDatePicker
            // 
            this.startDatePicker.Location = new System.Drawing.Point(15, 201);
            this.startDatePicker.Name = "startDatePicker";
            this.startDatePicker.Size = new System.Drawing.Size(200, 25);
            this.startDatePicker.TabIndex = 7;
            this.startDatePicker.ValueChanged += new System.EventHandler(this.startDatePicker_ValueChanged);
            // 
            // endDatePicker
            // 
            this.endDatePicker.Location = new System.Drawing.Point(270, 201);
            this.endDatePicker.Name = "endDatePicker";
            this.endDatePicker.Size = new System.Drawing.Size(200, 25);
            this.endDatePicker.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 253);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "일정 반복";
            // 
            // dailyRadioButton
            // 
            this.dailyRadioButton.AutoSize = true;
            this.dailyRadioButton.Location = new System.Drawing.Point(3, 12);
            this.dailyRadioButton.Name = "dailyRadioButton";
            this.dailyRadioButton.Size = new System.Drawing.Size(58, 19);
            this.dailyRadioButton.TabIndex = 10;
            this.dailyRadioButton.TabStop = true;
            this.dailyRadioButton.Text = "매일";
            this.dailyRadioButton.UseVisualStyleBackColor = true;
            // 
            // weeklyRadioButton
            // 
            this.weeklyRadioButton.AutoSize = true;
            this.weeklyRadioButton.Location = new System.Drawing.Point(133, 12);
            this.weeklyRadioButton.Name = "weeklyRadioButton";
            this.weeklyRadioButton.Size = new System.Drawing.Size(58, 19);
            this.weeklyRadioButton.TabIndex = 11;
            this.weeklyRadioButton.TabStop = true;
            this.weeklyRadioButton.Text = "매주";
            this.weeklyRadioButton.UseVisualStyleBackColor = true;
            // 
            // monthlyRadioButton
            // 
            this.monthlyRadioButton.AutoSize = true;
            this.monthlyRadioButton.Location = new System.Drawing.Point(263, 12);
            this.monthlyRadioButton.Name = "monthlyRadioButton";
            this.monthlyRadioButton.Size = new System.Drawing.Size(58, 19);
            this.monthlyRadioButton.TabIndex = 12;
            this.monthlyRadioButton.TabStop = true;
            this.monthlyRadioButton.Text = "매월";
            this.monthlyRadioButton.UseVisualStyleBackColor = true;
            this.monthlyRadioButton.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // yearlyRadioButton
            // 
            this.yearlyRadioButton.AutoSize = true;
            this.yearlyRadioButton.Location = new System.Drawing.Point(394, 12);
            this.yearlyRadioButton.Name = "yearlyRadioButton";
            this.yearlyRadioButton.Size = new System.Drawing.Size(58, 19);
            this.yearlyRadioButton.TabIndex = 13;
            this.yearlyRadioButton.TabStop = true;
            this.yearlyRadioButton.Text = "매년";
            this.yearlyRadioButton.UseVisualStyleBackColor = true;
            // 
            // repeatRadioPanel
            // 
            this.repeatRadioPanel.Controls.Add(this.dailyRadioButton);
            this.repeatRadioPanel.Controls.Add(this.monthlyRadioButton);
            this.repeatRadioPanel.Controls.Add(this.yearlyRadioButton);
            this.repeatRadioPanel.Controls.Add(this.weeklyRadioButton);
            this.repeatRadioPanel.Location = new System.Drawing.Point(15, 286);
            this.repeatRadioPanel.Name = "repeatRadioPanel";
            this.repeatRadioPanel.Size = new System.Drawing.Size(455, 45);
            this.repeatRadioPanel.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 369);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = "일정 알림";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(270, 689);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 17;
            this.cancelButton.Text = "취소";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(367, 689);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 18;
            this.okButton.Text = "저장";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // alarmComboBox
            // 
            this.alarmComboBox.FormattingEnabled = true;
            this.alarmComboBox.Location = new System.Drawing.Point(15, 407);
            this.alarmComboBox.Name = "alarmComboBox";
            this.alarmComboBox.Size = new System.Drawing.Size(121, 23);
            this.alarmComboBox.TabIndex = 19;
            // 
            // alarmTimePicker
            // 
            this.alarmTimePicker.Location = new System.Drawing.Point(15, 450);
            this.alarmTimePicker.Name = "alarmTimePicker";
            this.alarmTimePicker.Size = new System.Drawing.Size(200, 25);
            this.alarmTimePicker.TabIndex = 16;
            this.alarmTimePicker.ValueChanged += new System.EventHandler(this.alarmTimePicker_ValueChanged);
            // 
            // ScheduleAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 753);
            this.Controls.Add(this.alarmComboBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.alarmTimePicker);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.repeatRadioPanel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.endDatePicker);
            this.Controls.Add(this.startDatePicker);
            this.Controls.Add(this.endDateLabel);
            this.Controls.Add(this.startDateLabelstartDateLabel);
            this.Controls.Add(this.contentTextBox);
            this.Controls.Add(this.titleTextBox);
            this.Controls.Add(this.categoryComboBox);
            this.Controls.Add(this.label1);
            this.Name = "ScheduleAddForm";
            this.Text = "일정 추가";
            this.repeatRadioPanel.ResumeLayout(false);
            this.repeatRadioPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox categoryComboBox;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.TextBox contentTextBox;
        private System.Windows.Forms.Label startDateLabelstartDateLabel;
        private System.Windows.Forms.Label endDateLabel;
        private System.Windows.Forms.DateTimePicker startDatePicker;
        private System.Windows.Forms.DateTimePicker endDatePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton dailyRadioButton;
        private System.Windows.Forms.RadioButton weeklyRadioButton;
        private System.Windows.Forms.RadioButton monthlyRadioButton;
        private System.Windows.Forms.RadioButton yearlyRadioButton;
        private System.Windows.Forms.Panel repeatRadioPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox alarmComboBox;
        private System.Windows.Forms.DateTimePicker alarmTimePicker;
    }
}