using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataLoaders;
using Havit.Data.Entity.Patterns.DataLoaders.Internal;
using Havit.Data.Entity.Patterns.DataSources.Fakes;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Entity.Patterns.UnitOfWorks;
using Havit.Data.Entity.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.Entity.Patterns.UnitOfWorks.EntityValidation;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.UnitOfWorks
{
	[TestClass]
	public class DbUnitOfWorkTests
	{
		[TestMethod]
		public void DbUnitOfWork_Commit_CallsSaveChanges()
		{
			// Arrange
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(mockDbContext.Object, mockSoftDeleteManager.Object, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

			// Act
			dbUnitOfWork.Commit();

			// Assert
			mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
			mockDbContext.Verify(m => m.SaveChangesAsync(CancellationToken.None), Times.Never);
		}

		[TestMethod]
		public async Task DbUnitOfWork_CommitAsync_CallsSaveChangesAsync()
		{
			// Arrange
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(mockDbContext.Object, mockSoftDeleteManager.Object, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

			// Act
			await dbUnitOfWork.CommitAsync();

			// Assert
			mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
			mockDbContext.Verify(m => m.SaveChangesAsync(CancellationToken.None), Times.Once);
		}

		[TestMethod]
		public void DbUnitOfWork_Commit_CanBeCalledMultipleTimes()
		{
			// Arrange
			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(new TestDbContext(), new SoftDeleteManager(new ServerTimeService()), mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

			Language language = new Language();
			language.Culture = "";
			language.UiCulture = "";

			// Act
			dbUnitOfWork.AddForInsert(language);
			dbUnitOfWork.Commit();

			dbUnitOfWork.Commit();

			// Assert
			// No exception was thrown.
		}

		[TestMethod]
		public async Task DbUnitOfWork_CommitAsync_CanBeCalledMultipleTimes()
		{
			// Arrange
			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(new TestDbContext(), new SoftDeleteManager(new ServerTimeService()), mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

			Language language = new Language();
			language.Culture = "";
			language.UiCulture = "";
			
			// Act
			dbUnitOfWork.AddForInsert(language);
			await dbUnitOfWork.CommitAsync();

			await dbUnitOfWork.CommitAsync();

			// Assert
			// No exception was thrown.
		}

		[TestMethod]
		public void DbUnitOfWork_Commit_ClearsKnownChanges()
		{
			// Arrange
			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(new TestDbContext(), new SoftDeleteManager(new ServerTimeService()), mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

			// Act
			Language language = new Language();
			language.Culture = "";
			language.UiCulture = "";
			dbUnitOfWork.AddForInsert(language);
			dbUnitOfWork.Commit();

			Changes allKnownChanges = dbUnitOfWork.GetAllKnownChanges();

			// Assert
			Assert.AreEqual(0, allKnownChanges.Deletes.Length, "Deletes contains a registered change.");
			Assert.AreEqual(0, allKnownChanges.Inserts.Length, "Inserts contains a registered change.");
			Assert.AreEqual(0, allKnownChanges.Updates.Length, "Updates contains a registered change.");
		}

		[TestMethod]
		public async Task DbUnitOfWork_CommitAsync_ClearsKnownChanges()
		{
			// Arrange
			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(new TestDbContext(), new SoftDeleteManager(new ServerTimeService()), mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

			// Act
			Language language = new Language();
			language.Culture = "";
			language.UiCulture = "";
			dbUnitOfWork.AddForInsert(language);
			await dbUnitOfWork.CommitAsync();

			Changes allKnownChanges = dbUnitOfWork.GetAllKnownChanges();

			// Assert
			Assert.AreEqual(0, allKnownChanges.Deletes.Length, "Deletes contains a registered change.");
			Assert.AreEqual(0, allKnownChanges.Inserts.Length, "Inserts contains a registered change.");
			Assert.AreEqual(0, allKnownChanges.Updates.Length, "Updates contains a registered change.");
		}

		[TestMethod]
		public void DbUnitOfWork_Commit_CallsBeforeCommitAndAfterCommitInOrder()
		{
			// Arrange
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			Mock<DbUnitOfWork> mockDbUnitOfWork = new Mock<DbUnitOfWork>(mockDbContext.Object, mockSoftDeleteManager.Object, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);
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
			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			Mock<DbUnitOfWork> mockDbUnitOfWork = new Mock<DbUnitOfWork>(mockDbContext.Object, mockSoftDeleteManager.Object, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);
			mockDbUnitOfWork.CallBase = true;

			mockDbContext.Setup(m => m.SaveChangesAsync(CancellationToken.None))
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
			mockDbContext.Verify(m => m.SaveChangesAsync(CancellationToken.None), Times.Once);
			mockDbUnitOfWork.Verify(m => m.AfterCommit(), Times.Once);
		}

		[TestMethod]
		public void DbUnitOfWork_AddForInsert_EnsuresObjectIsRegistered()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			testDbContext.Database.Initialize(true);

			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, new SoftDeleteManager(new ServerTimeService()), mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);
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

			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, new SoftDeleteManager(new ServerTimeService()), mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);
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

			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

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

				DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, softDeleteManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

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
				Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
				Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

				DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, softDeleteManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

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
			var dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			var softDeleteManager = new SoftDeleteManager(new ServerTimeService());

			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			var dbUnitOfWork = new DbUnitOfWork(dbContext, softDeleteManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);
			var dataSource = new DbItemWithDeletedDataSource(dbContext, new SoftDeleteManager(new ServerTimeService()));
			var dbRepository = new DbItemWithDeletedRepository(dbContext, dataSource, dbDataLoader, softDeleteManager);
			Dictionary<int, ItemWithDeleted> dbRepositoryDbSetLocalsDictionary = dbRepository.DbSetLocalsDictionary;
			
			// Act
			dbUnitOfWork.AddForInsert(new ItemWithDeleted());
			dbUnitOfWork.AddForInsert(new ItemWithDeleted());

			// Assert
			Assert.AreEqual(0, dbRepositoryDbSetLocalsDictionary.Count);
		}

		/// <summary>
		/// Bug 26702: AddRangeForInsert vyvolává 'System.InvalidOperationException'
		/// </summary>
		[TestMethod]
		public void DbUnitOfWork_AddForInsertRange_SupportsRangeOfObject()
		{
			// Arrange
			TestDbContext dbContext = new TestDbContext();
			var softDeleteManager = new SoftDeleteManager(new ServerTimeService());

			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			var dbUnitOfWork = new DbUnitOfWork(dbContext, softDeleteManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

			// Act
			dbUnitOfWork.AddRangeForInsert(new[] { new ItemWithDeleted(), new ItemWithDeleted(), new ItemWithDeleted() });

			// Assert
			// no exception is thrown
		}

		[TestMethod]
		public void DbUnitOfWork_AfterCommit_DoesNotRepeatEventsAfterCommit()
		{
			// Arrange
			int counter = 0;
			TestDbContext dbContext = new TestDbContext();
			var softDeleteManager = new SoftDeleteManager(new ServerTimeService());

			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			var dbUnitOfWork = new DbUnitOfWork(dbContext, softDeleteManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);
			dbUnitOfWork.RegisterAfterCommitAction(() => counter += 1);
			Debug.Assert(counter == 0);

			// Act + Assert
			dbUnitOfWork.Commit();
			Assert.AreEqual(1, counter); // došlo k zavolání zaregistrované akce

			dbUnitOfWork.Commit();
			Assert.AreEqual(1, counter); // nedošlo k zavolání zaregistrované akce, registrace byla zrušena
		}

		[TestMethod]
		public void DbUnitOfWork_AddRangeForInsert_SetsEntitiesToAddedState()
		{
			// Arrange
			ParentEntity parent = new ParentEntity { Id = 100 };
			ChildEntity child1 = new ChildEntity
			{
				Parent = parent
			};

			ChildEntity child2 = new ChildEntity
			{
				Parent = parent
			};

			parent.Children.Add(child1);
			parent.Children.Add(child2);

			TestDbContext dbContext = new TestDbContext();
			var softDeleteManager = new SoftDeleteManager(new ServerTimeService());

			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			var dbUnitOfWork = new DbUnitOfWork(dbContext, softDeleteManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

			// Act
			dbUnitOfWork.AddRangeForInsert(new ChildEntity[] { child1, child2 });

			// Assert
			Assert.AreEqual(EntityState.Added, ((IDbContext)dbContext).GetEntityState(child1));
			Assert.AreEqual(EntityState.Added, ((IDbContext)dbContext).GetEntityState(child2));
		}

		[TestMethod]
		public void DbUnitOfWork_AddRangeForUpdate_SetsEntitiesToModifiedState()
		{
			// Arrange
			ParentEntity parent = new ParentEntity { Id = 100 };
			ChildEntity child1 = new ChildEntity
			{
				Id = 1,
				Parent = parent
			};

			ChildEntity child2 = new ChildEntity
			{
				Id = 2,
				Parent = parent
			};

			parent.Children.Add(child1);
			parent.Children.Add(child2);

			TestDbContext dbContext = new TestDbContext();
			var softDeleteManager = new SoftDeleteManager(new ServerTimeService());

			Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
			Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();

			var dbUnitOfWork = new DbUnitOfWork(dbContext, softDeleteManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object);

			// Act
			dbUnitOfWork.AddRangeForUpdate(new ChildEntity[] { child1, child2 });

			// Assert
			Assert.AreEqual(EntityState.Modified, ((IDbContext)dbContext).GetEntityState(child1));
			Assert.AreEqual(EntityState.Modified, ((IDbContext)dbContext).GetEntityState(child2));
		}
	}
}
