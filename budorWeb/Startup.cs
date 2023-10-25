using DataAccess.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;
using Shared.SignalR;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace budorWeb
{
    public class Startup
    {
        private readonly Container _container;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
            Configuration = configuration;
            _container = new Container();
            _container.Options.ResolveUnregisteredConcreteTypes = false;
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSimpleInjector(_container, options =>
            {
                options.AddAspNetCore()
                    .AddPageModelActivation();
                options.AddLogging();
            });
            InitializeContainer();

            services.AddSignalR();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSeq());
            var dbConnectionString = _hostingEnvironment.IsProduction()
                ? Configuration[GlobalConstants.ProductionDb]
                : Configuration[GlobalConstants.DevelopmentDb];
            services.AddDbContext<BudorDbContext>(options =>
            {
                options.UseSqlServer(dbConnectionString);
                options.EnableSensitiveDataLogging();
            });
        }

        private void InitializeContainer()
        {
            _container.RegisterSingleton<ISignalRService, SignalRService>();
            _container.Register<IAzureStorageService, AzureStorageService>();
            _container.Register<IMigrationService, MigrationService>();
            _container.Register<IImageConverter, ImageConverter>();
            _container.Register<IRepositories, Repositories>(Lifestyle.Scoped);
            _container.Register<ISpeciesNameTranslator, SpeciesNameTranslator>();
            _container.Register<IRpiDaemonSettingsService, RpiDaemonSettingsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<BudorHub>(GlobalConstants.HubEndpoint);
            });
            app.UseSimpleInjector(_container);
            _container.Verify();
        }
    }
}