using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Management;

namespace Havit.Web.Management
{
	/// <summary>
	/// Pomocná třída pro oznámení události health monitoringem.
	/// </summary>
	public class WebRequestErrorEventExt : WebRequestErrorEvent
	{
		public WebRequestErrorEventExt(string message, object eventSource, Exception exception)
			: base(message, eventSource, WebEventCodes.WebExtendedBase + 999, exception)
		{
		}
	}
}
