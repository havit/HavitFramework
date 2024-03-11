using Havit.Extensions.DependencyInjection.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Extensions.DependencyInjection.Tests.Infrastructure;

[Service(Profile = nameof(MyFirstAndSecondScopedService), Lifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped, ServiceTypes = new[] { typeof(IFirstService), typeof(ISecondService) })]
public class MyFirstAndSecondScopedService : IFirstService, ISecondService
{
}
