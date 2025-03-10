using Havit.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Havit.EntityFrameworkCore.TestSolution.Entity;

public class ApplicationDbContext : Havit.Data.EntityFrameworkCore.DbContext
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ApplicationDbContext(DbContextOptions options) : base(options)
	{
		// NOOP
	}

	/// <inheritdoc />
	protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);

		modelBuilder.RegisterModelFromAssembly(typeof(Havit.Data.EntityFrameworkCore.TestSolution.Model.User).Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

	}
}
