using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Collections;

/// <summary>
/// Compares the values of properties of two objects. Property names are provided and compared in the specified order.
/// Property names can be composite: for example, "Book.Author.LastName".
/// The property must implement IComparable.
/// </summary>
/// <typeparam name="T">The type of the object whose values are being compared.</typeparam>
public class GenericPropertyComparer<T> : IComparer<T>
{
	private readonly IList<SortItem> sortItems;
	private readonly Dictionary<object, IComparable>[] getValueCacheList;

	/// <summary>
	/// Creates an instance of the comparer for sorting by the specified property.
	/// </summary>
	/// <param name="sortItem">Specifies the sort parameter.</param>
	public GenericPropertyComparer(SortItem sortItem) : this(new SortItem[] { sortItem })
	{
	}

	/// <summary>
	/// Creates an instance of the comparer for sorting by the collection of properties.
	/// </summary>
	/// <param name="sortItems">Specifies the sort parameters.</param>
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
	/// Compares the properties of two objects.
	/// </summary>
	/// <param name="x">The first object to compare.</param>
	/// <param name="y">The second object to compare.</param>
	/// <returns>-1, 0, 1 - as Compare(T, T)</returns>
	public int Compare(T x, T y)
	{
		return Compare(x, y, 0);
	}

	/// <summary>
	/// Compares the properties of two objects. Compares the index-th property specified in the sortItemCollection field.
	/// </summary>
	/// <param name="x">The first object to compare.</param>
	/// <param name="y">The second object to compare.</param>
	/// <param name="index">The index of the property to compare.</param>
	/// <returns>-1, 0, 1 - as Compare(T, T)</returns>
	private int Compare(object x, object y, int index)
	{
		if (index >= sortItems.Count)
		{
			return 0;
		}

		/* written a bit more complicated - for clarity */
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
			// both null -> equal
			result = 0;
		}
		else if (value1 == null)
		{
			// value1 is null (value2 is not null), then value1 < value2
			result = -1;
		}
		else if (value2 == null)
		{
			// value2 is null (value1 is not null), then value2 < value1
			result = 1;
		}
		else /*if (value1 != null || value2 != null)*/
		{
			// neither is null -> compare
			result = value1.CompareTo(value2);
		}

		return (result == 0) ? Compare(x, y, index + 1) : result;
	}

	/// <summary>
	/// Returns the value of the index-th property of the object.
	/// If the value of this property is DBNull.Value, returns null.
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

			if (value == DBNull.Value) // for comparison purposes, we will assume that null and DBNull.Value are the same (i.e., null).
			{
				value = null;
			}
			result = (IComparable)value;

			getValueCache.Add(obj, result);
			return result;
		}
	}
}
