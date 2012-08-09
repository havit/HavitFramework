using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PropertyHolder<T> : PropertyHolderBase
	{
		#region owner
		/// <summary>
		/// Objekt, kterému property patøí.
		/// </summary>
		private BusinessObjectBase owner;
#warning Proè není Owner jako protected/internal propety už v PropertyHolderBase?
		#endregion

		#region Constructors
		/// <summary>
		/// Založí instanci PropertyHolderu.
		/// </summary>
		/// <param name="owner">objekt, kterému PropertyHolder patøí</param>
		public PropertyHolder(BusinessObjectBase owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}

			this.owner = owner;

			// zaregistrujeme se majiteli
			owner.RegisterPropertyHolder(this);
		}
		#endregion

		#region Value
		/// <summary>
		/// Hodnota, kterou PropertyHolder nese.
		/// </summary>
		public T Value
		{
			get
			{
				if (!_isInitialized)
				{
					throw new InvalidOperationException("Hodnota nebyla inicializována.");
				}
				return _value;
			}
			set
			{
				if (!Object.Equals(_value, value))
				{
					_isDirty = true;
					owner.SetDirty();
				}

				_isInitialized = true;
				
				// není pod podmínkou !Object.Equals(), protože mùže dojít k uložení jiné instance, která je sice nyní rovna,
				// ale pokud by se s ní dále pracovalo, mohou se už tyto instance rozcházet
				// (nastavení IsDirty musíme v tom pøípadì sledovat pøes odbìr události "zmìna")
				_value = value; 
			}
		}
		private T _value;
		#endregion

		#region IsDirty
		/// <summary>
		/// Indikuje, zda došlo ke zmìnì hodnoty.
		/// </summary>
		public bool IsDirty
		{
#warning Proè není už v PropertyHolderBase?
			get
			{
				return _isDirty;
			}
		}
		private bool _isDirty = false;
		#endregion

		/// <summary>
		/// Indikuje, zda je hodnota property nastavena.
		/// </summary>
		public bool IsInitialized
		{
#warning Proè není už v PropertyHolderBase? Takto se nemùžu objektu zeptat, jestli má inicializované všechny property...
			get
			{
				return _isInitialized;
			}
		}
		private bool _isInitialized = false;


		internal override void ClearDirty()
		{
#warning Proè nestaèí místo ClearDirty() jen set-accesor?
			_isDirty = false;
		}
	}
}
