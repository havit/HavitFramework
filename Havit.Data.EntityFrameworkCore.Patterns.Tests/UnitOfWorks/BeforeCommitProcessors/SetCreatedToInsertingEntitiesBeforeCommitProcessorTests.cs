using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Services.TimeServices;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.UnitOfWorks.BeforeCommitProcessors;

[TestClass]
public class SetCreatedToInsertingEntitiesBeforeCommitProcessorTests
{
	[TestMethod]
	public void SetCreatedToInsertingEntitiesBeforeCommitProcessor_Run_SetsCreatedWhenNotSet()
	{
		// Arrange
		EntityWithCreatedDateTime entity = new EntityWithCreatedDateTime();

		DateTime now = new DateTime(2017, 1, 1);
		Mock<ITimeService> mockTimeService = new Mock<ITimeService>(MockBehavior.Strict);
		mockTimeService.Setup(m => m.GetCurrentTime()).Returns(now);
		SetCreatedToInsertingEntitiesBeforeCommitProcessor processor = new SetCreatedToInsertingEntitiesBeforeCommitProcessor(mockTimeService.Object);

		// Act
		processor.Run(ChangeType.Insert, entity);

		// Assert
		Assert.AreEqual(now, entity.Created);
	}

	[TestMethod]
	public void SetCreatedToInsertingEntitiesBeforeCommitProcessor_Run_DoesNotSetsCreatedWhenSet()
	{
		// Arrange
		DateTime originalDateTime = new DateTime(2017, 1, 1);

		EntityWithCreatedDateTime entity = new EntityWithCreatedDateTime()
		{
			Created = originalDateTime
		};

		DateTime now = new DateTime(2017, 1, 1);
		Mock<ITimeService> mockTimeService = new Mock<ITimeService>(MockBehavior.Strict);
		mockTimeService.Setup(m => m.GetCurrentTime()).Returns(now);
		SetCreatedToInsertingEntitiesBeforeCommitProcessor processor = new SetCreatedToInsertingEntitiesBeforeCommitProcessor(mockTimeService.Object);

		// Act
		processor.Run(ChangeType.Insert, entity);

		// Assert
		Assert.AreEqual(originalDateTime, entity.Created);
	}

	[TestMethod]
	public void SetCreatedToInsertingEntitiesBeforeCommitProcessor_Run_SetsCreatedWhenNotInserting()
	{
		// Arrange
		EntityWithCreatedDateTime entity1 = new EntityWithCreatedDateTime();
		EntityWithCreatedDateTime entity2 = new EntityWithCreatedDateTime();

		DateTime now = new DateTime(2017, 1, 1);
		Mock<ITimeService> mockTimeService = new Mock<ITimeService>(MockBehavior.Strict);
		mockTimeService.Setup(m => m.GetCurrentTime()).Returns(now);
		SetCreatedToInsertingEntitiesBeforeCommitProcessor processor = new SetCreatedToInsertingEntitiesBeforeCommitProcessor(mockTimeService.Object);

		// Act
		processor.Run(ChangeType.Update, entity1);
		processor.Run(ChangeType.Delete, entity2);

		// Assert
		Assert.AreEqual(default(DateTime), entity1.Created);
		Assert.AreEqual(default(DateTime), entity2.Created);
	}

	[TestMethod]
	public void SetCreatedToInsertingEntitiesBeforeCommitProcessor_Run_DoesNothingWhenCreatedDoesNotExist()
	{
		// Arrange
		EntityWithoutCreated entity = new EntityWithoutCreated();

		DateTime now = new DateTime(2017, 1, 1);
		Mock<ITimeService> mockTimeService = new Mock<ITimeService>(MockBehavior.Strict);
		mockTimeService.Setup(m => m.GetCurrentTime()).Returns(now);
		SetCreatedToInsertingEntitiesBeforeCommitProcessor processor = new SetCreatedToInsertingEntitiesBeforeCommitProcessor(mockTimeService.Object);

		// Act
		processor.Run(ChangeType.Insert, entity);

		// Assert
		// no exception thrown
	}

	private class EntityWithCreatedDateTime
	{
		public DateTime Created { get; set; }
	}

	private class EntityWithoutCreated
	{
	}

}
