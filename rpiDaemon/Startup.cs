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

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                var pictureJobKey = new JobKey(nameof(TakePictureJob), "secondJobs");
                var temperatureJobKey = new JobKey(nameof(GetCurrentTemperatureJob), "secondJobs");

                q.AddJob<TakePictureJob>(j => j.WithIdentity(pictureJobKey));
                q.AddJob<GetCurrentTemperatureJob>(j => j.WithIdentity(temperatureJobKey));

                q.AddTrigger(t => t
                    .WithIdentity("pictureTrigger")
                    .ForJob(pictureJobKey)
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever()));
                q.AddTrigger(t => t
                    .WithIdentity("sensorTrigger")
                    .ForJob(temperatureJobKey)
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(3)).RepeatForever()));

                q.UseMicrosoftDependencyInjectionScopedJobFactory();
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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