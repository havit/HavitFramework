using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Collections
{
	/// <summary>
	/// Comparer pro øazení dle libobovolné IComparable property.
	/// </summary>
	public class GenericPropertyComparer<T> : IComparer<T>
	{
		private bool sortAscending = true;
		private string sortPropertyName = String.Empty;

		/// <summary>
		/// Vytvoøí instanci compareru pro øazení dle dané property.
		/// </summary>
		/// <param name="sortPropertyName">název property, podle které se má øadit</param>
		/// <param name="ascending">true, má-li se øadit vzestupnì, false, pokud sestupnì</param>
		public GenericPropertyComparer(String sortPropertyName, bool ascending)
		{
			this.sortPropertyName = sortPropertyName;
			this.sortAscending = ascending;
		}

		/// <summary>
		/// Porovná dva objekty.
		/// </summary>
		/// <param name="x">první objekt</param>
		/// <param name="y">druhý objekt</param>
		/// <returns>výsledek porovnání</returns>
		public int Compare(T x, T y)
		{
			if (String.IsNullOrEmpty(sortPropertyName))
			{
				return 0; // shoda
			}
			IComparable ic1 = (IComparable)x.GetType().GetProperty(sortPropertyName).GetValue(x, null);
			IComparable ic2 = (IComparable)y.GetType().GetProperty(sortPropertyName).GetValue(y, null);

			if ((ic1 == null) && (ic2 == null))
			{
				return 0; // shoda
			}
			else if (ic1 == null)
			{
				return sortAscending ? -1 : 1;
			}
			else if (ic2 == null)
			{
				return sortAscending ? 1 : -1;
			}

			if (sortAscending)
			{
				return ic1.CompareTo(ic2);
			}
			else
			{
				return ic2.CompareTo(ic1);
			}
		}
	}
}
