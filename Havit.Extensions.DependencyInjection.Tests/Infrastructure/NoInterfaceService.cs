using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.Tests.Infrastructure;

[Service(Profile = nameof(NoInterfaceService))]
public class NoInterfaceService
{
}
