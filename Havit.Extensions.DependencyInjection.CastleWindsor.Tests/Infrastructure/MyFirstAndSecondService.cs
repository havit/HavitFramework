using Havit.Extensions.DependencyInjection.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Extensions.DependencyInjection.CastleWindsor.Tests.Infrastructure;

[Service(Profile = nameof(MyFirstAndSecondService), Lifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, ServiceTypes = new[] { typeof(IFirstService), typeof(ISecondService) })]
public class MyFirstAndSecondService : IFirstService, ISecondService
{
}
