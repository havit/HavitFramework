
namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions;

/// <summary>
/// Konfigurace použití konvencí ve frameworku.
/// </summary>
public sealed record ConventionsOptions
{
	/// <summary>
	/// Indikuje používání konvence CascadeDeleteToRestrictConvention.
	/// Výchozí hodnota je true.
	/// </summary>
	public bool CascadeDeleteToRestrictConventionEnabled { get; init; } = true;

	/// <summary>
	/// Indikuje používání konvence CacheAttributeToAnnotationConvention.
	/// Výchozí hodnota je true.
	/// </summary>
	public bool CacheAttributeToAnnotationConventionEnabled { get; init; } = true;

	/// <summary>
	/// Indikuje používání konvence DataTypeAttributeConvention.
	/// Výchozí hodnota je true.
	/// </summary>
	public bool DataTypeAttributeConventionEnabled { get; init; } = true;

	/// <summary>
	/// Indikuje používání konvence StringPropertiesDefaultValueConvention.
	/// Výchozí hodnota je false.
	/// </summary>
	public bool StringPropertiesDefaultValueConventionEnabled { get; init; } = false;

	/// <summary>
	/// Indikuje používání konvence ManyToManyEntityKeyDiscoveryConvention.
	/// Výchozí hodnota je true.
	/// </summary>
	public bool ManyToManyEntityKeyDiscoveryConventionEnabled { get; init; } = true;

	/// <summary>
	/// Indikuje používání konvence LocalizationTableIndexConvention.
	/// Výchozí hodnota je true.
	/// </summary>
	public bool LocalizationTableIndexConventionEnabled { get; init; } = true;

}

