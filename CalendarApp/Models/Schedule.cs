using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarApp.Models
{
    public class Schedule
    {
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
        public DateTime RepeatStartDate { get; set; }  // 반복 시작일
        public DateTime RepeatEndDate { get; set; }    // 반복 종료일

        public RepeatType RepeatOption { get; set; }   // 반복 옵션

        public List<DayOfWeek> RepeatDays { get; set; } = new List<DayOfWeek>();  // 매주 반복 시 요일 리스트

        public int RepeatDay { get; set; }              // 매월, 매년 반복 시 날짜 (1~31)

        public int RepeatMonth { get; set; }            // 매년 반복 시 월 (1~12)

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
