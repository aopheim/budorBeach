using Microsoft.EntityFrameworkCore;
using rpiDaemon.Models;

namespace rpiDaemon
{
    public class SensorContext : DbContext
    {
        public SensorContext(DbContextOptions<SensorContext> options) : base(options)
        {
        }

        public DbSet<SensorReadingModel> SensorReadings { get; set; }
    }
}