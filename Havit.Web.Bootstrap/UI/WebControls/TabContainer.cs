using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Diagnostics.Contracts;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Container of tab panels.
	/// Renders only TabPanels in the Child controls (or Child of Child - Child can be nested in Repeater control, etc.).
	/// </summary>
	public class TabContainer : Control
	{
		#region Private fields
		/// <summary>
		/// List of child TabPanels.
		/// </summary>
		private List<TabPanel> tabPanels = new List<TabPanel>();

		/// <summary>
		/// Hidden field for reading active tab set by client.
		/// </summary>
		private HiddenField _activeTabHiddenField;
		#endregion

		#region TabMode
		/// <summary>
		/// Tab mode (tabs vs. pills).
		/// </summary>
		[DefaultValue(TabMode.Tabs)]
		public TabMode TabMode
		{
			get
			{
				return (TabMode)(ViewState["TabMode"] ?? TabMode.Tabs);
			}
			set
			{
				ViewState["TabMode"] = value;
			}
		}
		#endregion

		#region Justified
		/// <summary>
		/// Justified Tabs/Pills. 
		/// Bootstrap documentation: Easily make tabs or pills equal widths of their parent at screens wider than 768px with .nav-justified. On smaller screens, the nav links are stacked.
		/// </summary>
		[DefaultValue(false)]
		public bool Justified
		{
			get
			{
				return (bool)(ViewState["Justified"] ?? false);
			}
			set
			{
				ViewState["Justified"] = value;
			}
		}
		#endregion

		#region AutoPostBack
		/// <summary>
		/// Get or sets whether do postback when any active TabPanel changes.
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
		#endregion

		#region ActiveTabPanel
		/// <summary>
		/// Gets or sets active TabPanel.
		/// </summary>
		public TabPanel ActiveTabPanel
		{
			get
			{
				return tabPanels.FirstOrDefault(item => item.ActiveInternal);
			}
			set
			{
				tabPanels.ForEach(item => item.ActiveInternal = false);
				if (value != null)
				{
					value.ActiveInternal = true;
				}
			}
		}
		#endregion

		#region ActiveTabPanelChanged, OnActiveTabPanelChanged
		/// <summary>
		/// Occurs when TabPanel change between post to the server.
		/// </summary>
		public event EventHandler ActiveTabPanelChanged;

		/// <summary>
		/// Occurs when the value of the Value property changes between posts to the server.
		/// </summary>
		protected virtual void OnActiveTabPanelChanged(EventArgs args)
		{
			if (ActiveTabPanelChanged != null)
			{
				ActiveTabPanelChanged(this, args);
			}
		}
		#endregion

		#region AutoPostBack
		/// <summary>
		/// Get or sets whether use animations when changing TabPanels.
		/// Not used when TabPanel changed on server side.
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

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public TabContainer()
		{
			_activeTabHiddenField = new HiddenField();			
			_activeTabHiddenField.ValueChanged += ActiveTabHiddenField_ValueChanged;
		}
		#endregion

		#region OnInit
		/// <summary>
		/// Raises the Init event. This notifies the control to perform any steps necessary for its creation on a page request.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EnsureChildControls();
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Notifies any controls that use composition-based implementation to create any child controls they contain in preperation for postback or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.Controls.Add(_activeTabHiddenField);
		}
		#endregion

		#region RegisterTabPanel, UnregisterTabPanel
		/// <summary>
		/// Used from TabPanel to register in container.
		/// </summary>
		internal void RegisterTabPanel(TabPanel tabPanel)
		{
			if ((ActiveTabPanel != null) && tabPanel.Active)
			{
				ActiveTabPanel = null; // when adding active tab, deactivate current tab
			}
			tabPanels.Add(tabPanel);
		}

		/// <summary>
		/// Used from TabPanel to ungregister from container.
		/// </summary>
		internal void UnregisterTabPanel(TabPanel tabPanel)
		{
			tabPanels.Remove(tabPanel);
		}
		#endregion

		#region ActiveTabHiddenField_ValueChanged
		private void ActiveTabHiddenField_ValueChanged(object sender, EventArgs e)
		{
			TabPanel oldActiveTabPanel = this.ActiveTabPanel;
			string newActiveTabPanelClientID = _activeTabHiddenField.Value.TrimStart('#'); // value is a client url from tab container header (includes jQuery TabPanel selector - symbol # followed by ClientID)
			TabPanel newActiveTabPanel = tabPanels.FirstOrDefault(item => item.ClientID == newActiveTabPanelClientID);
			Contract.Assert<HttpException>(newActiveTabPanel != null, String.Format("No TabPanel was not found for ClientID '{0}'.", newActiveTabPanel));

			if (oldActiveTabPanel != newActiveTabPanel)
			{
				ActiveTabPanel = newActiveTabPanel;
				OnActiveTabPanelChanged(e);
			}
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Raises the <see langword='PreRender'/> event. This method uses event arguments to pass the event data to the control.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			_activeTabHiddenField.Value = "";
			// set active tab if not already set

			// when Repeater for TabPanels is used, registration of panel is not in order of appearance in page
			// so we have to sort TabPanels to have correct order of headers
			// (correct order of contents is not required)
			OrderTabPanelsInOrderOfAppearanceInPage();

			if ((this.ActiveTabPanel == null) && (tabPanels.Count > 0))
			{
				ActiveTabPanel = tabPanels.FirstOrDefault(item => item.Visible && item.Enabled);
			}
		}
		#endregion

		#region Render, RenderHeader, RenderContent
		/// <summary>
		/// Outputs control content to a provided HTMLTextWriter output stream.
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			RenderHeader(writer);
			RenderContent(writer); // base call in this method
		}

		/// <summary>
		/// Renders list of tab panels (headers).
		/// </summary>
		private void RenderHeader(HtmlTextWriter writer)
		{
			string cssClass = (this.TabMode.GetCssClass() + (Justified ? " nav-justified" : "")).Trim();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
			writer.AddAttribute("data-tab-persister", "#" + _activeTabHiddenField.ClientID);
			writer.RenderBeginTag(HtmlTextWriterTag.Ul);
			foreach (TabPanel tabPanel in tabPanels)
			{
				tabPanel.RenderHeader(writer);
			}
			writer.RenderEndTag();
		}

		/// <summary>
		/// Renders tab pages content.
		/// </summary>
		private void RenderContent(HtmlTextWriter writer)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "tab-content");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			base.Render(writer);
			writer.RenderEndTag();
		}
		#endregion

		#region OrderTabPanelsInOrderOfAppearanceInPage*
		/// <summary>
		/// Order TabPanels in order of appearance in page.
		/// </summary>
		private void OrderTabPanelsInOrderOfAppearanceInPage()
		{
			// we can use only results from GetFlattenedTabPanels - there should be no difference
			tabPanels = GetFlattenedTabPanels(this).ToList();
		}

		private IEnumerable<TabPanel> GetFlattenedTabPanels(Control control)
		{
			if (control is TabPanel)
			{
				yield return (TabPanel)control;
			}
			else if (control.HasControls())
			{
				foreach (Control childControl in control.Controls)
				{
					foreach (TabPanel nestedTabPanel in GetFlattenedTabPanels(childControl))
					{
						yield return nestedTabPanel;
					}
				}				
			}
		}
		#endregion

	}
}
