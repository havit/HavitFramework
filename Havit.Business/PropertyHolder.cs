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
		#region Private fields

		private T _value;
		private BusinessObjectBase _owner;
		private bool _isDirty = false;
		private bool _isInitialized = false;

		#endregion

		public PropertyHolder(BusinessObjectBase owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");

			this._owner = owner;
			// zaregistrujeme se majiteli
			_owner.RegisterPropertyHolder(this);
		}

		/// <summary>
		/// Hodnota, kterou PropertyHolder nese.
		/// </summary>
		public T Value
		{
			get
			{
				if (!_isInitialized)
					throw new InvalidOperationException("Hodnota nebyla inicializována.");

				return _value;
			}
			set
			{
				if (!Object.Equals(_value, value))
				{
					_isDirty = true;
					_owner.SetDirty();
				}

				_isInitialized = true;
				_value = value;
			}
		}

		/// <summary>
		/// Indikuje, zda došlo ke zmìnì hodnoty.
		/// </summary>
		public bool IsDirty
		{
			get
			{
				return _isDirty;
			}
		}

		/// <summary>
		/// Indikuje, zda je hodnota property nastavena.
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				return _isInitialized;
			}
		}


		internal override void ClearDirty()
		{
			_isDirty = false;
		}
	}
}
