using Havit.AspNet.Mvc.ValidationSummary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Havit.AspNet.Mvc.Html;

/// <summary>
/// Extension metody pro validaci.
/// </summary>
public static class ValidationExtensions
{
	/// <summary>
	/// Vyrenderuje validation summary zobrazené pomocí Toastru.
	/// </summary>
	public static MvcHtmlString ToastrValidationSummary(this HtmlHelper htmlHelper)
	{
		if (htmlHelper.ViewData.ModelState.IsValid)
		{
			return null;
		}

		List<ModelError> modelErrors = htmlHelper.ViewContext.ViewData.ModelState.SelectMany(item => item.Value.Errors).ToList();

		IValidationSummary validationSummary = new ToastrValidationSummary(modelErrors);
		return validationSummary.Render();
	}
}
