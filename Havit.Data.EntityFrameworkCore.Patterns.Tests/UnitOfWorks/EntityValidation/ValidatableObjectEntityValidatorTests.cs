using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.UnitOfWorks.EntityValidation;

[TestClass]
public class ValidatableObjectEntityValidatorTests
{
	[TestMethod]
	public void ValidatableObjectEntityValidator_SinglePlainMessage()
	{
		// Arrange
		var entity = new Entity(new[] { new ValidationResult("Message") });
		var validator = new ValidatableObjectEntityValidator();

		// Act
		var result = validator.Validate(Patterns.UnitOfWorks.ChangeType.Delete, entity);

		// Assert
		CollectionAssert.AreEquivalent(new[] { "Message" }, result.ToList());
	}

	[TestMethod]
	public void ValidatableObjectEntityValidator_MessageWithProperty()
	{
		// Arrange
		var entity = new Entity(new[] { new ValidationResult("Message", new[] { "Property" }) });
		var validator = new ValidatableObjectEntityValidator();

		// Act
		var result = validator.Validate(Patterns.UnitOfWorks.ChangeType.Delete, entity);

		// Assert
		CollectionAssert.AreEquivalent(new[] { "Message [Property]" }, result.ToList());
	}

	[TestMethod]
	public void ValidatableObjectEntityValidator_MessageWithProperties()
	{
		// Arrange
		var entity = new Entity(new[] { new ValidationResult("Message", new[] { "Property1", "Property2" }) });
		var validator = new ValidatableObjectEntityValidator();

		// Act
		var result = validator.Validate(Patterns.UnitOfWorks.ChangeType.Delete, entity);

		// Assert
		CollectionAssert.AreEquivalent(new[] { "Message [Property1, Property2]" }, result.ToList());
	}

	[TestMethod]
	public void ValidatableObjectEntityValidator_MultipleMessages()
	{
		// Arrange
		var entity = new Entity(new[] { new ValidationResult("Message1"), new ValidationResult("Message2") });
		var validator = new ValidatableObjectEntityValidator();

		// Act
		var result = validator.Validate(Patterns.UnitOfWorks.ChangeType.Delete, entity);

		// Assert
		CollectionAssert.AreEquivalent(new[] { "Message1", "Message2" }, result.ToList());
	}

	public class Entity : IValidatableObject
	{
		private readonly IEnumerable<ValidationResult> validationResults;

		public Entity(IEnumerable<ValidationResult> validationResults)
		{
			this.validationResults = validationResults;
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			return validationResults;
		}
	}

}
