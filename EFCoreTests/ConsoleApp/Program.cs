using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.EFCoreTests.Entity;
using Havit.Services;
using Havit.Services.Caching;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Havit.Data.Patterns.DataLoaders;
using Havit.EFCoreTests.Model;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using System.Transactions;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.EFCoreTests.DataLayer.Seeds.Core;
using System.Data.SqlClient;
using Havit.Data.Patterns.Exceptions;
using Havit.EFCoreTests.DataLayer.Repositories;
using System.Linq.Expressions;
using Havit.EFCoreTests.DataLayer.Lookups;
using Havit.EFCoreTests.DataLayer.DataSources;
using Havit.EFCoreTests.DataLayer.Seeds.ProtectedProperties;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Havit.EFCoreTests.DataLayer.Seeds.Persons;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ConsoleApp1;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var host = Host.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration(configurationBuilder =>
				configurationBuilder.AddJsonFile("appsettings.ConsoleApp.json", optional: false)
			)
			.ConfigureLogging((hostingContext, logging) => logging
			.AddSimpleConsole(config => config.TimestampFormat = "[hh:MM:ss.ffff] "))
			.ConfigureServices((hostingContext, services) => ConfigureServices(hostingContext, services))
			.Build();

		UpdateDatabase(host.Services);
		await DebugAsync(host.Services);
	}

	private static void ConfigureServices(HostBuilderContext hostingContext, IServiceCollection services)
	{
		services.WithEntityPatternsInstaller()
			.AddDataLayer(typeof(IPersonRepository).Assembly)
			.AddDbContext<Havit.EFCoreTests.Entity.ApplicationDbContext>(optionsBuilder =>
				optionsBuilder
					.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreTests;Application Name=EFCoreTests-Entity;ConnectRetryCount=0")
					.EnableSensitiveDataLogging(true))
			//.UseInMemoryDatabase("ConsoleApp")
			.AddEntityPatterns()
			.AddLookupService<IUserLookupService, UserLookupService>();

		services.AddSingleton<ITimeService, ServerTimeService>();
		services.AddSingleton<ICacheService, MemoryCacheService>();
		services.AddSingleton<IOptions<MemoryCacheOptions>, OptionsManager<MemoryCacheOptions>>();
		services.AddSingleton(typeof(IOptionsFactory<MemoryCacheOptions>), new OptionsFactory<MemoryCacheOptions>(Enumerable.Empty<IConfigureOptions<MemoryCacheOptions>>(), Enumerable.Empty<IPostConfigureOptions<MemoryCacheOptions>>()));
		services.AddSingleton<IMemoryCache, MemoryCache>();
	}

	private static void UpdateDatabase(IServiceProvider serviceProvider)
	{
		using (var scope = serviceProvider.CreateScope())
		{
			//scope.ServiceProvider.GetRequiredService<IDbContext>().Database.EnsureDeleted();
			scope.ServiceProvider.GetRequiredService<IDbContext>().Database.Migrate();
			scope.ServiceProvider.GetRequiredService<IDataSeedRunner>().SeedData<PersonsProfile>();
		}
	}

	private static async Task DebugAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
	{
		Stopwatch sw = Stopwatch.StartNew();
		for (int i = 0; i < 5; i++)
		{
			using (var scope = serviceProvider.CreateScope())
			{
				var repository = scope.ServiceProvider.GetRequiredService<IPersonRepository>();
				repository.GetObject(1);
				await repository.GetObjectAsync(2, cancellationToken);
				repository.GetObjects(3, 4);
				await repository.GetObjectsAsync(new int[] { 5, 6 }, cancellationToken);
				//repository.GetAll();
				await repository.GetAllAsync(cancellationToken);
			}
		}
		Console.WriteLine(sw.ElapsedMilliseconds);
	}

}
