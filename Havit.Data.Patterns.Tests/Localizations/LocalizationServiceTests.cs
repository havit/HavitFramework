using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Tests.Localizations.Model;
using Havit.Model.Localizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Patterns.Tests.Localizations;

[TestClass]
public class LocalizationServiceTests
{
	[TestMethod]
	public void LocalizationService_GetCurrentLocalization_CallsGetLocalizationWithCurrentLanguage()
	{
		// Arrange
		Language language = new Language()
		{
			UiCulture = ""
		};

		Mock<ICurrentCultureService> currentCultureServiceMock = new Mock<ICurrentCultureService>();
		currentCultureServiceMock.Setup(m => m.GetCurrentCulture()).Returns(CultureInfo.GetCultureInfo("cs-CZ"));
		currentCultureServiceMock.Setup(m => m.GetCurrentUICulture()).Returns(CultureInfo.GetCultureInfo("cs-CZ"));

		Mock<ILanguageService> languageServiceMock = new Mock<ILanguageService>();
		languageServiceMock.Setup(m => m.GetLanguage("cs-CZ")).Returns(language);

		Mock<LocalizationService> localizationServiceMock = new Mock<LocalizationService>(currentCultureServiceMock.Object, languageServiceMock.Object);
		localizationServiceMock.CallBase = true;

		LocalizedEntity localizedEntity = new LocalizedEntity
		{
			Localizations = new List<LocalizedEntityLocalization>()
		};

		// Act
		localizationServiceMock.Object.GetCurrentLocalization(localizedEntity);

		// Assert
		localizationServiceMock.Verify(m => m.GetLocalization(localizedEntity, language), Times.Once);
		localizationServiceMock.Verify(m => m.GetLocalization(It.IsAny<LocalizedEntity>(), It.IsAny<ILanguage>()), Times.Once);
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void LocalizationService_GetLocalization_ThrowsExceptionWhenLocalizationsNotLoaded()
	{
		// Arrange
		Language language = new Language();

		Mock<ICurrentCultureService> currentCultureServiceMock = new Mock<ICurrentCultureService>();
		Mock<ILanguageService> languageServiceMock = new Mock<ILanguageService>();

		LocalizationService localizationService = new LocalizationService(currentCultureServiceMock.Object, languageServiceMock.Object);

		LocalizedEntity localizedEntity = new LocalizedEntity()
		{
			Localizations = null
		};

		// Act
		localizationService.GetLocalization(localizedEntity, language);

		// Assert by method attribute
	}

	[TestMethod]
	public void LocalizationService_GetLocalization_ReturnsInvariantWhenSpecificDoesNotExists()
	{
		// Arrange
		Language languageCsCz = new Language()
		{
			UiCulture = "cs-CZ"
		};

		Language languageCs = new Language()
		{
			UiCulture = "cs"
		};

		Language languageInvariant = new Language()
		{
			UiCulture = ""
		};

		LocalizedEntityLocalization localizedEntityInvariantLocalization = new LocalizedEntityLocalization()
		{
			Language = languageInvariant
		};

		LocalizedEntity localizedEntity = new LocalizedEntity()
		{
			Localizations = new List<LocalizedEntityLocalization> { localizedEntityInvariantLocalization }
		};

		Mock<ICurrentCultureService> currentCultureServiceMock = new Mock<ICurrentCultureService>();
		currentCultureServiceMock.Setup(m => m.GetCurrentCulture()).Returns(CultureInfo.GetCultureInfo("cs-CZ"));
		currentCultureServiceMock.Setup(m => m.GetCurrentUICulture()).Returns(CultureInfo.GetCultureInfo("cs-CZ"));

		Mock<ILanguageService> languageServiceMock = new Mock<ILanguageService>();
		languageServiceMock.Setup(m => m.GetLanguage("cs-CZ")).Returns(languageCsCz);
		languageServiceMock.Setup(m => m.GetLanguage("cs")).Returns(languageCs);
		languageServiceMock.Setup(m => m.GetLanguage("")).Returns(languageInvariant);

		LocalizationService localizationService = new LocalizationService(currentCultureServiceMock.Object, languageServiceMock.Object);

		// Act
		LocalizedEntityLocalization result = localizationService.GetLocalization(localizedEntity, languageCsCz);

		// Assert
		Assert.AreSame(localizedEntityInvariantLocalization, result);
	}

	[TestMethod]
	public void LocalizationService_GetLocalization_ReturnsLocalization()
	{
		// Arrange			
		Language language1 = new Language { Id = 1, Culture = "cs-CZ", UiCulture = "" };
		Language language2 = new Language { Id = 2, Culture = "en-US", UiCulture = "en" };
		Language language3 = new Language { Id = 3, Culture = "en-GB", UiCulture = "en-GB" };
		Language language4 = new Language { Id = 4, Culture = "sk-SK", UiCulture = "sk-SK" };

		List<Language> languages = new List<Language> { language1, language2, language3, language4 };
		Mock<ICurrentCultureService> mockCurrentCulturesService = new Mock<ICurrentCultureService>();

		Mock<ILanguageService> mockLanguageService = new Mock<ILanguageService>();
		mockLanguageService.Setup(m => m.GetLanguage(It.IsAny<string>())).Returns((string cultureName) => languages.SingleOrDefault(language => language.UiCulture == cultureName) ?? language1);

		LocalizationService localizationService = new LocalizationService(mockCurrentCulturesService.Object, mockLanguageService.Object);

		// Act + Assert

		// Ověření, že nachází lokalizace pro jazyk samotný.
		LocalizedEntity entity = new LocalizedEntity
		{
			Localizations = new List<LocalizedEntityLocalization>
			{
				new LocalizedEntityLocalization { Language = language1 },
				new LocalizedEntityLocalization { Language = language2 },
				new LocalizedEntityLocalization { Language = language3 },
				new LocalizedEntityLocalization { Language = language4 },
			}
		};

		Assert.AreSame(language1, localizationService.GetLocalization(entity, language1).Language);
		Assert.AreSame(language2, localizationService.GetLocalization(entity, language2).Language);
		Assert.AreSame(language3, localizationService.GetLocalization(entity, language3).Language);
		Assert.AreSame(language4, localizationService.GetLocalization(entity, language4).Language);

		// Ověření, že nachází lokalizace pro "nadřazený" jazyk.
		entity = new LocalizedEntity
		{
			Localizations = new List<LocalizedEntityLocalization>
			{
				new LocalizedEntityLocalization { Language = language1 },
				new LocalizedEntityLocalization { Language = language2 },
			}
		};

		Assert.AreSame(language1, localizationService.GetLocalization(entity, language1).Language);
		Assert.AreSame(language2, localizationService.GetLocalization(entity, language2).Language);
		Assert.AreSame(language2, localizationService.GetLocalization(entity, language3).Language);
		Assert.AreSame(language1, localizationService.GetLocalization(entity, language4).Language);

		// Ověření, že nachází lokalizace pro invariantní jazyk.
		entity = new LocalizedEntity
		{
			Localizations = new List<LocalizedEntityLocalization>
			{
				new LocalizedEntityLocalization { Language = language1 },
			}
		};

		// Assert
		Assert.AreSame(language1, localizationService.GetLocalization(entity, language1).Language);
		Assert.AreSame(language1, localizationService.GetLocalization(entity, language2).Language);
		Assert.AreSame(language1, localizationService.GetLocalization(entity, language3).Language);
		Assert.AreSame(language1, localizationService.GetLocalization(entity, language4).Language);
	}

	[TestMethod]
	public void LocalizationService_GetLocalization_LanguageInstancesWithoutIdentityMap_MatchesLanguageUsingEquality()
	{
		// arrange
		const int languageId = 10;
		var language = new Language() { Id = languageId, UiCulture = "cs-CZ", Culture = "cs-CZ" };
		var localizedEntity = new LocalizedEntity()
		{
			Localizations = new List<LocalizedEntityLocalization>()
			{
				new LocalizedEntityLocalization() { LanguageId = 5, Language = new Language() { Id = 5, Culture = "en-GB", UiCulture = "en-GB" } },
				new LocalizedEntityLocalization() { LanguageId = languageId, Language = language }
			}

		};
		var languageService = new Mock<ILanguageService>();
		languageService.Setup(s => s.GetLanguage(It.IsAny<string>())).Returns(new Language() { Id = languageId, UiCulture = "cs-CZ", Culture = "cs-CZ" });

		var localizationService = new LocalizationService(new Mock<ICurrentCultureService>().Object, languageService.Object);

		// act
		var result = localizationService.GetLocalization(localizedEntity, new Language() { Id = languageId, UiCulture = "cs-CZ", Culture = "cs-CZ" });

		// assert
		Assert.IsNotNull(result);
		Assert.AreEqual(languageId, result.Language.Id);
	}
}
