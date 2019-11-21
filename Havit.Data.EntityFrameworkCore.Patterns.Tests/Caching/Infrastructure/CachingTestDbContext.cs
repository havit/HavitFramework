using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure
{
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
			modelBuilder.Entity<LoginAccount>();
			modelBuilder.Entity<Role>();

			// 1:N
			modelBuilder.Entity<Master>().Ignore(master => master.Children);
			modelBuilder.Entity<Master>().HasMany(master => master.ChildrenWithDeleted).WithOne(child => child.Parent);

			// M:N
			modelBuilder.Entity<Membership>().HasKey(membership => new { membership.LoginAccountId, membership.RoleId });
		}
	}
}
