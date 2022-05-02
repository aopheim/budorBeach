using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBirdNetServer
    {
        Task<bool> StartBirdNETServer();
    }
}