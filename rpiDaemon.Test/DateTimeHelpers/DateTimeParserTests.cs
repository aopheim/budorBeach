using FluentAssertions;
using NUnit.Framework;
using rpiDaemon.DateTimeHelpers;

namespace rpiDaemon.Test.DateTimeHelpers
{
    public class DateTimeParserTests
    {
        [TestCase("2020-11-1", 2020, 11, 1)]
        [TestCase("2020-12-1", 2020, 12, 1)]
        [TestCase("2021-1-1", 2021, 1, 1)]
        [TestCase("2021-01-01", 2021, 1, 1)]
        [TestCase("someOtherString", 1, 1, 1)]
        public void GetDateTimeFromFolderName_Works(string folderName, int expextedYear, int expectedMonth,
            int expectedDay)
        {
            var result = DateTimeParser.GetDateTimeFromFolderName(folderName);

            result.Year.Should().Be(expextedYear);
            result.Month.Should().Be(expectedMonth);
            result.Day.Should().Be(expectedDay);
        }
    }
}