using System;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers;

public static class EfCoreAccessor
{
	private static IServiceProvider serviceProvider;

	static EfCoreAccessor()
	{
		InitEfCore();
	}

	private static void InitEfCore()
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddEntityFrameworkSqlServer();
		serviceProvider = new DefaultServiceProviderFactory().CreateServiceProvider(serviceCollection);
	}

	public static TResult Use<T, TResult>(Func<T, TResult> func)
	{
		using (IServiceScope scope = serviceProvider.CreateScope())
		{
			var service = scope.ServiceProvider.GetService<T>();

			return func(service);
		}
	}
}