using System.Collections;
using Havit.Diagnostics.Contracts;

namespace Havit.Collections;

/// <summary>
/// Helper class for sorting.
/// </summary>
public static class SortHelper
{
	/// <summary>
	/// Returns data sorted by properties in sortItemCollection.
	/// If sortItemCollection is an empty collection, it returns the data parameter.
	/// </summary>
	/// <param name="data">Data to be sorted.</param>
	/// <param name="sortItemCollection">Instructions on how to sort.</param>
	/// <returns>Sorted data.</returns>
	public static IEnumerable PropertySort(IEnumerable data, SortItemCollection sortItemCollection)
	{
		Contract.Requires<ArgumentNullException>(data != null, nameof(data));
		Contract.Requires<ArgumentNullException>(sortItemCollection != null, nameof(sortItemCollection));

		if (sortItemCollection.Count == 0)
		{
			return data;
		}

		// copy the data to a different structure (capacity hint when the count is cheaply known to avoid repeated internal array growth)
		List<object> dataList = (data is ICollection collection)
			? new List<object>(collection.Count)
			: new List<object>();
		foreach (object o in data)
		{
			dataList.Add(o);
		}
		// sort the data
		dataList.Sort(new GenericPropertyComparer<object>(sortItemCollection));
		// perform databinding on the sorted data
		return dataList;
	}

	/// <summary>
	/// Returns data sorted by property.
	/// </summary>
	/// <param name="data">Data to be sorted.</param>
	/// <param name="property">Property to sort by.</param>
	/// <returns>Sorted data.</returns>
	public static IEnumerable PropertySort(IEnumerable data, string property)
	{
		Contract.Requires<ArgumentNullException>(data != null, nameof(data));
		Contract.Requires<ArgumentNullException>(property != null, nameof(property));

		SortItemCollection sortItemCollection = new SortItemCollection();
		sortItemCollection.Add(new SortItem(property, SortDirection.Ascending));
		return PropertySort(data, sortItemCollection);
	}
}
