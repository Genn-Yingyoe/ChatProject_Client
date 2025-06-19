namespace CalendarApp.Forms
{
    partial class EventForm
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
            this.txtContent = new System.Windows.Forms.TextBox();
            this.comboBoxCategory = new System.Windows.Forms.ComboBox();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.comboBoxAlert = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.radioBtnRepeatDaily = new System.Windows.Forms.RadioButton();
            this.radioBtnRepeatWeekly = new System.Windows.Forms.RadioButton();
            this.radioBtnRepeatMonthly = new System.Windows.Forms.RadioButton();
            this.radioBtnRepeatYearly = new System.Windows.Forms.RadioButton();
            this.labelAlert = new System.Windows.Forms.Label();
            this.dateTimePickerCustomAlert = new System.Windows.Forms.DateTimePicker();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelRepeatPeriod = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePickerRepeatEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerRepeatStart = new System.Windows.Forms.DateTimePicker();
            this.panelWeekDays = new System.Windows.Forms.Panel();
            this.checkBoxSun = new System.Windows.Forms.CheckBox();
            this.checkBoxSat = new System.Windows.Forms.CheckBox();
            this.checkBoxFri = new System.Windows.Forms.CheckBox();
            this.checkBoxThu = new System.Windows.Forms.CheckBox();
            this.checkBoxWed = new System.Windows.Forms.CheckBox();
            this.checkBoxTue = new System.Windows.Forms.CheckBox();
            this.checkBoxMon = new System.Windows.Forms.CheckBox();
            this.numericUpDownRepeatDay = new System.Windows.Forms.NumericUpDown();
            this.comboBoxYearlyMonth = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panelRepeatPeriod.SuspendLayout();
            this.panelWeekDays.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRepeatDay)).BeginInit();
            this.SuspendLayout();
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(22, 171);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(440, 55);
            this.txtContent.TabIndex = 1;
            this.txtContent.Enter += new System.EventHandler(this.txtContent_Enter);
            this.txtContent.Leave += new System.EventHandler(this.txtContent_Leave);
            // 
            // comboBoxCategory
            // 
            this.comboBoxCategory.FormattingEnabled = true;
            this.comboBoxCategory.Location = new System.Drawing.Point(24, 102);
            this.comboBoxCategory.Name = "comboBoxCategory";
            this.comboBoxCategory.Size = new System.Drawing.Size(121, 23);
            this.comboBoxCategory.TabIndex = 2;
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStart.Location = new System.Drawing.Point(30, 471);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(170, 25);
            this.dateTimePickerStart.TabIndex = 3;
            this.dateTimePickerStart.ValueChanged += new System.EventHandler(this.dateTimePickerStart_ValueChanged);
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dateTimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerEnd.Location = new System.Drawing.Point(282, 471);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(170, 25);
            this.dateTimePickerEnd.TabIndex = 4;
            // 
            // comboBoxAlert
            // 
            this.comboBoxAlert.FormattingEnabled = true;
            this.comboBoxAlert.Location = new System.Drawing.Point(22, 605);
            this.comboBoxAlert.Name = "comboBoxAlert";
            this.comboBoxAlert.Size = new System.Drawing.Size(121, 23);
            this.comboBoxAlert.TabIndex = 5;
            this.comboBoxAlert.SelectedIndexChanged += new System.EventHandler(this.comboBoxAlert_SelectedIndexChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(309, 715);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(394, 715);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 30);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "저장";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // radioBtnRepeatDaily
            // 
            this.radioBtnRepeatDaily.AutoSize = true;
            this.radioBtnRepeatDaily.Location = new System.Drawing.Point(22, 239);
            this.radioBtnRepeatDaily.Name = "radioBtnRepeatDaily";
            this.radioBtnRepeatDaily.Size = new System.Drawing.Size(58, 19);
            this.radioBtnRepeatDaily.TabIndex = 10;
            this.radioBtnRepeatDaily.TabStop = true;
            this.radioBtnRepeatDaily.Text = "매일";
            this.radioBtnRepeatDaily.UseVisualStyleBackColor = true;
            this.radioBtnRepeatDaily.CheckedChanged += new System.EventHandler(this.RepeatOptionChanged);
            this.radioBtnRepeatDaily.Click += new System.EventHandler(this.radioBtn_Click);
            // 
            // radioBtnRepeatWeekly
            // 
            this.radioBtnRepeatWeekly.AutoSize = true;
            this.radioBtnRepeatWeekly.Location = new System.Drawing.Point(142, 239);
            this.radioBtnRepeatWeekly.Name = "radioBtnRepeatWeekly";
            this.radioBtnRepeatWeekly.Size = new System.Drawing.Size(58, 19);
            this.radioBtnRepeatWeekly.TabIndex = 11;
            this.radioBtnRepeatWeekly.TabStop = true;
            this.radioBtnRepeatWeekly.Text = "매주";
            this.radioBtnRepeatWeekly.UseVisualStyleBackColor = true;
            this.radioBtnRepeatWeekly.CheckedChanged += new System.EventHandler(this.RepeatOptionChanged);
            this.radioBtnRepeatWeekly.Click += new System.EventHandler(this.radioBtn_Click);
            // 
            // radioBtnRepeatMonthly
            // 
            this.radioBtnRepeatMonthly.AutoSize = true;
            this.radioBtnRepeatMonthly.Location = new System.Drawing.Point(269, 239);
            this.radioBtnRepeatMonthly.Name = "radioBtnRepeatMonthly";
            this.radioBtnRepeatMonthly.Size = new System.Drawing.Size(58, 19);
            this.radioBtnRepeatMonthly.TabIndex = 12;
            this.radioBtnRepeatMonthly.TabStop = true;
            this.radioBtnRepeatMonthly.Text = "매월";
            this.radioBtnRepeatMonthly.UseVisualStyleBackColor = true;
            this.radioBtnRepeatMonthly.CheckedChanged += new System.EventHandler(this.RepeatOptionChanged);
            this.radioBtnRepeatMonthly.Click += new System.EventHandler(this.radioBtn_Click);
            // 
            // radioBtnRepeatYearly
            // 
            this.radioBtnRepeatYearly.AutoSize = true;
            this.radioBtnRepeatYearly.Location = new System.Drawing.Point(394, 239);
            this.radioBtnRepeatYearly.Name = "radioBtnRepeatYearly";
            this.radioBtnRepeatYearly.Size = new System.Drawing.Size(58, 19);
            this.radioBtnRepeatYearly.TabIndex = 13;
            this.radioBtnRepeatYearly.TabStop = true;
            this.radioBtnRepeatYearly.Text = "매년";
            this.radioBtnRepeatYearly.UseVisualStyleBackColor = true;
            this.radioBtnRepeatYearly.CheckedChanged += new System.EventHandler(this.RepeatOptionChanged);
            this.radioBtnRepeatYearly.Click += new System.EventHandler(this.radioBtn_Click);
            // 
            // labelAlert
            // 
            this.labelAlert.AutoSize = true;
            this.labelAlert.Location = new System.Drawing.Point(19, 571);
            this.labelAlert.Name = "labelAlert";
            this.labelAlert.Size = new System.Drawing.Size(72, 15);
            this.labelAlert.TabIndex = 21;
            this.labelAlert.Text = "일정 알림";
            // 
            // dateTimePickerCustomAlert
            // 
            this.dateTimePickerCustomAlert.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dateTimePickerCustomAlert.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerCustomAlert.Location = new System.Drawing.Point(22, 654);
            this.dateTimePickerCustomAlert.Name = "dateTimePickerCustomAlert";
            this.dateTimePickerCustomAlert.Size = new System.Drawing.Size(200, 25);
            this.dateTimePickerCustomAlert.TabIndex = 22;
            this.dateTimePickerCustomAlert.Visible = false;
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(22, 140);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(440, 25);
            this.txtTitle.TabIndex = 0;
            this.txtTitle.Enter += new System.EventHandler(this.txtTitle_Enter);
            this.txtTitle.Leave += new System.EventHandler(this.txtTitle_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(220, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 15);
            this.label2.TabIndex = 24;
            this.label2.Text = "일정 작성";
            // 
            // panelRepeatPeriod
            // 
            this.panelRepeatPeriod.Controls.Add(this.label4);
            this.panelRepeatPeriod.Controls.Add(this.label3);
            this.panelRepeatPeriod.Controls.Add(this.dateTimePickerRepeatEnd);
            this.panelRepeatPeriod.Controls.Add(this.dateTimePickerRepeatStart);
            this.panelRepeatPeriod.Location = new System.Drawing.Point(22, 271);
            this.panelRepeatPeriod.Name = "panelRepeatPeriod";
            this.panelRepeatPeriod.Size = new System.Drawing.Size(440, 50);
            this.panelRepeatPeriod.TabIndex = 27;
            this.panelRepeatPeriod.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(304, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 15);
            this.label4.TabIndex = 30;
            this.label4.Text = "반복 종료일";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 15);
            this.label3.TabIndex = 28;
            this.label3.Text = "반복 시작일";
            // 
            // dateTimePickerRepeatEnd
            // 
            this.dateTimePickerRepeatEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerRepeatEnd.Location = new System.Drawing.Point(285, 13);
            this.dateTimePickerRepeatEnd.Name = "dateTimePickerRepeatEnd";
            this.dateTimePickerRepeatEnd.Size = new System.Drawing.Size(125, 25);
            this.dateTimePickerRepeatEnd.TabIndex = 29;
            // 
            // dateTimePickerRepeatStart
            // 
            this.dateTimePickerRepeatStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerRepeatStart.Location = new System.Drawing.Point(30, 13);
            this.dateTimePickerRepeatStart.Name = "dateTimePickerRepeatStart";
            this.dateTimePickerRepeatStart.Size = new System.Drawing.Size(125, 25);
            this.dateTimePickerRepeatStart.TabIndex = 28;
            // 
            // panelWeekDays
            // 
            this.panelWeekDays.Controls.Add(this.checkBoxSun);
            this.panelWeekDays.Controls.Add(this.checkBoxSat);
            this.panelWeekDays.Controls.Add(this.checkBoxFri);
            this.panelWeekDays.Controls.Add(this.checkBoxThu);
            this.panelWeekDays.Controls.Add(this.checkBoxWed);
            this.panelWeekDays.Controls.Add(this.checkBoxTue);
            this.panelWeekDays.Controls.Add(this.checkBoxMon);
            this.panelWeekDays.Location = new System.Drawing.Point(12, 330);
            this.panelWeekDays.Name = "panelWeekDays";
            this.panelWeekDays.Size = new System.Drawing.Size(459, 63);
            this.panelWeekDays.TabIndex = 38;
            // 
            // checkBoxSun
            // 
            this.checkBoxSun.AutoSize = true;
            this.checkBoxSun.Location = new System.Drawing.Point(405, 22);
            this.checkBoxSun.Name = "checkBoxSun";
            this.checkBoxSun.Size = new System.Drawing.Size(44, 19);
            this.checkBoxSun.TabIndex = 27;
            this.checkBoxSun.Text = "일";
            this.checkBoxSun.UseVisualStyleBackColor = true;
            // 
            // checkBoxSat
            // 
            this.checkBoxSat.AutoSize = true;
            this.checkBoxSat.Location = new System.Drawing.Point(339, 22);
            this.checkBoxSat.Name = "checkBoxSat";
            this.checkBoxSat.Size = new System.Drawing.Size(44, 19);
            this.checkBoxSat.TabIndex = 26;
            this.checkBoxSat.Text = "토";
            this.checkBoxSat.UseVisualStyleBackColor = true;
            // 
            // checkBoxFri
            // 
            this.checkBoxFri.AutoSize = true;
            this.checkBoxFri.Location = new System.Drawing.Point(273, 22);
            this.checkBoxFri.Name = "checkBoxFri";
            this.checkBoxFri.Size = new System.Drawing.Size(44, 19);
            this.checkBoxFri.TabIndex = 25;
            this.checkBoxFri.Text = "금";
            this.checkBoxFri.UseVisualStyleBackColor = true;
            // 
            // checkBoxThu
            // 
            this.checkBoxThu.AutoSize = true;
            this.checkBoxThu.Location = new System.Drawing.Point(207, 22);
            this.checkBoxThu.Name = "checkBoxThu";
            this.checkBoxThu.Size = new System.Drawing.Size(44, 19);
            this.checkBoxThu.TabIndex = 24;
            this.checkBoxThu.Text = "목";
            this.checkBoxThu.UseVisualStyleBackColor = true;
            // 
            // checkBoxWed
            // 
            this.checkBoxWed.AutoSize = true;
            this.checkBoxWed.Location = new System.Drawing.Point(141, 22);
            this.checkBoxWed.Name = "checkBoxWed";
            this.checkBoxWed.Size = new System.Drawing.Size(44, 19);
            this.checkBoxWed.TabIndex = 23;
            this.checkBoxWed.Text = "수";
            this.checkBoxWed.UseVisualStyleBackColor = true;
            // 
            // checkBoxTue
            // 
            this.checkBoxTue.AutoSize = true;
            this.checkBoxTue.Location = new System.Drawing.Point(75, 22);
            this.checkBoxTue.Name = "checkBoxTue";
            this.checkBoxTue.Size = new System.Drawing.Size(44, 19);
            this.checkBoxTue.TabIndex = 22;
            this.checkBoxTue.Text = "화";
            this.checkBoxTue.UseVisualStyleBackColor = true;
            // 
            // checkBoxMon
            // 
            this.checkBoxMon.AutoSize = true;
            this.checkBoxMon.Location = new System.Drawing.Point(9, 22);
            this.checkBoxMon.Name = "checkBoxMon";
            this.checkBoxMon.Size = new System.Drawing.Size(44, 19);
            this.checkBoxMon.TabIndex = 21;
            this.checkBoxMon.Text = "월";
            this.checkBoxMon.UseVisualStyleBackColor = true;
            // 
            // numericUpDownRepeatDay
            // 
            this.numericUpDownRepeatDay.Location = new System.Drawing.Point(95, 407);
            this.numericUpDownRepeatDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDownRepeatDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRepeatDay.Name = "numericUpDownRepeatDay";
            this.numericUpDownRepeatDay.Size = new System.Drawing.Size(40, 25);
            this.numericUpDownRepeatDay.TabIndex = 39;
            this.numericUpDownRepeatDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // comboBoxYearlyMonth
            // 
            this.comboBoxYearlyMonth.FormattingEnabled = true;
            this.comboBoxYearlyMonth.Location = new System.Drawing.Point(22, 407);
            this.comboBoxYearlyMonth.Name = "comboBoxYearlyMonth";
            this.comboBoxYearlyMonth.Size = new System.Drawing.Size(40, 23);
            this.comboBoxYearlyMonth.TabIndex = 26;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(64, 410);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 15);
            this.label5.TabIndex = 40;
            this.label5.Text = "월";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(137, 410);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 15);
            this.label6.TabIndex = 41;
            this.label6.Text = "일";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(70, 453);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 15);
            this.label7.TabIndex = 42;
            this.label7.Text = "일정 시작일";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(326, 453);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 15);
            this.label8.TabIndex = 43;
            this.label8.Text = "일정 종료일";
            // 
            // EventForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 753);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDownRepeatDay);
            this.Controls.Add(this.panelWeekDays);
            this.Controls.Add(this.panelRepeatPeriod);
            this.Controls.Add(this.comboBoxYearlyMonth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTimePickerCustomAlert);
            this.Controls.Add(this.labelAlert);
            this.Controls.Add(this.radioBtnRepeatYearly);
            this.Controls.Add(this.radioBtnRepeatMonthly);
            this.Controls.Add(this.radioBtnRepeatWeekly);
            this.Controls.Add(this.radioBtnRepeatDaily);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.comboBoxAlert);
            this.Controls.Add(this.dateTimePickerEnd);
            this.Controls.Add(this.dateTimePickerStart);
            this.Controls.Add(this.comboBoxCategory);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.txtTitle);
            this.Name = "EventForm";
            this.Text = "EventForm";
            this.panelRepeatPeriod.ResumeLayout(false);
            this.panelRepeatPeriod.PerformLayout();
            this.panelWeekDays.ResumeLayout(false);
            this.panelWeekDays.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRepeatDay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.ComboBox comboBoxCategory;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.ComboBox comboBoxAlert;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.RadioButton radioBtnRepeatDaily;
        private System.Windows.Forms.RadioButton radioBtnRepeatWeekly;
        private System.Windows.Forms.RadioButton radioBtnRepeatMonthly;
        private System.Windows.Forms.RadioButton radioBtnRepeatYearly;
        private System.Windows.Forms.Label labelAlert;
        private System.Windows.Forms.DateTimePicker dateTimePickerCustomAlert;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelRepeatPeriod;
        private System.Windows.Forms.DateTimePicker dateTimePickerRepeatEnd;
        private System.Windows.Forms.DateTimePicker dateTimePickerRepeatStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelWeekDays;
        private System.Windows.Forms.CheckBox checkBoxSun;
        private System.Windows.Forms.CheckBox checkBoxSat;
        private System.Windows.Forms.CheckBox checkBoxFri;
        private System.Windows.Forms.CheckBox checkBoxThu;
        private System.Windows.Forms.CheckBox checkBoxWed;
        private System.Windows.Forms.CheckBox checkBoxTue;
        private System.Windows.Forms.CheckBox checkBoxMon;
        private System.Windows.Forms.NumericUpDown numericUpDownRepeatDay;
        private System.Windows.Forms.ComboBox comboBoxYearlyMonth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}