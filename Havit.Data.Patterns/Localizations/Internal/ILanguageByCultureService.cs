using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Localizations.Internal
{
	/// <summary>
	/// Služba vrací na základě culture (např. "en-US", "cs-CZ", "sk", "") ID jazyka.
	/// Jazykem se rozumí instance třídy modelu (implementující <see cref="ILanguage"/>).
	/// </summary>
	public interface ILanguageByCultureService
	{
		/// <summary>
		/// Vrací identifikátor jazyka podle culture.		
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Není-li jazyk podle culture nalezen.
		/// </exception>
		int GetLanguageId(string cultureName);
	}
}
