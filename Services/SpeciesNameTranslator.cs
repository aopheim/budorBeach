using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Services.Interfaces;

namespace Services;

public class SpeciesNameTranslator : ISpeciesNameTranslator
{
    private const string TaxonomyFileName = "eBird_taxonomy_codes_2021E.json";
    private const string TranslationFileName = "BirdNET_GLOBAL_3K_V2.2_Labels";
    private readonly IWebHostEnvironment _environment;

    public SpeciesNameTranslator(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [CanBeNull]
    public string TranslateFromTaxonomyCode(string eBirdTaxonomyCode, string translateToLocale = "no")
    {
        var latinName = GetLatinNameFromTaxonomyCode(eBirdTaxonomyCode);
        return GetLocaleNameFromLatinName(latinName, translateToLocale);
    }

    [CanBeNull]
    public string TranslateFromLatinName(string latinName, string translateToLocale = "no")
    {
        return GetLocaleNameFromLatinName(latinName, translateToLocale);
    }

    public string GetLatinNameFromTaxonomyCode(string eBirdTaxonomyCode)
    {
        var dict = GetTaxonomyDictionary();
        var latinAndEnglishName = dict?[eBirdTaxonomyCode];
        if (latinAndEnglishName == null || latinAndEnglishName.Split('_').Length != 2)
            return null;
        return latinAndEnglishName.Split('_').First();
    }

    [CanBeNull]
    public string GetTaxonomyCodeFromLatinAndEnglishName(string latinName, string englishName)
    {
        var dict = GetTaxonomyDictionary();
        return dict.TryGetValue($"{latinName}_{englishName}", out var speciesId) ? speciesId : null;
    }

    private Dictionary<string, string> GetTaxonomyDictionary()
    {
        var path = @$"{_environment.WebRootPath}/translations/{TaxonomyFileName}";
        if (!File.Exists(path)) return null;
        var taxonomyAsString = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(taxonomyAsString);
    }

    [CanBeNull]
    private string GetLocaleNameFromLatinName(string latinName, string translateToLocale)
    {
        var path = $@"{_environment.WebRootPath}/translations/{TranslationFileName}_{translateToLocale}.txt";
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