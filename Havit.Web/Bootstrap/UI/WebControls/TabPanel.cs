using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Diagnostics.Contracts;
using Havit.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// TabPanel - one item in TabContainer with its header and content.
	/// </summary>
	[PersistChildren(false)]
	[ParseChildren(true)]
	public class TabPanel : WebControl
	{
		#region ContentTemplate
		/// <summary>
		/// Gets or sets the template for displaying the content of TabPanel.
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
				this.Controls.Add(_contentTemplateContainer);				
			}

		}
		private ITemplate _contentTemplate;
		private Control _contentTemplateContainer;
		#endregion

		#region HeaderTemplate
		/// <summary>
		/// Gets or sets the template for displaying the header of TabPanel.
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
				this.Controls.Add(_headerTemplateContainer);
			}
		}
		private ITemplate _headerTemplate;
		private Control _headerTemplateContainer;
		#endregion

		#region HeaderText
		/// <summary>
		/// Header text of TabPanel. This property is ignored when HeaderTemplate is used.
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

		#region Active, ActiveInternal
		/// <summary>
		/// Indicates whether TabPanel is active (selected).
		/// Use TabContainer.ActiveTabPanel to set active container.
		/// </summary>
		public bool Active
		{
			get
			{
				return ActiveInternal;
			}
			set
			{
				if (_tabContainer != null)
				{
					//activate them by TabContainer - it deactivates other tabs
					_tabContainer.ActiveTabPanel = this; // ActiveInternal is set in this call
				}
				else
				{
					ActiveInternal = value;
				}
			}
		}

		/// <summary>
		/// Internal storege of Active property value with no logic (Active property has logic for setting ActiveTabPanel on TabContainer.)
		/// </summary>
		internal bool ActiveInternal
		{
			get
			{
				return (bool)(ViewState["Active"] ?? false);
			}
			set
			{
				ViewState["Active"] = value;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public TabPanel() : base(HtmlTextWriterTag.Div)
		{
		}
		#endregion

		#region OnInit
		/// <summary>
		/// Raises the Init event. This notifies the control to perform any steps necessary for its creation on a page request.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			RegisterToParentTabs();
			base.OnInit(e);
		}
		#endregion
		
		#region RegisterToParentTabs
		/// <summary>
		/// Registers TabPanel to closest parent TabContainer.
		/// </summary>
		private void RegisterToParentTabs()
		{
			Control parent = this.Parent;
			while (parent != null)
			{
				if (parent is TabContainer)
				{
					_tabContainer = (TabContainer)parent;
					_tabContainer.RegisterTabPanel(this);
					return;
				}
				else if (parent is UpdatePanel)
				{
					throw new HttpException("Update Panel cannot be positioned between TabContainer and TabPanel in the control tree hiearchy.");
				}
				else
				{
					parent = parent.Parent;
				}
			}
			throw new HttpException(String.Format("TabPanel '{0}' does not have TabContainer parent.", this.ID));
		}
		private TabContainer _tabContainer;
		#endregion

		#region RenderHeader
		/// <summary>
		/// Renders TabPanel header.
		/// </summary>
		internal void RenderHeader(HtmlTextWriter writer)
		{
			if (Visible)
			{				
				if (Active)
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "active");
				}
				else if (!Enabled)
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "disabled");
				}
				writer.RenderBeginTag(HtmlTextWriterTag.Li);

				if (!Enabled)
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
					writer.AddAttribute("disabled", "disabled");
					writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "return false;");
				}
				else
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Href, "#" + ClientID);
					writer.AddAttribute("data-toggle", "tab.havit");
				}

				writer.RenderBeginTag(HtmlTextWriterTag.A);
				
				if (_headerTemplateContainer != null)
				{
					_headerTemplateContainer.RenderControl(writer);
				}
				else
				{
					writer.WriteEncodedText(HeaderText);
				}

				writer.RenderEndTag(); //A
				writer.RenderEndTag(); // LI
			}
		}
		#endregion

		#region AddAttributesToRender
		/// <summary>
		/// Adds to the specified writer those HTML attributes and styles that need to be rendered.
		/// </summary>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			// prepare values for base call
			EnsureID();
			CssClass = (CssClass + " tab-pane"
				+ (_tabContainer.UseAnimations ? " fade" : String.Empty)
				+ (_tabContainer.UseAnimations && Active ? " in" : String.Empty)
				+ (Active ? " active" : String.Empty))
				.Trim();
			base.AddAttributesToRender(writer);
		}
		#endregion

		#region Render
		/// <summary>
		/// Ensures not rendering content of disabled TabPanel.
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			if (Enabled)
			{
				base.Render(writer);
			}
		}
		#endregion

		#region RenderContents
		/// <summary>
		/// Renders the control content into the specified writer.
		/// </summary>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			// Render only content (header rendered in RenderHeader method).
			if (_contentTemplateContainer != null)
			{
				_contentTemplateContainer.RenderControl(writer);
			}
		}
		#endregion

		#region OnUnload
		/// <summary>
		/// Occures when control unloads from page.
		/// </summary>
		protected override void OnUnload(EventArgs e)
		{
			base.OnUnload(e);
			if (_tabContainer != null)
			{
				_tabContainer.UnregisterTabPanel(this);
			}
		}
		#endregion
	}
}
