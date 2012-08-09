using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Collections
{
	/// <summary>
	/// Porovnává hodnoty vlastností dvou objektù. Názvy vlastností jsou dodány, porovnávají se v dodaném poøadí.
	/// Názvy vlastností mohou být složené: napø. "Kniha.Autor.Prijmeni".
	/// Property musí implementovat IComparable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class GenericPropertyComparer<T> : IComparer<T>
	{
		#region Private fields
		private SortItemCollection sortItemCollection;
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoøí instanci compareru pro øazení dle dané property.
		/// </summary>
		/// <param name="sortPropertyName">název property, podle které se má øadit</param>
		/// <param name="ascending">true, má-li se øadit vzestupnì, false, pokud sestupnì</param>
		public GenericPropertyComparer(String sortPropertyName, bool ascending)
		{			
			sortItemCollection = new SortItemCollection();
			sortItemCollection.Add(new SortItem(sortPropertyName, ascending ? SortDirection.Ascending : SortDirection.Descending));
		}

		/// <summary>
		/// Vytvoøí instanci compareru pro øazení dle kolekce vlastností.
		/// </summary>
		/// <param name="sortItemCollection"></param>
		public GenericPropertyComparer(SortItemCollection sortItemCollection)
		{
			this.sortItemCollection = sortItemCollection;
		}
		#endregion

		#region Compare methods
		int IComparer<T>.Compare(T x, T y)
		{
			return Compare(x, y, 0);
		}

		/// <summary>
		/// Porovná vlastnosti instancí dvou objektù. Porovnávají se index-té vlastnosti uvedené ve fieldu sortItemCollection.
		/// </summary>
		/// <param name="x">První porovnávaný objekt.</param>
		/// <param name="y">Druhý porovnávaný objekt.</param>
		/// <param name="index">Index porovnávané vlastnosti.</param>
		/// <returns>-1, 0, 1 - jako Compare(T, T)</returns>
		protected int Compare(object x, object y, int index)
		{
			if (index >= sortItemCollection.Count)
				return 0;

			/* napsáno trochu komplikovanìji - pro pøehlednost */
			IComparable value1;
			IComparable value2;
			if (sortItemCollection[index].Direction == SortDirection.Ascending)
			{
				value1 = (IComparable)GetValue(x, index);
				value2 = (IComparable)GetValue(y, index);
			}
			else
			{
				value2 = (IComparable)GetValue(x, index);
				value1 = (IComparable)GetValue(y, index);
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
			else if (value1 != null || value2 != null)
			{
				// ani jedno neni null -> porovname
				result = ((IComparable)value1).CompareTo((IComparable)value2);
			}

			return result == 0 ? Compare(x, y, index + 1) : result;
		}

		/// <summary>
		/// Vrátí hodnotu index-té property objektu.
		/// </summary>
		private object GetValue(object obj, int index)
		{
			return DataBinder.Eval(obj, sortItemCollection[index].Expression);
		}

		#endregion
	}
}