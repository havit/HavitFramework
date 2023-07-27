namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class NavigationTargetTypeStorage : INavigationTargetTypeStorage
{
	/// <inheritdoc />
	public Dictionary<TypePropertyName, Type> Value { get; set; }
}
