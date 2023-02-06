using System;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Tests
{
	[TestClass]
	public class DbContextTests
	{
		/// <summary>
		/// Ověřuje počet volání metody AfterSaveChanges po SaveChanges.
		/// Cílem je ověřit, zda je správně ošetřeno volání SaveChanges(bool acceptAllChangesOnSuccess) z SaveChanges().
		/// </summary>
		[TestMethod]
		public void DbContext_SaveChanges_CallsAfterSaveChangesOnlyOnce()
		{
			// Arrange
			Mock<EmptyDbContext> dbContextMock1 = new Mock<EmptyDbContext>();
			dbContextMock1.CallBase = true;

			Mock<EmptyDbContext> dbContextMock2 = new Mock<EmptyDbContext>();
			dbContextMock2.CallBase = true;

			// Act
			dbContextMock1.Object.SaveChanges();
			dbContextMock2.Object.SaveChanges(true);

			// Assert
			dbContextMock1.Verify(m => m.AfterSaveChanges(), Times.Once, "dbContextMock1");
			dbContextMock2.Verify(m => m.AfterSaveChanges(), Times.Once, "dbContextMock2");
		}

		/// <summary>
		/// Ověřuje počet volání metody AfterSaveChanges po SaveChangesAsync.
		/// Cílem je ověřit, zda je správně ošetřeno volání SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken) z SaveChangesAsync(CancellationToken cancellationToken).
		/// </summary>
		[TestMethod]
		public async Task DbContext_SaveChangesAsync_CallsAfterSaveChangesOnlyOnce()
		{
			// Arrange
			Mock<EmptyDbContext> dbContextMock1 = new Mock<EmptyDbContext>();
			dbContextMock1.CallBase = true;

			Mock<EmptyDbContext> dbContextMock2 = new Mock<EmptyDbContext>();
			dbContextMock2.CallBase = true;

			// Act
			await dbContextMock1.Object.SaveChangesAsync();
			await dbContextMock2.Object.SaveChangesAsync(true);

			// Assert
			dbContextMock1.Verify(m => m.AfterSaveChanges(), Times.Once, "dbContextMock1");
			dbContextMock2.Verify(m => m.AfterSaveChanges(), Times.Once, "dbContextMock2");
		}

		/// <summary>
		/// Ověřuje počet volání registrované akce po SaveChanges, cílem je, aby nebyla registrovaná akce spuštěna opakovaně (z více volání SaveChanges).
		/// </summary>
		[TestMethod]
		public void DbContext_AfterSaveChanges_CallsRegisteresAfterSaveChangesActionsOnlyOnce()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();
			int counter = 0;

			// Act + Assert
			dbContext.RegisterAfterSaveChangesAction(() => counter += 1);

			dbContext.AfterSaveChanges();
			Assert.AreEqual(1, counter); // došlo k zaregistrované akci

			dbContext.AfterSaveChanges();
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

		[TestMethod]
		public void DbContext_UsesDbLockedMigrator()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();

			// Act
			var migrator = ((IInfrastructure<IServiceProvider>)dbContext).GetService<IMigrator>();

			// Assert
#pragma warning disable EF1001 // Internal EF Core API usage.
			Assert.IsInstanceOfType(migrator, typeof(Havit.Data.EntityFrameworkCore.Migrations.Internal.DbLockedMigrator));
#pragma warning restore EF1001 // Internal EF Core API usage.
		}
	}
}
