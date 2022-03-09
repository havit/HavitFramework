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

namespace ConsoleApp1
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var host = Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration(configurationBuilder =>
					configurationBuilder.AddJsonFile("appsettings.ConsoleApp.json", optional: false)
				)
				.ConfigureLogging((hostingContext, logging) => logging
				.AddSimpleConsole(config => config.TimestampFormat = "[hh:MM:ss.ffff] ")
				.AddFile("%TEMP%\\dbdataloader_{0:yyyy}-{0:MM}-{0:dd}.log", fileLoggerOpts =>
				{
					fileLoggerOpts.FormatLogFileName = fName => String.Format(fName, DateTime.UtcNow);
				}))
				.ConfigureServices((hostingContext, services) => ConfigureServices(hostingContext, services))
				.Build();
					
			UpdateDatabase(host.Services);
			Debug(host.Services);
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
				//scope.ServiceProvider.GetRequiredService<IDataSeedRunner>().SeedData<PersonsProfile>();
			}
		}

		private static void Debug(IServiceProvider serviceProvider)
		{
			for (int i = 0; i < 2; i++)
			{
				using (var scope = serviceProvider.CreateScope())
				{
					var dataLoader = scope.ServiceProvider.GetRequiredService<IDataLoader>();
					var dataSource = scope.ServiceProvider.GetRequiredService<IPersonDataSource>();
					var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
					var persons = dataSource.Data.Where(p => p.BossId == null).ToList();

					dbContext.ChangeTracker.DetectChanges();

					Thread.Sleep(3000);
					//dataLoader.LoadAll(persons, p => p.Subordinates).ThenLoad(p => p.Subordinates);
				}
			}
		}
	}
}
