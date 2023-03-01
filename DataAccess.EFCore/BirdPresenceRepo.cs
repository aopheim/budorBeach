using System.Linq;
using Shared.Interfaces;
using Shared.Models;

namespace DataAccess.EFCore
{
    public class BirdPresenceRepo : Repository<BirdPresenceRegistration>, IBirdPresenceRepo
    {
        private readonly BudorDbContext _context;

        public BirdPresenceRepo(BudorDbContext context) : base(context)
        {
            _context = context;
        }

        public BirdPresenceRegistration GetLatestRegistration()
        {
            return _context.BirdPresenceRegistrations.OrderByDescending(m => m.StartedAt).FirstOrDefault();
        }
    }
}