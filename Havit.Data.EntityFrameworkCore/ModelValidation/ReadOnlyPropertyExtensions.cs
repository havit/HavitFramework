using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.ModelValidation;

internal static class ReadOnlyPropertyExtensions
{
	public static bool IsNonForeignKeyAllowedToEndWithId(this IReadOnlyProperty property)
	{
		object annotationValue = property.FindAnnotation(ModelValidationAnnotationContants.AllowNonForeignKeyToEndWithIdAnnotationName)?.Value;
		return (bool?)annotationValue == true;
	}
}
