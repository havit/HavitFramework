using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Havit.Collections
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

		/// <summary>
		/// Vytvoøí položku øazení podle expression a smìru øazení.
		/// </summary>
		public SortItem(string expression, SortDirection direction)
			: this()
		{
			this.expression = expression;
			this.direction = direction;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Výraz, dle kterého se øadí.
		/// </summary>
		public virtual string Expression
		{
			get { return expression; }
			set { expression = value; }
		}
		private string expression;

		/// <summary>
		/// Smìr øazení.
		/// </summary>
		public virtual SortDirection Direction
		{
			get { return direction; }
			set { direction = value; }
		}
		private SortDirection direction = SortDirection.Ascending;
		#endregion
	}
}
