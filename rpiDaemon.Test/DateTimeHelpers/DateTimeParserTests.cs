using FluentAssertions;
using NUnit.Framework;
using Shared.DateTimeHelpers;

namespace rpiDaemon.Test.DateTimeHelpers
{
    public class DateTimeParserTests
    {
        [TestCase("2020-11-1", 2020, 11, 1)]
        [TestCase("2020-12-1", 2020, 12, 1)]
        [TestCase("2021-1-1", 2021, 1, 1)]
        [TestCase("2021-01-01", 2021, 1, 1)]
        [TestCase("someOtherString", 1, 1, 1)]
        public void GetDateTimeFromFolderName_Works(string folderName, int expectedYear, int expectedMonth,
            int expectedDay)
        {
            var result = DateTimeParser.GetDateTimeDateFromFolderName(folderName);

            result.Year.Should().Be(expectedYear);
            result.Month.Should().Be(expectedMonth);
            result.Day.Should().Be(expectedDay);
        }

        [TestCase("2020-12-31/17-33-12.jpg", 2020, 12, 31, 17, 33, 12)]
        [TestCase("2021-1-17/19-21-49.jpg", 2021, 1, 17, 19, 21, 49)]
        public void GetDateTimeFromFolderName_Works(string folderAndFileName, int expectedYear, int expectedMonth,
            int expectedDay, int expectedHour, int expectedMinute, int expectedSecond)
        {
            var result = DateTimeParser.GetDateTimeFromFolderAndFileName(folderAndFileName).Value;

            result.Year.Should().Be(expectedYear);
            result.Month.Should().Be(expectedMonth);
            result.Day.Should().Be(expectedDay);
            result.Hour.Should().Be(expectedHour);
            result.Minute.Should().Be(expectedMinute);
            result.Second.Should().Be(expectedSecond);
        }
    }
}