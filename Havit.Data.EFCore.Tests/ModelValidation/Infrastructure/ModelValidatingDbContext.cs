using System;
using Havit.Data.EFCore;
using Havit.Data.Entity.Tests.Validators.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using DbContext = Havit.EntityFrameworkCore.DbContext;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure
{
	internal class ModelValidatingDbContext : DbContext
	{
		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.RegisterModelFromAssembly(typeof(ModelValidatingDbContext).Assembly, typeof(OneCorrectKeyClass).Namespace);
			
			modelBuilder.Entity<MoreInvalidKeysClass>().HasKey(x => new { x.Id1, x.Id2 }); // složený primární klíč
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseInMemoryDatabase(typeof(ModelValidatingDbContext).FullName);
		}
	}
}
