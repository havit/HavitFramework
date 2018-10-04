using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Diagnostics.Contracts;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Helper class for rendering RadioButtonList and CheckBoxList Extensions.
	/// </summary>
	internal static class RadioButtonListCheckBoxListHelper
	{
		#region Render
		/// <summary>
		/// Renders control.
		/// </summary>
		internal static void Render(IRadioButtonListCheckBoxList control, HtmlTextWriter writer)
		{
			Contract.Requires(control.RepeatLayout == RepeatLayout.Flow, "The only supported layout is RepeatLayout.Flow.");
			Contract.Requires(control.RepeatColumns <= 1, "RepeatColumns is not supported.");

			// Vypíšeme obalující SPAN
			control.CssClass = (control.CssClass + " " + ((control.RepeatDirection == RepeatDirection.Horizontal) ? "btn-group" : "btn-group-vertical")).Trim();
			control.Attributes["data-toggle"] = "buttons"; // zajišťuje funkci inputů
			control.AddAttributesToRender(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.Span);

			// vyrenderujeme jednotlivé položky
			for (int i = 0; i < control.Items.Count; i++)
			{
				control.RenderItem(ListItemType.Item, i, null, writer);
			}
			// ukončíme obalující SPAN
			writer.RenderEndTag();
		}
		#endregion

		#region RenderItem
		/// <summary>
		/// Rendres checkboxlist/radiobuttonlist item.
		/// </summary>
		internal static void RenderItem(IRadioButtonListCheckBoxList control, ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer, Action baseRenderItem)
		{
			ListItem item = control.Items[repeatIndex]; // item to render

			string cssClass = control.ItemCssClass ?? String.Empty;
			if (!String.IsNullOrEmpty(item.Attributes["class"]))
			{
				cssClass += " " + item.Attributes["class"];
				item.Attributes.Remove("class");
			}
			if (item.Selected)
			{
				cssClass += " active"; // selected items must have active class (otherwise the selection is not visible in UI)
			}
			writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass.Trim());
			writer.RenderBeginTag(HtmlTextWriterTag.Label); // start rendering LABEL

			string text = item.Text;
			item.Text = String.Empty; // hide item value
			if (String.IsNullOrEmpty(item.Value)) // handle situations where item value is not set
			{
				item.Value = text;
			}

			baseRenderItem(); // render INPUT

			if (control.HtmlEncode)
			{
				writer.WriteEncodedText(text); // render text to LABEL
			}
			else
			{
				writer.Write(text);
			}

			item.Text = text; // restore item text (it is not necessary to restore item value)
			writer.RenderEndTag(); // end LABEL
		}
		#endregion

	}
}
