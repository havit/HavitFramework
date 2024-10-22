using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.UnitOfWorks.BeforeCommitProcessors;

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
		beforeCommitEntityProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessor<object>> beforeCommitObjectProcessorMock = new Mock<IBeforeCommitProcessor<object>>(MockBehavior.Strict);
		beforeCommitObjectProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<object>())).Returns(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create<object>()).Returns(new List<IBeforeCommitProcessor<object>> { beforeCommitObjectProcessorMock.Object });
		beforeCommitProcessorFactoryMock.Setup(m => m.Create<Entity>()).Returns(new List<IBeforeCommitProcessor<Entity>> { beforeCommitEntityProcessorMock.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange { ChangeType = ChangeType.Insert, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entityInserting },
			new FakeChange { ChangeType = ChangeType.Update, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entityUpdating },
			new FakeChange { ChangeType = ChangeType.Delete, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entityDeleting },
		});

		// Act
		runner.Run(changes);

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

	[TestMethod]
	public void BeforeCommitProcessorsRunner_Run_RunsProcessors_ReturnsNoImpactWhenThereIsNoImpact()
	{
		// Arrange
		Entity entity = new Entity();

		Mock<IBeforeCommitProcessor<Entity>> beforeCommitEntityProcessorMock1 = new Mock<IBeforeCommitProcessor<Entity>>(MockBehavior.Strict);
		beforeCommitEntityProcessorMock1.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessor<Entity>> beforeCommitEntityProcessorMock2 = new Mock<IBeforeCommitProcessor<Entity>>(MockBehavior.Strict);
		beforeCommitEntityProcessorMock2.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create<object>()).Returns(Enumerable.Empty<IBeforeCommitProcessor<object>>());
		beforeCommitProcessorFactoryMock.Setup(m => m.Create<Entity>()).Returns(new List<IBeforeCommitProcessor<Entity>> { beforeCommitEntityProcessorMock1.Object, beforeCommitEntityProcessorMock2.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange { ChangeType = ChangeType.Update, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entity }
		});

		// Act
		ChangeTrackerImpact result = runner.Run(changes);

		// Assert
		Assert.AreEqual(ChangeTrackerImpact.NoImpact, result);
	}

	[TestMethod]
	public void BeforeCommitProcessorsRunner_Run_RunsProcessors_ReturnsStateChangesWhenThereIsAtLeastOneChange()
	{
		// Arrange
		Entity entity = new Entity();

		Mock<IBeforeCommitProcessor<Entity>> beforeCommitEntityProcessorMock1 = new Mock<IBeforeCommitProcessor<Entity>>(MockBehavior.Strict);
		beforeCommitEntityProcessorMock1.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessor<Entity>> beforeCommitEntityProcessorMock2 = new Mock<IBeforeCommitProcessor<Entity>>(MockBehavior.Strict);
		beforeCommitEntityProcessorMock2.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.StateChanged);

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create<object>()).Returns(Enumerable.Empty<IBeforeCommitProcessor<object>>());
		beforeCommitProcessorFactoryMock.Setup(m => m.Create<Entity>()).Returns(new List<IBeforeCommitProcessor<Entity>> { beforeCommitEntityProcessorMock1.Object, beforeCommitEntityProcessorMock2.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange { ChangeType = ChangeType.Update, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entity }
		});

		// Act
		ChangeTrackerImpact result = runner.Run(changes);

		// Assert
		Assert.AreEqual(ChangeTrackerImpact.StateChanged, result);
	}

	public class Entity
	{

	}
}
