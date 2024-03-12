using System;
using System.Collections.Generic;
using Shared.Models;

namespace Shared.Interfaces
{
    public interface ISensorReadingRepo : IRepository<SensorReadingModel>
    {
    }

    public interface IBirdPresenceRepo : IRepository<BirdPresenceRegistration>
    {
        BirdPresenceRegistration GetLatestRegistration();
    }

    public interface ISpeciesRecognitionRepo : IRepository<SpeciesRecognitionModel>
    {
        bool Exists(Guid recordingId);
        IEnumerable<SpeciesRecognitionModel> GetLatestRecognitions(int numberOfResults);
    }

    public interface IImageUploadRepo : IRepository<ImageUploadModel>
    {
        IEnumerable<ImageUploadModel> GetLatestUploads(int numberOfUploads);
        IEnumerable<ImageUploadModel> GetUploadsForDay(DateOnly date);
    }

    public interface IHiddenSpeciesRepo : IRepository<HiddenSpeciesModel>
    {
    }
}