using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models;

namespace DataAccess.EFCore
{
    public class SpeciesRecognitionRepo : Repository<SpeciesRecognitionModel>, ISpeciesRecognitionRepo
    {
        private readonly BudorDbContext _context;

        public SpeciesRecognitionRepo(BudorDbContext context) : base(context)
        {
            _context = context;
        }

        public bool Exists(Guid recordingId)
        {
            return _context.SpeciesRecognitions.Any(r => r.RecordingId == recordingId);
        }

        public IEnumerable<SpeciesRecognitionModel> GetLatestRecognitions(int numberOfResults)
        {
            return _context.SpeciesRecognitions.OrderByDescending(r => r.RecognizedAtUtc).Take(numberOfResults);
        }

        public async Task<IEnumerable<SpeciesRecognitionModel>> GetSpeciesRecognitionsUploadedSince(DateTime fromTime,
            CancellationToken cancellationToken)
        {
            return await WhereAsync(r => r.RecordingUploadedAt != null && r.RecordingUploadedAt > fromTime,
                cancellationToken) ?? new List<SpeciesRecognitionModel>();
        }

        public async Task<IEnumerable<SpeciesRecognitionModel>> GetUploadedRecognitionsForEBirdSpeciesId(
            string eBirdTaxonomyId, int maxReturn, CancellationToken cancellationToken)
        {
            return (await WhereAsync(r => r.RecordingUploadedAt != null && r.EBirdTaxonomyId == eBirdTaxonomyId,
                       cancellationToken))?.OrderByDescending(m => m.Confidence).Take(maxReturn) ??
                   new List<SpeciesRecognitionModel>();
        }
    }
}