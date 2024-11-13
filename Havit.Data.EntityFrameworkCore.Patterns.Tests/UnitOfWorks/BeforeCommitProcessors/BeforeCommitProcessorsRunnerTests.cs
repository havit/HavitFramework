using System.Text;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors.Internal;
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

		Mock<IBeforeCommitProcessorInternal> beforeCommitProcessorMock = new Mock<IBeforeCommitProcessorInternal>(MockBehavior.Strict);
		beforeCommitProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.NoImpact);
		beforeCommitProcessorMock.Setup(m => m.RunAsync(It.IsAny<ChangeType>(), It.IsAny<Entity>(), It.IsAny<CancellationToken>())).ReturnsAsync(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create(typeof(Entity))).Returns(new List<IBeforeCommitProcessorInternal> { beforeCommitProcessorMock.Object });

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
		beforeCommitProcessorFactoryMock.Verify(m => m.Create(typeof(Entity)), Times.AtLeastOnce);

		beforeCommitProcessorMock.Verify(m => m.Run(ChangeType.Insert, entityInserting), Times.Once);
		beforeCommitProcessorMock.Verify(m => m.Run(ChangeType.Update, entityUpdating), Times.Once);
		beforeCommitProcessorMock.Verify(m => m.Run(ChangeType.Delete, entityDeleting), Times.Once);
		beforeCommitProcessorMock.Verify(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>()), Times.Exactly(3));
	}

	[TestMethod]
	public async Task BeforeCommitProcessorsRunner_RunAsync_RunsProcessors()
	{
		// Arrange
		Entity entityInserting = new Entity();
		Entity entityUpdating = new Entity();
		Entity entityDeleting = new Entity();

		Mock<IBeforeCommitProcessorInternal> beforeCommitProcessorMock = new Mock<IBeforeCommitProcessorInternal>(MockBehavior.Strict);
		beforeCommitProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.NoImpact);
		beforeCommitProcessorMock.Setup(m => m.RunAsync(It.IsAny<ChangeType>(), It.IsAny<Entity>(), It.IsAny<CancellationToken>())).ReturnsAsync(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create(typeof(Entity))).Returns(new List<IBeforeCommitProcessorInternal> { beforeCommitProcessorMock.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange { ChangeType = ChangeType.Insert, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entityInserting },
			new FakeChange { ChangeType = ChangeType.Update, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entityUpdating },
			new FakeChange { ChangeType = ChangeType.Delete, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entityDeleting },
		});

		// Act
		await runner.RunAsync(changes);

		// Assert
		beforeCommitProcessorFactoryMock.Verify(m => m.Create(typeof(Entity)), Times.AtLeastOnce);

		beforeCommitProcessorMock.Verify(m => m.Run(ChangeType.Insert, entityInserting), Times.Once);
		beforeCommitProcessorMock.Verify(m => m.Run(ChangeType.Update, entityUpdating), Times.Once);
		beforeCommitProcessorMock.Verify(m => m.Run(ChangeType.Delete, entityDeleting), Times.Once);
		beforeCommitProcessorMock.Verify(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>()), Times.Exactly(3));
	}

	[TestMethod]
	public void BeforeCommitProcessorsRunner_Run_RunsProcessors_ReturnsNoImpactWhenThereIsNoImpact()
	{
		// Arrange
		Entity entity = new Entity();

		Mock<IBeforeCommitProcessorInternal> beforeCommitProcessorMock = new Mock<IBeforeCommitProcessorInternal>(MockBehavior.Strict);
		beforeCommitProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.NoImpact);
		beforeCommitProcessorMock.Setup(m => m.RunAsync(It.IsAny<ChangeType>(), It.IsAny<Entity>(), It.IsAny<CancellationToken>())).ReturnsAsync(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create(typeof(Entity))).Returns(new List<IBeforeCommitProcessorInternal> { beforeCommitProcessorMock.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange { ChangeType = ChangeType.Update, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entity }
		});

		// Act
		ChangeTrackerImpact result = runner.Run(changes);

		// Assert
		beforeCommitProcessorMock.Verify(m => m.Run(ChangeType.Update, entity), Times.Once);
		Assert.AreEqual(ChangeTrackerImpact.NoImpact, result);
	}

	[TestMethod]
	public async Task BeforeCommitProcessorsRunner_RunAsync_RunsProcessors_ReturnsNoImpactWhenThereIsNoImpact()
	{
		// Arrange
		Entity entity = new Entity();

		Mock<IBeforeCommitProcessorInternal> beforeCommitProcessorMock = new Mock<IBeforeCommitProcessorInternal>(MockBehavior.Strict);
		beforeCommitProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.NoImpact);
		beforeCommitProcessorMock.Setup(m => m.RunAsync(It.IsAny<ChangeType>(), It.IsAny<Entity>(), It.IsAny<CancellationToken>())).ReturnsAsync(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create(typeof(Entity))).Returns(new List<IBeforeCommitProcessorInternal> { beforeCommitProcessorMock.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange { ChangeType = ChangeType.Update, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entity }
		});

		// Act
		ChangeTrackerImpact result = await runner.RunAsync(changes);

		// Assert
		beforeCommitProcessorMock.Verify(m => m.Run(ChangeType.Update, entity), Times.Once);
		Assert.AreEqual(ChangeTrackerImpact.NoImpact, result);
	}

	[TestMethod]
	public void BeforeCommitProcessorsRunner_Run_RunsProcessors_ReturnsStateChangesWhenThereIsAtLeastOneChange()
	{
		// Arrange
		Entity entity = new Entity();

		Mock<IBeforeCommitProcessorInternal> beforeCommitProcessorMock = new Mock<IBeforeCommitProcessorInternal>(MockBehavior.Strict);
		beforeCommitProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.StateChanged);
		beforeCommitProcessorMock.Setup(m => m.RunAsync(It.IsAny<ChangeType>(), It.IsAny<Entity>(), It.IsAny<CancellationToken>())).ReturnsAsync(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create(typeof(Entity))).Returns(new List<IBeforeCommitProcessorInternal> { beforeCommitProcessorMock.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange { ChangeType = ChangeType.Update, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entity } });

		// Act
		ChangeTrackerImpact result = runner.Run(changes);

		// Assert
		beforeCommitProcessorMock.Verify(m => m.Run(ChangeType.Update, entity), Times.Once);
		Assert.AreEqual(ChangeTrackerImpact.StateChanged, result);
	}

	[TestMethod]
	public async Task BeforeCommitProcessorsRunner_RunAsync_RunsProcessors_ReturnsStateChangesWhenThereIsAtLeastOneChange()
	{
		// Arrange
		Entity entity = new Entity();

		Mock<IBeforeCommitProcessorInternal> beforeCommitProcessorMock = new Mock<IBeforeCommitProcessorInternal>(MockBehavior.Strict);
		beforeCommitProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.StateChanged);
		beforeCommitProcessorMock.Setup(m => m.RunAsync(It.IsAny<ChangeType>(), It.IsAny<Entity>(), It.IsAny<CancellationToken>())).ReturnsAsync(ChangeTrackerImpact.NoImpact);

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create(typeof(Entity))).Returns(new List<IBeforeCommitProcessorInternal> { beforeCommitProcessorMock.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange { ChangeType = ChangeType.Update, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entity } });

		// Act
		ChangeTrackerImpact result = await runner.RunAsync(changes);

		// Assert
		beforeCommitProcessorMock.Verify(m => m.Run(ChangeType.Update, entity), Times.Once);
		Assert.AreEqual(ChangeTrackerImpact.StateChanged, result);
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
	public void BeforeCommitProcessorsRunner_Run_ThrowsExceptionWhenThereIsAsyncBeforeCommitProcessor()
	{
		// Arrange
		Entity entity = new Entity();

		Mock<IBeforeCommitProcessorInternal> beforeCommitProcessorMock = new Mock<IBeforeCommitProcessorInternal>(MockBehavior.Strict);
		beforeCommitProcessorMock.Setup(m => m.Run(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(ChangeTrackerImpact.StateChanged);
		beforeCommitProcessorMock.Setup(m => m.RunAsync(It.IsAny<ChangeType>(), It.IsAny<Entity>(), It.IsAny<CancellationToken>())).Returns(async () =>
		{
			await Task.Yield();
			return ChangeTrackerImpact.NoImpact;
		});

		Mock<IBeforeCommitProcessorsFactory> beforeCommitProcessorFactoryMock = new Mock<IBeforeCommitProcessorsFactory>(MockBehavior.Strict);
		beforeCommitProcessorFactoryMock.Setup(m => m.Create(typeof(Entity))).Returns(new List<IBeforeCommitProcessorInternal> { beforeCommitProcessorMock.Object });

		BeforeCommitProcessorsRunner runner = new BeforeCommitProcessorsRunner(beforeCommitProcessorFactoryMock.Object);

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange { ChangeType = ChangeType.Update, ClrType = typeof(Entity), EntityType = null /* pro účely testu není třeba */, Entity = entity }
		});

		// Act
		ChangeTrackerImpact result = runner.Run(changes);

		// Assert by method attribute		
	}

	public class Entity
	{

	}
}
