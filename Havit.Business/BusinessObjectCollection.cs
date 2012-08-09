using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Havit.Collections;
using System.Data.Common;

namespace Havit.Business
{
	/// <summary>
	/// Bázová tøída pro všechny kolekce BusinessObjectBase (Layer SuperType)
	/// </summary>
	/// <remarks>
	/// POZOR! Vnitøní implementace je závislá na faktu, že this.Items je List(Of T).
	/// To je výchozí chování Collection(Of T), ale pro jistotu si to ještì vynucujeme
	/// použitím wrappujícího constructoru.
	/// </remarks>
	/// <typeparam name="T">èlenský typ kolekce</typeparam>
	[Serializable]
	public class BusinessObjectCollection<T> : Collection<T>
		where T : BusinessObjectBase
	{

		#region Event - CollectionChanged
		/// <summary>
		/// Událost vyvolaná po jakékoliv zmìnì kolekce (Insert, Remove, Set, Clear).
		/// </summary>
		public event EventHandler CollectionChanged;

		/// <summary>
		/// Provádí se jako volání události <see cref="CollectionChanged"/>.
		/// </summary>
		/// <param name="e">prázdné</param>
		protected void OnCollectionChanged(EventArgs e)
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, e);
			}
		}
		#endregion

		#region AllowDuplicates
		/// <summary>
		/// Urèuje, zda je možné do kolekce vložit hodnotu, která již v kolekci je.
		/// Pokud je nastaveno na true, pøidání hodnoty, která v kolekci již je, vyvolá výjimku.
		/// Pokud je nastaveno na false (výchozí), je možné hodnotu do kolekce pøidat vícekrát.
		/// </summary>
		public bool AllowDuplicates
		{
			get { return allowDuplicates; }
			set { allowDuplicates = value; }
		}
		private bool allowDuplicates = true;
		#endregion

		#region AddRange
		/// <summary>
		/// Pøidá do kolekce prvky pøedané kolekce.
		/// </summary>
		/// <param name="source">Kolekce, jejíž prvky mají být pøidány.</param>
		public void AddRange(IEnumerable<T> source)
		{
			foreach (T item in source)
			{
				this.Add(item);
			}
		}
		#endregion

		#region InsertItem (override)
		/// <summary>
		/// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"></see> at the specified index.
		/// When AllowDuplicates is false, checks whether item already is in the collection. If so, throws an ArgumentException.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert. The value can be null for reference types.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero.-or-index is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"></see>.</exception>
		protected override void InsertItem(int index, T item)
		{
			if ((!allowDuplicates) && (this.Contains(item)))
			{
				throw new ArgumentException("Položka v kolekci již existuje (a není povoleno vkládání duplicit).");
			}

			base.InsertItem(index, item);
			OnCollectionChanged(EventArgs.Empty);
		}
		#endregion

		#region RemoveItem (override)
		/// <summary>
		/// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"></see>.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero.-or-index is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"></see>.</exception>
		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
			OnCollectionChanged(EventArgs.Empty);
		}
		#endregion

		#region SetItem (override)
		/// <summary>
		/// Replaces the element at the specified index.
		/// When AllowDuplicates is false, checks whether item already is in the collection. If so, throws an ArgumentException.
		/// </summary>
		/// <param name="index">The zero-based index of the element to replace.</param>
		/// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero.-or-index is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"></see>.</exception>
		protected override void SetItem(int index, T item)
		{
			if (!allowDuplicates && (this.IndexOf(item) != index))
			{
				throw new ArgumentException("Položka v kolekci již existuje (a není povoleno vkládání duplicit).");
			}
			base.SetItem(index, item);
			OnCollectionChanged(EventArgs.Empty);
		}
		#endregion

		#region ClearItems (override)
		/// <summary>
		/// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"></see>.
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			OnCollectionChanged(EventArgs.Empty);
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoøí novou instanci kolekce bez prvkù - prázdnou.
		/// </summary>
		/// <remarks>
		/// Použit je wrappující constructor Collection(Of T), abychom si vynutili List(Of T).
		/// </remarks>
		public BusinessObjectCollection()
			: base(new List<T>())
		{
		}

		/// <summary>
		/// Vytvoøí novou instanci kolekce wrapnutím Listu prvkù (neklonuje!).
		/// </summary>
		/// <remarks>
		/// Je to rychlé! Nikam se nic nekopíruje, ale pozor, ani neklonuje!
		/// </remarks>
		/// <param name="list">List prvkù</param>
		public BusinessObjectCollection(List<T> list)
			: base(list)
		{
		}
		#endregion

		#region FindByID
		/// <summary>
		/// Prohledá kolekci a vrátí první nalezený prvek s požadovaným ID.
		/// </summary>
		/// <remarks>
		/// Vzhledem k tomu, že jsou prvky v kolekci obvykle unikátní, najde prostì zadané ID.
		/// </remarks>
		/// <param name="id">ID prvku</param>
		/// <returns>první nalezený prvek s požadovaným ID; null, pokud nic nenalezeno</returns>
		public T FindByID(int id)
		{
			List<T> innerList = (List<T>)Items;
			T result = null;
			result = innerList.Find(delegate(T item)
								  {
									  if (item.ID == id)
									  {
										  return true;
									  }
									  else
									  {
										  return false;
									  }
								  });

			return result;
		}
		#endregion

		#region Find
		/// <summary>
		/// Prohledá kolekci a vrátí první nalezený prvek odpovídající kritériu match.
		/// </summary>
		/// <remarks>
		/// Metoda pouze publikuje metodu Find() inner-Listu Items.
		/// </remarks>
		/// <param name="match">kritérium ve formì predikátu</param>
		/// <returns>kolekce všech prvkù odpovídajících kritériu match</returns>
		public virtual T Find(Predicate<T> match)
		{
			List<T> innerList = (List<T>)Items;
			return innerList.Find(match);
		}
		#endregion

		#region FindAll
		/// <summary>
		/// Prohledá kolekci a vrátí všechny prvky odpovídající kritériu match.
		/// </summary>
		/// <remarks>
		/// Metoda pouze publikuje metodu FindAll() inner-listu Items.
		/// </remarks>
		/// <param name="match">kritérium ve formì predikátu</param>
		/// <returns>kolekce všech prvkù odpovídajících kritériu match</returns>
		public virtual BusinessObjectCollection<T> FindAll(Predicate<T> match)
		{
			List<T> innerList = (List<T>)Items;
			return new BusinessObjectCollection<T>(innerList.FindAll(match));
		}
		#endregion

		#region ForEach
		/// <summary>
		/// Spustí akci nad všemi prvky kolekce.
		/// </summary>
		/// <example>
		/// orders.ForEach(delegate(Order item)
		///		{
		///			item.Delete();
		///		});
		/// </example>
		/// <remarks>
		/// Je rychlejší, než <c>foreach</c>, protože neprochází enumerator, ale iteruje prvky ve for cyklu podle indexu.
		/// </remarks>
		/// <param name="action">akce, která má být spuštìna</param>
		public void ForEach(Action<T> action)
		{
			List<T> innerList = (List<T>)Items;
			innerList.ForEach(action);
		}
		#endregion

		#region Sort
		/// <summary>
		/// Seøadí prvky kolekce dle požadované property, která implementuje IComparable.
		/// </summary>
		/// <remarks>
		/// Používá <see cref="Havit.Collections.GenericPropertyComparer{T}"/>. K porovnávání podle property
		/// tedy dochází pomocí reflexe - relativnì pomalu. Pokud je potøeba vyšší výkon, je potøeba použít
		/// overload Sort(Generic Comparsion) s pøímým pøístupem k property.
		/// </remarks>
		/// <param name="propertyName">property, podle které se má øadit</param>
		/// <param name="ascending">true, pokud se má øadit vzestupnì, false, pokud sestupnì</param>
		public void Sort(string propertyName, bool ascending)
		{
			List<T> innerList = (List<T>)Items;
			innerList.Sort(new GenericPropertyComparer<T>(propertyName, ascending));
		}

		/// <summary>
		/// Seøadí prvky kolekce dle zadaného srovnání. Publikuje metodu Sort(Generic Comparsion) inner-Listu.
		/// </summary>
		/// <param name="comparsion">srovnání, podle kterého mají být prvky seøazeny</param>
		public void Sort(Comparison<T> comparsion)
		{
			List<T> innerList = (List<T>)Items;
			innerList.Sort(comparsion);
		}
		#endregion

		#region SaveAll
		/// <summary>
		/// Uloží všechny prvky kolekce, v transakci.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v které mají být prvky uloženy</param>
		public virtual void SaveAll(DbTransaction transaction)
		{
			ForEach(delegate(T item)
				{
					item.Save(transaction);
				});
		}

		/// <summary>
		/// Uloží všechny prvky kolekce, bez transakce.
		/// </summary>
		public virtual void SaveAll()
		{
			SaveAll(null);
		}
		#endregion

		#region GetIDs
		/// <summary>
		/// Vrátí pole hodnot ID všech prvkù kolekce.
		/// </summary>
		/// <returns>pole hodnot ID všech prvkù kolekce</returns>
		public int[] GetIDs()
		{
			int[] array = new int[this.Count];
			List<T> innerList = (List<T>)Items;
			for (int i = 0; i < innerList.Count; i++)
			{
				array[i] = innerList[i].ID;
			}
			return array;
		}
		#endregion
	}
}
