using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rpiDaemon.Models
{
    public class SensorReadingModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime MeasuredAtUtc { get; set; }
        public double TemperatureInDegreesC { get; set; }
        public double PressureInKPa { get; set; }
    }
}