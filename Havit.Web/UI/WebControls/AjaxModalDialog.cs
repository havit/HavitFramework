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
	public class AjaxModalDialog : BasicModalDialog
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
		/// <summary>
		/// Konstuktor.
		/// </summary>
		public AjaxModalDialog()
		{
			_updatePanel = new UpdatePanelExt();
			_updatePanel.ID = "__UP";
			_updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

			_contentPlaceHolder = new PlaceHolder();
			_contentPlaceHolder.ID = "__CPH";
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Inicializuje podstrom controlů.
		/// </summary>
		protected override void CreateChildControls()
		{
			_updatePanel.ContentTemplateContainer.Controls.Add(_contentPlaceHolder);			
			_basicContainer = base.GetContentContainer();
			_basicContainer.Controls.Add(_updatePanel);
			base.CreateChildControls();
		}
		#endregion

		#region GetContentContainer
		/// <summary>
		/// Vrací control, který je kontejnerem, do kterého se bude instanciovat šablona obsahu.
		/// </summary>
		/// <returns></returns>
		protected override Control GetContentContainer()
		{
			return _contentPlaceHolder;
		}
		#endregion

		#region OnDialogShown
		/// <summary>
		/// Obsluhuje událost zobrazení dialogu.
		/// Provádí update vnořeného UpdatePanelu.
		/// </summary>
		/// <param name="eventArgs"></param>
		protected override void OnDialogShown(EventArgs eventArgs)
		{
			base.OnDialogShown(eventArgs);
			_updatePanel.Update();
		}
		#endregion

		#region OnDialogHidden
		/// <summary>
		/// Obsluhuje událost skrytí dialogu.
		/// Provádí update vnořeného UpdatePanelu.
		/// </summary>
		/// <param name="eventArgs"></param>
		protected override void OnDialogHidden(EventArgs eventArgs)
		{
			base.OnDialogHidden(eventArgs);
			_updatePanel.Update();
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// PreRender.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			_contentPlaceHolder.Visible = DialogVisible;
		}
		#endregion

		#region CheckDialogSize
		/// <summary>
		/// Ověří zadání velikosti dialogu.
		/// Kontrola probíhá jen, pokud je dialog zobrazen.
		/// </summary>
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
