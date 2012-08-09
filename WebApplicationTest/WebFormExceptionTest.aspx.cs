using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationTest
{
	public partial class WebFormExceptionTest : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			DoExceptionButton.Click += new EventHandler(DoExceptionButton_Click);

		}

		void DoExceptionButton_Click(object sender, EventArgs e)
		{
			throw new ApplicationException("Umyslna chyba.");
		}
	}
}