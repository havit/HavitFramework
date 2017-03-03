using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Havit.AspNet.Mvc.Html
{
	/// <summary>
	/// Extension metody pro renderování UI z modelu.
	/// </summary>
	public static class PartialExtensions
	{
		/// <summary>
		/// Vyrenderuje partial view s prefixem datových memberů.
		/// </summary>
		public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, System.Linq.Expressions.Expression<Func<TModel, TProperty>> expression, string partialViewName)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			return PartialFor(helper, expression, partialViewName, name);
		}

		/// <summary>
		/// Vyrenderuje partial view se zadaným prefixem a prefixem HTML fields.
		/// </summary>
		public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, System.Linq.Expressions.Expression<Func<TModel, TProperty>> expression, string partialViewName, string htmlPrefixField)
		{
			object model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;

			ViewDataDictionary viewData = new ViewDataDictionary(helper.ViewData)
			{
				TemplateInfo = new TemplateInfo
				{
					HtmlFieldPrefix = htmlPrefixField
				}
			};

			return helper.Partial(partialViewName, model, viewData);
		}
	}
}