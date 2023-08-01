namespace Havit.Services.Ares;

/// <summary>
/// Výjimka identifikující neúspěch při čtení dat z ARESu (ARES nedostupný, timeout, atp.).
/// </summary>
public class AresLoadException : AresBaseException
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="message">Chyba vrácená systém při příštupu do ARESu.</param>
	internal AresLoadException(string message)
		: base(message)
	{

	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AresLoadException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
	internal AresLoadException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

}
