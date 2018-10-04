using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Havit.Collections
{
	/// <summary>
	/// Pomocná třída pro řazení.
	/// </summary>
	public static class SortHelper
	{
		#region PropertySort
		/// <summary>
		/// Vrátí data seřazená podle properties v sortItemCollection.
		/// Pokud je sortItemCollection prázdná kolekce, vrací parametr data.
		/// </summary>
		/// <param name="data">Data k seřazení.</param>
		/// <param name="sortItemCollection">Instrukce, jak seřadit.</param>
		/// <returns>Seřazená data.</returns>
		public static IEnumerable PropertySort(IEnumerable data, SortItemCollection sortItemCollection)
		{
			if (sortItemCollection.Count == 0)
			{
				return data;
			}

			// překopírujeme data do jiné struktury
			List<object> dataList = new List<object>();
			foreach (object o in data)
			{
				dataList.Add(o);
			}
			// seředíme data
			dataList.Sort(new GenericPropertyComparer<object>(sortItemCollection));
			// provedeme databinding na seřazených datech
			return dataList;
		}

		/// <summary>
		/// Vrátí data seřazená podle property.
		/// </summary>
		/// <param name="data">Data k seřazení.</param>
		/// <param name="property">Property, podle které se řadí.</param>
		/// <returns>Seřazená data.</returns>
		public static IEnumerable PropertySort(IEnumerable data, string property)
		{
			SortItemCollection sortItemCollection = new SortItemCollection();
			sortItemCollection.Add(new SortItem(property, SortDirection.Ascending));
			return PropertySort(data, sortItemCollection);
		}

		#endregion
	}
}
