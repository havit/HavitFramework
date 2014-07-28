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
	/// Extends ModalDialogUserControlBase by persisting dialog Result value.
	/// </summary>
	public class ModalDialogUserControlBase<T> : ModalDialogUserControlBase
	{
		#region OnDialogShown
		/// <summary>
		/// Sets result value to type T default value.
		/// </summary>
		protected override void OnDialogShown(EventArgs eventArgs)
		{
			base.OnDialogShown(eventArgs);
			Result = default(T);
		}
		#endregion

		#region Result
		/// <summary>
		/// User result of modal dialog activity.
		/// Result value is automatically reset when the dialog in shown, it is set to the type T default value.
		/// Value is persisted in ViewState (so it must be serializable).
		/// </summary>
		public T Result
		{
			get
			{
				return (T)(ViewState["Result"] ?? default(T));
			}
			set
			{
				ViewState["Result"] = value;
			}
		}
		#endregion
	}
}
