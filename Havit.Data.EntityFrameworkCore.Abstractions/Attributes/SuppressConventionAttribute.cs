namespace Havit.Data.EntityFrameworkCore.Attributes;

/// <summary>
/// Slouží k označení konvenve jako potlačené.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class SuppressConventionAttribute : Attribute
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
		ConventionIdentifierToSuppress = conventionIdentifierToSuppress;
	}
}
