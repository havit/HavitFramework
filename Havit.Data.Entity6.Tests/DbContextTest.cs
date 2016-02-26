using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Tests.Infrastructure;
using Havit.Data.Entity.Tests.Validators.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Tests
{
	[TestClass]
	public class DbContextTest
	{
		[TestMethod]
		public void DbContext_SaveChanges_CallsRegisteresAfterSaveChangesActions()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();
			bool called = false;

			// Act
			dbContext.RegisterAfterSaveChangesAction(() => called = true);
			dbContext.SaveChanges();

			// Assert
			Assert.IsTrue(called);
		}

		[TestMethod]
		public async Task DbContext_SaveChangesAsync_CallsRegisteresAfterSaveChangesActions()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();
			bool called = false;

			// Act
			dbContext.RegisterAfterSaveChangesAction(() => called = true);
			await dbContext.SaveChangesAsync();

			// Assert
			Assert.IsTrue(called);
		}
	}
}
