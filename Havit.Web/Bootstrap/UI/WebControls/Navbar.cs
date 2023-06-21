using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Navbar control for displaying bootstrap navbar.
/// </summary>
[ParseChildren(true)]
public class Navbar : Control
{
	private readonly NavbarSection _mainMenuSection;
	private Control _brandContainer;
	private readonly NavbarSection _rightMenuSection;

	/// <summary>
	///  Navbar items.
	/// </summary>
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public NavbarItemCollection MenuItems
	{
		get { return _mainMenuSection.MenuItems; }
	}

	/// <summary>
	/// Brand template - template for rendering brand section of menu.
	/// </summary>
	[TemplateInstance(TemplateInstance.Single)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public virtual ITemplate BrandTemplate
	{
		get
		{
			return _brandTemplate;
		}
		set
		{
			_brandTemplate = value;
		}
	}
	private ITemplate _brandTemplate;

	/// <summary>
	/// Url for rendering in brand.
	/// If value is empty string, element A for navbar-brand is not rendered.
	/// Default value is ~/.
	/// </summary>
	[DefaultValue("~/")]
	public string BrandUrl
	{
		get
		{
			return (string)(ViewState["BrandUrl"] ?? "~/");
		}
		set
		{
			ViewState["BrandUrl"] = value;
		}
	}

	/// <summary>
	///  Right navbar items.
	/// </summary>
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public virtual NavbarItemCollection RightSectionItems
	{
		get
		{
			return _rightMenuSection.MenuItems;
		}
	}

	/// <summary>
	/// Indicates whether render caret for submenus.
	/// Default false.
	/// </summary>
	public bool ShowCaret
	{
		get { return _mainMenuSection.ShowCaret; }
		set { _mainMenuSection.ShowCaret = value; }
	}

	/// <summary>
	/// Css class for menu element.
	/// Default value is &quot;navbar navbar-default&quot;.
	/// </summary>
	[DefaultValue("navbar navbar-default")]
	public string CssClass
	{
		get
		{
			return (string)(ViewState["CssClass"] ?? "navbar navbar-default");
		}
		set
		{
			ViewState["CssClass"] = value;
		}
	}

	/// <summary>
	/// Container element mode.
	/// Default value ContainerFluid.
	/// </summary>
	[DefaultValue(NavbarContainerMode.ContainerFluid)]
	public NavbarContainerMode ContainerMode
	{
		get
		{
			return (NavbarContainerMode)(ViewState["ContainerMode"] ?? NavbarContainerMode.ContainerFluid);
		}
		set
		{
			ViewState["ContainerMode"] = value;
		}
	}

	/// <summary>
	/// Text to be rendered as navigation toggle (&lt;span class=&amp;sr-only&amp;&gt;Toggle navigation&lt;/span&gt;). Support resources pattern. Default value is empty string.
	/// </summary>
	public string ToggleNavigationText
	{
		get
		{
			return (string)(ViewState["ToggleNavigationText"] ?? String.Empty);
		}
		set
		{
			ViewState["ToggleNavigationText"] = value;
		}
	}

	/// <summary>
	/// Gets or sets the data source for menu items.
	/// </summary>
	public object DataSource
	{
		get { return _mainMenuSection.DataSource; }
		set { _mainMenuSection.DataSource = value; }			
	}

	/// <summary>
	/// Gets or sets the data source control ID for menu items.
	/// </summary>
	public string DataSourceID
	{
		get { return _mainMenuSection.DataSourceID; }
		set { _mainMenuSection.DataSourceID = value; }
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public Navbar()
	{
		_mainMenuSection = new NavbarSection();
		_rightMenuSection = new NavbarSection();
	}

	/// <summary>
	/// Ensures child control creation.
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		EnsureChildControls();
	}

	/// <summary>
	/// Ensures child control creation.
	/// </summary>
	protected override void CreateChildControls()
	{
		base.CreateChildControls();

		this.Controls.Add(_mainMenuSection);
		this.Controls.Add(_rightMenuSection);

		if (this._brandTemplate != null)
		{
			_brandContainer = new Control();
			_brandTemplate.InstantiateIn(_brandContainer);
			this.Controls.Add(_brandContainer);
		}
	}

	/// <summary>
	/// Renders menu with brand template, menu items and right menu section.
	/// </summary>
	protected override void Render(HtmlTextWriter writer)
	{
		if (!String.IsNullOrEmpty(CssClass))
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
		}
		writer.AddAttribute("role", "navigation");
		writer.RenderBeginTag("nav");

		if (ContainerMode == NavbarContainerMode.Container)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "container");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
		}
		else if (ContainerMode == NavbarContainerMode.ContainerFluid)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "container-fluid");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
		}

		writer.AddAttribute(HtmlTextWriterAttribute.Class, "navbar-header");
		writer.RenderBeginTag(HtmlTextWriterTag.Div);

		string collapseID = this.ClientID + "_collapse";

		writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
		writer.AddAttribute(HtmlTextWriterAttribute.Class, "navbar-toggle");
		writer.AddAttribute("data-toggle", "collapse");
		writer.AddAttribute("data-target", "#" + collapseID);
		writer.RenderBeginTag(HtmlTextWriterTag.Button);

		string toggleText = HttpUtilityExt.GetResourceString(ToggleNavigationText);
		if (!String.IsNullOrEmpty(toggleText))
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "sr-only");
			writer.RenderBeginTag(HtmlTextWriterTag.Span);
			writer.WriteEncodedText(toggleText);
			writer.RenderEndTag(); // span.sr-only
		}

		for (int i = 0; i < 3; i++)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "icon-bar");
			writer.RenderBeginTag(HtmlTextWriterTag.Span);
			writer.RenderEndTag(); // span.icon-bar
		}

		writer.RenderEndTag(); // button

		if (_brandContainer != null)
		{
			if (!String.IsNullOrEmpty(BrandUrl))
			{
				// when brand url is empty, we gives a developer a chance to use navbar-brand class in brand container
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "navbar-brand");
				writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(BrandUrl));
				writer.RenderBeginTag(HtmlTextWriterTag.A);
			}

			_brandContainer.RenderControl(writer);

			if (!String.IsNullOrEmpty(BrandUrl))
			{
				writer.RenderEndTag(); // span.navbar-brand
			}
		}

		writer.RenderEndTag(); // div.navbar-header

		writer.AddAttribute(HtmlTextWriterAttribute.Class, "collapse navbar-collapse");
		writer.AddAttribute(HtmlTextWriterAttribute.Id, collapseID);
		writer.RenderBeginTag(HtmlTextWriterTag.Div);

		_mainMenuSection.RenderControl(writer);

		if (_rightMenuSection.MenuItems.Count > 0)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "nav navbar-nav navbar-right");
			writer.RenderBeginTag(HtmlTextWriterTag.Ul);

			_rightMenuSection.RenderControl(writer);

			writer.RenderEndTag(); // ul
		}

		writer.RenderEndTag(); // div.collapse

		if ((ContainerMode == NavbarContainerMode.Container) || (ContainerMode == NavbarContainerMode.ContainerFluid))
		{
			writer.RenderEndTag(); // div.container/div.container-fluid
		}

		writer.RenderEndTag(); // nav
	}
}
