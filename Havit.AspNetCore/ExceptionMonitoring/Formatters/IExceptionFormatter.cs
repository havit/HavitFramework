namespace Havit.AspNetCore.ExceptionMonitoring.Formatters;

/// <summary>
/// Formatter výjimky.
/// </summary>
public interface IExceptionFormatter
{
	/// <summary>
	/// Vrátí text reprezentující lidsky čitelné informace o výjimce.
	/// </summary>
	string FormatException(Exception exception);
}
