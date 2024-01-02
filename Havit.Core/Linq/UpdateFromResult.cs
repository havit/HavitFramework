using System.Collections.Generic;

namespace Havit.Linq;

/// <summary>
/// Output of the UpdateFrom() extension method
/// </summary>
/// <typeparam name="TTarget">type of items in the target collection</typeparam>
public class UpdateFromResult<TTarget>
	where TTarget : class
{
	/// <summary>
	/// Items added to the target collection (missing).
	/// </summary>
	public List<TTarget> ItemsAdding { get; } = new List<TTarget>();

	/// <summary>
	/// Items that are updated in the target collection (existing).
	/// In case of duplicate keys, one item may appear multiple times here - if it is updated by multiple corresponding items from the source collection.
	/// </summary>
	public List<TTarget> ItemsUpdating { get; } = new List<TTarget>();

	/// <summary>
	/// Items that are removed from the target collection (redundant).
	/// </summary>
	public List<TTarget> ItemsRemoving { get; } = new List<TTarget>();

}
