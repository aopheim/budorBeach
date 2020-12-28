using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rpiDaemon.Controllers
{
    public class SensorController
    {

        public Tuple<DateTime, double> GetCurrentTemperature()
        {
            return new Tuple<DateTime, double>(DateTime.UtcNow, default);
        }


    }
}
