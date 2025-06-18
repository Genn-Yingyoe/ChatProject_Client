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

        // 제목, 내용 분리
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

        // 서버의 Daily, Weekly 등의 필드를 읽어 클라이언트의 반복 옵션으로 변환
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

        // 반복 옵션이 있을 경우, Begin/Finish_Date를 반복 기간으로 해석
        if (clientSchedule.RepeatOption != RepeatType.None)
        {
            DateTime.TryParse(serverSchedule.Begin_Date, out DateTime repeatStartDate);
            DateTime.TryParse(serverSchedule.Finish_Date, out DateTime repeatEndDate);
            clientSchedule.RepeatStartDate = repeatStartDate;
            clientSchedule.RepeatEndDate = repeatEndDate;
            // 반복 일정의 실제 시작/종료 시간은 시작일의 시간 부분을 따름
            clientSchedule.StartDate = repeatStartDate;
            clientSchedule.EndDate = repeatStartDate;
        }
        else // 반복이 아닐 경우, 일반 일정의 시작/종료일로 해석
        {
            DateTime.TryParse(serverSchedule.Begin_Date, out DateTime startDate);
            DateTime.TryParse(serverSchedule.Finish_Date, out DateTime endDate);
            clientSchedule.StartDate = startDate;
            clientSchedule.EndDate = endDate;
        }

        return clientSchedule;
    }

    /// 클라이언트 데이터 -> 서버 요청 데이터로 변환
    internal static List<string> ToServerRequestItems(Schedule clientSchedule)
    {
        string combinedString = $"{clientSchedule.Title}[CONTENT]{clientSchedule.Content}";

        // 반복 옵션에 따라 Daily, Weekly 등의 필드 값을 설정
        string daily = "", weekly = "", monthly = "", yearly = "";
        switch (clientSchedule.RepeatOption)
        {
            case RepeatType.Daily: daily = "1"; break;
            case RepeatType.Weekly: weekly = string.Join(";", clientSchedule.RepeatDays.Select(d => d.ToString())); break;
            case RepeatType.Monthly: monthly = clientSchedule.RepeatDay.ToString(); break;
            case RepeatType.Yearly: yearly = $"{clientSchedule.RepeatMonth}-{clientSchedule.RepeatDay}"; break;
        }

        // 반복일 경우 Begin/Finish_Date에 반복 기간을, 아닐 경우 일정 기간을 저장
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

        // 서버가 요구하는 순서(8개)에 맞춰 items 리스트 생성
        var items = new List<string>
        {
            clientSchedule.Category ?? "개인",
            beginDate,
            finishDate,
            combinedString,
            daily,
            weekly,
            monthly,
            yearly
        };

        return items;
    }

    // 서버의 채팅방 스케줄 데이터를 클라이언트의 Schedule 객체로 변환
    internal static Schedule ToClientSharedSchedule(Chat_Room__Room_Id__Scheduler serverSchedule)
    {
        if (serverSchedule == null) return null;

        // Chat_Room__Room_Id__Scheduler의 필드를 _User_Id__Scheduler 임시 객체로 복사합니다.
        // 서버에 실제로 존재하는 필드만 사용합니다.
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
            Yearly = serverSchedule.Yearly
        };

        // 이제 개인 일정을 변환하는 ToClientSchedule 메서드를 재사용하여 최종 클라이언트 객체를 만듭니다.
        return ToClientSchedule(tempPersonalSchedule);
    }
}