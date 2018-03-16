using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.EFCore.Tests.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EFCore.Tests
{
	[TestClass]
	public class DbContextTests
	{
		[TestMethod]
		public void DbContext_SaveChanges_CallsRegisteresAfterSaveChangesActionsOnlyOnce()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();
			int counter = 0;

			// Act + Assert
			dbContext.RegisterAfterSaveChangesAction(() => counter += 1);

			dbContext.SaveChanges();
			Assert.AreEqual(1, counter); // došlo k zaregistrované akci

			dbContext.SaveChanges();
			Assert.AreEqual(1, counter); // nedošlo k zaregistrované akci, registrace zrušena
		}

		[TestMethod]
		public async Task DbContext_SaveChangesAsync_CallsRegisteresAfterSaveChangesActionsOnlyOnce()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();
			int counter = 0;

			// Act + Assert
			dbContext.RegisterAfterSaveChangesAction(() => counter += 1);

			await dbContext.SaveChangesAsync();
			Assert.AreEqual(1, counter); // došlo k zaregistrované akci

			await dbContext.SaveChangesAsync();
			Assert.AreEqual(1, counter); // nedošlo k zaregistrované akci, registrace zrušena
		}
		
		[TestMethod]
		public void DbContext_ExecuteWithDbUpdateExceptionHandling_WrapsDbUpdateException()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();
			DbUpdateException exception = new DbUpdateException("", (Exception)null);

			// Act 
			DbUpdateException thrownException = null;
			try
			{
				dbContext.ExecuteWithDbUpdateExceptionHandling<object>(() => throw exception);
			}
			catch (DbUpdateException dbUpdateException)
			{
				thrownException = dbUpdateException;
			}

			// Assert
			Assert.IsNotNull(thrownException);
			Assert.AreSame(exception, thrownException.InnerException);
		}
	}
}
