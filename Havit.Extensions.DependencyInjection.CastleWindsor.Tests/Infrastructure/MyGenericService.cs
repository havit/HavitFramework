using Havit.Extensions.DependencyInjection.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Extensions.DependencyInjection.CastleWindsor.Tests.Infrastructure
{
	/// <summary>
	/// Implementuje jeden interface - IService.
	/// </summary>
	[Service(Profile = nameof(MyGenericService<object, object>))]
	public class MyGenericService<TA, TB> : IGenericService<TA, TB>
	{
	}
}
