using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader
{
	[TestClass]
	public class DbDataLoader_Caching_Tests
	{
		/// <summary>
		/// Cílem je ověřit, že dojde k fixupu při použití objektu z cache.
		/// </summary>
		[TestMethod]
		public void DbDataLoader_Load_GetsObjectFromCache()
		{			
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			LoginAccount loginAccount = new LoginAccount { Id = 1 };
			Membership membership = new Membership { LoginAccountId = 1, RoleId = 1 };
			Role role = new Role { Id = 1 };

			Mock<IEntityCacheManager> entityCacheManagerMock = new Mock<IEntityCacheManager>(MockBehavior.Strict);
			entityCacheManagerMock.Setup(m => m.TryGetEntity<Role>(1, out role)).Callback(() => dbContext.Set<Role>().Attach(role)).Returns(true); // roli zaregistrujeme do dbContextu až při odbavování z cache (jinak neplatí precondition)

			DbDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), entityCacheManagerMock.Object);
			dbContext.Update(loginAccount);
			dbContext.Update(membership);			

			// Act
			Assert.IsNull(membership.Role); // precondition - v tento okamžik musí být nenačtená, chceme ověřit dočítání
			dataLoader.Load(membership, m => m.Role);

			// Assert
			Assert.AreSame(role, membership.Role); // je použit objekt z cache
			Assert.IsTrue(dbContext.Entry(membership).Reference(nameof(Membership.Role)).IsLoaded); // vlastnost je označena jako načtená
			entityCacheManagerMock.Verify(m => m.TryGetEntity<Role>(1, out role), Times.Once); // použil se objekt z cache
		}

		/// <summary>
		/// Cílem je ověřit, že dojde k fixupu při použití objektu z cache.
		/// </summary>
		[TestMethod]
		public async Task DbDataLoader_LoadAsync_GetsObjectFromCache()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			LoginAccount loginAccount = new LoginAccount { Id = 1 };
			Membership membership = new Membership { LoginAccountId = 1, RoleId = 1 };
			Role role = new Role { Id = 1 };

			Mock<IEntityCacheManager> entityCacheManagerMock = new Mock<IEntityCacheManager>(MockBehavior.Strict);
			entityCacheManagerMock.Setup(m => m.TryGetEntity<Role>(1, out role)).Callback(() => dbContext.Set<Role>().Attach(role)).Returns(true); // roli zaregistrujeme do dbContextu až při odbavování z cache (jinak neplatí precondition)

			DbDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), entityCacheManagerMock.Object);
			dbContext.Update(loginAccount);
			dbContext.Update(membership);

			// Act
			Assert.IsNull(membership.Role); // precondition - v tento okamžik musí být nenačtená, chceme ověřit dočítání
			await dataLoader.LoadAsync(membership, m => m.Role);

			// Assert
			Assert.AreSame(role, membership.Role); // je použit objekt z cache
			Assert.IsTrue(dbContext.Entry(membership).Reference(nameof(Membership.Role)).IsLoaded); // vlastnost je označena jako načtená
			entityCacheManagerMock.Verify(m => m.TryGetEntity<Role>(1, out role), Times.Once); // použil se objekt z cache
		}
	}
}
