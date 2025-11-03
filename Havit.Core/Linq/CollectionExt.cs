using Havit.Diagnostics.Contracts;

namespace Havit.Linq;

/// <summary>
/// Extension methods for IColllection&lt;T&gt;.
/// </summary>
public static class CollectionExt
{
	/// <summary>
	/// EN: Merges collections. Applies changes from the second collection to the existing collection. Useful for applying changes from ViewModel to DataLayer-entities (and vice versa), for imports, etc.<br/>
	/// CZ: Sloučení kolekcí. Do existující kolekce aplikuje změny z kolekce druhé. Použitelné pro aplikaci změn z ViewModelu do DataLayer-entities (a naopak), pro importy, atp.<br/>
	/// </summary>
	/// <remarks>
	/// EN: <br />
	/// PERF: Internally uses FullOuterJoin for performance optimization, however, due to leaking EF-fixups, the performance is slightly degraded by the <c>target.Contains</c> check before adding a new item to the collection. If this proves to be a problem, it could be improved by overloading the method that can disable this check.
	/// Boundary situations (logic follows from the use of FullOuterJoin):
	/// <list type="bullet">
	/// <item><description>If there is a duplicate in the source collection, the records are applied repeatedly (in an undefined order).</description></item>
	/// <item><description>If there is a duplicate in the target collection, all records are updated.</description></item>
	/// <item><description><c>null</c> keys are paired with each other.</description></item>
	/// </list>
	/// <br/>
	/// CZ: <br />
	/// PERF: Interně používá FullOuterJoin pro výkonovou optimalizaci, nicméně z důvodu leakujících EF-fixups performance trochu degraduje kontrola <c>target.Contains</c> před přidáváním nového prvku do kolekce. Pokud by se ukázalo jako problém, dalo by se tomu pomoci přetížením, které by tuto kontrolu umělo vypnout.
	/// Mezní situace (logika vyplývá z použití FullOuterJoin):
	/// <list type="bullet">
	/// <item><description>Pokud je duplicita ve zdrojové kolekci, aplikují se záznamy opakovaně (v nedefinovaném pořadí).</description></item>
	/// <item><description>Pokud je duplicita v cílové kolekci, aktualizují se všechny záznamy.</description></item>
	/// <item><description>Klíče <c>null</c> se na sebe spárují.</description></item>
	/// </list>
	/// </remarks>
	/// <typeparam name="TSource">
	/// EN: The type of the elements in the collection to be applied.<br/>
	/// CZ: Typ prvků kolekce, kterou chceme aplikovat.<br/>
	/// </typeparam>
	/// <typeparam name="TTarget">
	/// EN: The type of the elements in the target collection to be updated. It must be a class, otherwise we would not be able to set values in the updateItemAction (it would require ref).<br/>
	/// CZ: Typ prvků cílové kolekce, kterou aktualizujeme. Musí to být třída, jinak bychom nemohli v updateItemAction nastavovat hodnoty (bylo by potřeba ref).<br/>
	/// </typeparam>
	/// <typeparam name="TKey">
	/// EN: The type of the matching key.<br/>
	/// CZ: Typ párovacího klíče.<br/>
	/// </typeparam>
	/// <param name="target">
	/// EN: The updated collection.<br/>
	/// CZ: Aktualizovaná kolekce.<br/>
	/// </param>
	/// <param name="source">
	/// EN: The collection with values to be applied.<br/>
	/// CZ: Kolekce s hodnotami, které chceme aplikovat.<br/>
	/// </param>
	/// <param name="targetKeySelector">
	/// EN: The selector for the matching key of the elements in the target collection.<br/>
	/// CZ: Selektor pro párovací klíč prvků cílové kolekce.<br/>
	/// </param>
	/// <param name="sourceKeySelector">
	/// EN: The selector for the matching key of the elements in the source (applied) collection.<br/>
	/// CZ: Selektor pro párovací klíč prvků zdrojové (aplikované) kolekce.<br/>
	/// </param>
	/// <param name="newItemCreateFunc">
	/// EN: The function to create a new element in the target collection (if the function is <c>null</c>, missing elements are ignored and nothing is added to <c>ItemsAdding</c>).<br/>
	/// CZ: Funkce pro založení nového prvku cílové kolekce (pokud bude funkce <c>null</c>, chybějící prvky ignorujeme a do <c>ItemsAdding</c> se nic nepřidá).<br/>
	/// </param>
	/// <param name="updateItemAction">
	/// EN: The action to update the matched element in the target collection with the values from the applied collection (if the action is <c>null</c>, matched elements are not updated and nothing is added to <c>ItemsUpdating</c>).<br/>
	/// CZ: Akce pro aktualizaci spárovaného prvku cílové kolekce z hodnot prvku aplikované kolekce (pokud bude akce <c>null</c>, spárované prvky neaktualizujeme a do <c>ItemsUpdating</c> se nic nepřidá).<br/>
	/// </param>
	/// <param name="removeItemAction">
	/// EN: The action to remove an element from the updated collection that was not found in the applied collection (if the action is <c>null</c>, excess elements are not removed and nothing is added to <c>ItemsRemoving</c>).<br/>
	/// CZ: Akce pro odebrání prvku z aktualizované kolekce, který nebyl nalezen v aplikované kolekci (pokud bude akce <c>null</c>, přebývající prvky neodebíráme a do <c>ItemsRemoving</c> se nic nepřidá).<br/>
	/// </param>
	/// <param name="removeItemFromCollection">
	/// EN: Indicates whether the excess element should be removed from the collection (default is <c>true</c>, for support of soft-deletes, <c>false</c> can be specified). Only makes sense if <c>removeItemAction</c> is not null, otherwise it is ignored.<br/>
	/// CZ: indikuje, zdali má být přebývající prvek z kolekce odebrán (default <c>true</c>, pro podporu soft-deletes je možno zadat <c>false</c>). Má smysl, pouze pokud není <c>removeItemAction</c> null, jinak se ignoruje.<br/>
	/// </param>
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

	/// <inheritdoc cref="UpdateFrom" />
	public static async Task<UpdateFromResult<TTarget>> UpdateFromAsync<TSource, TTarget, TKey>(
		this ICollection<TTarget> target,
		IEnumerable<TSource> source,
		Func<TTarget, TKey> targetKeySelector,
		Func<TSource, TKey> sourceKeySelector,
		Func<TSource, Task<TTarget>> newItemCreateFunc,
		Func<TSource, TTarget, Task> updateItemAction,
		Func<TTarget, Task> removeItemAction,
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
					var newTargetItem = await newItemCreateFunc(joinedItem.Source).ConfigureAwait(false);
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
					await updateItemAction(joinedItem.Source, joinedItem.Target).ConfigureAwait(false);
					result.ItemsUpdating.Add(joinedItem.Target);
				}
			}
			else // (Source == null) && (Target != null)
			{
				if (removeItemAction != null)
				{
					await removeItemAction(joinedItem.Target).ConfigureAwait(false);
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
