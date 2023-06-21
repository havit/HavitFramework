using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Havit.Web.Services.Extensions;

/// <summary>
/// WCF error handler, který zapisuje vzniklé výjimky do health monitoringu
/// </summary>
internal class HealthMonitoringErrorHandler : IErrorHandler
{
	/// <summary>
	/// Enables the creation of a custom <see cref="T:System.ServiceModel.FaultException`1"/> that is returned from an exception in the course of a service method.
	/// </summary>
	/// <param name="error">The <see cref="T:System.Exception"/> object thrown in the course of the service operation.</param><param name="version">The SOAP version of the message.</param><param name="fault">The <see cref="T:System.ServiceModel.Channels.Message"/> object that is returned to the client, or service, in the duplex case.</param>
	public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
	{
		// NOOP
	}

	/// <summary>
	/// Enables error-related processing and returns a value that indicates whether the dispatcher aborts the session and the instance context in certain cases. 
	/// </summary>
	/// <returns>
	/// true if Windows Communication Foundation (WCF) should not abort the session (if there is one) and instance context if the instance context is not <see cref="F:System.ServiceModel.InstanceContextMode.Single"/>; otherwise, false. The default is false.
	/// </returns>
	/// <param name="error">The exception thrown during processing.</param>
	public bool HandleError(Exception error)
	{
		try
		{
			Havit.Web.Management.WebRequestErrorEventExt customEvent = new Havit.Web.Management.WebRequestErrorEventExt(error.Message, this, error, HttpContext.Current);
			customEvent.Raise();
		}
		catch (Exception)
		{
			// NOOP
		}
		return false;
	}
}
