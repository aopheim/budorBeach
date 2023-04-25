using System;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Interfaces;

namespace Services;

public class QuartzNetService : IQuartzNetService
{
    private readonly ILogger<QuartzNetService> _logger;

    public QuartzNetService(ILogger<QuartzNetService> logger)
    {
        _logger = logger;
    }

    public void UpdateTriggerInterval(IJobExecutionContext context, TimeSpan newInterval)
    {
        var oldTrigger = context.Trigger;
        var builder = oldTrigger.GetTriggerBuilder();
        var nextFireTime = oldTrigger.GetNextFireTimeUtc();
        if (nextFireTime.HasValue &&
            DatesAreClose(nextFireTime.Value.UtcDateTime, DateTime.UtcNow.Add(newInterval)))
        {
            _logger.LogInformation(
                $"Trying to set new trigger interval for job {context.JobDetail.Key.Name} to {newInterval.Minutes} minutes, but trigger already set. Exiting.");
            return;
        }

        var firstNewTriggerTime = DateTimeOffset.UtcNow.Add(newInterval);
        var newTrigger = builder.StartAt(firstNewTriggerTime)
            .WithSimpleSchedule(s => s.WithInterval(newInterval).RepeatForever())
            .Build();
        context.Scheduler.RescheduleJob(oldTrigger.Key, newTrigger);
        _logger.LogInformation(
            $"New trigger interval for job {context.JobDetail.Key.Name} set to {newInterval.Minutes} minutes");
    }

    private static bool DatesAreClose(DateTimeOffset original, DateTimeOffset toCompare,
        TimeSpan errorMargin = default)
    {
        if (errorMargin == default)
            errorMargin = TimeSpan.FromSeconds(5);

        var differenceInSeconds = Math.Abs(original.Subtract(toCompare).Seconds);
        return +differenceInSeconds < errorMargin.Seconds;
    }
}