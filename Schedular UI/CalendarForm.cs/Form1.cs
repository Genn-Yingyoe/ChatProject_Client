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
    public partial class Form1 : Form
    {
        private List<Schedule> schedules = new List<Schedule>();
        private int selectedDay = -1;

        public Form1()
        {
            InitializeComponent();

            InitializeYearComboBox();
            InitializeMonthComboBox();

            yearComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            monthComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

            listBox1.SelectedIndexChanged += listBox_SelectedIndexChanged;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);


            DrawCalendar();
        }

        private void InitializeYearComboBox()
        {
            int currentYear = DateTime.Now.Year;
            int startYear = currentYear - 10;
            int endYear = currentYear + 10;

            yearComboBox.Items.Clear();

            for (int year = startYear; year <= endYear; year++)
            {
                yearComboBox.Items.Add(year.ToString());
            }

            yearComboBox.SelectedItem = currentYear.ToString();
        }

        private void InitializeMonthComboBox()
        {
            monthComboBox.Items.Clear();

            for (int month = 1; month <= 12; month++)
            {
                monthComboBox.Items.Add(month + "월");
            }

            monthComboBox.SelectedIndex = DateTime.Now.Month - 1;
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawCalendar();
        }

        private void DrawCalendar()
        {
            if (yearComboBox.SelectedItem == null || monthComboBox.SelectedItem == null)
                return;

            int year = int.Parse(yearComboBox.SelectedItem.ToString());
            int month = int.Parse(monthComboBox.SelectedItem.ToString().Replace("월", ""));

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek; // 일요일=0

            for (int i = calendarTableLayoutPanel.Controls.Count - 1; i >= 7; i--)
            {
                calendarTableLayoutPanel.Controls.RemoveAt(i);
            }

            // 2) 날짜 버튼 새로 생성
            for (int row = 1; row <= 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    Control existingControl = calendarTableLayoutPanel.GetControlFromPosition(col, row);
                    if (existingControl != null)
                    {
                        calendarTableLayoutPanel.Controls.Remove(existingControl);
                        existingControl.Dispose(); // 리소스 해제
                    }
                }
            }

            // 날짜 버튼 추가
            int day = 1;
            for (int row = 1; row <= 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    // 첫 주의 시작요일 이전은 건너뛰기
                    if (row == 1 && col < startDayOfWeek)
                        continue;

                    if (day > daysInMonth)
                        return;

                    Button dayButton = new Button();
                    dayButton.Text = day.ToString();
                    dayButton.Dock = DockStyle.Fill;
                    dayButton.Click += DateButton_Click;

                    calendarTableLayoutPanel.Controls.Add(dayButton, col, row);
                    day++;
                }

            }

        }

            private void DateButton_Click(object sender, EventArgs e)
        {
            if (sender is Button clickedButton)
            {
                if (int.TryParse(clickedButton.Text, out int day))
                {
                    selectedDay = day;

                    HighlightSelectedDateButton(clickedButton);

                    DateTime selectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                    UpdateScheduleList(selectedDate);
                }
            }
        }

        private void HighlightSelectedDateButton(Button selectedButton)
        {
            foreach (Control ctrl in calendarTableLayoutPanel.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = System.Drawing.SystemColors.Control;
                }
            }
            selectedButton.BackColor = System.Drawing.Color.LightBlue;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            using (ScheduleAddForm addForm = new ScheduleAddForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    // 새 일정 객체 생성
                    Schedule newSchedule = new Schedule
                    {
                        Content = addForm.ScheduleContent,
                        StartDate = addForm.StartDate,
                        EndDate = addForm.EndDate,
                        AlarmDate = addForm.AlarmDate,
                        Category = addForm.Category,
                        RepeatOption = addForm.RepeatOption
                    };

                    schedules.Add(newSchedule);  // 일정 목록에 추가
                    UpdateScheduleList(newSchedule.StartDate); // 리스트 업데이트
                }
            }
        }




        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("삭제할 일정을 선택하세요!");
                return;
            }

            // 선택된 일정을 ListBox에서 삭제
            listBox1.Items.Remove(listBox1.SelectedItem);
        }



        private void UpdateScheduleList(DateTime date)
        {
            // 해당 날짜의 일정만 필터링
            var items = schedules.Where(s => s.StartDate.Date == date.Date).ToList();

            listBox1.Items.Clear();  // 기존 항목 초기화

            foreach (var schedule in items)
            {
                // 카테고리가 null이나 공백이 아닐 때만 괄호로 표시
                string categoryText = string.IsNullOrWhiteSpace(schedule.Category) ? "" : $" ({schedule.Category})";
                string displayText = $"{schedule.Content}{categoryText}";
                listBox1.Items.Add(displayText);
            }
        }


        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                // 선택된 일정 항목에 대한 추가 작업 (예: 일정 내용 표시)
            }
        }


        private void Month_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void yearComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void monthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        
    }
}
