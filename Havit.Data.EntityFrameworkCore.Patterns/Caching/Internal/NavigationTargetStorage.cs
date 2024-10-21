namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class NavigationTargetStorage : INavigationTargetStorage
{
	/// <inheritdoc />
	public FrozenDictionary<TypePropertyName, NavigationTarget> Value { get; set; }
}
