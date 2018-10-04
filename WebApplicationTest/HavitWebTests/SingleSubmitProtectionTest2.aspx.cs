using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest
{
	public partial class SingleSubmitProtectionTest2 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Thread.Sleep(2500);
		}
	}
}