using System;
using System.Threading.Tasks;
using Quartz;

namespace rpiDaemon.Jobs
{
    public class GetCurrentTemperatureJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}