﻿using System;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.DataSources;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
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
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Diagnostics.Contracts;
using Havit.Model.Localizations;
using Microsoft.EntityFrameworkCore;

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

			installer.AddFactory(typeof(IDbContextFactory));

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
			installer.AddServiceSingleton(typeof(ILanguageByCultureService), currentLanguageByCultureServiceType);
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
			installer.AddServiceSingleton(typeof(IDataEntrySymbolStorage<>), typeof(DataEntrySymbolStorage<>));
			installer.AddServiceSingleton<ICurrentCultureService, CurrentCultureService>();
			installer.AddServiceTransient<IDataSeedRunner, DataSeedRunner>();
			installer.AddServiceTransient<IDataSeedRunDecision, OncePerVersionDataSeedRunDecision>();
			installer.AddServiceTransient<IDataSeedRunDecisionStatePersister, DbDataSeedRunDecisionStatePersister>();
			installer.AddServiceTransient<IDataSeedPersister, DbDataSeedPersister>();
			installer.AddFactory(typeof(IDataSeedPersisterFactory));

			installer.AddFactory(typeof(IDataSourceFactory<>));
			installer.AddFactory(typeof(IRepositoryFactory<>));

			installer.AddServices(new Type[] { typeof(IUnitOfWork), typeof(IUnitOfWorkAsync) }, componentRegistrationOptions.UnitOfWorkType, componentRegistrationOptions.UnitOfWorkLifestyle);
			installer.AddServices(new Type[] { typeof(IDataLoader), typeof(IDataLoaderAsync) }, typeof(DbDataLoaderWithLoadedPropertiesMemory), componentRegistrationOptions.DataLoaderLifestyle);

			installer.AddServiceSingleton<IPropertyLambdaExpressionManager, PropertyLambdaExpressionManager>();
			installer.AddServiceSingleton<IPropertyLambdaExpressionBuilder, PropertyLambdaExpressionBuilder>();
			installer.AddServiceSingleton<IPropertyLambdaExpressionStore, PropertyLambdaExpressionStore>();
			installer.AddServiceSingleton<IPropertyLoadSequenceResolver, PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution>();
			installer.AddServiceSingleton<IBeforeCommitProcessorsRunner, BeforeCommitProcessorsRunner>();

			installer.AddFactory(typeof(IBeforeCommitProcessorsFactory));

			installer.AddServiceSingleton<IBeforeCommitProcessor<object>, SetCreatedToInsertingEntitiesBeforeCommitProcessor>();
			installer.AddServiceSingleton<IEntityValidationRunner, EntityValidationRunner>();

			installer.AddFactory(typeof(IEntityValidatorsFactory));

			installer.AddServiceSingleton<IEntityKeyAccessor, DbEntityKeyAccessor>();
			installer.AddServiceSingleton<IReferencingCollectionsStore, ReferencingCollectionsStore>();
			installer.AddServiceSingleton<ICollectionTargetTypeStore, CollectionTargetTypeStore>();
			installer.AddServiceSingleton<IEntityCacheDependencyKeyGenerator, EntityCacheDependencyKeyGenerator>();
			installer.AddServiceSingleton<IEntityCacheDependencyManager, EntityCacheDependencyManager>();

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
				Type repositoryAsyncConstructedInterface = repositoryType.GetSingleConstructedType(typeof(IRepositoryAsync<>)); // získáme IRepositoryAsync<KonkrétníTyp>
				Type repositoryInterface = repositoryType.GetInterfaces().Where(repositoryTypeInterfaceType => repositoryTypeInterfaceType.ImplementsInterface(repositoryConstructedInterface)).Single(); // získáme IKonkrétníTypDataSource

				installer.AddServices(new Type[] { repositoryInterface, repositoryConstructedInterface, repositoryAsyncConstructedInterface }, repositoryType, componentRegistrationOptions.RepositoriesLifestyle);
			}

			// DataEntries
			Type[] dataEntryTypes = dataLayerDependencyInjectionEnabledTypes.Where(type => type.ImplementsInterface(typeof(IDataEntries))).ToArray();
			foreach (Type dataEntryType in dataEntryTypes)
			{
				Type dataEntryInterface = dataEntryType.GetInterfaces().Where(dataEntryTypeTypeInterfaceType => dataEntryTypeTypeInterfaceType.ImplementsInterface(typeof(IDataEntries))).Single(); // získáme IKonkrétníTypDataSource

				installer.AddService(dataEntryInterface, dataEntryType, componentRegistrationOptions.DataEntriesLifestyle);
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

					installer.AddServiceSingleton(interfaceType, implementationType);
				});

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
			return !type.IsDefined(typeof(FakeAttribute), true);
		}
	}
}