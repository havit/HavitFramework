using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Localizations.Internal;

/// <summary>
/// Služba vrací na základě culture (např. "en-US", "cs-CZ", "sk", "") ID jazyka.
/// Hledá se dle <see cref="ILanguage.UiCulture"/>.
/// Jazykem se rozumí instance třídy modelu (implementující <see cref="ILanguage"/>).
/// Jazyky jsou načteny do lokální proměné a nejsou nikdy invalidovány.
/// </summary>
public class LanguageByCultureService<TLanguage> : ILanguageByCultureService
	where TLanguage : class, ILanguage
{
	private readonly ILanguageByCultureStorage languageByCultureStorage;
	private readonly IRepository<TLanguage> languageRepository; // TODO: QueryTags nedokonalé, bude se hlásit query tag dle DbRepository.
	private readonly IEntityKeyAccessor<TLanguage, int> entityKeyAccessor;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public LanguageByCultureService(ILanguageByCultureStorage languageByCultureStorage, IRepository<TLanguage> languageRepository, IEntityKeyAccessor<TLanguage, int> entityKeyAccessor)
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
	public int GetLanguageId(string cultureName)
	{
		Dictionary<string, int> languagesByCulture = GetLanguagesByCulture();

		int tmp;

		// nejprve zkusíme hledat podle plného názvu
		if (languagesByCulture.TryGetValue(cultureName, out tmp))
		{
			return tmp;
		}

		// pokud není nalezeno, hledáme podle samotného jazyka
		if (cultureName.Length > 2)
		{
			if (languagesByCulture.TryGetValue(cultureName.Substring(0, 2), out tmp))
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
	private Dictionary<string, int> GetLanguagesByCulture()
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
