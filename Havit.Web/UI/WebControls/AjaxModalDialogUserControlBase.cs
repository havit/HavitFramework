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
	public abstract class AjaxModalDialogUserControlBase : ModalDialogUserControlBase
	{
		/// <summary>
		/// Zapouzdřený ModalDialog zajišťující zobrazování a schovávání obsahu stránky.
		/// </summary>
		private AjaxModalDialog AjaxModalDialog
		{
			get { return _ajaxModalDialog; }
		}
		private readonly AjaxModalDialog _ajaxModalDialog = new AjaxModalDialog();

		/// <summary>
		/// Triggery zapouzdřeného UpdatePanelu.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public UpdatePanelTriggerCollection Triggers
		{
			get
			{
				return AjaxModalDialog.Triggers;
			}
		}

		/// <summary>
		/// Šířka dialogu v pixelech.
		/// </summary>
		public Unit Width
		{
			get
			{
				return AjaxModalDialog.Width;
			}
			set
			{
				AjaxModalDialog.Width = value;
			}
		}

		/// <summary>
		/// Výška dialogu v pixelech.
		/// </summary>
		public Unit Height
		{
			get
			{
				return AjaxModalDialog.Height;
			}
			set
			{
				AjaxModalDialog.Height = value;
			}
		}

		/// <summary>
		/// Zapouzdřený ModalDialog zajišťující zobrazování a schovávání obsahu dialogu.
		/// </summary>
		protected override sealed ModalDialogBase GetModalDialogControl()
		{
			return AjaxModalDialog;
		}

		/// <summary>
		/// FrameworkInitialize.
		/// </summary>
		protected override void FrameworkInitialize()
		{
			base.FrameworkInitialize();
			this.Controls.Add(AjaxModalDialog);
		}

		/// <summary>
		/// Zajišťuje, aby se controly user controlu nevkládaly do kolece controls tohoto usercontrolu,
		/// ale do kolekce controlů MainWebModalDialog. Nedochází tak k žádnému přehazování controls ve stromu controlů, apod.
		/// </summary>
		protected override void AddParsedSubObject(object obj)
		{
			AjaxModalDialog.ContentTemplateContainer.Controls.Add((Control)obj);
		}
	}
}
