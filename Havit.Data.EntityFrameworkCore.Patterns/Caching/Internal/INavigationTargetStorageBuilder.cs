namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Sestaví (I)NavigationTargetStorage.
/// </summary>
public interface INavigationTargetStorageBuilder
{
	/// <summary>
	/// Sestaví (I)NavigationTargetStorage.
	/// </summary>
	INavigationTargetStorage Build();
}