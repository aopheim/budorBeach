using Shared.Interfaces;
using Shared.Models;

namespace DataAccess.EFCore
{
    public class SpeciesRecognitionRepo : Repository<SpeciesRecognitionModel>, ISpeciesRecognitionRepo
    {
        public SpeciesRecognitionRepo(BudorDbContext context) : base(context)
        {
        }
    }
}