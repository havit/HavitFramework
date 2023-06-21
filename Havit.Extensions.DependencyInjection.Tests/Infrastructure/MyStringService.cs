using Havit.Extensions.DependencyInjection.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Extensions.DependencyInjection.Tests.Infrastructure;

/// <summary>
/// Implementuje jeden interface - IGenericService&lt;string&gt;.
/// </summary>
[Service(Profile = nameof(MyStringService<object>))]
public class MyStringService<T> : IStringService<string>
{
}
