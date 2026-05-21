using System;
using System.Runtime.InteropServices;
using AudioService.Interfaces;
using BirdSpeciesNameTranslator;
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
        services.AddApplicationInsightsTelemetryWorkerService(options =>
        {
            options.ConnectionString =
                Configuration[GlobalConstants.AppInsightsConnectionString];
            options.EnableAdaptiveSampling = false;
        });
        services.AddHttpClient();        
        InitializeContainer();

        services.AddQuartz(q =>
        {
            q.UseJobFactory<JobFactory>();
            // q.AddJobAndTrigger<IndexImageUploadRepoJob>(GlobalConstants.DailyJobs,
            //     GlobalConstants.IndexImageUploadDbTrigger, TimeSpan.FromHours(24));
            // q.AddJobAndTrigger<GetBme280SensorReadingsJob>(GlobalConstants.SecondJobs,
            //     GlobalConstants.Bme280Trigger,
            //     _environment.IsDevelopment()
            //         ? TimeSpan.FromSeconds(2)
            //         : GetBme280SensorReadingsJob.ActiveStateTriggerInterval);
            q.AddJobAndTrigger<TakePictureJob>(GlobalConstants.SecondJobs, GlobalConstants.PictureTrigger,
                _environment.IsDevelopment() ? TimeSpan.FromSeconds(10) : TimeSpan.FromHours(1),
                DateTime.UtcNow.AddSeconds(10));
            q.AddJobAndTrigger<UploadImagesJob>(GlobalConstants.SecondJobs, GlobalConstants.UploadImagesTrigger,
                TimeSpan.FromMinutes(2), DateTime.UtcNow.AddSeconds(5));
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
            q.AddJobAndTrigger<AnalyzeAudioRecordingsJob>(GlobalConstants.SecondJobs,
                GlobalConstants.AudioAnalyzerTrigger,
                _environment.IsDevelopment() ? TimeSpan.FromSeconds(15) : TimeSpan.FromSeconds(60),
                _environment.IsDevelopment() ? DateTime.UtcNow.AddSeconds(15) : DateTime.UtcNow.AddSeconds(60));
            q.AddJobAndTrigger<UploadAudioRecordingsJob>(GlobalConstants.MinuteJobs,
                GlobalConstants.UploadAudioRecordingTrigger, TimeSpan.FromMinutes(1),
                DateTime.UtcNow.AddSeconds(35));
            q.AddJobAndTrigger<UploadVideoRecordingsJob>(GlobalConstants.MinuteJobs,
                "uploadVideoRecordingsTrigger", TimeSpan.FromMinutes(2),
                DateTime.UtcNow.AddSeconds(40));

            // Motion detection job scheduled daily at 06:00 local time
            var nowLocal = DateTime.Now;
            var next = new DateTime(nowLocal.Year, nowLocal.Month, nowLocal.Day, 6, 0, 0);
            if (nowLocal > next) next = next.AddDays(1);
            var nextUtc = next.ToUniversalTime();
            q.AddJobAndTrigger<MotionDetectionJob>(GlobalConstants.DailyJobs, GlobalConstants.MotionDetectionTrigger, TimeSpan.FromDays(1), nextUtc);
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        services.AddSignalR();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSeq());

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
        _container.RegisterSingleton<IExternalSingletonProcess, ExternalSingletonProcess>();
        _container.RegisterSingleton<IBirdSpeciesNameTranslator, SpeciesNameTranslator>();
        _container.Register<IAudioService, AudioService.AudioService>();
        _container.Register<IRepositories, Repositories>(Lifestyle.Scoped);

        _container.Register<IBirdRecordingAnalyzer, AudioRecordingRecordingAnalyzer>();
        _container.Register<CaptureAudioContinuouslyJob>();
        _container.Register<GetBme280SensorReadingsJob>();
        _container.Register<GetProximityJob>();
        _container.Register<TakeVideoJob>();
        _container.Register<StreamVideoJob>();
        _container.Register<TakePictureJob>();
        _container.Register<IAudioUploader, AudioUploaderService>();
        _container.Register<UploadAudioRecordingsJob>();
        _container.Register<UploadImagesJob>();
        _container.Register<UploadVideoRecordingsJob>();
        _container.Register<AnalyzeAudioRecordingsJob>();
        _container.Register<StartVideoSurveillanceJob>();
        _container.Register<MotionDetectionJob>();
        _container.Register<IndexImageUploadRepoJob>();
        _container.Register<IBirdNetResultConverter, BirdNetResultConverter>();
        _container.Register<IFileSystemService, FileSystemService>();
        _container.Register<IAzureStorageService, AzureStorageService>();
        _container.Register<IExternalProcess, ExternalProcess>();
        _container.Register<IPictureService, PictureService>();
        _container.Register<IPictureEditService, PictureEditService>();
        _container.Register<IQuartzNetService, QuartzNetService>();
        _container.Register<IRpiDaemonSettingsService, RpiDaemonSettingsService>();
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