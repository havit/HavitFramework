using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Localizations.Internal;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.Tests.Localizations.Model;
using Moq;

namespace Havit.Data.Patterns.Tests.Localizations;

[TestClass]
public class LanguageByCultureServiceTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void LanguageByCultureService_GetLanguageId_ReturnsLanguageByExactCulture()
	{
		// Arrange
		LanguageByCultureService<Language, int> service = CreateService(
			new Language { Id = 1, UiCulture = "" },
			new Language { Id = 2, UiCulture = "cs" },
			new Language { Id = 3, UiCulture = "cs-CZ" });

		// Act + Assert
		Assert.AreEqual(3, service.GetLanguageId("cs-CZ"));
		Assert.AreEqual(2, service.GetLanguageId("cs"));
		Assert.AreEqual(1, service.GetLanguageId(""));
	}

	[TestMethod]
	public void LanguageByCultureService_GetLanguageId_FallsBackToNeutralCulture()
	{
		// Arrange
		LanguageByCultureService<Language, int> service = CreateService(
			new Language { Id = 1, UiCulture = "" },
			new Language { Id = 2, UiCulture = "cs" });

		// Act + Assert
		Assert.AreEqual(2, service.GetLanguageId("cs-CZ"));
	}

	[TestMethod]
	public void LanguageByCultureService_GetLanguageId_DoesNotShortenNeutralCultureToTwoLetters()
	{
		// Arrange
		// Neutrální culture tříznakového jazykového kódu je "fil", nikoliv "fi" (finština).
		LanguageByCultureService<Language, int> service = CreateService(
			new Language { Id = 1, UiCulture = "" },
			new Language { Id = 2, UiCulture = "fi" },
			new Language { Id = 3, UiCulture = "fil" });

		// Act + Assert
		Assert.AreEqual(3, service.GetLanguageId("fil-PH"));
	}

	[TestMethod]
	public void LanguageByCultureService_GetLanguageId_DoesNotFallBackToDifferentLanguageWithSamePrefix()
	{
		// Arrange
		// Pro "fil-PH" nesmí být vrácena finština ("fi"), byť má společný dvouznakový prefix - očekává se výchozí jazyk.
		LanguageByCultureService<Language, int> service = CreateService(
			new Language { Id = 1, UiCulture = "" },
			new Language { Id = 2, UiCulture = "fi" });

		// Act + Assert
		Assert.AreEqual(1, service.GetLanguageId("fil-PH"));
	}

	[TestMethod]
	public void LanguageByCultureService_GetLanguageId_FallsBackToDefaultLanguage()
	{
		// Arrange
		LanguageByCultureService<Language, int> service = CreateService(
			new Language { Id = 1, UiCulture = "" },
			new Language { Id = 2, UiCulture = "cs" });

		// Act + Assert
		Assert.AreEqual(1, service.GetLanguageId("el-GR"));
	}

	[TestMethod]
	public void LanguageByCultureService_GetLanguageId_ThrowsExceptionWhenLanguageNotFound()
	{
		// Arrange
		LanguageByCultureService<Language, int> service = CreateService(
			new Language { Id = 2, UiCulture = "cs" }); // bez výchozího jazyka (UiCulture = "")

		// Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			// Act
			service.GetLanguageId("el-GR");
		});
	}

	[TestMethod]
	public async Task LanguageByCultureService_GetLanguageIdAsync_ReturnsLanguageWithFallback()
	{
		// Arrange
		LanguageByCultureService<Language, int> service = CreateService(
			new Language { Id = 1, UiCulture = "" },
			new Language { Id = 2, UiCulture = "cs" },
			new Language { Id = 3, UiCulture = "en-GB" });

		// Act + Assert
		Assert.AreEqual(3, await service.GetLanguageIdAsync("en-GB", TestContext.CancellationToken));
		Assert.AreEqual(2, await service.GetLanguageIdAsync("cs-CZ", TestContext.CancellationToken));
		Assert.AreEqual(1, await service.GetLanguageIdAsync("el-GR", TestContext.CancellationToken));
	}

	[TestMethod]
	public async Task LanguageByCultureService_GetLanguageIdAsync_ThrowsExceptionWhenLanguageNotFound()
	{
		// Arrange
		LanguageByCultureService<Language, int> service = CreateService(
			new Language { Id = 2, UiCulture = "cs" }); // bez výchozího jazyka (UiCulture = "")

		// Assert
		await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
		{
			// Act
			await service.GetLanguageIdAsync("el-GR", TestContext.CancellationToken);
		});
	}

	private static LanguageByCultureService<Language, int> CreateService(params Language[] languages)
	{
		Mock<IRepository<Language, int>> repositoryMock = new Mock<IRepository<Language, int>>(MockBehavior.Strict);
		repositoryMock.Setup(m => m.GetAll()).Returns(languages.ToList());
		repositoryMock.Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(languages.ToList());

		Mock<IEntityKeyAccessor<Language, int>> entityKeyAccessorMock = new Mock<IEntityKeyAccessor<Language, int>>(MockBehavior.Strict);
		entityKeyAccessorMock.Setup(m => m.GetEntityKeyValue(It.IsAny<Language>())).Returns((Language language) => language.Id);

		return new LanguageByCultureService<Language, int>(new LanguageByCultureStorage<int>(), repositoryMock.Object, entityKeyAccessorMock.Object);
	}
}
