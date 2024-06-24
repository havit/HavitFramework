using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Havit.AspNetCore.Mvc.Filters.ModelValidation;

/// <summary>
/// Pokud není ModelState validní, vrací odpověď (dle ResultSelectoru) bez dalšího zpracování action.
/// </summary>
/// <remarks>
/// Vracené fields jsou PascalCase - vychází z pojmenování v .NETu, nikoliv z pojmenování použitého JSON formatterem.
/// </remarks>
public class ValidateModelAttribute : ActionFilterAttribute
{
	/// <summary>
	/// Určí stavový kódu odpovědi.		
	/// </summary>
	/// <remarks>
	/// Výchozí hodnota je DefaultStatusCodeSelector.
	/// </remarks>
	public StatusCodeSelectorDelegate StatusCodeSelector { get; set; } = DefaultStatusCodeSelector;

	/// <summary>
	/// Vrací odpověď v případě nevalidního ModelState.
	/// </summary>
	/// <remarks>
	/// Výchozí hodnota je ValidationResultModel.FromModelState.
	/// </remarks>
	public ResultSelectorDelegate ResultSelector { get; set; } = ValidationResultModel.FromModelState;

	/// <summary>
	/// Pokud není ModelState validní, vrací odpověď dle ResultSelectoru, je nastaven StatusCode dle StatuCodeSelectoru.
	/// Dojde tím k zastavení zpracování vlastní action.
	/// </summary>
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		if (!context.ModelState.IsValid)
		{
			int statusCode = StatusCodeSelector(context.ModelState);
			object result = ResultSelector(statusCode, context.ModelState);

			context.Result = new ObjectResult(result) { StatusCode = statusCode };
		}
	}

	/// <summary>
	/// Určí status code dle ModelState.
	/// </summary>
	/// <returns>
	/// Pokud je v ModelState obsažena výjimka (došlo k chybě při deserializaci), je vrácen Status500InternalServerError.
	/// V ostatních případech Status422UnprocessableEntity.
	/// </returns>
	public static int DefaultStatusCodeSelector(ModelStateDictionary modelStateDictionary)
	{
		return modelStateDictionary.Values.Any(item => item.Errors.Any(error => error.Exception != null))
			? StatusCodes.Status500InternalServerError
			: StatusCodes.Status422UnprocessableEntity;
	}
}
