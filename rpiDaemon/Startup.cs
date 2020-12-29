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
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                var jobKey = new JobKey(nameof(TakePictureJob), "secondJobs");
                q.AddJob<TakePictureJob>(j => j.WithIdentity(jobKey));
                q.AddTrigger(t => t
                    .WithIdentity("secondsTrigger")
                    .ForJob(jobKey)
                    .StartNow()
                    .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(2)).RepeatForever()));

                q.UseMicrosoftDependencyInjectionScopedJobFactory();
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            services.AddDbContext<SensorContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
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