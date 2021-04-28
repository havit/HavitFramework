using System;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.Metadata.Conventions
{
	[TestClass]
	public class ForeignKeysIndexConventionTests
	{
		private class TestEntity
		{
			public int Id { get; set; }

			public string StringProperty { get; set; }
		}

		/// <summary>
		/// Bug #55136
		/// </summary>
		[TestMethod]
		public void ForeignKeysIndexConvention_PropertyAnnotationNullValue_InitializingModel_DoesNotThrowException()
		{
			var dbContext = new TestDbContext(builder =>
			{
				builder.Entity<TestEntity>().Property(x => x.StringProperty).HasDefaultValue(null);
			});

			_ = dbContext.Model;
		}

		/// <summary>
		/// Bug #55136
		/// </summary>
		[TestMethod]
		public void ForeignKeysIndexConvention_EntityAnnotationNullValue_InitializingModel_DoesNotThrowException()
		{
			var dbContext = new TestDbContext(builder =>
			{
				builder.Entity<TestEntity>()
					.HasComment("Abc")
					.HasComment(null);
			});

			_ = dbContext.Model;
		}

		// TODO: refactor, consolidate test DbContexts again
		private class TestDbContext : BusinessLayerDbContext
		{
			private readonly Action<ModelBuilder> onModelCreating;

			public TestDbContext(Action<ModelBuilder> onModelCreating = default)
			{
				this.onModelCreating = onModelCreating;

				Settings.UseDefaultValueAttributeConvention = true;
				Settings.UseForeignKeysIndexConvention = true;
			}

			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				base.OnConfiguring(optionsBuilder);

				optionsBuilder.UseSqlServer("Database=Dummy");
				optionsBuilder.EnableServiceProviderCaching(false);
			}

			protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
			{
				base.CustomizeModelCreating(modelBuilder);
				onModelCreating?.Invoke(modelBuilder);
			}
		}
	}
}