using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.Tutorial.Templates
{
	public partial class Main : System.Web.UI.MasterPage
	{
		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			TopPH.Visible = TopCPH.HasControls();
		}
		#endregion
	}
}