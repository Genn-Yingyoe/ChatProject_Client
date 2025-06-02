using CalendarApp.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalendarApp.Models;
using System.Diagnostics;
using System.IO;
using ChatMoa_DataBaseServer;

namespace CalendarApp
{
    public partial class MainForm : Form
    {
        private CustomCalendar customCalendar;
        private List<Schedule> schedules = new List<Schedule>();
        private System.Windows.Forms.Timer alertTimer;
        private ImageList imageListIcons;

        public MainForm()
        {
            InitializeComponent();

            imageListIcons = new ImageList();
            imageListIcons.ImageSize = new Size(16, 16); // 적당한 크기 설정
            imageListIcons.Images.Add("star", Properties.Resources.star);

            listViewSchedules.SmallImageList = imageListIcons;

            listViewSchedules.MouseDown += listViewSchedules_MouseDown;


            RefreshScheduleListUI();
            InitializeAlertTimer();

            customCalendar = new CustomCalendar
            {
                Name = "customCalendar",
                Location = new Point(10, 10),
                Size = new Size(400, 300)
            };
            this.Controls.Add(customCalendar);

            customCalendar.SelectedDateChanged += CustomCalendar_SelectedDateChanged;

            this.Load += (s, e) =>
            {
                LoadEventsForDate(customCalendar.SelectedDate);
            };
        }

        private void CustomCalendar_SelectedDateChanged(object sender, DateChangedEventArgs e)
        {
            DateTime selectedDate = e.SelectedDate;
            ShowSchedulesForDate(selectedDate); // 선택 날짜에 해당하는 일정 보여주기
        }

        private void ShowSchedulesForDate(DateTime date)
        {
            listViewSchedules.Items.Clear();

            var schedulesForDate = GetSchedulesForDate(date);  // 반복 일정 포함 필터링 함수

            foreach (var schedule in schedulesForDate)
            {
                var item = new ListViewItem();

                if (schedule.IsImportant) item.ImageKey = "star";

                item.Text = "";

                item.SubItems.Add(schedule.IsCompleted ? "✓" : "");
                item.SubItems.Add(schedule.Category);
                item.SubItems.Add(schedule.Title);

                listViewSchedules.Items.Add(item);
            }
        }

        public List<Schedule> GetSchedulesForDate(DateTime date)
        {
            List<Schedule> result = new List<Schedule>();

            foreach (var schedule in schedules)
            {
                // 반복하지 않는 일정: 날짜 범위 내 포함 여부만 확인
                if (schedule.RepeatOption == RepeatType.None)
                {
                    if (schedule.StartDate.Date <= date.Date && date.Date <= schedule.EndDate.Date)
                        result.Add(schedule);
                }
                else
                {
                    // 반복 기간 내인지 확인 (반복 시작일 ~ 반복 종료일)
                    if (date.Date < schedule.RepeatStartDate.Date || date.Date > schedule.RepeatEndDate.Date)
                        continue;

                    switch (schedule.RepeatOption)
                    {
                        case RepeatType.Daily:
                            // 반복 기간 내면 매일 반복이므로 추가
                            result.Add(schedule);
                            break;

                        case RepeatType.Weekly:
                            // 반복 요일에 해당하는지 확인
                            if (schedule.RepeatDays.Contains(date.DayOfWeek))
                                result.Add(schedule);
                            break;

                        case RepeatType.Monthly:
                            // 매월 반복일이 오늘 날짜의 일과 같은지 확인
                            if (schedule.RepeatDay == date.Day)
                                result.Add(schedule);
                            break;

                        case RepeatType.Yearly:
                            // 매년 반복일이 오늘 날짜의 월, 일과 같은지 확인
                            if (schedule.RepeatMonth == date.Month && schedule.RepeatDay == date.Day)
                                result.Add(schedule);
                            break;
                    }
                }
            }

            return result;
        }

        private void InitializeAlertTimer()
        {
            alertTimer = new System.Windows.Forms.Timer();
            alertTimer.Interval = 1000; // 1초마다 체크
            alertTimer.Tick += AlertTimer_Tick;
            alertTimer.Start();
        }


        private void AlertTimer_Tick(object sender, EventArgs e)
        { 
            DateTime now = DateTime.Now;
            DateTime nowTruncated = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

            // 알림 대기 중인 일정만 수집
            var dueSchedules = schedules
                .Where(s => !s.IsAlerted
                            && s.AlertDateTime != DateTime.MinValue
                            && s.AlertDateTime <= nowTruncated)
                .OrderBy(s => s.AlertDateTime)
                .ToList();

            if (dueSchedules.Any())
            {
                // 가장 이른 알림 일정 선택
                var nextAlert = dueSchedules.First();

                // 알림 받기 전 플래그 설정
                nextAlert.IsAlerted = true;

                ShowAlert(nextAlert);

                Debug.WriteLine($"알림 받은 일정: {nextAlert.Title} - IsAlerted: {nextAlert.IsAlerted}");
            }
        }

