using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.CastleWindsor.Tests.Infrastructure;

[Service(Profile = nameof(NoInterfaceService))]
public class NoInterfaceService
{
}
