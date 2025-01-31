namespace Havit.Web.UI;

/// <summary>
/// Výsledek o pokus zaregistrování klientských skriptů.
/// </summary>
public enum TryEnsureScriptRegistrationResult
{
	/// <summary>
	/// Skripty se podařilo zaregistorvat, pokud nebyli.
	/// </summary>
	OK,

	/// <summary>
	/// Chybí namapování skriptů.
	/// </summary>
	MissingScriptResourceMapping,

	/// <summary>
	/// Pokus o registraci skriptu probíhá při častečném (async) postabacku, což není možné.
	/// </summary>
	ScriptResourceMappingWhileAsyncPostback
}