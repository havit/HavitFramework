using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Web.UI.WebControls
{
    /// <summary>
    /// Tøída implementující customizaci DateTimeBox-u pro special date collection. 
    /// </summary>
    public class SpecialDateCustomization : DateTimeBoxDateCustomization
    {
        private List<SpecialDate> SpecialDates { get; set; }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public SpecialDateCustomization(List<SpecialDate> specialDates)
        {
            this.SpecialDates = specialDates;
        }

        /// <summary>
        /// Renderuje funkci pro customizaci hodnot v kalendáøi.
        /// </summary>
        public override string RenderDateStatusHandlerContent()
        {
            StringBuilder result = new StringBuilder();
            StringBuilder yearsStringBuilder = new StringBuilder();
            StringBuilder monthStringBuilder;
            StringBuilder dayStringBuilder;

            if (this.SpecialDates.Count > 0)
            {
                result.AppendLine("var specialDates = {");

                this.SpecialDates.GroupBy(item => item.Datum.Year).ToList().ForEach(yearGroup =>
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
                result.AppendLine("if (specialDates[y] && specialDates[y][m] && specialDates[y][m][d]) { return specialDates[y][m][d]; } else { return false; }");
            }
            else
            {
                result.Append("return false;");
            }

            return result.ToString();
        }
    }
}