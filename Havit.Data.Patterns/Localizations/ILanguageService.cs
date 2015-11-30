using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Localizations
{
	/// <summary>
	/// Služba vrací aktuální jazyk.
	/// Jazykem se rozumí instance třídy modelu (implementující ILanguage).
	/// </summary>
	public interface ILanguageService
	{
		/// <summary>
		/// Vrací akutální jazyk.
		/// </summary>
		ILanguage GetLanguage(string culture);
	}
}