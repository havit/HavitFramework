using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.UnitOfWorks.EntityValidation
{
	[TestClass]
	public class EntityValidationRunnerTests
	{
		[TestMethod]
		public void EntityValidationRunner_Validate()
		{
			// Arrange
			Entity entityInserting = new Entity();
			Entity entityUpdating = new Entity();
			Entity entityDeleting = new Entity();

			Mock<IEntityValidator<Entity>> entityValidatorMock = new Mock<IEntityValidator<Entity>>(MockBehavior.Strict);
			entityValidatorMock.Setup(m => m.Validate(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(Enumerable.Empty<string>());

			Mock<IEntityValidator<object>> entityValidatorMock2 = new Mock<IEntityValidator<object>>(MockBehavior.Strict);
			entityValidatorMock2.Setup(m => m.Validate(It.IsAny<ChangeType>(), It.IsAny<object>())).Returns(Enumerable.Empty<string>());

			Mock<IEntityValidatorsFactory> entityValidatorsFactoryMock = new Mock<IEntityValidatorsFactory>(MockBehavior.Strict);
			entityValidatorsFactoryMock.Setup(m => m.Create<object>()).Returns(new List<IEntityValidator<object>> { entityValidatorMock2.Object });
			entityValidatorsFactoryMock.Setup(m => m.Create<Entity>()).Returns(new List<IEntityValidator<Entity>> { entityValidatorMock.Object });

			EntityValidationRunner runner = new EntityValidationRunner(entityValidatorsFactoryMock.Object);

			// Act
			runner.Validate(new Changes
			{
				Inserts = new object[] { entityInserting },
				Updates = new object[] { entityUpdating },
				Deletes = new object[] { entityDeleting }
			});

			// Assert
			entityValidatorsFactoryMock.Verify(m => m.Create<Entity>(), Times.AtLeastOnce);
			entityValidatorsFactoryMock.Verify(m => m.Create<object>(), Times.AtLeastOnce);

			entityValidatorMock.Verify(m => m.Validate(ChangeType.Insert, entityInserting), Times.Once);
			entityValidatorMock.Verify(m => m.Validate(ChangeType.Update, entityUpdating), Times.Once);
			entityValidatorMock.Verify(m => m.Validate(ChangeType.Delete, entityDeleting), Times.Once);
			entityValidatorMock.Verify(m => m.Validate(It.IsAny<ChangeType>(), It.IsAny<Entity>()), Times.Exactly(3));

			entityValidatorMock2.Verify(m => m.Validate(ChangeType.Insert, entityInserting), Times.Once);
			entityValidatorMock2.Verify(m => m.Validate(ChangeType.Update, entityUpdating), Times.Once);
			entityValidatorMock2.Verify(m => m.Validate(ChangeType.Delete, entityDeleting), Times.Once);
			entityValidatorMock2.Verify(m => m.Validate(It.IsAny<ChangeType>(), It.IsAny<object>()), Times.Exactly(3));
		}

		[TestMethod]
		[ExpectedException(typeof(ValidationFailedException))]
		public void EntityValidationRunner_Validate_ThrowsExceptionWhenValidationFails()
		{
			// Arrange
			Entity entityInserting = new Entity();

			Mock<IEntityValidator<Entity>> entityValidatorMock = new Mock<IEntityValidator<Entity>>(MockBehavior.Strict);
			entityValidatorMock.Setup(m => m.Validate(It.IsAny<ChangeType>(), It.IsAny<Entity>())).Returns(new List<string> { "Some validation error." });

			Mock<IEntityValidatorsFactory> entityValidatorsFactoryMock = new Mock<IEntityValidatorsFactory>(MockBehavior.Strict);
			entityValidatorsFactoryMock.Setup(m => m.Create<object>()).Returns(new List<IEntityValidator<object>> { });
			entityValidatorsFactoryMock.Setup(m => m.Create<Entity>()).Returns(new List<IEntityValidator<Entity>> { entityValidatorMock.Object });

			EntityValidationRunner runner = new EntityValidationRunner(entityValidatorsFactoryMock.Object);

			// Act
			runner.Validate(new Changes
			{
				Inserts = new object[] { entityInserting },
				Updates = new object[] { },
				Deletes = new object[] { }
			});

			// Assert by method attribute
		}

		public class Entity
		{

		}
	}
}
