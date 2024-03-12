using Shared.Interfaces;
using Shared.Models;

namespace DataAccess.EFCore;

public class HiddenSpeciesRepo : Repository<HiddenSpeciesModel>, IHiddenSpeciesRepo
{
    public HiddenSpeciesRepo(BudorDbContext context) : base(context)
    {
    }
}