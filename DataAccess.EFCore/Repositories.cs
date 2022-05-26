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
        }
        public ISpeciesRecognitionRepo SpeciesRecognitions { get; }
        public ISensorReadingRepo SensorReadings { get; }
        public IBirdPresenceRepo BirdPresenceRegistrations { get; }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}