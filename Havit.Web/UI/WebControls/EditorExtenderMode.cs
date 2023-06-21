using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Režim editace v externím editoru.
/// </summary>
public enum EditorExtenderMode
{
	/// <summary>
	/// Editace existujícího záznamu.
	/// </summary>
	Edit = 1,

	/// <summary>
	/// Vkládání nového záznamu.
	/// </summary>
	Insert = 2
}
