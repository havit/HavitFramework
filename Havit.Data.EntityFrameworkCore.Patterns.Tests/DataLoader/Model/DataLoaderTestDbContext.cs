using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model
{
	public class DataLoaderTestDbContext : Havit.Data.EntityFrameworkCore.DbContext
	{
		public DbSet<Master> Master { get; set; }
		public DbSet<Child> Child { get; set; }

		public DbSet<LoginAccount> LoginAccount { get; set; }
		public DbSet<Role> Role { get; set; }
		public DbSet<Membership> Membership { get; set; }

		public DbSet<HiearchyItem> HiearchyItem { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseInMemoryDatabase(nameof(DataLoaderTestDbContext));
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			// 1:N
			modelBuilder.Entity<Master>().Ignore(master => master.Children);
			modelBuilder.Entity<Master>().HasMany(master => master.ChildrenWithDeleted).WithOne(child => child.Parent);

			// M:N
			modelBuilder.Entity<Membership>().HasKey(membership => new { membership.LoginAccountId, membership.RoleId });

			// Hierarchy
			modelBuilder.Entity<HiearchyItem>().HasMany(parent => parent.Children).WithOne(child => child.Parent);
		}
	}
}
