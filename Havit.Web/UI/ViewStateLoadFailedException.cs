namespace Havit.Web.UI;

/// <summary>
/// Výjimka oznamující nenačtení viewstate.
/// </summary>
[Serializable]
public class ViewStateLoadFailedException : Exception
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ViewStateLoadFailedException()
	{

	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ViewStateLoadFailedException(string message)
		: base(message)
	{

	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ViewStateLoadFailedException(string message, Exception innerException)
		: base(message, innerException)
	{

	}
}
