using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Havit.Services.SignalR.Tests.Caching;

internal static class ServiceProviderExtensions
{
	public static THostedService GetHostedService<THostedService>(this IServiceProvider sp)
		where THostedService : IHostedService
	{
		var hostedServices = sp.GetService<IEnumerable<IHostedService>>();
		return hostedServices.OfType<THostedService>().FirstOrDefault();
	}
}
