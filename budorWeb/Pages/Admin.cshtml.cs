using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

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

    public int PictureIntervalInMinutes { get; set; }
    public double SpeciesRecognitionConfidence { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var settings = await _rpiDaemonSettingsService.GetRpiDaemonSettings(cancellationToken);
        PictureIntervalInMinutes = settings.PictureIntervalInMinutes;
        SpeciesRecognitionConfidence = settings.SpeciesRecognitionConfidence;
    }

    public void OnPostConvertImages(CancellationToken cancellationToken)
    {
        _migrationService.MigrateJpgImagesToWebP(cancellationToken);
    }

    public async Task OnPostMigrateRecognitions(CancellationToken cancellationToken)
    {
        await _migrationService.MigrateSpeciesRecognitionsToIncludeEBirdTaxonomyId(cancellationToken);
    }
}