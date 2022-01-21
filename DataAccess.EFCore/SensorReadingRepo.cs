using Shared.Interfaces;
using Shared.Models;

namespace DataAccess.EFCore
{
    public class SensorReadingRepo : Repository<SensorReadingModel>, ISensorReadingRepo
    {
        public SensorReadingRepo(BudorDbContext context) : base(context)
        {
        }
    }
}