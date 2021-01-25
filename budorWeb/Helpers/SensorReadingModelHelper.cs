using System.Collections.Generic;
using System.Linq;

namespace budorWeb.Helpers
{
    public static class SensorReadingModelHelper
    {
        public static double GetAverageReadingFromRange(IEnumerable<double> readings)
        {
            var list = readings.ToList();
            return list.Any() ? list.Average() : 0;
        }
    }
}