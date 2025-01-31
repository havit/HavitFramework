using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.CastleWindsor.Tests.Infrastructure;

/// <summary>
/// Implementuje jeden interface - IService.
/// </summary>
[Service(Profile = nameof(StringGenericService<object>))]
public class StringGenericService<T> : IStringGenericService<string>
{
}
