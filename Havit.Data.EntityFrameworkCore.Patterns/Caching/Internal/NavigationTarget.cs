namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Informace o cíli navigace.
/// </summary>
public record class NavigationTarget
{
	/// <summary>
	/// Cílový typ. V případě kolekce jde o třídu kolekce.
	/// </summary>
	public required Type TargetClrType { get; init; }

	/// <summary>
	/// Indukuje, zda jde o kolekci.
	/// </summary>
	public required NavigationType NavigationType { get; init; }
}
