using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.ManyToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.ManyToManyAsTwoOneToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToOne;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;

public class CachingTestDbContext : DbContext
{
	public CachingTestDbContext()
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		optionsBuilder.UseInMemoryDatabase(nameof(CachingTestDbContext));
	}

	protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);

		// Entity

		// OneToOne
		modelBuilder.Entity<ClassOneToOneA>();
		modelBuilder.Entity<ClassOneToOneB>().HasOne(classB => classB.ClassA).WithOne(c => c.ClassB);
		modelBuilder.Entity<ClassOneToOneC>().HasOne(classC => classC.Direct).WithOne(c => c.Indirect);

		// OneToMany
		modelBuilder.Entity<Master>();
		modelBuilder.Entity<Child>();
		modelBuilder.Entity<Category>().HasMany(c => c.Children).WithOne(c => c.Parent).HasForeignKey(c => c.ParentId);

		// ManyToMany as two OneToMany
		modelBuilder.Entity<LoginAccount>();
		modelBuilder.Entity<Role>();
		modelBuilder.Entity<Membership>().HasKey(membership => new { membership.LoginAccountId, membership.RoleId });

		// ManyToMany
		modelBuilder.Entity<ClassManyToManyA>().HasMany(classA => classA.Items).WithMany().UsingEntity("ClassManyToManyA_Items");
		modelBuilder.Entity<ClassManyToManyB>();
	}
}
