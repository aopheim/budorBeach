using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.EFCore
{
    public class BudorDbContextDesignTimeFactory : IDesignTimeDbContextFactory<BudorDbContext>
    {
        public BudorDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                .AddUserSecrets("7a7d43a7-4a55-45c9-b915-7c7aec6d1751")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BudorDbContext>();
            optionsBuilder.UseSqlServer(environment == "Production"
                ? config["ProductionDb"]
                : config["DevelopmentDb"]);

            return new BudorDbContext(optionsBuilder.Options);
        }
    }
}