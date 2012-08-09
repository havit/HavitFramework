using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Pøedek generického typu <see cref="PropertyHolder{T}"/>. 
	/// Potøebujeme kolekci PropertyHolderù a kolekci generických typù nelze udìlat.
	/// </summary>
	public abstract class PropertyHolderBase
	{
		#region Owner
		/// <summary>
		/// Objekt, kterému property patøí.
		/// </summary>
		protected BusinessObjectBase Owner
		{
			get { return _owner; }
		}
		private BusinessObjectBase _owner;
		#endregion

		#region IsDirty
		/// <summary>
		/// Indikuje, zda došlo ke zmìnì hodnoty.
		/// </summary>
		public bool IsDirty
		{
			get
			{
				return _isDirty;
			}
			internal set
			{
				_isDirty = value;
				if (_isDirty)
					Owner.IsDirty = true;
			}
		}
		private bool _isDirty = false;
		#endregion

		#region IsInitialized
		/// <summary>
		/// Indikuje, zda je hodnota property nastavena.
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				return _isInitialized;
			}
			protected set
			{
				_isInitialized = value;
			}
		}
		private bool _isInitialized = false;
		#endregion

		#region CheckInitialization
		/// <summary>
		/// Pokud nebyla hodnota PropertyHolderu nastavena, vyhodí InvalidOperationException.
		/// Pokud byla hodnota PropertyHolderu nastavena, neudìlá nic (projde).
		/// </summary>
		protected void CheckInitialization()
		{
			if (!_isInitialized)
			{
				throw new InvalidOperationException("Hodnota nebyla inicializována.");
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Založí instanci PropertyHolderu.
		/// </summary>
		/// <param name="owner">objekt, kterému PropertyHolder patøí</param>
		public PropertyHolderBase(BusinessObjectBase owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}

			this._owner = owner;

			// zaregistrujeme se majiteli do kolekce PropertyHolders
			owner.RegisterPropertyHolder(this);
		}
		#endregion
	}
}
