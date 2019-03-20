using Havit.Diagnostics.Contracts;
using System;
using System.Collections;

namespace Havit.Business
{
	/// <summary>
	/// Dictionary pro klíč <see cref="System.DateTime"/> a hodnoty <see cref="DateInfo"/>.
	/// </summary>
	public class DateInfoDictionary : DictionaryBase
	{
		/// <summary>
		/// Indexer přes klíče <see cref="System.DateTime"/>.
		/// </summary>
		public DateInfo this[DateTime key]
		{
			get
			{
				return (DateInfo)Dictionary[key];
			}
			set
			{
				Dictionary[key] = value;
			}
		}

		/// <summary>
		/// Přidá <see cref="DateInfo"/> do slovníku.<br/>
		/// Klíčem je <see cref="DateInfo.Date"/>.
		/// </summary>
		/// <param name="value">Prvek, který má být přidán do slovníku.</param>
		public void Add(DateInfo value)
		{
			Contract.Requires<ArgumentNullException>(value != null, nameof(value));

			Dictionary.Add(value.Date, value);
		}

		/// <summary>
		/// Zjistí, zdali je ve slovníku požadovaný den.
		/// </summary>
		/// <param name="key">zjišťovaný den</param>
		public bool Contains(DateTime key)
		{
			return Dictionary.Contains(key);
		}
	}
}
