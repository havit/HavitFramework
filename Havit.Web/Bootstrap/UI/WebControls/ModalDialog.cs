﻿using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web;
using Havit.Diagnostics.Contracts;
using Havit.Web.UI;
using Havit.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Modal dialog with support of Ajax (by nested UpdatePanel). Support header, content and footer sections.
/// </summary>
[Themeable(true)]
public partial class ModalDialog : ModalDialogBase, IPostBackEventHandler
{
	private readonly Control dialogContainer;
	private readonly UpdatePanel updatePanel;
	private readonly Control contentContainer;
	private H4 headerH4;
	private readonly Button closeButton;

	/// <summary>
	/// Dialog header template.
	/// </summary>
	[TemplateInstance(TemplateInstance.Single)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public virtual ITemplate HeaderTemplate
	{
		get
		{
			return headerTemplate;
		}
		set
		{
			headerTemplate = value;
		}
	}
	private ITemplate headerTemplate;

	internal Control HeaderTemplateContainer
	{
		get
		{
			return headerContainer;
		}
	}
	private readonly Control headerContainer;

	/// <summary>
	/// Dialog footer template.
	/// </summary>
	[TemplateInstance(TemplateInstance.Single)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public virtual ITemplate FooterTemplate
	{
		get
		{
			return footerTemplate;
		}
		set
		{
			footerTemplate = value;
		}
	}
	private ITemplate footerTemplate;

	internal Control FooterTemplateContainer
	{
		get
		{
			return footerContainer;
		}
	}
	private readonly Control footerContainer;

	/// <summary>
	/// Dialog header text. Used only when HeaderTemplate not used.
	/// HeaderText is rendered in &lt;h4&gt; element with css class "modal-title".
	/// </summary>
	public string HeaderText
	{
		get
		{
			return (string)(ViewState["HeaderText"] ?? String.Empty);
		}
		set
		{
			ViewState["HeaderText"] = value;
		}
	}

	/// <summary>
	/// Nested UpdatePanel's triggers.
	/// </summary>
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public UpdatePanelTriggerCollection Triggers
	{
		get
		{
			return updatePanel.Triggers;
		}
	}

	/// <summary>
	/// Šířka dialogu v pixelech.
	/// </summary>
	public Unit Width
	{
		get
		{
			return (Unit)(ViewState["Width"] ?? Unit.Empty);
		}
		set
		{
			ViewState["Width"] = value;
		}
	}

	/// <summary>
	/// Get or sets whether use animations when showing and hiding dialog.
	/// Animation are not supported on nested modals.
	/// </summary>
	[DefaultValue(true)]
	public bool UseAnimations
	{
		get
		{
			return (bool)(ViewState["UseAnimations"] ?? true);
		}
		set
		{
			ViewState["UseAnimations"] = value;
		}
	}

	/// <summary>
	/// Css class to be used for dialog - used in element with modal-dialog class.
	/// </summary>
	//[DefaultValue("")]
	public string CssClass
	{
		get
		{
			return (string)(ViewState["CssClass"] ?? String.Empty);
		}
		set
		{
			ViewState["CssClass"] = value;
		}
	}

	/// <summary>
	/// Css class to be used for header - used in element with modal-header class.
	/// </summary>
	//[DefaultValue("")]
	public string HeaderCssClass
	{
		get
		{
			return (string)(ViewState["HeaderCssClass"] ?? String.Empty);
		}
		set
		{
			ViewState["HeaderCssClass"] = value;
		}
	}

	/// <summary>
	/// Css class to be used for content - used in element with modal-body class.
	/// </summary>
	//[DefaultValue("")]
	public string ContentCssClass
	{
		get
		{
			return (string)(ViewState["ContentCssClass"] ?? String.Empty);
		}
		set
		{
			ViewState["ContentCssClass"] = value;
		}
	}

	/// <summary>
	/// Css class to be used for footer - used in element with modal-footer class.
	/// </summary>
	//[DefaultValue("")]
	public string FooterCssClass
	{
		get
		{
			return (string)(ViewState["FooterCssClass"] ?? String.Empty);
		}
		set
		{
			ViewState["FooterCssClass"] = value;
		}
	}

	/// <summary>
	/// Indicates whether to show close button.
	/// Default value is true.
	/// </summary>
	//[DefaultValue(true)]
	public bool ShowCloseButton
	{
		get
		{
			return (bool)(ViewState["ShowCloseButton"] ?? true);
		}
		set
		{
			ViewState["ShowCloseButton"] = value;
		}
	}

	/// <summary>
	/// Indicates whether to close dialog on escape key pressed.
	/// </summary>
	//[DefaultValue(true)]
	public bool CloseOnEscapeKey
	{
		get
		{
			return (bool)(ViewState["CloseOnEscapeKey"] ?? true);
		}
		set
		{
			ViewState["CloseOnEscapeKey"] = value;
		}
	}

	/// <summary>
	/// Modal dialog drag mode.
	/// Default value is ModalDialogDragMode.IfAvailable.
	/// </summary>
	[DefaultValue(ModalDialogDragMode.IfAvailable)]
	public ModalDialogDragMode DragMode
	{
		get
		{
			return (ModalDialogDragMode)(ViewState["DragMode"] ?? ModalDialogDragMode.IfAvailable);
		}
		set
		{
			ViewState["DragMode"] = value;
		}
	}

	internal bool CurrentyShowing { get; set; }

	/// <summary>
	/// Constructor.
	/// </summary>
	public ModalDialog()
	{
		dialogContainer = new DialogControl(this);

		updatePanel = new UpdatePanelExt();
		updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

		headerContainer = new DialogSectionControl(() => ("modal-header " + HeaderCssClass).Trim());
		contentContainer = new DialogSectionControl(() => ("modal-body " + ContentCssClass).Trim());
		footerContainer = new DialogSectionControl(() => ("modal-footer " + FooterCssClass).Trim());

		closeButton = new Button();
		closeButton.CssClass = "close";
		closeButton.Controls.Add(new LiteralControl("&times;"));
		closeButton.Attributes.Add("aria-hidden", "true");
		closeButton.CausesValidation = false;
		closeButton.Click += CloseButton_Click;
	}

	/// <summary>
	/// Creates nested controls.
	/// </summary>
	protected override void CreateChildControls()
	{
		base.CreateChildControls();

		// contentTemplate in base class
		bool headerContainerHasControls = headerContainer.HasControls();

		headerContainer.Controls.Add(closeButton);

		if (!headerContainerHasControls)
		{
			if (headerTemplate != null)
			{
				headerTemplate.InstantiateIn(headerContainer);
			}
			else
			{
				headerH4 = new H4() { CssClass = "modal-title" };
				headerContainer.Controls.Add(headerH4);
			}
		}

		if (!footerContainer.HasControls())
		{
			if (footerTemplate != null)
			{
				footerTemplate.InstantiateIn(footerContainer);
			}
		}

		dialogContainer.ID = this.ID + "__DC";
		updatePanel.ID = this.ID + "__UP";
		contentContainer.ID = this.ID + "__CC";
		headerContainer.ID = this.ID + "__HC";
		footerContainer.ID = this.ID + "__FC";
		closeButton.ID = this.ID + "__CB";

		updatePanel.ContentTemplateContainer.Controls.Add(headerContainer);
		updatePanel.ContentTemplateContainer.Controls.Add(contentContainer);
		updatePanel.ContentTemplateContainer.Controls.Add(footerContainer);

		dialogContainer.Controls.Add(updatePanel);
	}

	/// <summary>
	/// Returns control to which to instantiate content template.
	/// </summary>
	protected override Control GetContentContainer()
	{
		return contentContainer;
	}

	/// <summary>
	/// Returns control which handles dialog behavior and operations.
	/// </summary>
	protected override Control GetDialogContainer()
	{
		return dialogContainer;
	}

	/// <summary>
	/// Handles show dialog event.
	/// And updates nested UpdatePanel.
	/// </summary>
	protected override void OnDialogShown(EventArgs eventArgs)
	{
		CurrentyShowing = true;
		base.OnDialogShown(eventArgs);
		updatePanel.Update();
	}

	/// <summary>
	/// Handles hide dialog event.
	/// And updates nested UpdatePanel.
	/// </summary>
	protected override void OnDialogHidden(EventArgs eventArgs)
	{
		base.OnDialogHidden(eventArgs);
		updatePanel.Update();
	}

	/// <summary>
	/// PreRender.
	/// </summary>
	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
		Contract.Assert<HttpException>(scriptManager != null, "ScriptManager must be registered within the form.");

		ClientScripts.BootstrapClientScriptHelper.RegisterBootstrapClientScript(this.Page);
		ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, ClientScripts.BootstrapClientScriptHelper.ModalScriptResourceMappingName);

		// if we are not able to hide whole dialogContainer, lets hide at least update panel content
		updatePanel.ContentTemplateContainer.Visible = DialogVisible;

		closeButton.Visible = ShowCloseButton;

		//if (DialogVisible)
		//{
		//	ScriptManager.RegisterStartupScript(this.Page, typeof(ModalDialog), "StartUp", "$(document).ready(function() { Havit_BootstrapExtensions_ResizeModal(); });", true);
		//}

		if (headerH4 != null)
		{
			headerH4.Text = HeaderText;
		}
	}

