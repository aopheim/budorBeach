using System;
using System.Collections.Generic;

namespace Services
{
    public class BirdPresenceCalculator : IBirdPresenceCalculator
    {
        public bool BirdIsPresent(Queue<Tuple<DateTime, double>> registrations)
        {
            throw new NotImplementedException();
        }
    }

    public interface IBirdPresenceCalculator
    {
        bool BirdIsPresent(Queue<Tuple<DateTime, double>> registrations);
    }
}