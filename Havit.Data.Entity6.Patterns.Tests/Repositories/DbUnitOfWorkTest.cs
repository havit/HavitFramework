using System;
using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity.Patterns.UnitOfWorks;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.Repositories
{
	[TestClass]
	public class DbUnitOfWorkTest
	{
		[TestMethod]
		public void DbUnitOfWork_Commit_CallsSaveChanges()
		{
			// Arrange
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(mockDbContext.Object, mockSoftDeleteManager.Object);

			// Act
			dbUnitOfWork.Commit();

			// Assert
			mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DbUnitOfWork_Commit_ThrowsExceptionWhenAlreadyCommited()
		{
			// Arrange
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(mockDbContext.Object, mockSoftDeleteManager.Object);

			// Act
			dbUnitOfWork.Commit();
			dbUnitOfWork.Commit();

			// Assert by method attribute
		}
	}
}
