using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Services;

namespace rpiDaemon.Test.Services
{
    public class BirdNetResultConverterTests
    {
        private BirdNetResultConverter Converter { get; set; }

        [SetUp]
        public void SetUp()
        {
            Converter = new BirdNetResultConverter();
        }

        [Test]
        public void ConvertJson_ShouldReadIntoList()
        {
            var json =
                "{\"msg\": \"success\", \"results\": [{\"common_name\": \"Great Tit\", \"scientific_name\": \"Parus major\", \"start_time\": 0.0, \"end_time\": 3.0, \"confidence\": 0.46539098024368286, \"label\": \"Parus major_Great Tit\"}, {\"common_name\": \"Great Tit\", \"scientific_name\": \"Parus major\", \"start_time\": 6.0, \"end_time\": 9.0, \"confidence\": 0.6198630332946777, \"label\": \"Parus major_Great Tit\"}]}";

            var res = Converter.ConvertJson(json);

            res.Message.Should().Be("success");
            res.Results.Should().HaveCount(2);
            res.Results.First().Confidence.Should().Be(0.46539098024368286);
            res.Results.First().EnglishName.Should().Be("Great Tit");
            res.Results.First().LatinName.Should().Be("Parus major");
            res.Results.First().StartTime.Should().Be(0.0);
            res.Results.First().EndTime.Should().Be(3.0);

            res.Results.Last().Confidence.Should().Be(0.6198630332946777);
            res.Results.Last().EnglishName.Should().Be("Great Tit");
            res.Results.Last().LatinName.Should().Be("Parus major");
            res.Results.Last().StartTime.Should().Be(6.0);
            res.Results.Last().EndTime.Should().Be(9.0);
        }

        [Test]
        public void NotSuccess_ShouldThrow()
        {
            var json =
                "{\"msg\": \"fail\", \"results\": []}";

            Assert.Throws<ArgumentException>(() => Converter.ConvertJson(json));
        }
    }
}