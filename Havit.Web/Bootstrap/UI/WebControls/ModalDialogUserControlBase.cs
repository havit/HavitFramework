using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// User Control for dialog behavior and operation.
	/// Must contain exacly one Havit.Web.Bootstrap.UI.WebControls.ModalDialog control.
	/// </summary>
	public abstract class ModalDialogUserControlBase : Havit.Web.UI.WebControls.ModalDialogUserControlBase
	{
		#region ModalDialog
		/// <summary>
		/// Modal dialog control handling dialog behavior and operations.
		/// </summary>
		private ModalDialog ModalDialog
		{
			get
			{
				if (_modalDialog == null)
				{
					ModalDialog[] dialogs = Controls.OfType<Havit.Web.Bootstrap.UI.WebControls.ModalDialog>().ToArray();
					if (dialogs.Length == 0)
					{
						throw new HttpException(String.Format("Havit.Web.Bootstrap.UI.WebControls.ModalDialogUserControlBase '{0}' does not contain Havit.Web.Bootstrap.UI.WebControls.ModalDialog.", this.ID));
					}
					if (dialogs.Length > 1)
					{
						throw new HttpException(String.Format("Havit.Web.Bootstrap.UI.WebControls.ModalDialogUserControlBase '{0}' contains more then one Havit.Web.Bootstrap.UI.WebControls.ModalDialog.", this.ID));
					}
					_modalDialog = dialogs[0];
				}
				return _modalDialog;
			}
		}
		private ModalDialog _modalDialog;
		#endregion

		#region Triggers
		/// <summary>
		/// Nested UpdatePanel's Triggers.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public UpdatePanelTriggerCollection Triggers
		{
			get
			{
				return ModalDialog.Triggers;
			}
		}
		#endregion

		#region GetModalDialogControl
		/// <summary>
		/// Return nested ModalDialog handling dialog behavior and appereance.
		/// </summary>
		protected sealed override ModalDialogBase GetModalDialogControl()
		{
			return ModalDialog;
		}
		#endregion
	}
}
