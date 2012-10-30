using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Collapser je ovládací prvek, který zajišťuje funkčnost collapse/expand pro libovolný jiný element stránky.
	/// </summary>
	/// <example>
	/// Jednoduchý Collapser:
	/// <code>
	///	&lt;havit:Collapser
	///		Text="První možnost zadání textu"
	///		ContentElement="CollapseDiv"
	///		CssClass="normal"
	///		CssClassExpanded="expanded"
	///		CssClassCollapsed="collapsed"
	///		Runat="server"
	///	&gt;Text je možné zadat i jako inner-text&lt;/havit:Collapser&gt;
	///	
	///	&lt;div id="CollapseDiv"&gt;
	///		Od:
	///		&lt;asp:TextBox ID="OdDateTB" Text="3.3.2004" Runat="server" /&gt;
	///	&lt;/div&gt;
	/// </code>
	/// </example>
	[ParseChildren(false)]
	public class Collapser : WebControl
	{
		#region Properties
		/// <summary>
		/// Text ovládací prvku.
		/// </summary>
		public string Text
		{
			get
			{
				string tmp = (string)ViewState["Text"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["Text"] = value;
			}
		}

		/// <summary>
		/// CssClass pro ovládací prvek ve stavu expanded.
		/// </summary>
		/// <remarks>
		/// Pomocí stylu můžeme například nastavit obrázek pozadí na mínus [-].
		/// </remarks>
		public string CssClassExpanded
		{
			get
			{
				string tmp = (string)ViewState["CssClassExpanded"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["CssClassExpanded"] = value;
			}
		}

		/// <summary>
		/// CssClass pro ovládací prvek ve stavu collapsed.
		/// </summary>
		/// <remarks>
		/// Pomocí stylu můžeme například nastavit obrázek pozadí na mínus [-].
		/// </remarks>
		public string CssClassCollapsed
		{
			get
			{
				string tmp = (string)ViewState["CssClassCollapsed"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["CssClassCollapsed"] = value;
			}
		}

		/// <summary>
		/// Odkaz (ID) na element, která má být expandována/collapsována.<br/>
		/// </summary>
		/// <remarks>
		/// Nejprve se zkouší, jestli neexistuje control s tímto ID.
		/// Pokud ano, použije se jeho ClientID, pokud ne, použije se přímo ContentElement.
		/// </remarks>
		public string ContentElement
		{
			get
			{
				string tmp = (string)ViewState["ContentElement"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["ContentElement"] = value;
			}
		}

		/// <summary>
		/// Indikuje, zda-li má být <see cref="ContentElement"/> zobrazen sbalený/rozbalený.<br/>
		/// </summary>
		public bool Collapsed
		{
			get
			{
				object tmp = ViewState["Collapsed"];
				if (tmp != null)
				{
					return (bool)tmp;
				}
				return true;
			}
			set
			{
				ViewState["Collapsed"] = value;
			}
		}
		#endregion

		#region private properties
		/// <summary>
		/// Úplná CssClass pro stav Collapsed
		/// </summary>
		private string CssClassCollapsedFull
		{
			get
			{
				return (this.CssClass + " " + this.CssClassCollapsed).Trim();
			}
		}

		/// <summary>
		/// Úplná CssClass pro stav Expanded
		/// </summary>
		private string CssClassExpandedFull
		{
			get
			{
				return (this.CssClass + " " + this.CssClassExpanded).Trim();
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci controlu.
		/// </summary>
		public Collapser()
			: base(HtmlTextWriterTag.Span)
		{
		}
		#endregion

		#region AddParsedSubObject
		/// <summary>
		/// Zajišťuje pronesení inner-textu controlu do property <see cref="Text"/>.
		/// </summary>
		protected override void AddParsedSubObject(object obj)
		{
			if (!(obj is LiteralControl))
			{
				throw new HttpException("Potomek musí být Literal.");
			}
			this.Text = ((LiteralControl)obj).Text;
		}
		#endregion

		#region OnLoad
		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data. </param>
		protected override void OnLoad(EventArgs e)
		{
			string value = this.Page.Request.Form[this.ClientID + "State"];
			if (value == "collapsed")
			{
				Collapsed = true;
			}
			else if (value == "expanded")
			{
				Collapsed = false;
			}

			base.OnLoad(e);
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Voláno před renderováním.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			if (this.Enabled)
			{
				RegisterClientScript();
			}
			
			ScriptManager.RegisterHiddenField(this, this.ClientID + "State", "");

			base.OnPreRender(e);
		}
		#endregion

		#region AddAttributesToRender
		/// <summary>
		/// Doplní Attributes o hodnoty z properties.
		/// </summary>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			// nastavení stylů
			if (this.Collapsed)
			{
				this.ControlStyle.CssClass = this.CssClassCollapsedFull;
			}
			else
			{
				this.ControlStyle.CssClass = this.CssClassExpandedFull;
			}

			// zajištění povinného renderování atributu ID
			if (this.ID == null)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			}
			base.AddAttributesToRender(writer);
		}
		#endregion

		#region RenderContents
		/// <summary>
		/// Renderuje obsahu elementu.
		/// </summary>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.Write(this.Text);
		}
		#endregion

		#region RegisterClientScript
		private void RegisterClientScript()
		{
			const string clientScriptKey = "Havit.Web.UI.WebControls.Collapser";
			const string toggleCollapser =
					 @"
function havitCollapserToggle(collapserElementId, collapserStateElementId, contentElementId, cssClassCollapsed, cssClassExpanded)
{
	var content = document.getElementById(contentElementId);
	if (content.style.display == 'none')
	{
		havitCollapserExpand(collapserElementId, collapserStateElementId, contentElementId, cssClassExpanded);
	}
	else
	{
		havitCollapserCollapse(collapserElementId, collapserStateElementId, contentElementId, cssClassCollapsed);
	}
}

function havitCollapserCollapse(collapserElementId, collapserStateElementId, contentElementId, cssClassCollapsed)
{
	var content = document.getElementById(contentElementId);
	if (content.style.display != 'none')
	{
		content.setAttribute('previousDisplayStyle', content.style.display);
	}
	content.style.display = 'none';

	var collapser = document.getElementById(collapserElementId);
	collapser.className = cssClassCollapsed;

	var collapserState = document.getElementById(collapserStateElementId);
	collapserState.value = 'collapsed';
}

function havitCollapserExpand(collapserElementId, collapserStateElementId, contentElementId, cssClassExpanded)
{

	var content = document.getElementById(contentElementId);
	var previousDisplayStyle = content.getAttribute('previousDisplayStyle');
	if (typeof(previousDisplayStyle) != 'undefined')
	{
		content.style.display = previousDisplayStyle;
	}
	else
	{
		content.style.display = '';
	}

	var collapser = document.getElementById(collapserElementId);
	collapser.className = cssClassExpanded;

	var collapserState = document.getElementById(collapserStateElementId);
	collapserState.value = 'expanded';

}";
			 
			ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), clientScriptKey, toggleCollapser, true);
			
			string toggleScript = String.Format("havitCollapserToggle('{0}', '{0}State', '{1}', '{2}', '{3}');",
				this.ClientID,
				ResolveID(this.ContentElement),
				this.CssClassCollapsedFull,
				this.CssClassExpandedFull);
			this.Attributes.Add("onclick", toggleScript);

			if (this.Collapsed)
			{
				string collapseScript = String.Format("havitCollapserCollapse('{0}', '{0}State', '{1}', '{2}');",
					this.ClientID,
					ResolveID(this.ContentElement),
					this.CssClassCollapsedFull);

				ScriptManager.RegisterStartupScript(this.Page, this.GetType(), clientScriptKey + ResolveID(this.ContentElement),  // zajistí jediné volání pro element
					collapseScript, true);
			}
		}
		#endregion

		#region ResolveID
		/// <summary>
		/// Pokud ID patří controlu, pak vrátí jeho ClientID, jinak vrátí zpět původní ID.
		/// </summary>
		/// <param name="id">ID k resolvování</param>
		/// <returns>cílové ID</returns>
		protected virtual string ResolveID(string id)
		{
			Control ctrl = this.NamingContainer.FindControl(id);
			if (ctrl != null)
			{
				return ctrl.ClientID;
			}
			return id;
		}
		#endregion
	}
}
