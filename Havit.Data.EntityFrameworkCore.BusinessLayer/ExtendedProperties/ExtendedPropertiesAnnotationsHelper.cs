using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties
{
	internal static class ExtendedPropertiesAnnotationsHelper
	{
		internal static readonly StringComparison Comparison = StringComparison.Ordinal;
		internal static readonly StringComparer Comparer = StringComparer.Ordinal;

		private const string Separator = ":";
		private const string ExtendedPropertyName = "ExtendedProperty";
		private const string AnnotationMarker = ExtendedPropertyName + Separator;
		private const string SchemaKey = "SCHEMA";

		internal static bool IsExtendedPropertyAnnotation(IAnnotation annotation) => annotation.Name.StartsWith(AnnotationMarker, Comparison);

		internal static string ParseAnnotationName(IAnnotation annotation)
		{
			return annotation.Name.Substring(AnnotationMarker.Length);
		}

		internal static bool TryParseExtraDatabaseObjectAnnotationName(IAnnotation annotation, out string schema, out string level1Type, out string level1Name, out string name)
		{
			const string SchemaGroupName = "schema";
			const string Level1TypeGroupName = "level1Type";
			const string Level1NameGroupName = "level1Name";
			const string NameGroupName = "name";
			var match = Regex.Match(annotation.Name, $@"^{ExtendedPropertyName}:{SchemaKey}=(?<{SchemaGroupName}>.*):(?<{Level1TypeGroupName}>.+)=(?<{Level1NameGroupName}>.+):(?<{NameGroupName}>.+)$");
			if (!match.Success)
			{
				schema = default;
				level1Type = default;
				level1Name = default;
				name = default;
				return false;
			}
			schema = NullIfEmpty(match.Groups[SchemaGroupName].Value);
			level1Type = match.Groups[Level1TypeGroupName].Value;
			level1Name = match.Groups[Level1NameGroupName].Value;
			name = match.Groups[NameGroupName].Value;
			return true;
		}

		internal static void AddExtendedPropertyAnnotations(IMutableAnnotatable annotatable, MemberInfo memberInfo)
		{
			var attributes = memberInfo.GetCustomAttributes(typeof(ExtendedPropertiesAttribute), false).Cast<ExtendedPropertiesAttribute>();
			foreach (var attribute in attributes)
			{
				AddExtendedPropertyAnnotations(annotatable, attribute.GetExtendedProperties(memberInfo));
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

		internal static IAnnotation ExtraDatabaseObjectAnnotation(string name, string value, string schema, string level1Type, string level1Name)
		{
			return new Annotation(BuildExtraDatabaseObjectAnnotationName(name, schema, level1Type, level1Name), value);
		}

		private static string BuildAnnotationName(string name) => $"{ExtendedPropertyName}{Separator}{name}";

		private static string BuildExtraDatabaseObjectAnnotationName(string name, string schema, string level1Type, string level1Name) => $"{ExtendedPropertyName}{Separator}{SchemaKey}={schema}{Separator}{level1Type}={level1Name}{Separator}{name}";

		private static string NullIfEmpty(string s) => string.IsNullOrEmpty(s) ? null : s;
	}
}
