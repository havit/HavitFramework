namespace Havit.Data.Entity.Patterns.UnitOfWorks.EntityValidation;

/// <summary>
/// Výjimka informující o chybě při validaci.
/// </summary>
public class ValidationFailedException : Exception
{
	/// <summary>
	/// Validační chyby.
	/// </summary>
	public IReadOnlyList<string> ValidationErrors { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ValidationFailedException(IEnumerable<string> validationErrors)
	{
		ValidationErrors = validationErrors.ToList().AsReadOnly();
	}

	/// <summary>
	/// Gets a message that describes the current exception.
	/// </summary>
	public override string Message => String.Join(Environment.NewLine, ValidationErrors);
}
