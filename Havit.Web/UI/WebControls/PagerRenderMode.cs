using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Režim renderování pageru GridViewExt.
	/// </summary>
	public enum PagerRenderMode
	{
		/// <summary>
		/// Standardní renderování.
		/// </summary>
		Standard,

		/// <summary>
		/// Renderování pomocí bootstrao pagination (struktura ul/li).
		/// </summary>
		BootstrapPagination
	}
}
