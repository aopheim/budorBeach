using System;
using NSubstitute;
using NUnit.Framework;
using Services;
using Shared.Interfaces;
using Shared.Models;

namespace rpiDaemon.Test.Services
{
    public class BirdPresenceCalculatorTests : UnitTestBase<BirdPresenceCalculator>
    {
        [Test]
        public void TODO()
        {
            SetupExistingRegistration();

            TestSubject.ReceiveDistanceUpdate(20, DateTime.Now);
        }

        private void SetupExistingRegistration()
        {
            var toReturn = new BirdPresenceRegistration();
            Get<IRepositories>().BirdPresenceReadings.GetLatestRegistration()
                .ReturnsForAnyArgs(toReturn);
        }
    }
}