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

namespace ConsoleApp1
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			IServiceProvider serviceProvider = CreateServiceProvider();
			UpdateDatabase(serviceProvider);
			Debug(serviceProvider);
		}

		private static IServiceProvider CreateServiceProvider()
		{
			//var loggerFactory = LoggerFactory.Create(builder => builder
			//.AddFilter((categoryName, logLevel) => (((logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name)) 
			//	|| (/*(logLevel == LogLevel.Info) &&*/ (categoryName == DbLoggerCategory.Database.Transaction.Name))))
			//.AddSimpleConsole());

			IServiceCollection services = new ServiceCollection();
			services.WithEntityPatternsInstaller()
				.AddDataLayer(typeof(IPersonRepository).Assembly)
				.AddDbContext<Havit.EFCoreTests.Entity.ApplicationDbContext>(optionsBuilder =>
					optionsBuilder
						.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreTests;Application Name=EFCoreTests-Entity;ConnectRetryCount=0")
						//.UseInMemoryDatabase("ConsoleApp")
						//.UseLoggerFactory(loggerFactory))
				)
				.AddEntityPatterns()
				.AddLookupService<IUserLookupService, UserLookupService>();

			services.AddSingleton<ITimeService, ServerTimeService>();
			services.AddSingleton<ICacheService, MemoryCacheService>();
			services.AddSingleton<IOptions<MemoryCacheOptions>, OptionsManager<MemoryCacheOptions>>();
			services.AddSingleton(typeof(IOptionsFactory<MemoryCacheOptions>), new OptionsFactory<MemoryCacheOptions>(Enumerable.Empty<IConfigureOptions<MemoryCacheOptions>>(), Enumerable.Empty<IPostConfigureOptions<MemoryCacheOptions>>()));
			services.AddSingleton<IMemoryCache, MemoryCache>();			

			return services.BuildServiceProvider(new ServiceProviderOptions
            {
				ValidateOnBuild = true,
				ValidateScopes = true
            });
		}

		private static void UpdateDatabase(IServiceProvider serviceProvider)
		{
			using (var scope = serviceProvider.CreateScope())
			{
				scope.ServiceProvider.GetRequiredService<IDbContext>().Database.Migrate();
			}
		}

		private static void Debug(IServiceProvider serviceProvider)
		{
			for (int i = 0; i < 10000; i++)
			{
				using var scope = serviceProvider.CreateScope();
				var dataSeedRunner = scope.ServiceProvider.GetRequiredService<IDbContext>();				
			}
		}
	}
}
