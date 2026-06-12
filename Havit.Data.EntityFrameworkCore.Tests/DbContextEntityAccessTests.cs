using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Havit.Data.EntityFrameworkCore.Tests;

// Testy služeb DbContextu publikovaných přes IDbContext (přístup k navigacím, stavu entit, entry a DbSetům).
[TestClass]
public class DbContextEntityAccessTests
{
	[TestMethod]
	public void DbContext_IsNavigationLoaded_ReturnsFalseForUntrackedEntity()
	{
		// Arrange
		IDbContext dbContext = new EntityAccessTestDbContext();
		Master master = new Master { Id = 1 };

		// Act + Assert
		// Netrackovaná entita - navigaci nepovažujeme za načtenou.
		Assert.IsFalse(dbContext.IsNavigationLoaded(master, nameof(Master.Children)));
	}

	[TestMethod]
	public void DbContext_MarkNavigationAsLoaded_CollectionNavigation_MarksAsLoaded()
	{
		// Arrange
		EntityAccessTestDbContext dbContext = new EntityAccessTestDbContext();
		IDbContext iDbContext = dbContext;
		Master master = new Master { Id = 1 };
		dbContext.Attach(master);

		// Act + Assert
		Assert.IsFalse(iDbContext.IsNavigationLoaded(master, nameof(Master.Children))); // před označením není kolekce načtena
		iDbContext.MarkNavigationAsLoaded(master, nameof(Master.Children));
		Assert.IsTrue(iDbContext.IsNavigationLoaded(master, nameof(Master.Children))); // po označení je kolekce načtena
	}

	[TestMethod]
	public void DbContext_MarkNavigationAsLoaded_ReferenceNavigation_MarksAsLoaded()
	{
		// Arrange
		EntityAccessTestDbContext dbContext = new EntityAccessTestDbContext();
		IDbContext iDbContext = dbContext;
		Child child = new Child { Id = 1, MasterId = 1 };
		dbContext.Attach(child);

		// Act + Assert
		Assert.IsFalse(iDbContext.IsNavigationLoaded(child, nameof(Child.Master))); // před označením není reference načtena
		iDbContext.MarkNavigationAsLoaded(child, nameof(Child.Master));
		Assert.IsTrue(iDbContext.IsNavigationLoaded(child, nameof(Child.Master))); // po označení je reference načtena
	}

	[TestMethod]
	public void DbContext_MarkNavigationAsLoaded_SkipNavigation_MarksAsLoaded()
	{
		// Arrange
		EntityAccessTestDbContext dbContext = new EntityAccessTestDbContext();
		IDbContext iDbContext = dbContext;
		Master master = new Master { Id = 1 };
		dbContext.Attach(master);

		// Act + Assert
		// Tags je skip navigation (M:N), pro kterou MarkNavigationAsLoaded/IsNavigationLoaded používá FindSkipNavigation.
		Assert.IsFalse(iDbContext.IsNavigationLoaded(master, nameof(Master.Tags))); // před označením není načtena
		iDbContext.MarkNavigationAsLoaded(master, nameof(Master.Tags));
		Assert.IsTrue(iDbContext.IsNavigationLoaded(master, nameof(Master.Tags))); // po označení je načtena
	}

	[TestMethod]
	public void DbContext_MarkNavigationAsLoaded_UnknownNavigation_Throws()
	{
		// Arrange
		IDbContext dbContext = new EntityAccessTestDbContext();
		Master master = new Master { Id = 1 };

		// Act + Assert
		Assert.ThrowsExactly<InvalidOperationException>(() => dbContext.MarkNavigationAsLoaded(master, "NonExistentNavigation"));
	}

	[TestMethod]
	public void DbContext_GetEntityState_ReturnsDetachedForUntrackedEntity()
	{
		// Arrange
		IDbContext dbContext = new EntityAccessTestDbContext();
		Master master = new Master { Id = 1 };

		// Act + Assert
		Assert.AreEqual(EntityState.Detached, dbContext.GetEntityState(master));
	}

	[TestMethod]
	public void DbContext_GetEntityState_ReturnsUnchangedForAttachedEntity()
	{
		// Arrange
		EntityAccessTestDbContext dbContext = new EntityAccessTestDbContext();
		Master master = new Master { Id = 1 };
		dbContext.Attach(master);

		// Act + Assert
		Assert.AreEqual(EntityState.Unchanged, ((IDbContext)dbContext).GetEntityState(master));
	}

	[TestMethod]
	public void DbContext_GetEntityState_ReturnsAddedForAddedEntity()
	{
		// Arrange
		EntityAccessTestDbContext dbContext = new EntityAccessTestDbContext();
		Master master = new Master { Id = 1 };
		dbContext.Add(master);

		// Act + Assert
		Assert.AreEqual(EntityState.Added, ((IDbContext)dbContext).GetEntityState(master));
	}

	[TestMethod]
	public void DbContext_GetEntry_ReturnsEntryForEntity()
	{
		// Arrange
		IDbContext dbContext = new EntityAccessTestDbContext();
		Master master = new Master { Id = 1 };

		// Act
		EntityEntry entry = dbContext.GetEntry(master, suppressDetectChanges: true);

		// Assert
		Assert.AreSame(master, entry.Entity);
	}

	[TestMethod]
	public void DbContext_GetEntry_WithoutSuppressDetectChanges_ReturnsEntryForEntity()
	{
		// Arrange
		IDbContext dbContext = new EntityAccessTestDbContext();
		Master master = new Master { Id = 1 };

		// Act
		EntityEntry entry = dbContext.GetEntry(master, suppressDetectChanges: false);

		// Assert
		Assert.AreSame(master, entry.Entity);
	}

	[TestMethod]
	public void DbContext_Set_ReturnsSameInstanceForSameType()
	{
		// Arrange
		IDbContext dbContext = new EntityAccessTestDbContext();

		// Act
		IDbSet<Master> set1 = dbContext.Set<Master>();
		IDbSet<Master> set2 = dbContext.Set<Master>();

		// Assert
		// IDbSet je cachován per typ entity, opakované volání vrací tutéž instanci.
		Assert.AreSame(set1, set2);
	}

	[TestMethod]
	public void DbContext_Set_ReturnsDifferentInstanceForDifferentType()
	{
		// Arrange
		IDbContext dbContext = new EntityAccessTestDbContext();

		// Act
		IDbSet<Master> masterSet = dbContext.Set<Master>();
		IDbSet<Child> childSet = dbContext.Set<Child>();

		// Assert
		Assert.AreNotSame((object)masterSet, childSet);
	}

	public class EntityAccessTestDbContext : DbContext
	{
		private readonly string _databaseName;

		public EntityAccessTestDbContext([CallerMemberName] string databaseName = default)
		{
			_databaseName = databaseName;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseInMemoryDatabase(_databaseName);
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.Entity<Master>().HasMany(master => master.Children).WithOne(child => child.Master).HasForeignKey(child => child.MasterId);
			modelBuilder.Entity<Master>().HasMany(master => master.Tags).WithMany(tag => tag.Masters);
		}
	}

	public class Master
	{
		public int Id { get; set; }

		public List<Child> Children { get; set; }
		public List<MasterTag> Tags { get; set; }
	}

	public class Child
	{
		public int Id { get; set; }

		public int MasterId { get; set; }
		public Master Master { get; set; }
	}

	public class MasterTag
	{
		public int Id { get; set; }

		public List<Master> Masters { get; set; }
	}
}
