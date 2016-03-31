using System.Diagnostics.CodeAnalysis;
using Havit.Data.Entity.Patterns.Tests.DataEntries.DataSources;
using Havit.Data.Entity.Patterns.Tests.DataEntries.Model;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.DataEntries
{
	// TODO: Přesunout do Havit.Data.Patterns.Tests

	[TestClass]
	public class DataEntriesTest
	{
		[TestMethod]		
		[SuppressMessage("SonicLint", "S1481", Justification = "Výsledek získání hodnoty vlastnosti je potřeba nějak zpracovat, zde jej ukládáme do proměné, která úmyslně není použita.")]
		[SuppressMessage("SonicLint", "S1854", Justification = "Uložení do proměné je zvolený způsob zpracování výsledku získání hodnoty vlastosti.")]
		public void DbDataEntries_GetEntry_UsesDataEntrySymbolStorageAndRepository()
		{
			// Arrange
			Mock<IDataEntrySymbolStorage<SupportedClass>> mockDataEntrySymbolStorage = new Mock<IDataEntrySymbolStorage<SupportedClass>>();
			mockDataEntrySymbolStorage.Setup(mock => mock.GetEntryId(SupportedClass.Entry.First)).Returns(1);
			Mock<IRepository<SupportedClass>> mockRepository = new Mock<IRepository<SupportedClass>>();			
			SupportedClassDataEntries supportClassDataEntries = new SupportedClassDataEntries(mockDataEntrySymbolStorage.Object, mockRepository.Object);

			// Act
			SupportedClass first = supportClassDataEntries.GetEntry(SupportedClass.Entry.First);

			// Assert
			mockDataEntrySymbolStorage.Verify(mock => mock.GetEntryId(SupportedClass.Entry.First), Times.Once);
			mockDataEntrySymbolStorage.Verify(mock => mock.GetEntryId(It.IsAny<SupportedClass.Entry>()), Times.Once);
			mockRepository.Verify(mock => mock.GetObject(1), Times.Once);
			mockRepository.Verify(mock => mock.GetObject(It.IsAny<int>()), Times.Once);
		}
	}
}
