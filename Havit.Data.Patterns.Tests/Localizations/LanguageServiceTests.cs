using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Localizations.Internal;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.Tests.Localizations.Model;
using Havit.Model.Localizations;
using Moq;

namespace Havit.Data.Patterns.Tests.Localizations;

[TestClass]
public class LanguageServiceTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void LanguageService_GetLanguage_ReturnsLanguage()
	{
		// Arrange
		Language[] languages = CreateLanguages();
		LanguageService<Language, int> dbLanguageService = CreateLanguageService(languages);

		// Act
		ILanguage languageResult1 = dbLanguageService.GetLanguage("");
		ILanguage languageResult2 = dbLanguageService.GetLanguage("en");
		ILanguage languageResult3 = dbLanguageService.GetLanguage("en-GB");
		ILanguage languageResult4 = dbLanguageService.GetLanguage("sk-SK");
		ILanguage languageResult5 = dbLanguageService.GetLanguage("el-GR");
		ILanguage languageResult6 = dbLanguageService.GetLanguage("en-US");

		// Assert
		Assert.AreSame(languages[0], languageResult1);
		Assert.AreSame(languages[1], languageResult2);
		Assert.AreSame(languages[2], languageResult3);
		Assert.AreSame(languages[3], languageResult4);
		Assert.AreSame(languages[0], languageResult5); // neznámá culture -> výchozí (invariantní) jazyk
		Assert.AreSame(languages[1], languageResult6); // "en-US" nenalezeno -> neutrální "en"
	}

	[TestMethod]
	public async Task LanguageService_GetLanguageAsync_ReturnsLanguage()
	{
		// Arrange
		Language[] languages = CreateLanguages();
		LanguageService<Language, int> dbLanguageService = CreateLanguageService(languages);

		// Act
		ILanguage languageResult1 = await dbLanguageService.GetLanguageAsync("", TestContext.CancellationToken);
		ILanguage languageResult2 = await dbLanguageService.GetLanguageAsync("en", TestContext.CancellationToken);
		ILanguage languageResult3 = await dbLanguageService.GetLanguageAsync("en-GB", TestContext.CancellationToken);
		ILanguage languageResult4 = await dbLanguageService.GetLanguageAsync("sk-SK", TestContext.CancellationToken);
		ILanguage languageResult5 = await dbLanguageService.GetLanguageAsync("el-GR", TestContext.CancellationToken);
		ILanguage languageResult6 = await dbLanguageService.GetLanguageAsync("en-US", TestContext.CancellationToken);

		// Assert
		Assert.AreSame(languages[0], languageResult1);
		Assert.AreSame(languages[1], languageResult2);
		Assert.AreSame(languages[2], languageResult3);
		Assert.AreSame(languages[3], languageResult4);
		Assert.AreSame(languages[0], languageResult5); // neznámá culture -> výchozí (invariantní) jazyk
		Assert.AreSame(languages[1], languageResult6); // "en-US" nenalezeno -> neutrální "en"
	}

	[TestMethod]
	public void LanguageService_GetDefaultLanguage_ReturnsDefaultLanguage()
	{
		// Arrange
		Language[] languages = CreateLanguages();
		LanguageService<Language, int> dbLanguageService = CreateLanguageService(languages);

		// Act
		ILanguage languageResult1 = dbLanguageService.GetDefaultLanguage();

		// Assert
		Assert.AreSame(languages[0], languageResult1);
	}

	[TestMethod]
	public async Task LanguageService_GetDefaultLanguageAsync_ReturnsDefaultLanguage()
	{
		// Arrange
		Language[] languages = CreateLanguages();
		LanguageService<Language, int> dbLanguageService = CreateLanguageService(languages);

		// Act
		ILanguage languageResult1 = await dbLanguageService.GetDefaultLanguageAsync(TestContext.CancellationToken);

		// Assert
		Assert.AreSame(languages[0], languageResult1);
	}

	/// <summary>
	/// Vytvoří sadu jazyků: výchozí (invariantní, UiCulture ""), neutrální "en", specifické "en-GB" a "sk-SK".
	/// </summary>
	private static Language[] CreateLanguages()
	{
		return new[]
		{
			new Language { Id = 1, Culture = "cs-CZ", UiCulture = "" },
			new Language { Id = 2, Culture = "en-US", UiCulture = "en" },
			new Language { Id = 3, Culture = "en-GB", UiCulture = "en-GB" },
			new Language { Id = 4, Culture = "sk-SK", UiCulture = "sk-SK" },
		};
	}

	/// <summary>
	/// Sestaví <see cref="LanguageService{TLanguage, TLanguageKey}"/> nad mockem repository naplněným předanými jazyky.
	/// </summary>
	private static LanguageService<Language, int> CreateLanguageService(Language[] languages)
	{
		Mock<IRepository<Language, int>> mockRepository = new Mock<IRepository<Language, int>>();
		foreach (Language language in languages)
		{
			mockRepository.Setup(m => m.GetObject(language.Id)).Returns(language);
			mockRepository.Setup(m => m.GetObjectAsync(language.Id, It.IsAny<CancellationToken>())).ReturnsAsync(language);
		}
		mockRepository.Setup(m => m.GetAll()).Returns(languages.ToList());
		mockRepository.Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(languages.ToList());

		Mock<IEntityKeyAccessor<Language, int>> entityKeyAccessorMock = new Mock<IEntityKeyAccessor<Language, int>>();
		entityKeyAccessorMock.Setup(m => m.GetEntityKeyValue(It.IsAny<Language>())).Returns<Language>(language => language.Id);

		return new LanguageService<Language, int>(mockRepository.Object, new LanguageByCultureService<Language, int>(new LanguageByCultureStorage<int>(), mockRepository.Object, entityKeyAccessorMock.Object));
	}
}
