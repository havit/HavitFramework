using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests
{
	internal class EndToEndTestDbContext : TestDbContext
	{
		private readonly Action<ModelBuilder> onModelCreating;

		public EndToEndTestDbContext(Action<ModelBuilder> onModelCreating = default)
		{
			this.onModelCreating = onModelCreating;
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);
			onModelCreating?.Invoke(modelBuilder);
		}

		public IReadOnlyList<MigrationOperation> Diff(DbContext target)
		{
			var differ = this.GetService<IMigrationsModelDiffer>();
			return differ.GetDifferences(Model, target.Model);
		}

		public IReadOnlyList<MigrationCommand> Migrate(DbContext target)
		{
			var diff = Diff(target);
			var generator = this.GetService<IMigrationsSqlGenerator>();
			return generator.Generate(diff, this.Model);
		}
	}
}