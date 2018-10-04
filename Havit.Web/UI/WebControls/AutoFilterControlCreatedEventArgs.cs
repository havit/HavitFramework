using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Argument pro událost AutoFilterControlCreated.
	/// </summary>
	public class AutoFilterControlCreatedEventArgs : EventArgs
	{
		#region Empty
		/// <summary>
		/// Parametr bez hodnot.
		/// </summary>
		public new static readonly AutoFilterControlCreatedEventArgs Empty = new AutoFilterControlCreatedEventArgs();
		#endregion
	}
}
