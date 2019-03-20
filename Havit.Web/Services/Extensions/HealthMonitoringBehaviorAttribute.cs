using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.Services.Extensions
{
	/// <summary>
	/// WCF service behavior, který zajišťuje funkčnost health monitoringu - přidává error handler, který zasílá chyby health monitoringu.
	/// </summary>
	public class HealthMonitoringBehaviorAttribute : Attribute, IServiceBehavior
	{
		/// <summary>
		/// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
		/// </summary>
		/// <param name="serviceDescription">The service description.</param><param name="serviceHostBase">The service host that is currently being constructed.</param>
		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			// NOOP
		}

		/// <summary>
		/// Provides the ability to pass custom data to binding elements to support the contract implementation.
		/// </summary>
		/// <param name="serviceDescription">The service description of the service.</param><param name="serviceHostBase">The host of the service.</param><param name="endpoints">The service endpoints.</param><param name="bindingParameters">Custom objects to which binding elements have access.</param>
		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
			// NOOP
		}

		/// <summary>
		/// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
		/// </summary>
		/// <param name="serviceDescription">The service description.</param><param name="serviceHostBase">The host that is currently being built.</param>
		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcherBase chanDispBase in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher channelDispatcher = chanDispBase as ChannelDispatcher;
				if (channelDispatcher == null)
				{
					continue;
				}
				channelDispatcher.ErrorHandlers.Add(new HealthMonitoringErrorHandler());
			}
		}
	}
}