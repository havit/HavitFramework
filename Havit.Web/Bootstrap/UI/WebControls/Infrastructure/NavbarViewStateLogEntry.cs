using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls.Infrastructure;

/// <summary>
/// Used for tracking changes of NavbarItemCollection for persisting Navbar/NavbarSection items.
/// </summary>	
internal class NavbarViewStateLogEntry : IStateManager
{
	protected StateBag ViewState
	{
		get
		{
			return _viewstate;
		}
	}
	private readonly StateBag _viewstate;

	public LogItemType EntryType
	{
		get { return (LogItemType)ViewState["EntryType"]; }
		private set { ViewState["EntryType"] = value; }
	}

	public int? Index
	{
		get { return (int?)ViewState["Index"]; }
		private set { ViewState["Index"] = value; }
	}

	public Type ItemType
	{
		get
		{
			IndexedString itemType = (IndexedString)ViewState["ItemType"];
			return (itemType == null) ? null : Type.GetType(itemType.Value);
		}
		private set
		{
			ViewState["ItemType"] = (value == null) ? null : new IndexedString(value.FullName);
		}
	}

	public bool IsTrackingViewState
	{
		get { return ((IStateManager)_viewstate).IsTrackingViewState; }
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	internal NavbarViewStateLogEntry(object saveState)
	{
		_viewstate = new StateBag();
		LoadViewState(saveState);
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	internal NavbarViewStateLogEntry(bool isTrackingState, LogItemType entryType, int? index = null, Type itemType = null)
	{
		_viewstate = new StateBag();
		if (isTrackingState)
		{
			((IStateManager)_viewstate).TrackViewState();
		}

		this.EntryType = entryType;
		if (index != null)
		{
			this.Index = index;
		}
		if (itemType != null)
		{
			this.ItemType = itemType;
		}
	}

	/// <summary>
	/// Switches tracking viewstate changes on.
	/// </summary>
	public void TrackViewState()
	{
		((IStateManager)_viewstate).TrackViewState();
	}

	/// <summary>
	/// Saves viewstate.
	/// </summary>
	public object SaveViewState()
	{
		return ((IStateManager)_viewstate).SaveViewState();
	}

	/// <summary>
	/// Loads viewstate.
	/// </summary>
	public void LoadViewState(object state)
	{
		((IStateManager)_viewstate).LoadViewState(state);
	}

	public void SetDirty()
	{
		_viewstate.SetDirty(true);
		if (!IsTrackingViewState)
		{
			TrackViewState();
		}
	}
}
