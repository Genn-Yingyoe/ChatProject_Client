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
using ChatMoa_DataBaseServer;


namespace CalendarApp.Forms
{
    public partial class ChatRoomCalendarForm : Form
    {
        private readonly string roomId;
        private readonly DCM dcm;
        private List<Schedule> sharedSchedules = new List<Schedule>();
        private CustomCalendar customCalendar;


        public ChatRoomCalendarForm(string roomId, DCM dcm)
        {
            InitializeComponent();
            this.roomId = roomId;
            this.dcm = dcm;
            this.Text = $"공유 캘린더 - 방 ID: {roomId}";

            customCalendar = new CustomCalendar
            {
                Name = "customCalendar",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(400, 300)
            };
            this.Controls.Add(customCalendar);

            customCalendar.SelectedDateChanged += CustomCalendar_SelectedDateChanged;

            this.Load += async (s, e) => await LoadSharedSchedulesFromServer();

            this.listViewSchedules.DoubleClick += new System.EventHandler(this.listViewSchedules_DoubleClick);
        }

        private void RefreshScheduleListUI()
        {
            listViewSchedules.Items.Clear();

            foreach (var schedule in sharedSchedules)
            {
                var item = new ListViewItem(schedule.Category);
                item.Tag = schedule;
                item.SubItems.Add(schedule.Title);
                listViewSchedules.Items.Add(item);
            }
        }

        private void CustomCalendar_SelectedDateChanged(object sender, DateChangedEventArgs e)
        {
            txtScheduleContent.Text = "";
            txtScheduleContent.Visible = false;

            chkSelectAll.Checked = false;

            ShowSchedulesForDate(e.SelectedDate);
        }

        private async void listViewSchedules_DoubleClick(object sender, EventArgs e)
        {
            if (listViewSchedules.SelectedItems.Count == 0) return;

            Schedule scheduleToEdit = listViewSchedules.SelectedItems[0].Tag as Schedule;

            if (scheduleToEdit == null) return;

            // 일정 수정 창으로 EventForm 사용
            using (EventForm editForm = new EventForm(scheduleToEdit, CalendarMode.Shared))
            {
                // 저장
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    var updatedSchedule = editForm.TheSchedule;

                    // 1. 수정된 객체를 서버 요청용 List<string>으로 변환
                    List<string> items = ScheduleMapper.ToServerRequestItems(updatedSchedule);

                    // roomId와 scheduleId를 추가
                    items.Insert(0, updatedSchedule.ServerId.ToString());
                    items.Insert(0, this.roomId);

                    // 서버에 수정 요청
                    var request = await dcm.db_request_data(69, items);

                    if (request.Key)
                    {
                        MessageBox.Show("공유 일정이 성공적으로 수정되었습니다.");
                        await LoadSharedSchedulesFromServer(); // 성공 시 목록 새로고침
                    }
                    else
                    {
                        MessageBox.Show("공유 일정 수정에 실패했습니다.");
                    }
                    dcm.Clear_receive_data(request.Value.Item1);
                }
            }
        }

        private async Task LoadSharedSchedulesFromServer()// 채팅방 스케쥴 목록 읽기
        {
            var items = new List<string> { this.roomId };
            var request = await dcm.db_request_data(71, items);

            sharedSchedules.Clear();

            if (request.Key && request.Value.Item2 != null && request.Value.Item2.Count > 0)
            {
                for (int i = 0; i < request.Value.Item2.Count - 1; i++)
                {
                    int dataIndex = request.Value.Item2[i];
                    // 서버의 Chat_Room__Room_Id__Scheduler 타입으로 역직렬화
                    var serverSchedule = dcm.DeSerializeJson<Chat_Room__Room_Id__Scheduler>(request.Value.Item1, dataIndex);

                    if (serverSchedule != null)
                    {
                        var clientSchedule = ScheduleMapper.ToClientSharedSchedule(serverSchedule);

                        // 변환된 객체를 공유 스케줄 리스트에 추가
                        sharedSchedules.Add(clientSchedule);
                    }
                }
            }
            dcm.Clear_receive_data(request.Value.Item1);

            RefreshScheduleListUI();

            ShowSchedulesForDate(customCalendar.SelectedDate);
        }

        // 특정 날짜에 해당하는 일정만 리스트뷰에 표시
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
            foreach (var schedule in sharedSchedules)
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

        private async void btnAddSchedule_Click(object sender, EventArgs e)// 채팅방 스케쥴 추가
        {
            using (EventForm addForm = new EventForm(customCalendar.SelectedDate, CalendarMode.Shared))
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    var newSchedule = addForm.TheSchedule;

                    List<string> items = ScheduleMapper.ToServerRequestItems(newSchedule);

                    items.Insert(0, this.roomId);

                    var request = await dcm.db_request_data(68, items);
                    if (request.Key)
                    {
                        MessageBox.Show("공유 일정이 추가되었습니다.");

                        await LoadSharedSchedulesFromServer();
                    }
                    else
                    {
                        MessageBox.Show("공유 일정 추가에 실패했습니다.");
                    }
                    dcm.Clear_receive_data(request.Value.Item1);
                }
            }
        }

        private async void btnDeleteSchedule_Click(object sender, EventArgs e)// 채팅방 스케쥴 삭제
        {
            // 체크된 항목들만 모아서 새로운 리스트 생성
            List<Schedule> schedulesToDelete = new List<Schedule>();
            foreach (ListViewItem item in listViewSchedules.Items)
            {
                if (item.Checked)
                {
                    // Tag에서 Schedule 객체를 가져와 리스트에 추가
                    schedulesToDelete.Add(item.Tag as Schedule);
                }
            }

            if (schedulesToDelete.Count == 0)
            {
                MessageBox.Show("삭제할 일정을 체크해주세요.");
                return;
            }

            var confirmResult = MessageBox.Show(
                $"{schedulesToDelete.Count}개의 일정을 정말 삭제하시겠습니까?",
                "삭제 확인",
                MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                foreach (var schedule in schedulesToDelete)
                {
                    if (schedule == null) continue;

                    var items = new List<string> { this.roomId, schedule.ServerId.ToString() };

                    await dcm.db_request_data(70, items);
                }

                MessageBox.Show("선택한 일정이 모두 삭제되었습니다.");

                await LoadSharedSchedulesFromServer();

                txtScheduleContent.Text = "";
                txtScheduleContent.Visible = false;
            }
        }

        private void listViewSchedules_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewSchedules.SelectedItems.Count > 0)
            {
                Schedule selectedSchedule = listViewSchedules.SelectedItems[0].Tag as Schedule;
                if (selectedSchedule != null)
                {
                    DateTime startDate = selectedSchedule.StartDate;
                    DateTime endDate = selectedSchedule.EndDate;
                    string scheduleDateInfo;

                    // 하루짜리 일정
                    if (startDate.Date == endDate.Date)
                    {
                        scheduleDateInfo = startDate.ToString("yyyy-MM-dd");
                    }
                    // 여러 날에 걸친 일정
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

        private void ChatRoomCalendarForm_Load(object sender, EventArgs e)
        {

        }
    }
}