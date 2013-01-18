using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Bázová třída pro všechny property-info objektu.
	/// </summary>
	public abstract class PropertyInfo
	{
		#region isInitialized (private field)
		private bool isInitialized = false;
		#endregion

		#region Owner
		/// <summary>
		/// Třída, které property náleží.
		/// </summary>
		public ObjectInfo Owner
		{
			get { return owner; }
		}
		private ObjectInfo owner;
		#endregion

		#region PropertyName
		/// <summary>
		/// Název property reprezentované instancí.
		/// </summary>
		public string PropertyName
		{
			get { return propertyName; }
		}
		private string propertyName;
		#endregion

		#region Initialize
		/// <summary>
		/// Inicializuje instanci.
		/// </summary>
		/// <param name="owner">ObjectInfo vlastnící property.</param>
		/// <param name="propertyName">Název vlastnosti.</param>
		protected virtual void Initialize(ObjectInfo owner, string propertyName)
		{
			this.owner = owner;
			this.propertyName = propertyName;
			this.isInitialized = true;
		}
		#endregion

		#region CheckInitialization
		/// <summary>
		/// Ověří, že byla instance inicializována. Pokud ne, vyhodí výjimku.
		/// </summary>
		protected void CheckInitialization()
		{
			if (!isInitialized)
			{
				throw new InvalidOperationException("Instance nebyla inicializována.");
			}
		}
		#endregion

	}
}
