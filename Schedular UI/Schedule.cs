using System;

namespace Schedular_UI
{
    public class Schedule
    {
        public string Content { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime AlarmDate { get; set; }
        public string Category { get; set; }
        public string RepeatOption { get; set; }

        public override string ToString()
        {
            return Content;  // 리스트박스 등에서 일정 제목만 보여주기 위해
        }
    }

}

