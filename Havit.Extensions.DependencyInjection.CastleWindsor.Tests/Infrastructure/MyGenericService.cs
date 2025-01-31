using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.CastleWindsor.Tests.Infrastructure;

/// <summary>
/// Implementuje jeden interface - IService.
/// </summary>
[Service(Profile = nameof(MyGenericService<object, object>))]
public class MyGenericService<TA, TB> : IGenericService<TA, TB>
{
}