	/// <summary>
	/// Ensures modal dialog is hidden.
	/// </summary>
	protected override void RegisterHideScriptFromPreRenderComplete()
	{
		RegisterHideScript();
	}

	/// <summary>
	/// Vrátí skript pro zobrazení dialogu na klientské straně.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override string GetShowScript()
	{
		string scriptPattern = CurrentyShowing
			? "Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.getInstance('#{0}').show({1}, '{2}', '{3}', '{4}');"
			: "Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.getInstance('#{0}').remainShown({1}, '{2}', '{3}', '{4}');";

		string postbackScript = String.Empty;
		if (CloseOnEscapeKey)
		{
			ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
			scriptManager.RegisterAsyncPostBackControl(this);

			postbackScript = this.Page.ClientScript.GetPostBackEventReference(new PostBackOptions(this, "Escape", null, false, false, false, true, false, null), false);
		}

		string script = String.Format(
			scriptPattern,
			GetDialogContainer().ClientID, // 0
			CloseOnEscapeKey.ToString().ToLower(), // 1
			postbackScript.Replace("'", "\\'"), // 2
			DragMode.ToString(), // 3
			Width.ToString()); // 4
		return script;
	}

	/// <summary>
	/// Vrátí skript pro skrytí dialogu na klientské straně.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override string GetHideScript()
	{
		string scriptPattern = "Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.getInstance('#{0}').hide();";
		string script = String.Format(scriptPattern, DialogPanelClientIDMemento ?? dialogContainer.ClientID);
		return script;
	}

	private void CloseButton_Click(object sender, EventArgs e)
	{
		Hide();
	}

	void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
	{
		if (eventArgument == "Escape")
		{
			Hide();
		}
	}
}