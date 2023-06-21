namespace Havit.Data.Entity.Patterns.UnitOfWorks.EntityValidation;

/// <summary>
/// Spouští registrované validátory entit.
/// </summary>
public interface IEntityValidationRunner
{
	/// <summary>
	/// Spustí registrované validátory entit.
	/// Pokud nějaký oznámí chybu, vyhazuje výjimku.	
	/// </summary>
	void Validate(Changes changes);
}
