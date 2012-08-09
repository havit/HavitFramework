using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Interface, který implementují controly, když chtìjí být nositelem skinovatelných properties pro CommandField.
	/// Implementuje napø. <see cref="GridViewExt"/>, aby pøes GridView mohl být skinován jeho CommandField.
	/// </summary>
	public interface ICommandFieldStyle
	{
		CommandFieldStyle CommandFieldStyle { get; }
	}
}
