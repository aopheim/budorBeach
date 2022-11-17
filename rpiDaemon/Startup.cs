using System;
using System.Runtime.InteropServices;
using AudioService.Interfaces;
using CameraService.Interfaces;
using DataAccess.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using rpiDaemon.Jobs;
using rpiDaemon.Jobs.JobFactories;
using Services;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;
using Shared.SignalR;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace rpiDaemon;

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
            // q.AddJobAndTrigger<GetBme280SensorReadingsJob>(GlobalConstants.SecondJobs,
            //     GlobalConstants.Bme280Trigger,
            //     _environment.IsDevelopment() ? TimeSpan.FromSeconds(2) : TimeSpan.FromSeconds(10));
            // q.AddJobAndTrigger<TakePictureJob>(GlobalConstants.SecondJobs, GlobalConstants.PictureTrigger,
            //     _environment.IsDevelopment() ? TimeSpan.FromSeconds(10) : TimeSpan.FromHours(4),
            //     DateTime.UtcNow.AddSeconds(10));
            // q.AddJobAndTrigger<TakeVideoJob>(GlobalConstants.SecondJobs, GlobalConstants.VideoRecordingTrigger,
            //     null, DateTime.UtcNow.AddSeconds(20));
            // q.AddJobAndTrigger<StreamVideoJob>(GlobalConstants.SecondJobs, GlobalConstants.StartVideoStreamTrigger,
            //     null, DateTime.UtcNow.AddSeconds(45));
            // q.AddJobAndTrigger<GetProximityJob>(GlobalConstants.SecondJobs, GlobalConstants.ProximityTrigger,
            //     TimeSpan.FromSeconds(5));
            // q.AddJobAndTrigger<StartVideoSurveillanceJob>(GlobalConstants.SecondJobs,
            //     GlobalConstants.StartVideSurveillanceTrigger, null, DateTime.UtcNow.AddSeconds(30));
            q.AddJobAndTrigger<CaptureAudioContinuouslyJob>(GlobalConstants.SecondJobs,
                GlobalConstants.AudioRecordingTrigger,
                // Adding trigger interval in order to trigger restart if recording has stopped
                TimeSpan.FromMinutes(5));
            // q.AddJobAndTrigger<AnalyzeAudioRecordingsJob>(GlobalConstants.SecondJobs,
            //     GlobalConstants.AudioAnalyzerTrigger,
            //     _environment.IsDevelopment() ? TimeSpan.FromSeconds(15) : TimeSpan.FromSeconds(60),
            //     _environment.IsDevelopment() ? DateTime.UtcNow.AddSeconds(15) : DateTime.UtcNow.AddSeconds(60));
            // q.AddJobAndTrigger<UploadAudioRecordingsJob>(GlobalConstants.MinuteJobs,
            //     GlobalConstants.UploadAudioRecordingTrigger, TimeSpan.FromMinutes(1),
            //     DateTime.UtcNow.AddSeconds(10));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        services.AddSignalR();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSeq());
        if (_environment.IsProduction())
        {
            var instrumentationKey = Configuration[GlobalConstants.AppInsightsConnectionString];
            Console.WriteLine($"Set up application insights with instrumentation key {instrumentationKey}");
            services.AddApplicationInsightsTelemetry(instrumentationKey);
        }

        var secretKey = _environment.IsDevelopment()
            ? RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? GlobalConstants.DevelopmentDb
                : GlobalConstants.DockerDevelopmentDb
            : GlobalConstants.ProductionDb;
        Console.WriteLine($"Reading db connection string from key: {secretKey}");
        services.AddDbContext<BudorDbContext>(options => { options.UseSqlServer(Configuration[secretKey]); });
    }

    private void InitializeContainer()
    {
        _container.RegisterSingleton<ICameraService, CameraService.CameraService>();
        _container.RegisterSingleton<ISignalRService, SignalRService>();
        _container.RegisterSingleton<IProximityService, ProximityService>();
        _container.RegisterSingleton<IBirdPresenceCalculator, BirdPresenceCalculator>();
        _container.RegisterSingleton<IBirdPresenceRegistrator, BirdPresenceRegistrator>();
        _container.RegisterSingleton<IBirdNetServer, BirdNetServer>();
        _container.RegisterSingleton<IBirdRecordingAnalyzer, AudioRecordingRecordingAnalyzer>();
        _container.RegisterSingleton<IAudioUploader, AudioUploaderService>();
        _container.RegisterSingleton<IExternalSingletonProcess, ExternalSingletonProcess>();
        _container.Register<CaptureAudioContinuouslyJob>();
        _container.Register<IAudioService, AudioService.AudioService>();
        _container.Register<IRepositories, Repositories>(Lifestyle.Scoped);
        _container.Register<GetBme280SensorReadingsJob>();
        _container.Register<TakePictureJob>();
        _container.Register<GetProximityJob>();
        _container.Register<TakeVideoJob>();
        _container.Register<StreamVideoJob>();
        _container.Register<UploadAudioRecordingsJob>();
        _container.Register<AnalyzeAudioRecordingsJob>();
        _container.Register<StartVideoSurveillanceJob>();
        _container.Register<IBirdNetResultConverter, BirdNetResultConverter>();
        _container.Register<IFileSystemService, FileSystemService>();
        _container.Register<IAzureStorageService, AzureStorageService>();
        _container.Register<IExternalProcess, ExternalProcess>();
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