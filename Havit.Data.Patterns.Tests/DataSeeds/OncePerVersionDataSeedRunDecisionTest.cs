using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;
using Havit.Services.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Patterns.Tests.DataSeeds
{
	[TestClass]
	public class OncePerVersionDataSeedRunDecisionTest
	{
		[TestMethod]
		public void OncePerVersionDataSeedRunDecision_ShouldSeedDataUntilSeedDataCompleted()
		{
			// Arrange
			string currentState = String.Empty;

			Mock<IDataSeedRunDecisionStatePersister> dataSeedRunDecisionStatePersisterMock = new Mock<IDataSeedRunDecisionStatePersister>();
			dataSeedRunDecisionStatePersisterMock.Setup(m => m.ReadCurrentState()).Returns(() => currentState); /* lambda - nutno vyhodnotit až při volání! */
			dataSeedRunDecisionStatePersisterMock.Setup(m => m.WriteCurrentState(It.IsAny<string>())).Callback((string newState) => { currentState = newState; });

			// Act + Assert
			OncePerVersionDataSeedRunDecision decision = new OncePerVersionDataSeedRunDecision(dataSeedRunDecisionStatePersisterMock.Object, typeof(OncePerVersionDataSeedRunDecisionTest).Assembly);

			Assert.IsTrue(decision.ShouldSeedData());
			decision.SeedDataCompleted();
			Assert.IsFalse(decision.ShouldSeedData());
		}
	}
}
