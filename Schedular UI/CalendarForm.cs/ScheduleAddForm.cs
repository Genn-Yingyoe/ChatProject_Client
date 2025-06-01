using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalendarForm.cs
{
    public partial class ScheduleAddForm : Form
    {
        public string ScheduleContent { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime AlarmDate { get; private set; }
        public string Category { get; private set; }
        public string RepeatOption { get; private set; }

        public ScheduleAddForm()
        {
            InitializeComponent();

            // 이벤트 연결, 기본값 설정 등
            this.Load += ScheduleAddForm_Load;
            alarmComboBox.SelectedIndexChanged += alarmComboBox_SelectedIndexChanged;
            
            startDatePicker.ValueChanged += startDatePicker_ValueChanged;
        }

        private void ScheduleAddForm_Load(object sender, EventArgs e)
        {
            // 기본 날짜 설정
            startDatePicker.Value = DateTime.Now;
            endDatePicker.Value = DateTime.Now;

            // 알림 시간 ComboBox 설정
            alarmComboBox.Items.AddRange(new string[] { "없음", "5분 전", "10분 전", "1시간 전", "시간 설정" });
            alarmComboBox.SelectedIndex = 0;  // 기본 '없음' 선택
            categoryComboBox.SelectedIndex = 0;

            alarmTimePicker.Visible = false;  // 기본적으로 숨김
        }

        private void startDatePicker_ValueChanged(object sender, EventArgs e)
        {
            
            if (endDatePicker.Value < startDatePicker.Value)
            {
                endDatePicker.Value = startDatePicker.Value;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {


            // 일정 제목 검증
            if (string.IsNullOrWhiteSpace(titleTextBox.Text) || titleTextBox.Text == "제목")
            {
                MessageBox.Show("일정 제목을 입력하세요.");
                return;
            }

            // 알림 시간 처리
            TimeSpan reminderTimeSpan = TimeSpan.Zero;

            string selectedAlarm = alarmComboBox.SelectedItem.ToString();

            // ComboBox에서 선택된 알림 시간 처리
            if (selectedAlarm == "없음")
            {
                // 알림 없음 -> SetAlarm 호출 안 함
                AlarmDate = DateTime.MinValue; // 의미 없는 값 저장하거나 별도 처리
            }
            else if (selectedAlarm == "5분 전")
            {
                reminderTimeSpan = TimeSpan.FromMinutes(5);
            }
            else if (selectedAlarm == "10분 전")
            {
                reminderTimeSpan = TimeSpan.FromMinutes(10);
            }
            else if (selectedAlarm == "1시간 전")
            {
                reminderTimeSpan = TimeSpan.FromHours(1);
            }
            else if (selectedAlarm == "시간 설정")
            {
                reminderTimeSpan = alarmTimePicker.Value - DateTime.Now;
            }

            // 일정 정보 저장
            ScheduleContent = titleTextBox.Text;
            StartDate = startDatePicker.Value;
            EndDate = endDatePicker.Value;

            if (selectedAlarm == "없음")
            {
                AlarmDate = DateTime.MinValue;
            }
            else
            {
                AlarmDate = startDatePicker.Value - reminderTimeSpan;  // 알림 시간 계산
                SetAlarm(AlarmDate);
            }

            Category = categoryComboBox.SelectedItem?.ToString() ?? "미지정";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void alarmComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (alarmComboBox.SelectedItem.ToString() == "시간 설정")
            {
                alarmTimePicker.Visible = true;  // 시간 설정 선택 시 DateTimePicker 보이기
            }
            else
            {
                alarmTimePicker.Visible = false; // 다른 옵션 선택 시 숨기기
            }
        }

        private void SetAlarm(DateTime alarmDate)
        {
            TimeSpan timeUntilAlarm = alarmDate - DateTime.Now;

            if (timeUntilAlarm.TotalMilliseconds > 0)
            {
                // 타이머를 설정하여 알림을 지정된 시간 전에 실행
                Timer alarmTimer = new Timer();
                alarmTimer.Interval = (int)timeUntilAlarm.TotalMilliseconds;
                alarmTimer.Tick += (s, e) =>
                {
                    alarmTimer.Stop();  // 타이머 중지
                    MessageBox.Show($"일정 알림: {ScheduleContent}", "일정 알림");  // 알림 팝업
                };
                alarmTimer.Start();  // 타이머 시작
            }
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void alarmTimePicker_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
