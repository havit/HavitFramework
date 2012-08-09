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
	public class SortItem
	{
		#region Constructors

		/// <summary>
		/// Vytvoøí prázdnou instanci poøadí.
		/// </summary>
		public SortItem()
		{
		}

		/*
		/// <summary>
		/// Vytvoøí položdu øazení podle fieldName, vzestupné øazení.
		/// </summary>
		public SortItem(string fieldName)
			: this(fieldName, SortDirection.Ascending)
		{
#warning Chceme umožnit stringové zadání fieldName?
		}
		 */

		/// <summary>
		/// Vytvoøí položdu øazení podle fieldName a daného poøadí.
		/// </summary>
		protected SortItem(string fieldName, SortDirection direction)
			: this()
		{
#warning Chceme umožnit stringové zadání fieldName?
			this.fieldName = fieldName;
			this.direction = direction;
		}

		/// <summary>
		/// Vytvoøí položdu øazení podle sloupce, vzestupné poøadí.
		/// </summary>
		public SortItem(FieldPropertyInfo property)
			: this(property.FieldName, SortDirection.Ascending)
		{
		}

		/// <summary>
		/// Vytvoøí položdu øazení podle sloupce a daného poøadí.
		/// </summary>
		public SortItem(FieldPropertyInfo property, SortDirection direction)
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
#warning Nebìl by SortItem pracovat ještì  s PropertyInfo, spíš než se stringovým FieldName?
			get { return fieldName; }
			set { fieldName = value; }
		}
		private string fieldName;

		/// <summary>
		/// Smìr øazení.
		/// </summary>
		public SortDirection Direction
		{
			get { return direction; }
			set { direction = value; }
		}
		private SortDirection direction = SortDirection.Ascending;

		#endregion

		#region GetSqlOrderBy
		/// <summary>
		/// Vrátí výraz pro øazení.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public virtual string GetSqlOrderBy()
		{
			string result = fieldName;
			if (direction == SortDirection.Descending)
			{
				result = result + " DESC";
			}
			return result;
		}
		#endregion

	}
}
