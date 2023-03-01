using System.Diagnostics;
using System.Threading;

namespace Services.Interfaces
{
    public interface IExternalProcess
    {
        Process StartExternalProcess(bool isWindows, string commandLineArguments, CancellationToken cancellationToken);
    }

    public interface IExternalSingletonProcess
    {
        Process StartExternalSingletonProcess(bool isWindows, string commandLineArguments,
            CancellationToken cancellationToken);
    }
}