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

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// RetractablePanel
	/// ================================================
	/// Nedodelano, v pripade pokracovani praci se musi:
	/// ================================================
	/// RetractablePanelTest1.aspx se build action se musi nastavit jako "content"
	/// RetractablePanelTest1.aspx.cs build action se musi nastavit "compile"
	/// RetractablePanelTest1.aspx.designer.cs build action se musi nastavit "compile"
	/// To same u RetractablePanelTest2*
	/// Dale RetractablePanel se musi zmenit na public
	/// </summary>
	[PersistChildren(false), ParseChildren(true)]
	internal class RetractablePanel : Control
	{
		#region Constants	
		private const string HavitRetractablePanelHeadText = "Havit_RetractablePanel";
		#endregion

		#region Controls
		private readonly HiddenField hiddenField;
		#endregion

		#region ContentTemplate
		/// <summary>
		/// Gets or sets the template for displaying the content of RetractablePanel.
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
		/// Gets or sets the template for displaying the header of RetractablePanel.
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

		#region HeaderText
		/// <summary>
		/// Header text of RetractablePanel. This property is ignored when HeaderTemplate is used.
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

		#region RetractablePanelUniqueId
		/// <summary>
		/// Retractable panel unique id on pages and on page...
		/// </summary>
		[DefaultValue("")]
		public string RetractablePanelUniqueId
		{
			get
			{
				string pageUniqueIdentificator = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Page.Request.Url.AbsolutePath.ToLower())).Replace("=", ""); // id nema rado '='
                return $"{HavitRetractablePanelHeadText}_{ClientID}_{pageUniqueIdentificator}";
            }
		}
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public RetractablePanel()
		{
			hiddenField = new HiddenField();
		}

		/// <summary>
		/// CreateChildControls
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			Controls.Add(hiddenField);
		}

		/// <summary>
		/// OnInit
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			EnsureChildControls();
			hiddenField.ValueChanged += HiddenField_ValueChanged;
		}

		#region HiddenField_ValueChanged
		private void HiddenField_ValueChanged(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region OnLoad
		/// <summary>
		/// OnLoad
		/// </summary>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			
            ScriptManager.RegisterStartupScript(this, GetType(), $"Havit_RetractablePanel_LoadCollapsibleStatus{ClientID}", $"Havit_RetractablePanel_LoadCollapsibleStatus('{RetractablePanelUniqueId}');", true);
			ScriptManager.RegisterStartupScript(this, GetType(), "Havit_RetractablePanel_Init", "Havit_RetractablePanel_Init();", true);			
        }
		#endregion

		#region RenderHeader
		/// <summary>
		/// Renders RetractablePanel header.
		/// </summary>
		private void RenderHeader(HtmlTextWriter writer)
		{
			// <span data-target="#collapsible-div-<%# Item.First().InvoiceState.ID %>" data-toggle="collapse">
			// <div class="havit_retractablepanel_header">
			// <span>Header text</span>
			// </div>
			// </span>

			writer.AddAttribute("data-target", $"#{RetractablePanelUniqueId}");
			writer.AddAttribute("data-toggle", "collapse");
			writer.RenderBeginTag(HtmlTextWriterTag.Span);

			if (_headerTemplateContainer != null)
			{
				_headerTemplateContainer.RenderControl(writer);
			}
			else
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, $"{HavitRetractablePanelHeadText}_Header");
				writer.RenderBeginTag(HtmlTextWriterTag.Div);

				writer.RenderBeginTag(HtmlTextWriterTag.Span);
				writer.Write(HeaderText);
				writer.RenderEndTag();

				writer.RenderEndTag(); // div
			}

			writer.RenderEndTag(); // span
		}
		#endregion

		#region RenderHeader
		/// <summary>
		/// Renders RetractablePanel header.
		/// </summary>
		private void RenderContent(HtmlTextWriter writer)
		{
			// <div id="collapsible-div-<%# Item.First().InvoiceState.ID %>" class="collapse">
			writer.AddAttribute(HtmlTextWriterAttribute.Id, RetractablePanelUniqueId);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "collapse");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			if (_contentTemplateContainer != null)
			{
				_contentTemplateContainer.RenderControl(writer);
			}

			writer.RenderEndTag(); // div
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Raises the <see langword='PreRender'/> event. This method uses event arguments to pass the event data to the control.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			// ensure requirements
			ClientScripts.BootstrapClientScriptHelper.RegisterBootstrapClientScript(Page);

			// register Tab Extensions script
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, ClientScripts.BootstrapClientScriptHelper.RetractablePanelScriptResourceMappingName);
		}
		#endregion

		#region Render
		/// <summary>
		/// Outputs control content to a provided HTMLTextWriter output stream.
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			RenderHeader(writer);
			RenderContent(writer);
		}
		#endregion
	}
}
