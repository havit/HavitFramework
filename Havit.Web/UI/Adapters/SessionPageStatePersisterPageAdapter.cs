using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.Adapters;
using System.Web.UI;

namespace Havit.Web.UI.Adapters
{
	/// <summary>
	/// <see cref="PageAdapter"/>, který jako <see cref="PageStatePersister"/> v metodě <see cref="GetStatePersister()"/>
	/// vrací <see cref="SessionPageStatePersister"/>.
	/// </summary>
	public class SessionPageStatePersisterPageAdapter : PageAdapter
	{
		#region GetStatePersister
		/// <summary>
		/// Returns an object that is used by the Web page to maintain the control and view states. 
		/// </summary>
		/// <returns>
		/// An object derived from <see cref="PageStatePersister"/> that supports creating and extracting the combined control and view states for the <see cref="Page"/>.
		/// </returns>
		public override PageStatePersister GetStatePersister()
		{
			return new SessionPageStatePersister(this.Page);
		}
		#endregion
	}
}
