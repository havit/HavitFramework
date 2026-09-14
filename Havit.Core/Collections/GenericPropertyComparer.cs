using System.Diagnostics.CodeAnalysis;

namespace Havit.Collections;

/// <summary>
/// Compares the values of properties of two objects. Property names are provided and compared in the specified order.
/// Property names can be composite: for example, "Book.Author.LastName".
/// The property must implement IComparable.
/// </summary>
/// <remarks>
/// The comparer caches the looked-up property values per compared object instance to speed up repeated comparisons during a single sort.
/// Consequences:
/// <list type="bullet">
/// <item><description>The instance is <b>not thread-safe</b>; do not use a single instance from multiple threads concurrently.</description></item>
/// <item><description>The instance keeps references to all compared objects (and their resolved values) for its whole lifetime. Use a short-lived instance (typically one per sort) so the objects can be garbage collected.</description></item>
/// <item><description>The cache assumes the property values of the compared objects do not change while the comparer is in use.</description></item>
/// </list>
/// </remarks>
/// <typeparam name="T">The type of the object whose values are being compared.</typeparam>
public class GenericPropertyComparer<T> : IComparer<T>
{
	// The RequiresUnreferencedCode annotation is placed on the constructors, not on Compare - Compare implements IComparer<T>.Compare,
	// which is not annotated, so the annotation cannot be put there (IL2046).
	private const string SortItemLookupIsNotTrimCompatibleMessage = "The sorted properties are resolved by name using DataBinderExt (reflection over the runtime type of the compared objects). Those members might be removed when trimming.";

	private readonly IList<SortItem> sortItems;
	private readonly Dictionary<object, IComparable>[] getValueCacheList;

	/// <summary>
	/// Creates an instance of the comparer for sorting by the specified property.
	/// </summary>
	/// <param name="sortItem">Specifies the sort parameter.</param>
	[RequiresUnreferencedCode(SortItemLookupIsNotTrimCompatibleMessage)]
	public GenericPropertyComparer(SortItem sortItem) : this(new SortItem[] { sortItem })
	{
	}

	/// <summary>
	/// Creates an instance of the comparer for sorting by the collection of properties.
	/// </summary>
	/// <param name="sortItems">Specifies the sort parameters.</param>
	[RequiresUnreferencedCode(SortItemLookupIsNotTrimCompatibleMessage)]
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
		// iterate the sort items (instead of recursing per item) - falls through to the next item only on equality
		for (int index = 0; index < sortItems.Count; index++)
		{
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

			int result;

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

			if (result != 0)
			{
				return result;
			}
		}

		return 0;
	}

	/// <summary>
	/// Returns the value of the index-th property of the object.
	/// If the value of this property is DBNull.Value, returns null.
	/// </summary>
	[UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "An instance can only be created through a constructor annotated with RequiresUnreferencedCode.")]
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
