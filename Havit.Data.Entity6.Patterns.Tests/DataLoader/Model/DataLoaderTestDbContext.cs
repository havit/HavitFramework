﻿using System.Data.Entity;

namespace Havit.Data.Entity.Patterns.Tests.DataLoader.Model
{
	public class DataLoaderTestDbContext : DbContext
	{
		public DbSet<Master> Master { get; set; }
		public DbSet<Child> Child { get; set; }
		public DbSet<LoginAccount> LoginAccount { get; set; }
		public DbSet<Role> Role { get; set; }

		static DataLoaderTestDbContext()
		{
			Database.SetInitializer(new DropCreateDatabaseAlways<DataLoaderTestDbContext>());
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// 1:N
			modelBuilder.Entity<Master>().HasMany(master => master.Children).WithRequired(child => child.Parent);

			// M:N
			modelBuilder.Entity<LoginAccount>().HasMany(loginAccount => loginAccount.Roles).WithMany();
		}
	}
}
