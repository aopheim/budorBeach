using System;
using Quartz;

namespace rpiDaemon.Jobs
{
    public static class QuartzConfiguratorExtensions
    {
        public static void AddJobAndTrigger<T>(
            this IServiceCollectionQuartzConfigurator quartz, string groupName, string triggerName,
            TimeSpan? triggerInterval, DateTime? startsAt = null)
            where T : IJob
        {
            var jobName = typeof(T).Name;
            var jobKey = new JobKey(jobName, groupName);

            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));
            if(triggerInterval != null)
            {
                quartz.AddTrigger(opts => opts.ForJob(jobKey).WithIdentity(triggerName)
                    .StartAt(startsAt ?? DateTime.UtcNow.AddSeconds(5)).WithSimpleSchedule(s =>
                        s.WithInterval(triggerInterval.Value).RepeatForever()));
            }
            else
            {
                quartz.AddTrigger(opts => opts.ForJob(jobKey).WithIdentity(triggerName)
                    .StartAt(startsAt ?? DateTime.UtcNow.AddSeconds(5)));
            }
        }
    }
}