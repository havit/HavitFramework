using System.Diagnostics.CodeAnalysis;

namespace Havit.Ares;

/// <summary>
/// Předek výjimek vracených z volání Nespolehlivého plátce DPH.
/// </summary>	
public class AresDphException : ApplicationException
{
	public PlatceDphStatusCode Code { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	internal AresDphException(string message, PlatceDphStatusCode code, Exception exception = null)
		: base(message, exception)
	{
		this.Code = code;
	}
}

