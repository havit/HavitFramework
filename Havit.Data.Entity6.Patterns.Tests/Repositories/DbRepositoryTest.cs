using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Patterns.DataLoaders;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.Repositories
{
	[TestClass]
	public class DbRepositoryTest
	{
		[TestMethod]
		[ExpectedException(typeof(ObjectNotFoundException), AllowDerivedTypes = false)]
		public void DbRepository_GetObject_ThrowsExceptionWhenNotFound()
		{
			// Arrange
			Mock<DbSet<ItemWithDeleted>> mockDbSet = new Mock<DbSet<ItemWithDeleted>>();
			mockDbSet.Setup(m => m.Find()).Returns((ItemWithDeleted)null);
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			mockDbContext.Setup(m => m.Set<ItemWithDeleted>()).Returns(mockDbSet.Object);

			Mock<IDataLoader> mockDbDataLoader = new Mock<IDataLoader>();
			Mock<IDataLoaderAsync> mockDbDataLoaderAsync = new Mock<IDataLoaderAsync>();

			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			mockSoftDeleteManager.Setup(m => m.IsSoftDeleteSupported<ItemWithDeleted>()).Returns(false);

			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(mockDbContext.Object, mockDbDataLoader.Object, mockDbDataLoaderAsync.Object, mockSoftDeleteManager.Object);

			// Act
			repository.GetObject(1);

			// Assert by method attribute 

		}

		[TestMethod]
		public void DbRepository_GetObject_ReturnsDeletedObjects()
		{
			// Arrange
			ItemWithDeleted instanceWithDeleted = new ItemWithDeleted { Id = 1, Deleted = DateTime.Now };
            Mock<DbSet<ItemWithDeleted>> mockDbSet = new Mock<DbSet<ItemWithDeleted>>();
			mockDbSet.Setup(m => m.Find(1)).Returns(instanceWithDeleted);
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			mockDbContext.Setup(m => m.Set<ItemWithDeleted>()).Returns(mockDbSet.Object);

			Mock<IDataLoader> mockDbDataLoader = new Mock<IDataLoader>();
			Mock<IDataLoaderAsync> mockDbDataLoaderAsync = new Mock<IDataLoaderAsync>();

			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			mockSoftDeleteManager.Setup(m => m.IsSoftDeleteSupported<ItemWithDeleted>()).Returns(true);

			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(mockDbContext.Object, mockDbDataLoader.Object, mockDbDataLoaderAsync.Object, mockSoftDeleteManager.Object);

			// Act
			ItemWithDeleted repositoryResult = repository.GetObject(1);

			// Assert
			Assert.AreSame(repositoryResult, instanceWithDeleted);
		}

		[TestMethod]
		public void DbRepository_GetAll_DoesNotReturnDeletedObjects()
		{
			// Arrange
			IQueryable<ItemWithDeleted> dataQueryable = new ItemWithDeleted[]
			{
				new ItemWithDeleted { Id = 1, Deleted = DateTime.Now }
			}.AsQueryable();
			Mock<DbSet<ItemWithDeleted>> mockDbSet = new Mock<DbSet<ItemWithDeleted>>();
			Mock<IQueryable<ItemWithDeleted>> mockDbSetQueryable = mockDbSet.As<IQueryable<ItemWithDeleted>>();
			mockDbSetQueryable.Setup(m => m.Provider).Returns(dataQueryable.Provider);
			mockDbSetQueryable.Setup(m => m.ElementType).Returns(dataQueryable.ElementType);
			mockDbSetQueryable.Setup(m => m.Expression).Returns(dataQueryable.Expression);
			mockDbSetQueryable.Setup(m => m.GetEnumerator()).Returns(dataQueryable.GetEnumerator());

			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			mockDbContext.Setup(m => m.Set<ItemWithDeleted>()).Returns(mockDbSet.Object);

			Mock<IDataLoader> mockDbDataLoader = new Mock<IDataLoader>();
			Mock<IDataLoaderAsync> mockDbDataLoaderAsync = new Mock<IDataLoaderAsync>();
			DbRepository<ItemWithDeleted> repository = new DbItemWithDeletedRepository(mockDbContext.Object, mockDbDataLoader.Object, mockDbDataLoaderAsync.Object, new SoftDeleteManager(new ServerTimeService()));

			// Act
			List<ItemWithDeleted> result = repository.GetAll();

			// Assert
			Assert.AreEqual(0, result.Count);
		}
	}
}
