using System;
using System.Linq;
using BaseUnitTests;
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
        public void ReceiveDistanceUpdate_WithNoExistingRegistrations_ShouldAddRegistration()
        {
            var measuredAt = DateTime.Now;

            TestSubject.ReceiveDistanceUpdate(20, measuredAt);

            TestSubject.Registrations.Should().HaveCount(1);
        }

        [Test]
        public void ReceiveDistanceUpdate_ShouldNotExceedMaxRegistrations()
        {
            const int maxRegs = 100;
            for (var i = 0; i < maxRegs + 1; i++) TestSubject.ReceiveDistanceUpdate(i, DateTime.UtcNow);

            TestSubject.Registrations.Should().HaveCount(maxRegs);
            TestSubject.Registrations.Should().NotContainNulls();
        }

        [Test]
        public void ReceiveDistanceUpdate_ShouldNotHaveOldRegistrations()
        {
            var maxTimeSpan = TimeSpan.FromSeconds(60);
            var measuredAt = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));
            for (var i = 0; i < 15; i++) TestSubject.ReceiveDistanceUpdate(i, measuredAt.AddSeconds(i));
            TestSubject.ReceiveDistanceUpdate(5, DateTime.UtcNow);

            TestSubject.Registrations.Should().HaveCount(1);
        }

        [Test]
        public void ReceiveDistanceUpdate_ShouldNotAddRegistrationsWithMeasuredAtBeforeFirstEnquedElement()
        {
            var now = DateTime.UtcNow;
            var beforeNow = now.AddSeconds(-1);

            TestSubject.ReceiveDistanceUpdate(20, now);
            TestSubject.Registrations.Should().HaveCount(1);

            TestSubject.ReceiveDistanceUpdate(25, beforeNow);
            TestSubject.Registrations.Should().HaveCount(1);
        }

        [Test]
        public void NoExistingRegistration_WithBirdPresence_ShouldAddInitialRegistration()
        {
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(true);

            var measuredAt = DateTime.Now;
            TestSubject.ReceiveDistanceUpdate(20, measuredAt);

            var calls = Get<IRepositories>().BirdPresenceRegistrations.ReceivedCalls();
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.Add)).GetArguments().First().Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = measuredAt, });
        }

        [Test]
        public void NoExistingRegistration_ButNoBirdPresence_ShouldNotAddInitialRegistration()
        {
            var measuredAt = DateTime.Now;

            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(false);
            TestSubject.ReceiveDistanceUpdate(20, measuredAt);

            var calls = Get<IRepositories>().BirdPresenceRegistrations.ReceivedCalls();
            calls.Should().BeEmpty();
        }

        [Test]
        public void OpenRegistration_AndBirdStillPresent_ShouldNotCallDbTwice()
        {
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(true);

            var now = DateTime.UtcNow;
            TestSubject.ReceiveDistanceUpdate(20, now);


            Get<IBirdPresenceCalculator>().BirdIsPresent(default).Returns(true);
            TestSubject.ReceiveDistanceUpdate(21, now.AddSeconds(2));

            var calls = Get<IRepositories>().BirdPresenceRegistrations.ReceivedCalls();
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.Add)).GetArguments().First().Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = now, });
        }

        [Test]
        public void ShouldAddAndCloseRegistration()
        {
            var now = DateTime.UtcNow;
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(true);
            TestSubject.ReceiveDistanceUpdate(20, now);

            Get<IRepositories>().BirdPresenceRegistrations.GetLatestRegistration()
                .ReturnsForAnyArgs(new BirdPresenceRegistration { StartedAt = now, DurationInSeconds = null });
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(false);
            TestSubject.ReceiveDistanceUpdate(20, now.AddSeconds(15));

            var calls = Get<IRepositories>().BirdPresenceRegistrations.ReceivedCalls().ToList();
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.Add)).GetArguments().First().Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = now, });
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.Update)).GetArguments().First()
                .Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = now, DurationInSeconds = 15 });
        }

        [Test]
        public void ShouldAddAndCloseRegistration_WithCorrectNumberOfSeconds()
        {
            var now = DateTime.UtcNow;
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(true);
            TestSubject.ReceiveDistanceUpdate(20, now);

            Get<IRepositories>().BirdPresenceRegistrations.GetLatestRegistration()
                .ReturnsForAnyArgs(new BirdPresenceRegistration { StartedAt = now, DurationInSeconds = null });
            Get<IBirdPresenceCalculator>().BirdIsPresent(default).ReturnsForAnyArgs(false);
            TestSubject.ReceiveDistanceUpdate(20, now.AddSeconds(100));

            var calls = Get<IRepositories>().BirdPresenceRegistrations.ReceivedCalls().ToList();
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.Add)).GetArguments().First().Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = now, });
            calls.Single(c => c.GetMethodInfo().Name == nameof(IBirdPresenceRepo.Update)).GetArguments().First()
                .Should()
                .BeEquivalentTo(new BirdPresenceRegistration { StartedAt = now, DurationInSeconds = 100 });
        }
    }
}