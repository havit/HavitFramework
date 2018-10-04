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
	/// Dialog s podporou Ajaxu. Forma je taková, že dialog zapouzdřuje UpdatePanel.
	/// </summary>
	public class AjaxModalDialog : BasicModalDialog
	{
		#region Private fiels
		private readonly UpdatePanel _updatePanel;
		private readonly PlaceHolder _contentPlaceHolder;
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
			_updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

			_contentPlaceHolder = new PlaceHolder();
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Inicializuje podstrom controlů.
		/// </summary>
		protected override void CreateChildControls()
		{
			_updatePanel.ID = this.ID + "__UP";
			_contentPlaceHolder.ID = this.ID + "__CPH";

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
			_contentPlaceHolder.Visible = DialogVisible;
			base.OnPreRender(e);
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
