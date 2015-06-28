using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.SystemWebTests
{
	public partial class HealthMonitoringTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			DoExceptionButton.Click += DoExceptionButton_Click;
		}
		#endregion

		#region DoExceptionButton_Click
		private void DoExceptionButton_Click(object sender, EventArgs e)
		{
			throw new ApplicationException("Zkoušíme HealtMonitoring.");
		}
		#endregion
	}
}