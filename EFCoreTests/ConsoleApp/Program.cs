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
using Havit.EFCoreTests.DataLayer.Repositories.Localizations;
using Havit.EFCoreTests.Entity;
using Havit.EFCoreTests.Model.Localizations;
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
using Havit.EFCoreTests.Model.Security;
using Microsoft.Extensions.Logging;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using System.Transactions;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.EFCoreTests.DataLayer.Seeds.Core;
using Havit.EFCoreTests.DataLayer.Repositories.Security;
using System.Data.SqlClient;

namespace ConsoleApp1
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var container = ConfigureAndCreateWindsorContainer();
			//UpdateDatabase(container);
			//GenerateLanguages(1, container);
			//GenerateSecurity(100, 10, 3, container);
			//DebugModelInfo(container);
			DebugDataLoader(container);
			//DebugFlagClass(container);
			//DebugTransactions(container);
			//DebugSeeding(container);
			//DebugCaching(container)
			//DebugOwnedTypes(container);
		}

		private static IWindsorContainer ConfigureAndCreateWindsorContainer()
		{
			var loggerFactory = new LoggerFactory();
			//loggerFactory.AddConsole((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name));
			//loggerFactory.AddConsole((categoryName, logLevel) => (categoryName == DbLoggerCategory.Database.Transaction.Name));

			DbContextOptions options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreTests;Application Name=EFCoreTests-Entity")
				//.UseInMemoryDatabase("ConsoleApp")
				.UseLoggerFactory(loggerFactory)
				.Options;

			IWindsorContainer container = new WindsorContainer();

			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
			container.AddFacility<TypedFactoryFacility>();
			container.Register(Component.For(typeof(IServiceFactory<>)).AsFactory());

			container.WithEntityPatternsInstaller(new ComponentRegistrationOptions { GeneralLifestyle = lf => lf.Scoped() })
				.RegisterDataLayer(typeof(ILanguageRepository).Assembly)
				.RegisterDbContext<Havit.EFCoreTests.Entity.ApplicationDbContext>(options)
				.RegisterEntityPatterns();

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
				//dbContext.Database.EnsureDeleted();
				dbContext.Database.Migrate();
			}
		}

		private static void GenerateLanguages(int targetCount, IWindsorContainer container)
		{
			using (var scope = container.BeginScope())
			{
				var languageRepository = container.Resolve<ILanguageRepository>();
				var unitOfWork = container.Resolve<IUnitOfWork>();

				var currentLanguages = languageRepository.GetAll();
				for (int i = currentLanguages.Count; i < targetCount; i++)
				{
					unitOfWork.AddRangeForInsert<Language>(new Language[] { new Language() });
				}

				if (targetCount > currentLanguages.Count)
				{
					unitOfWork.AddRangeForDelete(currentLanguages.Take(currentLanguages.Count - targetCount).ToArray());
				}
				unitOfWork.Commit();
			}
		}

		private static void GenerateSecurity(int loginAccountsCount, int rolesCount, int membershipsPerLoginAccountCount, IWindsorContainer container)
		{
			using (var scope = container.BeginScope())
			{
				var dbContext = container.Resolve<IDbContext>();
				dbContext.Database.Migrate();

				var unitOfWork = container.Resolve<IUnitOfWork>();

				List<LoginAccount> loginAccounts = Enumerable.Range(0, loginAccountsCount).Select(i => new LoginAccount { Username = Guid.NewGuid().ToString() }).ToList();
				List<Role> roles = Enumerable.Range(0, rolesCount).Select(i => new Role { Id = i + 1, Name = Guid.NewGuid().ToString() }).ToList();

				foreach (LoginAccount loginAccount in loginAccounts)
				{
					loginAccount.Memberships.AddRange(roles
						.OrderBy(item => Guid.NewGuid()) // náhodná role
						.Take(membershipsPerLoginAccountCount)
						.Select(role => new Membership
						{
							LoginAccount = loginAccount,
							Role = role
						})
						.ToList());
				}

				unitOfWork.AddRangeForInsert(roles);
				unitOfWork.AddRangeForInsert(loginAccounts);
				unitOfWork.Commit();
			}
		}


		private static void DebugDataLoader(IWindsorContainer container)
		{
			using (var scope = container.BeginScope())
			{
				var dbContext = container.Resolve<IDbContext>();
				var loginAccounts = dbContext.Set<LoginAccount>().AsQueryable().Take(3).ToList();
				var dataLoader = container.Resolve<IDataLoader>();

				dataLoader.LoadAll(loginAccounts, m => m.Memberships).ThenLoad(m => m.Role);
				dataLoader.LoadAll(loginAccounts, m => m.Memberships).ThenLoad(m => m.Role);
				dataLoader.LoadAll(loginAccounts, m => m.Memberships).ThenLoad(m => m.Role);
			}

		}

		private static void DebugModelInfo(IWindsorContainer container)
		{
			using (var scope = container.BeginScope())
			{

			}
		}

		private static void DebugFlagClass(IWindsorContainer container)
		{
			using (var scope = container.BeginScope())
			{
				var dbContext = container.Resolve<IDbContext>();
				dbContext.Database.Migrate();
				FlagClass flagClass = new FlagClass();
				flagClass.MyFlag = false;
				dbContext.Set<FlagClass>().AddRange(new FlagClass[] { flagClass });
				dbContext.SaveChanges();
			}
		}

		private static void DebugTransactions(IWindsorContainer container)
		{
			Action<IDbContext> action = (IDbContext dbContext) =>
			{
				dbContext.Set<FlagClass>().AsQueryable().ToList();
			};

			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required))
			{
				using (var scope = container.BeginScope())
				{
					var dbContext = container.Resolve<IDbContext>();
					action(dbContext);
				}

				using (var scope = container.BeginScope())
				{
					var dbContext = container.Resolve<IDbContext>();
					action(dbContext);
				}

				transactionScope.Complete();
			}
			System.Threading.Thread.Sleep(1000);
		}

		private static void DebugSeeding(IWindsorContainer container)
		{
			using (var scope = container.BeginScope())
			{
				var dataSeedRunner = container.Resolve<IDataSeedRunner>();
				dataSeedRunner.SeedData<CoreProfile>();
			}
		}

		private static void DebugCaching(IWindsorContainer container)
		{
			//DebugSeeding(container); // seed role
			// do cache

			//using (var scope = container.BeginScope())
			//{
			//	var dbContext = container.Resolve<IDbContext>();
			//	var role = dbContext.Set<Role>().Find(1);
			//	Console.WriteLine(dbContext.GetEntry(role, suppressDetectChanges: true).State);
			//}

			// do cache
			using (var scope = container.BeginScope())
			{
				var roleRepository = container.Resolve<IRoleRepository>();
				roleRepository.GetObject(1);
			}

			// z cache
			using (var scope = container.BeginScope())
			{
				var roleRepository = container.Resolve<IRoleRepository>();
				Role role = roleRepository.GetObject(1);

				// invalidace
				role.Name += "0";

				var unitOfWork = container.Resolve<IUnitOfWork>();
				unitOfWork.Commit();
			}

			// do cache
			using (var scope = container.BeginScope())
			{
				var roleRepository = container.Resolve<IRoleRepository>();
				roleRepository.GetObject(1);
			}
		}

		private static void DebugOwnedTypes(IWindsorContainer container)
		{
			using (var scope = container.BeginScope())
			{
				Subject subject = new Subject { Name = "Name", HomeAddress = new Address { City = "City", Street = "Street", ZipCode = "Zip" } };
				var unitOfWork = container.Resolve<IUnitOfWork>();
				unitOfWork.AddForInsert(subject);
				unitOfWork.Commit();
				subject.HomeAddress.City = "New City";
				unitOfWork.Commit();
			}
		}

	}
}