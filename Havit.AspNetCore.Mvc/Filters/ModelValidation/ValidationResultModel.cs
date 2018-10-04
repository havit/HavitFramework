using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Havit.AspNetCore.Mvc.Filters.ModelValidation
{
	/// <summary>
	/// Model pro odpověď pro nevalidní ModelState.
	/// </summary>
	public class ValidationResultModel
	{
		/// <summary>
		/// Status kód odpovědi (zajišťuje se, aby měl stejnou hodnotu jako http response).
		/// </summary>
		public int StatusCode { get; }

		/// <summary>
		/// Kolekce validačních chyb.
		/// </summary>
		public ReadOnlyCollection<FieldValidationError> Errors { get; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ValidationResultModel(int statusCode, ModelStateDictionary modelState)
		{
			this.StatusCode = statusCode;
			this.Errors = FieldValidationError.FromModelState(modelState);
		}

		/// <summary>
		/// Funkce vrací ValidationResultModel pro daný StatusCode a ModelState.
		/// </summary>
		public static object FromModelState(int statusCode, ModelStateDictionary modelState)
		{
			return new ValidationResultModel(statusCode, modelState);
		}
	}
}
