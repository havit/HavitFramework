using System;
using System.Diagnostics.CodeAnalysis;
using Havit.AspNetCore.Mvc.ErrorToJson.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Havit.AspNetCore.Mvc.ErrorToJson.Filters;

    /// <summary>
    /// Filtr zajišťující konverzi výjimky na JSON odpověď.
    /// </summary>
[SuppressMessage("SonarLint", "S3376", Justification = "V ASP.NET Core MVC je toto zamýšleno jako globální filtr, pak se slovo Attribute na konci názvu nevyžaduje.")]
    public class ErrorToJsonFilter : ExceptionFilterAttribute
    {
	private readonly IErrorToJsonService errorToJsonService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ErrorToJsonFilter(IErrorToJsonService errorToJsonService)
        {
		this.errorToJsonService = errorToJsonService;
	}

	/// <summary>
	/// Zajistí konverzi výjimky na JSON odpověď.
	/// </summary>
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

		var result = errorToJsonService.GetResultData(context.Exception);
		if (result != null)
		{
			context.Result = new Microsoft.AspNetCore.Mvc.ObjectResult(result.Data) { StatusCode = result.StatusCode };
			context.ExceptionHandled = result.ExceptionHandled;
		}
        }
    }