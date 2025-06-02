using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schedular_UI
{
    public partial class CalendarForm : Form
    {
        private TableLayoutPanel calendarTable;
        private ComboBox monthComboBox;
        private int selectedDay = -1;

        public CalendarForm()
        {
            InitializeComponent();
            InitializeCalendarUI();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void InitializeCalendarUI()
        {
            // ComboBox: 월 선택
            monthComboBox = new ComboBox();
            monthComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            for (int m = 1; m <= 12; m++)
                monthComboBox.Items.Add($"{m}월");
            monthComboBox.SelectedIndex = DateTime.Now.Month - 1;
            monthComboBox.SelectedIndexChanged += MonthComboBox_SelectedIndexChanged;
            monthComboBox.Dock = DockStyle.Top;
            this.Controls.Add(monthComboBox);

            // TableLayoutPanel: 달력
            calendarTable = new TableLayoutPanel();
            calendarTable.RowCount = 7; // 요일 헤더 + 최대 6주
            calendarTable.ColumnCount = 7;
            calendarTable.Dock = DockStyle.Fill;
            calendarTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            // 요일 헤더 추가
            string[] days = { "일", "월", "화", "수", "목", "금", "토" };
            for (int i = 0; i < 7; i++)
            {
                Label lbl = new Label();
                lbl.Text = days[i];
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                lbl.Dock = DockStyle.Fill;
                calendarTable.Controls.Add(lbl, i, 0);
            }

            // 날짜 버튼 추가 (빈칸 포함)
            for (int r = 1; r <= 6; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    Button dayBtn = new Button();
                    dayBtn.Dock = DockStyle.Fill;
                    dayBtn.Click += DayBtn_Click;
                    calendarTable.Controls.Add(dayBtn, c, r);
                }
            }

            this.Controls.Add(calendarTable);
            calendarTable.BringToFront();

            DrawCalendar(monthComboBox.SelectedIndex + 1, DateTime.Now.Year);
        }

        private void DrawCalendar(int month, int year)
        {
            DateTime firstDay = new DateTime(year, month, 1);
            int startDayOfWeek = (int)firstDay.DayOfWeek; // 일요일=0
            int daysInMonth = DateTime.DaysInMonth(year, month);

            int btnIndex = 7; // 0~6은 요일 헤더

            // 전체 버튼 가져오기
            for (int i = 7; i < calendarTable.Controls.Count; i++)
            {
                Button btn = calendarTable.Controls[i] as Button;
                btn.Text = "";
                btn.Enabled = false;
                btn.BackColor = System.Drawing.SystemColors.Control;
            }

            // 날짜 채우기
            for (int day = 1; day <= daysInMonth; day++)
            {
                Button btn = calendarTable.Controls[btnIndex + startDayOfWeek] as Button;
                btn.Text = day.ToString();
                btn.Enabled = true;
                btn.BackColor = System.Drawing.SystemColors.Control;
                btnIndex++;
            }
        }

        private void MonthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawCalendar(monthComboBox.SelectedIndex + 1, DateTime.Now.Year);
        }

        private void DayBtn_Click(object sender, EventArgs e)
        {
            Button clickedBtn = sender as Button;
            if (clickedBtn == null || string.IsNullOrEmpty(clickedBtn.Text))
                return;

            // 이전 선택 초기화
            foreach (Control ctrl in calendarTable.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = System.Drawing.SystemColors.Control;
                }
            }

            clickedBtn.BackColor = System.Drawing.Color.LightBlue;
            selectedDay = int.Parse(clickedBtn.Text);

            // 선택된 날짜 출력 (나중에 일정 리스트 필터링 등에 활용)
            MessageBox.Show($"선택한 날짜: {DateTime.Now.Year}년 {monthComboBox.SelectedIndex + 1}월 {selectedDay}일");
        }


    }
}

