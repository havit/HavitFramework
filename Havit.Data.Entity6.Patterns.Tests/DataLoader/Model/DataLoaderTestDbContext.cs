using System.Data.Entity;
using Havit.Data.Entity.Helpers;
using Havit.Data.Entity.Tests;

namespace Havit.Data.Entity.Patterns.Tests.DataLoader.Model
{
	public class DataLoaderTestDbContext : DbContext
	{
		public DbSet<Master> Master { get; set; }
		public DbSet<Child> Child { get; set; }
		public DbSet<LoginAccount> LoginAccount { get; set; }
		public DbSet<Role> Role { get; set; }
		public DbSet<HiearchyItem> HiearchyItem { get; set; }

		static DataLoaderTestDbContext()
		{
			Database.SetInitializer(new DropCreateDatabaseAlways<DataLoaderTestDbContext>());
		}

		public DataLoaderTestDbContext() : base(DatabaseNameHelper.GetDatabaseNameForUnitTest("Havit.Data.Entity6.Patterns.Tests"))
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// 1:N
			modelBuilder.Entity<Master>().HasMany(master => master.Children).WithOptional(child => child.Parent);

			// M:N
			modelBuilder.Entity<LoginAccount>().HasMany(loginAccount => loginAccount.Roles).WithMany();

			// Hierarchy
			modelBuilder.Entity<HiearchyItem>().HasMany(parent => parent.Children).WithOptional(child => child.Parent);
		}
	}
}
