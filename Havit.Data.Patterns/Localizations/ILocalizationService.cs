using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Localizations
{
	/// <summary>
	/// Služba pro získání položky z lokalizovaných hodnot entity na základě jazyka.
	/// Nástupce dřívějšího datového přístupu (entity.Localizations.Current).
	/// Vyžaduje, aby položky v kolekci entity.Localizations byly načtené.
	/// </summary>
	public interface ILocalizationService
	{
		/// <summary>
		/// Vrátí položku z lokalizovaných hodnot entity na základě aktuálního jazyka.
		/// </summary>
		/// <param name="entity">Entita, jejíž lokalizaný záznam bude vrácen.</param>
		TLocalizationEntity GetCurrentLocalization<TLocalizationEntity>(ILocalized<TLocalizationEntity, ILanguage> entity)
			where TLocalizationEntity : class, ILocalization<object, ILanguage>;

		/// <summary>
		/// Vrátí položku z lokalizovaných hodnot entity na základě předaného jazyka.
		/// </summary>
		/// <param name="entity">Entita, jejíž lokalizaný záznam bude vrácen.</param>
		/// <param name="language">Jazyk, pro který se hledá lokalizace.</param>
		TLocalizationEntity GetLocalization<TLocalizationEntity>(ILocalized<TLocalizationEntity, ILanguage> entity, ILanguage language)
			where TLocalizationEntity : class, ILocalization<object, ILanguage>;
	}
}