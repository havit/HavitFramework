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
			var container = ConfigureAndCreateWindsorContainer();
			UpdateDatabase(container);
            Debug(container);
		}

		private static IWindsorContainer ConfigureAndCreateWindsorContainer()
		{
			var loggerFactory = new LoggerFactory();
			//loggerFactory.AddConsole((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name));

			DbContextOptions options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreTests;Application Name=EFCoreTests-Entity;ConnectRetryCount=0")
				//.UseInMemoryDatabase("ConsoleApp")
				.UseLoggerFactory(loggerFactory)
				.Options;

			IWindsorContainer container = new WindsorContainer();

			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
			container.AddFacility<TypedFactoryFacility>();
			container.Register(Component.For(typeof(IServiceFactory<>)).AsFactory());

			container.WithEntityPatternsInstaller(c => c.GeneralLifestyle = lf => lf.Scoped() )
				.AddDataLayer(typeof(IPersonRepository).Assembly)
				.AddDbContext<Havit.EFCoreTests.Entity.ApplicationDbContext>(options)
				.AddEntityPatterns();

            container.Register(Component.For<ITimeService>().ImplementedBy<ServerTimeService>().LifestyleSingleton());
			container.Register(Component.For<ICacheService>().ImplementedBy<MemoryCacheService>().LifestyleSingleton());
            container.Register(Component.For<IOptions<MemoryCacheOptions>>().ImplementedBy<OptionsManager<MemoryCacheOptions>>().LifestyleSingleton());
			container.Register(Component.For<IOptionsFactory<MemoryCacheOptions>>().Instance(new OptionsFactory<MemoryCacheOptions>(Enumerable.Empty<IConfigureOptions<MemoryCacheOptions>>(), Enumerable.Empty<IPostConfigureOptions<MemoryCacheOptions>>())));
			container.Register(Component.For<IMemoryCache>().ImplementedBy<MemoryCache>().LifestyleSingleton());

			return container;
		}

		private static void UpdateDatabase(IWindsorContainer container)
		{
			using (var scope = container.BeginScope())
			{
				var dbContext = container.Resolve<IDbContext>();
				dbContext.Database.EnsureDeleted();
				dbContext.Database.Migrate();
			}
		}

		private static void Debug(IWindsorContainer container)
		{
			using (var scope = container.BeginScope())
			{
				var uow = container.Resolve<IUnitOfWork>();
				
				uow.AddRangeForInsert(Enumerable.Range(0, 10).Select(i => new Person()));
				uow.Commit();

				var personRepository = container.Resolve<IPersonRepository>();
				var persons = personRepository.GetAll();
				Console.WriteLine(persons.Count);
			}

			using (var scope = container.BeginScope())
			{
				var personRepository = container.Resolve<IPersonRepository>();
				var dataLoader = container.Resolve<IDataLoader>();

				List<Person> persons = personRepository.GetObjects(1, 3, 5, 7, 9);
				dataLoader.LoadAll(persons, p => p.Subordinates);
			}

		}


	}
}