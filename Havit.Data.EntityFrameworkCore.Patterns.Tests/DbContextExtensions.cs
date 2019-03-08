using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests
{
    /// <summary>
    /// Extension metody k DbContext.
    /// </summary>
    internal static class DbContextExtensions
    {
        public static Mock<IDbContextFactory> CreateDbContextFactoryMock(this IDbContext dbContext)
        {
            Mock<IDbContextFactory> dbContextFactoryMock = new Mock<IDbContextFactory>();
            dbContextFactoryMock.Setup(m => m.CreateService()).Returns(dbContext);
            dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

            return dbContextFactoryMock;
        }

        public static IDbContextFactory CreateDbContextFactory(this IDbContext dbContext)
        {
            return dbContext.CreateDbContextFactoryMock().Object;
        }
    }

}
