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

namespace ConsoleApp1
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();
			var container = ConfigureAndCreateWindsorContainer();
			for (int i = 0; i < 10000; i++)
			{
				GetObject(sw, container, i > 0);
				if ((i > 0) && (i % 1000 == 0))
				{
					Console.WriteLine(sw.ElapsedMilliseconds / (decimal)i);
				}
			}			
			
		}

		private static IWindsorContainer ConfigureAndCreateWindsorContainer()
		{
			IWindsorContainer container = new WindsorContainer();

			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
			container.AddFacility<TypedFactoryFacility>();
			container.Register(Component.For(typeof(IServiceFactory<>)).AsFactory());

			container.WithEntityPatternsInstaller(new ComponentRegistrationOptions { GeneralLifestyle = lf => lf.Scoped() }.ConfigureCacheAllEntitiesWithDefaultSlidingExpirationCaching(TimeSpan.FromMinutes(5)))
				.RegisterDataLayer(typeof(ILanguageRepository).Assembly)
				//.RegisterDbContext<Havit.EFCoreTests.Entity.ApplicationDbContext>()
				.RegisterEntityPatterns();

			// TODO: Connection string
			container.Register(Component.For<IDbContext>()
								.ImplementedBy<ApplicationDbContext>()
								.LifestyleScoped()
								.DependsOn(Dependency.OnValue("options", new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreTests;Application Name=EFCoreTests-Entity").Options)));
			container.Register(Component.For<ITimeService>().ImplementedBy<ServerTimeService>().LifestyleSingleton());
			container.Register(Component.For<ICacheService>().ImplementedBy<MemoryCacheService>().LifestyleSingleton());
			container.Register(Component.For<IOptions<MemoryCacheOptions>>().ImplementedBy<OptionsManager<MemoryCacheOptions>>().LifestyleSingleton());			
			container.Register(Component.For<IOptionsFactory<MemoryCacheOptions>>().Instance(new OptionsFactory<MemoryCacheOptions>(Enumerable.Empty<IConfigureOptions<MemoryCacheOptions>>(), Enumerable.Empty<IPostConfigureOptions<MemoryCacheOptions>>())));
			container.Register(Component.For<IMemoryCache>().ImplementedBy<MemoryCache>().LifestyleSingleton());

			return container;
		}

		private static void GenerateLanguages(int targetCount, IWindsorContainer container)
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

		private static void GetObject(Stopwatch stopwatch, IWindsorContainer container, bool stopwatchEnabled)
		{			
			using (var scope = container.BeginScope())
			{				
				var languageRepository = container.Resolve<ILanguageRepository>();
				if (stopwatchEnabled)
				{
					stopwatch.Start();
				}
				languageRepository.GetObject(1);
				stopwatch.Stop();
			}
		}

	}
}
