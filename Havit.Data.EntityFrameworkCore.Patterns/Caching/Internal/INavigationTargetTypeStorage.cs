namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Úložiště dat pro <see cref="NavigationTargetTypeStorage"/>.
/// </summary>
public interface INavigationTargetTypeStorage
{
	/// <summary>
	/// Úložiště dat pro <see cref="NavigationTargetTypeStorage"/>.
	/// </summary>
	Dictionary<TypePropertyName, Type> Value { get; set; }
}
