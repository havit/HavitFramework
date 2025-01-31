using System.Collections.Immutable;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;

/// <summary>
/// <see cref="IDbContextOptionsExtension"/> for configuring <see cref="IMigrationsAnnotationProvider"/>s and <see cref="IModelExtensionSqlGenerator"/>s.
/// </summary>
public class ModelExtensionsExtension : IDbContextOptionsExtension
{
	private ImmutableList<Type> _annotationProviders = ImmutableList.Create<Type>();
	private ImmutableList<Type> _sqlGenerators = ImmutableList.Create<Type>();
	private bool _consolidateStatementsForMigrationsAnnotationsForModel = true;
	private Assembly _extensionsAssembly;

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
		_annotationProviders = copyFrom._annotationProviders;
		_sqlGenerators = copyFrom._sqlGenerators;
		_consolidateStatementsForMigrationsAnnotationsForModel = copyFrom._consolidateStatementsForMigrationsAnnotationsForModel;
		_extensionsAssembly = copyFrom._extensionsAssembly;
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
	public bool ConsolidateStatementsForMigrationsAnnotationsForModel => _consolidateStatementsForMigrationsAnnotationsForModel;

	/// <summary>
	/// <see cref="Assembly"/> that contains <see cref="IModelExtender"/>s. This assembly is used to register <see cref="IModelExtender"/>s into the model.
	/// </summary>
	public Assembly ExtensionsAssembly => _extensionsAssembly;

	/// <summary>
	/// Consolidate generated code statements in migrations with annotations (e.g. AlterDatabase().Annotation().OldAnnotation()).
	///
	/// Enables or disables <see cref="AlterOperationsFixUpMigrationsModelDiffer"/>.
	/// </summary>
	public ModelExtensionsExtension WithConsolidateStatementsForMigrationsAnnotationsForModel(bool consolidateStatementsForMigrationsAnnotationsForModel)
	{
		var clone = Clone();
		clone._consolidateStatementsForMigrationsAnnotationsForModel = consolidateStatementsForMigrationsAnnotationsForModel;
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
		clone._annotationProviders = clone._annotationProviders.Add(typeof(T));
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
		clone._sqlGenerators = clone._sqlGenerators.Add(typeof(T));
		return clone;
	}

	/// <summary>
	/// Configures assembly that contains model extenders.
	/// </summary>
	/// <param name="extensionsAssembly">Assembly with model extenders.</param>
	/// <returns>A new instance of <see cref="ModelExtensionsExtension"/> with option changed.</returns>
	public ModelExtensionsExtension WithExtensionsAssembly(Assembly extensionsAssembly)
	{
		// currently we don't allow setting null, Model Extensions is based around using this assembly

		Contract.Requires<ArgumentNullException>(extensionsAssembly != null);

		// clone with new extensions assembly 
		var clone = Clone();
		clone._extensionsAssembly = extensionsAssembly;
		return clone;
	}

	/// <inheritdoc />
	public void ApplyServices(IServiceCollection services)
	{
		var currentProviderTypes = _annotationProviders.ToArray();
		CompositeModelExtensionAnnotationProvider AnnotationProviderFactory(IServiceProvider serviceProvider)
		{
			var providers = currentProviderTypes.Select(type => (IModelExtensionAnnotationProvider)serviceProvider.GetService(type)).ToArray();
			return new CompositeModelExtensionAnnotationProvider(providers);
		}
		var currentSqlGeneratorTypes = _sqlGenerators.ToArray();
		ModelExtensionSqlResolver DropSqlResolverFactory(IServiceProvider serviceProvider)
		{
			var generators = currentSqlGeneratorTypes.Select(type => (IModelExtensionSqlGenerator)serviceProvider.GetService(type)).ToArray();
			return new ModelExtensionSqlResolver(generators);
		}

		services.Add(_annotationProviders.ToArray().Select(t => ServiceDescriptor.Singleton(t, t)));
		services.Add(_sqlGenerators.ToArray().Select(t => ServiceDescriptor.Singleton(t, t)));
		services.AddSingleton<IModelExtensionAnnotationProvider, CompositeModelExtensionAnnotationProvider>(AnnotationProviderFactory);
		services.AddSingleton<IModelExtensionSqlResolver, ModelExtensionSqlResolver>(DropSqlResolverFactory);
		if (ConsolidateStatementsForMigrationsAnnotationsForModel)
		{
			services.ReplaceCoreService<IMigrationsModelDiffer, AlterOperationsFixUpMigrationsModelDiffer>();
		}

		services.TryAddScoped<IModelExtensionsAssembly, ModelExtensionsAssembly>();
	}

	/// <inheritdoc />
	public void Validate(IDbContextOptions options)
	{
		// NOOP
	}

	private class ExtensionInfo : DbContextOptionsExtensionInfo
	{
		public override bool IsDatabaseProvider => false;

		public override string LogFragment => "";

		public ExtensionInfo(IDbContextOptionsExtension dbContextOptionsExtension) : base(dbContextOptionsExtension)
		{
		}

		public override int GetServiceProviderHashCode() => 0xE436;

		public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is ExtensionInfo;

		public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
		{
			// NOOP
		}
	}
}