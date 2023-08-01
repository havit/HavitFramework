namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Úložiště dat pro <see cref="NavigationTargetStorage"/>.
/// </summary>
public interface INavigationTargetStorage
{
	/// <summary>
	/// Úložiště dat pro <see cref="NavigationTargetStorage"/>.
	/// </summary>
	Dictionary<TypePropertyName, NavigationTarget> Value { get; set; }
}
