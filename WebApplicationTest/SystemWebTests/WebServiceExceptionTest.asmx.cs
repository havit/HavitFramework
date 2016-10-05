using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Havit.WebApplicationTest.SystemWebTests
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class WebServiceExceptionTest : System.Web.Services.WebService
	{
		[WebMethod]
		public void DoException()
		{
			throw new ApplicationException("Zkoušíme HealtMonitoring v ASMX.");
		}
	}
}
