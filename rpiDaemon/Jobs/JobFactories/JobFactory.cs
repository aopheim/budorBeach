using System;
using JetBrains.Annotations;
using Quartz;
using Quartz.Spi;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace rpiDaemon.Jobs.JobFactories
{
    [UsedImplicitly]
    public class JobFactory : IJobFactory
    {
        private readonly Container _container;

        public JobFactory(Container container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            AsyncScopedLifestyle.BeginScope(_container);
            return _container.GetInstance(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}