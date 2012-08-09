using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Web;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Dialog, který zapouzdřuje UpdatePanel.
	/// </summary>
	public class WebModalDialog : BasicModalDialog
	{
		#region Private fiels
		private UpdatePanel _updatePanel;
		private PlaceHolder _contentPlaceHolder;
		private Control _basicContainer;
		#endregion

		#region Triggers
		/// <summary>
		/// Triggery zapouzdřeného UpdatePanelu.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public UpdatePanelTriggerCollection Triggers
		{
			get
			{
				return _updatePanel.Triggers;
			}
		}
		#endregion

		#region Constructor
		public WebModalDialog()
		{
			_updatePanel = new UpdatePanel();
			_updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

			_contentPlaceHolder = new PlaceHolder();
		}
		#endregion

		#region CreateChildControls
		protected override void CreateChildControls()
		{
			_updatePanel.ContentTemplateContainer.Controls.Add(_contentPlaceHolder);			
			_basicContainer = base.GetContentContainer();
			_basicContainer.Controls.Add(_updatePanel);
			base.CreateChildControls();
		}
		#endregion

		#region GetContentContainer
		protected override Control GetContentContainer()
		{
			return _contentPlaceHolder;
		}
		#endregion

		#region OnDialogShown
		protected override void OnDialogShown(EventArgs eventArgs)
		{
			base.OnDialogShown(eventArgs);
			_updatePanel.Update();
		}
		#endregion

		#region OnDialogHidden
		protected override void OnDialogHidden(EventArgs eventArgs)
		{
			base.OnDialogHidden(eventArgs);
			//_updatePanel.Update();
		}
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			_contentPlaceHolder.Visible = DialogVisible;
		}
		#endregion

		#region CheckDialogSize
		protected override void CheckDialogSize()
		{
			// velikost kontrolujeme pouze u zobrazeného dialogu
			if (DialogVisible)
			{
				base.CheckDialogSize();
			}
		}
		#endregion
	}
}
