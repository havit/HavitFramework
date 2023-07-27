namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Evidence typu entity a názvu vlastnosti pro použití jako klíč v Dictionary (<see cref="NavigationTargetTypeService"/>).
/// </summary>
/// <param name="Type">
/// Typ entity.
/// </param>
/// <param name="PropertyName">
/// Vlastnost.
/// </param>
public record TypePropertyName(Type Type, string PropertyName);