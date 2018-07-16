using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	internal static class ExtendedPropertiesAnnotationsHelper
	{
		private const string Separator = ":";
		private const string AnnotationPrefix = "ExtendedProperty";

		public static string BuildAnnotationName(ExtendedPropertyAttribute attribute) => $"{AnnotationPrefix}{Separator}{attribute.Name}";

		public static bool AnnotationsFilter(IAnnotation annotation) => annotation.Name.StartsWith($"{AnnotationPrefix}{Separator}");
	}
}
