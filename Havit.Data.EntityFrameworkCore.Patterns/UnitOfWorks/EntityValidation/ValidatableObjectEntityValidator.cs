using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;

/// <summary>
/// Validates the entity using its IValidatableObject.Validate() method.
/// </summary>
public class ValidatableObjectEntityValidator : IEntityValidator<object>
{
	/// <summary>
	/// Validates the entity using its IValidatableObject.Validate() method.
	/// </summary>
	public IEnumerable<string> Validate(ChangeType changeType, object changingEntity)
	{
		if (changingEntity is IValidatableObject validatableObject)
		{
			return validatableObject
				.Validate(new ValidationContext(validatableObject))
				.Select(validationResult =>
					validationResult.MemberNames.Any()
					? validationResult.ErrorMessage + " [" + string.Join(", ", validationResult.MemberNames) + "]"
					: validationResult.ErrorMessage);
		}
		return Enumerable.Empty<string>();
	}
}
