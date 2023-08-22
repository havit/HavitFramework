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
	private readonly ICurrentCultureService currentCultureService;
	private readonly ILanguageService languageService;

	/// <summary>
	/// Konstruktor. 
	/// </summary>
	public LocalizationService(ICurrentCultureService currentCultureService, ILanguageService languageService)
	{
		this.currentCultureService = currentCultureService;
		this.languageService = languageService;
	}

	/// <summary>
	/// Vrátí položku z lokalizovaných hodnot entity na základě aktuálního jazyka.
	/// </summary>
	/// <param name="entity">Entita, jejíž lokalizaný záznam bude vrácen.</param>
	public virtual TLocalizationEntity GetCurrentLocalization<TLocalizationEntity>(ILocalized<TLocalizationEntity, ILanguage> entity)
		where TLocalizationEntity : class, ILocalization<object, ILanguage>
	{
		string cultureName = currentCultureService.GetCurrentUICulture().Name;
		ILanguage language = languageService.GetLanguage(cultureName);
		return GetLocalization(entity, language);
	}

	/// <summary>
	/// Vrátí položku z lokalizovaných hodnot entity na základě předaného jazyka.
	/// </summary>
	/// <param name="entity">Entita, jejíž lokalizaný záznam bude vrácen.</param>
	/// <param name="language">Jazyk, pro který je lokalizovaný záznam vrácen.</param>
	public virtual TLocalizationEntity GetLocalization<TLocalizationEntity>(ILocalized<TLocalizationEntity, ILanguage> entity, ILanguage language)
			where TLocalizationEntity : class, ILocalization<object, ILanguage>
	{
		Contract.Requires<ArgumentNullException>(entity != null, nameof(entity));
		Contract.Requires<ArgumentNullException>(language != null, nameof(language));
		Contract.Requires<InvalidOperationException>(entity.Localizations != null, "Localized items (entity.Localization) cannot be null.");

		TLocalizationEntity result = entity.Localizations.SingleOrDefault(item => language.Equals(item.Language)); // pokusíme se nalézt lokalizaci pro daný jazyk

		// pokud jsme nic nenašli a použili jsme jazyk s culture ve formátu "cs-CZ", zkusíme hledat "cs".
		if ((result == null) && (language.UiCulture.Length > 2))
		{
			language = languageService.GetLanguage(language.UiCulture.Substring(0, 2));
			result = entity.Localizations.SingleOrDefault(item => language.Equals(item.Language));
		}

		// pokud jsme pořád nic nenašli a použili jsme jazyk s culture ve formátu "cs", zkusíme hledat "".
		if ((result == null) && (language.UiCulture.Length > 0))
		{
			language = languageService.GetLanguage("");
			result = entity.Localizations.SingleOrDefault(item => language.Equals(item.Language));
		}

		return result;
	}
}
