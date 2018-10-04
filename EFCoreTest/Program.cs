using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore;
using System;

namespace EFCoreTest
{
	public static class Program
	{
		static void Main(string[] args)
		{

		}

		public class TestDbContext : Havit.Data.EntityFrameworkCore.DbContext
		{
			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				base.OnConfiguring(optionsBuilder);
				optionsBuilder.UseSqlServer(@"Data Source=(localdb)\mssqllocaldb;Initial Catalog=HFW");
			}

			protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
			{
				base.CustomizeModelCreating(modelBuilder);

				// Act
				modelBuilder.Entity<EntityWithoutSuppression>();
				modelBuilder.Entity<EntityWithSuppression>(eb =>
				{
					eb.HasConventionSuppressed<TestConvention>();
					eb.Property(p => p.Value).HasConventionSuppressed<TestConvention>();
				});
			}
		}

		public class EntityWithSuppression
		{
			public int Id { get; set; }
			public string Value { get; set; }
		}

		public class EntityWithoutSuppression
		{
			public int Id { get; set; }
			public string Value { get; set; }
		}

		public class TestConvention : IModelConvention
		{
			public void Apply(ModelBuilder modelBuilder)
			{
				// NOOP
			}
		}
	}
}
