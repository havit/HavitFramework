using Havit.Services.Caching;
using Havit.Services.SignalR.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Havit.Services.SignalR.ConsoleApp;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var host = Host.CreateApplicationBuilder(args);

		host.Logging.AddSimpleConsole();
		host.Logging.SetMinimumLevel(LogLevel.Debug);

		host.Services.AddMemoryCache();

		host.Services.AddDistributedCacheInvalidation(o => o.HubUrl = "https://localhost:44304/DistributedCacheInvalidation");
		host.Services.AddSingleton(new MemoryCacheServiceOptions { UseCacheDependenciesSupport = true });

		var app = host.Build();
		app.Start();

		var cacheService = app.Services.GetRequiredService<ICacheService>();
		for (int i = 0; i < 10_000; i++)
		{
			cacheService.Remove("CacheKey_" + i);
			await Task.Delay(1);
		}

		cacheService.Clear();

		await app.StopAsync();

	}
}
