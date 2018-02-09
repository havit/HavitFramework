using System;

namespace Havit.Services
{
	/// <summary>
	/// Extension methods for <c>IServiceFactory&lt;TService&gt;</c>
	/// </summary>
	public static class ServiceFactoryExtensions
	{
		/// <summary>
		/// Executes action using service from service factory. Releases the service after executing the action.
		/// </summary>
		/// <param name="serviceFactory">Service factory providing the service to be used.</param>
		/// <param name="action">Action to be executed.</param>
		/// <typeparam name="TService">type of service managed by the service factory</typeparam>
		public static void ExecuteAction<TService>(this IServiceFactory<TService> serviceFactory, Action<TService> action)
			where TService : class
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(serviceFactory != null);

			TService service = null;

			try
			{
				service = serviceFactory.CreateService();
				action(service);
			}
			finally
			{
				serviceFactory.ReleaseService(service);
			}
		}
	}
}