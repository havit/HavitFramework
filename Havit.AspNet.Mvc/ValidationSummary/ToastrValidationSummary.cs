using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Havit.Web;

namespace Havit.AspNet.Mvc.ValidationSummary
{
	/// <summary>
	/// Validation summary represented by toastr javascript library.
	/// </summary>
	public class ToastrValidationSummary : IValidationSummary
	{
		private readonly List<ModelError> modelErrors;

		/// <summary>
		/// Initializes a new instance of the <see cref="ToastrValidationSummary"/> class.
		/// </summary>
		/// <param name="modelErrors">The model errors.</param>
		public ToastrValidationSummary(List<ModelError> modelErrors)
		{
			this.modelErrors = modelErrors;
		}

		/// <summary>
		/// Returns validation summary prepared to render into page.
		/// </summary>
		public MvcHtmlString Render()
		{
			StringBuilder sb = new StringBuilder();

			foreach (ModelError modelError in modelErrors)
			{
				string encodedMessage = HttpUtilityExt.HtmlEncode(modelError.ErrorMessage, HtmlEncodeOptions.None);
				sb.AppendLine(encodedMessage);
			}

			TagBuilder builder = new TagBuilder("script");
			builder.Attributes.Add("type", "text/javascript");
			builder.InnerHtml = "toastr.error(\"" + sb.ToString().TrimEnd().Replace("\n", "<br />").Replace("\r", "") + "\");";

			return new MvcHtmlString(builder.ToString());
		}
	}
}
