using System;

namespace DailyProject_221204
{
    public static class ExtendDateTime
    {
        public static DateTime GetFirstDayOfWeek(this DateTime dataTime, DayOfWeek startDayOfWeek = DayOfWeek.Sunday)
        {
            if (startDayOfWeek == DayOfWeek.Sunday)
            {
                return dataTime.AddDays(DayOfWeek.Sunday - dataTime.DayOfWeek);
            }
            else
            {
                var d = startDayOfWeek - dataTime.DayOfWeek;
                return dataTime.AddDays((d > 0) ? d - 7 : d);
            }
        }

        public static DateTime GetLastDayOfWeek(this DateTime dataTime, DayOfWeek startDayOfWeek = DayOfWeek.Sunday)
        {
            if (startDayOfWeek == DayOfWeek.Sunday)
            {
                return dataTime.AddDays(DayOfWeek.Saturday - dataTime.DayOfWeek);
            }
            else
            {
                var d = startDayOfWeek - dataTime.DayOfWeek;
                return dataTime.AddDays((d == 1) ? 0 : 6 + d);
            }
        }

        public static DateTime GetDayOfWeekInThisWeek(this DateTime date, DayOfWeek dayOfWeek = DayOfWeek.Sunday)
        {
            var fisrtDay = date.GetFirstDayOfWeek(DayOfWeek.Sunday);
            var targetDay = fisrtDay.AddDays((int)dayOfWeek);
            return targetDay;
        }
    }
}
