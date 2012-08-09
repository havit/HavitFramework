using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Havit.Collections
{
	/// <summary>
	/// Pomocná tøída pro øazení.
	/// </summary>
	public static class SortHelper
	{
		#region PropertySort
		/// <summary>
		/// Vrátí data seøazená podle properties v sortItemCollection.
		/// Pokud je sortItemCollection prázdná kolekce, vrací parametr data.
		/// </summary>
		/// <param name="data">Data k seøazení.</param>
		/// <param name="sortItemCollection">Instrukce, jak seøadit.</param>
		/// <returns>Seøazená data.</returns>
		public static IEnumerable PropertySort(IEnumerable data, SortItemCollection sortItemCollection)
		{
			if (sortItemCollection.Count == 0)
			{
				return data;
			}

			// pøekopírujeme data do jiné struktury
			List<object> dataList = new List<object>();
			foreach (object o in data)
			{
				dataList.Add(o);
			}
			// seøedíme data
			dataList.Sort(new GenericPropertyComparer<object>(sortItemCollection));
			// provedeme databinding na seøazených datech
			return dataList;
		}

		/// <summary>
		/// Vrátí data seøazená podle property.
		/// </summary>
		/// <param name="data">Data k seøazení.</param>
		/// <param name="property">Property, podle které se øadí.</param>
		/// <returns>Seøazená data.</returns>
		public static IEnumerable PropertySort(IEnumerable data, string property)
		{
			SortItemCollection sortItemCollection = new SortItemCollection();
			sortItemCollection.Add(new SortItem(property, SortDirection.Ascending));
			return PropertySort(data, sortItemCollection);
		}

		#endregion
	}
}
