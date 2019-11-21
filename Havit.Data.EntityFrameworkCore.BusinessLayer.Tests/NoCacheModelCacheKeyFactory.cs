using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests
{
    internal class NoCacheModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(Microsoft.EntityFrameworkCore.DbContext context) => context.GetHashCode();
    }
}