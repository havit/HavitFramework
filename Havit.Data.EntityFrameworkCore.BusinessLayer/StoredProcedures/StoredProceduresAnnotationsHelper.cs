using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.StoredProcedures
{
	internal static class StoredProceduresAnnotationsHelper
	{
		internal static readonly StringComparison Comparison = StringComparison.Ordinal;
		internal static readonly StringComparer Comparer = StringComparer.Ordinal;

		private const string AnnotationMarker = "StoredProcedure:";

		internal static bool IsStoredProcedureAnnotation(IAnnotation annotation) => annotation.Name.StartsWith(AnnotationMarker, Comparison);

		internal static string ParseAnnotationName(IAnnotation annotation) => IsStoredProcedureAnnotation(annotation) ? annotation.Name.Substring(AnnotationMarker.Length) : null;

		internal static void AddStoredProcedureAnnotations(IMutableModel model, Assembly assembly)
		{
			var extensionMethodClasses = assembly.GetTypes().Where(t => t.IsClass && t.IsDefined(typeof(ExtensionAttribute)));

			foreach (Type extensionMethodClass in extensionMethodClasses)
			{
				// TODO: check for return value
				var extensionMethods = extensionMethodClass.GetMethods()
					.Where(m => m.IsStatic && m.IsDefined(typeof(ExtensionAttribute)) && model.FindEntityType(m.GetParameters().FirstOrDefault()?.ParameterType) != null);

				foreach (MethodInfo methodInfo in extensionMethods)
				{
					Type entityType = methodInfo.GetParameters()[0].ParameterType;

					var sql = (string)methodInfo.Invoke(null, new object[] { null });

					model.AddAnnotation(BuildAnnotationName(entityType.Name, methodInfo.Name), sql);
				}
			}
		}

		private static string BuildAnnotationName(string typeName, string methodName) => $"{AnnotationMarker}{typeName}_{methodName}";
	}
}