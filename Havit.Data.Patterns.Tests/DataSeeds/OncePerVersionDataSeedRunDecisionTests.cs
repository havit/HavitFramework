using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;
using Havit.Services.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Patterns.Tests.DataSeeds;

[TestClass]
public class OncePerVersionDataSeedRunDecisionTests
{
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
}
