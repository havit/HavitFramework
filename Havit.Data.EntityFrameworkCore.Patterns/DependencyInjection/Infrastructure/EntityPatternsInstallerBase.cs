using System;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.DataSources;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using Havit.Data.Patterns.Attributes;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Localizations.Internal;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.Transactions.Internal;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Diagnostics.Contracts;
using Havit.Model.Localizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure
{
	/// <summary>
	/// Bázová implementace <see cref="IEntityPatternsInstaller"/>u.
	/// Pro použití pro jednotlivé DI kontejnery. Chce se, aby pro každý kontejner byla práce minimální, de facto jen implementace IServiceInstaller&lt;TLifetime&gt;.
	/// </summary>
	public class EntityPatternsInstallerBase<TLifetime> : IEntityPatternsInstaller
	{
		private readonly IServiceInstaller<TLifetime> installer;
		private readonly ComponentRegistrationOptionsBase<TLifetime> componentRegistrationOptions;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="installer">Installer, kterým budou provedeny registrace.</param>
		/// <param name="componentRegistrationOptions">Nastavení registrací.</param>
		public EntityPatternsInstallerBase(IServiceInstaller<TLifetime> installer, ComponentRegistrationOptionsBase<TLifetime> componentRegistrationOptions)
		{
			Contract.Requires<ArgumentNullException>(installer != null);
			Contract.Requires<ArgumentNullException>(componentRegistrationOptions != null);

			this.installer = installer;
			this.componentRegistrationOptions = componentRegistrationOptions;
		}

		/// <summary>
		/// Viz <see cref="IEntityPatternsInstaller"/>
		/// </summary>
		public IEntityPatternsInstaller AddDbContext<TDbContext>(DbContextOptions dbContextOptions = null)
			where TDbContext : class, IDbContext
		{
			installer.AddService(typeof(IDbContext), typeof(TDbContext), componentRegistrationOptions.DbContextLifestyle);
			installer.AddServiceTransient(typeof(IDbContextTransient), typeof(TDbContext));

			if (dbContextOptions != null)
			{
				installer.AddServiceSingletonInstance(typeof(DbContextOptions), dbContextOptions);
			}

			return this;
		}

		/// <summary>
		/// Viz <see cref="IEntityPatternsInstaller"/>
		/// </summary>
		public IEntityPatternsInstaller AddLocalizationServices<TLanguage>()
			where TLanguage : class, ILanguage
		{
			Type currentLanguageServiceType = typeof(LanguageService<>).MakeGenericType(typeof(TLanguage));
			Type currentLanguageByCultureServiceType = typeof(LanguageByCultureService<>).MakeGenericType(typeof(TLanguage));

			installer.AddService(typeof(ILanguageService), currentLanguageServiceType, componentRegistrationOptions.GeneralLifestyle);
			installer.AddServiceTransient(typeof(ILanguageByCultureService), currentLanguageByCultureServiceType);
			installer.AddServiceSingleton<ILanguageByCultureStorage, LanguageByCultureStorage>();
			installer.AddServiceTransient<ILocalizationService, LocalizationService>();

			return this;
		}

		/// <summary>
		/// Viz <see cref="IEntityPatternsInstaller"/>
		/// </summary>
		public IEntityPatternsInstaller AddEntityPatterns()
		{
			componentRegistrationOptions.CachingInstaller.Install(installer);

			installer.AddServiceSingleton<ISoftDeleteManager, SoftDeleteManager>();
			installer.AddServiceSingleton<ICurrentCultureService, CurrentCultureService>();
			installer.AddServiceTransient<IDataSeedRunner, DataSeedRunner>();
			installer.AddServiceTransient<ITransactionWrapper, TransactionScopeTransactionWrapper>();
			installer.AddServiceTransient<IDataSeedRunDecision, OncePerVersionDataSeedRunDecision>();
			installer.AddServiceTransient<IDataSeedRunDecisionStatePersister, DbDataSeedRunDecisionStatePersister>();
			installer.AddServiceTransient<IDataSeedPersister, DbDataSeedPersister>();
			installer.AddFactory(typeof(IDataSeedPersisterFactory));

			installer.AddService(typeof(IUnitOfWork), componentRegistrationOptions.UnitOfWorkType, componentRegistrationOptions.UnitOfWorkLifestyle);
			installer.AddService<IDataLoader, DbDataLoaderWithLoadedPropertiesMemory>(componentRegistrationOptions.DataLoaderLifestyle);
			installer.AddServiceTransient<ILookupDataInvalidationRunner, LookupDataInvalidationRunner>();

			installer.AddServiceSingleton<IPropertyLambdaExpressionManager, PropertyLambdaExpressionManager>();
			installer.AddServiceSingleton<IPropertyLambdaExpressionBuilder, PropertyLambdaExpressionBuilder>();
			installer.AddServiceSingleton<IPropertyLambdaExpressionStore, PropertyLambdaExpressionStore>();
			installer.AddServiceSingleton<IPropertyLoadSequenceResolver, PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution>();
			installer.AddServiceSingleton<IBeforeCommitProcessorsRunner, BeforeCommitProcessorsRunner>();

			installer.AddFactory(typeof(IBeforeCommitProcessorsFactory));

			installer.AddServiceSingleton<IBeforeCommitProcessor<object>, SetCreatedToInsertingEntitiesBeforeCommitProcessor>();
			installer.AddServiceSingleton<IEntityValidationRunner, EntityValidationRunner>();

			installer.AddFactory(typeof(IEntityValidatorsFactory));

			installer.AddServiceTransient<IEntityKeyAccessor, DbEntityKeyAccessor>();
			installer.AddServiceSingleton<IDbEntityKeyAccessorStorage, DbEntityKeyAccessorStorage>();
			installer.AddServiceTransient<IReferencingCollectionsService, ReferencingCollectionsService>();
			installer.AddServiceSingleton<IReferencingCollectionsStorage, ReferencingCollectionsStorage>();
			installer.AddServiceTransient<ICollectionTargetTypeService, CollectionTargetTypeService>();
			installer.AddServiceSingleton<ICollectionTargetTypeStorage, CollectionTargetTypeStorage>();
			installer.AddServiceSingleton<IEntityCacheDependencyKeyGenerator, EntityCacheDependencyKeyGenerator>();
			installer.AddServiceTransient<IEntityCacheDependencyManager, EntityCacheDependencyManager>();

			return this;
		}

		/// <summary>
		/// Viz <see cref="IEntityPatternsInstaller"/>
		/// </summary>
		public IEntityPatternsInstaller AddDataLayer(Assembly dataLayerAssembly)
		{
			Type[] dataLayerDependencyInjectionEnabledTypes = dataLayerAssembly.GetTypes().Where(type => type.IsClass && type.IsPublic).Where(IsNotAbstract).Where(DoesNotHaveFakeAttribute).ToArray();

			// DataSeeds
			Type[] dataSeedTypes = dataLayerDependencyInjectionEnabledTypes.Where(type => type.ImplementsInterface(typeof(IDataSeed))).ToArray();

			foreach (Type dataSeedType in dataSeedTypes)
			{
				installer.AddServiceTransient(typeof(IDataSeed), dataSeedType);
			}

			// Registrace přes IDataSource<T> nestačí, protože při pokusu získání instance dostaneme chybu
			// proto registrujeme přes IDataSource<KonkrétníTyp> pomocí metody WithServiceConstructedInterface.
			// Dále registrujeme přes potomky IDataSource<>, např. IKonkrétníTypDataSource.

			// DataSources
			Type[] dataSourceTypes = dataLayerDependencyInjectionEnabledTypes.Where(type => type.HasAncestorOfType(typeof(DbDataSource<>))).ToArray();
			foreach (Type dataSourceType in dataSourceTypes)
			{
				Type dataSourceConstructedInterface = dataSourceType.GetSingleConstructedType(typeof(IDataSource<>)); // získáme IDataSource<KonkrétníTyp>
				Type dataSourceInterface = dataSourceType.GetInterfaces().Where(dataSourceTypeInterfaceType => dataSourceTypeInterfaceType.ImplementsInterface(dataSourceConstructedInterface)).Single(); // získáme IKonkrétníTypDataSource

				installer.AddServicesTransient(new Type[] { dataSourceInterface, dataSourceConstructedInterface }, dataSourceType);
			}


			// Repositories
			Type[] repositoryTypes = dataLayerDependencyInjectionEnabledTypes.Where(type => type.HasAncestorOfType(typeof(DbRepository<>))).ToArray();
			foreach (Type repositoryType in repositoryTypes)
			{
				Type repositoryConstructedInterface = repositoryType.GetSingleConstructedType(typeof(IRepository<>)); // získáme IRepository<KonkrétníTyp>
				Type repositoryInterface = repositoryType.GetInterfaces().Where(repositoryTypeInterfaceType => repositoryTypeInterfaceType.ImplementsInterface(repositoryConstructedInterface)).Single(); // získáme IKonkrétníTypDataSource

				installer.AddServices(new Type[] { repositoryInterface, repositoryConstructedInterface }, repositoryType, componentRegistrationOptions.RepositoriesLifestyle);
			}

			// DataEntries
			Type[] dataEntryTypes = dataLayerDependencyInjectionEnabledTypes
				.Where(type => type.ImplementsInterface(typeof(IDataEntries)) // musí implementovat IDataEntries
					&& (type.BaseType != null)
					&& (type.BaseType.IsGenericType)
					&& (type.BaseType.GetGenericTypeDefinition() == typeof(DataEntries<>))) // a dědit z DataEntries (pro test konstruktorů, viz dále)
				.ToArray();

			foreach (Type dataEntryType in dataEntryTypes)
			{
				Type dataEntryInterface = dataEntryType.GetInterfaces().Where(dataEntryTypeTypeInterfaceType => dataEntryTypeTypeInterfaceType.ImplementsInterface(typeof(IDataEntries))).Single(); // získáme IKonkrétníTypDataSource

				installer.AddService(dataEntryInterface, dataEntryType, componentRegistrationOptions.DataEntriesLifestyle);

				// DataEntrySymbolService+Storage potřebujeme jen pro ty dataEntryTypes, které mají dva konstruktory.
				// Pokud má jeden konstruktor, je to IRepository.
				// Pokud má dva konstruktory, je to IDataEntrySymbolService a IRepository.

				if (dataEntryType.GetConstructors().Single().GetParameters().Count() == 2)
				{					
					Type entityType = dataEntryType.BaseType.GetGenericArguments().Single();  // získáme KonkretníTyp

					installer.AddServiceTransient(typeof(IDataEntrySymbolService<>).MakeGenericType(entityType), typeof(DataEntrySymbolService<>).MakeGenericType(entityType));
					installer.AddServiceSingleton(typeof(IDataEntrySymbolStorage<>).MakeGenericType(entityType), typeof(DataEntrySymbolStorage<>).MakeGenericType(entityType));
				}
			}

			// Potřebujeme zaregistrovat IEntityKeyAccessor<TEntity, int>.
			// Nemáme seznam TEntity, tak je získáme z existujících implementací IRepository<>.
			// Pak pro každou TEntity zaregistrujeme DbEntityKeyAccesor<TEntity, int> pod IEntityKeyAccessor<TEntity, int>.
			dataLayerAssembly
				.GetExportedTypes()
				.SelectMany(type => type.GetInterfaces())
				.Where(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IRepository<>))
				.Select(repositoryInterfaceType => repositoryInterfaceType.GetGenericArguments().Single()) // IRepository<TEntity> --> TEntity (tj. výsledkem je neunikátní seznam modelových tříd)
				.Distinct() // různé interface a třídy nám (DbRepository<Xy>, IXyRepository) nám dají stejné modelové třídy
				.ToList() // aby šel použít foreach
				.ForEach(modelType =>
				{
					Type interfaceType = typeof(IEntityKeyAccessor<,>).MakeGenericType(modelType, typeof(int)); // --> IEntityKeyAccessor<TEntity, int>
					Type implementationType = typeof(DbEntityKeyAccessor<,>).MakeGenericType(modelType, typeof(int)); // --> DbEntityKeyAccessor<TEntity, int>

					installer.AddServiceTransient(interfaceType, implementationType);
				});

			return this;
		}

		/// <summary>
		/// Viz <see cref="IEntityPatternsInstaller"/>
		/// </summary>
		public IEntityPatternsInstaller AddLookupService<TService, TImplementation>()
			where TService : class
			where TImplementation : class, TService, ILookupDataInvalidationService
		{
			return AddLookupService<TService, TImplementation, TImplementation>();
		}

		/// <summary>
		/// Viz <see cref="IEntityPatternsInstaller"/>
		/// </summary>
		public IEntityPatternsInstaller AddLookupService<TService, TImplementation, TLookupDataInvalidationService>()
			where TService : class
			where TImplementation : class, TService
			where TLookupDataInvalidationService: ILookupDataInvalidationService
		{
			if (!lookupServicesAdded)
			{
				installer.AddServiceSingleton<IEntityLookupDataStorage, EntityLookupDataStorage>();
				lookupServicesAdded = true;
			}
			installer.AddServicesTransient(new Type[] { typeof(TService), typeof(ILookupDataInvalidationService) }, typeof(TLookupDataInvalidationService));

			return this;
		}
		private bool lookupServicesAdded = false;

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
			return !type.IsDefined(typeof(FakeAttribute), true);
		}
	}
}
