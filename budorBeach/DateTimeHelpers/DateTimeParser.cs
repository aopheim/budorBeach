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

        public static DateTime GetDateTimeFromFolderName(string folderName)
        {
            var split = folderName.Split('-');
            return split.Length != 3
                ? default
                : new DateTime(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), 00, 00, 00);
        }
    }
}