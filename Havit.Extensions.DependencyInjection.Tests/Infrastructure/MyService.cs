using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.Tests.Infrastructure;

/// <summary>
/// Implementuje jeden interface - IService.
/// </summary>
[Service(Profile = nameof(MyService))]
public class MyService : IService
{
}
