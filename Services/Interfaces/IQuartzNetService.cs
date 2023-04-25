using System;
using Quartz;

namespace Services.Interfaces;

public interface IQuartzNetService
{
    void UpdateTriggerInterval(IJobExecutionContext context, TimeSpan newInterval);
}