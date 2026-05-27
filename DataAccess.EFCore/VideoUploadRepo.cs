using System.Collections.Generic;
using System.Linq;
using Shared.Interfaces;
using Shared.Models;

namespace DataAccess.EFCore;

public class VideoUploadRepo : Repository<VideoUploadModel>, IVideoUploadRepo
{
    private readonly BudorDbContext _context;

    public VideoUploadRepo(BudorDbContext context) : base(context)
    {
        _context = context;
    }

    public IEnumerable<VideoUploadModel> GetLatestVideoUploads(int numberOfUploads)
    {
        return _context.VideoUploads.OrderByDescending(vu => vu.RecordedAtUtc).Take(numberOfUploads);
    }
}

