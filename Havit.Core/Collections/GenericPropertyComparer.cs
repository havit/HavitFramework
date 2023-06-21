using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Collections;

/// <summary>
/// Porovnává hodnoty vlastností dvou objektů. Názvy vlastností jsou dodány, porovnávají se v dodaném pořadí.
/// Názvy vlastností mohou být složené: např. "Kniha.Autor.Prijmeni".
/// Property musí implementovat IComparable.
/// </summary>
/// <typeparam name="T">Typ objektu, jehož hodnoty jsou porovnávány.</typeparam>
public class GenericPropertyComparer<T> : IComparer<T>
{
	private readonly IList<SortItem> sortItems;
	private readonly Dictionary<object, IComparable>[] getValueCacheList;

	/// <summary>
	/// Vytvoří instanci compareru pro řazení dle dané property.
	/// </summary>
	/// <param name="sortItem">Určuje parametr řazení.</param>
	public GenericPropertyComparer(SortItem sortItem) : this(new SortItem[] { sortItem })
	{
	}

	/// <summary>
	/// Vytvoří instanci compareru pro řazení dle kolekce vlastností.
	/// </summary>
	/// <param name="sortItems">Určuje parametry řazení.</param>
	public GenericPropertyComparer(IList<SortItem> sortItems)
	{
		this.sortItems = sortItems;
		this.getValueCacheList = new Dictionary<object, IComparable>[sortItems.Count];
		for (int i = 0; i < sortItems.Count; i++)
		{
			getValueCacheList[i] = new Dictionary<object, IComparable>();
		}
	}

	/// <summary>
	/// Porovná vlastnosti instancí dvou objektů.
	/// </summary>
	/// <param name="x">První porovnávaný objekt.</param>
	/// <param name="y">Druhý porovnávaný objekt.</param>
	/// <returns>-1, 0, 1 - jako Compare(T, T)</returns>
	public int Compare(T x, T y)
	{
		return Compare(x, y, 0);
	}

	/// <summary>
	/// Porovná vlastnosti instancí dvou objektů. Porovnávají se index-té vlastnosti uvedené ve fieldu sortItemCollection.
	/// </summary>
	/// <param name="x">První porovnávaný objekt.</param>
	/// <param name="y">Druhý porovnávaný objekt.</param>
	/// <param name="index">Index porovnávané vlastnosti.</param>
	/// <returns>-1, 0, 1 - jako Compare(T, T)</returns>
	private int Compare(object x, object y, int index)
	{
		if (index >= sortItems.Count)
		{
			return 0;
		}

		/* napsáno trochu komplikovaněji - pro přehlednost */
		IComparable value1;
		IComparable value2;
		if (sortItems[index].Direction == SortDirection.Ascending)
		{
			value1 = GetValue(x, index);
			value2 = GetValue(y, index);
		}
		else
		{
			value2 = GetValue(x, index);
			value1 = GetValue(y, index);
		}

		int result = 0;

		if (value1 == null && value2 == null)
		{
			// oboji null -> stejne
			result = 0;
		}
		else if (value1 == null)
		{
			// value1 je null (value2 neni null), potom value1 < value2
			result = -1;
		}
		else if (value2 == null)
		{
			// value2 je null (value1 neni null), potom value2 < value1
			result = 1;
		}
		else /*if (value1 != null || value2 != null)*/
		{
			// ani jedno neni null -> porovname
			result = value1.CompareTo(value2);
		}

		return (result == 0) ? Compare(x, y, index + 1) : result;
	}

	/// <summary>
	/// Vrátí hodnot index-té property objektu.
	/// Pokud je hodnota této property DBNull.Value, vrací null.
	/// </summary>
	private IComparable GetValue(object obj, int index)
	{
		if ((obj == null) || (obj == DBNull.Value))
		{
			return null;
		}

		Dictionary<object, IComparable> getValueCache = getValueCacheList[index];

		IComparable result;
		if (getValueCache.TryGetValue(obj, out result))
		{
			return result;
		}
		else
		{
			object value = DataBinderExt.GetValue(obj, sortItems[index].Expression);

			if (value == DBNull.Value) // pro účely srovnání budeme tvrdit, že null a DBNull.Value jsou shodné (tedy null).
			{
				value = null;
			}
			result = (IComparable)value;

			getValueCache.Add(obj, result);
			return result;
		}
	}
}