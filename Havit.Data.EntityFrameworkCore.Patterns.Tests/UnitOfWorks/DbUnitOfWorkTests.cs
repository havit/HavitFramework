using System.Diagnostics;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.UnitOfWorks;

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
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(mockDbContext.Object, mockSoftDeleteManager.Object, new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

		// Act
		dbUnitOfWork.Commit();

		// Assert
		mockDbContext.Verify(m => m.SaveChanges(true), Times.Once);
		mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
		mockDbContext.Verify(m => m.SaveChangesAsync(true, It.IsAny<CancellationToken>()), Times.Never);
		mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
	}

	[TestMethod]
	public async Task DbUnitOfWork_CommitAsync_CallsSaveChangesAsync()
	{
		// Arrange
		Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
		Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
		Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(mockDbContext.Object, mockSoftDeleteManager.Object, new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

		// Act
		await dbUnitOfWork.CommitAsync();

		// Assert
		mockDbContext.Verify(m => m.SaveChanges(true), Times.Never);
		mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
		mockDbContext.Verify(m => m.SaveChangesAsync(true, It.IsAny<CancellationToken>()), Times.Once);
		mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
	}

	[TestMethod]
	public void DbUnitOfWork_Commit_CanBeCalledMultipleTimes()
	{
		// Arrange
		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
		Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(new TestDbContext(), new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

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
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(new TestDbContext(), new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

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
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(new TestDbContext(), new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

		// Act
		Language language = new Language();
		language.Culture = "";
		language.UiCulture = "";
		dbUnitOfWork.AddForInsert(language);
		dbUnitOfWork.Commit();

		Changes allKnownChanges = dbUnitOfWork.GetAllKnownChanges();

		// Assert
		Assert.AreEqual(0, allKnownChanges.Items.Count(), "Changes contains a registered change.");
	}

	[TestMethod]
	public async Task DbUnitOfWork_CommitAsync_ClearsKnownChanges()
	{
		// Arrange
		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
		Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(new TestDbContext(), new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

		// Act
		Language language = new Language();
		language.Culture = "";
		language.UiCulture = "";
		dbUnitOfWork.AddForInsert(language);
		await dbUnitOfWork.CommitAsync();

		Changes allKnownChanges = dbUnitOfWork.GetAllKnownChanges();

		// Assert
		Assert.AreEqual(0, allKnownChanges.Items.Count(), "Changes contains a registered change.");
	}

	[TestMethod]
	public void DbUnitOfWork_Commit_CallsBeforeCommitAndAfterCommitInOrder()
	{
		// Arrange
		Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
		Mock<ISoftDeleteManager> mockSoftDeleteManager = new Mock<ISoftDeleteManager>();
		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
		Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		Mock<DbUnitOfWork> mockDbUnitOfWork = new Mock<DbUnitOfWork>(mockDbContext.Object, mockSoftDeleteManager.Object, new NoCachingEntityCacheManager(), CreateEntityCacheDependencyManager(), mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));
		mockDbUnitOfWork.CallBase = true;

		mockDbContext.Setup(m => m.SaveChanges(true)).Callback(() =>
		{
			mockDbUnitOfWork.Verify(m => m.BeforeCommit(), Times.Once);
			mockDbUnitOfWork.Verify(m => m.AfterCommit(), Times.Never);
		});

		// Act
		mockDbUnitOfWork.Object.Commit();

		// Assert
		mockDbUnitOfWork.Verify(m => m.BeforeCommit(), Times.Once);
		mockDbContext.Verify(m => m.SaveChanges(true), Times.Once);
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

		Mock<DbUnitOfWork> mockDbUnitOfWork = new Mock<DbUnitOfWork>(mockDbContext.Object, mockSoftDeleteManager.Object, new NoCachingEntityCacheManager(), CreateEntityCacheDependencyManager(), mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));
		mockDbUnitOfWork.CallBase = true;

		mockDbContext.Setup(m => m.SaveChangesAsync(true, It.IsAny<CancellationToken>()))
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
		mockDbContext.Verify(m => m.SaveChangesAsync(true, It.IsAny<CancellationToken>()), Times.Once);
		mockDbUnitOfWork.Verify(m => m.AfterCommit(), Times.Once);
	}

	[TestMethod]
	public void DbUnitOfWork_AddForInsert_EnsuresObjectIsRegistered()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
		Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));
		Language language = new Language();

		// Act
		dbUnitOfWork.AddForInsert(language);

		// Assert
		Assert.AreEqual(EntityState.Added, ((IDbContext)testDbContext).GetEntityState(language));
	}

	[TestMethod]
	public void DbUnitOfWork_AddRangeForInsert_EnsuresObjectIsRegistered()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
		Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));
		Language language = new Language();

		// Act
		dbUnitOfWork.AddRangeForInsert([language]);

		// Assert
		Assert.AreEqual(EntityState.Added, ((IDbContext)testDbContext).GetEntityState(language));
	}

	[TestMethod]
	public void DbUnitOfWork_AddForUpdate_EnsuresObjectIsRegistered()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
		Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));
		Language language = new Language { Id = 100 };

		// Act
		dbUnitOfWork.AddForUpdate(language);

		// Assert
		Assert.AreEqual(EntityState.Modified, ((IDbContext)testDbContext).GetEntityState(language));
	}

	[TestMethod]
	public void DbUnitOfWork_AddRangeForUpdate_EnsuresObjectIsRegistered()
	{
		// Arrange
		TestDbContext testDbContext = new TestDbContext();
		testDbContext.Database.DropCreate();

		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
		Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));
		Language language = new Language { Id = 100 };

		// Act
		dbUnitOfWork.AddRangeForUpdate([language]);

		// Assert
		Assert.AreEqual(EntityState.Modified, ((IDbContext)testDbContext).GetEntityState(language));
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
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		using (TestDbContext testDbContext = new TestDbContext())
		{
			testDbContext.Database.DropCreate();
			testDbContext.Set<Language>().Add(new Language() { Culture = "cs-CZ", UiCulture = "cs-CZ" });
			testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
			testDbContext.SaveChanges();
		}

		using (TestDbContext testDbContext = new TestDbContext())
		{
			Language language = testDbContext.Set<Language>().Single();
			ItemWithDeleted itemWithDeleted = testDbContext.Set<ItemWithDeleted>().Single();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, softDeleteManager, new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

			// Act
			dbUnitOfWork.AddForDelete(language);
			dbUnitOfWork.AddForDelete(itemWithDeleted);

			// Assert
			testDbContext.ChangeTracker.DetectChanges();
			Assert.AreEqual(EntityState.Deleted, ((IDbContext)testDbContext).GetEntityState(language));
			Assert.AreEqual(EntityState.Modified, ((IDbContext)testDbContext).GetEntityState(itemWithDeleted));
		}
	}

	[TestMethod]
	public void DbUnitOfWork_AddRangeForDelete_EnsuresObjectIsRegistered()
	{
		// Arrange
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(new ServerTimeService());
		Assert.IsFalse(softDeleteManager.IsSoftDeleteSupported<Language>(), "Test vyžaduje objekt, který není mazán příznakem.");
		Assert.IsTrue(softDeleteManager.IsSoftDeleteSupported<ItemWithDeleted>(), "Test vyžaduje objekt, který je mazán příznakem.");

		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
		Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		using (TestDbContext testDbContext = new TestDbContext())
		{
			testDbContext.Database.DropCreate();
			testDbContext.Set<Language>().Add(new Language() { Culture = "cs-CZ", UiCulture = "cs-CZ" });
			testDbContext.Set<ItemWithDeleted>().Add(new ItemWithDeleted());
			testDbContext.SaveChanges();
		}

		using (TestDbContext testDbContext = new TestDbContext())
		{
			Language language = testDbContext.Set<Language>().Single();
			ItemWithDeleted itemWithDeleted = testDbContext.Set<ItemWithDeleted>().Single();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, softDeleteManager, new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

			// Act
			dbUnitOfWork.AddRangeForDelete([language]);
			dbUnitOfWork.AddRangeForDelete([itemWithDeleted]);

			// Assert
			testDbContext.ChangeTracker.DetectChanges();
			Assert.AreEqual(EntityState.Deleted, ((IDbContext)testDbContext).GetEntityState(language));
			Assert.AreEqual(EntityState.Modified, ((IDbContext)testDbContext).GetEntityState(itemWithDeleted));
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
			testDbContext.Database.DropCreate();

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
			IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

			DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(testDbContext, softDeleteManager, new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

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
	public void DbUnitOfWork_Clear()
	{
		// Arrange
		Mock<IBeforeCommitProcessorsRunner> mockBeforeCommitProcessorsRunner = new Mock<IBeforeCommitProcessorsRunner>();
		Mock<IEntityValidationRunner> mockEntityValidationRunner = new Mock<IEntityValidationRunner>();
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		DbUnitOfWork dbUnitOfWork = new DbUnitOfWork(new TestDbContext(), new SoftDeleteManager(new ServerTimeService()), new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));

		Language language = new Language();
		language.Id = 1;
		language.Culture = "";
		language.UiCulture = "";
		dbUnitOfWork.AddForUpdate(language);
		dbUnitOfWork.RegisterAfterCommitAction(() => { /* something */ });

		// Prerequisities
		Assert.AreEqual(1, dbUnitOfWork.GetAllKnownChanges().Items.Count());
		Assert.AreEqual(1, dbUnitOfWork._afterCommits.Count);
		Assert.AreEqual(1, dbUnitOfWork.DbContext.GetEntries(suppressDetectChanges: false).Count());

		// Act
		dbUnitOfWork.Clear();

		// Assert
		Assert.AreEqual(0, dbUnitOfWork.GetAllKnownChanges().Items.Count());
		Assert.AreEqual(0, dbUnitOfWork._updateRegistrations.Count);
		Assert.IsNull(dbUnitOfWork._afterCommits);
		Assert.AreEqual(0, dbUnitOfWork.DbContext.GetEntries(suppressDetectChanges: false).Count());
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
		IEntityCacheDependencyManager entityCacheDependencyManager = CreateEntityCacheDependencyManager();

		var dbUnitOfWork = new DbUnitOfWork(dbContext, softDeleteManager, new NoCachingEntityCacheManager(), entityCacheDependencyManager, mockBeforeCommitProcessorsRunner.Object, mockEntityValidationRunner.Object, new LookupDataInvalidationRunner(Enumerable.Empty<ILookupDataInvalidationService>()));
		dbUnitOfWork.RegisterAfterCommitAction(() => counter += 1);
		Debug.Assert(counter == 0);

		// Act + Assert
		dbUnitOfWork.Commit();
		Assert.AreEqual(1, counter); // došlo k zavolání zaregistrované akce

		dbUnitOfWork.Commit();
		Assert.AreEqual(1, counter); // nedošlo k zavolání zaregistrované akce, registrace byla zrušena
	}

	private IEntityCacheDependencyManager CreateEntityCacheDependencyManager()
	{
		Mock<IEntityCacheDependencyManager> mockEntityCacheDependencyManager = new Mock<IEntityCacheDependencyManager>(MockBehavior.Strict);
		mockEntityCacheDependencyManager.Setup(m => m.PrepareCacheInvalidation(It.IsAny<Changes>())).Returns((CacheInvalidationOperation)null);

		return mockEntityCacheDependencyManager.Object;
	}

}
