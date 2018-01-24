using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Havit.AspNetCore.Mvc.Filters.ErrorToJson
{
	/// <summary>
	/// Filtr zajišťující konverzi výjimky na JSON odpověď.
	/// </summary>
    public class ErrorJoJsonFilter : ExceptionFilterAttribute
    {
        private readonly ErrorToJsonConfiguration configuration;

		/// <summary>
		/// Konstruktor.
		/// </summary>
        public ErrorJoJsonFilter(Action<ErrorToJsonSetup> setupAction)
        {
            ErrorToJsonSetup setup = new ErrorToJsonSetup();
            setupAction.Invoke(setup);
            configuration = setup.GetConfiguration();
        }

		/// <summary>
		/// Zajistí konverzi výjimky ja JSON odpověď.
		/// </summary>
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            Exception exception = context.Exception;

            var mappingItem = this.configuration.FindMapping(exception);
            if (mappingItem != null)
            {
	            object result = mappingItem.ResultSelector(exception);
                bool markExceptionAsHandled = mappingItem.MarkExceptionAsHandledFunc(exception);

                context.Result = new Microsoft.AspNetCore.Mvc.ObjectResult(result) { StatusCode = mappingItem.StatusCodeSelector(exception) };
                context.ExceptionHandled = markExceptionAsHandled;
            }
        }
    }
}