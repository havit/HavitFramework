using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls.Infrastructure
{
	/// <summary>
	/// Used for tracking changes of NavbarItemCollection for persisting Navbar/NavbarSection items.
	/// </summary>	
	internal class NavbarViewStateLogEntry : IStateManager
	{
		#region ViewState
		protected StateBag ViewState
		{
			get
			{
				return _viewstate;
			}
		}
		private readonly StateBag _viewstate;
		#endregion

		#region EntryType
		public LogItemType EntryType
		{
			get { return (LogItemType)ViewState["EntryType"]; }
			private set { ViewState["EntryType"] = value; }
		}
		#endregion

		#region Index
		public int? Index
		{
			get { return (int?)ViewState["Index"]; }
			private set { ViewState["Index"] = value; }
		}
		#endregion

		#region ItemType
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
		#endregion

		#region IsTrackingViewState
		public bool IsTrackingViewState
		{
			get { return ((IStateManager)_viewstate).IsTrackingViewState; }
		}
		#endregion

		#region Constructors
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
		#endregion

		#region TrackViewState, SaveViewState, LoadViewState
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
		#endregion

		#region SetDirty
		public void SetDirty()
		{
			_viewstate.SetDirty(true);
			if (!IsTrackingViewState)
			{
				TrackViewState();
			}
		}
		#endregion
	}
}
