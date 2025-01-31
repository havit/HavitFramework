using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.CastleWindsor.Tests.Infrastructure;

/// <summary>
/// Implementuje dva interfaces - IFirstService a předka IService.
/// </summary>
[Service(Profile = nameof(MyFirstService))]
internal class MyFirstService : IFirstService
{
}
