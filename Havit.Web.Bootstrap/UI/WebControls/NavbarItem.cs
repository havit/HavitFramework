using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Abstract class for navbar items.
	/// </summary>
	public abstract class NavbarItem: IStateManager
	{
		#region ID
		/// <summary>
		/// Item ID.
		/// (enableds to generate property in ASP.NET codebehind designer file).
		/// </summary>
		public string ID { get; set; }
		#endregion

		#region ViewState
		/// <summary>
		/// ViewState for storing item state.
		/// </summary>
		protected StateBag ViewState
		{
			get
			{
				if (_viewstate == null)
				{
					_viewstate = new StateBag();
					if (_isTrackingViewState)
					{
						((IStateManager)_viewstate).TrackViewState();
					}
				}
				return _viewstate;
			}
		}
		private StateBag _viewstate;
		#endregion

		#region Visible
		/// <summary>
		/// When false, item is hidden (not rendered) not rendered.
		/// </summary>
		public bool Visible
		{
			get
			{
				return (bool)(ViewState["Visible"] ?? true);
			}
			set
			{
				ViewState["Visible"] = value;
			}
		}
		#endregion

		#region VisibleFunc
		/// <summary>
		/// When function returns false, item is hidden (not rendered).
		/// </summary>
		public Func<bool> VisibleFunc { get; set; }
		#endregion

		#region IsDecoration
		/// <summary>
		/// Returns true when item is a decoration.		
		/// </summary>
		public abstract bool IsDecoration { get; }
		#endregion

		#region IsVisible
		/// <summary>
		/// Returns true if item should be rendered. Includes evaluation of Visible property and VisibleFunc delegate.
		/// </summary>
		public virtual bool IsVisible
		{
			get { return Visible && ((VisibleFunc == null) || VisibleFunc()); }
		}
		#endregion

		#region SetDirty
		
		internal virtual void SetDirty()
		{
			this.ViewState.SetDirty(true);
		}
		#endregion

		#region Render
		/// <summary>
		/// Renders navbar item.
		/// </summary>
		public abstract void Render(HtmlTextWriter writer, bool showCaret, int nestingLevel);
		#endregion

		#region IStateManager interface implementation
		/// <summary>
		/// Gets an indicator whether tracking viewstate changes is on.
		/// </summary>
		public bool IsTrackingViewState
		{
			get { return _isTrackingViewState; }
		}
		private bool _isTrackingViewState = false;

		/// <summary>
		/// Switches tracking viewstate changes on.
		/// </summary>
		public virtual void TrackViewState()
		{
			_isTrackingViewState = true;
			if (_viewstate != null)
			{
				((IStateManager)_viewstate).TrackViewState();
			}
		}

		/// <summary>
		/// Saves viewstate.
		/// </summary>
		public virtual object SaveViewState()
		{
			return (_viewstate == null) ? null : ((IStateManager)_viewstate).SaveViewState();
		}

		/// <summary>
		/// Loads viewstate.
		/// </summary>
		public virtual void LoadViewState(object state)
		{
			if (state != null)
			{
				((IStateManager)ViewState).LoadViewState(state);
			}
		}
		#endregion

	}
}
