using Havit.Diagnostics.Contracts;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Localizations;

/// <summary>
/// Služba pro získání položky z lokalizovaných hodnot entity na základě aktuálního jazyka.
/// Nástupce dřívějšího datového přístupu (entity.Localizations.Current).
/// Vyžaduje, aby položky v kolekci entity.Localizations byly načtené.
/// </summary>
public class LocalizationService : ILocalizationService
{
	private readonly ICurrentCultureService _currentCultureService;
	private readonly ILanguageService _languageService;

	/// <summary>
	/// Konstruktor. 
	/// </summary>
	public LocalizationService(ICurrentCultureService currentCultureService, ILanguageService languageService)
	{
		this._currentCultureService = currentCultureService;
		this._languageService = languageService;
	}

	/// <summary>
	/// Vrátí položku z lokalizovaných hodnot entity na základě aktuálního jazyka.
	/// </summary>
	/// <param name="entity">Entita, jejíž lokalizovaný záznam bude vrácen.</param>
	public virtual TLocalizationEntity GetCurrentLocalization<TLocalizationEntity>(ILocalized<TLocalizationEntity, ILanguage> entity)
		where TLocalizationEntity : class, ILocalization<object, ILanguage>
	{
		string cultureName = _currentCultureService.GetCurrentUICulture().Name;
		ILanguage language = _languageService.GetLanguage(cultureName);
		return GetLocalization(entity, language);
	}

	/// <summary>
	/// Vrátí položku z lokalizovaných hodnot entity na základě předaného jazyka.
	/// </summary>
	/// <param name="entity">Entita, jejíž lokalizovaný záznam bude vrácen.</param>
	/// <param name="language">Jazyk, pro který je lokalizovaný záznam vrácen.</param>
	public virtual TLocalizationEntity GetLocalization<TLocalizationEntity>(ILocalized<TLocalizationEntity, ILanguage> entity, ILanguage language)
			where TLocalizationEntity : class, ILocalization<object, ILanguage>
	{
		Contract.Requires<ArgumentNullException>(entity != null, nameof(entity));
		Contract.Requires<ArgumentNullException>(language != null, nameof(language));
		Contract.Requires<InvalidOperationException>(entity.Localizations != null, "Localized items (entity.Localization) cannot be null.");

		TLocalizationEntity result = GetLocalizationByLanguage(entity.Localizations, language); // pokusíme se nalézt lokalizaci pro daný jazyk

		// pokud jsme nic nenašli a použili jsme specifickou culture ("cs-CZ"), zkusíme neutrální culture ("cs").
		// (neutrální culture je část před první pomlčkou - NE první dva znaky, jinak by tříznakové jazykové kódy jako "fil-PH" padly na "fi")
		int separatorIndex = language.UiCulture.IndexOf('-');
		if ((result == null) && (separatorIndex > 0))
		{
			language = _languageService.GetLanguage(language.UiCulture.Substring(0, separatorIndex));
			result = GetLocalizationByLanguage(entity.Localizations, language);
		}

		// pokud jsme pořád nic nenašli a použili jsme jazyk s culture ve formátu "cs", zkusíme hledat "".
		if ((result == null) && (language.UiCulture.Length > 0))
		{
			language = _languageService.GetLanguage("");
			result = GetLocalizationByLanguage(entity.Localizations, language);
		}

		return result;
	}

	/// <summary>
	/// Vrátí lokalizaci pro daný jazyk (či null, není-li nalezena).
	/// </summary>
	/// <remarks>
	/// PERF: Záměrně ruční cyklus namísto LINQ - metoda je volána z render hot-path a ruční cyklus nealokuje closure ani delegát a končí u prvního nálezu.
	/// Dříve použitý SingleOrDefault navíc kontroloval unikátnost lokalizace (procházel tak vždy celou kolekci);
	/// unikátnost lokalizace pro daný jazyk však zajišťuje unikátní databázový index (viz LocalizationTableIndexConvention), kontrola za běhu proto není potřeba.
	/// </remarks>
	private static TLocalizationEntity GetLocalizationByLanguage<TLocalizationEntity>(IEnumerable<TLocalizationEntity> localizations, ILanguage language)
		where TLocalizationEntity : class, ILocalization<object, ILanguage>
	{
		foreach (TLocalizationEntity localization in localizations)
		{
			if (language.Equals(localization.Language))
			{
				return localization;
			}
		}

		return null;
	}
}
