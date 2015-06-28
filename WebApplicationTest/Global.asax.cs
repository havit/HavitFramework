using System;
using System.Collections.Generic;
using System.Web;
using Havit.Web.Security;
using Havit.Web.UI.WebControls;
using Havit.WebApplicationTest;
using Havit.WebApplicationTest.App_Start;

namespace Havit.WebApplicationTest
{
	public class Global : System.Web.HttpApplication
	{
		#region Application_Start
		protected void Application_Start(object sender, EventArgs e)
		{
			DateTimeBox.GetDateTimeBoxCustomizationDefault += new DateTimeBox.DateTimeBoxDateCustomizationEventHandler(this.DateTimeBox_GetDateTimeBoxCustomizationDefaults);
			ScriptManagerConfig.RegisterScriptResourceMappings();
			WindsorCastleConfig.RegisterDiContainer();

			Messenger.StorageType = MessengerStorageType.Cookies;
		}
		#endregion

		#region IdentityMapScope management (Application_PostAcquireRequestState, Application_PreRequestHandlerExecute, Application_EndRequest)
		protected void Application_PostAcquireRequestState(object sender, EventArgs e)
		{
			this.Context.Items["IdentityMapScope"] = new Havit.Business.IdentityMapScope();
		}

		protected void Application_EndRequest(object sender, EventArgs e)
		{
			Havit.Business.IdentityMapScope identityMapScope = this.Context.Items["IdentityMapScope"] as Havit.Business.IdentityMapScope;
			if (identityMapScope != null)
			{
				identityMapScope.Dispose();
			}
		}
		#endregion

		#region Application_Error
		private void Application_Error(object sender, EventArgs e)
		{
			Exception exception = this.Server.GetLastError();
			if (exception != null)
			{
				Havit.Web.Management.WebRequestErrorEventExt customEvent = new Havit.Web.Management.WebRequestErrorEventExt(exception.Message, this, exception, HttpContext.Current);
				customEvent.Raise();
			}
		}
		#endregion

		#region Application_AuthenticateRequest
		private void Application_AuthenticateRequest()
		{
			FormsRolesAuthentication.ApplyAuthenticationTicket();
		}
		#endregion

		#region DateTimeBox_GetDateTimeBoxCustomizationDefaults
		private void DateTimeBox_GetDateTimeBoxCustomizationDefaults(object sender, DateTimeBoxDateCustomizationEventArgs args)
		{
			if (this.specialDatesDefault == null)
			{
				List<SpecialDate> specialDatesList = new List<SpecialDate>()
				{
					new SpecialDate(new DateTime(2012, 1, 1), false, "special"), 
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

				this.specialDatesDefault = new SpecialDateCustomization(specialDatesList);
			}
			args.DateCustomization = this.specialDatesDefault;
		}
		private SpecialDateCustomization specialDatesDefault;
		#endregion
	}
}