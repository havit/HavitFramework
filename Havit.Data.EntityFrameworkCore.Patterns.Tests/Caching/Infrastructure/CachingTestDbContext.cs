using System;
using System.Collections.Generic;
using System.Text;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToOne;
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
			modelBuilder.Entity<Master>().HasMany(master => master.ChildrenIncludingDeleted).WithOne(child => child.Parent);

			// M:N
			modelBuilder.Entity<Membership>().HasKey(membership => new { membership.LoginAccountId, membership.RoleId });

			// 1:1
			modelBuilder.Entity<ClassB>().HasOne(classB => classB.ClassA).WithOne(c => c.ClassB);
		}
	}
}
