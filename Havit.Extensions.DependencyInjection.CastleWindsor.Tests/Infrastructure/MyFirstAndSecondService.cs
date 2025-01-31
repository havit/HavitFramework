using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.CastleWindsor.Tests.Infrastructure;

[Service(Profile = nameof(MyFirstAndSecondService), Lifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, ServiceTypes = new[] { typeof(IFirstService), typeof(ISecondService) })]
public class MyFirstAndSecondService : IFirstService, ISecondService
{
}
