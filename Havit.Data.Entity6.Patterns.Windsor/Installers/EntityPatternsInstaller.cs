using System;
using System.Reflection;
using Castle.Core.Internal;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Havit.Data.Entity.Patterns.DataEntries;
using Havit.Data.Entity.Patterns.DataLoaders;
using Havit.Data.Entity.Patterns.QueryServices;
using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.Seeds;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.Attributes;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.QueryServices;
using Havit.Data.Patterns.Repositories;
using Havit.Diagnostics.Contracts;
using Havit.Model.Localizations;

namespace Havit.Data.Entity.Patterns.Windsor.Installers
{
	/// <summary>
	/// Implementace <see cref="IEntityPatternsInstaller"/>u.
	/// </summary>
	internal class EntityPatternsInstaller : IEntityPatternsInstaller
	{
		private readonly IWindsorContainer container;
		private readonly ComponentRegistrationOptions componentRegistrationOptions;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="container">Windsor container, do kterého budou provedeny registrace.</param>
		/// <param name="componentRegistrationOptions">Nastavení registrací.</param>
		public EntityPatternsInstaller(IWindsorContainer container, ComponentRegistrationOptions componentRegistrationOptions)
		{
			Contract.Requires<ArgumentNullException>(container != null);
			Contract.Requires<ArgumentNullException>(componentRegistrationOptions != null);

			this.container = container;
			this.componentRegistrationOptions = componentRegistrationOptions;
		}

		/// <summary>
		/// Viz <see cref="IEntityPatternsInstaller"/>
		/// </summary>
		public IEntityPatternsInstaller RegisterDbContext<TDbContext>()
			where TDbContext : class, IDbContext
		{
			container.Register(Component.For(typeof(IDbContext)).ImplementedBy(typeof(TDbContext)).ApplyLifestyle(componentRegistrationOptions.DbContextLifestyle));
			return this;
		}

		/// <summary>
		/// Viz <see cref="IEntityPatternsInstaller"/>
		/// </summary>
		public IEntityPatternsInstaller RegisterLocalizationServices<TLanguage>()
		{
			Type currentLanguageServiceType = typeof(LanguageService<>).MakeGenericType(typeof(TLanguage));
			container.Register(
				Component.For<ILanguageService>().ImplementedBy(currentLanguageServiceType).LifestyleSingleton(),
				Component.For<ILocalizationService>().ImplementedBy<LocalizationService>().LifestyleSingleton(),

				// TODO: Registrujeme jen pro ILanguage, chtělo by obecně na modelové třídy (takže toto je takový malý HACK).
				Component.For<IEntityKeyAccessor<ILanguage, int>>().ImplementedBy<EntityKeyAccessor>().LifestyleSingleton()
			);
			return this;
		}

		/// <summary>
		/// Viz <see cref="IEntityPatternsInstaller"/>
		/// </summary>
		public IEntityPatternsInstaller RegisterEntityPatterns()
		{
			container.Register(
				Component.For<ISoftDeleteManager>().ImplementedBy<SoftDeleteManager>().LifestyleSingleton(),
				Component.For(typeof(IDataEntrySymbolStorage<>)).ImplementedBy(typeof(DbDataEntrySymbolStorage<>)).LifestyleSingleton(),
				Component.For<ICurrentCultureService>().ImplementedBy<CurrentCultureService>().LifestyleSingleton(),
				Component.For<IDataSeedRunner>().ImplementedBy<DataSeedRunner>().LifestyleTransient(),
				Component.For<IDataSeedPersister>().ImplementedBy<DbDataSeedPersister>().LifestyleTransient(),
				Component.For(typeof(IDataSourceFactory<>)).AsFactory(),
				Component.For(typeof(IRepositoryFactory<>)).AsFactory(),
				Component.For(typeof(IUnitOfWork), typeof(IUnitOfWorkAsync)).ImplementedBy(componentRegistrationOptions.UnitOfWorkType).ApplyLifestyle(componentRegistrationOptions.UnitOfWorkLifestyle),
				Component.For(typeof(IDataLoader), typeof(IDataLoaderAsync)).ImplementedBy<DbDataLoader>().ApplyLifestyle(componentRegistrationOptions.DataLoaderLifestyle)
			);
			return this;
		}

		/// <summary>
		/// Viz <see cref="IEntityPatternsInstaller"/>
		/// </summary>
		public IEntityPatternsInstaller RegisterDataLayer(Assembly dataLayerAssembly)
		{
			container.Register(
				Classes.FromAssembly(dataLayerAssembly).BasedOn(typeof(IDataSeed)).If(IsNotAbstract).If(DoesNotHaveFakeAttribute).WithServices(typeof(IDataSeed)).LifestyleTransient(),

				// Registrace přes IDataSource<T> nestačí, protože při pokusu získání instance dostaneme chybu
				// proto registrujeme přes IDataSource<KonkrétníTyp> pomocí metody WithServiceConstructedInterface.
				// Dále registrujeme přes potomky IDataSource<>, např. IKonkrétníTypDataSource.
				Classes.FromAssembly(dataLayerAssembly).BasedOn(typeof(DbDataSource<>)).WithServiceConstructedInterface(typeof(IDataSource<>)).If(IsNotAbstract).If(DoesNotHaveFakeAttribute).WithServiceFromInterface(typeof(IDataSource<>)).LifestyleTransient(),
				// TODO: Async?
				Classes.FromAssembly(dataLayerAssembly).BasedOn(typeof(DbRepository<>)).If(IsNotAbstract).If(DoesNotHaveFakeAttribute).WithServiceConstructedInterface(typeof(IRepository<>)).WithServiceFromInterface(typeof(IRepository<>)).ApplyLifestyle(componentRegistrationOptions.RepositoriesLifestyle),
				Classes.FromAssembly(dataLayerAssembly).BasedOn(typeof(IDataEntries)).If(IsNotAbstract).If(DoesNotHaveFakeAttribute).WithServiceFromInterface(typeof(IDataEntries)).ApplyLifestyle(componentRegistrationOptions.DataEntriesLifestyle)
			);
			return this;
		}

		/// <summary>
		/// Vrací true, pokud NEJDE o abstraktní typ.
		/// </summary>
		private static bool IsNotAbstract(Type type)
		{
			return !type.IsAbstract;
		}

		/// <summary>
		/// Vrací true, pokud typ NENÍ dekorován atributem FakeAttribute.
		/// </summary>
		private static bool DoesNotHaveFakeAttribute(Type type)
		{
			return !type.HasAttribute<FakeAttribute>();
		}
	}
}
