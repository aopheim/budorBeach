using Microsoft.EntityFrameworkCore;
using Shared.Models;

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