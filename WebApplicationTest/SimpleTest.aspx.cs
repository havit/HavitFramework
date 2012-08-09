using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit;

namespace WebApplicationTest
{
	public partial class SimpleTest : System.Web.UI.Page
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			try
			{
				DataBinderExt.GetValue("Hello", "Length.A.B");
			}
			catch (HttpException ex)
			{
				Response.Write(ex.ToString());
				Response.End();
			}

		}
	}
}