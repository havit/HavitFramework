using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	internal static class ExtendedPropertiesAnnotationsHelper
	{
		internal static readonly StringComparison Comparison = StringComparison.Ordinal;
		internal static readonly StringComparer Comparer = StringComparer.Ordinal;

		private const string AnnotationMarker = "ExtendedProperty:";

		internal static bool IsExtendedPropertyAnnotation(IAnnotation annotation) => annotation.Name.StartsWith(AnnotationMarker, Comparison);

		internal static string ParseAnnotationName(IAnnotation annotation) => IsExtendedPropertyAnnotation(annotation) ? annotation.Name.Substring(AnnotationMarker.Length) : null;

		internal static void AddExtendedPropertyAnnotations(IMutableAnnotatable annotatable, MemberInfo memberInfo)
		{
			var attributes = memberInfo.GetCustomAttributes(typeof(ExtendedPropertyAttribute), false).Cast<ExtendedPropertyAttribute>();
			foreach (var attribute in attributes)
			{
				AddExtendedPropertyAnnotations(annotatable, attribute.ExtendedProperties);
			}
		}

		internal static void AddExtendedPropertyAnnotations(IMutableAnnotatable annotatable, IDictionary<string, string> extendedProperties)
		{
			if (extendedProperties == null)
			{
				return;
			}

			foreach (var property in extendedProperties)
			{
				annotatable.AddAnnotation(BuildAnnotationName(property.Key), property.Value);
			}
		}

		private static string BuildAnnotationName(string name) => $"{AnnotationMarker}{name}";
	}
}
