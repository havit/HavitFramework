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
	public abstract class AjaxModalDialogUserControlBase : DialogUserControlBase
	{
		protected override ModalDialogBase CreateModalDialog()
		{
			return new AjaxModalDialog();
		}

		protected new AjaxModalDialog MainModalDialog
		{
			get
			{
				return (AjaxModalDialog)base.MainModalDialog;
			}
		}

		#region Triggers
		/// <summary>
		/// Triggery zapouzdřeného UpdatePanelu.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public UpdatePanelTriggerCollection Triggers
		{
			get
			{
				return MainModalDialog.Triggers;
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
				return MainModalDialog.Width;
			}
			set
			{
				MainModalDialog.Width = value;
			}
		}

		/// <summary>
		/// Výška dialogu v pixelech.
		/// </summary>
		public Unit Height
		{
			get
			{
				return MainModalDialog.Height;
			}
			set
			{
				MainModalDialog.Height = value;
			}
		}
		#endregion

		#region FrameworkInitialize, AddParsedSubObject
		/// <summary>
		/// FrameworkInitialize.
		/// </summary>
		protected override void FrameworkInitialize()
		{
			base.FrameworkInitialize();
			this.Controls.Add(MainModalDialog);
		}

		/// <summary>
		/// Zajišťuje, aby se controly user controlu nevkládaly do kolece controls tohoto usercontrolu,
		/// ale do kolekce controlů MainWebModalDialog. Nedochází tak k žádnému přehazování controls ve stromu controlů, apod.
		/// </summary>
		protected override void AddParsedSubObject(object obj)
		{
			MainModalDialog.ContentTemplateContainer.Controls.Add((Control)obj);
		}
		#endregion

	}
}
