namespace HangfireWeb;

public class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		ConfigureConfigurationAndLogging(builder);
		ConfigureServices(builder);

		var app = builder.Build();

		ConfigureMiddleware(app);
		ConfigureEndpoints(app);

		app.Run();
	}

	private static void ConfigureConfigurationAndLogging(WebApplicationBuilder builder)
	{
		builder.Configuration.AddJsonFile("appsettings.HangfireWeb.json", optional: false, reloadOnChange: false);
		builder.Configuration.AddJsonFile($"appsettings.HangfireWeb.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: false);
		builder.Configuration.AddEnvironmentVariables();
	}

	private static void ConfigureServices(WebApplicationBuilder builder)
	{
		Startup startup = new Startup(builder.Configuration);
		startup.ConfigureServices(builder.Services);
	}

	private static void ConfigureMiddleware(WebApplication app)
	{
		Startup startup = new Startup(app.Configuration);
		startup.ConfigureMiddleware(app);
	}

	private static void ConfigureEndpoints(WebApplication app)
	{
		Startup startup = new Startup(app.Configuration);
		startup.ConfigureEndpoints(app);
	}
}
