namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class NavigationTargetStorage : INavigationTargetStorage
{
	/// <inheritdoc />
	public required FrozenDictionary<TypePropertyName, NavigationTarget> Value { get; init; }
}
