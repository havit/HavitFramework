namespace Havit.Web.UI.WebControls;

/// <summary>
/// Typ zprávy Messengeru.
/// </summary>
public enum MessageType
{
	/// <summary>
	/// Informace, potvrzení operace.
	/// </summary>
	Information = 0,

	/// <summary>
	/// Varování, např. upozornění na další nutné kroky k dokončení operace.
	/// </summary>
	Warning = 1,

	/// <summary>
	/// Chyba.
	/// </summary>
	Error = 2
}
