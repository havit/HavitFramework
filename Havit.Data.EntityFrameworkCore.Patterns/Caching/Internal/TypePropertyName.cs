namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Evidence typu entity a názvu vlastnosti pro použití jako klíč v Dictionary (<see cref="NavigationTargetService"/>).
/// </summary>
public record struct TypePropertyName
{
	/// <summary>
	/// Typ entity.
	/// </summary>
	public required Type Type { get; init; }

	/// <summary>
	/// Vlastnost.
	/// </summary>
	public required string PropertyName { get; init; }
}