using Azure;
using Havit.Data.EntityFrameworkCore;
using Havit.EFCoreTests.Model;
using Microsoft.EntityFrameworkCore;

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

		modelBuilder.Entity<User>()
			.HasMany(user => user.PrimaryRoles)
			.WithMany()
			.UsingEntity("User_PrimaryRoles",
			l => l.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId"),
			r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId"));

		modelBuilder.Entity<User>()
			.HasMany(user => user.SecondaryRoles)
			.WithMany()
			.UsingEntity<UserRole>(joinEntityName: "User_SecondaryRoles");

		modelBuilder.Entity<User>()
			.HasMany(user => user.AdditionalRoles)
			.WithMany()
			.UsingEntity<UserRole>("User_AdditionalRoles",
				x => x.HasOne<Role>().WithMany().HasForeignKey(nameof(UserRole.RoleId)),
				x => x.HasOne<User>().WithMany(e => e.AdditionalUserRoles).HasForeignKey(nameof(UserRole.UserId)));

		modelBuilder.RegisterModelFromAssembly(typeof(Havit.EFCoreTests.Model.Person).Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
	}
}
