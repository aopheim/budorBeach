using System;

namespace rpiDaemon.DateTimeHelpers
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

        public static DateTime GetDateTimeFromFolderAndFileName(string folderAndFileName)
        {
            var split = folderAndFileName.Split('/');

            if (split.Length != 2)
                return default;

            var folderDate = GetDateTimeDateFromFolderName(split[0]);
            var timeWithDash = split[1].Split('.')[0];
            var splitTime = timeWithDash.Split('-');
            if (splitTime.Length != 3)
                return default;

            var hour = int.Parse(splitTime[0]);
            var minute = int.Parse(splitTime[1]);
            var second = int.Parse(splitTime[2]);

            var toReturn = folderDate.AddHours(hour).AddMinutes(minute).AddSeconds(second);

            return toReturn;
        }
    }
}