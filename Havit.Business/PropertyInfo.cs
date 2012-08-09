using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Bázová tøída pro všechny property-info objektu.
	/// </summary>
	public abstract class PropertyInfo
	{
		/// <summary>
		/// Tøída, které property náleží.
		/// </summary>
		public ObjectInfo Parent
		{
			get { return parent; }
			internal set { parent = value; }
		}
		private ObjectInfo parent;
	}
}
