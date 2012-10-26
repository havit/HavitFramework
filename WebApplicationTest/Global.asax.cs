using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using Havit.Web.UI.WebControls;

namespace WebApplicationTest
{
	public class Global : System.Web.HttpApplication
	{
		SpecialDateCustomization specialDatesDefault;

		protected void Application_Start(object sender, EventArgs e)
		{
			
			DateTimeBox.GetDateTimeBoxCustomizationDefault += new DateTimeBox.DateTimeBoxDateCustomizationEventHandler(DateTimeBox_GetDateTimeBoxCustomizationDefaults);

		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		#region Application_Error
		private void Application_Error(object sender, EventArgs e)
		{
			Exception exception = Server.GetLastError();
			if (exception != null)
			{
				Havit.Web.Management.WebRequestErrorEventExt customEvent = new Havit.Web.Management.WebRequestErrorEventExt(exception.Message, this, exception, HttpContext.Current);
				customEvent.Raise();
			}
		}
		#endregion

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}

		private void DateTimeBox_GetDateTimeBoxCustomizationDefaults(object sender, DateTimeBoxDateCustomizationEventArgs args)
		{
			if (specialDatesDefault == null)
			{
				List<SpecialDate> specialDatesList = new List<SpecialDate>()
				{	new SpecialDate(new DateTime(2012, 1, 1), false, "special"), 
					new SpecialDate(new DateTime(2012, 2, 12), false, "special"),
					new SpecialDate(new DateTime(2012, 4, 9), false, "special"),
					new SpecialDate(new DateTime(2012, 5, 1), false, "special"),
					new SpecialDate(new DateTime(2012, 5, 8), true, String.Empty),
					new SpecialDate(new DateTime(2012, 7, 5), false, "special"),
					new SpecialDate(new DateTime(2012, 7, 6), false, "special"),
					new SpecialDate(new DateTime(2012, 9, 28), false, "special"),
					new SpecialDate(new DateTime(2012, 10, 28), false, "special"),
					new SpecialDate(new DateTime(2012, 11, 17), false, "special"),
					new SpecialDate(new DateTime(2012, 12, 24), false, "special"),
					new SpecialDate(new DateTime(2012, 12, 25), false, "special"),
					new SpecialDate(new DateTime(2012, 12, 26), false, "special"),
					new SpecialDate(new DateTime(2013, 1, 1), false, "special")
				};

				specialDatesDefault = new SpecialDateCustomization(specialDatesList);
			}
			args.DateCustomization = specialDatesDefault;
		}
	}
}