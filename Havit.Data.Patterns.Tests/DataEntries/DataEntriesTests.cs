using System.Diagnostics.CodeAnalysis;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.Tests.DataEntries.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Patterns.Tests.DataEntries;

[TestClass]
public class DataEntriesTests
{
	[TestMethod]
	[SuppressMessage("SonicLint", "S1481", Justification = "Výsledek získání hodnoty vlastnosti je potřeba nějak zpracovat, zde jej ukládáme do proměné, která úmyslně není použita.")]
	[SuppressMessage("SonicLint", "S1854", Justification = "Uložení do proměné je zvolený způsob zpracování výsledku získání hodnoty vlastnosti.")]
	public void DbDataEntries_GetEntry_UsesDataEntrySymbolServiceAndRepository()
	{
		// Arrange
		Mock<IDataEntrySymbolService<SystemCodebookEntry>> mockDataEntrySymbolService = new Mock<IDataEntrySymbolService<SystemCodebookEntry>>(MockBehavior.Strict);
		mockDataEntrySymbolService.Setup(mock => mock.GetEntryId(SystemCodebookEntry.Entry.First)).Returns(1);
		Mock<IRepository<SystemCodebookEntry>> mockRepository = new Mock<IRepository<SystemCodebookEntry>>(MockBehavior.Strict);
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
	public void DbDataEntries_GetEntry_GetsObjectByEnumWhenDataEntrySymbolServiceNotUsed()
	{
		// Arrange
		var first = new SystemCodebookEntry();
		Mock<IRepository<SystemCodebookEntry>> mockRepository = new Mock<IRepository<SystemCodebookEntry>>(MockBehavior.Strict);
		mockRepository.Setup(m => m.GetObject(1)).Returns(first);
		SystemCodebookEntryDataEntries supportClassDataEntries = new SystemCodebookEntryDataEntries(mockRepository.Object);

		// Act
		var resultGetEntry = supportClassDataEntries.GetEntry(SystemCodebookEntry.Entry.First);

		// Assert
		Assert.AreSame(first, resultGetEntry);
	}
}
