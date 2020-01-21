using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.EFCoreTests.Entity;
using Havit.Services;
using Havit.Services.Caching;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Castle.MicroKernel.Lifestyle;
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
			var loggerFactory = new LoggerFactory();
			//loggerFactory.AddConsole((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name));

			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreTests;Application Name=EFCoreTests-Entity;ConnectRetryCount=0")
				//.UseInMemoryDatabase("ConsoleApp")
				.UseLoggerFactory(loggerFactory)
				.Options;

			IServiceCollection services = new ServiceCollection();
			services.WithEntityPatternsInstaller()
				.AddDataLayer(typeof(IPersonRepository).Assembly)
				.AddDbContext<Havit.EFCoreTests.Entity.ApplicationDbContext>(options)
				.AddEntityPatterns();

			services.AddSingleton<ITimeService, ServerTimeService>();
			services.AddSingleton<ICacheService, MemoryCacheService>();
			services.AddSingleton<IOptions<MemoryCacheOptions>, OptionsManager<MemoryCacheOptions>>();
			services.AddSingleton(typeof(IOptionsFactory<MemoryCacheOptions>), new OptionsFactory<MemoryCacheOptions>(Enumerable.Empty<IConfigureOptions<MemoryCacheOptions>>(), Enumerable.Empty<IPostConfigureOptions<MemoryCacheOptions>>()));
			services.AddSingleton<IMemoryCache, MemoryCache>();

			return services.BuildServiceProvider();
		}

		private static void UpdateDatabase(IServiceProvider serviceProvider)
		{
			using (serviceProvider.CreateScope())
			{
				var dbContext = serviceProvider.GetRequiredService<IDbContext>();
				dbContext.Database.EnsureDeleted();
				dbContext.Database.Migrate();
			}
		}

		private static void Debug(IServiceProvider serviceProvider)
		{
			using (IServiceScope scope = serviceProvider.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
				var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
				var dataLoader = scope.ServiceProvider.GetRequiredService<IDataLoader>();

				BusinessCase businessCase = new BusinessCase();
				uow.AddForInsert(businessCase);

				Modelation modelation = new Modelation();
				uow.AddForInsert(modelation);
				modelation.BusinessCase = businessCase; // instance nastavena
				modelation.BusinessCaseId = businessCase.Id;
				businessCase.Modelations.Add(modelation);
				uow.Commit();

				Console.WriteLine(dbContext.GetEntry(modelation, true).Navigation("BusinessCase").IsLoaded); // avšak vlastnost není považována za načtenou

				IBusinessCaseRepository businessCaseRepository = scope.ServiceProvider.GetRequiredService<IBusinessCaseRepository>();
				businessCaseRepository.GetObject(1); // dostaneme objekt do cache

				dataLoader.Load(modelation, m => m.BusinessCase);
				Console.WriteLine(dbContext.GetEntry(modelation, true).Navigation("BusinessCase").IsLoaded); // avšak vlastnost není považována za načtenou
			}

		}

	}
}
