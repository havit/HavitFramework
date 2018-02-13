using System.Collections.Generic;

namespace Havit.Linq
{
	/// <summary>
	/// Výstup extension-metody UpdateFrom()
	/// </summary>
	/// <typeparam name="TTarget">typ prvků cílové kolekce</typeparam>
	public class UpdateFromResult<TTarget>
		where TTarget : class
	{
		/// <summary>
		/// Prvky přidávané do cílové kolekce (chybějící).
		/// </summary>
		public List<TTarget> ItemsAdding { get; } = new List<TTarget>();

		/// <summary>
		/// Prvky, které se v cílové kolekci aktualizují (existující).
		/// Při duplicitách klíčů se zde může objevit jeden prvek vícekrát - pokud je aktualizován více odpovídajícími prvky zdrojové kolekce.
		/// </summary>
		public List<TTarget> ItemsUpdating { get; } = new List<TTarget>();

		/// <summary>
		/// Prvky, které se z cílové kolekce odebírají (přebývající).
		/// </summary>
		public List<TTarget> ItemsRemoving { get; } = new List<TTarget>();

	}
}