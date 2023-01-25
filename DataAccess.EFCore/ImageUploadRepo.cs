using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Interfaces;
using Shared.Models;

namespace DataAccess.EFCore;

public class ImageUploadRepo : Repository<ImageUploadModel>, IImageUploadRepo
{
    private readonly BudorDbContext _context;

    public ImageUploadRepo(BudorDbContext context) : base(context)
    {
        _context = context;
    }

    public IEnumerable<ImageUploadModel> GetLatestUploads(int numberOfUploads)
    {
        return _context.ImageUploads.OrderByDescending(iu => iu.TakenAtUtc).Take(numberOfUploads);
    }

    public IEnumerable<ImageUploadModel> GetUploadsForDay(DateOnly date)
    {
        var startOfDay = date.ToDateTime(new TimeOnly(00, 00));
        var endOfDay = date.AddDays(1).ToDateTime(new TimeOnly(00, 00)).AddSeconds(-1);
        return _context.ImageUploads.Where(iu =>
            iu.TakenAtUtc > startOfDay && iu.TakenAtUtc < endOfDay
        );
    }
}