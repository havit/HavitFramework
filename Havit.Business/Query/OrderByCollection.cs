using System;
using System.Collections.Generic;
using System.Text;
using Havit.Collections;

namespace Havit.Business.Query;

/// <summary>
/// Kolekce položek SortItem.
/// Určeno pro položky ORDER BY skládače SQL dotazu (QueryParameters).
/// </summary>
public class OrderByCollection : Havit.Collections.SortItemCollection
{
	/// <summary>
	/// Přidá na konec kolekce položku pro vzestupné řazení.
	/// </summary>
	public void Add(FieldPropertyInfo propertyInfo)
	{
		Add(propertyInfo, SortDirection.Ascending);
	}

	/// <summary>
	/// Přidá na konec kolekce položku pro řazení.
	/// </summary>
	public void Add(FieldPropertyInfo propertyInfo, SortDirection direction)
	{
		Add(new FieldPropertySortItem(propertyInfo, direction));
	}

	/// <summary>
	/// Přidá do kolekce položku pro vzestuné řazení.
	/// </summary>
	public void Insert(int index, FieldPropertyInfo propertyInfo)
	{
		Insert(index, propertyInfo, SortDirection.Ascending);
	}

	/// <summary>
	/// Přidá do kolekce položku pro řazení.
	/// </summary>
	public void Insert(int index, FieldPropertyInfo propertyInfo, SortDirection direction)
	{
		Insert(index, new FieldPropertySortItem(propertyInfo, direction));
	}
}
