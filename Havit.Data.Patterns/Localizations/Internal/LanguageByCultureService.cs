using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Localizations.Internal;

/// <summary>
/// Služba vrací na základě culture (např. "en-US", "cs-CZ", "sk", "") ID jazyka.
/// Hledá se dle <see cref="ILanguage.UiCulture"/>.
/// Jazykem se rozumí instance třídy modelu (implementující <see cref="ILanguage"/>).
/// Jazyky jsou načteny do lokální proměnné a nejsou nikdy invalidovány.
/// </summary>
public class LanguageByCultureService<TLanguage, TLanguageKey> : ILanguageByCultureService<TLanguageKey>
	where TLanguage : class, ILanguage
{
	private readonly ILanguageByCultureStorage<TLanguageKey> languageByCultureStorage;
	private readonly IRepository<TLanguage, TLanguageKey> languageRepository; // TODO: QueryTags nedokonalé, bude se hlásit query tag dle DbRepository.
	private readonly IEntityKeyAccessor<TLanguage, TLanguageKey> entityKeyAccessor;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public LanguageByCultureService(ILanguageByCultureStorage<TLanguageKey> languageByCultureStorage, IRepository<TLanguage, TLanguageKey> languageRepository, IEntityKeyAccessor<TLanguage, TLanguageKey> entityKeyAccessor)
	{
		this.languageByCultureStorage = languageByCultureStorage;
		this.languageRepository = languageRepository;
		this.entityKeyAccessor = entityKeyAccessor;
	}

	/// <summary>
	/// Vrací identifikátor jazyka podle culture.		
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Není-li jazyk podle culture nalezen.
	/// </exception>
	public TLanguageKey GetLanguageId(string cultureName)
	{
		Dictionary<string, TLanguageKey> languagesByCulture = GetLanguagesByCulture();

		TLanguageKey tmp;

		// nejprve zkusíme hledat podle plného názvu
		if (languagesByCulture.TryGetValue(cultureName, out tmp))
		{
			return tmp;
		}

		// pokud není nalezeno a jde o specifickou culture ("cs-CZ"), hledáme podle neutrální culture ("cs")
		// (neutrální culture je část před první pomlčkou - NE první dva znaky, jinak by tříznakové jazykové kódy jako "fil-PH" padly na "fi")
		int separatorIndex = cultureName.IndexOf('-');
		if (separatorIndex > 0)
		{
			if (languagesByCulture.TryGetValue(cultureName.Substring(0, separatorIndex), out tmp))
			{
				return tmp;
			}
		}

		// pokud není nalezeno, použijeme výchozí jazyk (je-li stanoven).
		if (languagesByCulture.TryGetValue("", out tmp))
		{
			return tmp;
		}

		throw new InvalidOperationException(String.Format("Language identifier for culture '{0}' was not found.", cultureName));
	}

	/// <summary>
	/// Zajistí načtení jazyků do paměti pro opakované použití.
	/// </summary>
	private Dictionary<string, TLanguageKey> GetLanguagesByCulture()
	{
		if (languageByCultureStorage.Value == null)
		{
			lock (languageByCultureStorage)
			{
				if (languageByCultureStorage.Value == null)
				{
					languageByCultureStorage.Value = languageRepository.GetAll().ToDictionary(item => item.UiCulture, item => entityKeyAccessor.GetEntityKeyValue(item));
				}
			}
		}

		return languageByCultureStorage.Value;
	}
}
