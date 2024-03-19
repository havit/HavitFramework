using System.Diagnostics.CodeAnalysis;

namespace Havit.Ares;

/// <summary>
/// Předek výjimek vracených z volání Nespolehlivého plátce DPH.
/// </summary>	
public class AresDphException : ApplicationException
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="message">Exception message.</param>
	/// <param name="code">""</param>
	internal AresDphException(string message, PlatceDphStatusCode code)
		: base(message)
	{
		this.Code = code;
	}

	internal PlatceDphStatusCode Code { get; }
}

