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
		/// Interně používá FullOuterJoin pro výkonovou optimalizaci.
		/// Mezní situace (logika vyplývá z použití FullOuterJoin):
		/// - Pokud je duplicita ve zdrojové kolekci, aplikují se záznamy opakovaně (v nedefinovaném pořadí).
		/// - Pokud je duplicita v cílové kolekci, aktualizují se všechny záznamy.
		/// - Klíče <c>null</c> se na sebe spárují.
		/// </remarks>
		/// <typeparam name="TSource">typ prvků kolekce, kterou chceme aplikovat</typeparam>
		/// <typeparam name="TTarget">typ prvků cílové kolekce, kterou aktualizujeme</typeparam>
		/// <typeparam name="TKey">typ párovacího klíče</typeparam>
		/// <param name="target">aktualizovaná kolekce</param>
		/// <param name="source">kolekece s hodnotami, které chceme aplikovat</param>
		/// <param name="targetKeySelector">selektor pro párovací klíč prvků cílové kolekce</param>
		/// <param name="sourceKeySelector">selektor pro párovací klíč prvků zdrojové (aplikované) kolekce</param>
		/// <param name="newItemCreateFunc">funkce pro založení nového prvku cílové kolekce (pokud bude funkce <c>null</c>, chybějící prvky ignorujeme)</param>
		/// <param name="updateItemAction">akce pro aktualizaci spárovaného prvku cílové kolekce z hodnot prvku aplikované kolekce (pokud bude akce <c>null</c>, spárované prvky neaktualizujeme)</param>
		/// <param name="removeItemAction">akce pro odebrání prvku z aktualizované kolekce, který nebyl nalezen v aplikované kolekci (pokud bude akce <c>null</c>, přebývající prvky neodebíráme) </param>
		public static void UpdateFrom<TSource, TTarget, TKey>(
			this ICollection<TTarget> target,
			IEnumerable<TSource> source,
			Func<TTarget, TKey> targetKeySelector,
			Func<TSource, TKey> sourceKeySelector,
			Func<TSource, TTarget> newItemCreateFunc,
			Action<TSource, TTarget> updateItemAction,
			Action<TTarget> removeItemAction)
		{
			Contract.Requires<ArgumentNullException>(target != null);
			Contract.Requires<ArgumentNullException>(source != null);
			Contract.Requires<ArgumentNullException>(sourceKeySelector != null);
			Contract.Requires<ArgumentNullException>(targetKeySelector != null);

			var joinedCollections = target.FullOuterJoin(
				rightSource: source,
				leftKeySelector: targetKeySelector,
				rightKeySelector: sourceKeySelector,
				resultSelector: (targetItem, sourceItem) => new { Target = targetItem, Source = sourceItem });

			foreach (var joinedItem in joinedCollections)
			{
				if (joinedItem.Target == null) // && (Source != null)
				{
					// new item
					if (newItemCreateFunc != null)
					{
						var newTargetItem = newItemCreateFunc(joinedItem.Source);
						target.Add(newTargetItem);
					}
				}
				else if (joinedItem.Source != null) // && (Target != null)
				{
					// existing item
					if (updateItemAction != null)
					{
						updateItemAction(joinedItem.Source, joinedItem.Target);
					}
				}
				else // (Source == null) && (Target != null)
				{
					if (removeItemAction != null)
					{
						removeItemAction(joinedItem.Target);
						target.Remove(joinedItem.Target);
					}

				}
			}
		}
	}
}
