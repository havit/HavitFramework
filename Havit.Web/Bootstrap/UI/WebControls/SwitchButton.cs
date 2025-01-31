﻿using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Control  with touchable selection of Yes/No value.
/// By default, the "No" choice is selected (Value is false).
/// </summary>
[Themeable(true)]
public class SwitchButton : Control, INamingContainer
{
	private readonly RadioButtonList radioButtonList;
	private readonly ListItem yesItem;
	private readonly ListItem noItem;

	/// <summary>
	/// Returns true if "yes" choice is selected.
	/// Default value is false.		
	/// </summary>
	[DefaultValue(false)]
	public virtual bool Value
	{
		get
		{
			return yesItem.Selected;
		}
		set
		{
			yesItem.Selected = value;
			noItem.Selected = !value;
		}
	}

	/// <summary>
	/// Occurs when the value of the Value property changes between posts to the server.
	/// </summary>
	public event EventHandler ValueChanged;

	/// <summary>
	/// Occurs when the value of the Value property changes between posts to the server.
	/// </summary>
	protected virtual void OnValueChanged(EventArgs eventArgs)
	{
		if (ValueChanged != null)
		{
			ValueChanged(this, eventArgs);
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether an automatic postback to the server will occur whenever the user changes the selection of the list.
	/// </summary>
	[DefaultValue(false)]
	public virtual bool AutoPostBack
	{
		get { return radioButtonList.AutoPostBack; }
		set { radioButtonList.AutoPostBack = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether validation is performed when a control that is derived from the <see cref="T:System.Web.UI.WebControls.ListControl"/> class is clicked.
	/// </summary>
	public bool CausesValidation
	{
		get { return radioButtonList.CausesValidation; }
		set { radioButtonList.CausesValidation = value; }
	}

	/// <summary>
	/// Gets or sets the group of controls for which the control that is derived from the <see cref="T:System.Web.UI.WebControls.ListControl"/> class causes validation when it posts back to the server.
	/// </summary>
	public string ValidationGroup
	{
		get { return radioButtonList.ValidationGroup; }
		set { radioButtonList.ValidationGroup = value; }
	}

	/// <summary>
	/// Text for "Yes" choice.
	/// Supports localization pattern.
	/// </summary>
	[DefaultValue("Yes")]
	public string YesText
	{
		get { return yesItem.Text; }
		set { yesItem.Text = value; }
	}

	/// <summary>
	/// Text for "No" choice.
	/// Supports localization pattern.
	/// </summary>
	[DefaultValue("No")]
	public string NoText
	{
		get { return noItem.Text; }
		set { noItem.Text = value; }
	}

	/// <summary>
	/// CssClass for yes item.
	/// Default value is "yes".
	/// </summary>
	[DefaultValue("yes")]
	public string YesCssClass
	{
		get
		{
			return (string)(ViewState["YesCssClass"] ?? "yes");
		}
		set
		{
			ViewState["YesCssClass"] = value;
		}
	}

	/// <summary>
	/// CssClass for no item.
	/// Default value is "no".
	/// </summary>
	[DefaultValue("no")]
	public string NoCssClass
	{
		get
		{
			return (string)(ViewState["NoCssClass"] ?? "no");
		}
		set
		{
			ViewState["NoCssClass"] = value;
		}
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public SwitchButton()
	{
		yesItem = new ListItem("$resources: Glossary, Yes, Yes", "1") { Selected = false };
		noItem = new ListItem("$resources: Glossary, No, No", "0") { Selected = true };

		if (!String.IsNullOrEmpty(YesCssClass))
		{
			yesItem.Attributes["class"] = YesCssClass;
		}

		if (!String.IsNullOrEmpty(NoCssClass))
		{
			noItem.Attributes["class"] = NoCssClass;
		}

		radioButtonList = new RadioButtonList();
		radioButtonList.Items.Add(yesItem);
		radioButtonList.Items.Add(noItem);
		radioButtonList.SelectedIndexChanged += RadioButtonList_SelectedIndexChanged;
	}

	private void RadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
	{
		OnValueChanged(e);
	}

	/// <summary>
	/// Raises the Init event. This notifies the control to perform any steps necessary for its creation on a page request.
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		EnsureChildControls();
	}

	/// <summary>
	/// Notifies any controls that use composition-based implementation to create any
	/// child controls they contain in preperation for postback or rendering.
	/// </summary>
	protected override void CreateChildControls()
	{
		base.CreateChildControls();
		Controls.Add(radioButtonList);
	}

	/// <summary>
	/// Outputs control content to a provided HTMLTextWriter output stream.
	/// </summary>
	protected override void Render(HtmlTextWriter writer)
	{
		yesItem.Text = HttpUtilityExt.GetResourceString(YesText);
		noItem.Text = HttpUtilityExt.GetResourceString(NoText);

		base.Render(writer);
	}
}
