using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BirdSpeciesNameTranslator;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;
using Shared.Models;

namespace budorWeb.Pages.Species;

public class Species : PageModel
{
    private readonly IAzureStorageService _azureStorageService;
    private readonly IRepositories _repos;
    private readonly IBirdSpeciesNameTranslator _translator;

    public Species(IRepositories repos, IBirdSpeciesNameTranslator translator, IAzureStorageService azureStorageService)
    {
        _repos = repos;
        _translator = translator;
        _azureStorageService = azureStorageService;
    }

    public IEnumerable<SpeciesCountDto> AllSpeciesCount { get; set; }
    [CanBeNull] public string SpeciesId { get; set; }
    [CanBeNull] public string NorwegianSpeciesName { get; set; }
    [CanBeNull] public string LatinSpeciesName { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public IEnumerable<SpeciesRecognitionDto> HighestRecognitionConfidences { get; set; }

    public async Task OnGet([CanBeNull] string id, DateTime? fromDate, DateTime? toDate,
        CancellationToken cancellationToken)
    {
        SpeciesId = id;
        NorwegianSpeciesName = id != null ? _translator.TranslateFromTaxonomyCode(id) : null;
        LatinSpeciesName = id != null ? _translator.GetLatinNameFromTaxonomyCode(id) : null;
        AllSpeciesCount = SpeciesId == null
            ? await GetSpeciesCount(fromDate, toDate, cancellationToken)
            : new List<SpeciesCountDto>();
        HighestRecognitionConfidences = SpeciesId != null
            ? await GetRecognitionsWithHighestConfidence(SpeciesId, cancellationToken)
            : new List<SpeciesRecognitionDto>();
        FromDate = fromDate;
        ToDate = toDate;
    }


    private async Task<IEnumerable<SpeciesCountDto>> GetSpeciesCount(DateTime? fromDate, DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var foundRecognitions = new List<SpeciesRecognitionModel>();
        if (fromDate != null && toDate != null)
            foundRecognitions =
                (await _repos.SpeciesRecognitions.WhereAsync(
                    r => r.RecognizedAtUtc > fromDate && r.RecognizedAtUtc <= toDate,
                    cancellationToken)).ToList();
        else foundRecognitions = (await _repos.SpeciesRecognitions.GetAllAsync(cancellationToken)).ToList();
        var dict = new Dictionary<string, List<SpeciesRecognitionModel>>();
        foreach (var recognition in foundRecognitions)
        {
            if (!dict.ContainsKey(recognition.LatinName))
                dict[recognition.LatinName] = new List<SpeciesRecognitionModel>();
            dict[recognition.LatinName].Add(recognition);
        }

        return dict.Select(pair => new SpeciesCountDto
        {
            LatinName = pair.Key,
            TotalCount = pair.Value.Count,
            NorwegianName = _translator.TranslateFromLatinName(pair.Key),
            SpeciesId = pair.Value.FirstOrDefault()?.EBirdTaxonomyId ?? ""
        }).OrderByDescending(p => p.TotalCount);
    }

    private async Task<IEnumerable<SpeciesRecognitionDto>> GetRecognitionsWithHighestConfidence(string speciesId,
        CancellationToken cancellationToken)
    {
        var speciesRecognitions =
            await _repos.SpeciesRecognitions.WhereAsync(s => s.EBirdTaxonomyId == speciesId, cancellationToken);
        return speciesRecognitions.OrderByDescending(r => r.Confidence).Select(r => new SpeciesRecognitionDto
        {
            Confidence = r.Confidence,
            RecordingUrl =
                _azureStorageService.GetBlobUrl(GlobalConstants.AudioRecordingsContainerName, $"{r.RecordingId}.wav"),
            RecognizedAtUtc = r.RecognizedAtUtc
        }).Take(50);
    }
}

public class SpeciesRecognitionDto
{
    public double Confidence { get; set; }
    public string RecordingUrl { get; set; }
    public DateTime RecognizedAtUtc { get; set; }
}

public class SpeciesCountDto
{
    public string LatinName { get; set; }
    public string NorwegianName { get; set; }
    public string SpeciesId { get; set; }
    public int TotalCount { get; set; }
}