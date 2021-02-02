using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using rpiDaemon.Jobs;

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

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                var pictureJobKey = new JobKey(nameof(TakePictureJob), "secondJobs");
                var bme280JobKey = new JobKey(nameof(GetBme280SensorReadingsJob), "secondJobs");
                //var signalRTestKey = new JobKey(nameof(TestSignalRJob), "secondJobs");

                q.AddJob<TakePictureJob>(j => j.WithIdentity(pictureJobKey));
                q.AddJob<GetBme280SensorReadingsJob>(j => j.WithIdentity(bme280JobKey));
                //q.AddJob<TestSignalRJob>(j => j.WithIdentity(signalRTestKey));

                q.AddTrigger(t => t
                    .WithIdentity("pictureTrigger")
                    .ForJob(pictureJobKey)
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(5)).RepeatForever()));
                q.AddTrigger(t => t.WithIdentity("bme280Trigger")
                    .ForJob(bme280JobKey)
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(5)).RepeatForever()));
                //q.AddTrigger(t => t.WithIdentity("signalRTest")
                //    .ForJob(signalRTestKey)
                //    .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
                //    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(5)).RepeatForever()));

                q.UseMicrosoftDependencyInjectionScopedJobFactory();
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = false);

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}