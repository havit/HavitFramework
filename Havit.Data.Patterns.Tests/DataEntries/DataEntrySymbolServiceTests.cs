using System.Diagnostics.CodeAnalysis;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Exceptions;
using Havit.Data.Patterns.Tests.DataEntries.Infrastructure;
using Moq;

namespace Havit.Data.Patterns.Tests.DataEntries;

[TestClass]
public class DataEntrySymbolServiceTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void DataEntrySymbolService_GetEntryId_ReturnsId()
	{
		// Arrange
		DataEntrySymbolService<SymbolCodebookEntry, int> service = CreateService(
			new SymbolCodebookEntry { Id = 1, Symbol = SymbolCodebookEntry.Entry.First.ToString() },
			new SymbolCodebookEntry { Id = 2, Symbol = SymbolCodebookEntry.Entry.Second.ToString() });

		// Act + Assert
		Assert.AreEqual(2, service.GetEntryId(SymbolCodebookEntry.Entry.Second));
	}

	[TestMethod]
	public void DataEntrySymbolService_GetEntryId_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		DataEntrySymbolService<SymbolCodebookEntry, int> service = CreateService(); // žádná data

		// Assert
		Assert.ThrowsExactly<ObjectNotFoundException>(() =>
		{
			// Act
			service.GetEntryId(SymbolCodebookEntry.Entry.First);
		});
	}

	[TestMethod]
	[SuppressMessage("SonicLint", "S1848", Justification = "Pravidlo říká, nemáme vytvořit instanci, kterou pak nepoužijeme. Zde však testujeme vytvoření instance a k ničemu dalšímu ji nepotřebujeme.")]
	public void DataEntrySymbolService_Constructor_ThrowsExceptionWhenNotSupported()
	{
		// Arrange
		Mock<IDataSource<EntityWithoutSymbol>> dataSourceMock = new Mock<IDataSource<EntityWithoutSymbol>>(MockBehavior.Strict);

		// Assert
		Assert.ThrowsExactly<NotSupportedException>(() =>
		{
			// Act
			new DataEntrySymbolService<EntityWithoutSymbol, int>(new DataEntrySymbolStorage<EntityWithoutSymbol, int>(), dataSourceMock.Object);
		});
	}

	[TestMethod]
	public void DataEntrySymbolService_GetEntryId_SupportsDeletedObjects()
	{
		// Arrange
		// Služba čte z DataIncludingDeleted, proto musí najít i objekt smazaný příznakem.
		DataEntrySymbolService<SymbolCodebookEntry, int> service = CreateService(
			new SymbolCodebookEntry { Id = 1, Symbol = SymbolCodebookEntry.Entry.First.ToString(), Deleted = DateTime.Now });

		// Act + Assert
		Assert.AreEqual(1, service.GetEntryId(SymbolCodebookEntry.Entry.First));
	}

	[TestMethod]
	public void DataEntrySymbolService_GetEntryId_SkipsNullAndEmptySymbols()
	{
		// Arrange
		DataEntrySymbolService<SymbolCodebookEntry, int> service = CreateService(
			new SymbolCodebookEntry { Id = 1, Symbol = SymbolCodebookEntry.Entry.First.ToString() },
			new SymbolCodebookEntry { Id = 2, Symbol = null },
			new SymbolCodebookEntry { Id = 3, Symbol = String.Empty });

		// Act + Assert (prázdné a null symboly nesmí způsobit kolizi klíčů ve slovníku)
		Assert.AreEqual(1, service.GetEntryId(SymbolCodebookEntry.Entry.First));
	}

	[TestMethod]
	public async Task DataEntrySymbolService_GetEntryIdAsync_ReturnsId()
	{
		// Arrange
		DataEntrySymbolService<SymbolCodebookEntry, int> service = CreateService(
			new SymbolCodebookEntry { Id = 1, Symbol = SymbolCodebookEntry.Entry.First.ToString() },
			new SymbolCodebookEntry { Id = 2, Symbol = SymbolCodebookEntry.Entry.Second.ToString() });

		// Act + Assert
		Assert.AreEqual(2, await service.GetEntryIdAsync(SymbolCodebookEntry.Entry.Second, TestContext.CancellationToken));
	}

	[TestMethod]
	public async Task DataEntrySymbolService_GetEntryIdAsync_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		DataEntrySymbolService<SymbolCodebookEntry, int> service = CreateService(); // žádná data

		// Assert
		await Assert.ThrowsExactlyAsync<ObjectNotFoundException>(async () =>
		{
			// Act
			await service.GetEntryIdAsync(SymbolCodebookEntry.Entry.First, TestContext.CancellationToken);
		});
	}

	[TestMethod]
	[SuppressMessage("SonicLint", "S1848", Justification = "Pravidlo říká, nemáme vytvořit instanci, kterou pak nepoužijeme. Zde však testujeme vytvoření instance a k ničemu dalšímu ji nepotřebujeme.")]
	public void DataEntrySymbolService_Constructor_ThrowsExceptionWhenSymbolPropertyIsNotString()
	{
		// Arrange
		Mock<IDataSource<EntityWithIntSymbol>> dataSourceMock = new Mock<IDataSource<EntityWithIntSymbol>>(MockBehavior.Strict);

		// Assert
		Assert.ThrowsExactly<NotSupportedException>(() =>
		{
			// Act (vlastnost Symbol je typu int, nikoliv string)
			new DataEntrySymbolService<EntityWithIntSymbol, int>(new DataEntrySymbolStorage<EntityWithIntSymbol, int>(), dataSourceMock.Object);
		});
	}

	[TestMethod]
	public void DataEntrySymbolService_GetEntryId_QueriesDataSourceOnlyOnce()
	{
		// Arrange
		Mock<IDataSource<SymbolCodebookEntry>> dataSourceMock = new Mock<IDataSource<SymbolCodebookEntry>>(MockBehavior.Strict);
		dataSourceMock.Setup(m => m.DataIncludingDeleted).Returns(new[] { new SymbolCodebookEntry { Id = 1, Symbol = SymbolCodebookEntry.Entry.First.ToString() } }.AsQueryable());
		DataEntrySymbolService<SymbolCodebookEntry, int> service = new DataEntrySymbolService<SymbolCodebookEntry, int>(new DataEntrySymbolStorage<SymbolCodebookEntry, int>(), dataSourceMock.Object);

		// Act
		service.GetEntryId(SymbolCodebookEntry.Entry.First);
		service.GetEntryId(SymbolCodebookEntry.Entry.First);

		// Assert (data se z datového zdroje načtou jen jednou, dále se používá cache ve storage)
		dataSourceMock.Verify(m => m.DataIncludingDeleted, Times.Once);
	}

	private static DataEntrySymbolService<SymbolCodebookEntry, int> CreateService(params SymbolCodebookEntry[] entries)
	{
		Mock<IDataSource<SymbolCodebookEntry>> dataSourceMock = new Mock<IDataSource<SymbolCodebookEntry>>(MockBehavior.Strict);
		dataSourceMock.Setup(m => m.DataIncludingDeleted).Returns(entries.AsQueryable());

		return new DataEntrySymbolService<SymbolCodebookEntry, int>(new DataEntrySymbolStorage<SymbolCodebookEntry, int>(), dataSourceMock.Object);
	}
}
