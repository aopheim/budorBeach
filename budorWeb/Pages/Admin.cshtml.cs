using System.Threading;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace budorWeb.Pages;

public class Admin : PageModel
{
    private readonly ILogger<Admin> _logger;
    private readonly IMigrationService _migrationService;

    public Admin(IMigrationService migrationService, ILogger<Admin> logger)
    {
        _migrationService = migrationService;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public void OnPostConvertImages(CancellationToken cancellationToken)
    {
        _migrationService.MigrateJpgImagesToWebP(cancellationToken);
    }
}