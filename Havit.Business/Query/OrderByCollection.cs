using System;
using System.Collections.Generic;
using System.Text;
using Havit.Collections;

namespace Havit.Business.Query
{
	/// <summary>
	/// Kolekce položek SortItem.
	/// Urèeno pro položky ORDER BY skládaèe SQL dotazu (QueryParameters).
	/// </summary>
	[Serializable]
	public class OrderByCollection: Havit.Collections.SortItemCollection
	{
		/// <summary>
		/// Pøidá na konec kolekce položku pro vzestupné øazení.
		/// </summary>
		public void Add(FieldPropertyInfo propertyInfo)
		{
			Add(propertyInfo, SortDirection.Ascending);
		}

		/// <summary>
		/// Pøidá na konec kolekce položku pro øazení.
		/// </summary>
		public void Add(FieldPropertyInfo propertyInfo, SortDirection direction)
		{
			Add(new FieldPropertySortItem(propertyInfo, direction));						
		}

		/// <summary>
		/// Pøidá do kolekce položku pro vzestuné øazení.
		/// </summary>
		public void Insert(int index, FieldPropertyInfo propertyInfo)
		{
			Insert(index, propertyInfo, SortDirection.Ascending);
		}

		/// <summary>
		/// Pøidá do kolekce položku pro øazení.
		/// </summary>
		public void Insert(int index, FieldPropertyInfo propertyInfo, SortDirection direction)
		{
			Insert(index, new FieldPropertySortItem(propertyInfo, direction));			
		}
	}
}
