using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Localizations;

/// <summary>
/// Služba vrací dohledání jazyka dle culture.
/// Jazykem se rozumí instance třídy modelu (implementující ILanguage).
/// </summary>
public interface ILanguageService
{
	/// <summary>
	/// Vrací jazyk pro danou culture.
	/// </summary>
	ILanguage GetLanguage(string cultureName);

	/// <summary>
	/// Vrací jazyk pro danou culture.
	/// </summary>
	ValueTask<ILanguage> GetLanguageAsync(string cultureName, CancellationToken cancellationToken = default);

	/// <summary>
	/// Vrací výchozí jazyk.
	/// </summary>
	ILanguage GetDefaultLanguage();

	/// <summary>
	/// Vrací výchozí jazyk.
	/// </summary>
	ValueTask<ILanguage> GetDefaultLanguageAsync(CancellationToken cancellationToken = default);

}