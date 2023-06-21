using System;
using Havit.Data.EntityFrameworkCore.Migrations.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure;

/// <summary>
/// Internal class for creating <see cref="IModel"/> that adds annotations for Model Extensions using convention.
/// </summary>
/// <remarks>
/// Applies <see cref="ModelExtensionRegistrationConvention"/> only to main DbContext model (i.e. adds Model Extensions annotations).
/// Does not pollute any other <see cref="IModel"/> created (within DbContext using this <see cref="IModelSource"/>) - mainly HistoryRepository's model.
/// 
/// Fixes bug #48448 with polluting HistoryRepository model (and other secondary models).
///
/// <see cref="ModelExtensionsModelSource"/> is scoped, because <see cref="ModelExtensionRegistrationConventionPlugin"/> is scoped as well.
/// However EF Core depends on <see cref="IModelSource"/> being singleton, so we want to adhere to this precondition.
/// <see cref="ScopeBridgingModelSource"/> handles transitioning to scope of currently used DbContext along with caching implemented in <see cref="ModelSource"/>.
/// </remarks>
public class ModelExtensionsModelSource : ModelSource, IScopedModelSource
{
	private readonly ModelExtensionRegistrationConventionPlugin conventionPlugin;

	/// <summary>
	/// Constructor.
	/// </summary>
	public ModelExtensionsModelSource(
		ModelSourceDependencies dependencies,
		ModelExtensionRegistrationConventionPlugin conventionPlugin)
		: base(dependencies)
	{
		this.conventionPlugin = conventionPlugin;
	}

	/// <summary>
	///     Creates the model with Model Extensions annotations. Caching is not implemented,
	///     since dependency <see cref="ModelExtensionRegistrationConventionPlugin"/> has scoped lifestyle.
	/// </summary>
	/// <remarks>
	///     Implemented by adding <see cref="ModelExtensionRegistrationConvention"/> into <see cref="ConventionSet"/> used to create <see cref="IModel"/>.
	/// </remarks>
	/// <returns> The model to be used. </returns>
	public override IModel GetModel(DbContext context, ModelCreationDependencies modelCreationDependencies, bool designTime)
	{
		var model = CreateModel(context, modelCreationDependencies.ConventionSetBuilder, modelCreationDependencies.ModelDependencies);

		// Vrácený model musí být finalizovaný, proto nestačí vrátit výsledek metody CreateModel.
		// Finalizace modelu - vykopírováno z bázové ModelSource.GetModel(...).
		model = modelCreationDependencies.ModelRuntimeInitializer.Initialize(model, designTime, modelCreationDependencies.ValidationLogger);
		return model;
	}

	/// <summary>
	///     Creates the model with Model Extensions annotations. Caching is not implemented,
	///     since dependency <see cref="ModelExtensionRegistrationConventionPlugin"/> has scoped lifestyle.
	/// </summary>
	///
	/// <remarks>
	///     Implemented by adding <see cref="ModelExtensionRegistrationConvention"/> into <see cref="ConventionSet"/> used to create <see cref="IModel"/>.
	/// </remarks>
	/// <returns> The model to be used. </returns>
	protected override IModel CreateModel(DbContext context, IConventionSetBuilder conventionSetBuilder, ModelDependencies modelDependencies)
	{
		ConventionSet conventionSet = conventionPlugin.ModifyConventions(conventionSetBuilder.CreateConventionSet());
		return base.CreateModel(context, new StaticConventionSetBuilder(conventionSet), modelDependencies);
	}

}