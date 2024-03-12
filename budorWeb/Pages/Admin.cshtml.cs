using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared.RpiDaemonSettings;

namespace budorWeb.Pages;

public class Admin : PageModel
{
    private readonly ILogger<Admin> _logger;
    private readonly IMigrationService _migrationService;
    private readonly IRpiDaemonSettingsService _rpiDaemonSettingsService;

    public Admin(IMigrationService migrationService, ILogger<Admin> logger,
        IRpiDaemonSettingsService rpiDaemonSettingsService)
    {
        _migrationService = migrationService;
        _logger = logger;
        _rpiDaemonSettingsService = rpiDaemonSettingsService;
    }

    [BindProperty] public int PictureIntervalInMinutes { get; set; }
    [BindProperty] public double SpeciesRecognitionConfidence { get; set; }
    [BindProperty] public bool TakeImagesInTheDark { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var settings = await _rpiDaemonSettingsService.GetRpiDaemonSettings(cancellationToken);
        PictureIntervalInMinutes = settings.PictureIntervalInMinutes;
        SpeciesRecognitionConfidence = settings.SpeciesRecognitionConfidence;
        TakeImagesInTheDark = settings.TakeImagesInTheDark;
    }

    public void OnPostConvertImages(CancellationToken cancellationToken)
    {
        _migrationService.MigrateJpgImagesToWebP(cancellationToken);
    }

    public async Task OnPostMigrateRecognitions(CancellationToken cancellationToken)
    {
        await _migrationService.MigrateSpeciesRecognitionsToIncludeEBirdTaxonomyId(cancellationToken);
    }

    public async Task OnPostSetRpiDaemonSettings(CancellationToken cancellationToken)
    {
        var settings = new RpiDaemonSettings
        {
            SpeciesRecognitionConfidence = SpeciesRecognitionConfidence,
            PictureIntervalInMinutes = PictureIntervalInMinutes,
            TakeImagesInTheDark = TakeImagesInTheDark
        };
        if (!SettingsAreValid(settings))
        {
            _logger.LogWarning("Tried to set invalid RpiDaemon settings. Not saving.");
            return;
        }
        await _rpiDaemonSettingsService.SetRpiDaemonSettings(settings, cancellationToken);
    }

    private static bool SettingsAreValid(RpiDaemonSettings settings)
    {
        if (settings.SpeciesRecognitionConfidence is <= 0 or > 1)
            return false;
        if (settings.PictureIntervalInMinutes <= 0)
            return false;
        return true;
    }
}