using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}