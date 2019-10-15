using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
	/// <summary>
	/// <see cref="IDbContextOptionsExtension"/> for configuring <see cref="IMigrationsAnnotationProvider"/>s and <see cref="IModelExtensionSqlGenerator"/>s.
	/// </summary>
	public class ModelExtensionsExtension : IDbContextOptionsExtension
	{
		private ImmutableList<Type> annotationProviders = ImmutableList.Create<Type>();
		private ImmutableList<Type> sqlGenerators = ImmutableList.Create<Type>();
		private bool consolidateStatementsForMigrationsAnnotationsForModel = true;

		private DbContextOptionsExtensionInfo _info;

		/// <inheritdoc />
		public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ModelExtensionsExtension()
		{
		}

		// NB: When adding new options, make sure to update the copy ctor below.

		/// <summary>
		/// Copy constructor.
		///
		/// <remarks>Pattern from original EF Core source.</remarks>
		/// </summary>
		protected ModelExtensionsExtension(ModelExtensionsExtension copyFrom)
		{
			annotationProviders = copyFrom.annotationProviders;
			sqlGenerators = copyFrom.sqlGenerators;
			consolidateStatementsForMigrationsAnnotationsForModel = copyFrom.consolidateStatementsForMigrationsAnnotationsForModel;
		}

		/// <summary>
		/// Clones this <see cref="IDbContextOptionsExtension"/>.
		///
		/// <remarks>Pattern from original EF Core source.</remarks>
		/// </summary>
		protected virtual ModelExtensionsExtension Clone() => new ModelExtensionsExtension(this);

		/// <summary>
		/// Specifies, whether generated code statements in migrations with annotations should be consolidated.
		/// 
		/// If enabled <see cref="AlterOperationsFixUpMigrationsModelDiffer"/> is used instead of original implementation of <see cref="IMigrationsModelDiffer"/>.
		/// </summary>
		public bool ConsolidateStatementsForMigrationsAnnotationsForModel => consolidateStatementsForMigrationsAnnotationsForModel;

		/// <summary>
		/// Consolidate generated code statements in migrations with annotations (e.g. AlterDatabase().Annotation().OldAnnotation()).
		///
		/// Enables or disables <see cref="AlterOperationsFixUpMigrationsModelDiffer"/>.
		/// </summary>
		public ModelExtensionsExtension WithConsolidateStatementsForMigrationsAnnotationsForModel(bool consolidateStatementsForMigrationsAnnotationsForModel)
		{
			var clone = Clone();
			clone.consolidateStatementsForMigrationsAnnotationsForModel = consolidateStatementsForMigrationsAnnotationsForModel;
			return clone;
		}

		/// <summary>
		/// Registers <see cref="IModelExtensionAnnotationProvider"/> to use.
		/// </summary>
		/// <typeparam name="T">Implementation of <see cref="IModelExtensionAnnotationProvider"/> to register.</typeparam>
		/// <returns>A new instance of <see cref="ModelExtensionsExtension"/> with option changed.</returns>
		public ModelExtensionsExtension WithAnnotationProvider<T>()
			where T : IModelExtensionAnnotationProvider
		{
			// clone with new IModelExtensionAnnotationProvider
			// https://github.com/aspnet/EntityFrameworkCore/issues/10559#issuecomment-351753702
			// https://github.com/aspnet/EntityFrameworkCore/blob/779d43731773d59ecd5f899a6330105879263cf3/src/EFCore.InMemory/Infrastructure/Internal/InMemoryOptionsExtension.cs#L47
			var clone = Clone();
			clone.annotationProviders = clone.annotationProviders.Add(typeof(T));
			return clone;
		}

		/// <summary>
		/// Registers <see cref="IModelExtensionSqlGenerator"/> to use.
		/// </summary>
		/// <typeparam name="T">Implementation of <see cref="IModelExtensionSqlGenerator"/> to register.</typeparam>
		/// <returns>A new instance of <see cref="ModelExtensionsExtension"/> with option changed.</returns>
		public ModelExtensionsExtension WithSqlGenerator<T>()
			where T : IModelExtensionSqlGenerator
		{
			// clone with new IModelExtensionSqlGenerator 
			var clone = Clone();
			clone.sqlGenerators = clone.sqlGenerators.Add(typeof(T));
			return clone;
		}

		/// <inheritdoc />
		public void ApplyServices(IServiceCollection services)
		{
			var currentProviderTypes = annotationProviders.ToArray();
			CompositeModelExtensionAnnotationProvider AnnotationProviderFactory(IServiceProvider serviceProvider)
			{
				var providers = currentProviderTypes.Select(type => (IModelExtensionAnnotationProvider)serviceProvider.GetService(type)).ToArray();
				return new CompositeModelExtensionAnnotationProvider(providers);
			}
			var currentSqlGeneratorTypes = sqlGenerators.ToArray();
			ModelExtensionSqlResolver DropSqlResolverFactory(IServiceProvider serviceProvider)
			{
				var generators = currentSqlGeneratorTypes.Select(type => (IModelExtensionSqlGenerator)serviceProvider.GetService(type)).ToArray();
				return new ModelExtensionSqlResolver(generators);
			}

			services.Add(annotationProviders.ToArray().Select(t => ServiceDescriptor.Singleton(t, t)));
			services.Add(sqlGenerators.ToArray().Select(t => ServiceDescriptor.Singleton(t, t)));
			services.AddSingleton<IModelExtensionAnnotationProvider, CompositeModelExtensionAnnotationProvider>(AnnotationProviderFactory);
			services.AddSingleton<IModelExtensionSqlResolver, ModelExtensionSqlResolver>(DropSqlResolverFactory);
			if (ConsolidateStatementsForMigrationsAnnotationsForModel)
			{
				var serviceCharacteristics = EntityFrameworkRelationalServicesBuilder.RelationalServices[typeof(IMigrationsModelDiffer)];

				services.Add(ServiceDescriptor.Describe(typeof(IMigrationsModelDiffer), typeof(AlterOperationsFixUpMigrationsModelDiffer), serviceCharacteristics.Lifetime));
			}
		}

		/// <inheritdoc />
		public long GetServiceProviderHashCode()
		{
			var hashCode = annotationProviders.Aggregate(358, (current, next) => current ^ next.GetHashCode());
			hashCode = sqlGenerators.Aggregate(hashCode, (current, next) => current ^ next.GetHashCode());
			return hashCode;
		}

		/// <inheritdoc />
		public void Validate(IDbContextOptions options)
		{
			// no validations
		}

		private class ExtensionInfo : DbContextOptionsExtensionInfo
		{
			public override bool IsDatabaseProvider => false;

			public override string LogFragment => "";

			public ExtensionInfo(IDbContextOptionsExtension dbContextOptionsExtension) : base(dbContextOptionsExtension)
			{
			}

			public override long GetServiceProviderHashCode()
			{
				return 0xE436;
			}

			public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
			{
				// NOOP
			}
		}
	}
}