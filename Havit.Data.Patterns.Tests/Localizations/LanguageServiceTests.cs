using System;
using System.Collections.Generic;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Localizations.Internal;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.Tests.Localizations.Model;
using Havit.Model.Localizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Patterns.Tests.Localizations;

[TestClass]
public class LanguageServiceTests
{
	[TestMethod]
	public void LanguageServiceTests_GetLanguage_ReturnsLanguage()
	{
		// Arrange
		Language language1 = new Language { Id = 1, Culture = "cs-CZ", UiCulture = "" };
		Language language2 = new Language { Id = 2, Culture = "en-US", UiCulture = "en" };
		Language language3 = new Language { Id = 3, Culture = "en-GB", UiCulture = "en-GB" };
		Language language4 = new Language { Id = 4, Culture = "sk-SK", UiCulture = "sk-SK" };

		Mock<IRepository<Language>> mockRepository = new Mock<IRepository<Language>>();
		mockRepository.Setup(m => m.GetObject(1)).Returns(language1);
		mockRepository.Setup(m => m.GetObject(2)).Returns(language2);
		mockRepository.Setup(m => m.GetObject(3)).Returns(language3);
		mockRepository.Setup(m => m.GetObject(4)).Returns(language4);
		mockRepository.Setup(m => m.GetAll()).Returns(new List<Language> { language1, language2, language3, language4 });

		Mock<IEntityKeyAccessor<Language, int>> dataEntryIdentifierAccessorMock = new Mock<IEntityKeyAccessor<Language, int>>();
		dataEntryIdentifierAccessorMock.Setup(m => m.GetEntityKeyValue(It.IsAny<Language>())).Returns<Language>(language => language.Id);

		// Act
		LanguageService<Language> dbLanguageService = new LanguageService<Language>(mockRepository.Object, new LanguageByCultureService<Language>(new LanguageByCultureStorage(), mockRepository.Object, dataEntryIdentifierAccessorMock.Object));
		ILanguage languageResult1 = dbLanguageService.GetLanguage("");
		ILanguage languageResult2 = dbLanguageService.GetLanguage("en");
		ILanguage languageResult3 = dbLanguageService.GetLanguage("en-GB");
		ILanguage languageResult4 = dbLanguageService.GetLanguage("sk-SK");
		ILanguage languageResult5 = dbLanguageService.GetLanguage("el-GR");
		ILanguage languageResult6 = dbLanguageService.GetLanguage("en-US");

		// Assert
		Assert.AreSame(language1, languageResult1);
		Assert.AreSame(language2, languageResult2);
		Assert.AreSame(language3, languageResult3);
		Assert.AreSame(language4, languageResult4);
		Assert.AreSame(language1, languageResult5);
		Assert.AreSame(language2, languageResult6);
	}

	[TestMethod]
	public void LanguageServiceTests_GetDefaultLanguage_ReturnsDefaultLanguage()
	{
		// Arrange
		Language language1 = new Language { Id = 1, Culture = "cs-CZ", UiCulture = "" };
		Language language2 = new Language { Id = 2, Culture = "en-US", UiCulture = "en" };
		Language language3 = new Language { Id = 3, Culture = "en-GB", UiCulture = "en-GB" };
		Language language4 = new Language { Id = 4, Culture = "sk-SK", UiCulture = "sk-SK" };

		Mock<IRepository<Language>> mockRepository = new Mock<IRepository<Language>>();
		mockRepository.Setup(m => m.GetObject(1)).Returns(language1);
		mockRepository.Setup(m => m.GetObject(2)).Returns(language2);
		mockRepository.Setup(m => m.GetObject(3)).Returns(language3);
		mockRepository.Setup(m => m.GetObject(4)).Returns(language4);
		mockRepository.Setup(m => m.GetAll()).Returns(new List<Language> { language1, language2, language3, language4 });

		Mock<IEntityKeyAccessor<Language, int>> dataEntryIdentifierAccessorMock = new Mock<IEntityKeyAccessor<Language, int>>();
		dataEntryIdentifierAccessorMock.Setup(m => m.GetEntityKeyValue(It.IsAny<Language>())).Returns<Language>(language => language.Id);

		// Act
		LanguageService<Language> dbLanguageService = new LanguageService<Language>(mockRepository.Object, new LanguageByCultureService<Language>(new LanguageByCultureStorage(), mockRepository.Object, dataEntryIdentifierAccessorMock.Object));
		ILanguage languageResult1 = dbLanguageService.GetDefaultLanguage();

		// Assert
		Assert.AreSame(language1, languageResult1);
	}

}