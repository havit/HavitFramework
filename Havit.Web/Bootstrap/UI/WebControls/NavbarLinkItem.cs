using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Havit.Linq;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Navbar item - Link item.
	/// </summary>
	public class NavbarLinkItem : NavbarItem
	{
		#region Items
		/// <summary>
		/// Child items.
		/// </summary>
		public NavbarItemCollection Items { get; private set; }
		#endregion

		#region AutoHide
		/// <summary>
		/// Enables item autohide when set to true.
		/// When menu item has not any visible children (or has visible only decoration items)
		/// AND items has not set Url, menu item is automatically hidden.
		/// </summary>
		public bool AutoHide
		{
			get
			{
				return (bool)(ViewState["AutoHide"] ?? true);
			}
			set
			{
				ViewState["AutoHide"] = value;
			}
		}
		#endregion

		#region Text
		/// <summary>
		/// Header item text.
		/// Supports resources pattern.
		/// </summary>
		public string Text
		{
			get
			{
				return (string)(ViewState["Text"] ?? String.Empty);
			}
			set
			{
				ViewState["Text"] = value;
			}
		}
		#endregion

		#region Url
		/// <summary>
		/// Header item url.
		/// </summary>
		public string Url
		{
			get
			{
				return (string)(ViewState["Url"] ?? String.Empty);
			}
			set
			{
				ViewState["Url"] = value;
			}
		}
		#endregion

		#region IsDecoration
		/// <summary>
		/// Returns false - menu item is not a decoration.
		/// </summary>
		public override bool IsDecoration
		{
			get { return false; }
		}
		#endregion

		#region Enabled
		/// <summary>
		/// Indicates whether the item is enabled.
		/// Default value is true.
		/// </summary>
		public bool Enabled
		{
			get
			{
				return (bool)(ViewState["Enabled"] ?? true);
			}
			set
			{
				ViewState["Enabled"] = value;
			}
		}
		#endregion

		#region CssClass
		/// <summary>
		/// Gets or sets the CSS class.
		/// </summary>
		public string CssClass
		{
			get
			{
				return (string)ViewState["CssClass"];
			}
			set
			{
				ViewState["CssClass"] = value;
			}
		}
		#endregion

		#region EnabledFunc
		/// <summary>
		/// When function returns false, item is disabled.
		/// </summary>
		public Func<bool> EnabledFunc { get; set; }
		#endregion

		#region IsEnabled
		/// <summary>
		/// Returns true if item should be rendered. Includes evaluation of Visible property and VisibleFunc delegate.
		/// </summary>
		protected virtual bool IsEnabled
		{
			get { return Enabled && ((EnabledFunc == null) || EnabledFunc()); }
		}
		#endregion

		#region IsVisible
		/// <summary>
		/// Returns true if item is visible.
		/// </summary>
		protected internal override bool IsVisible
		{
			get
			{
				if (AutoHide && String.IsNullOrEmpty(Url) && !HasVisibleNonDecorationChildItem())
				{
					return false;
				}
				return base.IsVisible;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public NavbarLinkItem()
		{
			this.Items = new NavbarItemCollection();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public NavbarLinkItem(string text, string url, Func<bool> visibleFunc, params NavbarItem[] childItems)
		{
			this.Text = text;
			this.Url = url;
			this.VisibleFunc = visibleFunc;
			this.Items = new NavbarItemCollection(childItems);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public NavbarLinkItem(string text, string url, params NavbarItem[] childItems) : this(text, url, null, childItems)
		{
		}
		#endregion

		#region HasVisibleNonDecorationChildItem
		/// <summary>
		/// Returns true if has visible (non decoration) child item.
		/// </summary>
		private bool HasVisibleNonDecorationChildItem()
		{
			return Items.Any(item => item.IsVisible && !item.IsDecoration);
		}
		#endregion

		#region SetDirty
		/// <summary>
		/// Set child items dirty.
		/// Used internally for tracking items state.
		/// </summary>
		internal override void SetDirty()
		{
			base.SetDirty();
			if (this.Items.Count > 0)
			{
				this.Items.SetDirty();
			}
		}
		#endregion

		#region Render
		/// <summary>
		/// Renders menu items.
		/// </summary>
		public override void Render(HtmlTextWriter writer, Control container, bool showCaret, int nestingLevel)
		{
			string cssClass = "";

			if (Items.Count > 0)
			{
				cssClass += "dropdown ";
			}

			if (!Enabled)
			{
				cssClass += "disabled ";
			}

			cssClass += CssClass;

			if (!String.IsNullOrEmpty(cssClass))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass.Trim());
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Li);

			//writer.AddAttribute(HtmlTextWriterAttribute.Href, String.IsNullOrEmpty(Url) ? "#" : Url);

			if (!Enabled)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "return false;");
			}
			else
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Href, String.IsNullOrEmpty(Url) ? "#" : container.ResolveUrl(Url));
			}

			if (Items.Count > 0)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "data-toggle");
				writer.AddAttribute("data-toggle", "dropdown");
			}
			writer.RenderBeginTag(HtmlTextWriterTag.A);
			writer.WriteEncodedText(HttpUtilityExt.GetResourceString(Text));

			if (showCaret && HasVisibleNonDecorationChildItem())
			{
				writer.Write(@" <b class=""caret""></b>");
			}

			writer.RenderEndTag(); // A

			if (Items.Count > 0)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "dropdown-menu");
				writer.RenderBeginTag(HtmlTextWriterTag.Ul);
				RenderChildren(writer, container, showCaret, nestingLevel);
				writer.RenderEndTag(); // Li
			}
			writer.RenderEndTag(); // Li
		}
		#endregion

		#region RenderChildren
		/// <summary>
		/// Renders child items.
		/// </summary>
		protected void RenderChildren(HtmlTextWriter writer, Control container, bool showCaret, int nestingLevel)
		{
			bool lastWasSeparator = false;

			if ((Items.Count > 0) && (nestingLevel >= 2))
			{
				throw new InvalidOperationException(String.Format("Bootstrap menu cannot have more then one child level. Item '{0}' should not have child items.", HttpUtilityExt.GetResourceString(Text)));
			}

			// take only visible items and skip first and last separators
			NavbarItem[] navbarsVisible = Items.Where(item => item.IsVisible)
				.SkipWhile(item => item is NavbarSeparatorItem)
				.SkipLastWhile(item => item is NavbarSeparatorItem)
				.ToArray();

			foreach (NavbarItem item in navbarsVisible)
			{
				// No two separators each after other.
				if (lastWasSeparator && (item is NavbarSeparatorItem))
				{
					continue;
				}

				item.Render(writer, container, showCaret, nestingLevel + 1);
				lastWasSeparator = item is NavbarSeparatorItem;
			}
		}
		#endregion

		#region TrackViewState, SaveViewState, LoadViewState

		/// <summary>
		/// Switches tracking viewstate changes on.
		/// </summary>
		public override void TrackViewState()
		{
			base.TrackViewState();
			((IStateManager)Items).TrackViewState();
		}

		/// <summary>
		/// Saves viewstate.
		/// </summary>
		public override object SaveViewState()
		{
			object baseState = base.SaveViewState();
			object itemsState = ((IStateManager)Items).SaveViewState();
			if ((baseState == null) && (itemsState == null))
			{
				return null;
			}
			else
			{
				return new object[]
				{
					baseState,
					itemsState
				};
			}
		}

		/// <summary>
		/// Loads viewstate.
		/// </summary>
		public override void LoadViewState(object saveState)
		{
			object baseState = null;
			object itemsState = null;
			if (saveState != null)
			{
				object[] state = (object[])saveState;
				baseState = state[0];
				itemsState = state[1];
			}
			base.LoadViewState(baseState);
			((IStateManager)Items).LoadViewState(itemsState);
		}
		#endregion
	}
}