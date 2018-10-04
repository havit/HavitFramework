using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Havit.Web.Management;

namespace Havit.AspNet.Mvc
{
	/// <summary>
	/// Zajišťuje zpracování neobsloužené výjimky v aplikaci (controller, view, atp.).
	/// </summary>
	/// <remarks>
	/// Ke zpracování výjimek dochází pouze v případě, že jsou povoleny custom errors (HttpContext.IsCustomErrorEnabled). Tím se snažíme neřešit chyby při lokálním vývoji.
	/// Výjimka je odeslána healthmonitoringem a označena jako zpracovaná.
	/// Attribut dále zajistí zobrazení view "Error".
	/// </remarks>
	public class HandleErrorHmAttribute : HandleErrorAttribute
	{
		/// <summary>
		/// Called when an exception occurs.
		/// </summary>
		public override void OnException(ExceptionContext context)
		{
			base.OnException(context);

			if (context.HttpContext.IsCustomErrorEnabled)
			{
				new WebRequestErrorEventExt(context.Exception.Message, this, context.Exception, HttpContext.Current).Raise();

				context.ExceptionHandled = true;
				context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				context.HttpContext.Response.TrySkipIisCustomErrors = true;
				context.Result = new ViewResult { ViewName = "Error" };
			}
		}
	}
}