using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Conventions
{
	/// <summary>
	/// Extension metody umožňující označit v modelu požadavek na potlačení určité konvence.
	/// </summary>
	public static class ConventionSuppressionsExtensions
	{
		/// <summary>
		/// Vrací jen ty objekty modelu, které nemají danou konvenci potlačenu.
		/// </summary>
		public static IEnumerable<TSource> WhereNotConventionSuppressed<TSource>(this IEnumerable<TSource> source, Type conventionType)
			where TSource : IAnnotatable
		{
			ValidateConventionType(conventionType);
			return source.Where(annotatable => !annotatable.IsConventionSuppressed(conventionType));
		}

		/// <summary>
		/// Vrací true, pokud je konvence na daném objektu modelu potlačena.
		/// </summary>
		public static bool IsConventionSuppressed<TConvention>(this IAnnotatable annotatable)
			where TConvention : IModelConvention
		{
			return annotatable.IsConventionSuppressed(typeof(TConvention));
		}

		/// <summary>
		/// Vrací true, pokud je konvence na daném objektu modelu potlačena.
		/// </summary>
		public static bool IsConventionSuppressed(this IAnnotatable annotatable, Type conventionType)
		{
			ValidateConventionType(conventionType);
			return annotatable.FindAnnotation(GetAnnotationName(conventionType)) != null;
		}


		/// <summary>
		/// Označí konvenci na entitě jako potlačenou.
		/// </summary>
		public static void HasConventionSuppressed<TConvention>(this EntityTypeBuilder entityTypeBuilder)
			where TConvention : IModelConvention
		{
			entityTypeBuilder.HasAnnotation(GetAnnotationName(typeof(TConvention)), "");
		}

		/// <summary>
		/// Označí konvenci na vlastnosti jako potlačenou.
		/// </summary>
		public static void HasConventionSuppressed<TConvention>(this PropertyBuilder propertyBuilder)
			where TConvention : IModelConvention
		{
			propertyBuilder.HasAnnotation(GetAnnotationName(typeof(TConvention)), "");
		}

		private static void ValidateConventionType(Type conventionType)
		{
			if (!conventionType.GetInterfaces().Any(interfaceType => interfaceType == typeof(IModelConvention)))
			{
				throw new InvalidOperationException($"Type {conventionType.FullName} does not implement IModelConvention.");
			}
		}

		private static string GetAnnotationName(Type conventionType)
		{
			return "ConventionSuppression:" + conventionType.FullName;
		}
	}
}
