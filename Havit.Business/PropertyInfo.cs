using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Bázová třída pro všechny property-info objektu.
	/// </summary>
	public abstract class PropertyInfo
	{
		private bool isInitialized = false;

		/// <summary>
		/// Třída, které property náleží.
		/// </summary>
		public ObjectInfo Owner
		{
			get { return owner; }
		}
		private ObjectInfo owner;

		/// <summary>
		/// Název property reprezentované instancí.
		/// </summary>
		public string PropertyName
		{
			get { return propertyName; }
		}
		private string propertyName;

		/// <summary>
		/// Inicializuje instanci.
		/// </summary>
		/// <param name="owner">ObjectInfo vlastnící property.</param>
		/// <param name="propertyName">Název vlastnosti.</param>
		[SuppressMessage("SonarLint", "S1117", Justification = "Není chybou mít parametr metody stejného jména ve třídě.")]
		protected virtual void Initialize(ObjectInfo owner, string propertyName)
		{
			this.owner = owner;
			this.propertyName = propertyName;
			this.isInitialized = true;
		}

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
	}
}
