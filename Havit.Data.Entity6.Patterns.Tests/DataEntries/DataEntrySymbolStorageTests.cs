using System;
using System.Diagnostics.CodeAnalysis;
using Havit.Data.Entity.Patterns.Tests.DataEntries.DataSources;
using Havit.Data.Entity.Patterns.Tests.DataEntries.Model;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
		DataEntrySymbolService<SupportedClass> dbDataEntrySymbolService = new DataEntrySymbolService<SupportedClass>(new DataEntrySymbolStorage<SupportedClass>(), fakeDataSource);
		int id = dbDataEntrySymbolService.GetEntryId(SupportedClass.Entry.Second);

		// Assert
		Assert.AreEqual(2, id);
	}

	[TestMethod]
	[ExpectedException(typeof(Havit.Data.Patterns.Exceptions.ObjectNotFoundException))]
	public void DbDataEntrySymbolService_GetEntryId_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		FakeSupportedClassDataSource fakeDataSource = new FakeSupportedClassDataSource();
		DataEntrySymbolService<SupportedClass> dbDataEntrySymbolService = new DataEntrySymbolService<SupportedClass>(new DataEntrySymbolStorage<SupportedClass>(), fakeDataSource);
		
		// Act
		dbDataEntrySymbolService.GetEntryId(SupportedClass.Entry.First);

		// Assert by method attribute 
	}

	[TestMethod]
	[ExpectedException(typeof(NotSupportedException))]
	[SuppressMessage("SonicLint", "S1848", Justification = "Pravidlo říká, nemáme vytvořit instanci, kterou pak nepoužijeme. Zde však testujeme vytvoření instance a k ničemu dalšímu ji nepotřebujeme.")]
	public void DbDataEntrySymbolService_GetEntryId_ThrowsExceptionWhenNotSupported()
	{
		// Arrange
		FakeNotSupportedClassDataSource fakeDataSource = new FakeNotSupportedClassDataSource();

		// Act
		new DataEntrySymbolService<NotSupportedClass>(new DataEntrySymbolStorage<NotSupportedClass>(), fakeDataSource);

		// Assert by method attribute 
	}

	[TestMethod]
	public void DbDataEntrySymbolService_GetEntryId_SupportsDeletedObjects()
	{
		// Arrange
		FakeSupportedClassDataSource fakeDataSource = new FakeSupportedClassDataSource(new SupportedClass { Id = 1, Symbol = SupportedClass.Entry.First.ToString(), Deleted = DateTime.Now });

		// Act
		DataEntrySymbolService<SupportedClass> dbDataEntrySymbolService = new DataEntrySymbolService<SupportedClass>(new DataEntrySymbolStorage<SupportedClass>(), fakeDataSource);
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
		DataEntrySymbolService<SupportedClass> dbDataEntrySymbolService = new DataEntrySymbolService<SupportedClass>(new DataEntrySymbolStorage<SupportedClass>(), fakeDataSource);
		dbDataEntrySymbolService.GetEntryId(SupportedClass.Entry.First);

		// Assert
		// no exception was thrown
	}

}
