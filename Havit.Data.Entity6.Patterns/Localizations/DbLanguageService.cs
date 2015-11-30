using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.QueryServices;
using Havit.Data.Patterns.Repositories;
using Havit.Model.Localizations;

namespace Havit.Data.Entity.Patterns.Localizations
{
	/// <summary>
	/// Služba vrací aktuální jazyk.
	/// Jazykem se rozumí instance třídy modelu (implementující <see cref="ILanguage"/>).
	/// </summary>
	public class DbLanguageService<TLanguage> : ILanguageService
		where TLanguage : class, ILanguage
	{
		private readonly IDataSourceFactory<TLanguage> languageDataSourceFactory;
		private readonly IRepositoryFactory<TLanguage> languageRepositoryFactory;

		private volatile Dictionary<string, int> languages;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="languageDataSourceFactory">Služba pro získání datového zdroje jazyků.</param>
		/// <param name="languageRepositoryFactory">Služba pro získání repository jazyků.</param>
		public DbLanguageService(IDataSourceFactory<TLanguage> languageDataSourceFactory, IRepositoryFactory<TLanguage> languageRepositoryFactory)
		{
			this.languageDataSourceFactory = languageDataSourceFactory;
			this.languageRepositoryFactory = languageRepositoryFactory;
		}

		/// <summary>
		/// Vrací akutální jazyk.
		/// </summary>
		public ILanguage GetLanguage(string cultureName)
		{
			int languageId = GetLanguageId(cultureName);
			return GetLanguageById(languageId);
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
						IDataSource<TLanguage> languageDataSource = languageDataSourceFactory.Create();
						try
						{
							languages = languageDataSource.Data.ToList().ToDictionary(item => item.UiCulture, item => ((int)((dynamic)item).Id));
						}
						finally
						{
							languageDataSourceFactory.Release(languageDataSource);
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
