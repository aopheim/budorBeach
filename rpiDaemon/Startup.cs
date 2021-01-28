using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using rpiDaemon.Jobs;

namespace rpiDaemon
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration config, IWebHostEnvironment environment, ILogger<Startup> logger)
        {
            _environment = environment;
            _logger = logger;
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogDebug("Starting configureServices");
            services.AddQuartz(q =>
            {
                var pictureJobKey = new JobKey(nameof(TakePictureJob), "secondJobs");
                var bme280JobKey = new JobKey(nameof(GetBme280SensorReadingsJob), "secondJobs");

                q.AddJob<TakePictureJob>(j => j.WithIdentity(pictureJobKey));
                q.AddJob<GetBme280SensorReadingsJob>(j => j.WithIdentity(bme280JobKey));

                q.AddTrigger(t => t
                    .WithIdentity("pictureTrigger")
                    .ForJob(pictureJobKey)
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromMinutes(1)).RepeatForever()));
                q.AddTrigger(t => t.WithIdentity("bme280Trigger")
                    .ForJob(bme280JobKey)
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(5)).RepeatForever()));

                q.UseMicrosoftDependencyInjectionScopedJobFactory();
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = false);
            _logger.LogDebug("Quartz service configured");

            if (_environment.IsProduction())
            {
                _logger.LogDebug("Setting up production db");
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("ProductionDb")));
            }
            else
            {
                _logger.LogDebug("Setting ut development db");
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DevelopmentDb")));
            }

            _logger.LogDebug("Finished configuring services");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}