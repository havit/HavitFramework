using Havit.Data.EntityFrameworkCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure
{
	/// <summary>
	/// Extension metody umožňující označit v modelu požadavek na potlačení určité konvence.
	/// </summary>
	public static class ConventionSuppressionsExtensions
	{
		/// <summary>
		/// Vrací true, pokud je konvence na daném objektu modelu potlačena.
		/// </summary>
		public static bool IsConventionSuppressed<TConvention>(this IEntityType entityType)
		{
			return entityType.ClrType.IsConventionSuppressed<TConvention>();
		}

		/// <summary>
		/// Vrací true, pokud je konvence na dané vlastnosti modelu potlačena.
		/// </summary>
		public static bool IsConventionSuppressed<TConvention>(this IProperty property)
		{
			return !property.IsShadowProperty() && property.PropertyInfo.IsConventionSuppressed<TConvention>();
		}

		/// <summary>
		/// Vrací true, pokud je konvence potlačena na daném customAttributeProvideru.
		/// </summary>
		private static bool IsConventionSuppressed<TConvention>(this ICustomAttributeProvider customAttributeProvider)
		{
			return customAttributeProvider.GetCustomAttributes(typeof(SuppressConventionAttribute), true).Cast<SuppressConventionAttribute>().Any(attribute => attribute.ConventionTypeToSuppress == typeof(TConvention));
		}
	}
}
