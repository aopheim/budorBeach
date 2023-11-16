using System;
using DataAccess.EFCore;
using DataAccess.EFCore.Init;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

namespace rpiDaemon;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args)
            .UseSystemd()
            .Build();
        Console.WriteLine(
            $"Starting up! Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
        CreateDbIfNotExists(host);

        host.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureAppConfiguration(builder =>
                {
                    builder.AddUserSecrets<Startup>();
                    IConfiguration config = builder.Build();
                    var appConfigConnectionString = config[GlobalConstants.AppConfig];
                    builder.AddAzureAppConfiguration(appConfigConnectionString);
                });
                webBuilder.UseStartup<Startup>();
            });

    }

    private static void CreateDbIfNotExists(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Creating db if not exists...");
            try
            {
                var context = services.GetRequiredService<BudorDbContext>();
                DbInitializer.Initialize(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }
    }
}