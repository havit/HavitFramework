﻿using System.Collections;
using System.Text;

namespace Havit.Web.UI.WebControls;

/// <summary>Used for writing out JSON data code.</summary>
/// <remarks>This class inherits from System.Web.UI.HtmlTextWriter class which contains many methods for writing out Html.</remarks>
internal class XJsonWriter : StringWriter
{
	public void WriteNameValue(string name, object value)
	{
		WriteNameValue(name, value, true);
	}

	public void WriteNameValue(string name, object value, bool formatValue)
	{
		string valueText = GetValueText(value, formatValue);

		//Add delimiter
		if (this.GetStringBuilder().Length > 0)
		{
			this.WriteLine(", ");
		}

		//Prefix value with new line if it starts with {
		if (valueText[0] == '{')
		{
			valueText = this.NewLine + valueText;
		}

		this.Write("\"" + name + "\": " + valueText);
	}

	public void WriteList(string name, IList list)
	{
		WriteList(name, list, true);
	}

	public void WriteList(string name, IList list, bool formatValues)
	{
		string valueText;
		StringBuilder sb = new StringBuilder();

		foreach (object item in list)
		{
			valueText = GetValueText(item, formatValues);

			if (sb.Length != 0)
			{
				sb.AppendLine(", ");
			}

			sb.Append(valueText);
		}

		string listText = this.NewLine + "[" + this.NewLine + sb.ToString() + this.NewLine + "]";
		WriteNameValue(name, listText, false);
	}

	private string GetValueText(object value, bool formatValue)
	{
		string valueText;
		if (value == null)
		{
			valueText = "null";
			return valueText;
		}

		if (!formatValue)
		{
			valueText = value.ToString();
			return valueText;
		}

		if (value is string)
		{
			valueText = (string)value;

			//Escape double-quotes for strings and enclose in quotes
			// RH: Doplněno escapování zpětného lomítka 
			valueText = "\"" + valueText.Replace(@"\", @"\\").Replace("\"", "\\\"") + "\"";
		}
		else
		{
			//Need toLower for bool. In javascript - true & false.
			valueText = value.ToString().ToLower();
		}

		return valueText;
	}

	public override string ToString()
	{
		string result = base.ToString();
		result = "{" + this.NewLine + result + this.NewLine + "}";
		return result;
	}
}
