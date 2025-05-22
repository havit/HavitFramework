﻿using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure;

internal class ModelValidatingDbContext : EntityFrameworkCore.DbContext
{
	protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);

		modelBuilder.RegisterModelFromAssembly(typeof(ModelValidatingDbContext).Assembly, typeof(OneCorrectKeyClass).Namespace);

		modelBuilder.Entity<MoreInvalidKeysClass>().HasKey(x => new { x.Id1, x.Id2 }); // složený primární klíč
		modelBuilder.Entity<UserRoleMembership>().HasKey(x => new { x.UserId, x.RoleId }); // složený primární klíč
		modelBuilder.Entity<ForeignKeyWithNoNavigationPropertyMasterClass>().HasMany(m => m.Children).WithOne().HasForeignKey(c => c.MasterId);

		modelBuilder.Entity<GroupToGroup>().HasOne(groupHierarchy => groupHierarchy.ChildGroup)
			.WithMany(group => group.Parents)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<GroupToGroup>().HasOne(groupHierarchy => groupHierarchy.ParentGroup)
			.WithMany(group => group.Children)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<GroupToGroup>().HasKey(groupHierarchy => new { groupHierarchy.ChildGroupId, groupHierarchy.ParentGroupId });

		modelBuilder.Entity<IdWithNoForeignKeyButAllowed>().Property(entity => entity.MyId).SuppressModelValidatorRule(ModelValidatorRule.OnlyForeignKeyPropertiesCanEndWithId);

		modelBuilder.Entity<IdWithPoorlyNamedForeignKey>().HasOne(item => item.ForeignKey).WithMany().HasForeignKey(item => item.ForeignKeyCode);

		modelBuilder.Entity<TrueManyToManyA>().HasMany(item => item.Items).WithMany().UsingEntity("TrueManyToManyA_Items");

		modelBuilder.Entity<KeylessClass>().HasNoKey(); // bez primárního klíče

		modelBuilder.HasSequence<int>("EntrySequence");
		modelBuilder.Entity<EntryWithSequencePrimaryKeyAndNoSymbol>().Property(e => e.Id).HasDefaultValueSql("NEXT VALUE FOR EntrySequence");

		modelBuilder.Entity<WithComputedColumns>().Property(e => e.Computed).HasComputedColumnSql("Id");
		modelBuilder.Entity<WithComputedColumns>().Property(e => e.ComputedStored).HasComputedColumnSql("Id", stored: true);
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		// DbContext slouží jen k validaci modelu, neřeší práci s daty, nepotřebujeme jej tedy mezi testy více izolovat.
		optionsBuilder.UseInMemoryDatabase(typeof(ModelValidatingDbContext).FullName);
	}
}
