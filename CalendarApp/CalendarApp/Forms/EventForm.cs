using CalendarApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;


namespace CalendarApp.Forms
{
    public partial class EventForm : Form
    {
        public Schedule TheSchedule { get; private set; }

        private RadioButton lastCheckedRadio = null;

        private readonly CalendarMode _calendarMode;

        public EventForm(DateTime selectedDate, CalendarMode mode)
        {
            InitializeComponent();
            TheSchedule = new Schedule();
            _calendarMode = mode;
            this.Load += EventForm_Load;
            this.dateTimePickerStart.Value = selectedDate;
            this.dateTimePickerEnd.Value = selectedDate;
        }


        public EventForm(Schedule scheduleToEdit, CalendarMode mode)
        {
            InitializeComponent();
            TheSchedule = scheduleToEdit;
            _calendarMode = mode;
            this.Load += EventForm_Load;
            this.Load += (sender, e) => PopulateForm();
        }

        private void txtTitle_Enter(object sender, EventArgs e)
        {
            if (txtTitle.Text == "제목")
            {
                txtTitle.Text = "";
                txtTitle.ForeColor = Color.Black;
            }
        }

        private void txtTitle_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                txtTitle.Text = "제목";
                txtTitle.ForeColor = Color.Gray;
            }
        }

        private void txtContent_Enter(object sender, EventArgs e)
        {
            if (txtContent.Text == "내용")
            {
                txtContent.Text = "";
                txtContent.ForeColor = Color.Black;
            }
        }

        private void txtContent_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                txtContent.Text = "내용";
                txtContent.ForeColor = Color.Gray;
            }
        }

        private void PopulateForm()
        {
            this.Text = "일정 수정";

            txtTitle.Text = TheSchedule.Title;
            txtContent.Text = TheSchedule.Content;
            comboBoxCategory.SelectedItem = TheSchedule.Category;
            dateTimePickerStart.Value = TheSchedule.StartDate;
            dateTimePickerEnd.Value = TheSchedule.EndDate;


            switch (TheSchedule.RepeatOption)
            {
                case RepeatType.Daily:
                    radioBtnRepeatDaily.Checked = true;
                    break;

                case RepeatType.Weekly:
                    radioBtnRepeatWeekly.Checked = true;

                    checkBoxMon.Checked = TheSchedule.RepeatDays.Contains(DayOfWeek.Monday);
                    checkBoxTue.Checked = TheSchedule.RepeatDays.Contains(DayOfWeek.Tuesday);
                    checkBoxWed.Checked = TheSchedule.RepeatDays.Contains(DayOfWeek.Wednesday);
                    checkBoxThu.Checked = TheSchedule.RepeatDays.Contains(DayOfWeek.Thursday);
                    checkBoxFri.Checked = TheSchedule.RepeatDays.Contains(DayOfWeek.Friday);
                    checkBoxSat.Checked = TheSchedule.RepeatDays.Contains(DayOfWeek.Saturday);
                    checkBoxSun.Checked = TheSchedule.RepeatDays.Contains(DayOfWeek.Sunday);
                    break;

                case RepeatType.Monthly:
                    radioBtnRepeatMonthly.Checked = true;
                    numericUpDownRepeatDay.Value = TheSchedule.RepeatDay;
                    break;

                case RepeatType.Yearly:
                    radioBtnRepeatYearly.Checked = true;
                    comboBoxYearlyMonth.SelectedIndex = TheSchedule.RepeatMonth - 1;
                    numericUpDownRepeatDay.Value = TheSchedule.RepeatDay;
                    break;

                case RepeatType.None:
                default:
                    break;
            }

            // 반복 기간 설정
            if (TheSchedule.RepeatStartDate != DateTime.MinValue)
            {
                dateTimePickerRepeatStart.Value = TheSchedule.RepeatStartDate;
            }
            if (TheSchedule.RepeatEndDate != DateTime.MinValue)
            {
                dateTimePickerRepeatEnd.Value = TheSchedule.RepeatEndDate;
            }


            // 알림 설정 
            if (TheSchedule.AlertDateTime == DateTime.MinValue)
            {
                comboBoxAlert.SelectedItem = "없음";
            }
            else
            {
                TimeSpan timeDiff = TheSchedule.StartDate.Subtract(TheSchedule.AlertDateTime);

                if (timeDiff.TotalMinutes == 5)
                {
                    comboBoxAlert.SelectedItem = "5분 전";
                }
                else if (timeDiff.TotalMinutes == 10)
                {
                    comboBoxAlert.SelectedItem = "10분 전";
                }
                else if (timeDiff.TotalHours == 1)
                {
                    comboBoxAlert.SelectedItem = "1시간 전";
                }
                else
                {
                    comboBoxAlert.SelectedItem = "사용자 지정";
                    dateTimePickerCustomAlert.Value = TheSchedule.AlertDateTime;
                    dateTimePickerCustomAlert.Visible = true;
                }
            }
        }


        private void EventForm_Load(object sender, EventArgs e)
        {
            comboBoxCategory.Items.Clear();

            comboBoxCategory.Items.Add("개인");
            comboBoxCategory.Items.Add("업무");
            comboBoxCategory.Items.Add("가족");
            comboBoxCategory.Items.Add("기타");

            comboBoxCategory.SelectedIndex = 0;

            comboBoxAlert.Items.Clear();

            comboBoxAlert.Items.Add("없음");
            comboBoxAlert.Items.Add("5분 전");
            comboBoxAlert.Items.Add("10분 전");
            comboBoxAlert.Items.Add("1시간 전");
            comboBoxAlert.Items.Add("사용자 지정");

            comboBoxAlert.SelectedIndex = 0;

            dateTimePickerCustomAlert.Visible = false;
            panelRepeatPeriod.Visible = false;
            panelWeekDays.Enabled = false;
            numericUpDownRepeatDay.Enabled = false;
            comboBoxYearlyMonth.Enabled = false;

            comboBoxYearlyMonth.Items.Clear();
            for (int i = 1; i <= 12; i++)
                comboBoxYearlyMonth.Items.Add(i.ToString());

            comboBoxYearlyMonth.SelectedIndex = 0;

            numericUpDownRepeatDay.Minimum = 1;
            numericUpDownRepeatDay.Maximum = 31;
            numericUpDownRepeatDay.Value = 1;

            txtTitle_Leave(null, null);
            txtContent_Leave(null, null);

            if (_calendarMode == CalendarMode.Shared)
            {
                comboBoxAlert.Visible = false;
                dateTimePickerCustomAlert.Visible = false;
                labelAlert.Visible = false;

            }

        }

        private void RepeatOptionChanged(object sender, EventArgs e)
        {
            bool repeatSelected = radioBtnRepeatDaily.Checked || radioBtnRepeatWeekly.Checked
                                  || radioBtnRepeatMonthly.Checked || radioBtnRepeatYearly.Checked;

            panelRepeatPeriod.Visible = repeatSelected;

            panelWeekDays.Enabled = radioBtnRepeatWeekly.Checked;

            numericUpDownRepeatDay.Enabled = radioBtnRepeatMonthly.Checked || radioBtnRepeatYearly.Checked;

            comboBoxYearlyMonth.Enabled = radioBtnRepeatYearly.Checked;

            if (!radioBtnRepeatMonthly.Checked && !radioBtnRepeatYearly.Checked)
            {
                numericUpDownRepeatDay.Value = 1;
                comboBoxYearlyMonth.SelectedIndex = 0;
            }
        }

        private void radioBtn_Click(object sender, EventArgs e)
        {
            RadioButton clickedRadio = sender as RadioButton;

            if (clickedRadio != null && clickedRadio.Equals(lastCheckedRadio))
            {
                clickedRadio.Checked = false;
                lastCheckedRadio = null;
            }
            else
            {
                lastCheckedRadio = clickedRadio;
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || txtTitle.Text == "제목")
            {
                MessageBox.Show("제목을 입력해 주세요.");
                return;
            }

            // TheSchedule 객체 업데이트
            TheSchedule.Title = (txtTitle.Text == "제목") ? "" : txtTitle.Text;
            TheSchedule.Content = (txtContent.Text == "내용") ? "" : txtContent.Text;
            TheSchedule.Category = comboBoxCategory.SelectedItem?.ToString() ?? "개인";
            TheSchedule.StartDate = dateTimePickerStart.Value;
            TheSchedule.EndDate = dateTimePickerEnd.Value;

            // 반복 설정 저장
            if (radioBtnRepeatDaily.Checked) TheSchedule.RepeatOption = RepeatType.Daily;
            else if (radioBtnRepeatWeekly.Checked)
            {
                TheSchedule.RepeatOption = RepeatType.Weekly;
                TheSchedule.RepeatDays = GetSelectedWeekDays();
            }
            else if (radioBtnRepeatMonthly.Checked)
            {
                TheSchedule.RepeatOption = RepeatType.Monthly;
                TheSchedule.RepeatDay = (int)numericUpDownRepeatDay.Value;
            }
            else if (radioBtnRepeatYearly.Checked)
            {
                TheSchedule.RepeatOption = RepeatType.Yearly;
                TheSchedule.RepeatMonth = comboBoxYearlyMonth.SelectedIndex + 1;
                TheSchedule.RepeatDay = (int)numericUpDownRepeatDay.Value;
            }
            else
            {
                TheSchedule.RepeatOption = RepeatType.None;
            }
            if (TheSchedule.RepeatOption != RepeatType.None)
            {
                TheSchedule.RepeatStartDate = dateTimePickerRepeatStart.Value.Date;
                TheSchedule.RepeatEndDate = dateTimePickerRepeatEnd.Value.Date;
            }
            else
            {
                TheSchedule.RepeatStartDate = DateTime.MinValue;
                TheSchedule.RepeatEndDate = DateTime.MinValue;
            }

            // 알림 시간 저장
            DateTime alertTime = DateTime.MinValue;

            switch (comboBoxAlert.SelectedItem?.ToString())
            {
                case "5분 전":
                    alertTime = TheSchedule.StartDate.AddMinutes(-5);
                    break;
                case "10분 전":
                    alertTime = TheSchedule.StartDate.AddMinutes(-10);
                    break;
                case "1시간 전":
                    alertTime = TheSchedule.StartDate.AddHours(-1);
                    break;
                case "사용자 지정":
                    alertTime = dateTimePickerCustomAlert.Value;
                    break;
                default: // "없음"
                    alertTime = DateTime.MinValue;
                    break;
            }

            if (alertTime != DateTime.MinValue)
            {
                TheSchedule.AlertDateTime = new DateTime(
                    alertTime.Year, alertTime.Month, alertTime.Day,
                    alertTime.Hour, alertTime.Minute, 0
                );
            }
            else
            {
                TheSchedule.AlertDateTime = DateTime.MinValue;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private List<DayOfWeek> GetSelectedWeekDays()
        {
            var selectedDays = new List<DayOfWeek>();

            if (checkBoxSun.Checked) selectedDays.Add(DayOfWeek.Sunday);
            if (checkBoxMon.Checked) selectedDays.Add(DayOfWeek.Monday);
            if (checkBoxTue.Checked) selectedDays.Add(DayOfWeek.Tuesday);
            if (checkBoxWed.Checked) selectedDays.Add(DayOfWeek.Wednesday);
            if (checkBoxThu.Checked) selectedDays.Add(DayOfWeek.Thursday);
            if (checkBoxFri.Checked) selectedDays.Add(DayOfWeek.Friday);
            if (checkBoxSat.Checked) selectedDays.Add(DayOfWeek.Saturday);

            return selectedDays;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBoxAlert_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAlert.SelectedItem?.ToString() == "사용자 지정")
            {
                dateTimePickerCustomAlert.Visible = true;
            }
            else
            {
                dateTimePickerCustomAlert.Visible = false;
            }
        }

        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            // 시작일이 종료일보다 크면 종료일을 시작일로 맞춤
            if (dateTimePickerStart.Value > dateTimePickerEnd.Value)
            {
                dateTimePickerEnd.Value = dateTimePickerStart.Value;
            }
        }
    }
}
public enum CalendarMode
{
    Personal, // 개인 캘린더 모드
    Shared    // 공유 캘린더 모드
}