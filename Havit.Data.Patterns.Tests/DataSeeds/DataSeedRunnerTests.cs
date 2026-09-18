using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;
using Moq;

namespace Havit.Data.Patterns.Tests.DataSeeds;

[TestClass]
public class DataSeedRunnerTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void DataSeedRunner_Constructor_ThrowsExceptionWhenOneTypeUsedMoreTimes()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		Assert.ThrowsExactly<ArgumentException>(() =>
		{
			new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object, dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);
		});
	}

	[TestMethod]
	public void DataSeedRunner_SeedData_CallsSeedDataOnAllDataSeeds()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = CreateDataSeedMock();
		Mock<IDataSeedPersister> dataSeedPersisterMock = CreateDataSeedPersisterMock();
		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = CreateDataSeedPersisterFactoryMock(dataSeedPersisterMock.Object);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		runner.SeedData<DefaultProfile>();

		// Assert
		dataSeedMock.Verify(m => m.SeedData(dataSeedPersisterMock.Object), Times.Once);
		dataSeedMock.Verify(m => m.SeedDataAsync(dataSeedPersisterMock.Object, It.IsAny<CancellationToken>()), Times.Once);
	}

	[TestMethod]
	public async Task DataSeedRunner_SeedDataAsync_CallsSeedDataOnAllDataSeeds()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = CreateDataSeedMock();
		Mock<IDataSeedPersister> dataSeedPersisterMock = CreateDataSeedPersisterMock();
		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = CreateDataSeedPersisterFactoryMock(dataSeedPersisterMock.Object);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		await runner.SeedDataAsync<DefaultProfile>(cancellationToken: TestContext.CancellationToken);

		// Assert
		dataSeedMock.Verify(m => m.SeedData(dataSeedPersisterMock.Object), Times.Once);
		dataSeedMock.Verify(m => m.SeedDataAsync(dataSeedPersisterMock.Object, It.IsAny<CancellationToken>()), Times.Once);
	}

	[TestMethod]
	public void DataSeedRunner_SeedData_SeedsPrerequisiteProfileBeforeDependentProfile()
	{
		// Arrange
		// ProfileWithPrerequisite má jako prerekvizitu DefaultProfile - seed z DefaultProfile proto musí proběhnout dříve.
		List<string> seedOrder = new List<string>();

		RecordingDataSeed<DefaultProfile> prerequisiteProfileSeed = new RecordingDataSeed<DefaultProfile>(seedOrder, "prerequisite");
		RecordingDataSeed<ProfileWithPrerequisite> dependentProfileSeed = new RecordingDataSeed<ProfileWithPrerequisite>(seedOrder, "dependent");

		Mock<IDataSeedPersister> dataSeedPersisterMock = CreateDataSeedPersisterMock();
		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = CreateDataSeedPersisterFactoryMock(dataSeedPersisterMock.Object);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dependentProfileSeed, prerequisiteProfileSeed }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		runner.SeedData<ProfileWithPrerequisite>();

		// Assert
		Assert.AreSequenceEqual(new[] { "prerequisite", "dependent" }, seedOrder, "Prerekvizitní profil musí být naseedován před závislým profilem (a každý právě jednou).");
	}

	[TestMethod]
	public async Task DataSeedRunner_SeedDataAsync_SeedsPrerequisiteProfileBeforeDependentProfile()
	{
		// Arrange
		List<string> seedOrder = new List<string>();

		RecordingDataSeed<DefaultProfile> prerequisiteProfileSeed = new RecordingDataSeed<DefaultProfile>(seedOrder, "prerequisite");
		RecordingDataSeed<ProfileWithPrerequisite> dependentProfileSeed = new RecordingDataSeed<ProfileWithPrerequisite>(seedOrder, "dependent");

		Mock<IDataSeedPersister> dataSeedPersisterMock = CreateDataSeedPersisterMock();
		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = CreateDataSeedPersisterFactoryMock(dataSeedPersisterMock.Object);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dependentProfileSeed, prerequisiteProfileSeed }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		await runner.SeedDataAsync<ProfileWithPrerequisite>(cancellationToken: TestContext.CancellationToken);

		// Assert
		Assert.AreSequenceEqual(new[] { "prerequisite", "dependent" }, seedOrder, "Prerekvizitní profil musí být naseedován před závislým profilem (a každý právě jednou).");
	}

	[TestMethod]
	public void DataSeedRunner_SeedData_ThrowsExceptionWhenCycleInPrerequisities()
	{
		// Arrange
		// DataSeedCycleA -> DataSeedCycleB -> DataSeedCycleC -> DataSeedCycleA (všechny tři musí být registrovány, aby šlo skutečně o cyklus, nikoliv chybějící prerekvizitu).
		DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();
		DataSeedCycleB dataSeedCycleB = new DataSeedCycleB();
		DataSeedCycleC dataSeedCycleC = new DataSeedCycleC();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA, dataSeedCycleB, dataSeedCycleC }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			runner.SeedData<DefaultProfile>();
		});

		// Assert
		Assert.Contains("cycle", exception.Message);
		Assert.Contains("DataSeedCycleA -> DataSeedCycleB -> DataSeedCycleC -> DataSeedCycleA", exception.Message);
	}

	[TestMethod]
	public async Task DataSeedRunner_SeedDataAsync_ThrowsExceptionWhenCycleInPrerequisities()
	{
		// Arrange
		DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();
		DataSeedCycleB dataSeedCycleB = new DataSeedCycleB();
		DataSeedCycleC dataSeedCycleC = new DataSeedCycleC();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA, dataSeedCycleB, dataSeedCycleC }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
		{
			await runner.SeedDataAsync<DefaultProfile>(cancellationToken: TestContext.CancellationToken);
		});

		// Assert
		Assert.Contains("cycle", exception.Message);
		Assert.Contains("DataSeedCycleA -> DataSeedCycleB -> DataSeedCycleC -> DataSeedCycleA", exception.Message);
	}

	[TestMethod]
	public void DataSeedRunner_SeedData_ThrowsExceptionWhenPrerequisiteIsItself()
	{
		// Arrange
		DataSeedDependentOnItself dataSeedDependentOnItself = new DataSeedDependentOnItself();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedDependentOnItself }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			runner.SeedData<DefaultProfile>();
		});

		// Assert
		Assert.Contains("cycle", exception.Message);
		Assert.Contains("DataSeedDependentOnItself -> DataSeedDependentOnItself", exception.Message);
	}

	[TestMethod]
	public async Task DataSeedRunner_SeedDataAsync_ThrowsExceptionWhenPrerequisiteIsItself()
	{
		// Arrange
		DataSeedDependentOnItself dataSeedDependentOnItself = new DataSeedDependentOnItself();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedDependentOnItself }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
		{
			await runner.SeedDataAsync<DefaultProfile>(cancellationToken: TestContext.CancellationToken);
		});

		// Assert
		Assert.Contains("cycle", exception.Message);
		Assert.Contains("DataSeedDependentOnItself -> DataSeedDependentOnItself", exception.Message);
	}

	[TestMethod]
	public void DataSeedRunner_SeedData_ThrowsExceptionWhenPrerequisiteNotFound()
	{
		// Arrange
		// DataSeedCycleA závisí na DataSeedCycleB, který ale není registrován.
		DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			runner.SeedData<DefaultProfile>();
		});

		// Assert
		Assert.Contains("was not found", exception.Message);
		Assert.Contains("DataSeedCycleB", exception.Message);
		Assert.Contains("DataSeedCycleA", exception.Message);
	}

	[TestMethod]
	public async Task DataSeedRunner_SeedDataAsync_ThrowsExceptionWhenPrerequisiteNotFound()
	{
		// Arrange
		DataSeedCycleA dataSeedCycleA = new DataSeedCycleA();

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedCycleA }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
		{
			await runner.SeedDataAsync<DefaultProfile>(cancellationToken: TestContext.CancellationToken);
		});

		// Assert
		Assert.Contains("was not found", exception.Message);
		Assert.Contains("DataSeedCycleB", exception.Message);
		Assert.Contains("DataSeedCycleA", exception.Message);
	}

	[TestMethod]
	public void DataSeedRunner_SeedData_ThrowsExceptionWhenCycleInProfiles()
	{
		// Arrange
		// ProfileCycleA -> ProfileCycleB -> ProfileCycleA (cyklus závislostí profilů, detekováno ještě před vlastním seedováním).
		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			runner.SeedData<ProfileCycleA>();
		});

		// Assert
		Assert.Contains("profiles", exception.Message); // odlišení od hlášky cyklu data seedů
		Assert.Contains("cycle", exception.Message);
		Assert.Contains("ProfileCycleA -> ProfileCycleB -> ProfileCycleA", exception.Message);
	}

	[TestMethod]
	public async Task DataSeedRunner_SeedDataAsync_ThrowsExceptionWhenCycleInProfiles()
	{
		// Arrange
		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () =>
		{
			await runner.SeedDataAsync<ProfileCycleA>(cancellationToken: TestContext.CancellationToken);
		});

		// Assert
		Assert.Contains("profiles", exception.Message); // odlišení od hlášky cyklu data seedů
		Assert.Contains("cycle", exception.Message);
		Assert.Contains("ProfileCycleA -> ProfileCycleB -> ProfileCycleA", exception.Message);
	}

	[TestMethod]
	public void DataSeedRunner_SeedData_ThrowsExceptionWhenSeedDataAsyncReturnsNotCompletedTask()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = CreateDataSeedMock();
		// Nikdy nedokončený task (deterministicky, bez závislosti na časování) - simuluje async data seed spuštěný přes synchronní runner.
		dataSeedMock.Setup(m => m.SeedDataAsync(It.IsAny<IDataSeedPersister>(), It.IsAny<CancellationToken>())).Returns(new TaskCompletionSource().Task);

		Mock<IDataSeedPersister> dataSeedPersisterMock = CreateDataSeedPersisterMock();
		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = CreateDataSeedPersisterFactoryMock(dataSeedPersisterMock.Object);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act + Assert
		Assert.ThrowsExactly<SeedAsyncFromSyncSeedDataException>(() =>
		{
			runner.SeedData<ProfileWithPrerequisite>();
		});
	}

	[TestMethod]
	public async Task DataSeedRunner_SeedDataAsync_DoesNotThrowExceptionWhenSeedDataAsyncReturnsNotCompletedTask()
	{
		// Arrange
		Mock<IDataSeed> dataSeedMock = CreateDataSeedMock();
		// Task, který se dokončí až po krátké prodlevě - async runner na něj korektně počká.
		dataSeedMock.Setup(m => m.SeedDataAsync(It.IsAny<IDataSeedPersister>(), It.IsAny<CancellationToken>())).Returns(() => Task.Delay(1, TestContext.CancellationToken));

		Mock<IDataSeedPersister> dataSeedPersisterMock = CreateDataSeedPersisterMock();
		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = CreateDataSeedPersisterFactoryMock(dataSeedPersisterMock.Object);

		DataSeedRunner runner = new DataSeedRunner(new IDataSeed[] { dataSeedMock.Object }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

		// Act
		await runner.SeedDataAsync<ProfileWithPrerequisite>(cancellationToken: TestContext.CancellationToken);

		// Assert - no exception is thrown
	}

	/// <summary>
	/// Vytvoří strict mock <see cref="IDataSeed"/> v <see cref="DefaultProfile"/> bez prerekvizit
	/// a s NOOP implementací synchronního i asynchronního seedování.
	/// </summary>
	private static Mock<IDataSeed> CreateDataSeedMock()
	{
		Mock<IDataSeed> dataSeedMock = new Mock<IDataSeed>(MockBehavior.Strict);
		dataSeedMock.Setup(m => m.ProfileType).Returns(typeof(DefaultProfile));
		dataSeedMock.Setup(m => m.GetPrerequisiteDataSeeds()).Returns(Enumerable.Empty<Type>());
		dataSeedMock.Setup(m => m.SeedData(It.IsAny<IDataSeedPersister>()));
		dataSeedMock.Setup(m => m.SeedDataAsync(It.IsAny<IDataSeedPersister>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
		return dataSeedMock;
	}

	/// <summary>
	/// Vytvoří strict mock <see cref="IDataSeedPersister"/> přijímající attach libovolného data seedu.
	/// </summary>
	private static Mock<IDataSeedPersister> CreateDataSeedPersisterMock()
	{
		Mock<IDataSeedPersister> dataSeedPersisterMock = new Mock<IDataSeedPersister>(MockBehavior.Strict);
		dataSeedPersisterMock.Setup(m => m.AttachDataSeed(It.IsAny<IDataSeed>()));
		return dataSeedPersisterMock;
	}

	/// <summary>
	/// Vytvoří strict mock <see cref="IDataSeedPersisterFactory"/> vracející předaný persister.
	/// </summary>
	private static Mock<IDataSeedPersisterFactory> CreateDataSeedPersisterFactoryMock(IDataSeedPersister dataSeedPersister)
	{
		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);
		dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersister);
		dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));
		return dataSeedPersisterFactoryMock;
	}
}
