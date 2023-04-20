using System.Threading;
using System.Threading.Tasks;
using Shared.Interfaces;

namespace DataAccess.EFCore
{
    public class Repositories : IRepositories
    {
        private readonly BudorDbContext _context;

        public Repositories(BudorDbContext context)
        {
            _context = context;
            SensorReadings = new SensorReadingRepo(context);
            BirdPresenceRegistrations = new BirdPresenceRepo(context);
            SpeciesRecognitions = new SpeciesRecognitionRepo(context);
            ImageUploads = new ImageUploadRepo(context);
            HiddenSpecies = new HiddenSpeciesRepo(context);
        }

        public ISpeciesRecognitionRepo SpeciesRecognitions { get; }
        public IImageUploadRepo ImageUploads { get; set; }
        public IHiddenSpeciesRepo HiddenSpecies { get; set; }
        public ISensorReadingRepo SensorReadings { get; }
        public IBirdPresenceRepo BirdPresenceRegistrations { get; }


        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}