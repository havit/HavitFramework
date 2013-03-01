using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Třída pro customizaci datetimeboxu
	/// </summary>
	public abstract class DateTimeBoxDateCustomization
	{
		#region DateCustomizationFunctionName
		/// <summary>
		/// Název javascript funkce pro customizaci DateTimeBoxu.
		/// </summary>
		private string DateCustomizationFunctionName
		{
			get;
			set;
		} 
		#endregion

		#region DateCustomizationFunctionContent
		/// <summary>
		/// Obsah (tělo) javascript funkce pro customizaci DateTimeBoxu.
		/// </summary>
		private string DateCustomizationFunctionContent
		{
			get;
			set;
		}
		#endregion

		#region GetDatesCustomizationFunction
		/// <summary>
		/// Vygeneruje funkci se special dates.
		/// </summary>		
		public string GetDatesCustomizationFunction(Page page)
		{
			if (String.IsNullOrEmpty(DateCustomizationFunctionName))
			{
				lock (_getDatesCustomizationFunctionLock)
				{
					if (String.IsNullOrEmpty(DateCustomizationFunctionName))
					{
						Guid functionNameGuid = Guid.NewGuid();
						string functionName = "_" + functionNameGuid.ToString().Replace("-", "");
						DateCustomizationFunctionName = functionName;

						StringBuilder sb = new StringBuilder();
						sb.AppendLine("function " + functionName + "(date, y, m, d) {");
						sb.AppendLine(RenderDateStatusHandlerContent());
						sb.AppendLine("}");
						DateCustomizationFunctionContent = sb.ToString();
					}
				}
			}

			ScriptManager.RegisterClientScriptBlock(page, typeof(DateTimeBoxDateCustomization), DateCustomizationFunctionName, DateCustomizationFunctionContent, true);

			return DateCustomizationFunctionName;
		}
		private static object _getDatesCustomizationFunctionLock = new object();
		#endregion

		#region RenderDateStatusHandlerContent (abstract)
		/// <summary>
		/// Template metoda pro vyrenderování html pro customizaci hodnot v kalendáři.
		/// </summary>
		public abstract string RenderDateStatusHandlerContent();
		#endregion
	}
}