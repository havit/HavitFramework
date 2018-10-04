using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Modal Dialog Drag Mode.
	/// </summary>
	public enum ModalDialogDragMode
	{
		/// <summary>
		/// Drag mode is used when drag support (jQuery UI) is available on client side.
		/// </summary>
		IfAvailable,

		/// <summary>
		/// No dragging support.
		/// </summary>
		No
	}
}
