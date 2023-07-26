using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.ManyToMany;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure;

public class ReferencingNavigationsTestDbContext : DbContext
{
	public ReferencingNavigationsTestDbContext()
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		optionsBuilder.UseInMemoryDatabase(nameof(ReferencingNavigationsTestDbContext));
	}

	protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);

		// Entity

		// OneToOne
		modelBuilder.Entity<Model.OneToOne.ClassOneToOneA>();
		modelBuilder.Entity<Model.OneToOne.ClassOneToOneB>().HasOne(classB => classB.ClassA).WithOne(c => c.ClassB);

		// OneToMany
		modelBuilder.Entity<Model.OneToMany.Master>();
		modelBuilder.Entity<Model.OneToMany.Child>();

		// ManyToMany as two OneToMany
		modelBuilder.Entity<Model.ManyToManyAsTwoOneToMany.LoginAccount>();
		modelBuilder.Entity<Model.ManyToManyAsTwoOneToMany.Role>();
		modelBuilder.Entity<Model.ManyToManyAsTwoOneToMany.Membership>().HasKey(membership => new { membership.LoginAccountId, membership.RoleId });

		// ManyToMany
		modelBuilder.Entity<ClassManyToManyA>().HasMany(classA => classA.Items).WithMany().UsingEntity("ClassManyToManyA_Items");
		modelBuilder.Entity<ClassManyToManyB>();
	}
}
