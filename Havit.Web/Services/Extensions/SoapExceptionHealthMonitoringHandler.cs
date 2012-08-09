using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Management;
using System.Web.Services.Protocols;
using Havit.Web.Management;
using System.Web;

namespace Havit.Web.Services.Extensions
{
	/// <summary>
	/// V případě chyby ve zpracování web metody (webové služby) zajistí oznámení chyby health monitoringem.
	/// Pozor, toto nefunguje (a chyby healthmonitoringu tak nejsou oznamovány),
	/// pokud se webové služby testují v browseru!!! Pro testování nutno použít skutečného klienta webové služby (třeba service reference v konzolovce).
	/// </summary>
	public class SoapExceptionHealthMonitoringHandler : System.Web.Services.Protocols.SoapExtension
	{
		#region ProcessMessage
		public override void ProcessMessage(System.Web.Services.Protocols.SoapMessage message)
		{
			try
			{
				if ((message != null) && (message.Stage == SoapMessageStage.AfterSerialize))
				{
					if (message.Exception != null)
					{
						Exception exception = message.Exception;
						if ((exception is SoapException) && (exception.InnerException != null))
						{
							exception = exception.InnerException;
						}
						if ((exception is HttpUnhandledException) && (exception.InnerException != null))
						{
							exception = exception.InnerException;
						}
						new WebRequestErrorEventExt(exception.Message, message, exception).Raise();
					}
				}
			}
			catch // pokud by zde nedejbože došlo k nějaké další výjimce, tak ji zamaskujeme
			{
				// NOOP
			}
		}
		#endregion

		#region GetInitializer
		public override object GetInitializer(Type serviceType)
		{
			return null;
		}

		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return null;
		}
		#endregion

		#region Initialize
		public override void Initialize(object initializer)
		{
		}
		#endregion
	}

}
