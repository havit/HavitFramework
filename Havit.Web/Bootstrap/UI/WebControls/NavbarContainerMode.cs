using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Mode of rendering container in Navbar.
	/// </summary>
	public enum NavbarContainerMode
	{
		/// <summary>
		/// Render as "container".
		/// </summary>
		Container,

		/// <summary>
		/// Render as "container-fluid".
		/// </summary>
		ContainerFluid,

		/// <summary>
		/// Do not render container.
		/// </summary>
		None
	}
}
