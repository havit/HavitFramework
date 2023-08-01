namespace Havit.Services.TimeServices;

/// <summary>
/// Služba vracející aktuální čas a aktuální datum (bez času).
/// </summary>
public interface ITimeService
{
	/// <summary>
	/// Vrací aktuální čas.
	/// </summary>
	DateTime GetCurrentTime();

	/// <summary>
	/// Vrací aktuální datum (bez času).
	/// </summary>
	DateTime GetCurrentDate();
}
