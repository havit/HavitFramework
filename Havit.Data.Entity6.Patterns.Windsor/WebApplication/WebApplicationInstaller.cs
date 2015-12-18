using System;
using System.Reflection;
using Castle.Core.Internal;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Havit.Data.Entity.Patterns.DataEntries;
using Havit.Data.Entity.Patterns.DataLoaders;
using Havit.Data.Entity.Patterns.Localizations;
using Havit.Data.Entity.Patterns.QueryServices;
using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.Seeds;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Attributes;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.QueryServices;
using Havit.Data.Patterns.Repositories;
using Havit.Model.Localizations;

namespace Havit.Data.Entity.Patterns.Windsor.WebApplication
{
	// TODO: Doladit komentáře metod.

	/// <summary>
	/// Extention metody pro instalaci služeb Havit.Data.Entity a Havit.Data.Patterns.Entity do Windsor Castle Containeru.
	/// </summary>
	/// <example><code>
	///		container
	///			.InstallEntityPatterns()
	///			.InstallDbContext&lt;MyApplicationDbContext&lt;()
	///			.InstallLocalizationServices&gt;Language&lt;()
	///			.InstallDataLayer(typeof(ILoginAccountDataSource).Assembly);
	/// </code></example>
	public static class Installer
	{
		/// <summary>
		/// Registruje do Windsor Castle Containeru třídy používané v Havit.Data.Patterns a Havit.Data.Entity.Patterns.
		/// </summary>
		/// <remarks>
		/// Registruje:
		/// <list type="bullet">
		/// <item><description>
		/// Pro <see cref="ISoftDeleteManager"/> registruje <see cref="SoftDeleteManager"/> jako singleton.
		/// </description></item>
		/// <item><description>
		/// Pro IDataEntrySymbolStorage registruje DbDataEntrySymbolStorage jako singleton,
		/// </description></item>
		/// <item><description>
		/// Pro ICurrentCultureService registruje CurrentCultureService jako singleton,
		/// </description></item>
		/// <item><description>
		/// Pro IDataSeedRunner registruje DataSeedRunner jako transientní třídu,
		/// </description></item>
		/// <item><description>
		/// Pro IDataSeedPersister registruje DbDataSeedPersister jako transientní třídu,
		/// </description></item>
		/// <item><description>
		/// /PRo IUnitOfWork a IUnitOfWorkAsync registruje DbUnitOfWork s lifestylem PerWebRequest/PerThread,
		/// </description></item>
		/// <item><description>
		/// Pro IDataLoader a IDataLoaderAsync registruje DbDataLoader s lifestylem PerWebRequest/PerThread.
		/// </description></item>
		/// </list>
		/// </remarks>
		public static IWindsorContainer InstallEntityPatterns(this IWindsorContainer container)
		{
			// framework services & factories
			container.Register(
				Component.For<ISoftDeleteManager>().ImplementedBy<SoftDeleteManager>().LifestyleSingleton(),
				Component.For(typeof(IDataEntrySymbolStorage<>)).ImplementedBy(typeof(DbDataEntrySymbolStorage<>)).LifestyleSingleton(),
				Component.For<ICurrentCultureService>().ImplementedBy<CurrentCultureService>().LifestyleSingleton(),
				Component.For<IDataSeedRunner>().ImplementedBy<DataSeedRunner>().LifestyleTransient(),
				Component.For<IDataSeedPersister>().ImplementedBy<DbDataSeedPersister>().LifestyleTransient(),
				Component.For(typeof(IDataSourceFactory<>)).AsFactory(),
				Component.For(typeof(IRepositoryFactory<>)).AsFactory(),
				Component.For<IUnitOfWork, IUnitOfWorkAsync>().ImplementedBy<DbUnitOfWork>().LifeStyle.HybridPerWebRequestPerThread(),
				Component.For<IDataLoader, IDataLoaderAsync>().ImplementedBy<DbDataLoader>().LifeStyle.HybridPerWebRequestPerThread()
			);

			return container;
		}

