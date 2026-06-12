using System.Diagnostics.CodeAnalysis;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.Tests.DataEntries.Infrastructure;
using Moq;

namespace Havit.Data.Patterns.Tests.DataEntries;

[TestClass]
public class DataEntriesTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	[SuppressMessage("SonicLint", "S1481", Justification = "Výsledek získání hodnoty vlastnosti je potřeba nějak zpracovat, zde jej ukládáme do proměné, která úmyslně není použita.")]
	[SuppressMessage("SonicLint", "S1854", Justification = "Uložení do proměné je zvolený způsob zpracování výsledku získání hodnoty vlastnosti.")]
	public void DataEntries_GetEntry_UsesDataEntrySymbolServiceAndRepository()
	{
		// Arrange
		Mock<IDataEntrySymbolService<SystemCodebookEntry, int>> mockDataEntrySymbolService = new Mock<IDataEntrySymbolService<SystemCodebookEntry, int>>(MockBehavior.Strict);
		mockDataEntrySymbolService.Setup(mock => mock.GetEntryId(SystemCodebookEntry.Entry.First)).Returns(1);
		Mock<IRepository<SystemCodebookEntry, int>> mockRepository = new Mock<IRepository<SystemCodebookEntry, int>>(MockBehavior.Strict);
		mockRepository.Setup(m => m.GetObject(1)).Returns(new SystemCodebookEntry());
		SystemCodebookEntryDataEntries supportClassDataEntries = new SystemCodebookEntryDataEntries(mockDataEntrySymbolService.Object, mockRepository.Object);

		// Act
		SystemCodebookEntry first = supportClassDataEntries.GetEntry(SystemCodebookEntry.Entry.First);

		// Assert
		mockDataEntrySymbolService.Verify(mock => mock.GetEntryId(SystemCodebookEntry.Entry.First), Times.Once);
		mockDataEntrySymbolService.Verify(mock => mock.GetEntryId(It.IsAny<SystemCodebookEntry.Entry>()), Times.Once);
		mockRepository.Verify(mock => mock.GetObject(1), Times.Once);
		mockRepository.Verify(mock => mock.GetObject(It.IsAny<int>()), Times.Once);
	}

	[TestMethod]
	public void DataEntries_GetEntry_GetsObjectByEnumWhenDataEntrySymbolServiceNotUsed()
	{
		// Arrange
		var first = new SystemCodebookEntry();
		Mock<IRepository<SystemCodebookEntry, int>> mockRepository = new Mock<IRepository<SystemCodebookEntry, int>>(MockBehavior.Strict);
		mockRepository.Setup(m => m.GetObject(1)).Returns(first);
		SystemCodebookEntryDataEntries supportClassDataEntries = new SystemCodebookEntryDataEntries(mockRepository.Object);

		// Act
		var resultGetEntry = supportClassDataEntries.GetEntry(SystemCodebookEntry.Entry.First);

		// Assert
		Assert.AreSame(first, resultGetEntry);
	}

	[TestMethod]
	public async Task DataEntries_GetEntryAsync_UsesDataEntrySymbolServiceAndRepository()
	{
		// Arrange
		SystemCodebookEntry first = new SystemCodebookEntry();
		Mock<IDataEntrySymbolService<SystemCodebookEntry, int>> mockDataEntrySymbolService = new Mock<IDataEntrySymbolService<SystemCodebookEntry, int>>(MockBehavior.Strict);
		mockDataEntrySymbolService.Setup(mock => mock.GetEntryIdAsync(SystemCodebookEntry.Entry.First, It.IsAny<CancellationToken>())).Returns(new ValueTask<int>(1));
		Mock<IRepository<SystemCodebookEntry, int>> mockRepository = new Mock<IRepository<SystemCodebookEntry, int>>(MockBehavior.Strict);
		mockRepository.Setup(m => m.GetObjectAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(first);
		SystemCodebookEntryDataEntries supportClassDataEntries = new SystemCodebookEntryDataEntries(mockDataEntrySymbolService.Object, mockRepository.Object);

		// Act
		SystemCodebookEntry resultGetEntry = await supportClassDataEntries.GetEntryAsync(SystemCodebookEntry.Entry.First, TestContext.CancellationToken);

		// Assert
		Assert.AreSame(first, resultGetEntry);
		mockDataEntrySymbolService.Verify(mock => mock.GetEntryIdAsync(SystemCodebookEntry.Entry.First, It.IsAny<CancellationToken>()), Times.Once);
		mockDataEntrySymbolService.Verify(mock => mock.GetEntryIdAsync(It.IsAny<SystemCodebookEntry.Entry>(), It.IsAny<CancellationToken>()), Times.Once);
		mockRepository.Verify(mock => mock.GetObjectAsync(1, It.IsAny<CancellationToken>()), Times.Once);
		mockRepository.Verify(mock => mock.GetObjectAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[TestMethod]
	public async Task DataEntries_GetEntryAsync_GetsObjectByEnumWhenDataEntrySymbolServiceNotUsed()
	{
		// Arrange
		var first = new SystemCodebookEntry();
		Mock<IRepository<SystemCodebookEntry, int>> mockRepository = new Mock<IRepository<SystemCodebookEntry, int>>(MockBehavior.Strict);
		mockRepository.Setup(m => m.GetObjectAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(first);
		SystemCodebookEntryDataEntries supportClassDataEntries = new SystemCodebookEntryDataEntries(mockRepository.Object);

		// Act
		var resultGetEntry = await supportClassDataEntries.GetEntryAsync(SystemCodebookEntry.Entry.First, TestContext.CancellationToken);

		// Assert
		Assert.AreSame(first, resultGetEntry);
	}
}
