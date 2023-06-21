namespace Havit.Data.EntityFrameworkCore.Attributes;

/// <summary>
/// Priorita položky v cache.
/// </summary>
public enum CacheItemPriority
{
	/// <summary>
	/// Low.
	/// </summary>
	Low,

	/// <summary>
	/// Normal.
	/// </summary>
	Normal,

	/// <summary>
	/// High.
	/// </summary>
	High,

	/// <summary>
	/// NotRemovable.
	/// </summary>
	NotRemovable
}
