﻿using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Control to be used independently or (internally) in Navbar for displaying bootstrap menu items.
/// </summary>
[ParseChildren(true)]
public class NavbarSection : Control
{
	/// <summary>
	/// Nested NavbarSectionInternal control.
	/// </summary>
	private readonly NavbarSectionInternal _navbarSectionInternal;

	/// <summary>
	///  Navbar items.
	/// </summary>
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public NavbarItemCollection MenuItems
	{
		get { return _navbarSectionInternal.MenuItems; }
	}

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

	/// <summary>
	/// Indicates whether render caret for submenus.
	/// Default false.
	/// </summary>
	public bool ShowCaret
	{
		get { return _navbarSectionInternal.ShowCaret; }
		set { _navbarSectionInternal.ShowCaret = value; }
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public NavbarSection()
	{
		_navbarSectionInternal = new NavbarSectionInternal();
	}

	/// <summary>
	/// Ensures child controls created.
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		EnsureChildControls();
	}

	/// <summary>
	/// Adds NavbarSectionInternal control to control tree.
	/// </summary>
	protected override void CreateChildControls()
	{
		base.CreateChildControls();
		Controls.Add(_navbarSectionInternal);
	}
}
