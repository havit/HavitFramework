using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.Bootstrap.UI.ClientScripts;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Display CheckBoxList as buttons for better Touch UI experience.
/// </summary>
public class CheckBoxList : System.Web.UI.WebControls.CheckBoxList, IRadioButtonListCheckBoxList
{
	/// <summary>
	/// Css class for list items.
	/// Default value is "btn btn-default".
	/// </summary>
	[DefaultValue("btn btn-default")]
	public string ItemCssClass
	{
		get
		{
			return (string)(ViewState["ItemCssClass"] ?? "btn btn-default");
		}
		set
		{
			ViewState["ItemCssClass"] = value;
		}
	}

	/// <summary>
	/// Indikuje, zda dochází k encode textu k zobrazení.
	/// Výchozí hodnota je true.
	/// </summary>
	public bool HtmlEncode
	{
		get
		{
			return (bool)(ViewState["HtmlEncode"] ?? true);
		}
		set
		{
			ViewState["HtmlEncode"] = value;
		}
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public CheckBoxList()
	{
		this.RepeatLayout = RepeatLayout.Flow;
		this.RepeatDirection = RepeatDirection.Horizontal;
	}

	/// <summary>
	/// Configures the CheckBoxList prior to rendering on the client.
	/// </summary>
	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		BootstrapClientScriptHelper.RegisterBootstrapClientScript(this.Page);
	}

	/// <summary>
	/// Displays the CheckBoxList on the client.
	/// </summary>
	protected override void Render(HtmlTextWriter writer)
	{
		// No base call
		RadioButtonListCheckBoxListHelper.Render(this, writer);
	}

	/// <summary>
	/// /// Called to render each item.
	/// </summary>
	protected override void RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo,
		HtmlTextWriter writer)
	{
		RadioButtonListCheckBoxListHelper.RenderItem(this, itemType, repeatIndex, repeatInfo, writer,
			() => base.RenderItem(itemType, repeatIndex, repeatInfo, writer));
	}

	void IRadioButtonListCheckBoxList.AddAttributesToRender(HtmlTextWriter writer)
	{
		this.AddAttributesToRender(writer);
	}

	void IRadioButtonListCheckBoxList.RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo,
		HtmlTextWriter writer)
	{
		this.RenderItem(itemType, repeatIndex, repeatInfo, writer);
	}
}
