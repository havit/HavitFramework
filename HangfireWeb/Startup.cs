using Hangfire;
using Hangfire.Dashboard;
using Havit.HangfireApp.Infrastructure.Security;
using Havit.HangfireWeb.Infrastructure.ConfigurationExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                //options.AddPolicy(PolicyNames.HangfireDashboardAcccessPolicy, policy => policy...);
            });

            services.AddRazorPages();

            // Hangfire
            services.AddCustomizedHangfire(Configuration);
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
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new List<IDashboardAuthorizationFilter>() { }, // see https://sahansera.dev/securing-hangfire-dashboard-with-endpoint-routing-auth-policy-aspnetcore/
                    DisplayStorageConnectionString = false,
                    DashboardTitle = "NewProjectTemplate - Jobs",
                    StatsPollingInterval = 60_000, // once a minute
                    DisplayNameFunc = (_, job) => Havit.Hangfire.Extensions.Helpers.JobNameHelper.TryGetSimpleName(job, out string simpleName)
                        ? simpleName
                        : job.ToString()
                });
                //.RequireAuthorization(PolicyNames.HangfireDashboardAcccessPolicy);

            });
        }
    }
}
