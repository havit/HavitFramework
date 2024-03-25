using System.Diagnostics.CodeAnalysis;

namespace Havit.Ares;

/// <summary>
/// Předek výjimek vracených z volání Nespolehlivého plátce DPH.
/// </summary>	
public class PlatceDphException : ApplicationException
{
	/// Podrobný Typ chyby. Chyba se může vyskytnout na WebService (kladné hodnoty), během spojení nebo během parsování XML odpovědi. 
	public PlatceDphStatusCode Code { get; }
	/// Obsahuje XML Response. Pouze pro chybu XMLError. 
	public string Response { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	internal PlatceDphException(string message, PlatceDphStatusCode code, string response = null, Exception exception = null)
		: base(message, exception)
	{
		this.Code = code;
		this.Response = response;
	}
}

