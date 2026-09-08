namespace Havit.GoPay.DataObjects;

/// <summary>
/// Go pay exception
/// </summary>
public class GoPayResponseException : Exception
{
	/// <summary>
	/// Konstruktor
	/// </summary>
	/// <param name="message">Text vyjímky</param>
	/// <param name="innerException">Vnitřní vyjímka</param>
	public GoPayResponseException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
