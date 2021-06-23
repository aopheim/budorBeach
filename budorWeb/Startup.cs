using System;
using Azure.Core.Extensions;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rpiDaemon;
using Shared;

namespace budorWeb
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSignalR();

            var connectionString = _hostingEnvironment.IsProduction()
                ? Configuration.GetConnectionString("ProductionDb")
                : Configuration.GetConnectionString("DevelopmentDb");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging();
            });

            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["ConnectionStrings:AzureStorageConnectionString:blob"],
                    true);
                builder.AddQueueServiceClient(Configuration["ConnectionStrings:AzureStorageConnectionString:queue"],
                    true);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //app.UseHsts();

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<BudorHub>(GlobalConstants.HubEndpoint);
            });
        }
    }

    internal static class StartupExtensions
    {
        public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(
            this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out var serviceUri))
                return builder.AddBlobServiceClient(serviceUri);
            return builder.AddBlobServiceClient(serviceUriOrConnectionString);
        }

        public static IAzureClientBuilder<QueueServiceClient, QueueClientOptions> AddQueueServiceClient(
            this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out var serviceUri))
                return builder.AddQueueServiceClient(serviceUri);
            return builder.AddQueueServiceClient(serviceUriOrConnectionString);
        }
    }
}