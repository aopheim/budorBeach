using System.Threading.Tasks;
using Quartz;

namespace rpiDaemon.Jobs
{
    public class TakeVideoJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}