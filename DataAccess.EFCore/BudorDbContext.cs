using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace DataAccess.EFCore
{
    public class BudorDbContext : DbContext
    {
        public BudorDbContext(DbContextOptions<BudorDbContext> option) : base(option)
        {
        }


        public DbSet<SensorReadingModel> SensorReadings { get; set; }
        public DbSet<BirdPresenceRegistration> BirdPresenceRegistrations { get; set; }
        public DbSet<SpeciesRecognitionModel> SpeciesRecognitions { get; set; }
        public DbSet<ImageUploadModel> ImageUploads { get; set; }
        public DbSet<HiddenSpeciesModel> HiddenSpecies { get; set; }
    }
}