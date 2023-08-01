namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class NavigationTargetStorage : INavigationTargetStorage
{
	/// <inheritdoc />
	public Dictionary<TypePropertyName, NavigationTarget> Value { get; set; }
}
