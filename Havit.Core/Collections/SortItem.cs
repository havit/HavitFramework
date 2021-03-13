using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace Havit.Collections
{
	/// <summary>
	/// Reprezentuje položku řazení.
	/// </summary>
	[Serializable]
	[DataContract]
	public class SortItem
	{
		/// <summary>
		/// Výraz, dle kterého se řadí.
		/// </summary>
		[DataMember(Order = 1)]
		public virtual string Expression { get; set; }

		/// <summary>
		/// Směr řazení.
		/// </summary>
		[DataMember(Order = 2)]
		public virtual SortDirection Direction { get; set; }

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
			this.Expression = expression;
			this.Direction = direction;
		}
	}
}
