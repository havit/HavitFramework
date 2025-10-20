using System.Diagnostics.CodeAnalysis;
using Havit.Data.Entity.Patterns.Tests.DataEntries.DataSources;
using Havit.Data.Entity.Patterns.Tests.DataEntries.Model;
using Havit.Data.Patterns.DataEntries;

namespace Havit.Data.Entity.Patterns.Tests.DataEntries;

// TODO: Přesunout do Havit.Data.Patterns.Tests

[TestClass]
public class DataEntrySymbolServiceTests
{
	[TestMethod]
	public void DbDataEntrySymbolService_GetEntryId_ReturnsId()
	{
		// Arrange
		FakeSupportedClassDataSource fakeDataSource = new FakeSupportedClassDataSource(
			new SupportedClass { Id = 1, Symbol = SupportedClass.Entry.First.ToString() },
			new SupportedClass { Id = 2, Symbol = SupportedClass.Entry.Second.ToString() },
			new SupportedClass { Id = 3, Symbol = SupportedClass.Entry.Third.ToString() });

		// Act
		DataEntrySymbolService<SupportedClass, int> dbDataEntrySymbolService = new DataEntrySymbolService<SupportedClass, int>(new DataEntrySymbolStorage<SupportedClass, int>(), fakeDataSource);
		int id = dbDataEntrySymbolService.GetEntryId(SupportedClass.Entry.Second);

		// Assert
		Assert.AreEqual(2, id);
	}

	[TestMethod]
	public void DbDataEntrySymbolService_GetEntryId_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		FakeSupportedClassDataSource fakeDataSource = new FakeSupportedClassDataSource();
		DataEntrySymbolService<SupportedClass, int> dbDataEntrySymbolService = new DataEntrySymbolService<SupportedClass, int>(new DataEntrySymbolStorage<SupportedClass, int>(), fakeDataSource);

		// Assert
		Assert.ThrowsExactly<Havit.Data.Patterns.Exceptions.ObjectNotFoundException>(() =>
		{
			// Act
			dbDataEntrySymbolService.GetEntryId(SupportedClass.Entry.First);
		});
	}

	[TestMethod]
	[SuppressMessage("SonicLint", "S1848", Justification = "Pravidlo říká, nemáme vytvořit instanci, kterou pak nepoužijeme. Zde však testujeme vytvoření instance a k ničemu dalšímu ji nepotřebujeme.")]
	public void DbDataEntrySymbolService_GetEntryId_ThrowsExceptionWhenNotSupported()
	{
		// Arrange
		FakeNotSupportedClassDataSource fakeDataSource = new FakeNotSupportedClassDataSource();

		// Assert
		Assert.ThrowsExactly<NotSupportedException>(() =>
		{
			// Act
			new DataEntrySymbolService<NotSupportedClass, int>(new DataEntrySymbolStorage<NotSupportedClass, int>(), fakeDataSource);
		});
	}

	[TestMethod]
	public void DbDataEntrySymbolService_GetEntryId_SupportsDeletedObjects()
	{
		// Arrange
		FakeSupportedClassDataSource fakeDataSource = new FakeSupportedClassDataSource(new SupportedClass { Id = 1, Symbol = SupportedClass.Entry.First.ToString(), Deleted = DateTime.Now });

		// Act
		DataEntrySymbolService<SupportedClass, int> dbDataEntrySymbolService = new DataEntrySymbolService<SupportedClass, int>(new DataEntrySymbolStorage<SupportedClass, int>(), fakeDataSource);
		int id = dbDataEntrySymbolService.GetEntryId(SupportedClass.Entry.First);

		// Assert
		Assert.AreEqual(1, id);
	}

	[TestMethod]
	public void DbDataEntrySymbolService_GetEntryId_SkipsNullAndEmptySymbols()
	{
		// Arrange
		FakeSupportedClassDataSource fakeDataSource = new FakeSupportedClassDataSource(
			new SupportedClass { Id = 1, Symbol = SupportedClass.Entry.First.ToString() },
			new SupportedClass { Id = 2, Symbol = null },
			new SupportedClass { Id = 3, Symbol = String.Empty });

		// Act
		DataEntrySymbolService<SupportedClass, int> dbDataEntrySymbolService = new DataEntrySymbolService<SupportedClass, int>(new DataEntrySymbolStorage<SupportedClass, int>(), fakeDataSource);
		dbDataEntrySymbolService.GetEntryId(SupportedClass.Entry.First);

		// Assert
		// no exception was thrown
	}

}
