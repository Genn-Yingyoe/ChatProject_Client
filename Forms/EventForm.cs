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

namespace CalendarApp.Forms
{
    public partial class EventForm : Form
    {
        public EventForm()
        {
            InitializeComponent();
        }

        public Schedule NewSchedule { get; private set; }


        private void EventForm_Load(object sender, EventArgs e)
        {
            comboBoxCategory.Items.Clear();
            comboBoxCategory.SelectedIndex = 0;


            comboBoxAlert.Items.Clear();
            comboBoxAlert.SelectedIndex = 0;

            dateTimePickerCustomAlert.Visible = false;
            panelRepeatPeriod.Visible = false;
            panelWeekDays.Enabled = false;
            numericUpDownRepeatDay.Enabled = false;
            comboBoxYearlyMonth.Enabled = false;

            comboBoxYearlyMonth.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                comboBoxYearlyMonth.Items.Add(i.ToString() + "월");
            }
            comboBoxYearlyMonth.SelectedIndex = 0;

            numericUpDownRepeatDay.Minimum = 1;
            numericUpDownRepeatDay.Maximum = 31;
            numericUpDownRepeatDay.Value = 1;
        }

        private List<DayOfWeek> GetSelectedWeekDays()
        {
            List<DayOfWeek> selectedDays = new List<DayOfWeek>();

            if (checkBoxMon.Checked) selectedDays.Add(DayOfWeek.Monday);
            if (checkBoxTue.Checked) selectedDays.Add(DayOfWeek.Tuesday);
            if (checkBoxWed.Checked) selectedDays.Add(DayOfWeek.Wednesday);
            if (checkBoxThu.Checked) selectedDays.Add(DayOfWeek.Thursday);
            if (checkBoxFri.Checked) selectedDays.Add(DayOfWeek.Friday);
            if (checkBoxSat.Checked) selectedDays.Add(DayOfWeek.Saturday);
            if (checkBoxSun.Checked) selectedDays.Add(DayOfWeek.Sunday);

            return selectedDays;
        }

        private void RepeatOptionChanged(object sender, EventArgs e)
        {
            bool repeatSelected = radioBtnRepeatDaily.Checked || radioBtnRepeatWeekly.Checked
                                  || radioBtnRepeatMonthly.Checked || radioBtnRepeatYearly.Checked;

            // 반복 기간 패널은 반복 옵션 선택 시 보임
            panelRepeatPeriod.Visible = repeatSelected;

            // 매주일 때 요일 선택 패널 활성화
            panelWeekDays.Enabled = radioBtnRepeatWeekly.Checked;

            // 매월, 매년 반복일 때 일 선택 NumericUpDown 활성화
            numericUpDownRepeatDay.Enabled = radioBtnRepeatMonthly.Checked || radioBtnRepeatYearly.Checked;

            // 매년 반복 시 월 선택 ComboBox 활성화
            comboBoxYearlyMonth.Enabled = radioBtnRepeatYearly.Checked;

            // 매월, 매년 반복 아닐 때는 일/월 선택 비활성화
            if (!radioBtnRepeatMonthly.Checked && !radioBtnRepeatYearly.Checked)
            {
                numericUpDownRepeatDay.Value = 1; // 기본값 초기화
                comboBoxYearlyMonth.SelectedIndex = 0; // 기본값 초기화
            }
        }




        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("제목을 입력해 주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Schedule 객체 생성
            Schedule newSchedule = new Schedule
            {
                Title = txtTitle.Text,
                Content = txtContent.Text,
                Category = comboBoxCategory.SelectedItem?.ToString() ?? "개인",
                StartDate = dateTimePickerStart.Value,
                EndDate = dateTimePickerEnd.Value,
                IsImportant = checkBoxImportant.Checked,
                IsCompleted = false,
                RepeatStartDate = dateTimePickerRepeatStart.Value,
                RepeatEndDate = dateTimePickerRepeatEnd.Value,
                RepeatOption = RepeatType.None,
                RepeatDays = new List<DayOfWeek>(),
                RepeatDay = 1,
                RepeatMonth = 1
            };

            if (radioBtnRepeatDaily.Checked) newSchedule.RepeatOption = RepeatType.Daily;
            else if (radioBtnRepeatWeekly.Checked)
            {
                newSchedule.RepeatOption = RepeatType.Weekly;
                newSchedule.RepeatDays = GetSelectedWeekDays(); // 체크박스 함수
            }
            else if (radioBtnRepeatMonthly.Checked)
            {
                newSchedule.RepeatOption = RepeatType.Monthly;
                newSchedule.RepeatDay = (int)numericUpDownRepeatDay.Value;
            }
            else if (radioBtnRepeatYearly.Checked)
            {
                newSchedule.RepeatOption = RepeatType.Yearly;
                newSchedule.RepeatMonth = comboBoxYearlyMonth.SelectedIndex + 1;
                newSchedule.RepeatDay = (int)numericUpDownRepeatDay.Value;
            }
            else
            {
                newSchedule.RepeatOption = RepeatType.None;
            }

            // 알림 옵션 처리
            switch (comboBoxAlert.SelectedItem?.ToString())
            {
                case "5분 전":
                    //초는 0으로 설정
                    newSchedule.AlertDateTime = new DateTime(newSchedule.StartDate.Year,
                                                             newSchedule.StartDate.Month,
                                                             newSchedule.StartDate.Day,
                                                             newSchedule.StartDate.Hour,
                                                             newSchedule.StartDate.Minute,
                                                             0).AddMinutes(-5);
                    break;

                case "10분 전":
                    newSchedule.AlertDateTime = new DateTime(newSchedule.StartDate.Year,
                                                             newSchedule.StartDate.Month,
                                                             newSchedule.StartDate.Day,
                                                             newSchedule.StartDate.Hour,
                                                             newSchedule.StartDate.Minute,
                                                             0).AddMinutes(-10);
                    break;

                case "1시간 전":
                    newSchedule.AlertDateTime = new DateTime(newSchedule.StartDate.Year,
                                                             newSchedule.StartDate.Month,
                                                             newSchedule.StartDate.Day,
                                                             newSchedule.StartDate.Hour,
                                                             newSchedule.StartDate.Minute,
                                                             0).AddHours(-1);
                    break;

                case "사용자 지정":
                    newSchedule.AlertDateTime = dateTimePickerCustomAlert.Value; // 사용자 지정 알림 시간
                    break;

                default:
                    newSchedule.AlertDateTime = DateTime.MinValue; // 알림 없음
                    break;
            }

            newSchedule.AlertDateTime = new DateTime(newSchedule.AlertDateTime.Year,
                                             newSchedule.AlertDateTime.Month,
                                             newSchedule.AlertDateTime.Day,
                                             newSchedule.AlertDateTime.Hour,
                                             newSchedule.AlertDateTime.Minute,
                                             0);

            NewSchedule = newSchedule;

            ((MainForm)this.Owner).AddSchedule(newSchedule);

            this.DialogResult = DialogResult.OK;
            this.Close();
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
        


        private void dateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
