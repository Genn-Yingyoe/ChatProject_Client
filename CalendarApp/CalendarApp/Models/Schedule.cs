using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarApp.Models
{
    public class Schedule
    {
        public int ServerId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsImportant { get; set; }
        public DateTime AlertDateTime { get; set; }
        public bool IsAlerted { get; set; } = false;


        // 알림 기능
        public TimeSpan AlertBefore { get; set; }
        public DateTime AlertTime => StartDate - AlertBefore;

        // 반복 기능
        public DateTime RepeatStartDate { get; set; }
        public DateTime RepeatEndDate { get; set; }

        public RepeatType RepeatOption { get; set; }

        public List<DayOfWeek> RepeatDays { get; set; } = new List<DayOfWeek>();

        public int RepeatDay { get; set; }

        public int RepeatMonth { get; set; }

        public Schedule()
        {
            RepeatDays = new List<DayOfWeek>();
            AlertBefore = TimeSpan.Zero;
            RepeatOption = RepeatType.None;
        }
    }

    public enum RepeatType
    {
        None,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}
