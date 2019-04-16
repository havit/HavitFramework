using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Havit.Model.Localizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.Localizations.Internal
{
	/// <summary>
	/// Služba vrací na základě culture (např. "en-US", "cs-CZ", "sk", "") ID jazyka.
	/// Jazykem se rozumí instance třídy modelu (implementující <see cref="ILanguage"/>).
	/// Jazyky jsou načteny do lokální proměné a nejsou nikdy invalidovány.
	/// </summary>
	/// <remarks>
	/// Revize použití s ohledem na https://github.com/volosoft/castle-windsor-ms-adapter/issues/32:
	/// Tato třída vznikla jako řešení tohoto problému (v dřívejší implementaci LanguageService).
	/// Repository je registrována jako scoped, proto se této factory popsaná issue týká.
	/// Použití v metodě EnsureLanguages: Jde o přečtení dat z databáze a jejich zpracování. Použití je zde bezpečné, použití nové Repository a DbContextu zde nevadí.
	/// </remarks>	
	public class LanguageByCultureService<TLanguage> : ILanguageByCultureService
		where TLanguage : class, ILanguage
	{
		private readonly IRepositoryFactory<TLanguage> languageRepositoryFactory;
		private readonly IEntityKeyAccessor<TLanguage, int> entityKeyAccessor;

		private volatile Dictionary<string, int> languages;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public LanguageByCultureService(IRepositoryFactory<TLanguage> languageRepositoryFactory, IEntityKeyAccessor<TLanguage, int> entityKeyAccessor)
		{
			this.languageRepositoryFactory = languageRepositoryFactory;
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
			EnsureLanguages();

			int tmp;

			// nejprve zkusíme hledat podle plného názvu
			if (languages.TryGetValue(cultureName, out tmp))
			{
				return tmp;
			}

			// pokud není nalezeno, hledáme podle samotného jazyka
			if (cultureName.Length > 2)
			{
				if (languages.TryGetValue(cultureName.Substring(0, 2), out tmp))
				{
					return tmp;
				}
			}

			// pokud není nalezeno, použijeme výchozí jazyk (je-li stanoven).
			if (languages.TryGetValue("", out tmp))
			{
				return tmp;
			}

			throw new InvalidOperationException(String.Format("Language identifier for culture '{0}' was not found.", cultureName));
		}

		/// <summary>
		/// Zajistí načtení jazyků do poměti pro opakované použití.
		/// </summary>
		private void EnsureLanguages()
		{
			if (languages == null)
			{
				lock (_ensureLanguagesLock)
				{
					if (languages == null)
					{
						IRepository<TLanguage> languageRepository = languageRepositoryFactory.Create();
						try
						{
							languages = languageRepository.GetAll().ToDictionary(item => item.UiCulture, item => entityKeyAccessor.GetEntityKeyValue(item));
						}
						finally
						{
							languageRepositoryFactory.Release(languageRepository);
						}
					}
				}
			}
		}
		private readonly object _ensureLanguagesLock = new object();
	}
}
