using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Havit.Business
{
	/// <summary>
	/// Rozhraní kolekce lokalizačních business objektů.
	/// </summary>
	public interface ILocalizationCollection : ICollection
	{
		/// <summary>
		/// Vrátí business objekt pro aktuální jazyk.
		/// </summary>
		BusinessObjectBase Current { get; }

		/// <summary>
		/// Vrýtí business objekt pro zadaný jazyk.
		/// </summary>
		BusinessObjectBase this[ILanguage language] { get; }
	}
}
