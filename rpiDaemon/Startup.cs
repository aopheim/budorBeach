using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using rpiDaemon.Jobs;
using Shared;

namespace rpiDaemon
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration config, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = config;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
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
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(60 * 60)).RepeatForever()));
                q.AddTrigger(t => t.WithIdentity("bme280Trigger")
                    .ForJob(bme280JobKey)
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(20)).RepeatForever()));

                q.UseMicrosoftDependencyInjectionScopedJobFactory();
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = false);

            services.AddSignalR();
            services.AddHostedService<BudorHubPiClient>();

            if (_environment.IsProduction())
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("ProductionDb")));
            else
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DevelopmentDb")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapHub<BudorHub>("/budorhub"); });
        }
    }
}