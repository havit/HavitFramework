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
		private string DateCustomizationFunctionName
		{
			get;
			set;
		} 
		#endregion

		/// <summary>
		/// Vygeneruje funkci se special dates
		/// </summary>
		/// <returns></returns>
		public string GetDatesCustomizationFunction(Page page)
		{
			if (String.IsNullOrEmpty(DateCustomizationFunctionName))
			{
				StringBuilder sb = new StringBuilder();

				Guid functionNameGuid = Guid.NewGuid();
				string functionName = "_" + functionNameGuid.ToString().Replace("-", "");
				DateCustomizationFunctionName = functionName;
				sb.AppendLine("function " + functionName + "(date, y, m, d) {");
				sb.AppendLine(RenderDateStatusHandlerContent());
				sb.AppendLine("}");
				ScriptManager.RegisterClientScriptBlock(page, typeof(DateTimeBoxDateCustomization), DateCustomizationFunctionName, sb.ToString(), true);
			}

			return DateCustomizationFunctionName; 
		}
		public abstract string RenderDateStatusHandlerContent();
	}
}