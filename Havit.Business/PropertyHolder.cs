using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Třída pro objekt, který nese hodnotu a vlastnosti jednotlivé property BusinessObjectu.
	/// </summary>
	/// <typeparam name="T">typ property, jíž je PropertyHolder nosičem</typeparam>
	public class PropertyHolder<T> : PropertyHolderBase
	{
		/// <summary>
		/// Založí instanci PropertyHolderu.
		/// </summary>
		/// <param name="owner">objekt, kterému PropertyHolder patří</param>
		public PropertyHolder(BusinessObjectBase owner)
			: base(owner)
		{
		}

		/// <summary>
		/// Hodnota, kterou PropertyHolder nese.
		/// </summary>
		public T Value
		{
			get
			{
				CheckInitialization();
				return _value;
			}
			set
			{
				if (!Object.Equals(_value, value) || (!IsInitialized))
				{
					// pokud meníme hodnotu nebo nastavujeme novou hodnotu, rekneme, ze jsme zmeneny
					IsDirty = true;					
				}

				IsInitialized = true;

				// není pod podmínkou !Object.Equals(), protože může dojít k uložení jiné instance, která je sice nyní rovna,
				// ale pokud by se s ní dále pracovalo, mohou se už tyto instance rozcházet
				// (nastavení IsDirty musíme v tom případě sledovat přes odběr události "změna")
				_value = value;
			}
		}
		private T _value;
	}
}
