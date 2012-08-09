using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI
{
	/// <summary>
	/// Rozšiøující funkènost analogická k <see cref="System.Web.UI.DataBinder"/>.
	/// </summary>
	/// <remarks>
	/// Používá se napø. pro resolvování BoundFieldExt.DataField, DropDownListExt.DataTextField, ...
	/// </remarks>
	public static class DataBinderExt
	{
		/// <summary>
		/// Získá hodnotu pro zobrazení z pøedaného objektu a dataField.
		/// </summary>
		/// <param name="dataItem">Položka dat z DataSource</param>
		/// <param name="dataField">DataField</param>
		/// <returns>hodnota, pokud se ji podaøilo získat; jinak <c>null</c> nebo DBNull.Value</returns>
		public static object GetValue(object dataItem, string dataField)
		{
			string[] expressionParts = dataField.Split('.');

			object currentValue = dataItem;

			int i = 0;
			int lastExpressionIndex = expressionParts.Length - 1;
			for (i = 0; i <= lastExpressionIndex; i++)
			{
				string expression = expressionParts[i];

				if (expression.IndexOfAny(indexExprStartChars) < 0)
				{
					currentValue = DataBinder.GetPropertyValue(currentValue, expression);
				}
				else
				{
					currentValue = DataBinder.GetIndexedPropertyValue(currentValue, expression);
				}

				if (currentValue == null) // && (i < lastExpressionIndex))
				{
					return null;
				}

				if (currentValue == DBNull.Value) // && (i < lastExpressionIndex))
				{
					return DBNull.Value;
				}
			}

			return currentValue;
		}
		private static readonly char[] indexExprStartChars = new char[] { '[', '(' };

		/// <summary>
		/// Získá hodnotu pro zobrazení z pøedaného objektu a dataField a zformtátuje ji.
		/// </summary>
		/// <param name="dataItem">Položka dat z DataSource</param>
		/// <param name="dataField">DataField</param>
		/// <param name="format">formátovací øetìzec</param>
		/// <returns>hodnota, pokud se ji podaøilo získat a zformátovat; jinak <c>String.Empty</c></returns>
		public static string GetValue(object dataItem, string dataField, string format)
		{
			object propertyValue = GetValue(dataItem, dataField);
			if ((propertyValue == null) || (propertyValue == DBNull.Value))
			{
				return string.Empty;
			}
			if (string.IsNullOrEmpty(format))
			{
				return propertyValue.ToString();
			}
			return string.Format(format, propertyValue);
		}
	}
}
