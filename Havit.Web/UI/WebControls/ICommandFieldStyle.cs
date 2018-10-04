using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Interface, který implementují controly, když chtějí být nositelem skinovatelných properties pro CommandField.
	/// Implementuje např. <see cref="GridViewExt"/>, aby přes GridView mohl být skinován jeho CommandField.
	/// </summary>
	public interface ICommandFieldStyle
	{
		/// <summary>
		/// CommandFieldStyle.
		/// </summary>
		CommandFieldStyle CommandFieldStyle { get; }
	}
}
