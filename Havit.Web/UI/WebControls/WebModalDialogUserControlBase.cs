using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Předek pro vlastní user controly, 
	/// které jsou zobrazovány jako dialog.
	/// </summary>
	public class WebModalDialogUserControlBase: UserControl
	{

		#region MainWebModalDialog
		private WebModalDialog MainWebModalDialog
		{
			get
			{
				return _mainWebModalDialog;
			}
		}
		private WebModalDialog _mainWebModalDialog;
		#endregion

		#region Constructor
		public WebModalDialogUserControlBase()
		{
			_mainWebModalDialog = new WebModalDialog();
		}
		#endregion

		#region FrameworkInitialize, AddParsedSubObject
		protected override void FrameworkInitialize()
		{
			base.FrameworkInitialize();
			this.Controls.Add(MainWebModalDialog);
		}

		/// <summary>
		/// Zajišťuje, aby se controly user controlu nevkládaly do kolece controls tohoto usercontrolu,
		/// ale do kolekce controlů MainWebModalDialog. Nedochází tak k žádnému přehazování controls ve stromu controlů, apod.
		/// </summary>
		protected override void AddParsedSubObject(object obj)
		{
			MainWebModalDialog.ContentTemplateContainer.Controls.Add((Control)obj);
		}
		#endregion
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			MainWebModalDialog.DialogShown += new EventHandler(MainWebModalDialog_DialogShown);
			MainWebModalDialog.DialogHidden += new EventHandler(MainWebModalDialog_DialogHidden);
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
				return MainWebModalDialog.DialogVisible;
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
				return MainWebModalDialog.Width;
			}
			set
			{
				MainWebModalDialog.Width = value;
			}
		}

		/// <summary>
		/// Výška dialogu v pixelech.
		/// </summary>
		public Unit Height
		{
			get
			{
				return MainWebModalDialog.Height;
			}
			set
			{
				MainWebModalDialog.Height = value;
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
			MainWebModalDialog.Show();
		}

		/// <summary>
		/// Skryje dialog.
		/// </summary>
		public void Hide()
		{
			MainWebModalDialog.Hide();
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
		/// <param name="eventArgs"></param>
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
				return MainWebModalDialog.Triggers;
			}
		}
		#endregion
	}
}
