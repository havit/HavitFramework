using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using Havit.Diagnostics.Contracts;
using Havit.Web.UI;
using Havit.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Modal dialog with support of Ajax (by nested UpdatePanel). Support header, content and footer sections.
	/// </summary>
	public partial class ModalDialog : ModalDialogBase, IPostBackEventHandler
	{
		#region Private fiels
		private Control _dialogContainer;
		private UpdatePanel _updatePanel;
		private Control _contentContainer;
		private Control _headerContainer;
		private Control _footerContainer;
		private Button _closeButton;
		#endregion

		#region HeaderTemplate
		/// <summary>
		/// Dialog header template.
		/// </summary>
		[TemplateInstance(TemplateInstance.Single)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ITemplate HeaderTemplate
		{
			get
			{
				return _headerTemplate;
			}
			set
			{
				_headerTemplate = value;				
			}
		}
		private ITemplate _headerTemplate;
		#endregion		

		#region FooterTemplate
		/// <summary>
		/// Dialog footer template.
		/// </summary>
		[TemplateInstance(TemplateInstance.Single)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ITemplate FooterTemplate
		{
			get
			{
				return _footerTemplate;
			}
			set
			{
				_footerTemplate = value;
			}
		}
		private ITemplate _footerTemplate;
		#endregion		

		#region HeaderText
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
		#endregion

		#region Triggers
		/// <summary>
		/// Nested UpdatePanel's triggers.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public UpdatePanelTriggerCollection Triggers
		{
			get
			{
				return _updatePanel.Triggers;
			}
		}
		#endregion

		#region Width
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
		#endregion

		#region UseAnimations
		/// <summary>
		/// Get or sets whether use animations when showing and hiding dialog.
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
		#endregion

		#region CssClass
		/// <summary>
		/// Css class to be used for dialog - used in element with modal-dialog class.
		/// </summary>
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
		#endregion

		#region HeaderCssClass
		/// <summary>
		/// Css class to be used for header - used in element with modal-header class.
		/// </summary>
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
		#endregion

		#region ContentCssClass
		/// <summary>
		/// Css class to be used for content - used in element with modal-body class.
		/// </summary>
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
		#endregion

		#region FooterCssClass
		/// <summary>
		/// Css class to be used for footer - used in element with modal-footer class.
		/// </summary>
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
		#endregion

		#region ShowCloseButton
		/// <summary>
		/// Indicates whether to show close button.
		/// Default value is true.
		/// </summary>
		[DefaultValue(true)]
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
		#endregion

		#region CloseOnEscapeKey
		/// <summary>
		/// Indicates whether to close dialog on escape key pressed.
		/// </summary>
		[DefaultValue(true)]
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
		#endregion

		#region DragMode
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
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public ModalDialog()
		{
			_dialogContainer = new DialogControl(this);
			_dialogContainer.Visible = false;

			_updatePanel = new UpdatePanel();
			_updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

			_headerContainer = new DialogSectionControl(() => ("modal-header " + HeaderCssClass).Trim());
			_contentContainer = new DialogSectionControl(() => ("modal-body " + ContentCssClass).Trim());
			_footerContainer = new DialogSectionControl(() => ("modal-footer " + FooterCssClass).Trim());
		
			_closeButton = new Button();
			_closeButton.CssClass = "close";
			_closeButton.Controls.Add(new LiteralControl("&times;"));
			_closeButton.Attributes.Add("aria-hidden", "true");
			_closeButton.CausesValidation = false;
			_closeButton.Click += CloseButton_Click;
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Creates nested controls.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// contentTemplate in base class

			_headerContainer.Controls.Add(_closeButton);

			if (this._headerTemplate != null)
			{
				_headerTemplate.InstantiateIn(_headerContainer);
			}
			else if (!String.IsNullOrEmpty(HeaderText))
			{
				_headerContainer.Controls.Add(new H4() { CssClass = "modal-title", Text = HeaderText });
			}

			if (this._footerTemplate != null)
			{
				_footerTemplate.InstantiateIn(_footerContainer);
			}

			_dialogContainer.ID = this.ID + "__DC";
			_updatePanel.ID = this.ID + "__UP";
			_contentContainer.ID = this.ID + "__CC";
			_headerContainer.ID = this.ID + "__HC";
			_footerContainer.ID = this.ID + "__FC";
			_closeButton.ID = this.ID + "__CB";

			_updatePanel.ContentTemplateContainer.Controls.Add(_headerContainer);
			_updatePanel.ContentTemplateContainer.Controls.Add(_contentContainer);
			_updatePanel.ContentTemplateContainer.Controls.Add(_footerContainer);			

			_dialogContainer.Controls.Add(_updatePanel);
		}
		#endregion

		#region GetContentContainer
		/// <summary>
		/// Returns control to which to instantiate content template.
		/// </summary>
		protected override Control GetContentContainer()
		{
			return _contentContainer;
		}
		#endregion

		#region GetDialogContainer
		/// <summary>
		/// Returns control which handles dialog behavior and operations.
		/// </summary>
		protected override Control GetDialogContainer()
		{
			return _dialogContainer;
		}
		#endregion

		#region OnDialogShown
		/// <summary>
		/// Handles show dialog event.
		/// And updates nested UpdatePanel.
		/// </summary>
		protected override void OnDialogShown(EventArgs eventArgs)
		{
			base.OnDialogShown(eventArgs);
			_updatePanel.Update();
		}
		#endregion

		#region OnDialogHidden
		/// <summary>
		/// Handles hide dialog event.
		/// And updates nested UpdatePanel.
		/// </summary>
		protected override void OnDialogHidden(EventArgs eventArgs)
		{
			base.OnDialogHidden(eventArgs);
			_updatePanel.Update();
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// PreRender.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			
			// until first display of ModalDialog, dialogContainer is not visible
			// then we do not hide whole dialogContainer because it is not in update panel,
			// therefore it remains in client page event if not visible and this causes EventValidation error.
			ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
			Contract.Assert<HttpException>(scriptManager != null, "ScriptManager must be registered within the form.");

			if (!scriptManager.IsInAsyncPostBack)
			{
				_dialogContainer.Visible = DialogVisible;
			}
			else
			{
				if (DialogVisible)
				{
					_dialogContainer.Visible = true;
				}
			}

			ClientScripts.BootstrapClientScriptHelper.RegisterBootstrapClientScript(this.Page);
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, ClientScripts.BootstrapClientScriptHelper.ModalScriptResourceMappingName);

			if (DragMode == ModalDialogDragMode.Required)
			{
				ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery.ui.combined");
			}

			if (DragMode == ModalDialogDragMode.IfAvailable)
			{
				ScriptManager.ScriptResourceMapping.TryEnsureScriptRegistration(this.Page, "jquery.ui.combined");
			}

			// if we are not able to hide whole dialogContainer, lets hide at least update panel content
			_updatePanel.ContentTemplateContainer.Visible = DialogVisible;

			_closeButton.Visible = ShowCloseButton;

			if (DialogVisible)
			{
				ScriptManager.RegisterStartupScript(this.Page, typeof(ModalDialog), "StartUp", "$(document).ready(function() { Havit_BootstrapExtensions_ResizeModal(); });", true);
			}
		}
		#endregion

		#region GetShowScript, GetHideScript
		/// <summary>
		/// Vrátí skript pro zobrazení dialogu na klientské straně.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override string GetShowScript()
		{
			string postbackScript = null;
			if (CloseOnEscapeKey)
			{
				ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
				scriptManager.RegisterAsyncPostBackControl(this);

				postbackScript = this.Page.ClientScript.GetPostBackEventReference(new PostBackOptions(this, "Escape", null, false, false, false, true, false, null), false);
			}

			string scriptPattern = "$('#{0}').modal('show');";
			if (CloseOnEscapeKey)
			{
				scriptPattern += "$('body').on('keyup.havit.web.bootstrap', function(e) {{ if (e.which == 27) {{ {1}; }} }});";
			}

			if (DragMode == ModalDialogDragMode.Required)
			{
				scriptPattern += "$('#{0} .modal-dialog').draggable({{handle: '.modal-header'}});";
			}

			if (DragMode == ModalDialogDragMode.IfAvailable)
			{
				scriptPattern += "if (!!window.jQuery.ui && !!window.jQuery.ui.version) {{ $('#{0} .modal-dialog').draggable({{handle: '.modal-header'}}); }}";
			}

			string script = String.Format(				
				scriptPattern,
				GetDialogContainer().ClientID, // 0
				postbackScript); // 1
			return script;
		}

		/// <summary>
		/// Vrátí skript pro skrytí dialogu na klientské straně.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override string GetHideScript()
		{
			string scriptPattern = "";
			if (CloseOnEscapeKey)
			{
				scriptPattern += "$('body').off('keyup.havit.web.bootstrap');";
			}
			scriptPattern += "$('#{0}').modal('hide');";

			if (DragMode == ModalDialogDragMode.Required)
			{
				scriptPattern += "$('#{0} .modal-dialog').draggable('destroy');";
			}

			if (DragMode == ModalDialogDragMode.IfAvailable)
			{
				scriptPattern += "if (!!window.jQuery.ui && !!window.jQuery.ui.version) {{ $('#{0} .modal-dialog').draggable('destroy'); }}";
			}

			string script = String.Format(
				scriptPattern,
				DialogPanelClientIDMemento ?? _dialogContainer.ClientID);
			return script;
		}
		#endregion

		#region CloseButton_Click
		private void CloseButton_Click(object sender, EventArgs e)
		{
			Hide();
		}
		#endregion

		#region IPostBackEventHandler.RaisePostBackEvent
		void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
		{
			if (eventArgument == "Escape")
			{
				Hide();
			}
		}
		#endregion
	}
}
