using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Havit.AspNetCore.Mvc.Filters.ModelValidation;

/// <summary>
/// Položka chyby ModelState.
/// </summary>
public class FieldValidationError
{
	/// <summary>
	/// Pole, ve kterém došlo k validační chybě.
	/// </summary>
	/// <remarks>
	/// Vracené fields jsou PascalCase - vychází z pojmenování v .NETu, nikoliv z pojmenování použitého JSON formatterem.
	/// </remarks>
	[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
	public string Field { get; }

	/// <summary>
	/// Zpráva popisující validační chybu.
	/// </summary>
	public string Message { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public FieldValidationError(string field, ModelError modelError)
	{
		Field = (field != string.Empty) ? field : null;
		Message = GetMessage(modelError);
	}

	private string GetMessage(ModelError modelError)
	{
		if (!String.IsNullOrEmpty(modelError.ErrorMessage))
		{
			return modelError.ErrorMessage;
		}

		if (!String.IsNullOrEmpty(modelError.Exception?.Message))
		{
			return modelError.Exception?.Message;
		}

		return String.Empty;
	}

	/// <summary>
	/// Vrací kolekci validačních chyb pro ModelState.
	/// </summary>
	public static ReadOnlyCollection<FieldValidationError> FromModelState(ModelStateDictionary modelState)
	{
		return modelState.Keys
			.SelectMany(key => modelState[key].Errors.Select(modelError => new FieldValidationError(key, modelError)))
			.ToList()
			.AsReadOnly();
	}
}