		/// <summary>
		/// Registruje pro IDbContext zadaný TDbContext s lifestylem PerWebRequest/PerThread. 
		/// </summary>
		public static IWindsorContainer InstallDbContext<TDbContext>(this IWindsorContainer container)
			where TDbContext : class, IDbContext
		{
			container.Register(Component.For<IDbContext>().ImplementedBy<TDbContext>().LifeStyle.HybridPerWebRequestPerThread());
			return container;
		}

		/// <summary>
		/// Registruje pro ILanguageService zadaný DbLanguageService&lt;TLanguage&gt;, TLanguage je generickým parametrem metody, jako singleton.
		/// Dále registruje pro ILocalizationService službu LocalizationService jako singleton.
		/// </summary>
		public static IWindsorContainer InstallLocalizationServices<TLanguage>(this IWindsorContainer container)
			where TLanguage : class, ILanguage
		{
			Type currentLanguageServiceType = typeof(DbLanguageService<>).MakeGenericType(typeof(TLanguage));
			container.Register(
				Component.For<ILanguageService>().ImplementedBy(currentLanguageServiceType).LifestyleSingleton(),
				Component.For<ILocalizationService>().ImplementedBy<LocalizationService>().LifestyleSingleton()
			);
			return container;
		}

		/// <summary>
		/// Registruje služby DataLayeru z dané assembly do Windsor Castle Containeru.		
		/// </summary>
		/// <remarks>
		/// Registruje:
		/// <list type="bullet">
		/// <item><description>
		/// Pro IDataSeed služby implementující tento interface.
		/// </description></item>
		/// <item><description>
		/// Pro IDataSource&lt;TEntity&gt; a IEntityDataSource službu (služby) dědící z DbDataSource.
		/// </description></item>
		/// <item><description>
		/// Pro IRepository&lt;TEntity&gt; a IEntityRepository službu (služby) dědící z DbRepository.
		/// </description></item>
		/// <item><description>
		/// Pro IDataEntries&lt;TEntity&gt; a IEntityDataSource službu (služby) implementující IDataEntries.
		/// </description></item>
		/// </list>
		/// Služby označené atributem <see cref="FakeAttribute"/> nejsou metodou registrovány.
		/// </remarks>
		public static IWindsorContainer InstallDataLayer(this IWindsorContainer container, Assembly dataLayerAssembly)
		{
			container.Register(
				Classes.FromAssembly(dataLayerAssembly).BasedOn(typeof(IDataSeed)).If(type => !type.HasAttribute<FakeAttribute>()).WithServices(typeof(IDataSeed)).LifestyleTransient(),

				// Registrace přes IDataSource<T> nestačí, protože při pokusu získání instance dostaneme chybu
				// proto registrujeme přes IDataSource<KonkrétníTyp> pomocí metody WithServiceConstructedInterface.
				// Dále registrujeme přes potomky IDataSource<>, např. IKonkrétníTypDataSource.
				Classes.FromAssembly(dataLayerAssembly).BasedOn(typeof(DbDataSource<>)).WithServiceConstructedInterface(typeof(IDataSource<>)).If(DoesNotHaveFakeAttribute).WithServiceFromInterface(typeof(IDataSource<>)).LifestyleTransient(),
				// TODO: Async?
				Classes.FromAssembly(dataLayerAssembly).BasedOn(typeof(DbRepository<>)).If(DoesNotHaveFakeAttribute).WithServiceConstructedInterface(typeof(IRepository<>)).WithServiceFromInterface(typeof(IRepository<>)).Configure(c => c.LifeStyle.HybridPerWebRequestPerThread()),
				Classes.FromAssembly(dataLayerAssembly).BasedOn(typeof(IDataEntries)).If(DoesNotHaveFakeAttribute).WithServiceFromInterface(typeof(IDataEntries)).Configure(c => c.LifeStyle.HybridPerWebRequestPerThread())
			);

			return container;
		}

		private static bool DoesNotHaveFakeAttribute(Type type)
		{
			return !type.HasAttribute<FakeAttribute>();
		}
	}
}