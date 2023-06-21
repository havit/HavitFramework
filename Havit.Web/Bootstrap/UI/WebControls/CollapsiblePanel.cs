using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI;
using Havit.Diagnostics.Contracts;
using Havit.Web.Bootstrap.UI.WebControls.ControlsValues;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// CollapsiblePanel je flexibilní control, který vám umožní snadno přidávat skládací oddíly na vaše webové stránky.
/// </summary>
//
//INamingContainer je nutny, aby sel definovat "PostBackTrigger":
//<asp:UpdatePanel runat="server">
//<Triggers>
//	< asp:PostBackTrigger ControlID="TestUpdatePanelCollapsiblePanel1" />
//</Triggers>
//<ContentTemplate>
//	<bc:CollapsiblePanel ID="TestUpdatePanelCollapsiblePanel" AutoPostBack="True" runat="server">
//	...
//	...
//	...
[PersistChildren(false)]
[ParseChildren(true)]
public class CollapsiblePanel : Control, INamingContainer
{
	/// <summary>
	/// Vyvolá se, pokud je změněn stav panelu (z collapsible na expanded a obráceně).
	/// </summary>
	public event EventHandler CollapsedStateChanged;

	private const bool DefaultCollapsionPanelState = true;

	private readonly HiddenField collapsedHiddenField;

	/// <summary>
	/// Je polozka sbalena?
	/// </summary>
	[DefaultValue(DefaultCollapsionPanelState)]
	public bool Collapsed
	{
		get
		{
			bool result;
			if (Boolean.TryParse(collapsedHiddenField.Value, out result))
			{
				return result;
			}
			return DefaultCollapsionPanelState;
		}
		set
		{
			collapsedHiddenField.Value = value.ToString();
		}
	}

	/// <summary>
	/// Automaticky postback?
	/// </summary>
	[DefaultValue(false)]
	public bool AutoPostBack
	{
		get
		{
			return (bool)(ViewState["AutoPostBack"] ?? false);
		}
		set
		{
			ViewState["AutoPostBack"] = value;
		}
	}

	/// <summary>
	/// Header text of CollapsiblePanel. This property is ignored when HeaderTemplate is used.
	/// </summary>
	[DefaultValue("")]
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
	/// Gets or sets the template for displaying the content of CollapsiblePanel.
	/// When content template set it cannot be changed anymore.
	/// </summary>
	[Browsable(false)]
	[TemplateInstance(TemplateInstance.Single)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public ITemplate ContentTemplate
	{
		get
		{
			return this._contentTemplate;
		}
		set
		{
			Contract.Requires<ArgumentNullException>(value != null, "value");
			Contract.Assert<InvalidOperationException>(_contentTemplate == null, "When content template set it cannot be changed anymore.");

			this._contentTemplate = value;
			_contentTemplateContainer = new Control();
			_contentTemplate.InstantiateIn(this._contentTemplateContainer);
			EnsureChildControls();
			this.Controls.Add(_contentTemplateContainer);
		}
	}
	private ITemplate _contentTemplate;
	private Control _contentTemplateContainer;

	/// <summary>
	/// Gets or sets the template for displaying the header of CollapsiblePanel.
	/// If not set, HeaderText is used instead of HeaderTemplate.
	/// </summary>
	[Browsable(false)]
	[TemplateInstance(TemplateInstance.Single)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public ITemplate HeaderTemplate
	{
		get
		{
			return this._headerTemplate;
		}
		set
		{
			Contract.Requires<ArgumentNullException>(value != null, "value");
			Contract.Assert<InvalidOperationException>(_headerTemplate == null, "When header template set it cannot be changed anymore.");

			this._headerTemplate = value;
			_headerTemplateContainer = new Control();
			_headerTemplate.InstantiateIn(this._headerTemplateContainer);
			EnsureChildControls();
			this.Controls.Add(_headerTemplateContainer);
		}
	}
	private ITemplate _headerTemplate;
	private Control _headerTemplateContainer;

	/// <summary>
	/// Public constructor.
	/// </summary>
	public CollapsiblePanel()
	{
		collapsedHiddenField = new HiddenField();
	}

	/// <summary>
	/// Called by the ASP.NET page framework to notify server controls that use composition-based
	/// implementation to create any child controls they contain in preparation for posting
	/// back or rendering.
	/// </summary>
	protected override void CreateChildControls()
	{
		base.CreateChildControls();

		Controls.Add(collapsedHiddenField);
	}

	/// <summary>
	/// Raises the System.Web.UI.Control.Init event.
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

		EnsureChildControls();

		collapsedHiddenField.ValueChanged += CollapsedHiddenField_ValueChanged;
	}

	/// <summary>
	/// Occurs when the value of the Value property changes between posts to the server.
	/// </summary>
	protected virtual void OnCollapsedStateChanged(EventArgs eventArgs)
	{
		if (CollapsedStateChanged != null)
		{
			CollapsedStateChanged(this, eventArgs);
		}
	}

	private void CollapsedHiddenField_ValueChanged(object sender, EventArgs e)
	{
		OnCollapsedStateChanged(e);
	}

	/// <summary>
	/// Raises the System.Web.UI.Control.PreRender event.
	/// </summary>
	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		// ensure requirements
		ClientScripts.BootstrapClientScriptHelper.RegisterBootstrapClientScript(Page);

		// register Tab Extensions script
		ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, ClientScripts.BootstrapClientScriptHelper.CollapsiblePanelScriptResourceMappingName);

