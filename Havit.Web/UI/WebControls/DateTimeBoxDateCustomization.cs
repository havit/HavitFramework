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

		#region DateCustomizationFunctionContent
		private string DateCustomizationFunctionContent
		{
			get
			{
				if (String.IsNullOrEmpty(_dateCustomizationFunctionContent))
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendLine("function " + DateCustomizationFunctionName + "(date, y, m, d) {");
					sb.AppendLine(RenderDateStatusHandlerContent());
					sb.AppendLine("}");

					_dateCustomizationFunctionContent = sb.ToString();
				}

				return _dateCustomizationFunctionContent;
			}
		}
		private string _dateCustomizationFunctionContent;
		#endregion

		/// <summary>
		/// Vygeneruje funkci se special dates.
		/// </summary>		
		public string GetDatesCustomizationFunction(Page page)
		{
			if (String.IsNullOrEmpty(DateCustomizationFunctionName))
			{
				Guid functionNameGuid = Guid.NewGuid();
				string functionName = "_" + functionNameGuid.ToString().Replace("-", "");
				DateCustomizationFunctionName = functionName;								
			}			

			// to avoid rare conditions
			string dateCustomizationFunctionNameTemp = DateCustomizationFunctionName;

			ScriptManager.RegisterClientScriptBlock(page, typeof(DateTimeBoxDateCustomization), dateCustomizationFunctionNameTemp, DateCustomizationFunctionContent, true);

			return DateCustomizationFunctionName; 
		}

		
		public abstract string RenderDateStatusHandlerContent();
	}
}