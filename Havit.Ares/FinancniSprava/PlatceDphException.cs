namespace Havit.Ares.FinancniSprava;

/// <summary>
/// Předek výjimek vracených z volání Nespolehlivého plátce DPH.
/// </summary>	
public class PlatceDphException : ApplicationException
{
	/// <summary>
	/// Podrobný Typ chyby. Chyba se může vyskytnout na WebService (kladné hodnoty), během spojení nebo během parsování XML odpovědi. 
	/// </summary>
	public PlatceDphStatusCode Code { get; }

	/// <summary>
	/// Obsahuje response z volání webové služby. Pouze pro chybu XMLError. 
	/// </summary>
	public string Response { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	internal PlatceDphException(string message, PlatceDphStatusCode code, string response = null, Exception exception = null)
		: base(message, exception)
	{
		Code = code;
		Response = response;
	}
}

