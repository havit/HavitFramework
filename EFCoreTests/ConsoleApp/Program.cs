using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.EFCoreTests.DataLayer.Repositories.Localizations;
using Havit.EFCoreTests.Entity;
using Havit.EFCoreTests.Model.Localizations;
using Havit.Services;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp1
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			IWindsorContainer container = ConfigureAndCreateWindsorContainer();
			//GenerateLanguages(10000);

			Stopwatch sw = new Stopwatch();
			for (int i = 0; i < 1000; i++)
			{
				GetObject(sw);
				if ((i > 0) && (i % 100 == 0))
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

			container.WithEntityPatternsInstaller(new ComponentRegistrationOptions { GeneralLifestyle = lf => lf.Singleton })
				.RegisterDataLayer(typeof(ILanguageRepository).Assembly)
				//.RegisterDbContext<Havit.EFCoreTests.Entity.ApplicationDbContext>()
				.RegisterEntityPatterns();

			// TODO: Connection string
			container.Register(Component.For<IDbContext>()
								.ImplementedBy<ApplicationDbContext>()
								.LifestyleSingleton()
								.DependsOn(Dependency.OnValue("options", new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreTests;Application Name=EFCoreTests-Entity").Options)));
			container.Register(Component.For<ITimeService>().ImplementedBy<ServerTimeService>().LifestyleSingleton());
			return container;
		}

		private static void GenerateLanguages(int targetCount)
		{
			var container = ConfigureAndCreateWindsorContainer();
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

		private static void GetObject(Stopwatch stopwatch)
		{
			var container = ConfigureAndCreateWindsorContainer();
			var languageRepository = container.Resolve<ILanguageRepository>();
			stopwatch.Start();
			languageRepository.GetObject(1);
			stopwatch.Start();
		}

	}
}
