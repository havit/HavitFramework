namespace Havit.Core;

/// <summary>
/// Pomocné metody pro práci s výjimkami souvisejícími se zrušením operace.
/// </summary>
public static class CancellationExceptionChecker
{
	/// <summary>
	/// Vrací true, pokud je výjimka způsobena zrušením operace.
	/// Podporuje OperationCanceledException (včetně odvozené TaskCanceledException) a SqlException (identifikováno dle textu se zrušením operace, lepší identifikace není k dispozici).
	/// Texty SqlException jsou lokalizované, identifikace zrušení operace je podporována pro text v angličtině a češtině.
	/// </summary>
	public static bool IsCancellationException(Exception exception)
	{
		// "Basic" cancellations. (TaskCanceledException derives from OperationCanceledException)
		// (may also be from SQL client if no network communication has occurred yet)
		if (exception is OperationCanceledException)
		{
			return true;
		}

		// Cancellation from SQL client
		return (exception.GetType().FullName == "Microsoft.Data.SqlClient.SqlException")
			&& ((exception.Message.Contains("A severe error occurred on the current command.  The results, if any, should be discarded.") || exception.Message.Contains("Operation cancelled by user."))
				|| (exception.Message.Contains("U současného příkazu se stala závažná chyba. Pokud existují nějaké výsledky, měly by se zahodit.") && exception.Message.Contains("Operace byla zrušena uživatelem.")));
	}
}
