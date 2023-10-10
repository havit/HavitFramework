using System;
using Havit.Data.EntityFrameworkCore;
using Havit.EFCoreTests.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Havit.EFCoreTests.Entity;

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

		modelBuilder.Entity<Person>().Ignore(p => p.Subordinates);
		modelBuilder.Entity<Person>().HasMany(p => p.SubordinatesIncludingDeleted).WithOne(p => p.Boss).HasForeignKey(p => p.BossId);

		modelBuilder.RegisterModelFromAssembly(typeof(Havit.EFCoreTests.Model.Person).Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
	}
}
