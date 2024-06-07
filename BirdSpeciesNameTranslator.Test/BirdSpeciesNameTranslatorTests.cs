using FluentAssertions;

namespace BirdSpeciesNameTranslator.Test;

public class BirdSpeciesNameTranslatorTests
{
    [Test]
    public void TranslateFromLatinName_ShouldWork()
    {
        var translator = new SpeciesNameTranslator();

        var norwegianName = translator.TranslateFromLatinName("Turdus torquatus");

        norwegianName.Should().Be("Ringtrost");
    }

    [Test]
    public void TranslateFromTaxonomyCode_ShouldWork()
    {
        var translator = new SpeciesNameTranslator();

        var norwegianName = translator.TranslateFromTaxonomyCode("rinouz1");

        norwegianName.Should().Be("Ringtrost");
    }

    [Test]
    public void GetTaxonomyCodeFromLatinAndEnglishName_ShouldWork()
    {
        var translator = new SpeciesNameTranslator();

        var taxonomyCode = translator.GetTaxonomyCodeFromLatinAndEnglishName("Turdus torquatus", "Ring Ouzel");

        taxonomyCode.Should().Be("rinouz1");
    }

    [Test]
    public void GetLatinNameFromTaxonomyCode_ShouldWork()
    {
        var translator = new SpeciesNameTranslator();

        var latinName = translator.GetLatinNameFromTaxonomyCode("rinouz1");

        latinName.Should().Be("Turdus torquatus");
    }
}