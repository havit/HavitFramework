using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.Tests.Infrastructure;

[Service(Profile = nameof(MyFirstAndSecondScopedService), Lifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped, ServiceTypes = new[] { typeof(IFirstService), typeof(ISecondService) })]
public class MyFirstAndSecondScopedService : IFirstService, ISecondService
{
}
