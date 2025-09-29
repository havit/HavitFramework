using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests;

internal class NoCacheModelCacheKeyFactory : IModelCacheKeyFactory
{
	public object Create(Microsoft.EntityFrameworkCore.DbContext context, bool designTime) => context is TestDbContext
		? (context, designTime).GetHashCode()
		: context.GetHashCode();
}