using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;

/// <summary>
/// Gives access to <see cref="IAnnotation"/>s that represent <see cref="IModelExtension"/> (used by EF Core Migrations on various elements of the <see cref="IModel" />).
///
/// <see cref="IModelExtension"/> objects are stored as <see cref="IAnnotation"/> inside <see cref="IModel"/>.
/// </summary>
public class ModelExtensionsMigrationsAnnotationProvider : MigrationsAnnotationProvider
{
	private readonly IModelExtensionAnnotationProvider annotationProvider;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ModelExtensionsMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies, IModelExtensionAnnotationProvider annotationProvider)
		: base(dependencies)
	{
		this.annotationProvider = annotationProvider;
	}

	/// <inheritdoc />
	public override IEnumerable<IAnnotation> ForRemove(IRelationalModel relationalModel)
	{
		return relationalModel.GetAnnotations().Where(IsModelExtension);
	}

	private bool IsModelExtension(IAnnotation annotation)
	{
		return annotationProvider.GetModelExtensions(new List<IAnnotation> { annotation }).Any();
	}
}