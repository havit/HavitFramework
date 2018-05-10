﻿using Microsoft.EntityFrameworkCore;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure
{
	public class TestDbContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseInMemoryDatabase(nameof(TestDbContext));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Model.AddEntityType(typeof(ItemWithDeleted));
			modelBuilder.Model.AddEntityType(typeof(ItemWithNullableProperty));
            modelBuilder.Model.AddEntityType(typeof(Language));
		}
	}
}