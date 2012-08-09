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
		#endregion
	}
}
