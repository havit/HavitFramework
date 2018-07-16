namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	internal static class ExtendedPropertiesAnnotationsHelper
	{
		public static string AnnotationPrefix => "ExtendedProperty";

		public static string BuildAnnotationName(ExtendedPropertyAttribute attribute) => $"{AnnotationPrefix}:{attribute.Name}";
	}
}
