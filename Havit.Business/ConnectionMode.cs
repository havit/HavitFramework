namespace Havit.Business;

/// <summary>
/// Režim funkce objektů.
/// </summary>
public enum ConnectionMode
{
	/// <summary>
	/// Klasický běžný "connected" business objekt.
	/// </summary>
	Connected = 1,

	/// <summary>
	/// Disconnected business objekt pro účely testování.
	/// </summary>
	Disconnected = 2
}