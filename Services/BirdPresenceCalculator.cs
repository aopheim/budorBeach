using System;
using System.Collections.Generic;
using System.Linq;
using Services.Interfaces;

namespace Services
{
    public class BirdPresenceCalculator : IBirdPresenceCalculator
    {
        public const double EmptyDistance = 30;
        public const double MinimumChangeInDistance = 2;

        public bool BirdIsPresent(IEnumerable<Tuple<DateTime, double>> registrations)
        {
            var distances = registrations.ToList().Select(reg => reg.Item2).ToList();
            var avg = distances.Average();
            var stdDev = GetStandardDeviation(distances);

            return stdDev > MinimumChangeInDistance;
        }

        private static double GetStandardDeviation(IEnumerable<double> sequence)
        {
            sequence = sequence.ToList();
            double result = 0;

            if (!sequence.Any()) return result;
            var average = sequence.Average();
            var sum = sequence.Sum(d => Math.Pow(d - average, 2));
            result = Math.Sqrt(sum / (sequence.Count() - 1));
            return result;
        }
    }
}