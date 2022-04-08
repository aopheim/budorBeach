using System;
using System.Collections.Generic;

namespace Services.Interfaces
{
    public interface IBirdPresenceCalculator
    {
        bool BirdIsPresent(IEnumerable<Tuple<DateTime, double>> registrations);
    }
}