using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls;

internal static class ListControlExtensions
{
	internal static void RenderContents(HtmlTextWriter writer, ListControl listControl, Page page, Action verifyMultiSelect)
	{
		string lastOptionGroup = String.Empty;

		if (listControl.Items.Count > 0)
		{
			bool selectedValueRendered = false;

			foreach (ListItem item in listControl.Items)
			{
				if (item.Enabled)
				{
					page.ClientScript.RegisterForEventValidation(listControl.UniqueID, item.Value);

					string optionGroup = item.GetOptionGroup() ?? String.Empty;

					if (lastOptionGroup != optionGroup)
					{
						// pokud jsme měli otevřenou option group, tak ji zavřeme
						if (!String.IsNullOrEmpty(lastOptionGroup))
						{
							RenderContents_OptionGroupEndTag(writer);
						}

						// máme otevřít option group, tak ji otevřeme
						if (!String.IsNullOrEmpty(optionGroup))
						{
							RenderContents_OptionGroupBeginTag(optionGroup, writer);
						}

						lastOptionGroup = optionGroup;
					}

					RenderContents_ListItem(item, writer, ref selectedValueRendered, verifyMultiSelect);
				}
			}
		}

		// zůstala nám otevřená option group? zavřeme ji
		if (!String.IsNullOrEmpty(lastOptionGroup))
		{
			RenderContents_OptionGroupEndTag(writer);
		}
	}

	private static void RenderContents_OptionGroupBeginTag(string name, HtmlTextWriter writer)
	{
		writer.WriteBeginTag("optgroup");
		writer.WriteAttribute("label", name);
		writer.Write(HtmlTextWriter.TagRightChar);
		writer.WriteLine();
	}

	private static void RenderContents_OptionGroupEndTag(HtmlTextWriter writer)
	{
		writer.WriteEndTag("optgroup");
		writer.WriteLine();
	}

	private static void RenderContents_ListItem(ListItem item, HtmlTextWriter writer, ref bool selectedValueRendered, Action verifyMultiSelect)
	{
		writer.WriteBeginTag("option");
		writer.WriteAttribute("value", item.Value, true);
		if (item.Selected)
		{
			writer.WriteAttribute("selected", "selected", false);

			if (selectedValueRendered)
			{
				verifyMultiSelect();
			}
			selectedValueRendered = true;
		}

		foreach (string key in item.Attributes.Keys)
		{
			if (!String.Equals(ListItemExtensions.OptionGroupAttributeName, key, StringComparison.InvariantCultureIgnoreCase))
			{
				writer.WriteAttribute(key, item.Attributes[key]);
			}
		}

		writer.Write(HtmlTextWriter.TagRightChar);
		HttpUtility.HtmlEncode(item.Text, writer);
		writer.WriteEndTag("option");
		writer.WriteLine();
	}

	/// <summary>
	/// Vytvoří ListItem, hodnoty získá reflexí na základě zadaných fieldů.
	/// </summary>
	internal static ListItem CreateListItem(object dataItem, string dataTextField, string dataTextFormatString, string dataValueField, string dataOptionGroupField)
	{
		ListItem item = new ListItem();

		if ((dataTextField.Length != 0) || (dataValueField.Length != 0))
		{
			if (dataTextField.Length > 0)
			{
				item.Text = DataBinderExt.GetValue(dataItem, dataTextField, dataTextFormatString);
			}
			if (dataValueField.Length > 0)
			{
				item.Value = DataBinderExt.GetValue(dataItem, dataValueField, null);
			}
		}
		else
		{
			if (dataTextFormatString.Length != 0)
			{
				item.Text = string.Format(CultureInfo.CurrentCulture, dataTextFormatString, new object[] { dataItem });
			}
			else
			{
				item.Text = dataItem.ToString();
			}
			item.Value = dataItem.ToString();
		}

		if (dataOptionGroupField.Length != 0)
		{
			item.SetOptionGroup(DataBinderExt.GetValue(dataItem, dataOptionGroupField, null));
		}

		return item;
	}

	internal static object SaveViewState(Func<object> baseSaveViewState, ListItemCollection items)
	{
		string[] optionGroups = items.AsEnumerable().Select(item => item.GetOptionGroup()).ToArray();
		if (!optionGroups.Any(item => (item != null) && item.Length > 0))
		{
			optionGroups = null;
		}

		return new object[]
		{
			baseSaveViewState(),
			optionGroups
		};
	}

	public static void LoadViewState(object savedState, Action<object> baseLoadViewState, Func<ListItemCollection> itemsFunc)
	{
		object[] viewState = (object[])savedState;
		baseLoadViewState(viewState[0]);
		if (viewState[1] != null)
		{
			ListItemCollection items = itemsFunc();
			string[] optionGroups = (string[])viewState[1];
			for (int i = 0; i < optionGroups.Length; i++)
			{
				items[i].SetOptionGroup(optionGroups[i]);
			}
		}
	}
}
