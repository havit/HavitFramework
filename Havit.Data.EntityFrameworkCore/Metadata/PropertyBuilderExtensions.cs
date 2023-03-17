using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.EntityFrameworkCore.Metadata;

/// <summary>
/// Extension metody k PropertyBuilder&lt;TProperty&gt;.
/// </summary>
public static class PropertyBuilderExtensions
{
	/// <summary>
	/// Doplní k property anotaci povolující pojmenování vlastnosti s 'Id' na konci, přestože nejde o cizí klíč.
	/// </summary>
	public static PropertyBuilder<TProperty> AllowNonForeignKeyToEndWithId<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
	{
		return propertyBuilder.HasAnnotation(ModelValidationAnnotationContants.AllowNonForeignKeyToEndWithIdAnnotationName, true);
	}
}
