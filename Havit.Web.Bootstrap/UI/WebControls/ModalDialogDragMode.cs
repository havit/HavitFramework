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
		/// Drag mode is required.
		/// Requires script resource definition "jquery.ui.combined".
		/// </summary>
		Required,
		
		/// <summary>
		/// Drag mode is used when drag support is available.
		/// Uses script resource definition "jquery.ui.combined".
		/// </summary>
		IfAvailable,

		/// <summary>
		/// No dragging support.
		/// </summary>
		No
	}
}
