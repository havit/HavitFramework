using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Volba controlu pro automatický filter.
	/// </summary>
	public enum AutoFilterMode
	{
		/// <summary>
		/// No filter.
		/// </summary>
		None,

		/// <summary>
		/// DropDownList filter.
		/// </summary>
		DropDownList,

		/// <summary>
		/// TextBox filter.
		/// </summary>
		TextBox
	}
}
