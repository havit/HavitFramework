using System;
using System.Collections.Generic;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Patterns.Tests.DataSeeds
{
	[TestClass]
	public class DataSeedRunnerTest
	{
		[TestMethod]
		public void DataSeedPersister_SeedData_CallsSeedDataOnAllDataSeeds()
		{
		    Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>();
		    dataSeedMock.Setup(m => m.ProfileType).Returns(typeof(DefaultProfile));

			Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>();

			DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterMock.Object);
			
			// Act
			runner.SeedData<DefaultProfile>();

			// Assert
			dataSeedMock.Verify(m => m.SeedData(dataSeedPersisterMock.Object), Times.Once);
		}

	    [TestMethod]
	    public void DataSeedPersister_SeedData_CallsSeedDataOnPrerequisiteProfile()
	    {
	        Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>();
	        dataSeedMock.Setup(m => m.ProfileType).Returns(typeof(DefaultProfile));

	        Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>();

	        DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterMock.Object);

	        // Act
	        runner.SeedData<ProfileWithPrerequisite>();

	        // Assert
	        dataSeedMock.Verify(m => m.SeedData(dataSeedPersisterMock.Object), Times.Once);
	    }

        [TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataSeedPersister_SeedData_ThrowsExceptionWhenCycleInPrerequisities()
		{
			DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();
			DataSeedCycleB dataSeedCycleB = new DataSeedCycleB();

			Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>();

			DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA, dataSeedCycleB }, new AlwaysRunDecision(), dataSeedPersisterMock.Object);

			// Act
			runner.SeedData<DefaultProfile>();

			// Assert by method attribute
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataSeedPersister_SeedData_ThrowsExceptionWhenPrerequisiteIsItself()
		{
			DataSeedDependentOnItself dataSeedDependentOnItself = new DataSeedDependentOnItself();

			Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>();

			DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedDependentOnItself }, new AlwaysRunDecision(), dataSeedPersisterMock.Object);

			// Act
			runner.SeedData<DefaultProfile>();

			// Assert by method attribute
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void DataSeedPersister_SeedData_ThrowsExceptionWhenOneTypeUsedMoreTimes()
		{
			Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>();
			Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>();

			DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object, dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterMock.Object);

			// Act
			runner.SeedData<DefaultProfile>();

			// Assert by method attribute
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataSeedPersister_SeedData_ThrowsExceptionWhenPrerequisiteNotFound()
		{
			DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();
			Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>();

			DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA }, new AlwaysRunDecision(), dataSeedPersisterMock.Object);

			// Act
			runner.SeedData<DefaultProfile>();

			// Assert by method attribute
		}
	}
}
