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

        private DCM dcm;
        private string currentUserId;

        public MainForm()
        {
            InitializeComponent();

            dcm = new DCM();

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
                ShowSchedulesForDate(customCalendar.SelectedDate);
            };
        }

        private void CustomCalendar_SelectedDateChanged(object sender, DateChangedEventArgs e)
        {
            txtScheduleContent.Text = "";
            txtScheduleContent.Visible = false;

            chkSelectAll.Checked = false;

            DateTime selectedDate = e.SelectedDate;
            ShowSchedulesForDate(selectedDate);
        }

        private void ShowSchedulesForDate(DateTime date)
        {
            listViewSchedules.Items.Clear();

            var schedulesForDate = GetSchedulesForDate(date);

            foreach (var schedule in schedulesForDate)
            {
                var item = new ListViewItem(schedule.Category);
                item.Tag = schedule;
                item.SubItems.Add(schedule.Title);
                listViewSchedules.Items.Add(item);
            }
        }

        public List<Schedule> GetSchedulesForDate(DateTime date)
        {
            List<Schedule> result = new List<Schedule>();

            foreach (var schedule in schedules)
            {
                if (schedule.RepeatOption == RepeatType.None)
                {
                    if (date.Date >= schedule.StartDate.Date && date.Date <= schedule.EndDate.Date)
                        result.Add(schedule);
                }
                else
                {
                    if (date.Date < schedule.RepeatStartDate.Date || date.Date > schedule.RepeatEndDate.Date)
                        continue;

                    bool isOccurring = false;
                    switch (schedule.RepeatOption)
                    {
                        case RepeatType.Daily:
                            isOccurring = true;
                            break;
                        case RepeatType.Weekly:
                            if (schedule.RepeatDays.Contains(date.DayOfWeek))
                                isOccurring = true;
                            break;
                        case RepeatType.Monthly:
                            if (schedule.RepeatDay == date.Day)
                                isOccurring = true;
                            break;
                        case RepeatType.Yearly:
                            if (schedule.RepeatMonth == date.Month && schedule.RepeatDay == date.Day)
                                isOccurring = true;
                            break;
                    }

                    if (isOccurring)
                    {
                        result.Add(schedule);
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

            // 알림 대기 중인 일정 수집
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

                nextAlert.IsAlerted = true;

                ShowAlert(nextAlert);
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
        }

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
                var item = new ListViewItem(schedule.Category);
                item.Tag = schedule;
                item.SubItems.Add(schedule.Title);
                listViewSchedules.Items.Add(item);
            }
        }

        private async void btnAddSchedule_Click(object sender, EventArgs e)
        {
            using (EventForm addForm = new EventForm(customCalendar.SelectedDate, CalendarMode.Personal))
            {
                if (addForm.ShowDialog() != DialogResult.OK) return;

                var newSchedule = addForm.TheSchedule;
                // 클라이언트 Schedule 객체를 서버 전송용 List<string>으로 변환
                List<string> items = ScheduleMapper.ToServerRequestItems(newSchedule);

                // 서버에 일정 추가 요청
                var request = await dcm.db_request_data(64, items);

                if (request.Key)
                {
                    MessageBox.Show("일정이 추가되었습니다.");
                    await LoadSchedulesFromServer();
                }
                else
                {
                    MessageBox.Show("일정 추가에 실패했습니다.");
                }
                dcm.Clear_receive_data(request.Value.Item1);
            }
        }

        private async void btnDeleteSchedule_Click(object sender, EventArgs e)
        {
            List<Schedule> schedulesToDelete = listViewSchedules.CheckedItems
                                                    .Cast<ListViewItem>()
                                                    .Select(item => item.Tag as Schedule)
                                                    .ToList();

            if (schedulesToDelete.Count == 0)
            {
                MessageBox.Show("삭제할 일정을 체크해주세요.");
                return;
            }

            var confirmResult = MessageBox.Show(
                $"{schedulesToDelete.Count}개의 일정을 정말 삭제하시겠습니까?",
                "삭제 확인", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                foreach (var schedule in schedulesToDelete)
                {
                    // Sche_Id를 리스트에 담아 전송
                    var items = new List<string> { schedule.ServerId.ToString() };
                    var request = await dcm.db_request_data(66, items);
                    dcm.Clear_receive_data(request.Value.Item1);
                }

                MessageBox.Show("선택한 일정이 모두 삭제되었습니다.");
                await LoadSchedulesFromServer();

                txtScheduleContent.Text = "";
                txtScheduleContent.Visible = false;
            }
        }

        private async void listViewSchedules_DoubleClick(object sender, EventArgs e)
        {
            if (listViewSchedules.SelectedItems.Count == 0) return;
            Schedule scheduleToEdit = listViewSchedules.SelectedItems[0].Tag as Schedule;
            if (scheduleToEdit == null) return;

            using (EventForm editForm = new EventForm(scheduleToEdit, CalendarMode.Personal))
            {
                if (editForm.ShowDialog() != DialogResult.OK) return;

                var updatedSchedule = editForm.TheSchedule;
                List<string> items = ScheduleMapper.ToServerRequestItems(updatedSchedule);
                // Sche_Id 추가
                items.Insert(0, updatedSchedule.ServerId.ToString());

                var request = await dcm.db_request_data(65, items);

                if (request.Key)
                {
                    MessageBox.Show("일정이 수정되었습니다.");
                    await LoadSchedulesFromServer();
                }
                else
                {
                    MessageBox.Show("일정 수정에 실패했습니다.");
                }
                dcm.Clear_receive_data(request.Value.Item1);
            }
        }


        private async Task LoadSchedulesFromServer()// 유저 스케쥴 목록 읽기
        {
            var request = await dcm.db_request_data(67, new List<string>());

            schedules.Clear();

            if (request.Key && request.Value.Item2.Count > 0)
            {
                for (int i = 0; i < request.Value.Item2.Count - 1; i++)
                {
                    int dataIndex = request.Value.Item2[i];
                    // 받은 JSON 문자열을 서버 데이터 객체로 역직렬화
                    var serverSchedule = dcm.DeSerializeJson<_User_Id__Scheduler>(request.Value.Item1, dataIndex);
                    if (serverSchedule != null)
                    {
                        // 서버 객체를 클라이언트 객체로 변환하여 리스트에 추가
                        schedules.Add(ScheduleMapper.ToClientSchedule(serverSchedule));
                    }
                }
            }

            dcm.Clear_receive_data(request.Value.Item1);

            ShowSchedulesForDate(customCalendar.SelectedDate);
        }

        private void listViewSchedules_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewSchedules.SelectedItems.Count > 0)
            {
                // Tag에서 Schedule 객체를 가져옴
                Schedule selectedSchedule = listViewSchedules.SelectedItems[0].Tag as Schedule;
                if (selectedSchedule != null)
                {
                    DateTime startDate = selectedSchedule.StartDate;
                    DateTime endDate = selectedSchedule.EndDate;
                    string scheduleDateInfo;

                    if (startDate.Date == endDate.Date)
                    {
                        scheduleDateInfo = startDate.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        scheduleDateInfo = $"{startDate:yyyy-MM-dd} ~ {endDate:yyyy-MM-dd}";
                    }

                    string scheduleDetails = $"{scheduleDateInfo}" +
                                             $"{Environment.NewLine}" +
                                             $"{Environment.NewLine}" +
                                             $"{selectedSchedule.Category}" +
                                             $"{Environment.NewLine}" +
                                             $"{Environment.NewLine}" +
                                             $"{selectedSchedule.Title}" +
                                             $"{Environment.NewLine}" +
                                             $"{Environment.NewLine}" +
                                             $"{selectedSchedule.Content}";

                    txtScheduleContent.Text = scheduleDetails;

                    txtScheduleContent.Visible = true;
                }
            }
            else
            {
                txtScheduleContent.Text = "";
                txtScheduleContent.Visible = false;
            }
        }
        
        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = chkSelectAll.Checked;

            foreach (ListViewItem item in listViewSchedules.Items)
            {
                item.Checked = isChecked;
            }
        }

        public async void UserLoggedIn(string userId)
        {
            this.currentUserId = userId;

            dcm.Login(userId);

            MessageBox.Show($"사용자로 로그인되었습니다. 스케줄을 불러옵니다.");
            await LoadSchedulesFromServer();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        /*private void btnTempLogin_Click(object sender, EventArgs e)
        {
            // 테스트할 임시 사용자 ID
            string tempUserId = "000000";

            UserLoggedIn(tempUserId);

            ((Button)sender).Enabled = false;
        }

        public void UserLoggedOut()
        {
            this.currentUserId = string.Empty;
            dcm.Logout();
            schedules.Clear();
            RefreshScheduleListUI();
            MessageBox.Show("로그아웃되었습니다.");
        }
        */
    }
}