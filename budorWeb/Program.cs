using System;
using budorWeb;
using DataAccess.EFCore;
using DataAccess.EFCore.Init;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Shared;

var builder = WebApplication.CreateBuilder(args);
var azureAppConfigConnectionString = builder.Configuration[GlobalConstants.AppConfig];
builder.Configuration.AddAzureAppConfiguration(azureAppConfigConnectionString);
if (builder.Environment.IsProduction())
{
    builder.Logging.AddApplicationInsights(
        config =>
        {
            config.ConnectionString = builder.Configuration[GlobalConstants.AppInsightsConnectionString];
            config.DisableTelemetry = false;
        },
        options => { }
    );
    builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Default", LogLevel.Information);
}

var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
if (builder.Environment.IsDevelopment())
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        var dataBaseContext = serviceProvider.GetRequiredService<BudorDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Initializing db for budorWeb if it not exists...");
        try
        {
            DbInitializer.Initialize(dataBaseContext, logger);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured when initializing database");
        }
    }

startup.Configure(app, app.Environment);

app.Run();