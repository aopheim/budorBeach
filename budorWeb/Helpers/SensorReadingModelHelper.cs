using System.Collections.Generic;
using System.Linq;

namespace budorWeb.Helpers
{
    public static class SensorReadingModelHelper
    {
        public static double GetAverageReadingFromRange(IEnumerable<double> readings)
        {
            return readings.Average();
        }
    }
}