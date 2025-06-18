using System;
using System.Drawing;
using System.Windows.Forms;
using CalendarApp.Models;

namespace CalendarApp.Forms
{
    public partial class CustomCalendar : UserControl
    {
        private int displayYear, displayMonth;
        public DateTime SelectedDate { get; private set; }
        public event EventHandler<DateChangedEventArgs> SelectedDateChanged;

        private ComboBox cmbYear;
        private ComboBox cmbMonth;

        public CustomCalendar()
        {
            displayYear = DateTime.Today.Year;
            displayMonth = DateTime.Today.Month;
            SelectedDate = DateTime.Today;

            this.DoubleBuffered = true;
            this.ResizeRedraw = true;

            cmbYear = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMonth = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };

            for (int y = 2000; y <= 2030; y++)
                cmbYear.Items.Add(y);

            for (int m = 1; m <= 12; m++)
                cmbMonth.Items.Add(m);

            cmbYear.SelectedItem = displayYear;
            cmbMonth.SelectedItem = displayMonth;

            cmbYear.Location = new Point(20, 2);
            cmbYear.Size = new Size(80, 25);
            cmbMonth.Location = new Point(100, 2);
            cmbMonth.Size = new Size(70, 25);

            this.Controls.Add(cmbYear);
            this.Controls.Add(cmbMonth);

            cmbYear.SelectedIndexChanged += (s, e) =>
            {
                displayYear = (int)cmbYear.SelectedItem;
                SelectedDate = new DateTime(displayYear, displayMonth, 1);
                Invalidate();
            };

            cmbMonth.SelectedIndexChanged += (s, e) =>
            {
                displayMonth = (int)cmbMonth.SelectedItem;
                SelectedDate = new DateTime(displayYear, displayMonth, 1);
                Invalidate();
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawDays(e.Graphics);
            DrawNavigationButtons(e.Graphics);
        }

        private Rectangle GetCellRectangle(int row, int col)
        {
            int cellWidth = this.Width / 7;
            int cellHeight = (this.Height - 120) / 6;
            return new Rectangle(col * cellWidth, 60 + row * cellHeight, cellWidth, cellHeight);
        }

        private void DrawDays(Graphics g)
        {
            string[] days = { "일", "월", "화", "수", "목", "금", "토" };
            for (int i = 0; i < 7; i++)
            {
                var rect = new Rectangle(i * (Width / 7), 30, Width / 7, 25);
                g.DrawString(days[i], this.Font, Brushes.DarkBlue, rect,
                             new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            var first = new DateTime(displayYear, displayMonth, 1);
            int startDow = (int)first.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(displayYear, displayMonth);

            for (int day = 1; day <= daysInMonth; day++)
            {
                int idx = startDow + day - 1;
                int row = idx / 7;
                int col = idx % 7;
                var cell = GetCellRectangle(row, col);

                if (SelectedDate.Year == displayYear &&
                    SelectedDate.Month == displayMonth &&
                    SelectedDate.Day == day)
                {
                    g.FillRectangle(Brushes.LightSkyBlue, cell);
                }

                g.DrawRectangle(Pens.Gray, cell);

                var dayStringSize = g.MeasureString(day.ToString(), this.Font);
                PointF dayStringPoint = new PointF(
                    cell.X + (cell.Width - dayStringSize.Width) / 2,
                    cell.Y + (cell.Height - dayStringSize.Height) / 2);

                g.DrawString(day.ToString(), this.Font, Brushes.Black, dayStringPoint);
            }
        }

        private void DrawNavigationButtons(Graphics g)
        {
            g.DrawString("<", this.Font, Brushes.Black, 5, 5);
            g.DrawString(">", this.Font, Brushes.Black, this.Width - 15, 5);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.X >= 5 && e.X <= 20 && e.Y >= 5 && e.Y <= 20)
            {
                ChangeMonth(-1);
                return;
            }

            if (e.X >= this.Width - 20 && e.X <= this.Width && e.Y >= 5 && e.Y <= 20)
            {
                ChangeMonth(1);
                return;
            }

            var first = new DateTime(displayYear, displayMonth, 1);
            int startDow = (int)first.DayOfWeek;

            for (int day = 1; day <= DateTime.DaysInMonth(displayYear, displayMonth); day++)
            {
                int idx = startDow + day - 1;
                int row = idx / 7;
                int col = idx % 7;
                var cell = GetCellRectangle(row, col);

                if (cell.Contains(e.Location))
                {
                    SelectedDate = new DateTime(displayYear, displayMonth, day);
                    SelectedDateChanged?.Invoke(this, new DateChangedEventArgs(SelectedDate));
                    Invalidate();
                    break;
                }
            }
        }

        private void ChangeMonth(int delta)
        {
            var dt = new DateTime(displayYear, displayMonth, 1).AddMonths(delta);
            displayYear = dt.Year;
            displayMonth = dt.Month;

            cmbYear.SelectedItem = displayYear;
            cmbMonth.SelectedItem = displayMonth;

            Invalidate();
        }

        public void SetSelectedDate(DateTime date)
        {
            this.SelectedDate = date;
            this.displayYear = date.Year;
            this.displayMonth = date.Month;

            cmbYear.SelectedItem = this.displayYear;
            cmbMonth.SelectedItem = this.displayMonth;

            Invalidate();

            SelectedDateChanged?.Invoke(this, new DateChangedEventArgs(this.SelectedDate));
        }
    }
}