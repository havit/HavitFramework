using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Havit.AspNet.Mvc.ValidationSummary
{
	/// <summary>
	/// Defines validation summary.
	/// </summary>
	public interface IValidationSummary
	{
		/// <summary>
		/// Returns validation summary prepared to render into page.
		/// </summary>
		MvcHtmlString Render();
	}
}
