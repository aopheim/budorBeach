using JetBrains.Annotations;

namespace Services.Interfaces;

public interface ISpeciesNameTranslator
{
    [CanBeNull] string TranslateFromTaxonomyCode(string eBirdTaxonomyCode, string translateToLocale = "no");
    [CanBeNull] string TranslateFromLatinName(string eBirdTaxonomyCode, string translateToLocale = "no");
}