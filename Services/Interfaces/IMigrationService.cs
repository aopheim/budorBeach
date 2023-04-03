using System.Threading;
using System.Threading.Tasks;

namespace Services.Interfaces;

public interface IMigrationService
{
    Task MigrateJpgImagesToWebP(CancellationToken cancellationToken);
    Task MigrateSpeciesRecognitionsToIncludeEBirdTaxonomyId(CancellationToken cancellationToken);
}