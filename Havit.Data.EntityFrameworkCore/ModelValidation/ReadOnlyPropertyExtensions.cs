using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.ModelValidation;

internal static class ReadOnlyPropertyExtensions
{
	public static bool IsModelValidatorRuleSupressed(this IReadOnlyProperty property, ModelValidatorRule modelValidatorRule)
	{
		object annotationValue = property.FindAnnotation(modelValidatorRule.SuppressModelValidatorRuleAnnotationName)?.Value;
		return (bool?)annotationValue == true;
	}
}
