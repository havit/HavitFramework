using Havit.Data.Entity.Patterns.UnitOfWorks;
using Havit.Data.Entity.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.UnitOfWorks.BeforeCommitProcessors;

[TestClass]
public class BeforeCommitProcessorsRunnerTests
{
	[TestMethod]
	public void BeforeCommitProcessorsRunner_Run_RunsProcessors()
	{
		// Arrange
		Entity entityInserting = new Entity();
		Entity entityUpdating = new Entity();
		Entity entityDeleting = new Entity();

		Mock<IBeforeCommitProcessor<Entity>> beforeCommitEntityProcessorMock = new Mock<IBeforeCommitProcessor<Entity>>(MockBehavior.Strict);
		beforeCommitEntityProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>()));

		Mock<IBeforeCommitProcessor<object>> beforeCommitObjectProcessorMock = new Mock<IBeforeCommitProcessor<object>>(MockBehavior.Strict);
		beforeCommitObjectProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<object>()));

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create<object>()).Returns(new List<IBeforeCommitProcessor<object>> { beforeCommitObjectProcessorMock.Object });
		beforeCommitProcessorFactoryMock.Setup(m => m.Create<Entity>()).Returns(new List<IBeforeCommitProcessor<Entity>> { beforeCommitEntityProcessorMock.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		// Act
		runner.Run(new Changes
		{
			Inserts = new object[] { entityInserting },
			Updates = new object[] { entityUpdating },
			Deletes = new object[] { entityDeleting }
		});

		// Assert
		beforeCommitProcessorFactoryMock.Verify(m => m.Create<Entity>(), Times.AtLeastOnce);
		beforeCommitProcessorFactoryMock.Verify(m => m.Create<object>(), Times.AtLeastOnce);

		beforeCommitEntityProcessorMock.Verify(m => m.Run(ChangeType.Insert, entityInserting), Times.Once);
		beforeCommitEntityProcessorMock.Verify(m => m.Run(ChangeType.Update, entityUpdating), Times.Once);
		beforeCommitEntityProcessorMock.Verify(m => m.Run(ChangeType.Delete, entityDeleting), Times.Once);
		beforeCommitEntityProcessorMock.Verify(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>()), Times.Exactly(3));

		beforeCommitObjectProcessorMock.Verify(m => m.Run(ChangeType.Insert, entityInserting), Times.Once);
		beforeCommitObjectProcessorMock.Verify(m => m.Run(ChangeType.Update, entityUpdating), Times.Once);
		beforeCommitObjectProcessorMock.Verify(m => m.Run(ChangeType.Delete, entityDeleting), Times.Once);
		beforeCommitObjectProcessorMock.Verify(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<object>()), Times.Exactly(3));
	}

	public class Entity
	{

	}
}
