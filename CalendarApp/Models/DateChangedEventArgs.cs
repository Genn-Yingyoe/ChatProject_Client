using System;

namespace CalendarApp.Models
{
    public class DateChangedEventArgs : EventArgs
    {
        public DateTime SelectedDate { get; private set; }

        public DateChangedEventArgs(DateTime selectedDate)
        {
            SelectedDate = selectedDate;
        }
    }
}
