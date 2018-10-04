using System;
using System.Collections.Generic;

namespace Havit.AspNetCore.Mvc.Filters.ErrorToJson
{
	/// <summary>
	/// Zajišuje konfiguraci mapování z uivatelského kódu.
	/// </summary>
    public class ErrorToJsonSetup
    {
        private readonly List<ErrorToJsonMappingItem> mapping = new List<ErrorToJsonMappingItem>();

		/// <summary>
		/// Mapuje danı typ vıjimky na danı stavovı kód. Jako návratová hodnota je vrácen objekt s vlastnostmi StatusCode a Message s hodnotou text vıjimky (exception.Message).
		/// </summary>
		/// <param name="exceptionType">Typ vıjimky.</param>
		/// <param name="statusCode">Stavovı kód pro http odpovìï (a objekt s odpovìdí).</param>
		/// <param name="markExceptionAsHandled">Indikace, zda má bıt vıjimka oznaèena za zpracovanou.</param>
        public void Map(Type exceptionType, int statusCode, bool markExceptionAsHandled = false)
        {
	        if (!typeof(Exception).IsAssignableFrom(exceptionType))
	        {
		        throw new ArgumentException("Only exception types can be used.", nameof(exceptionType));
	        }

			this.Map(e => exceptionType.IsAssignableFrom(e.GetType()), e => statusCode, e => new { StatusCode = statusCode, Message = e.Message }, e => markExceptionAsHandled);
        }

		/// <summary>
		/// Mapuje danı typ vıjimky na danı stavovı kód. Jako návratová hodnota je vrácen objekt s vlastnostmi StatusCode a Message s hodnotou text vıjimky (exception.Message).
		/// </summary>
		/// <param name="predicate">Predikát ovìøující poloku mapování podle vıjimky.</param>
		/// <param name="statusCodeSelector">Funkce vracející pro danou vıjimku stavovı kód http odpovìdi.</param>
		/// <param name="resultSelector">Funkce vracející pro danou vıjimku odpovìï (objekt následnì vracenı jako JSON).</param>
		/// <param name="markExceptionAsHandled">Funkce rozhodující, zda má bıt vıjimka oznaèena za zpracovanou.</param>
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
}