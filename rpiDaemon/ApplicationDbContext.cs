using Microsoft.EntityFrameworkCore;
using rpiDaemon.Models;

namespace rpiDaemon
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<SensorReadingModel> SensorReadings { get; set; }
    }
}