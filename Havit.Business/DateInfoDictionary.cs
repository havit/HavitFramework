using System;
using System.Collections;

namespace Havit.Business
{
	/// <summary>
	/// Dictionary pro klíè <see cref="System.DateTime"/> a hodnoty <see cref="DateInfo"/>.
	/// </summary>
	public class DateInfoDictionary : DictionaryBase
	{
		#region Constructor
		/// <summary>
		/// Vytvoøí prázdnou instanci <see cref="DateInfoDictionary"/>
		/// </summary>
		public DateInfoDictionary()
		{
		}
		#endregion

		#region Indexer
		/// <summary>
		/// Indexer pøes klíèe <see cref="System.DateTime"/>.
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
		#endregion

		#region Add
		/// <summary>
		/// Pøidá <see cref="DateInfo"/> do slovníku.<br/>
		/// Klíèem je <see cref="DateInfo.Date"/>.
		/// </summary>
		/// <param name="value">Prvek, kterı má bıt pøidán do slovníku.</param>
		public void Add(DateInfo value)
		{
			Dictionary.Add(value.Date, value);
		}
		#endregion

		#region Constains
		/// <summary>
		/// Zjistí, zda-li je ve slovníku poadovanı den.
		/// </summary>
		/// <param name="key">zjišovanı den</param>
		public bool Contains(DateTime key)
		{
			return Dictionary.Contains(key);
		}
		#endregion
	}
}
