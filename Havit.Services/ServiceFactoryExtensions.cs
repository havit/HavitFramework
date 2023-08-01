using Havit.Diagnostics.Contracts;

namespace Havit.Services;

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
		Contract.Requires(serviceFactory != null);

		using (var service = serviceFactory.CreateDisposableService())
		{
			action(service);
		}
	}

	/// <summary>
	/// Creates a service from factory, wrapping the release logic into IDisposable pattern.
	/// </summary>
	/// <param name="serviceFactory">Service factory providing the service to be used.</param>
	/// <returns>Disposable wrapper.</returns>
	/// <typeparam name="TService">type of service managed by the service factory</typeparam>
	public static ServiceFactoryDisposableWrapper<TService> CreateDisposableService<TService>(this IServiceFactory<TService> serviceFactory)
		where TService : class
	{
		Contract.Requires(serviceFactory != null);

		return new ServiceFactoryDisposableWrapper<TService>(serviceFactory);
	}
}