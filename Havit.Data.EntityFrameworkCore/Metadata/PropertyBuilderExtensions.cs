using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.EntityFrameworkCore.Metadata;

/// <summary>
/// Extension metody k PropertyBuilder&lt;TProperty&gt;.
/// </summary>
public static class PropertyBuilderExtensions
{
	/// <summary>
	/// Doplní k property anotaci potlačující pravidlo model validátoru.
	/// </summary>
	public static PropertyBuilder<TProperty> SuppressModelValidatorRule<TProperty>(this PropertyBuilder<TProperty> propertyBuilder, ModelValidatorRule modelValidatorRule)
	{
		return propertyBuilder.HasAnnotation(modelValidatorRule.SuppressModelValidatorRuleAnnotationName, true);
	}
}
