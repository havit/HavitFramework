using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.Tests.Infrastructure;

[Service<DecoratedGenericServiceAttributeService>(Profile = nameof(DecoratedGenericServiceAttributeService))]
public class DecoratedGenericServiceAttributeService
{
}
