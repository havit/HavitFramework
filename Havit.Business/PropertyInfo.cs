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
		/// Inicializuje instanci.
		/// </summary>
		/// <param name="owner"></param>
		protected virtual void Initialize(ObjectInfo owner)
		{
			this.owner = owner;
			this.isInitialized = true;
		}
		private bool isInitialized = false;

		/// <summary>
		/// Tøída, které property náleží.
		/// </summary>
		public ObjectInfo Owner
		{
			get { return owner; }
		}
		private ObjectInfo owner;

		/// <summary>
		/// Ovìøí, že byla instance inicializována. Pokud ne, vyhodí výjimku.
		/// </summary>
		protected void CheckInitialization()
		{
			if (!isInitialized)
			{
				throw new InvalidOperationException("Instance nebyla inicializována.");
			}
		}
	}
}
