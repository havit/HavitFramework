using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Předek pro vlastní user controly, 
	/// které jsou zobrazovány jako dialog prostřednictvím zapouzdřeného AjaxModalDialogu.
	/// </summary>
	public abstract class DialogUserControlBase : UserControl
	{
		protected abstract ModalDialogBase CreateModalDialog();

		#region MainWebModalDialog
		/// <summary>
		/// Zapouzdřený ModalDialog.
		/// </summary>
		protected ModalDialogBase MainModalDialog
		{
			get
			{
				if (_mainAjaxModalDialog == null)
				{
					_mainAjaxModalDialog = CreateModalDialog();
				}
				return _mainAjaxModalDialog;
			}
		}

		private ModalDialogBase _mainAjaxModalDialog;
		#endregion

		#region DialogVisible
		/// <summary>
		/// Udává, zda je dialog viditelný.
		/// </summary>
		protected bool DialogVisible
		{
			get
			{
				return MainModalDialog.DialogVisible;
			}
		}
		#endregion




		#region OnInit
		/// <summary>
		/// OnInit.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			MainModalDialog.DialogShown += new EventHandler(MainWebModalDialog_DialogShown);
			MainModalDialog.DialogHidden += new EventHandler(MainWebModalDialog_DialogHidden);
		}
		#endregion
		
		#region DialogShown, DialogHidden
		/// <summary>
		/// Událost oznamující zobrazení dialogu.
		/// </summary>
		public event EventHandler DialogShown;

		/// <summary>
		/// Událost oznamujíxí skrytí dialogu.
		/// </summary>
		public event EventHandler DialogHidden;
		#endregion

		#region MainWebModalDialog_DialogShown, MainWebModalDialog_DialogHidden
		private void MainWebModalDialog_DialogShown(object sender, EventArgs e)
		{
			OnDialogShown(e);
		}

		private void MainWebModalDialog_DialogHidden(object sender, EventArgs e)
		{
			OnDialogHidden(e);
		}
		#endregion

		#region Show, Hide, OnDialogShown, OnDialogHidden
		/// <summary>
		/// Zobrazí dialog.
		/// </summary>
		public void Show()
		{
			MainModalDialog.Show();
		}

		/// <summary>
		/// Skryje dialog.
		/// </summary>
		public void Hide()
		{
			MainModalDialog.Hide();
		}

		/// <summary>
		/// Obsluhuje událost zobrazení dialogu.
		/// </summary>
		protected virtual void OnDialogShown(EventArgs eventArgs)
		{
			if (DialogShown != null)
			{
				DialogShown(this, eventArgs);
			}
		}

		/// <summary>
		/// Obsluhuje událost skrytí dialogu.
		/// </summary>
		protected virtual void OnDialogHidden(EventArgs eventArgs)
		{
			if (DialogHidden != null)
			{
				DialogHidden(this, eventArgs);
			}
		}
		#endregion
	}
}
