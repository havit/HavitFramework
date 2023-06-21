using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Havit.Business;

/// <summary>
/// Rozhraní označující lokalizovaný objekt.
/// </summary>
public interface ILocalizable
{
	/// <summary>
	/// Lokalizace.
	/// </summary>
	ILocalizationCollection Localizations { get; }

	/// <summary>
	/// Vytvoří položku lokalizace pro daný jazyk.
	/// </summary>
	BusinessObjectBase CreateLocalization(ILanguage language);

}
