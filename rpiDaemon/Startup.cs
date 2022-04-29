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
using rpiDaemon.Jobs.JobFactories;
using Services;
using Shared;
using Shared.SignalR;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace rpiDaemon
{
    public class Startup
    {
        private readonly Container _container;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration config, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = config;
            _container = new Container();
            _container.Options.ResolveUnregisteredConcreteTypes = false;
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSimpleInjector(_container, options =>
            {
                options.AddAspNetCore();
                options.AddLogging();
                options.AddHostedService<BudorHubPiClient>();
            });
            InitializeContainer();

            services.AddQuartz(q =>
            {
                q.UseJobFactory<JobFactory>();
                q.AddJobAndTrigger<GetBme280SensorReadingsJob>(GlobalConstants.SecondJobs,
                    GlobalConstants.Bme280Trigger,
                    _environment.IsDevelopment() ? TimeSpan.FromSeconds(2) : TimeSpan.FromSeconds(10));
                q.AddJobAndTrigger<TakePictureJob>(GlobalConstants.SecondJobs, GlobalConstants.PictureTrigger,
                    _environment.IsDevelopment() ? TimeSpan.FromSeconds(10) : TimeSpan.FromHours(4));
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            services.AddSignalR();
            if (_environment.IsProduction())
                services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsightsConnectionString"]);

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
            _container.RegisterSingleton<ISignalRService, SignalRService>();
            _container.Register<GetBme280SensorReadingsJob>();
            _container.Register<TakePictureJob>();
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