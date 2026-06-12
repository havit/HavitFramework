using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;
using Moq;

namespace Havit.Data.Patterns.Tests.DataSeeds;

[TestClass]
public class OncePerVersionDataSeedRunDecisionTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void OncePerVersionDataSeedRunDecision_ShouldSeedDataUntilSeedDataCompleted()
	{
		// Arrange
		string currentState = String.Empty;

		IDataSeedProfile defaultProfile = new DefaultProfile();
		Mock<IDataSeedRunDecisionStatePersister> dataSeedRunDecisionStatePersisterMock = new Mock<IDataSeedRunDecisionStatePersister>();
		dataSeedRunDecisionStatePersisterMock.Setup(m => m.ReadCurrentState(defaultProfile.ProfileName)).Returns((string profileName) => currentState); /* lambda - nutno vyhodnotit až při volání! */
		dataSeedRunDecisionStatePersisterMock.Setup(m => m.WriteCurrentState(defaultProfile.ProfileName, It.IsAny<string>())).Callback((string profileName, string newState) => { currentState = newState; });

		// Act + Assert
		OncePerVersionDataSeedRunDecision decision = new OncePerVersionDataSeedRunDecision(dataSeedRunDecisionStatePersisterMock.Object);

		List<Type> dataSeedTypes = new List<Type> { typeof(DataSeedCycleA), typeof(DataSeedCycleB), typeof(DataSeedDependentOnItself) };

		Assert.IsTrue(decision.ShouldSeedData(defaultProfile, dataSeedTypes));
		decision.SeedDataCompleted(defaultProfile, dataSeedTypes);
		Assert.IsFalse(decision.ShouldSeedData(defaultProfile, dataSeedTypes));
	}

	[TestMethod]
	public async Task OncePerVersionDataSeedRunDecision_ShouldSeedDataAsyncUntilSeedDataCompletedAsync()
	{
		// Arrange
		string currentState = String.Empty;

		IDataSeedProfile defaultProfile = new DefaultProfile();
		Mock<IDataSeedRunDecisionStatePersister> dataSeedRunDecisionStatePersisterMock = new Mock<IDataSeedRunDecisionStatePersister>();
		dataSeedRunDecisionStatePersisterMock.Setup(m => m.ReadCurrentStateAsync(defaultProfile.ProfileName, It.IsAny<CancellationToken>())).ReturnsAsync(() => currentState); /* lambda - nutno vyhodnotit až při volání! */
		dataSeedRunDecisionStatePersisterMock.Setup(m => m.WriteCurrentStateAsync(defaultProfile.ProfileName, It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.Callback((string profileName, string newState, CancellationToken cancellationToken) => { currentState = newState; })
			.Returns(Task.CompletedTask);

		// Act + Assert
		OncePerVersionDataSeedRunDecision decision = new OncePerVersionDataSeedRunDecision(dataSeedRunDecisionStatePersisterMock.Object);

		List<Type> dataSeedTypes = new List<Type> { typeof(DataSeedCycleA), typeof(DataSeedCycleB), typeof(DataSeedDependentOnItself) };

		Assert.IsTrue(await decision.ShouldSeedDataAsync(defaultProfile, dataSeedTypes, TestContext.CancellationToken));
		await decision.SeedDataCompletedAsync(defaultProfile, dataSeedTypes, TestContext.CancellationToken);
		Assert.IsFalse(await decision.ShouldSeedDataAsync(defaultProfile, dataSeedTypes, TestContext.CancellationToken));
	}

	[TestMethod]
	public void OncePerVersionDataSeedRunDecision_GetState_IsIndependentOfTypeOrder()
	{
		// Arrange
		OncePerVersionDataSeedRunDecision decision = new OncePerVersionDataSeedRunDecision(new Mock<IDataSeedRunDecisionStatePersister>().Object);

		// Act
		string state1 = decision.GetState(new List<Type> { typeof(DataSeedCycleA), typeof(DataSeedCycleB) });
		string state2 = decision.GetState(new List<Type> { typeof(DataSeedCycleB), typeof(DataSeedCycleA) });

		// Assert (stav závisí na množině assembly, ne na pořadí typů)
		Assert.AreEqual(state1, state2);
	}

	[TestMethod]
	public void OncePerVersionDataSeedRunDecision_GetState_CollapsesTypesFromSameAssemblyToSingleSegment()
	{
		// Arrange
		OncePerVersionDataSeedRunDecision decision = new OncePerVersionDataSeedRunDecision(new Mock<IDataSeedRunDecisionStatePersister>().Object);

		// Act
		// Oba typy jsou ze stejné (testovací) assembly - segmenty jsou oddělené čárkou, takže jeden segment = bez čárky.
		string state = decision.GetState(new List<Type> { typeof(DataSeedCycleA), typeof(DataSeedCycleB) });

		// Assert
		Assert.DoesNotContain(",", state);
		Assert.Contains(typeof(DataSeedCycleA).Assembly.GetName().Name, state);
	}

	[TestMethod]
	public void OncePerVersionDataSeedRunDecision_GetState_ContainsSegmentPerDistinctAssembly()
	{
		// Arrange
		OncePerVersionDataSeedRunDecision decision = new OncePerVersionDataSeedRunDecision(new Mock<IDataSeedRunDecisionStatePersister>().Object);

		// Act
		// Typ z testovací assembly + typ z assembly Havit.Data.Patterns -> dvě různé assembly, tedy dva segmenty oddělené čárkou.
		string state = decision.GetState(new List<Type> { typeof(DataSeedCycleA), typeof(OncePerVersionDataSeedRunDecision) });

		// Assert
		Assert.Contains(",", state);
		Assert.Contains(typeof(DataSeedCycleA).Assembly.GetName().Name, state);
		Assert.Contains(typeof(OncePerVersionDataSeedRunDecision).Assembly.GetName().Name, state);
	}
}