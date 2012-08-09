using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Třída implementující special date zobrazovaný v DateTimeBox-u.
	/// </summary>
	public class SpecialDate
	{
		public DateTime Datum { get; private set; }
		public bool Disabled { get; private set; }
		public string CssClass { get; private set; }

		/// <summary>
		/// SpecialDate (ctor)
		/// </summary>		
		public SpecialDate(DateTime datum, bool disabled, string cssClass)
		{
			Datum = datum;
			Disabled = disabled;
			CssClass = cssClass;
		}
	}

	/// <summary>
	/// Třída implementující customizaci DateTimeBox-u pro special date collection. 
	/// </summary>
	public class SpecialDateCustomization : DateTimeBoxDateCustomization
	{
		private List<SpecialDate> SpecialDates { get; set; }

		public SpecialDateCustomization(List<SpecialDate> specialDates)
		{
			SpecialDates = specialDates;
		}

		public override string RenderDateStatusHandlerContent()
		{
			StringBuilder result = new StringBuilder();
			StringBuilder yearsStringBuilder = new StringBuilder();
			StringBuilder monthStringBuilder;
			StringBuilder dayStringBuilder;

			if (SpecialDates.Count > 0)
			{
				result.AppendLine("var specialDates = {");

				SpecialDates.GroupBy(item => item.Datum.Year).ToList().ForEach(yearGroup =>
				{
					if (yearsStringBuilder.Length > 0)
					{
						yearsStringBuilder.Append(", ");
					}

					yearsStringBuilder.AppendLine(yearGroup.Key.ToString() + " : {");

					monthStringBuilder = new StringBuilder();

					yearGroup.GroupBy(m => m.Datum.Month).ToList().ForEach(monthGroup =>
					{

						if (monthStringBuilder.Length > 0)
						{
							monthStringBuilder.Append(", ");
						}

						monthStringBuilder.Append(monthGroup.Key.ToString() + " : {");
						dayStringBuilder = new StringBuilder();

						monthGroup.ToList().ForEach(day =>
						{

							if (dayStringBuilder.Length > 0)
							{
								dayStringBuilder.Append(", ");
							}

							dayStringBuilder.Append(day.Datum.Day + " : " + (day.Disabled ? "'disabled'" : "'" + day.CssClass + "'"));
						});

						monthStringBuilder.Append(dayStringBuilder.ToString());
						monthStringBuilder.Append("}");
					});

					yearsStringBuilder.Append(monthStringBuilder.ToString());
					yearsStringBuilder.Append("}");
				});

				result.Append(yearsStringBuilder.ToString());
				result.Append("};");
			}

			result.AppendLine("if (specialDates[y] && specialDates[y][m] && specialDates[y][m][d]) { return specialDates[y][m][d]; } else { return false; }");

			return result.ToString();			
		}
	}
}