        private void ShowAlert(Schedule schedule)
        {
            DateTime startTruncated = new DateTime(
                schedule.StartDate.Year,
                schedule.StartDate.Month,
                schedule.StartDate.Day,
                schedule.StartDate.Hour,
                schedule.StartDate.Minute,
                0);

            TimeSpan diff = startTruncated - schedule.AlertDateTime;

            string whenText;
            if (Math.Abs(diff.TotalMinutes - 5) < 0.1)
                whenText = "5분 전";
            else if (Math.Abs(diff.TotalMinutes - 10) < 0.1)
                whenText = "10분 전";
            else if (Math.Abs(diff.TotalHours - 1) < 0.1)
                whenText = "1시간 전";
            else
                whenText = schedule.AlertDateTime.ToString("yyyy-MM-dd HH:mm");

            MessageBox.Show(
                $"[{schedule.Title}] 일정 시작 {whenText}입니다.",
                "일정 알림",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            schedule.IsAlerted = true;
            Debug.WriteLine($"알림 받은 일정(ShowAlert): {schedule.Title}, IsAlerted={schedule.IsAlerted}");
        }


        /*private void SaveSchedules()
        {
            // 예시: 일정 상태를 파일이나 DB에 저장하는 방식
            using (var writer = new StreamWriter("schedules.txt"))
            {
                foreach (var schedule in schedules)
                {
                    writer.WriteLine($"{schedule.Title},{schedule.StartDate},{schedule.IsAlerted}");
                }
            }
        }*/


        public void AddSchedule(Schedule newSchedule)
        {
            if (!schedules.Any(s => s.Title == newSchedule.Title && s.StartDate == newSchedule.StartDate))
            {
                schedules.Add(newSchedule);
                RefreshScheduleListUI();
            }
        }

        private void RefreshScheduleListUI()
        {
            listViewSchedules.Items.Clear();

            foreach (var schedule in schedules)
            {
                var item = new ListViewItem();

                if (schedule.IsImportant) item.ImageKey = "star";

                item.Text = "";

                item.SubItems.Add(schedule.IsCompleted ? "✓" : "");
                item.SubItems.Add(schedule.Category);
                item.SubItems.Add(schedule.Title);

                listViewSchedules.Items.Add(item);
            }
        }

        private void btnAddSchedule_Click(object sender, EventArgs e)
        {
            using (EventForm addForm = new EventForm())
            {
                addForm.Owner = this;
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    schedules.Add(addForm.NewSchedule);
                    RefreshScheduleListUI();
                    //AddSchedule(addForm.NewSchedule);
                }
            }
        }

        private void btnDeleteSchedule_Click(object sender, EventArgs e)
        {
            if (listViewSchedules.SelectedIndices.Count == 0)
            {
                MessageBox.Show("삭제할 일정을 선택하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 선택된 첫 번째 일정 인덱스 가져오기
            int selectedIndex = listViewSchedules.SelectedIndices[0];

            if (selectedIndex >= 0 && selectedIndex < schedules.Count)
            {
                schedules.RemoveAt(selectedIndex);
                RefreshScheduleListUI();
            }
        }

        

        private void listViewSchedules_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int colIndex = -1;
            int widthSum = 0;

            // 각 컬럼 너비 누적해서 클릭한 컬럼 인덱스 계산
            foreach (ColumnHeader col in listViewSchedules.Columns)
            {
                widthSum += col.Width;
                if (x < widthSum)
                {
                    colIndex = col.Index;
                    break;
                }
            }

            if (colIndex == 1)  // 완료 칼럼 인덱스 (중요 0, 완료 1...)
            {
                var hit = listViewSchedules.HitTest(e.Location);
                if (hit.Item != null)
                {
                    int itemIndex = hit.Item.Index;
                    schedules[itemIndex].IsCompleted = !schedules[itemIndex].IsCompleted;
                    RefreshScheduleListUI();
                }
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadEventsForDate(DateTime date)
        {

        }

        private void listViewSchedules_DoubleClick(object sender, EventArgs e)
        {
            if (listViewSchedules.SelectedIndices.Count == 0) return;

            int idx = listViewSchedules.SelectedIndices[0];
            if (idx < 0 || idx >= schedules.Count) return;

            // 수정하려는 원본 객체 복사 (참조가 아닌 새 객체로 전달해도 되지만, 여기서는 직접 참조를 넘겨서 수정합니다)
            Schedule toEdit = schedules[idx];

            // EventForm에 Schedule 객체를 넘겨서 '수정 모드'로 연다
            using (EventForm editForm = new EventForm(toEdit))
            {
                editForm.Owner = this;
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // EventForm 내부에서 toEdit 객체를 바로 수정해주므로, schedules[idx]는 이미 바뀐 상태
                    // 혹은 (복사본을 사용했다면) schedules[idx] = editForm.EditingSchedule;
                    RefreshScheduleListUI();
                }
            }
        }
    }
}
