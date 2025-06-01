using System;

namespace CalendarForm
{
    public class Schedule
    {
        public string Content { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime AlarmDate { get; set; }
        public string Category { get; set; }
        public string RepeatOption { get; set; }
    }

}