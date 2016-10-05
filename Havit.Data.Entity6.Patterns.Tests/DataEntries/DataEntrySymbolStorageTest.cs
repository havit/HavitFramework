using System;
using System.Diagnostics.CodeAnalysis;
using Havit.Data.Entity.Patterns.Tests.DataEntries.DataSources;
using Havit.Data.Entity.Patterns.Tests.DataEntries.Model;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.Exceptions;
using Havit.Data.Patterns.QueryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.DataEntries
{
	// TODO: Přesunout do Havit.Data.Patterns.Tests

	[TestClass]
	public class DataEntrySymbolStorageTest
	{
		[TestMethod]
		public void DbDataEntrySymbolStorage_GetEntryId_ReturnsId()
		{
			// Arrange
			FakeSupportedClassDataSource fakeDataSource = new FakeSupportedClassDataSource(
				new SupportedClass { Id = 1, Symbol = SupportedClass.Entry.First.ToString() },
				new SupportedClass { Id = 2, Symbol = SupportedClass.Entry.Second.ToString() },
				new SupportedClass { Id = 3, Symbol = SupportedClass.Entry.Third.ToString() });
			Mock<IDataSourceFactory<SupportedClass>> mockDataSourceFactory = new Mock<IDataSourceFactory<SupportedClass>>();
			mockDataSourceFactory.Setup(mock => mock.Create()).Returns(fakeDataSource);
			
			// Act
			DataEntrySymbolStorage<SupportedClass> dbDataEntrySymbolStorage = new DataEntrySymbolStorage<SupportedClass>(mockDataSourceFactory.Object);
			int id = dbDataEntrySymbolStorage.GetEntryId(SupportedClass.Entry.Second);

			// Assert
			Assert.AreEqual(2, id);
		}

		[TestMethod]
		[ExpectedException(typeof(ObjectNotFoundException))]
		public void DbDataEntrySymbolStorage_GetEntryId_ThrowsExceptionWhenNotFound()
		{
			// Arrange
			FakeSupportedClassDataSource fakeDataSource = new FakeSupportedClassDataSource();
			Mock<IDataSourceFactory<SupportedClass>> mockDataSourceFactory = new Mock<IDataSourceFactory<SupportedClass>>();
			mockDataSourceFactory.Setup(mock => mock.Create()).Returns(fakeDataSource);
			DataEntrySymbolStorage<SupportedClass> dbDataEntrySymbolStorage = new DataEntrySymbolStorage<SupportedClass>(mockDataSourceFactory.Object);
			
			// Act
			dbDataEntrySymbolStorage.GetEntryId(SupportedClass.Entry.First);

			// Assert by method attribute 
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		[SuppressMessage("SonicLint", "S1848", Justification = "Pravidlo říká, nemáme vytvořit instanci, kterou pak nepoužijeme. Zde však testujeme vytvoření instance a k ničemu dalšímu ji nepotřebujeme.")]
		public void DbDataEntrySymbolStorage_GetEntryId_ThrowsExceptionWhenNotSupported()
		{
			// Arrange
			FakeNotSupportedClassDataSource fakeDataSource = new FakeNotSupportedClassDataSource();
			Mock<IDataSourceFactory<NotSupportedClass>> mockDataSourceFactory = new Mock<IDataSourceFactory<NotSupportedClass>>();
			mockDataSourceFactory.Setup(mock => mock.Create()).Returns(fakeDataSource);

			// Act
			new DataEntrySymbolStorage<NotSupportedClass>(mockDataSourceFactory.Object);
	
			// Assert by method attribute 
		}

		[TestMethod]
		public void DbDataEntrySymbolStorage_GetEntryId_SupportsDeletedObjects()
		{
			// Arrange
			FakeSupportedClassDataSource fakeDataSource = new FakeSupportedClassDataSource(new SupportedClass { Id = 1, Symbol = SupportedClass.Entry.First.ToString(), Deleted = DateTime.Now });
			Mock<IDataSourceFactory<SupportedClass>> mockDataSourceFactory = new Mock<IDataSourceFactory<SupportedClass>>();
			mockDataSourceFactory.Setup(mock => mock.Create()).Returns(fakeDataSource);

			// Act
			DataEntrySymbolStorage<SupportedClass> dbDataEntrySymbolStorage = new DataEntrySymbolStorage<SupportedClass>(mockDataSourceFactory.Object);
			int id = dbDataEntrySymbolStorage.GetEntryId(SupportedClass.Entry.First);

			// Assert
			Assert.AreEqual(1, id);
		}

		[TestMethod]
		public void DbDataEntrySymbolStorage_GetEntryId_SkipsNullAndEmptySymbols()
		{
			// Arrange
			FakeSupportedClassDataSource fakeDataSource = new FakeSupportedClassDataSource(
				new SupportedClass { Id = 1, Symbol = SupportedClass.Entry.First.ToString() },				
				new SupportedClass { Id = 2, Symbol = null },
				new SupportedClass { Id = 3, Symbol = String.Empty });
			Mock<IDataSourceFactory<SupportedClass>> mockDataSourceFactory = new Mock<IDataSourceFactory<SupportedClass>>();
			mockDataSourceFactory.Setup(mock => mock.Create()).Returns(fakeDataSource);

			// Act
			DataEntrySymbolStorage<SupportedClass> dbDataEntrySymbolStorage = new DataEntrySymbolStorage<SupportedClass>(mockDataSourceFactory.Object);
			dbDataEntrySymbolStorage.GetEntryId(SupportedClass.Entry.First);

			// Assert
			// no exception was thrown
		}

	}
}
