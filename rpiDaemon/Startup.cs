using System;
using CameraService.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using rpiDaemon.Jobs;
using Shared;
using SimpleInjector;

namespace rpiDaemon
{
    public class Startup
    {
        private readonly Container _container = new Container();
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration config, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = config;
            _container.Options.ResolveUnregisteredConcreteTypes = false;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSimpleInjector(_container, options =>
            {
                options.AddAspNetCore();
                options.AddLogging();
                //options.AddHostedService<BudorHubPiClient>();
            });
            InitializeContainer();

            services.AddQuartz(q =>
            {
                var pictureJobKey = new JobKey(nameof(TakePictureJob), "secondJobs");
                var bme280JobKey = new JobKey(nameof(GetBme280SensorReadingsJob), "secondJobs");

                q.AddJob<TakePictureJob>(j => j.WithIdentity(pictureJobKey));
                q.AddJob<GetBme280SensorReadingsJob>(j => j.WithIdentity(bme280JobKey));

                q.AddTrigger(t => t
                    .WithIdentity("pictureTrigger")
                    .ForJob(pictureJobKey)
                    .StartAt(DateTime.UtcNow.AddSeconds(10))
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromHours(4)).RepeatForever()));
                q.AddTrigger(t => t.WithIdentity("bme280Trigger")
                    .ForJob(bme280JobKey)
                    .StartAt(DateTime.UtcNow.AddSeconds(10))
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever()));

                q.UseMicrosoftDependencyInjectionJobFactory();
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = false);

            services.AddSignalR();

            if (_environment.IsProduction())
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["ProductionDb"]));
            else
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["DevelopmentDb"]));
        }

        private void InitializeContainer()
        {
            _container.RegisterSingleton<ICameraService, CameraService.CameraService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseSimpleInjector(_container);
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapHub<BudorHub>(GlobalConstants.HubEndpoint); });
            _container.Verify();
        }
    }
}