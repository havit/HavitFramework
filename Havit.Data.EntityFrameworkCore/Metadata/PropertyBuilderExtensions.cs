using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.EntityFrameworkCore.Metadata;

public static class PropertyBuilderExtensions
{
	public static PropertyBuilder<TProperty> AllowNonForeignKeyToEndWithId<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
	{
		return propertyBuilder.HasAnnotation(ModelValidationAnnotationContants.AllowNonForeignKeyToEndWithIdAnnotationName, true);
	}
}
