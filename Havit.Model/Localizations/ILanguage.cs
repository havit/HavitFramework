using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Model.Localizations
{
	/// <summary>
	/// Předpis pro třídu reprezentující jazyk.
	/// </summary>
	public interface ILanguage
	{
		/// <summary>
		/// Culture.
		/// Používáme pro uložení hodnoty, která má být nastavena jako Culture pro použití daného jazyku.
		/// Hodnota bývá vždy ve formátu "en-US", "cs-CZ".
		/// </summary>
		string Culture { get; }

		/// <summary>
		/// UICulture.
		/// Používáme pro uložení hodnoty, pomocí které rozpoznáváme, o který jazyk jde, na základě Culture (Culture.Name).
		/// Hodnoty bývají ve formátu "en-US", "en", "cs-CZ", "cs", ev. pro výchozí jazyk "".
		/// </summary>
		string UiCulture { get; }
	}
}
