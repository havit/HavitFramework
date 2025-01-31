using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Patterns.Tests.DataSeeds;

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
		dataSeedMock.Setup(m => m.SeedDataAsync(It.IsAny<IDataSeedPersister>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

		Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>(MockBehavior.Strict);
		dataSeedPersisterMock.Setup(m => m.AttachDataSeed(dataSeedMock.Object));

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);
		dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersisterMock.Object);
		dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		runner.SeedData<DefaultProfile>();

		// Assert
		dataSeedMock.Verify(m => m.SeedData(dataSeedPersisterMock.Object), Times.Once);
		dataSeedMock.Verify(m => m.SeedDataAsync(dataSeedPersisterMock.Object, It.IsAny<CancellationToken>()), Times.Once);
	}

	[TestMethod]
	public async Task DataSeedPersister_SeedDataAsync_CallsSeedDataOnAllDataSeeds()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>(MockBehavior.Strict);
		dataSeedMock.Setup(m => m.ProfileType).Returns(typeof(DefaultProfile));
		dataSeedMock.Setup(m => m.GetPrerequisiteDataSeeds()).Returns(Enumerable.Empty<Type>());
		dataSeedMock.Setup(m => m.SeedData(It.IsAny<IDataSeedPersister>()));
		dataSeedMock.Setup(m => m.SeedDataAsync(It.IsAny<IDataSeedPersister>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

		Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>(MockBehavior.Strict);
		dataSeedPersisterMock.Setup(m => m.AttachDataSeed(dataSeedMock.Object));

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);
		dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersisterMock.Object);
		dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		await runner.SeedDataAsync<DefaultProfile>();

		// Assert
		dataSeedMock.Verify(m => m.SeedData(dataSeedPersisterMock.Object), Times.Once);
		dataSeedMock.Verify(m => m.SeedDataAsync(dataSeedPersisterMock.Object, It.IsAny<CancellationToken>()), Times.Once);
	}

	[TestMethod]
	public void DataSeedPersister_SeedData_CallsSeedDataOnPrerequisiteProfile()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>(MockBehavior.Strict);
		dataSeedMock.Setup(m => m.ProfileType).Returns(typeof(DefaultProfile));
		dataSeedMock.Setup(m => m.GetPrerequisiteDataSeeds()).Returns(Enumerable.Empty<Type>());
		dataSeedMock.Setup(m => m.SeedData(It.IsAny<IDataSeedPersister>()));
		dataSeedMock.Setup(m => m.SeedDataAsync(It.IsAny<IDataSeedPersister>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

		Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>(MockBehavior.Strict);
		dataSeedPersisterMock.Setup(m => m.AttachDataSeed(dataSeedMock.Object));

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);
		dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersisterMock.Object);
		dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		runner.SeedData<ProfileWithPrerequisite>();

		// Assert
		dataSeedMock.Verify(m => m.SeedData(dataSeedPersisterMock.Object), Times.Once);
		dataSeedMock.Verify(m => m.SeedDataAsync(dataSeedPersisterMock.Object, It.IsAny<CancellationToken>()), Times.Once);
	}

	[TestMethod]
	public async Task DataSeedPersister_SeedDataAsync_CallsSeedDataOnPrerequisiteProfile()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>(MockBehavior.Strict);
		dataSeedMock.Setup(m => m.ProfileType).Returns(typeof(DefaultProfile));
		dataSeedMock.Setup(m => m.GetPrerequisiteDataSeeds()).Returns(Enumerable.Empty<Type>());
		dataSeedMock.Setup(m => m.SeedData(It.IsAny<IDataSeedPersister>()));
		dataSeedMock.Setup(m => m.SeedDataAsync(It.IsAny<IDataSeedPersister>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

		Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>(MockBehavior.Strict);
		dataSeedPersisterMock.Setup(m => m.AttachDataSeed(dataSeedMock.Object));

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);
		dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersisterMock.Object);
		dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		await runner.SeedDataAsync<ProfileWithPrerequisite>();

		// Assert
		dataSeedMock.Verify(m => m.SeedData(dataSeedPersisterMock.Object), Times.Once);
		dataSeedMock.Verify(m => m.SeedDataAsync(dataSeedPersisterMock.Object, It.IsAny<CancellationToken>()), Times.Once);
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void DataSeedPersister_SeedData_ThrowsExceptionWhenCycleInPrerequisities()
	{
		// Arrange
		DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();
		DataSeedCycleB dataSeedCycleB = new DataSeedCycleB();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA, dataSeedCycleB }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		runner.SeedData<DefaultProfile>();

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public async Task DataSeedPersister_SeedDataAsync_ThrowsExceptionWhenCycleInPrerequisities()
	{
		// Arrange
		DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();
		DataSeedCycleB dataSeedCycleB = new DataSeedCycleB();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA, dataSeedCycleB }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		await runner.SeedDataAsync<DefaultProfile>();

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void DataSeedPersister_SeedData_ThrowsExceptionWhenPrerequisiteIsItself()
	{
		// Arrange
		DataSeedDependentOnItself dataSeedDependentOnItself = new DataSeedDependentOnItself();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedDependentOnItself }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		runner.SeedData<DefaultProfile>();

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public async Task DataSeedPersister_SeedDataAsync_ThrowsExceptionWhenPrerequisiteIsItself()
	{
		// Arrange
		DataSeedDependentOnItself dataSeedDependentOnItself = new DataSeedDependentOnItself();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedDependentOnItself }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		await runner.SeedDataAsync<DefaultProfile>();

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public void DataSeedPersister_SeedData_ThrowsExceptionWhenOneTypeUsedMoreTimes()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object, dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		runner.SeedData<DefaultProfile>();

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public async Task DataSeedPersister_SeedDataAsync_ThrowsExceptionWhenOneTypeUsedMoreTimes()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object, dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		await runner.SeedDataAsync<DefaultProfile>();

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void DataSeedPersister_SeedData_ThrowsExceptionWhenPrerequisiteNotFound()
	{
		// Arrange
		DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		runner.SeedData<DefaultProfile>();

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public async Task DataSeedPersister_SeedDataAsync_ThrowsExceptionWhenPrerequisiteNotFound()
	{
		// Arrange
		DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		await runner.SeedDataAsync<DefaultProfile>();

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(SeedAsyncFromSyncSeedDataException))]
	public void DataSeedPersister_SeedData_ThrowsExceptionWhenSeedDataAsyncReturnsNotCompletedTask()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>(MockBehavior.Strict);
		dataSeedMock.Setup(m => m.ProfileType).Returns(typeof(DefaultProfile));
		dataSeedMock.Setup(m => m.GetPrerequisiteDataSeeds()).Returns(Enumerable.Empty<Type>());
		dataSeedMock.Setup(m => m.SeedData(It.IsAny<IDataSeedPersister>()));
		dataSeedMock.Setup(m => m.SeedDataAsync(It.IsAny<IDataSeedPersister>(), It.IsAny<CancellationToken>())).Returns(() => new Task(() => { }));

		Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>(MockBehavior.Strict);
		dataSeedPersisterMock.Setup(m => m.AttachDataSeed(dataSeedMock.Object));

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);
		dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersisterMock.Object);
		dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		runner.SeedData<ProfileWithPrerequisite>();

		// Assert by method attribute
	}

	[TestMethod]
	public async Task DataSeedPersister_SeedDataAsync_DoesNotThrowExceptionWhenSeedDataAsyncReturnsNotCompletedTask()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>(MockBehavior.Strict);
		dataSeedMock.Setup(m => m.ProfileType).Returns(typeof(DefaultProfile));
		dataSeedMock.Setup(m => m.GetPrerequisiteDataSeeds()).Returns(Enumerable.Empty<Type>());
		dataSeedMock.Setup(m => m.SeedData(It.IsAny<IDataSeedPersister>()));
		dataSeedMock.Setup(m => m.SeedDataAsync(It.IsAny<IDataSeedPersister>(), It.IsAny<CancellationToken>())).Returns(() => Task.Delay(1));

		Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>(MockBehavior.Strict);
		dataSeedPersisterMock.Setup(m => m.AttachDataSeed(dataSeedMock.Object));

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);
		dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersisterMock.Object);
		dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		await runner.SeedDataAsync<ProfileWithPrerequisite>();

		// Assert - no exception is throw
	}
}

