using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
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
using Havit.Data.Patterns.Transactions.Internal;
using Havit.EFCoreTests.DataLayer.Lookups;
using Havit.EFCoreTests.DataLayer.DataSources;
using Havit.EFCoreTests.DataLayer.Seeds.ProtectedProperties;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			IServiceProvider serviceProvider = CreateServiceProvider();
			UpdateDatabase(serviceProvider);
			//Debug(serviceProvider);
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
				.AddEntityPatterns()
				.AddLookupService<IUserLookupService, UserLookupService>();

			services.AddSingleton<ITimeService, ServerTimeService>();
			services.AddSingleton<ICacheService, MemoryCacheService>();
			services.AddSingleton<IOptions<MemoryCacheOptions>, OptionsManager<MemoryCacheOptions>>();
			services.AddSingleton(typeof(IOptionsFactory<MemoryCacheOptions>), new OptionsFactory<MemoryCacheOptions>(Enumerable.Empty<IConfigureOptions<MemoryCacheOptions>>(), Enumerable.Empty<IPostConfigureOptions<MemoryCacheOptions>>()));
			services.AddSingleton<IMemoryCache, MemoryCache>();			


			return services.BuildServiceProvider();
		}

		private static void UpdateDatabase(IServiceProvider serviceProvider)
		{

			using (var scope0 = serviceProvider.CreateScope())
			{
				scope0.ServiceProvider.GetRequiredService<IDbContext>().Database.EnsureDeleted();
				scope0.ServiceProvider.GetRequiredService<IDbContext>().Database.Migrate();
			}

			for (int i = 0; i < 100; i++)
			{
				Console.WriteLine(i);
				Parallel.For(0, 100, _ =>
				{
					using (var scope1 = serviceProvider.CreateScope())
					{
						//scope1.ServiceProvider.GetRequiredService<IDbContext>().Database.Migrate();
						scope1.ServiceProvider.GetRequiredService<IDataSeedRunner>().SeedData<DefaultProfile>();
					}
				});
			}
		}

		private static void Debug(IServiceProvider serviceProvider)
		{
			//var dataSeedRunner = serviceProvider.GetRequiredService<IDataSeedRunner>();
			//dataSeedRunner.SeedData<ProtectedPropertiesProfile>();

			using var scope1 = serviceProvider.CreateScope();
			var dbContext1 = scope1.ServiceProvider.GetRequiredService<IDbContext>();
			var uow1 = scope1.ServiceProvider.GetRequiredService<IUnitOfWork>();

			using var scope2 = serviceProvider.CreateScope();
			var dbContext2 = scope2.ServiceProvider.GetRequiredService<IDbContext>();
			var uow2 = scope2.ServiceProvider.GetRequiredService<IUnitOfWork>();

			var entity1 = dbContext1.Set<CheckedEntity>().AsQueryable().Where(item => item.Id == 1).SingleOrDefault();			
			if (entity1 == null)
			{
				entity1 = new CheckedEntity { Value = "", Version = 0 };
				dbContext1.Set<CheckedEntity>().Add(entity1);
				dbContext1.SaveChanges();
			}
			
			var entity2 = dbContext2.Set<CheckedEntity>().AsQueryable().Where(item => item.Id == 1).Single();

			entity1.Value = Guid.NewGuid().ToString("N");
			entity1.Version += 1;
			entity1.Address = new Address();
			uow1.AddForUpdate(entity1);

			entity2.Value = Guid.NewGuid().ToString("N");
			entity2.Version += 1;
			entity2.Address = new Address();
			uow2.AddForUpdate(entity2);

			uow1.Commit();
			try
			{
				uow2.Commit();
			}
			catch (DbUpdateException e) when (e.InnerException is DbUpdateConcurrencyException ce)
			{
				Console.WriteLine("catch");
				foreach (var entry in ce.Entries)
				{
					if (entry.Entity is CheckedEntity checkedEntity)
					{
						entry.CurrentValues.SetValues(entry.OriginalValues);
						entry.State = EntityState.Unchanged;
					}
				}
				uow2.Commit();				
			}
		}

	}
}
