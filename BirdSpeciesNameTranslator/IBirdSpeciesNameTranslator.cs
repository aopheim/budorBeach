namespace BirdSpeciesNameTranslator;

public interface IBirdSpeciesNameTranslator
{
    string? TranslateFromTaxonomyCode(string eBirdTaxonomyCode, string translateToLocale = "no");

    string? TranslateFromLatinName(string eBirdTaxonomyCode, string translateToLocale = "no");

    string? GetLatinNameFromTaxonomyCode(string eBirdTaxonomyCode);

    string? GetTaxonomyCodeFromLatinAndEnglishName(string latinName, string englishName);
}