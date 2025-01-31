using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.Tests.Infrastructure;

/// <summary>
/// Implementuje jeden interface - IGenericService&lt;string&gt;.
/// </summary>
[Service(Profile = nameof(MyStringService<object>))]
public class MyStringService<T> : IStringService<string>
{
}
