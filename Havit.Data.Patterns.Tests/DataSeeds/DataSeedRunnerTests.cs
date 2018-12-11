using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Data.Patterns.Transactions.Internal;
using Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;
using Havit.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Patterns.Tests.DataSeeds
{
	[TestClass]
	public class DataSeedRunnerTests
	{
		[TestMethod]
		public void DataSeedPersister_SeedData_CallsSeedDataOnAllDataSeeds()
		{
			// Arrange
		    Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>(MockBehavior.Strict);
		    dataSeedMock.Setup(m => m.ProfileType).Returns(typeof(DefaultProfile));
		    dataSeedMock.Setup(m => m.GetPrerequisiteDataSeeds()).Returns(Enumerable.Empty<Type>());
		    dataSeedMock.Setup(m => m.SeedData(It.IsAny<IDataSeedPersister>()));

			Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>(MockBehavior.Strict);
			Mock<IServiceFactory<IDataSeedPersister>> dataSeedPersisterFactoryMock = new Mock<IServiceFactory<IDataSeedPersister>>(MockBehavior.Strict);
			dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersisterMock.Object);
			dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

			DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object, new NullTransactionWrapper());
			
			// Act
			runner.SeedData<DefaultProfile>();

			// Assert
			dataSeedMock.Verify(m => m.SeedData(dataSeedPersisterMock.Object), Times.Once);
		}
		
	    [TestMethod]
	    public void DataSeedPersister_SeedData_CallsSeedDataOnPrerequisiteProfile()
	    {
			// Arrange
	        Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>(MockBehavior.Strict);
	        dataSeedMock.Setup(m => m.ProfileType).Returns(typeof(DefaultProfile));
			dataSeedMock.Setup(m => m.GetPrerequisiteDataSeeds()).Returns(Enumerable.Empty<Type>());
			dataSeedMock.Setup(m => m.SeedData(It.IsAny<IDataSeedPersister>()));

			Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>(MockBehavior.Strict);

			Mock<IServiceFactory<IDataSeedPersister>> dataSeedPersisterFactoryMock = new Mock<IServiceFactory<IDataSeedPersister>>(MockBehavior.Strict);
			dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersisterMock.Object);
			dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

	        DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object, new NullTransactionWrapper());

	        // Act
	        runner.SeedData<ProfileWithPrerequisite>();

	        // Assert
	        dataSeedMock.Verify(m => m.SeedData(dataSeedPersisterMock.Object), Times.Once);
	    }
		
        [TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataSeedPersister_SeedData_ThrowsExceptionWhenCycleInPrerequisities()
		{
			// Arrange
			DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();
			DataSeedCycleB dataSeedCycleB = new DataSeedCycleB();

			Mock<IServiceFactory<IDataSeedPersister>> dataSeedPersisterFactoryMock = new Mock<IServiceFactory<IDataSeedPersister>>(MockBehavior.Strict);

			DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA, dataSeedCycleB }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object, new NullTransactionWrapper());

			// Act
			runner.SeedData<DefaultProfile>();

			// Assert by method attribute
		}
		
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataSeedPersister_SeedData_ThrowsExceptionWhenPrerequisiteIsItself()
		{
			// Arrange
			DataSeedDependentOnItself dataSeedDependentOnItself = new DataSeedDependentOnItself();

			Mock<IServiceFactory<IDataSeedPersister>> dataSeedPersisterFactoryMock = new Mock<IServiceFactory<IDataSeedPersister>>(MockBehavior.Strict);

			DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedDependentOnItself }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object, new NullTransactionWrapper());

			// Act
			runner.SeedData<DefaultProfile>();

			// Assert by method attribute
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void DataSeedPersister_SeedData_ThrowsExceptionWhenOneTypeUsedMoreTimes()
		{
			// Arrange
			Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>();

			Mock<IServiceFactory<IDataSeedPersister>> dataSeedPersisterFactoryMock = new Mock<IServiceFactory<IDataSeedPersister>>(MockBehavior.Strict);

			DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object, dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object, new NullTransactionWrapper());

			// Act
			runner.SeedData<DefaultProfile>();

			// Assert by method attribute
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataSeedPersister_SeedData_ThrowsExceptionWhenPrerequisiteNotFound()
		{
			// Arrange
			DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();

			Mock<IServiceFactory<IDataSeedPersister>> dataSeedPersisterFactoryMock = new Mock<IServiceFactory<IDataSeedPersister>>(MockBehavior.Strict);

			DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object, new NullTransactionWrapper());

			// Act
			runner.SeedData<DefaultProfile>();

			// Assert by method attribute
		}
	}
}
