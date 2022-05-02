using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Services;

namespace rpiDaemon.Test.ServicesTests
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
                "{\"msg\": \"success\", \"results\": [[\"Poecile atricapillus_Black-capped Chickadee\", 0.7889], [\"Spinus tristis_American Goldfinch\", 0.5028], [\"Junco hyemalis_Dark-eyed Junco\", 0.4943]]}";

            var res = Converter.ConvertJson(json);

            res.Message.Should().Be("success");
            res.Results.Should().HaveCount(3);
            res.Results.First().Confidence.Should().Be(0.7889);
            res.Results.Last().Confidence.Should().Be(0.4943);
        }

        [Test]
        public void ConvertJson_ShouldParseNames()
        {
            var json =
                "{\"msg\": \"success\", \"results\": [[\"Poecile atricapillus_Black-capped Chickadee\", 0.7889], [\"Spinus tristis_American Goldfinch\", 0.5028], [\"Junco hyemalis_Dark-eyed Junco\", 0.4943]]}";

            var res = Converter.ConvertJson(json);

            res.Results.First().LatinName.Should().Be("Poecile atricapillus");
            res.Results.First().EnglishName.Should().Be("Black-capped Chickadee");
        }
    }
}