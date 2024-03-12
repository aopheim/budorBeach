using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
using Shared.Interfaces;
using Shared.Models;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Services
{
    public class BirdPresenceRegistrator : IBirdPresenceRegistrator
    {
        private const int MaximumNumberOfRegistrations = 100;
        private readonly TimeSpan _bufferTimeSpanForRegistrations = TimeSpan.FromSeconds(60);
        private readonly IBirdPresenceCalculator _calculator;
        private readonly Container _container;
        private readonly Queue<Tuple<DateTime, double>> _registrations = new Queue<Tuple<DateTime, double>>();
        private bool _dbHasOpenPresenceRegistration;
        private TimeSpan _triggerIntervalAtBirdPresence = TimeSpan.FromSeconds(2);
        private TimeSpan _triggerIntervalAtRest = TimeSpan.FromSeconds(10);

        public BirdPresenceRegistrator(
            Container container, IBirdPresenceCalculator calculator
        )
        {
            _container = container;
            _calculator = calculator;
        }

        public IEnumerable Registrations => _registrations;

        public async Task ReceiveDistanceUpdate(double currentDistance, DateTime measuredAt, CancellationToken
            cancellationToken)
        {
            RemoveOutdatedRegistrations();
            var shouldAddRegistrationToQueue = _registrations.Count == 0 || _registrations.First().Item1 < measuredAt;
            if (shouldAddRegistrationToQueue)
                _registrations.Enqueue(new Tuple<DateTime, double>(measuredAt, currentDistance));

            using var scope = AsyncScopedLifestyle.BeginScope(_container);
            var repos = scope.GetRequiredService<IRepositories>();

            var birdIsPresent = _calculator.BirdIsPresent(_registrations);
            switch (birdIsPresent)
            {
                case true when !_dbHasOpenPresenceRegistration:
                    await AddBirdPresenceRegistrationToDb(measuredAt, repos, cancellationToken);
                    break;
                case false when !_dbHasOpenPresenceRegistration:
                case true when _dbHasOpenPresenceRegistration:
                    return;
                default:
                    await CloseBirdPresenceRegistrationInDb(measuredAt, repos, cancellationToken);
                    break;
            }

            await repos.SaveChangesAsync(cancellationToken);
        }

        private async Task AddBirdPresenceRegistrationToDb(DateTime measuredAt, IRepositories repos,
            CancellationToken cancellationToken)
        {
            var newReg = new BirdPresenceRegistration
            {
                StartedAt = measuredAt,
                DurationInSeconds = null
            };

            await repos.BirdPresenceRegistrations.AddAsync(newReg, cancellationToken);
            _dbHasOpenPresenceRegistration = true;
        }

        private async Task CloseBirdPresenceRegistrationInDb(DateTime birdDisappearedAt, IRepositories repos,
            CancellationToken cancellationToken)
        {
            var openRegistration = repos.BirdPresenceRegistrations.GetLatestRegistration();
            openRegistration.DurationInSeconds =
                (int)Math.Round((birdDisappearedAt - openRegistration.StartedAt).TotalSeconds);
            await repos.BirdPresenceRegistrations.UpdateAsync(openRegistration, cancellationToken);
            _dbHasOpenPresenceRegistration = false;
        }

        private void RemoveOutdatedRegistrations()
        {
            while (true)
            {
                var existOldRegs = _registrations.Any(reg =>
                    reg.Item1 < DateTime.UtcNow.Subtract(_bufferTimeSpanForRegistrations));
                var shouldRemoveRegistrations = _registrations.Count >= MaximumNumberOfRegistrations || existOldRegs;
                if (shouldRemoveRegistrations)
                {
                    _registrations.Dequeue();
                    continue;
                }

                break;
            }
        }
    }

    public interface IBirdPresenceRegistrator
    {
        Task ReceiveDistanceUpdate(double currentDistance, DateTime measuredAt, CancellationToken cancellationToken);
    }
}