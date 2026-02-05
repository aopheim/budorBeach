using System.Text.Json;

namespace BirdSpeciesNameTranslator;

public class SpeciesNameTranslator : IBirdSpeciesNameTranslator
{
    private const string TaxonomyFileName = "eBird_taxonomy_codes_2021E.json";
    private const string TranslationFileName = "BirdNET_GLOBAL_3K_V2.2_Labels";
    private readonly Dictionary<string, string> _taxonomyDictionary;

    public SpeciesNameTranslator()
    {
        _taxonomyDictionary = GetTaxonomyDictionary() ?? new Dictionary<string, string>();
    }


    public string? TranslateFromTaxonomyCode(string eBirdTaxonomyCode, string translateToLocale = "no")
    {
        var latinName = GetLatinNameFromTaxonomyCode(eBirdTaxonomyCode);
        if (latinName == null) return null;
        return GetLocaleNameFromLatinName(latinName, translateToLocale);
    }

    public string? TranslateFromLatinName(string latinName, string translateToLocale = "no")
    {
        return GetLocaleNameFromLatinName(latinName, translateToLocale);
    }

    public string? GetLatinNameFromTaxonomyCode(string eBirdTaxonomyCode)
    {
        var latinAndEnglishName = _taxonomyDictionary?[eBirdTaxonomyCode];
        if (latinAndEnglishName == null || latinAndEnglishName.Split('_').Length != 2)
            return null;
        return latinAndEnglishName.Split('_').First();
    }

    public string? GetTaxonomyCodeFromLatinAndEnglishName(string latinName, string englishName)
    {
        if (!_taxonomyDictionary.Any()) return null;
        return _taxonomyDictionary.TryGetValue($"{latinName}_{englishName}", out var speciesId) ? speciesId : null;
    }

    private Dictionary<string, string>? GetTaxonomyDictionary()
    {
        var path = @$"translations/{TaxonomyFileName}";
        if (!File.Exists(path)) return null;
        var taxonomyAsString = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(taxonomyAsString);
    }

    private string? GetLocaleNameFromLatinName(string latinName, string translateToLocale)
    {
        var path = $@"translations/{TranslationFileName}_{translateToLocale}.txt";
        if (!File.Exists(path)) return null;
        var lines = File.ReadAllLines(
            path);
        foreach (var line in lines)
        {
            if (line.Split('_').Length != 2) return null;
            var latinNameCandidate = line.Split('_').First();
            if (latinName != latinNameCandidate) continue;
            var localeName = line.Split('_').Last();
            return string.Concat(localeName[..1].ToUpper(), localeName.AsSpan(1));
        }

        return null;
    }
}