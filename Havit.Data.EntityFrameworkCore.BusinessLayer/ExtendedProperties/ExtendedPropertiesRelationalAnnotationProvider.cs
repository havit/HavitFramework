using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;

internal class ExtendedPropertiesRelationalAnnotationProvider : RelationalAnnotationProvider
{
	public ExtendedPropertiesRelationalAnnotationProvider(RelationalAnnotationProviderDependencies dependencies)
		: base(dependencies)
	{ }

	public override IEnumerable<IAnnotation> For(IColumn column, bool designTime)
	{
		return column.PropertyMappings.Select(mapping => mapping.Property).SelectMany(Handle);
	}

	public override IEnumerable<IAnnotation> For(ITable table, bool designTime)
	{
		return table.EntityTypeMappings.Select(mapping => mapping.TypeBase).SelectMany(Handle);
	}

	public override IEnumerable<IAnnotation> For(IRelationalModel model, bool designTime)
	{
		return Handle(model.Model);
	}

	private static IEnumerable<IAnnotation> Handle(IAnnotatable annotatable)
	{
		return annotatable.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation);
	}
}
