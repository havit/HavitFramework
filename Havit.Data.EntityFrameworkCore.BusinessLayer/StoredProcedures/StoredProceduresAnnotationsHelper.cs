using System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.StoredProcedures
{
	internal static class StoredProceduresAnnotationsHelper
	{
		internal static readonly StringComparison Comparison = StringComparison.Ordinal;
		internal static readonly StringComparer Comparer = StringComparer.Ordinal;

		private const string AnnotationMarker = "StoredProcedure:";

		internal static bool IsStoredProcedureAnnotation(IAnnotation annotation) => annotation.Name.StartsWith(AnnotationMarker, Comparison);

		internal static string ParseAnnotationName(IAnnotation annotation) => IsStoredProcedureAnnotation(annotation) ? annotation.Name.Substring(AnnotationMarker.Length) : null;
	}
}