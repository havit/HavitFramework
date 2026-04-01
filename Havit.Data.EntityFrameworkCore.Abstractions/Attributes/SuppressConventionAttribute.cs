namespace Havit.Data.EntityFrameworkCore.Attributes;

/// <summary>
/// Slouží k označení konvence jako potlačené.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public sealed class SuppressConventionAttribute : Attribute
{
	/// <summary>
	/// Potlačená konvence (resp. její typ).
	/// </summary>
	public string ConventionIdentifierToSuppress { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public SuppressConventionAttribute(string conventionIdentifierToSuppress)
	{
		ArgumentNullException.ThrowIfNull(conventionIdentifierToSuppress);
		ConventionIdentifierToSuppress = conventionIdentifierToSuppress;
	}
}
