using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Tøída pro objekt, který nese hodnotu a vlastnosti jednotlivé property BusinessObjectu.
	/// </summary>
	/// <typeparam name="T">typ property, jíž je PropertyHolder nosièem</typeparam>
	public class PropertyHolder<T> : PropertyHolderBase
	{
		#region Constructors
		/// <summary>
		/// Založí instanci PropertyHolderu.
		/// </summary>
		/// <param name="owner">objekt, kterému PropertyHolder patøí</param>
		public PropertyHolder(BusinessObjectBase owner)
			: base(owner)
		{
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
				InitializationCheck();
				return _value;
			}
			set
			{
				if (!Object.Equals(_value, value))
				{
					IsDirty = true;
					
				}

				IsInitialized = true;

				// není pod podmínkou !Object.Equals(), protože mùže dojít k uložení jiné instance, která je sice nyní rovna,
				// ale pokud by se s ní dále pracovalo, mohou se už tyto instance rozcházet
				// (nastavení IsDirty musíme v tom pøípadì sledovat pøes odbìr události "zmìna")
				_value = value;
			}
		}
		private T _value;
		#endregion
	}
}
