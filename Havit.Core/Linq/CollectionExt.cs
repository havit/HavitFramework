using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;

namespace Havit.Linq;

/// <summary>
/// Extension methods for IColllection&lt;T&gt;.
/// </summary>
public static class CollectionExt
{
	/// <summary>
	/// Merges collections. Applies changes from the second collection to the existing collection.
	/// Useful for applying changes from ViewModel to DataLayer-entities (and vice versa), for imports, etc.
	/// </summary>
	/// <remarks>
	/// PERF: Internally uses FullOuterJoin for performance optimization, however, due to leaking EF-fixups, the performance is slightly degraded by the <c>target.Contains</c> check before adding a new item to the collection. If this proves to be a problem, it could be improved by overloading the method that can disable this check.
	/// Boundary situations (logic follows from the use of FullOuterJoin):
	/// - If there is a duplicate in the source collection, the records are applied repeatedly (in an undefined order).
	/// - If there is a duplicate in the target collection, all records are updated.
	/// - <c>null</c> keys are paired with each other.
	/// </remarks>
	/// <typeparam name="TSource">The type of the elements in the collection to be applied.</typeparam>
	/// <typeparam name="TTarget">The type of the elements in the target collection to be updated. It must be a class, otherwise we would not be able to set values in the updateItemAction (it would require ref).</typeparam>
	/// <typeparam name="TKey">The type of the matching key.</typeparam>
	/// <param name="target">The updated collection.</param>
	/// <param name="source">The collection with values to be applied.</param>
	/// <param name="targetKeySelector">The selector for the matching key of the elements in the target collection.</param>
	/// <param name="sourceKeySelector">The selector for the matching key of the elements in the source (applied) collection.</param>
	/// <param name="newItemCreateFunc">The function to create a new element in the target collection (if the function is <c>null</c>, missing elements are ignored and nothing is added to <c>ItemsAdding</c>).</param>
	/// <param name="updateItemAction">The action to update the matched element in the target collection with the values from the applied collection (if the action is <c>null</c>, matched elements are not updated and nothing is added to <c>ItemsUpdating</c>).</param>
	/// <param name="removeItemAction">The action to remove an element from the updated collection that was not found in the applied collection (if the action is <c>null</c>, excess elements are not removed and nothing is added to <c>ItemsRemoving</c>)</param>
	/// <param name="removeItemFromCollection">Indicates whether the excess element should be removed from the collection (default is <c>true</c>, for support of soft-deletes, <c>false</c> can be specified). Only makes sense if <c>removeItemAction</c> is not null, otherwise it is ignored.</param>
	public static UpdateFromResult<TTarget> UpdateFrom<TSource, TTarget, TKey>(
		this ICollection<TTarget> target,
		IEnumerable<TSource> source,
		Func<TTarget, TKey> targetKeySelector,
		Func<TSource, TKey> sourceKeySelector,
		Func<TSource, TTarget> newItemCreateFunc,
		Action<TSource, TTarget> updateItemAction,
		Action<TTarget> removeItemAction,
		bool removeItemFromCollection = true)
		where TTarget : class
	{
		Contract.Requires<ArgumentNullException>(target != null, nameof(target));
		Contract.Requires<ArgumentNullException>(source != null, nameof(source));
		Contract.Requires<ArgumentNullException>(sourceKeySelector != null, nameof(sourceKeySelector));
		Contract.Requires<ArgumentNullException>(targetKeySelector != null, nameof(targetKeySelector));

		var joinedCollections = target.FullOuterJoin(
			rightSource: source,
			leftKeySelector: targetKeySelector,
			rightKeySelector: sourceKeySelector,
			resultSelector: (targetItem, sourceItem) => new { Target = targetItem, Source = sourceItem });

		var result = new UpdateFromResult<TTarget>();

		foreach (var joinedItem in joinedCollections)
		{
			if (object.Equals(joinedItem.Target, default(TTarget))) // && (Source != null)
			{
				// new item
				if (newItemCreateFunc != null)
				{
					var originalCount = target.Count;
					var newTargetItem = newItemCreateFunc(joinedItem.Source);
					// leaking EF fixup hell workaround :-((
					// PERF: If the number of items did not change, we interpret it as no one else added a new item to the collection.
					// We rely on the fact that the vast majority of ICollection.Count implementations are O(1).
					// (Unfortunately, with the knowledge that this is not a 100% reliable shortcut - another item could have been added and/or removed)
					if ((target.Count == originalCount)
						|| !target.Contains(newTargetItem))
					{
						target.Add(newTargetItem);
					}
					result.ItemsAdding.Add(newTargetItem);
				}
			}
			else if (!object.Equals(joinedItem.Source, default(TSource))) // && (Target != null)
			{
				// existing item
				if (updateItemAction != null)
				{
					updateItemAction(joinedItem.Source, joinedItem.Target);
					result.ItemsUpdating.Add(joinedItem.Target);
				}
			}
			else // (Source == null) && (Target != null)
			{
				if (removeItemAction != null)
				{
					removeItemAction(joinedItem.Target);
					result.ItemsRemoving.Add(joinedItem.Target);
					if (removeItemFromCollection)
					{
						target.Remove(joinedItem.Target);
					}
				}
			}
		}

		return result;
	}
}
