using System.Threading;
using System.Threading.Tasks;
using Shared.RpiDaemonSettings;

namespace Services.Interfaces;

public interface IRpiDaemonSettingsService
{
    Task SetRpiDaemonSettings(IRpiDaemonSettings settings, CancellationToken cancellationToken);
    Task<IRpiDaemonSettings> GetRpiDaemonSettings(CancellationToken cancellationToken);
}