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
}