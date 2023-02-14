using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Services;

namespace rpiDaemon.Test.Services
{
    public class BirdPresenceCalculatorTests : UnitTestBase<BirdPresenceCalculator>
    {
        [TestCaseSource(typeof(BirdPresenceDistanceCreator))]
        public void BirdShouldBePresent(params double[] distances)
        {
            var registrations = new Queue<Tuple<DateTime, double>>();
            var now = DateTime.UtcNow;
            foreach (var distance in distances)
            {
                registrations.Enqueue(new Tuple<DateTime, double>(now.AddSeconds(1), distance));
                now = now.AddSeconds(1);
            }

            TestSubject.BirdIsPresent(registrations).Should().BeTrue();
        }

        [TestCaseSource(typeof(BirdAbsenceDistanceCreator))]
        public void BirdShouldBeAbsent(params double[] distances)
        {
            var registrations = new Queue<Tuple<DateTime, double>>();
            var now = DateTime.UtcNow;
            foreach (var distance in distances)
            {
                registrations.Enqueue(new Tuple<DateTime, double>(now.AddSeconds(1), distance));
                now = now.AddSeconds(1);
            }

            TestSubject.BirdIsPresent(registrations).Should().BeFalse();
        }

        [TestCaseSource(typeof(BirdPresenceDistanceCreator))]
        [Ignore("Dependent on what hardware the test is run on")]
        public void BirdIsPresent_ShouldHaveGoodPerformance(params double[] distances)
        {
            var registrations = new Queue<Tuple<DateTime, double>>();
            var now = DateTime.UtcNow;
            foreach (var distance in distances)
            {
                registrations.Enqueue(new Tuple<DateTime, double>(now.AddSeconds(1), distance));
                now = now.AddSeconds(1);
            }

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            const int numberOfRuns = 100;
            for (var i = 0; i < numberOfRuns; i++) TestSubject.BirdIsPresent(registrations);
            watch.Stop();

            watch.ElapsedMilliseconds.Should().BeLessThan((int)Math.Round(numberOfRuns * 1.0));
        }
    }

    public class BirdPresenceDistanceCreator : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            const double avg = BirdPresenceCalculator.EmptyDistance - 5;
            var distances = new List<double>();
            var variation = (int)Math.Round(BirdPresenceCalculator.MinimumChangeInDistance) * 2;
            var random = new Random();
            for (var i = 0; i < 50; i++)
            {
                var randomVariation = random.Next(-variation, variation);
                distances.Add(avg + randomVariation);
            }

            yield return distances.ToArray();
        }
    }

    public class BirdAbsenceDistanceCreator : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            const double avg = BirdPresenceCalculator.EmptyDistance - 5;
            var distances = new List<double>();
            var variation = 1;
            var random = new Random();
            for (var i = 0; i < 50; i++)
            {
                var randomVariation = random.Next(-variation, variation);
                distances.Add(avg + randomVariation);
            }

            yield return distances.ToArray();
        }
    }
}