using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataSeeds.Infrastructure;

internal class LambdaDbContextFactory : IDbContextFactory
{
	private readonly Func<IDbContext> factoryFunc;

	public LambdaDbContextFactory(Func<IDbContext> factoryFunc)
	{
		this.factoryFunc = factoryFunc;
	}

	public IDbContext CreateDbContext()
	{
		return factoryFunc();
	}
}
