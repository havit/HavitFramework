using Havit.Data.Entity.Patterns.Localizations;
using Havit.Data.Entity.Patterns.QueryServices.Fakes;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Patterns.QueryServices;
using Havit.Data.Patterns.Repositories;
using Havit.Model.Localizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.Localizations
{
	[TestClass]
	public class DbLanguageServiceTest
	{
		[TestMethod]
		public void DbLanguageService_GetLanguage_ReturnsLanguage()
		{
			// Arrange
			Language language1 = new Language { Id = 1, Culture = "cs-CZ", UiCulture = "" };
			Language language2 = new Language { Id = 2, Culture = "en-US", UiCulture = "en" };
			Language language3 = new Language { Id = 3, Culture = "en-GB", UiCulture = "en-GB" };
			Language language4 = new Language { Id = 4, Culture = "sk-SK", UiCulture = "sk-SK" };

			FakeDataSource<Language> dataSource = new FakeLanguageDataSource(language1, language2, language3, language4);
			Mock<IDataSourceFactory<Language>> mockDataSourceFactory = new Mock<IDataSourceFactory<Language>>();
			mockDataSourceFactory.Setup(m => m.Create()).Returns(dataSource);

			Mock<IRepository<Language>> mockRepository = new Mock<IRepository<Language>>();
			mockRepository.Setup(m => m.GetObject(1)).Returns(language1);
			mockRepository.Setup(m => m.GetObject(2)).Returns(language2);
			mockRepository.Setup(m => m.GetObject(3)).Returns(language3);
			mockRepository.Setup(m => m.GetObject(4)).Returns(language4);

			Mock<IRepositoryFactory<Language>> mockRepositoryFactory = new Mock<IRepositoryFactory<Language>>();
			mockRepositoryFactory.Setup(m => m.Create()).Returns(mockRepository.Object);

			// Act
			DbLanguageService<Language> dbLanguageService = new DbLanguageService<Language>(mockDataSourceFactory.Object, mockRepositoryFactory.Object);
			ILanguage languageResult1 = dbLanguageService.GetLanguage("");
			ILanguage languageResult2 = dbLanguageService.GetLanguage("en");
			ILanguage languageResult3 = dbLanguageService.GetLanguage("en-GB");
			ILanguage languageResult4 = dbLanguageService.GetLanguage("sk-SK");
			ILanguage languageResult5 = dbLanguageService.GetLanguage("el-GR");

			// Assert
			Assert.AreSame(language1, languageResult1);
			Assert.AreSame(language2, languageResult2);
			Assert.AreSame(language3, languageResult3);
			Assert.AreSame(language4, languageResult4);
			Assert.AreSame(language1, languageResult5);
		}
	}
}
