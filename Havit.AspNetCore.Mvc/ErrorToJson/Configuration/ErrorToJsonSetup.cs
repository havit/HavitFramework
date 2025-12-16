namespace Havit.AspNetCore.Mvc.ErrorToJson.Configuration;

/// <summary>
/// Zajišťuje konfiguraci mapování z uživatelského kódu.
/// </summary>
public class ErrorToJsonSetup
{
	private readonly List<ErrorToJsonMappingItem> mapping = new List<ErrorToJsonMappingItem>();

	/// <summary>
	/// Mapuje daný typ výjimky na daný stavový kód. Jako návratová hodnota je vrácen objekt s vlastnostmi StatusCode a Message s hodnotou text výjimky (exception.Message).
	/// </summary>
	/// <param name="exceptionType">Typ výjimky.</param>
	/// <param name="statusCode">Stavový kód pro http odpověď (a objekt s odpovědí).</param>
	/// <param name="markExceptionAsHandled">Indikace, zda má být výjimka označena za zpracovanou.</param>
	public void Map(Type exceptionType, int statusCode, bool markExceptionAsHandled = false)
	{
		if (!typeof(Exception).IsAssignableFrom(exceptionType))
		{
			throw new ArgumentException($"{nameof(exceptionType)}: Only exception types can be used.");
		}

		this.Map(e => exceptionType.IsAssignableFrom(e.GetType()), e => statusCode, e => new { StatusCode = statusCode, Message = e.Message }, e => markExceptionAsHandled);
	}

	/// <summary>
	/// Mapuje daný typ výjimky na daný stavový kód. Jako návratová hodnota je vrácen objekt s vlastnostmi StatusCode a Message s hodnotou text výjimky (exception.Message).
	/// </summary>
	/// <param name="predicate">Predikát ověřující položku mapování podle výjimky.</param>
	/// <param name="statusCodeSelector">Funkce vracející pro danou výjimku stavový kód http odpovědi.</param>
	/// <param name="resultSelector">Funkce vracející pro danou výjimku odpověď (objekt následně vracený jako JSON).</param>
	/// <param name="markExceptionAsHandled">Funkce rozhodující, zda má být výjimka označena za zpracovanou.</param>
	public void Map(Predicate<Exception> predicate, Func<Exception, int> statusCodeSelector, Func<Exception, object> resultSelector, Func<Exception, bool> markExceptionAsHandled)
	{
		mapping.Add(new ErrorToJsonMappingItem(predicate, statusCodeSelector, resultSelector, markExceptionAsHandled));
	}

	/// <summary>
	/// Vrací provedenou konfiguraci.
	/// </summary>
	public ErrorToJsonConfiguration GetConfiguration()
	{
		return new ErrorToJsonConfiguration(mapping.AsReadOnly());
	}
}