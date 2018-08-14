using Havit.Data.Entity.Tests.ModelValidation.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using DbContext = Havit.Data.Entity.DbContext;

namespace Havit.Data.Entity.Tests.ModelValidation.Infrastructure
{
	internal class ModelValidatingDbContext : DbContext
	{
		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.RegisterModelFromAssembly(typeof(ModelValidatingDbContext).Assembly, typeof(OneCorrectKeyClass).Namespace);
			
			modelBuilder.Entity<MoreInvalidKeysClass>().HasKey(x => new { x.Id1, x.Id2 }); // složený primární klíč
			modelBuilder.Entity<UserRoleMembership>().HasKey(x => new { x.UserId, x.RoleId }); // složený primární klíč
			modelBuilder.Entity<ForeignKeyWithNoNavigationPropertyMasterClass>().HasMany(m => m.Children).WithOne().HasForeignKey(c => c.MasterId);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseInMemoryDatabase(typeof(ModelValidatingDbContext).FullName);
		}
	}
}
