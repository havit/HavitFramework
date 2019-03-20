using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Havit.Collections
{
	/// <summary>
	/// Reprezentuje položku řazení.
	/// </summary>
	[Serializable]	
	public class SortItem
	{
		/// <summary>
		/// Výraz, dle kterého se řadí.
		/// </summary>
		public virtual string Expression
		{
			get { return expression; }
			set { expression = value; }
		}
		private string expression;

		/// <summary>
		/// Směr řazení.
		/// </summary>
		public virtual SortDirection Direction
		{
			get { return direction; }
			set { direction = value; }
		}
		private SortDirection direction = SortDirection.Ascending;

		/// <summary>
		/// Vytvoří prázdnou instanci pořadí.
		/// </summary>
		public SortItem()
		{
		}

		/// <summary>
		/// Vytvoří položku řazení podle expression a směru řazení.
		/// </summary>
		public SortItem(string expression, SortDirection direction)
			: this()
		{
			this.expression = expression;
			this.direction = direction;
		}
	}
}
