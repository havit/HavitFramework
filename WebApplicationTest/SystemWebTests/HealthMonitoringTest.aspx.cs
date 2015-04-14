using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationTest.SystemWebTests
{
	public partial class HealthMonitoringTest : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			DoExceptionButton.Click += DoExceptionButton_Click;
		}

		private void DoExceptionButton_Click(object sender, EventArgs e)
		{
			throw new ApplicationException("Zkoušíme HealtMonitoring.");
		}
	}
}