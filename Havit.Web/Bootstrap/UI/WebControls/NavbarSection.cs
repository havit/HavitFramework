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
	/// Control to be used independently or (internally) in Navbar for displaying bootstrap menu items.
	/// </summary>
	[ParseChildren(true)]
	public class NavbarSection : Control
	{
		#region Private fields
		/// <summary>
		/// Nested NavbarSectionInternal control.
		/// </summary>
		private NavbarSectionInternal _navbarSectionInternal;
		#endregion

		#region MenuItems
		/// <summary>
		///  Navbar items.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public NavbarItemCollection MenuItems
		{
			get { return _navbarSectionInternal.MenuItems; }
		}
		#endregion

		#region DataSource, DataSourceID
		/// <summary>
		/// Gets or sets the data source for menu items.
		/// </summary>		
		public object DataSource
		{
			get { return _navbarSectionInternal.DataSource; }
			set { _navbarSectionInternal.DataSource = value; }
		}

		/// <summary>
		/// Gets or sets the data source control ID for menu items.
		/// </summary>
		public string DataSourceID
		{
			get { return _navbarSectionInternal.DataSourceID; }
			set { _navbarSectionInternal.DataSourceID = value; }
		}
		#endregion

		#region ShowCaret
		/// <summary>
		/// Indicates whether render caret for submenus.
		/// Default false.
		/// </summary>
		[DefaultValue(false)]
		public bool ShowCaret
		{
			get { return _navbarSectionInternal.ShowCaret; }
			set { _navbarSectionInternal.ShowCaret = value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public NavbarSection()
		{
			_navbarSectionInternal = new NavbarSectionInternal();
		}
		#endregion

		#region OnInit
		/// <summary>
		/// Ensures child controls created.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EnsureChildControls();
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Adds NavbarSectionInternal control to control tree.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			Controls.Add(_navbarSectionInternal);
		}
		#endregion
	}
}
