using System.Collections.Generic;
using Shared.Models;

namespace Shared.Interfaces;

public interface IVideoUploadRepo : IRepository<VideoUploadModel>
{
    IEnumerable<VideoUploadModel> GetLatestVideoUploads(int numberOfUploads);
}

