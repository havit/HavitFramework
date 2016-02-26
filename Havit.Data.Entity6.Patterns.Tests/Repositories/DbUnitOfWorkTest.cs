using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataLoaders;
using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
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
			mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Never);
		}

		[TestMethod]
		public async Task DbUnitOfWork_CommitAsync_CallsSaveChangesAsync()
		{
			// Arrange
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(mockDbContext.Object, mockSoftDeleteManager.Object);

			// Act
			await dbUnitOfWork.CommitAsync();

			// Assert
			mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
			mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Once);
		}

		[TestMethod]
		public void DbUnitOfWork_Commit_CallsBeforeCommitAndAfterCommitInOrder()
		{
			// Arrange
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			Mock<DbUnitOfWork> mockDbUnitOfWork = new Mock<DbUnitOfWork>(mockDbContext.Object, mockSoftDeleteManager.Object);
			mockDbUnitOfWork.CallBase = true;

			mockDbContext.Setup(m => m.SaveChanges()).Callback(() =>
			{
				mockDbUnitOfWork.Verify(m => m.BeforeCommit(), Times.Once);
				mockDbUnitOfWork.Verify(m => m.AfterCommit(), Times.Never);
			});

			// Act
			mockDbUnitOfWork.Object.Commit();

			// Assert
			mockDbUnitOfWork.Verify(m => m.BeforeCommit(), Times.Once);
			mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
			mockDbUnitOfWork.Verify(m => m.AfterCommit(), Times.Once);
		}

		[TestMethod]
		public async Task DbUnitOfWork_CommitAsync_CallsBeforeCommitAndAfterCommitInOrder()
		{
			// Arrange
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			Mock<DbUnitOfWork> mockDbUnitOfWork = new Mock<DbUnitOfWork>(mockDbContext.Object, mockSoftDeleteManager.Object);
			mockDbUnitOfWork.CallBase = true;

			mockDbContext.Setup(m => m.SaveChangesAsync())
				.Callback(() =>
				{
					mockDbUnitOfWork.Verify(m => m.BeforeCommit(), Times.Once);
					mockDbUnitOfWork.Verify(m => m.AfterCommit(), Times.Never);
				})
				.Returns(Task.CompletedTask);

			// Act
			await mockDbUnitOfWork.Object.CommitAsync();

			// Assert
			mockDbUnitOfWork.Verify(m => m.BeforeCommit(), Times.Once);
			mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Once);
			mockDbUnitOfWork.Verify(m => m.AfterCommit(), Times.Once);
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

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public async Task DbUnitOfWork_CommitAsync_ThrowsExceptionWhenAlreadyCommited()
		{
			// Arrange
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(mockDbContext.Object, mockSoftDeleteManager.Object);

			// Act
			await dbUnitOfWork.CommitAsync();
			await dbUnitOfWork.CommitAsync();

			// Assert by method attribute
		}

		[TestMethod]
		public void DbUnitOfWork_AddForInsert_EnsuresObjectIsRegistered()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.Initialize(true);

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			Language language = new Language();

			// Act
			dbUnitOfWork.AddForInsert(language);
			Changes changes = dbUnitOfWork.GetRegisteredChanges();

			// Assert
			Assert.IsTrue(changes.Inserts.Contains(language));
		}

		[TestMethod]
		public void DbUnitOfWork_AddForUpdate_EnsuresObjectIsRegistered()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.Initialize(true);

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, new SoftDeleteManager(new ServerTimeService()));
			Language language = new Language();

			// Act
			dbUnitOfWork.AddForUpdate(language);
			Changes changes = dbUnitOfWork.GetRegisteredChanges();

			// Assert
			Assert.IsTrue(changes.Updates.Contains(language));
		}

		[TestMethod]
		public void DbUnitOfWork_AddForDelete_EnsuresObjectIsRegistered()
		{
			// Arrange
			SoftDeleteManager softDeleteManager = new SoftDeleteManager(new ServerTimeService());
			Assert.IsFalse(softDeleteManager.IsSoftDeleteSupported<Language>(), "Test vyžaduje objekt, který není mazán příznakem.");
			Assert.IsTrue(softDeleteManager.IsSoftDeleteSupported<ItemWithDeleted>(), "Test vyžaduje objekt, který je mazán příznakem.");

			using (TestDbContext testDbContext = new TestDbContext())
			{
				testDbContext.Database.Initialize(true);
				testDbContext.Set<Language>().Add(new Language() { Culture = "cs-CZ", UiCulture = "cs-CZ" });
				testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
				testDbContext.SaveChanges();
			}

			using (TestDbContext testDbContext = new TestDbContext())
			{
				Language language = testDbContext.Set<Language>().Single();
				ItemWithDeleted itemWithDeleted = testDbContext.Set<ItemWithDeleted>().Single();

				DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, softDeleteManager);

				// Act
				dbUnitOfWork.AddForDelete(language);
				dbUnitOfWork.AddForDelete(itemWithDeleted);
				Changes changes = dbUnitOfWork.GetRegisteredChanges();

				// Assert
				Assert.IsFalse(changes.Updates.Contains(language));
				Assert.IsTrue(changes.Deletes.Contains(language));
				Assert.IsTrue(changes.Updates.Contains(itemWithDeleted));
				Assert.IsFalse(changes.Deletes.Contains(itemWithDeleted));
			}
		}

		[TestMethod]
		public void DbUnitOfWork_AddForUpdate_UpdatesFromNonTrackedObject()
		{
			// Arrange
			int languageId;
			using (TestDbContext testDbContext = new TestDbContext())
			{
				Language language1 = new Language() { Culture = "cs-CZ", UiCulture = "cs-CZ" };
				testDbContext.Database.Initialize(true);

				testDbContext.Set<Language>().Add(language1);
				testDbContext.SaveChanges();
				languageId = language1.Id;
			}

			Language language2 = new Language
			{
				Id = languageId,
				Culture = "cs-CZ",
				UiCulture = "en-EN"
			};

			using (TestDbContext testDbContext = new TestDbContext())
			{
				SoftDeleteManager softDeleteManager = new SoftDeleteManager(new ServerTimeService());
				DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, softDeleteManager);

				// Act
				dbUnitOfWork.AddForUpdate(language2);
				dbUnitOfWork.Commit();
			}

			// Assert
			using (TestDbContext testDbContext = new TestDbContext())
			{
				Language language = testDbContext.Set<Language>().Single();

				Assert.AreEqual(languageId, language.Id);
				Assert.AreEqual("cs-CZ", language.Culture);
				Assert.AreEqual("en-EN", language.UiCulture);
			}
		}

		[TestMethod]
		public void DbUnitOfWork_AddForInsert_DoNotFailDbRepositoryWhenMoreNewObjectsAdded()
		{
			// Arrange
			TestDbContext dbContext = new TestDbContext();
			var dbDataLoader = new DbDataLoader(dbContext);
			var softDeleteManager = new SoftDeleteManager(new ServerTimeService());
			var dbUnitOfWork = new DbUnitOfWork(dbContext, softDeleteManager);
			var dbRepository = new DbItemWithDeletedRepository(dbContext, dbDataLoader, dbDataLoader, softDeleteManager);
			Dictionary<int, ItemWithDeleted> dbRepositoryDbSetLocalsDictionary = dbRepository.DbSetLocalsDictionary;
			
			// Act
			dbUnitOfWork.AddForInsert(new ItemWithDeleted());
			dbUnitOfWork.AddForInsert(new ItemWithDeleted());

			// Assert
			Assert.AreEqual(0, dbRepositoryDbSetLocalsDictionary.Count);
		}

	}
}
