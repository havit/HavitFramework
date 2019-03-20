using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;

namespace Havit.Linq
{
	/// <summary>
	/// Extension metody pro IColllection&lt;T&gt;.
	/// </summary>
	public static class CollectionExt
	{
		/// <summary>
		/// Sloučení kolekcí. Do existující kolekce aplikuje změny z kolekce druhé.
		/// Použitelné pro aplikaci změn z ViewModelu do DataLayer-entities (a naopak), pro importy, atp.
		/// </summary>
		/// <remarks>
		/// PERF: Interně používá FullOuterJoin pro výkonovou optimalizaci, nicméně z důvodu leakujících EF-fixups performance trochu degraduje kontrola <c>target.Contains</c> před přidáváním nového prvku do kolekce. Pokud by se ukázalo jako problém, dalo by se tomu pomoci přetížením, které by tuto kontrolu umělo vypnout.
		/// Mezní situace (logika vyplývá z použití FullOuterJoin):
		/// - Pokud je duplicita ve zdrojové kolekci, aplikují se záznamy opakovaně (v nedefinovaném pořadí).
		/// - Pokud je duplicita v cílové kolekci, aktualizují se všechny záznamy.
		/// - Klíče <c>null</c> se na sebe spárují.
		/// </remarks>
		/// <typeparam name="TSource">typ prvků kolekce, kterou chceme aplikovat</typeparam>
		/// <typeparam name="TTarget">typ prvků cílové kolekce, kterou aktualizujeme. Musí to být třída, jinak bychom nemohli v updateItemAction nastavovat hodnoty (bylo by potřeba ref).</typeparam>
		/// <typeparam name="TKey">typ párovacího klíče</typeparam>
		/// <param name="target">aktualizovaná kolekce</param>
		/// <param name="source">kolekece s hodnotami, které chceme aplikovat</param>
		/// <param name="targetKeySelector">selektor pro párovací klíč prvků cílové kolekce</param>
		/// <param name="sourceKeySelector">selektor pro párovací klíč prvků zdrojové (aplikované) kolekce</param>
		/// <param name="newItemCreateFunc">funkce pro založení nového prvku cílové kolekce (pokud bude funkce <c>null</c>, chybějící prvky ignorujeme a do <c>ItemsAdding</c> se nic nepřidá)</param>
		/// <param name="updateItemAction">akce pro aktualizaci spárovaného prvku cílové kolekce z hodnot prvku aplikované kolekce (pokud bude akce <c>null</c>, spárované prvky neaktualizujeme a do <c>ItemsUpdating</c> se nic nepřidá)</param>
		/// <param name="removeItemAction">akce pro odebrání prvku z aktualizované kolekce, který nebyl nalezen v aplikované kolekci (pokud bude akce <c>null</c>, přebývající prvky neodebíráme a do <c>ItemsRemoving</c> se nic nepřidá) </param>
		/// <param name="removeItemFromCollection">indikuje, zdali má být přebývající prvek z kolekce odebrán (default <c>true</c>, pro podporu soft-deletes je možno zadat <c>false</c>)</param>
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
			Contract.Requires<ArgumentNullException>(target != null);
			Contract.Requires<ArgumentNullException>(source != null);
			Contract.Requires<ArgumentNullException>(sourceKeySelector != null);
			Contract.Requires<ArgumentNullException>(targetKeySelector != null);
			Contract.Requires<InvalidOperationException>(removeItemFromCollection || (removeItemAction != null));

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
						// PERF: Pokud se počet prvků nezměnil, interpretujeme to tak, že do kolekce nový prvek "někdo jiný" nepřidal.
						// Spoléháme na to, že drtivá většina implementací ICollection.Count je O(1).
						// (Bohužel i s vědomím, že to není 100% spolehlivá zkratka - mohl být přidán a/nebo odebrán jiný prvek)
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
}
