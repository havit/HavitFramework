using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Havit.Business.Query
{
	/// <summary>
	/// Reprezentuje položku øazení.
	/// </summary>
	[Serializable]	
	public class SortingItem
	{
		#region Constructors

		/// <summary>
		/// Vytvoøí prázdnou instanci poøadí.
		/// </summary>
		public SortingItem()
		{
		}

		/// <summary>
		/// Vytvoøí položdu øazení podle fieldName, vzestupné øazení.
		/// </summary>
		public SortingItem(string fieldName)
			: this(fieldName, ListSortDirection.Ascending)
		{			
		}

		/// <summary>
		/// Vytvoøí položdu øazení podle fieldName a daného poøadí.
		/// </summary>
		public SortingItem(string fieldName, ListSortDirection direction)
			: this()
		{
			this.fieldName = fieldName;
			this.direction = direction;
		}

		/// <summary>
		/// Vytvoøí položdu øazení podle sloupce, vzestupné poøadí.
		/// </summary>
		public SortingItem(Property property)
			: this(property.FieldName, ListSortDirection.Ascending)
		{
		}

		/// <summary>
		/// Vytvoøí položdu øazení podle sloupce a daného poøadí.
		/// </summary>
		public SortingItem(Property property, ListSortDirection direction)
			: this(property.FieldName, direction)
		{
		}

		#endregion

		#region Properties
		/// <summary>
		/// Název sloupce, dle kterého se øadí.
		/// </summary>
		public string FieldName
		{
			get { return fieldName; }
			set { fieldName = value; }
		}
		private string fieldName;

		/// <summary>
		/// Smìr øazení.
		/// </summary>
		public ListSortDirection Direction
		{
			get { return direction; }
			set { direction = value; }
		}
		private ListSortDirection direction = ListSortDirection.Ascending;

		#endregion

		#region GetSqlOrderBy
		/// <summary>
		/// Vrátí výraz pro øazení.
		/// </summary>
		public virtual string GetSqlOrderBy()
		{
			string result = fieldName;
			if (direction == ListSortDirection.Descending)
				result = result + " DESC";
			return result;
		}
		#endregion

	}
}