		// ukladani stavu collapsible panelu
		string autoPostBackScript = AutoPostBack ? this.Page.ClientScript.GetPostBackEventReference(this, String.Empty) : String.Empty;
		ScriptManager.RegisterStartupScript(
			this, // zde nechceme page, jinak se nam budou v JS navazovat udalosti vicekrat
			GetType(), 
			$"Havit_CollapsiblePanel_Init_{this.ClientID}",
			$@"Havit_CollapsiblePanel_Init(
					""{this.ClientID}"",
					""{collapsedHiddenField.ClientID}"",
					""{autoPostBackScript}""
				); ", true);		
	}

	/// <summary>
	/// Renders CollapsiblePanel header.
	/// </summary>
	private void RenderHeader(HtmlTextWriter writer)
	{
		//<div data-target="#{this.ClientID}" data-toggle="collapse" class="panel-heading" Onclick="return false; __doPostBack()...">
		//  Panel heading...
		// </div>
		writer.AddAttribute("data-target", $"#{this.ClientID}");
		writer.AddAttribute("data-toggle", "collapse");
		writer.AddAttribute(HtmlTextWriterAttribute.Class, "panel-heading");
		writer.RenderBeginTag(HtmlTextWriterTag.Div);

		if (_headerTemplateContainer != null)
		{
			_headerTemplateContainer.RenderControl(writer);
		}
		else
		{
			writer.Write(HeaderText);
		}

		writer.RenderEndTag(); // div
	}

	/// <summary>
	/// Renders CollapsiblePanel header.
	/// </summary>
	private void RenderContent(HtmlTextWriter writer)
	{
		// <div id="#{this.ClientID}" class="collapse panel-collapse">
		//   <div class="panel-body">
		//     Panel content
		//   </div>
		// </ div >
		writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
		writer.AddAttribute(HtmlTextWriterAttribute.Class, "collapse" + (Collapsed ? String.Empty : " in") + " panel-collapse");
		writer.RenderBeginTag(HtmlTextWriterTag.Div);

		writer.AddAttribute(HtmlTextWriterAttribute.Class, "panel-body");
		writer.RenderBeginTag(HtmlTextWriterTag.Div);

		if (_contentTemplateContainer != null)
		{
			_contentTemplateContainer.RenderControl(writer);
		}

		writer.RenderEndTag(); // div
		writer.RenderEndTag(); // div
	}

	/// <summary>
	/// Sends server control content to a provided System.Web.UI.HtmlTextWriter object,
	/// which writes the content to be rendered on the client.
	/// </summary>		
	protected override void Render(HtmlTextWriter writer)
	{
		collapsedHiddenField.RenderControl(writer);

		// http://getbootstrap.com/components/#panels
		// <div class="panel panel-default">
		//   <div data-target="#{this.ClientID}" data-toggle="collapse" class="panel-heading" Onclick="return false; __doPostBack()...">
		//     Panel heading without title
		//	 </div>
		//   <div id="#{this.ClientID}" class="collapse panel-collapse">
		//     <div class="panel-body">
		//       Panel content
		//     </div>
		//   </div>
		// </div>

		writer.AddAttribute(HtmlTextWriterAttribute.Class, "panel panel-default");
		writer.RenderBeginTag(HtmlTextWriterTag.Div);

		RenderHeader(writer);
		RenderContent(writer);

		writer.RenderEndTag(); // div
	}
}