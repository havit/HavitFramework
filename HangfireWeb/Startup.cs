using Hangfire;
using Hangfire.Dashboard;
using Havit.HangfireWeb.Infrastructure.ConfigurationExtensions;

namespace HangfireWeb;

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
		//services.AddAuthorizationBuilder();

		services.AddRazorPages();

		// ExceptionMonitoring
		services.AddExceptionMonitoring(Configuration);

		// Hangfire
		services.AddCustomizedHangfire(Configuration);
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void ConfigureMiddleware(WebApplication app)
	{
		if (app.Environment.IsDevelopment())
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
	}

	public void ConfigureEndpoints(WebApplication app)
	{
		app.MapRazorPages();

		app.MapHangfireDashboard("/hangfire", new DashboardOptions
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

		app.Map("/custom-error", _ => throw new System.ApplicationException("My application exception"));
	}
}
