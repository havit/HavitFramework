using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure
{
	internal class ModelValidatingDbContext : EntityFrameworkCore.DbContext
	{
		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.RegisterModelFromAssembly(typeof(ModelValidatingDbContext).Assembly, typeof(OneCorrectKeyClass).Namespace);
			
			modelBuilder.Entity<MoreInvalidKeysClass>().HasKey(x => new { x.Id1, x.Id2 }); // složený primární klíč
			modelBuilder.Entity<UserRoleMembership>().HasKey(x => new { x.UserId, x.RoleId }); // složený primární klíč
			modelBuilder.Entity<ForeignKeyWithNoNavigationPropertyMasterClass>().HasMany(m => m.Children).WithOne().HasForeignKey(c => c.MasterId);

			modelBuilder.Entity<GroupToGroup>().HasKey(groupHierarchy => new { groupHierarchy.ChildGroupId, groupHierarchy.ParentGroupId });

			modelBuilder.Entity<GroupToGroup>().HasOne(groupHierarchy => groupHierarchy.ChildGroup)
				.WithMany(group => group.Parents)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<GroupToGroup>().HasOne(groupHierarchy => groupHierarchy.ParentGroup)
				.WithMany(group => group.Children)
				.OnDelete(DeleteBehavior.Restrict);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseInMemoryDatabase(typeof(ModelValidatingDbContext).FullName);
		}
	}
}
