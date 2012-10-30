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
		/// <summary>
		/// When overridden in a derived class, allows a SOAP extension to receive a <see cref="T:System.Web.Services.Protocols.SoapMessage"/> to process at each <see cref="T:System.Web.Services.Protocols.SoapMessageStage"/>.
		/// </summary>
		/// <param name="message">The <see cref="T:System.Web.Services.Protocols.SoapMessage"/> to process. </param>
		public override void ProcessMessage(System.Web.Services.Protocols.SoapMessage message)
		{
			try
			{
				if ((message != null) && (message.Stage == SoapMessageStage.AfterSerialize))
				{
					if (message.Exception != null)
					{						
						new WebRequestErrorEventExt(message.Exception.Message, message, message.Exception, HttpContext.Current).Raise();
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
		/// <summary>
		/// When overridden in a derived class, allows a SOAP extension to initialize data specific to a class implementing an XML Web service at a one time performance cost.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Object"/> that the SOAP extension initializes for caching.
		/// </returns>
		/// <param name="serviceType">The type of the class implementing the XML Web service to which the SOAP extension is applied. </param>
		public override object GetInitializer(Type serviceType)
		{
			return null;
		}

		/// <summary>
		/// When overridden in a derived class, allows a SOAP extension to initialize data specific to an XML Web service method using an attribute applied to the XML Web service method at a one time performance cost.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Object"/> that the SOAP extension initializes for caching.
		/// </returns>
		/// <param name="methodInfo">A <see cref="T:System.Web.Services.Protocols.LogicalMethodInfo"/> representing the specific function prototype for the XML Web service method to which the SOAP extension is applied. </param><param name="attribute">The <see cref="T:System.Web.Services.Protocols.SoapExtensionAttribute"/> applied to the XML Web service method. </param>
		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return null;
		}
		#endregion

		#region Initialize
		/// <summary>
		/// When overridden in a derived class, allows a SOAP extension to initialize itself using the data cached in the <see cref="M:System.Web.Services.Protocols.SoapExtension.GetInitializer(System.Web.Services.Protocols.LogicalMethodInfo,System.Web.Services.Protocols.SoapExtensionAttribute)"/> method.
		/// </summary>
		/// <param name="initializer">The <see cref="T:System.Object"/> returned from <see cref="M:System.Web.Services.Protocols.SoapExtension.GetInitializer(System.Web.Services.Protocols.LogicalMethodInfo,System.Web.Services.Protocols.SoapExtensionAttribute)"/> cached by ASP.NET. </param>
		public override void Initialize(object initializer)
		{
		}
		#endregion
	}

}
