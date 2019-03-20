using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Diagnostics.Contracts;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Internal representation of NavbarSection. Main reason is to hide WebControl API from general usage.
	/// Therefore only NavbarSection control is public (and is used as a wrapper for this control).
	/// </summary>
	[ParseChildren(true)]
	internal class NavbarSectionInternal : HierarchicalDataBoundControl
	{
		/// <summary>
		///  Navbar items.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public NavbarItemCollection MenuItems { get; private set; }

		/// <summary>
		/// Indicates whether render caret for submenus.
		/// Default false.
		/// </summary>
		public bool ShowCaret
		{
			get
			{
				return (bool)(ViewState["ShowCaret"] ?? false);
			}
			set
			{
				ViewState["ShowCaret"] = value;
			}
		}

		/// <summary>
		/// Returns HtmlTextWriterTag.Ul to be used for control rendering.
		/// </summary>
		protected override HtmlTextWriterTag TagKey
		{
			get { return HtmlTextWriterTag.Ul; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public NavbarSectionInternal()
		{
			MenuItems = new NavbarItemCollection();
		}

		/// <summary>
		/// Sets default value to CssClass.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			CssClass = "nav navbar-nav";
		}

		/// <summary>
		/// Creates menu items from data source.
		/// </summary>
		protected override void PerformDataBinding()
		{
			base.PerformDataBinding();

			HierarchicalDataSourceView data = GetData(null);
			IHierarchicalEnumerable menuDataSource = data.Select();
			if (menuDataSource != null)
			{
				MenuItems.Clear();
				CreateMenuItems(menuDataSource, MenuItems, 1);
			}
		}

		/// <summary>
		/// Create menu items from data source to menu collection. Creating is limited to two levels of nesting.
		/// </summary>
		/// <param name="menuDataSource">Data source for menu items.</param>
		/// <param name="targetContainer">Container for menu items.</param>
		/// <param name="level">Nesting level.</param>
		private void CreateMenuItems(IHierarchicalEnumerable menuDataSource, IList<NavbarItem> targetContainer, int level)
		{
			foreach (object menuDataSourceItem in menuDataSource)
			{
				IHierarchyData menuItemData = menuDataSource.GetHierarchyData(menuDataSourceItem);
				if (!(menuItemData is INavigateUIData))
				{
					throw new HttpException("Only data items of INavigateUIData are supported for databinding.");
				}
				INavigateUIData navigateUIData = (INavigateUIData)menuItemData;

				if ((level > 1) && String.IsNullOrEmpty(navigateUIData.NavigateUrl) && (navigateUIData.Name == "-"))
				{
					targetContainer.Add(new NavbarSeparatorItem());
				}
				else if ((level > 1) && (String.IsNullOrEmpty(navigateUIData.NavigateUrl) && (!menuItemData.HasChildren)))
				{
					targetContainer.Add(new NavbarHeaderItem(navigateUIData.Name));
				}
				else
				{
					NavbarLinkItem linkItem = new NavbarLinkItem(navigateUIData.Name, navigateUIData.NavigateUrl);
					targetContainer.Add(linkItem);

					if ((menuItemData.HasChildren) && (level < 2))
					{
						CreateMenuItems(menuItemData.GetChildren(), linkItem.Items, level + 1);
					}
				}
			}
		}

		/// <summary>
		/// Renders menu items.
		/// </summary>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			foreach (NavbarItem item in MenuItems)
			{
				Contract.Assert(item is NavbarLinkItem || item is NavbarTextItem, "Only instances of NavbarLinkItem and NavbarTextItem are supported in menu root.");
				if (item.IsVisible)
				{
					item.Render(writer, this, this.ShowCaret, 1);
				}
			}
		}

		/// <summary>
		/// Set tracking of viewstae for menu items.
		/// </summary>
		protected override void TrackViewState()
		{
			base.TrackViewState();
			((IStateManager)MenuItems).TrackViewState();
		}
		
		/// <summary>
		/// Adds menu items to saved state.
		/// </summary>
		protected override object SaveViewState()
		{
			object baseState = base.SaveViewState();
			object menuItemsState = ((IStateManager)MenuItems).SaveViewState();
			if ((baseState == null) && (menuItemsState == null))
			{
				return null;
			}
			else
			{
				return new object[]
				{
					baseState,
					menuItemsState
				};
			}
		}

		/// <summary>
		/// Add loading saved state od menu items.
		/// </summary>
		protected override void LoadViewState(object savedState)
		{
			object baseState = null;
			object menuItemsState = null;
			if (savedState != null)
			{
				object[] state = (object[])savedState;
				baseState = state[0];
				menuItemsState = state[1];
			}
			base.LoadViewState(baseState);
			((IStateManager)MenuItems).LoadViewState(menuItemsState);
		}
	}
}
