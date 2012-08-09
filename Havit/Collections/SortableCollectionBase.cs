using System;

namespace Havit.Collections
{
	/// <summary>
	/// Abstraktní tøída pro strong-typed collections s øazením dle jedné libovolné property.
	/// </summary>
	/// <remarks>
	/// Property, podle které má být øazeno, musí implementovat <see cref="System.IComparable"/>
	/// </remarks>
	[Obsolete]
	public abstract class SortableCollectionBase : System.Collections.CollectionBase
	{
		/// <summary>
		/// Seøadí prvky dle požadované property, která implementuje IComparable.
		/// </summary>
		/// <param name="propertyName">property, podle které se má øadit</param>
		/// <param name="ascending">true, pokud se má øadit vzestupnì, false, pokud sestupnì</param>
		public virtual void Sort(string propertyName, bool ascending) 
		{
			InnerList.Sort(new GenericPropertySort(propertyName, ascending));
		}

		/// <summary>
		/// Vrátí polohu prvku v seøazené collection.
		/// </summary>
		/// <param name="searchedValue">hodnota property prvku</param>
		/// <param name="propertyName">jméno property</param>
		/// <returns>poloha prvku</returns>
		public int IndexOf(string propertyName, object searchedValue) 
		{
			for (int i=0; i < InnerList.Count; i++)
			{
				if (((IComparable)InnerList[i].GetType().GetProperty(propertyName).GetValue(InnerList[i], null)).CompareTo(searchedValue) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Comparer pro øazení dle libobovolné IComparable property.
		/// </summary>
		internal class GenericPropertySort : System.Collections.IComparer
		{
			private bool sortAscending = true;
			private string sortPropertyName = String.Empty;
		
			/// <summary>
			/// Vytvoøí instanci compareru pro øazení dle dané property.
			/// </summary>
			/// <param name="sortPropertyName">název property, podle které se má øadit</param>
			/// <param name="ascending">true, má-li se øadit vzestupnì, false, pokud sestupnì</param>
			public GenericPropertySort(String sortPropertyName, bool ascending) 
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
			public int Compare(object x, object y)
			{
				if ((sortPropertyName == null) || (sortPropertyName.Length == 0))
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
}
