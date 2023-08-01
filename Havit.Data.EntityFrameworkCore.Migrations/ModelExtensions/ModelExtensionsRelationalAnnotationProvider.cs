using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;

/// <summary>
/// Gives access to <see cref="IAnnotation"/>s that represent <see cref="IModelExtension"/> (used by EF Core Migrations on various elements of the <see cref="IRelationalModel" />).
///
/// <see cref="IModelExtension"/> objects are stored as <see cref="IAnnotation"/> inside <see cref="IModel"/>.
/// </summary>
public class ModelExtensionsRelationalAnnotationProvider : RelationalAnnotationProvider
{
	private readonly IModelExtensionAnnotationProvider annotationProvider;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ModelExtensionsRelationalAnnotationProvider(
		RelationalAnnotationProviderDependencies dependencies,
		IModelExtensionAnnotationProvider annotationProvider)
		: base(dependencies)
	{
		this.annotationProvider = annotationProvider;
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> For(IRelationalModel relationalModel, bool designTime)
	{
		// This is crucial: Model Extensions are defined on IModel (not IRelationalModel) as annotations.
		// IRelationalModel is created from IModel as a "wrapper" around IModel.
		// RelationalModelConvention (in EF Core) creates IRelationalModel and attaches it to IModel.
		// Under the hood it uses IRelationalAnnotationProvider
		return relationalModel.Model.GetAnnotations().Where(IsModelExtension);
	}

	private bool IsModelExtension(IAnnotation annotation)
	{
		return annotationProvider.GetModelExtensions(new List<IAnnotation> { annotation }).Any();
	}
}