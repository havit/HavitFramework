using Havit.Data.Patterns.Repositories;
using Havit.Model.Localizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.Infrastructure;

namespace Havit.Data.Patterns.Localizations
{
	/// <summary>
	/// Služba vrací aktuální jazyk.
	/// Jazykem se rozumí instance třídy modelu (implementující <see cref="ILanguage"/>).
	/// </summary>
	public class LanguageService<TLanguage> : ILanguageService
		where TLanguage : class, ILanguage
	{
		private readonly IRepositoryFactory<TLanguage> languageRepositoryFactory;
		private readonly IEntityKeyAccessor<TLanguage, int> entityKeyAccessor;

		private volatile Dictionary<string, int> languages;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="languageRepositoryFactory">Služba pro získání repository jazyků.</param>
		/// <param name="entityKeyAccessor">Služba pro získání identifikátoru entity.</param>
		public LanguageService(IRepositoryFactory<TLanguage> languageRepositoryFactory, IEntityKeyAccessor<TLanguage, int> entityKeyAccessor)
		{
			this.languageRepositoryFactory = languageRepositoryFactory;
			this.entityKeyAccessor = entityKeyAccessor;
		}

		/// <summary>
		/// Vrací jazyk pro danou culture.
		/// </summary>
		public ILanguage GetLanguage(string cultureName)
		{
			int languageId = GetLanguageId(cultureName);
			return GetLanguageById(languageId);
		}

		/// <summary>
		/// Vrací výchozí jazyk (vyhledáním pro prázdnou cultureName).
		/// </summary>
		public virtual ILanguage GetDefaultLanguage()
		{
			return GetLanguage("");
		}

		/// <summary>
		/// Vrací identifikátor jazyka podle culture.		
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Není-li jazyk podle culture nalezen.
		/// </exception>
		private int GetLanguageId(string cultureName)
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
							languages = languageRepository.GetAll().ToDictionary(item => item.UiCulture, item => entityKeyAccessor.GetEntityKey(item));
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

		/// <summary>
		/// Vrací jazyk podle identifikátoru.
		/// </summary>
		/// <param name="languageId">The language identifier.</param>
		/// <returns>ILanguage.</returns>
		private ILanguage GetLanguageById(int languageId)
		{
			ILanguage result;
			IRepository<TLanguage> languageRepository = languageRepositoryFactory.Create();
			try
			{
				result = languageRepository.GetObject(languageId);
			}
			finally
			{
				languageRepositoryFactory.Release(languageRepository);
			}
			return result;
		}

	}
}
