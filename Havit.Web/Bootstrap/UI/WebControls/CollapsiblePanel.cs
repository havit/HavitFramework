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

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// CollapsiblePanel je flexibilní control, který vám umožní snadno přidávat skládací oddíly na vaše webové stránky.
	/// </summary>
	[PersistChildren(false), ParseChildren(true)]
	public class CollapsiblePanel : Control
	{
		#region Constants	
		private const bool DafaultCollapsionPanelState = true;
		#endregion

		#region Controls
		private readonly HiddenField collapsedHiddenField;
		#endregion

		/// <summary>
		/// Je polozka sbalena?
		/// </summary>
		[DefaultValue(DafaultCollapsionPanelState)]
		public bool Collapsed
		{
			get
			{
				bool result;
				if (Boolean.TryParse(collapsedHiddenField.Value, out result))
				{
					return result;
				}
				return DafaultCollapsionPanelState;
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

		#region HeaderText
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
		#endregion

		#region ContentTemplate
		/// <summary>
		/// Gets or sets the template for displaying the content of CollapsiblePanel.
		/// When content template set it cannot be changed anymore.
		/// </summary>
		[Browsable(false), TemplateInstance(TemplateInstance.Single), PersistenceMode(PersistenceMode.InnerProperty)]
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
		#endregion

		#region HeaderTemplate
		/// <summary>
		/// Gets or sets the template for displaying the header of CollapsiblePanel.
		/// If not set, HeaderText is used instead of HeaderTemplate.
		/// </summary>
		[Browsable(false), TemplateInstance(TemplateInstance.Single), PersistenceMode(PersistenceMode.InnerProperty)]
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
		#endregion

		#region CollapsiblePanel
		/// <summary>
		/// Public constructor.
		/// </summary>
		public CollapsiblePanel()
		{
			collapsedHiddenField = new HiddenField();
		}
		#endregion

		#region CreateChildControls
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
		#endregion

		#region OnInit
		/// <summary>
		/// Raises the System.Web.UI.Control.Init event.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			EnsureChildControls();
		}
		#endregion

		#region OnPreRender
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
				);", true);		
		}
		#endregion

		#region RenderHeader
		/// <summary>
		/// Renders CollapsiblePanel header.
		/// </summary>
		private void RenderHeader(HtmlTextWriter writer)
		{
			//<div data-target="#{this.ClientID}" data-toggle="collapse" class="panel-heading">
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
		#endregion

		#region RenderHeader
		/// <summary>
		/// Renders CollapsiblePanel header.
		/// </summary>
		private void RenderContent(HtmlTextWriter writer)
		{
			//<div id="#{this.ClientID}" class="collapse panel-collapse">
			//  Panel content
			//</div>
			writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "collapse" + (Collapsed ? String.Empty : " in") + " panel-collapse");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			if (_contentTemplateContainer != null)
			{
				_contentTemplateContainer.RenderControl(writer);
			}

			writer.RenderEndTag(); // div
		}
		#endregion

		#region Render
		/// <summary>
		/// Sends server control content to a provided System.Web.UI.HtmlTextWriter object,
		/// which writes the content to be rendered on the client.
		/// </summary>		
		protected override void Render(HtmlTextWriter writer)
		{
			collapsedHiddenField.RenderControl(writer);

			// http://getbootstrap.com/components/#panels
			// <div class="panel panel-default">
			//   <div data-target="#{this.ClientID}" data-toggle="collapse" class="panel-heading">
			//     Panel heading without title
			//	 </div>
			//   <div id="#{this.ClientID}" class="collapse panel-collapse">
			//     Panel content
			//   </div>
			// </div>

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "panel panel-default");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			RenderHeader(writer);
			RenderContent(writer);

			writer.RenderEndTag(); // div
		}
		#endregion
	}
}