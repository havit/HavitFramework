using Havit.Data.EntityFrameworkCore.Attributes;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

/// <summary>
/// Extension metody umožňující označit v modelu požadavek na potlačení určité konvence.
/// </summary>
public static class ConventionSuppressionsExtensions
{
	/// <summary>
	/// Vrací true, pokud je konvence na daném objektu modelu potlačena.
	/// </summary>
	public static bool IsConventionSuppressed(this IReadOnlyEntityType entityType, string conventionIdentifier)
	{
		return entityType.ClrType.IsConventionSuppressed(conventionIdentifier);
	}

	/// <summary>
	/// Vrací true, pokud je konvence na dané vlastnosti modelu potlačena.
	/// </summary>
	public static bool IsConventionSuppressed(this IReadOnlyProperty property, string conventionIdentifier)
	{
		return !property.IsShadowProperty() && property.PropertyInfo.IsConventionSuppressed(conventionIdentifier);
	}

	/// <summary>
	/// Vrací true, pokud je konvence potlačena na daném customAttributeProvideru.
	/// </summary>
	private static bool IsConventionSuppressed(this ICustomAttributeProvider customAttributeProvider, string conventionIdentifier)
	{
		return customAttributeProvider.GetCustomAttributes(typeof(SuppressConventionAttribute), true).Cast<SuppressConventionAttribute>().Any(attribute => attribute.ConventionIdentifierToSuppress == conventionIdentifier);
	}
}
