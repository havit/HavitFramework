using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.Metadata.Conventions;

[TestClass]
public class CascadeDeleteToRestrictConventionTests
{
	/// <summary>
	/// Ověřuje, zda mají cizí klíče, které jsou v tabulkách, které jsou mapovány pomocí ToTable (klidně prázdné),
	/// nastaven DeleteBehavior.Restrict.
	/// </summary>
	[TestMethod]
	public void CascadeDeleteToRestrictConvention_SetsRestrictForForeignKeysInClassesMappedUsingToTable()
	{
		// Arrange
		var dbContext = new TestDbContext();

		// Act = conventions v TestDbContext

		// Assert
		Assert.AreEqual(DeleteBehavior.Restrict, dbContext.Model.FindEntityType(typeof(ModelClass)).GetForeignKeys().Single().DeleteBehavior);
	}
	public class TestDbContext : Havit.Data.EntityFrameworkCore.DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer();
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			// Testovací scénář: Při použití ToTable zůstanou cizí klíče takové tabulky nastaveny s chováním DeleteBehavior.Cascade, což chceme opravit na Restrict.
			modelBuilder.Entity<ModelClass>().ToTable(t => { /* NOOP */ });
		}
	}

	public class ModelClass
	{
		public int Id { get; set; }

		public ModelClass LinkedModelClass { get; set; }
		public int? LinkedModelClassId { get; set; }
	}
}
