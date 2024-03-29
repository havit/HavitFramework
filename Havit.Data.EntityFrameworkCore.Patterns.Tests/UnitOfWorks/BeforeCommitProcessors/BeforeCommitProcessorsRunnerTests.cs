﻿using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;
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
		beforeCommitEntityProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>()));

		Mock<IBeforeCommitProcessor<object>> beforeCommitObjectProcessorMock = new Mock<IBeforeCommitProcessor<object>>(MockBehavior.Strict);
		beforeCommitObjectProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<object>()));

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create<object>()).Returns(new List<IBeforeCommitProcessor<object>> { beforeCommitObjectProcessorMock.Object });
		beforeCommitProcessorFactoryMock.Setup(m => m.Create<Entity>()).Returns(new List<IBeforeCommitProcessor<Entity>> { beforeCommitEntityProcessorMock.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		Changes changes = new Changes(new[]
		{
			new FakeChange { ChangeType = ChangeType.Insert, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entityInserting },
			new FakeChange { ChangeType = ChangeType.Update, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entityUpdating },
			new FakeChange { ChangeType = ChangeType.Delete, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entityDeleting },
		});

		// Act
		runner.Run(new Changes(changes));

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
