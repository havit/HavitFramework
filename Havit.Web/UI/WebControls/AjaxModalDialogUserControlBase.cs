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
	public class AjaxModalDialogUserControlBase : UserControl
	{

		#region MainWebModalDialog
		/// <summary>
		/// Zapouzdřený AjaxModalDialog.
		/// </summary>
		private AjaxModalDialog MainAjaxModalDialog
		{
			get
			{
				return _mainAjaxModalDialog;
			}
		}
		private AjaxModalDialog _mainAjaxModalDialog;
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AjaxModalDialogUserControlBase()
		{
			_mainAjaxModalDialog = new AjaxModalDialog();
		}
		#endregion

		#region FrameworkInitialize, AddParsedSubObject
		/// <summary>
		/// FrameworkInitialize.
		/// </summary>
		protected override void FrameworkInitialize()
		{
			base.FrameworkInitialize();
			this.Controls.Add(MainAjaxModalDialog);
		}

		/// <summary>
		/// Zajišťuje, aby se controly user controlu nevkládaly do kolece controls tohoto usercontrolu,
		/// ale do kolekce controlů MainWebModalDialog. Nedochází tak k žádnému přehazování controls ve stromu controlů, apod.
		/// </summary>
		protected override void AddParsedSubObject(object obj)
		{
			MainAjaxModalDialog.ContentTemplateContainer.Controls.Add((Control)obj);
		}
		#endregion

		#region OnInit
		/// <summary>
		/// OnInit.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			MainAjaxModalDialog.DialogShown += new EventHandler(MainWebModalDialog_DialogShown);
			MainAjaxModalDialog.DialogHidden += new EventHandler(MainWebModalDialog_DialogHidden);
		}
		#endregion
		
		#region DialogVisible
		/// <summary>
		/// Udává, zda je dialog viditelný.
		/// </summary>
		protected bool DialogVisible
		{
			get
			{
				return MainAjaxModalDialog.DialogVisible;
			}
		}
		#endregion

		#region Width, Height
		/// <summary>
		/// Šířka dialogu v pixelech.
		/// </summary>
		public Unit Width
		{
			get
			{
				return MainAjaxModalDialog.Width;
			}
			set
			{
				MainAjaxModalDialog.Width = value;
			}
		}

		/// <summary>
		/// Výška dialogu v pixelech.
		/// </summary>
		public Unit Height
		{
			get
			{
				return MainAjaxModalDialog.Height;
			}
			set
			{
				MainAjaxModalDialog.Height = value;
			}
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
			MainAjaxModalDialog.Show();
		}

		/// <summary>
		/// Skryje dialog.
		/// </summary>
		public void Hide()
		{
			MainAjaxModalDialog.Hide();
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
		
		#region Triggers
		/// <summary>
		/// Triggery zapouzdřeného UpdatePanelu.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public UpdatePanelTriggerCollection Triggers
		{
			get
			{
				return MainAjaxModalDialog.Triggers;
			}
		}
		#endregion
	}
}
