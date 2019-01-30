using System;

namespace Havit.AspNetCore.Mvc.ErrorToJson.Configuration
{
	/// <summary>
	/// Položka mapování výjimky
	/// </summary>
    public class ErrorToJsonMappingItem
    {
		/// <summary>
		/// Predikát ověřující položku mapování podle výjimky.
		/// </summary>
        public Predicate<Exception> ExceptionPredicate { get; private set; }

		/// <summary>
		/// Funkce vracející pro danou výjimku stavový kód http odpovědi.
		/// </summary>
        public Func<Exception, int> StatusCodeSelector { get; private set; }

	    /// <summary>
	    /// Funkce vracející pro danou výjimku odpověď (objekt následně vracený jako JSON).
	    /// </summary>
        public Func<Exception, object> ResultSelector { get; private set; }

		/// <summary>
		/// Funkce rozhodující, zda má být výjimka označena za zpracovanou.
		/// </summary>
        public Func<Exception, bool> MarkExceptionAsHandledFunc { get; private set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="predicate">Predikát ověřující položku mapování podle typu výjimky.</param>
		/// <param name="statusCodeSelector">Funkce vracející pro danou výjimku stavový kód http odpovědi.</param>
		/// <param name="resultSelector">Funkce vracející pro danou výjimku odpověď (objekt následně vracený jako JSON).</param>
		/// <param name="markExceptionAsHandledFunc">Funkce rozhodující, zda má být výjimka označena za zpracovanou.</param>
        public ErrorToJsonMappingItem(Predicate<Exception> predicate, Func<Exception, int> statusCodeSelector, Func<Exception, object> resultSelector, Func<Exception, bool> markExceptionAsHandledFunc)
        {
            ExceptionPredicate = predicate;
            StatusCodeSelector = statusCodeSelector;
            ResultSelector = resultSelector;
            MarkExceptionAsHandledFunc = markExceptionAsHandledFunc;
        }
    }
}