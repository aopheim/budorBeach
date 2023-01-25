using System;

namespace Shared.DateTimeHelpers
{
    public static class DateTimeParser
    {
        public static string GetFolderName(DateTime date)
        {
            return $"{date.Year}-{date.Month}-{date.Day}";
        }

        public static string GetFileName(DateTime date)
        {
            return $"{date.Hour}-{date.Minute}-{date.Second}";
        }

        public static DateTime GetDateTimeDateFromFolderName(string folderName)
        {
            var split = folderName.Split('-');
            return split.Length != 3
                ? default
                : new DateTime(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), 00, 00, 00);
        }

        // Returns null if unable to parse filename
        public static DateTime? GetDateTimeFromFolderAndFileName(string folderAndFileName)
        {
            var split = folderAndFileName.Split('/');

            if (split.Length != 2)
                return null;

            var folderDate = GetDateTimeDateFromFolderName(split[0]);
            var timeWithDash = split[1].Split('.')[0];
            var splitTime = timeWithDash.Split('-');
            if (splitTime.Length != 3)
                return null;

            var hour = int.Parse(splitTime[0]);
            var minute = int.Parse(splitTime[1]);
            var second = int.Parse(splitTime[2]);

            var toReturn = folderDate.AddHours(hour).AddMinutes(minute).AddSeconds(second);

            return toReturn;
        }

        public static string GetLocalDateTimeAsString(DateTime? dateTimeInUtc)
        {
            if (dateTimeInUtc == null)
                return "";
            var localTime = dateTimeInUtc.Value.ToEuropeanStandardTime();
            return
                $"{localTime.Day:D2}.{localTime.Month:D2}.{localTime.Year:D4}, {localTime.Hour:D2}:{localTime.Minute:D2}:{localTime.Second:D2}";
        }

        public static string GetLocalDateAsString(DateTime? dateTimeInUtc)
        {
            if (dateTimeInUtc == null)
                return "";
            var localTime = dateTimeInUtc.Value.ToEuropeanStandardTime();
            return
                $"{localTime.Day:D2}.{localTime.Month:D2}.{localTime.Year:D4}";
        }

        public static DateTime ToEuropeanStandardTime(this DateTime dateTimeInUtc)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTimeInUtc, timeZoneInfo);
        }
    }
}