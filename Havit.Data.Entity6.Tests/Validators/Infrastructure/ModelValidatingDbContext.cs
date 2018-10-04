using System;
using System.Data.Entity;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure
{
	internal class ModelValidatingDbContext : Havit.Data.Entity.DbContext
	{
		static ModelValidatingDbContext()
		{
			System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseAlways<ModelValidatingDbContext>());
		}

		public ModelValidatingDbContext() : base("Havit.Data.Entity6.Tests")
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Configurations.AddFromAssembly(this.GetType().Assembly);
			modelBuilder.RegisterModelFromAssembly(this.GetType().Assembly);
		}
	}
}
