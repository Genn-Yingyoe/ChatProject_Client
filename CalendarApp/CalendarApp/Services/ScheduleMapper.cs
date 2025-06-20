using CalendarApp.Models;
using ChatMoa_DataBaseServer;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ScheduleMapper
{
    // 서버 데이터 -> 클라이언트 데이터로 변환
    internal static Schedule ToClientSchedule(_User_Id__Scheduler serverSchedule)
    {
        if (serverSchedule == null) return null;

        var clientSchedule = new Schedule();

        clientSchedule.ServerId = serverSchedule.Sche_Id;
        clientSchedule.Category = serverSchedule.Category;

        string delimiter = "[CONTENT]";
        if (serverSchedule.Sche_Str != null && serverSchedule.Sche_Str.Contains(delimiter))
        {
            var parts = serverSchedule.Sche_Str.Split(new[] { delimiter }, StringSplitOptions.None);
            clientSchedule.Title = parts[0];
            clientSchedule.Content = parts.Length > 1 ? parts[1] : "";
        }
        else
        {
            clientSchedule.Title = serverSchedule.Sche_Str ?? "";
            clientSchedule.Content = "";
        }
        // 일정 알림
        if (!string.IsNullOrEmpty(serverSchedule.Alert_Date))
        {
            DateTime.TryParse(serverSchedule.Alert_Date, out DateTime alertTime);
            clientSchedule.AlertDateTime = alertTime;
        }
        // 일정 반복
        clientSchedule.RepeatOption = RepeatType.None;
        if (!string.IsNullOrEmpty(serverSchedule.Daily))
        {
            clientSchedule.RepeatOption = RepeatType.Daily;
        }
        else if (!string.IsNullOrEmpty(serverSchedule.Weekly))
        {
            clientSchedule.RepeatOption = RepeatType.Weekly;
            clientSchedule.RepeatDays = serverSchedule.Weekly.Split(';')
                .Select(dayStr => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayStr))
                .ToList();
        }
        else if (!string.IsNullOrEmpty(serverSchedule.Monthly) && int.TryParse(serverSchedule.Monthly, out int day))
        {
            clientSchedule.RepeatOption = RepeatType.Monthly;
            clientSchedule.RepeatDay = day;
        }
        else if (!string.IsNullOrEmpty(serverSchedule.Yearly))
        {
            var parts = serverSchedule.Yearly.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int yearlyDay))
            {
                clientSchedule.RepeatOption = RepeatType.Yearly;
                clientSchedule.RepeatMonth = month;
                clientSchedule.RepeatDay = yearlyDay;
            }
        }

        if (clientSchedule.RepeatOption != RepeatType.None)
        {
            DateTime.TryParse(serverSchedule.Begin_Date, out DateTime repeatStartDate);
            DateTime.TryParse(serverSchedule.Finish_Date, out DateTime repeatEndDate);
            clientSchedule.RepeatStartDate = repeatStartDate;
            clientSchedule.RepeatEndDate = repeatEndDate;
            clientSchedule.StartDate = repeatStartDate;
            clientSchedule.EndDate = repeatStartDate;
        }
        else
        {
            DateTime.TryParse(serverSchedule.Begin_Date, out DateTime startDate);
            DateTime.TryParse(serverSchedule.Finish_Date, out DateTime endDate);
            clientSchedule.StartDate = startDate;
            clientSchedule.EndDate = endDate;
        }

        return clientSchedule;
    }

    // 클라이언트 데이터 -> 서버 요청 데이터로 변환
    internal static List<string> ToServerRequestItems(Schedule clientSchedule)
    {
        string combinedString = $"{clientSchedule.Title}[CONTENT]{clientSchedule.Content}";

        string daily = "", weekly = "", monthly = "", yearly = "";
        switch (clientSchedule.RepeatOption)
        {
            case RepeatType.Daily: daily = "1"; break;
            case RepeatType.Weekly: weekly = string.Join(";", clientSchedule.RepeatDays.Select(d => d.ToString())); break;
            case RepeatType.Monthly: monthly = clientSchedule.RepeatDay.ToString(); break;
            case RepeatType.Yearly: yearly = $"{clientSchedule.RepeatMonth}-{clientSchedule.RepeatDay}"; break;
        }

        string beginDate, finishDate;
        if (clientSchedule.RepeatOption != RepeatType.None)
        {
            beginDate = clientSchedule.RepeatStartDate.ToString("yyyy-MM-dd HH:mm:ss");
            finishDate = clientSchedule.RepeatEndDate.ToString("yyyy-MM-dd HH:mm:ss");
        }
        else
        {
            beginDate = clientSchedule.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
            finishDate = clientSchedule.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // 알림 시간이 설정되지 않았으면 빈 문자열을 보냄
        string alertDateString = (clientSchedule.AlertDateTime == DateTime.MinValue) ? "" : clientSchedule.AlertDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        var items = new List<string>
        {
            clientSchedule.Category ?? "개인",
            beginDate,
            finishDate,
            combinedString,
            daily,
            weekly,
            monthly,
            yearly,
            alertDateString
        };

        return items;
    }

    internal static Schedule ToClientSharedSchedule(Chat_Room__Room_Id__Scheduler serverSchedule)
    {
        if (serverSchedule == null) return null;
        var tempPersonalSchedule = new _User_Id__Scheduler
        {
            Sche_Id = serverSchedule.Sche_Id,
            Category = serverSchedule.Category,
            Begin_Date = serverSchedule.Begin_Date,
            Finish_Date = serverSchedule.Finish_Date,
            Sche_Str = serverSchedule.Sche_Str,
            Daily = serverSchedule.Daily,
            Weekly = serverSchedule.Weekly,
            Monthly = serverSchedule.Monthly,
            Yearly = serverSchedule.Yearly,
            Alert_Date = serverSchedule.Alert_Date
        };
        return ToClientSchedule(tempPersonalSchedule);
    }
}