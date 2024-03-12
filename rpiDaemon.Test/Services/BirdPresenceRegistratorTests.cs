using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Services;
using Services.Interfaces;
using Shared.Interfaces;
using Shared.Models;

namespace rpiDaemon.Test.Services
{
    public class BirdPresenceRegistratorTests : UnitTestBase<BirdPresenceRegistrator>
    {
        [Test]
        public async Task ReceiveDistanceUpdate_WithNoExistingRegistrations_ShouldAddRegistration()
        {
            var measuredAt = DateTime.Now;

            await TestSubject.ReceiveDistanceUpdate(20, measuredAt, default);

            TestSubject.Registrations.Should().HaveCount(1);
        }

        [Test]
        public async Task ReceiveDistanceUpdate_ShouldNotExceedMaxRegistrations()
        {
            const int maxRegs = 100;
            for (var i = 0; i < maxRegs + 1; i++) await TestSubject.ReceiveDistanceUpdate(i, DateTime.UtcNow, default);

            TestSubject.Registrations.Should().HaveCount(maxRegs);
            TestSubject.Registrations.Should().NotContainNulls();
        }

        [Test]
        public async Task ReceiveDistanceUpdate_ShouldNotHaveOldRegistrations()
        {
            var maxTimeSpan = TimeSpan.FromSeconds(60);
            var measuredAt = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));
            for (var i = 0; i < 15; i++) await TestSubject.ReceiveDistanceUpdate(i, measuredAt.AddSeconds(i), default);
            await TestSubject.ReceiveDistanceUpdate(5, DateTime.UtcNow, default);

            TestSubject.Registrations.Should().HaveCount(1);
        }

        [Test]
        public async Task ReceiveDistanceUpdate_ShouldNotAddRegistrationsWithMeasuredAtBeforeFirstEnquedElement()
        {
            var now = DateTime.UtcNow;
            var beforeNow = now.AddSeconds(-1);

            await TestSubject.ReceiveDistanceUpdate(20, now, default);
            TestSubject.Registrations.Should().HaveCount(1);

            await TestSubject.ReceiveDistanceUpdate(25, beforeNow, default);
            TestSubject.Registrations.Should().HaveCount(1);
        }

        [Test]
        public async Task NoExistingRegistration_WithBirdPresence_ShouldAddInitialRegistration()
        {
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(true);

            var measuredAt = DateTime.Now;
            await TestSubject.ReceiveDistanceUpdate(20, measuredAt, default);

            var calls = Get<IRepositories>().BirdPresenceRegistrations.ReceivedCalls();
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.AddAsync)).GetArguments().First()
                .Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = measuredAt, });
        }

        [Test]
        public async Task NoExistingRegistration_ButNoBirdPresence_ShouldNotAddInitialRegistration()
        {
            var measuredAt = DateTime.Now;

            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(false);
            await TestSubject.ReceiveDistanceUpdate(20, measuredAt, default);

            var calls = Get<IRepositories>().BirdPresenceRegistrations.ReceivedCalls();
            calls.Should().BeEmpty();
        }

        [Test]
        public async Task TaskOpenRegistration_AndBirdStillPresent_ShouldNotCallDbTwice()
        {
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(true);

            var now = DateTime.UtcNow;
            await TestSubject.ReceiveDistanceUpdate(20, now, default);


            Get<IBirdPresenceCalculator>().BirdIsPresent(default).Returns(true);
            await TestSubject.ReceiveDistanceUpdate(21, now.AddSeconds(2), default);

            var calls = Get<IRepositories>().BirdPresenceRegistrations.ReceivedCalls();
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.AddAsync)).GetArguments().First()
                .Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = now, });
        }

        [Test]
        public async Task ShouldAddAndCloseRegistration()
        {
            var now = DateTime.UtcNow;
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(true);
            await TestSubject.ReceiveDistanceUpdate(20, now, default);

            Get<IRepositories>().BirdPresenceRegistrations.GetLatestRegistration()
                .ReturnsForAnyArgs(new BirdPresenceRegistration { StartedAt = now, DurationInSeconds = null });
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(false);
            await TestSubject.ReceiveDistanceUpdate(20, now.AddSeconds(15), default);

            var calls = Get<IRepositories>().BirdPresenceRegistrations.ReceivedCalls().ToList();
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.AddAsync)).GetArguments().First()
                .Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = now, });
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.UpdateAsync)).GetArguments().First()
                .Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = now, DurationInSeconds = 15 });
        }

        [Test]
        public async Task ShouldAddAndCloseRegistration_WithCorrectNumberOfSeconds()
        {
            var now = DateTime.UtcNow;
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(true);
            await TestSubject.ReceiveDistanceUpdate(20, now, default);

            Get<IRepositories>().BirdPresenceRegistrations.GetLatestRegistration()
                .ReturnsForAnyArgs(new BirdPresenceRegistration { StartedAt = now, DurationInSeconds = null });
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(false);
            await TestSubject.ReceiveDistanceUpdate(20, now.AddSeconds(100), default);

            var calls = Get<IRepositories>().BirdPresenceRegistrations.ReceivedCalls().ToList();
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.AddAsync)).GetArguments().First()
                .Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = now, });
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.UpdateAsync)).GetArguments().First()
                .Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = now, DurationInSeconds = 100 });
        }
    }
}