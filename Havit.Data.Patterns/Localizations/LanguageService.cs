﻿using Havit.Data.Patterns.Repositories;
using Havit.Model.Localizations;
using Havit.Data.Patterns.Localizations.Internal;

namespace Havit.Data.Patterns.Localizations;

/// <summary>
/// Služba vrací aktuální jazyk nebo jazyk dle culture.
/// Jazykem se rozumí instance třídy modelu (implementující <see cref="ILanguage"/>).
/// </summary>	
public class LanguageService<TLanguage, TLanguageKey> : ILanguageService
	where TLanguage : class, ILanguage
{
	private readonly IRepository<TLanguage, TLanguageKey> _languageRepository; // TODO: QueryTags nedokonalé, bude se hlásit query tag dle DbRepository.
	private readonly ILanguageByCultureService<TLanguageKey> _languageByCultureService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public LanguageService(IRepository<TLanguage, TLanguageKey> languageRepository, ILanguageByCultureService<TLanguageKey> languageByCultureService)
	{
		this._languageRepository = languageRepository;
		this._languageByCultureService = languageByCultureService;
	}

	/// <summary>
	/// Vrací výchozí jazyk (vyhledáním pro prázdnou cultureName).
	/// </summary>
	public virtual ILanguage GetDefaultLanguage()
	{
		return GetLanguage("");
	}

	/// <summary>
	/// Vrací jazyk pro danou culture.
	/// </summary>
	public ILanguage GetLanguage(string cultureName)
	{
		TLanguageKey languageId = _languageByCultureService.GetLanguageId(cultureName);
		return _languageRepository.GetObject(languageId);
	}
}
